using System;

namespace TwitterClient
{
    class Program
    {
         static void Main(string[] args)
        {
            var client = new Client();

            var token =client.GetBearerToken().GetAwaiter().GetResult();

            var tweets = client.GetUserTimelineJson(token, "@twitterapi", 1000, true, false).GetAwaiter().GetResult();
        }
    }
}
