using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TwitterClient
{
    public class Client
    {
        private const string ApiBaseUrl = "https://api.twitter.com";
        private static readonly HttpClient client = new HttpClient();
        private static TwitterOptions options = new TwitterOptions();

        #region Authentication

        /// <summary>
        /// Gets bearer token for application-only authentication from Twitter API 1.1, obtaining key and secret from web.config/app.config.
        /// * NOTE: This token should be cached by the application -- for up to 15 mins.
        /// * Dependant on web.config appSettings params twitterConsumerKey and twitterConsumerSecret.
        /// * Twitter API client oAuth settings: https://dev.twitter.com/app
        /// * Application-only authentication: https://dev.twitter.com/docs/auth/application-only-auth
        /// * API endpoint oauth2/token: https://dev.twitter.com/docs/api/1.1/post/oauth2/token
        /// </summary>
        /// <returns>oAuth bearer token for Twitter API authentication.</returns>
        public async Task<string> GetBearerToken()
        {
            JToken token = JObject.Parse(
                await GetBearerTokenJson(
                   options.ConsumerKey,options.ConsumerSecret));

            return token.SelectToken("access_token").Value<string>();
        }

        /// <summary>
        /// Gets bearer token for application-only authentication from Twitter API 1.1.
        /// * NOTE: This token should be cached by the application -- for up to 15 mins.
        /// * Twitter API client oAuth settings: https://dev.twitter.com/app
        /// * Application-only authentication: https://dev.twitter.com/docs/auth/application-only-auth
        /// * API endpoint oauth2/token: https://dev.twitter.com/docs/api/1.1/post/oauth2/token
        /// </summary>
        /// <param name="consumerKey">Token obtained from authentication.</param>
        /// <param name="consumerSecret">Token obtained from authentication.</param>
        /// <returns>oAuth bearer token for Twitter API authentication.</returns>
        public async Task<string> GetBearerToken(string consumerKey, string consumerSecret)
        {
            JToken token = JObject.Parse(
                await GetBearerTokenJson(consumerKey, consumerSecret));

            return token.SelectToken("access_token").Value<string>();
        }

        private async static Task<string> GetBearerTokenJson(string consumerKey, string consumerSecret)
        {
            var webrequest = CreateRequest("/oauth2/token",HttpMethod.Get);

            webrequest.Headers.Add("Authorization", "Basic " + GetBasicAuthToken(consumerKey, consumerSecret));

            WriteRequest(webrequest, "grant_type=client_credentials");

            return await ReadResponse(webrequest);
        }

        #endregion

        #region UserTimeline

        /// <summary>
        /// Gets a user timeline.
        /// * API endpoint user_timeline: https://dev.twitter.com/docs/api/1.1/get/statuses/user_timeline
        /// </summary>
        /// <param name="bearerToken">Token obtained from authentication.</param>
        /// <param name="screenName">username of user whose timeline will be returned.</param>
        /// <param name="count">Number of tweets to return</param>
        /// <param name="excludeReplies">Whether to exclude replies. False is reccomended because API removed replies after obtaining requested number of tweets, leading to unpredictable result counts.</param>
        /// <param name="includeRTs">Whether to include retweets. True is reccomended because API removes retweets after obtaining requested number of tweets, leading to unpredictable result counts.(</param>
        /// <returns>Raw JSON response from API.</returns>
        public async Task<string> GetUserTimelineJson(string bearerToken, string screenName, int count = 10, bool excludeReplies = false, bool includeRTs = true)
        {
            var webrequest = CreateRequest(
                "/1.1/statuses/user_timeline.json"
                + "?screen_name=" + screenName + "&count=" + count + "&exclude_replies=" + excludeReplies.ToString().ToLower() + "&include_rts=" + includeRTs.ToString().ToLower(),HttpMethod.Get);

            webrequest.Headers.Add("Authorization", "Bearer " + bearerToken);

            return await ReadResponse(webrequest);
        }

        #endregion

        #region Search

        /// <summary>
        /// Gets a user timeline.
        /// * API endpoint search: https://dev.twitter.com/docs/api/1.1/get/search/tweets
        /// </summary>
        /// <param name="bearerToken">Token obtained from authentication.</param>
        /// <param name="parameters">Search parameters in raw query string format, e.g. "q=from:BritishVogue, e.g. "q=#FNO".</param>
        /// <returns>Raw JSON response from API.</returns>
        public async Task<string> GetSearchJson(string bearerToken, string parameters)
        {
            var webrequest = CreateRequest(("/1.1/search/tweets.json" + parameters),HttpMethod.Get);

            webrequest.Headers.Add("Authorization", "Bearer " + bearerToken);


            return await ReadResponse(webrequest);
        }

        #endregion

        #region Helper methods

        private static HttpRequestMessage CreateRequest(string url,HttpMethod method)
        {
            var uri = new Uri(ApiBaseUrl + url);
            var webrequest = new HttpRequestMessage(method,uri);          
            return webrequest;
        }

        private static string GetBasicAuthToken(string consumerKey, string consumerSecret)
        {
            return Base64Encode(consumerKey + ":" + consumerSecret);
        }

        private static void WriteRequest(HttpRequestMessage webrequest, string postData)
        {
            webrequest.Method = HttpMethod.Post;
            webrequest.Content = new StringContent(
                  postData, 
                  Encoding.UTF8, 
                  "application/x-www-form-urlencoded"
             );           
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private async static Task<string> ReadResponse(HttpRequestMessage request)
        {
            var response =await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();

            }

            return null;
        }

        /// <summary>
        /// encode string to base 64
        /// </summary>
        /// <param name="s">string to encode</param>
        /// <returns></returns>
        private static string Base64Encode(string s)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(s);
            return Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Decode base 64 string
        /// </summary>
        /// <param name="s">string to decode</param>
        /// <returns></returns>
        private static string Base64Decode(string s)
        {
            byte[] bytes = Convert.FromBase64String(s);
            return Encoding.ASCII.GetString(bytes);
        }

        #endregion
    }
}

