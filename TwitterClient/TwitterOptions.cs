using System;
using System.Collections.Generic;
using System.Text;

namespace TwitterClient
{
    /// <summary>
    /// twitter authentication options
    /// </summary>
    public class TwitterOptions
    {
       
        /// <summary>
        /// twitter app consumer key
        /// </summary>
        public string ConsumerKey { get; set; }

        /// <summary>
        /// twitter app consumer secret
        /// </summary>
        public string ConsumerSecret { get; set; }

        /// <summary>
        /// twitter access token
        /// </summary>
        public string AccessToken { get; set; } 

        /// <summary>
        /// twitter access token secret
        /// </summary>
        public string AccessTokenSecret { get; set; } 
    }
}
