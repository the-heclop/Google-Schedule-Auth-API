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
using Microsoft.AspNetCore.Authorization;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace PersonalAPI.Controllers
{
    [Route("/")]
    [ApiController]
    public class GoogleEventController : ControllerBase
    {
                
        [HttpGet("available-hour-blocks")]
        [Authorize]
        public async Task<IActionResult> GetAvailableHourBlocks()
        {
            string json = System.IO.File.ReadAllText("credentials.json");
            string tokens = System.IO.File.ReadAllText("tokens.json");

            dynamic tokenObj = Newtonsoft.Json.JsonConvert.DeserializeObject(tokens);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            string clientId = jsonObj.client_id;
            string clientSecret = jsonObj.client_secret;
            string accessToken = tokenObj.access_token;
            string refreshToken = tokenObj.refresh_token;

            var clientSecrets = new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            };

            var token = new TokenResponse { AccessToken = accessToken, RefreshToken = refreshToken };
            var credentials = new UserCredential(new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = clientSecrets
                }),
                "user",
                token);

            var calendarService = new CalendarService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credentials,
                ApplicationName = "Google Calendar API Sample",
            });

            DateTime start = WholeHourModel.RoundUpToNextWholeHour(DateTime.Now);
            DateTime end = start.AddDays(7); // Adjust as needed for the range you want to check
            int blockSizeInMinutes = 60;

            var freeBusyRequest = new FreeBusyRequest
            {
                TimeMin = start,
                TimeMax = end,
                TimeZone = "EST", // Set the time zone as needed
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
                        availableBlocks.Add(new { Start = current });
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
            string json = System.IO.File.ReadAllText("credentials.json");
            string tokens = System.IO.File.ReadAllText("tokens.json");

            dynamic tokenObj = Newtonsoft.Json.JsonConvert.DeserializeObject(tokens);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(json);

            string clientId = jsonObj.client_id;
            string clientSecret = jsonObj.client_secret;
            string accessToken = tokenObj.access_token;
            string refreshToken = tokenObj.refresh_token;

            var clientSecrets = new ClientSecrets
            {
                ClientId = clientId,
                ClientSecret = clientSecret
            };

            var token = new TokenResponse { AccessToken = accessToken, RefreshToken = refreshToken };
            var credentials = new UserCredential(new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = clientSecrets
                }),
                "user",
                token);



            var service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credentials,
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
                    DateTime = eventRequest.Start.AddMinutes(60),
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
