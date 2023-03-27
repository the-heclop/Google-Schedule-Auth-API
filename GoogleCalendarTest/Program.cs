using Google.Apis.Auth.OAuth2; // for authenticating with the Google Calendar API
using Google.Apis.Calendar.v3; // for accessing the Google Calendar API
using Google.Apis.Calendar.v3.Data; // for working with Calendar API data objects
using Google.Apis.Services; // for creating a CalendarService instance
using Google.Apis.Util;
using Google.Apis.Util.Store; // for working with the FileDataStore
using System; // for DateTime
using System.Globalization;
using System.IO; // for FileStream
using System.Linq.Expressions;
using System.Threading; // for CancellationToken


    UserCredential credential;
    using (var stream = new FileStream("C:\\Users\\bich\\OneDrive\\Desktop\\C#-projects\\PersonalAPIApp\\GoogleCalendarTest\\credentials.json", FileMode.Open, FileAccess.Read))
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

        //DateTime start = DateTime.Now;
        //DateTime end = start.AddDays(7); // Adjust as needed for the range you want to check
        //int blockSizeInMinutes = 60;

        //var freeBusyRequest = new FreeBusyRequest
        //{
        //    TimeMin = start,
        //    TimeMax = end,
        //    TimeZone = "UTC", // Set the time zone as needed
        //    Items = new List<FreeBusyRequestItem> { new FreeBusyRequestItem { Id = "416d4b4746f9b653c84ff795bf940762d9e295cf7d18f63afddebd899bb72fa7@group.calendar.google.com" } },
        //};

        //FreebusyResource.QueryRequest query = calendarService.Freebusy.Query(freeBusyRequest);
        //FreeBusyResponse response = await query.ExecuteAsync();

        
        
















