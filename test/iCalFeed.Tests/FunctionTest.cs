
using Amazon.Lambda.TestUtilities;
using Amazon.XRay.Recorder.Core;
using Xunit;

namespace iCalFeed.Tests
{
    public class FunctionTest
    {
        private string icalurl = "https://calendar.google.com/calendar/ical/mg8l31ag8ua059trmktgdq6v80%40group.calendar.google.com/private-342fffdc823bfcaea433775659169545/basic.ics";

        public FunctionTest() => AWSXRayRecorder.Instance.BeginSegment("unit-test");

        [Fact]
        public void FunctionHandler_ValidParams_ReturnsEventList()
        {
            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            var request = new ICalFeedRequest
            {
                Url = icalurl,
                Date = "2019-02-22 00:00:00 -05:00",
                Timezone = "America/New_York",
            };
            var response = function.FunctionHandler(request, context);
            
        }

        [Fact]
        public void FunctionHandler_NoDate_ReturnsTodaysEventList()
        {
            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            var request = new ICalFeedRequest
            {
                Url = icalurl,
                Timezone = "America/New_York",
            };
            var response = function.FunctionHandler(request, context);

        }

        [Fact]
        public void FunctionHandler_NoTimezone_ReturnsTodaysEventListInUTC()
        {
            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            var request = new ICalFeedRequest
            {
                Url = icalurl,
            };
            var response = function.FunctionHandler(request, context);

        }
    }
}
