using Amazon.Lambda.Core;
using Ical.Net;
using Ical.Net.CalendarComponents;
using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace iCalFeed
{
    /// <summary>
    /// The request object for the Lambda handler
    /// </summary>
    public class ICalFeedRequest
    {
        public string Url { get; set; }
        public string Timezone { get; set; }
        public string Date { get; set; }
    }

    /// <summary>
    /// An iCal event
    /// </summary>
    [DataContract]
    public class iCalEvent
    {
        [DataMember(Name ="uid")]
        public string UID { get; set; }

        [DataMember(Name = "summary")]
        public string Summary { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "starttime")]
        public DateTime StartTime { get; set; }

        [DataMember(Name = "endtime")]
        public DateTime EndTime { get; set; }
    }

    public class Function
    {    
        /// <summary>
        /// A simple function that takes a cal feed request and returns events that happen
        /// on the date specified in the calendar specified
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<iCalEvent> FunctionHandler(ICalFeedRequest input, ILambdaContext context)
        {
            //  Our return value:
            List<iCalEvent> response = new List<iCalEvent>();

            //  --- Sanity check our inputs ...
            //  If no date specified, assume today
            if (string.IsNullOrWhiteSpace(input.Date))
            {
                input.Date = DateTime.Now.ToShortDateString();
            }
            
            //  Download the calendar:
            string calData = string.Empty;
            using (var client = new WebClient())
            {
                calData = client.DownloadString(input.Url);
            }

            //  Load the calendar data
            var calendar = Calendar.Load(calData);

            //  Get all events that happen in this window
            var startDate = DateTime.Parse(input.Date).ToUniversalTime();
            var endDate = startDate.AddDays(1);
            var occurrances = calendar.GetOccurrences(startDate, endDate);
            
            //  Convert to our response type
            foreach (var item in occurrances)
            {                                
                response.Add(new iCalEvent
                {
                    Summary = ((CalendarEvent)item.Source).Summary,
                    Description = ((CalendarEvent)item.Source).Description,
                    UID = ((CalendarEvent)item.Source).Uid,
                    /* If we have a timezone, use it (as long as we actually have a time part) -- otherwise default to UTC */
                    StartTime = !string.IsNullOrWhiteSpace(input.Timezone) && item.Period.StartTime.HasTime ? item.Period.StartTime.ToTimeZone(input.Timezone).Value : item.Period.StartTime.Value,
                    EndTime = !string.IsNullOrWhiteSpace(input.Timezone) && item.Period.EndTime.HasTime ? item.Period.EndTime.ToTimeZone(input.Timezone).Value : item.Period.EndTime.Value,
                });
            }

            return response;
        }
    }
}
