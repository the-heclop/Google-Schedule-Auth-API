using Google.Apis.Calendar.v3.Data;
using Google.Apis.Calendar.v3;
using Microsoft.AspNetCore.Mvc;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System.Runtime.CompilerServices;
using PersonalAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Net;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PersonalAPI.Controllers
{
    [Route("/")]
    [ApiController]
    public class GoogleEventController : ControllerBase
    {

        // GET api/<GoogleEventController>/5
        [HttpGet("available-hour-blocks")]
        public async Task<IActionResult> GetAvailableHourBlocks()
        {
            UserCredential credential;
            using (var stream = new FileStream(".\\credentials.json", FileMode.Open, FileAccess.Read))
            {
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    new[] { CalendarService.Scope.Calendar },
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
            }

            var calendarService = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Google Calendar API Sample",
            });

            DateTime start = WholeHourModel.RoundUpToNextWholeHour(DateTime.Now);
            DateTime end = start.AddDays(3); // Adjust as needed for the range you want to check
            int blockSizeInMinutes = 60;

            var freeBusyRequest = new FreeBusyRequest
            {
                TimeMin = start,
                TimeMax = end,
                TimeZone = "UTC", // Set the time zone as needed
                Items = new List<FreeBusyRequestItem> { new FreeBusyRequestItem { Id = "primary" } },
            };

            FreebusyResource.QueryRequest query = calendarService.Freebusy.Query(freeBusyRequest);
            FreeBusyResponse response = await query.ExecuteAsync();

            var availableBlocks = new List<object>();

            foreach (KeyValuePair<string, FreeBusyCalendar> calendar in response.Calendars)
            {

                IList<TimePeriod> busyTimes = calendar.Value.Busy;
                DateTime current = start;

                while (current < end)
                {
                    DateTime next = current.AddMinutes(blockSizeInMinutes);
                    TimePeriod conflict = null;

                    foreach (TimePeriod busy in busyTimes)
                    {
                        DateTime busyStart = Convert.ToDateTime(busy.Start);
                        DateTime busyEnd = Convert.ToDateTime(busy.End);

                        if ((current >= busyStart && current < busyEnd) || (next > busyStart && next <= busyEnd))
                        {
                            conflict = busy;
                            break;
                        }
                    }

                    if (conflict == null)
                    {
                        availableBlocks.Add(new { Start = current, End = next });
                        current = next;
                    }
                    else
                    {
                        current = Convert.ToDateTime(conflict.End);
                    }


                }
            }
            return Ok(availableBlocks);
        }

        // POST api/<GoogleEventController>

        [HttpPost("schedule")]
        public async Task<IActionResult> Scheduler([FromBody] EventRequestModel eventRequest )
        {
            // Initialize the Google Calendar API client library
            string credPath = ".\\credentials.json"; // path to your JSON file containing API credentials
            UserCredential credential;
            using (var stream = new FileStream(credPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.FromStream(stream).Secrets,
                    new[] { CalendarService.Scope.Calendar },
                    "user",
                    CancellationToken.None,
                    new FileDataStore("Calendar.Auth.Store")).Result;
            }
            // The above code uses the GoogleWebAuthorizationBroker to authenticate the user with the Calendar API using the credentials specified in the JSON file.
            // The credentials are loaded from the JSON file using GoogleClientSecrets.Load().
            // The user is authorized to access the Calendar API by requesting the Calendar scope.
            // The authorization result is stored in a FileDataStore so that the user does not have to be re-authenticated on subsequent requests.

            // Create a new CalendarService instance using the authorized credentials
            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Consult Software Guy"
            });
            // The above code creates a new CalendarService instance using the authorized credentials.
            // The application name specified in the initializer is used to identify your application in the Google API Console.

            var newEvent = new Event()
            {
                Summary = eventRequest.Summary,
                Location = eventRequest.Location,
                Start = new EventDateTime()
                {
                    DateTime = eventRequest.Start,                    
                },
                End = new EventDateTime()
                {
                    DateTime = eventRequest.End,
                },
                Description = eventRequest.Description,
            };



            // Check for time conflicts
            EventsResource.ListRequest request = service.Events.List("primary");
            request.TimeMin = newEvent.Start.DateTime;
            request.TimeMax = newEvent.End.DateTime;
            request.ShowDeleted = false;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            

            // Insert the new event
            EventsResource.InsertRequest insertRequest = service.Events.Insert(newEvent, "primary");
            var createdEvent = await service.Events.Insert(newEvent, "primary").ExecuteAsync();

            if (createdEvent != null)
            {
                return CreatedAtAction(nameof(Scheduler), eventRequest);
            }
            else
            {
                return BadRequest("Error creating event.");
            }

        }        
    }    
}
