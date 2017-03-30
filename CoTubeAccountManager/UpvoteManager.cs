// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpvoteManager.cs" company="GMT">
//   Created by Fagenorn
// </copyright>
// <summary>
//   Defines the UpvoteManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoTubeAccountManager
{
    using System;
    using System.Net;
    using System.Text;

    /// <summary>
    ///     The up-vote manager.
    /// </summary>
    public static class UpvoteManager
    {
        /// <summary>
        ///     The cookies.
        /// </summary>
        private static readonly CookieContainer Cookies = new CookieContainer();

        /// <summary>
        ///     Gets or sets a value indicating whether cookies have been set for up-vote service.
        /// </summary>
        public static bool IsLoggedIn { get; set; }

        /// <summary>
        ///     The comment site service.
        /// </summary>
        private static string CommentSite => "http://www.youtubecommenter.com/panel/";

        /// <summary>
        ///     The user agent. Can't be static, since site doesn't perform any special checks.
        /// </summary>
        private static string UserAgent
            =>
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.110 Safari/537.36"
        ;

        /// <summary>
        ///     Login into panel.
        /// </summary>
        /// <param name="username">
        ///     The username.
        /// </param>
        /// <param name="password">
        ///     The password.
        /// </param>
        public static void Login(string username, string password)
        {
            var body =
                $"username={WebUtility.UrlEncode(username)}&password={WebUtility.UrlEncode(password)}&submit=login";

            LoginRequest();
            LoginRequest(body, true);

            IsLoggedIn = true;

            // Need to perform check to see if actually logged in, but assuming atm that provided Username and Password is correct.
        }

        /// <summary>
        ///     The submit an up-vote request to external service.
        /// </summary>
        /// <param name="url">
        ///     The url.
        /// </param>
        /// <param name="amount">
        ///     The amount.
        /// </param>
        /// <exception cref="ArgumentException">
        ///     Amount has to be between 15 and 500
        /// </exception>
        public static void SubmitUpvoteRequest(string url, int amount)
        {
            if (amount < 15 || amount > 500)
            {
                throw new ArgumentException("Amount has to be between 15 and 500.");
            }

            if (!IsLoggedIn)
            {
                throw new ArgumentException("Need to login into panel first.");
            }

            var body = $"url={WebUtility.UrlEncode(url)}&value={amount}";

            var request = (HttpWebRequest)WebRequest.Create($"{CommentSite}index.php");

            request.UserAgent = UserAgent;
            request.CookieContainer = Cookies;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.8,nl;q=0.6");
            request.Headers.Add("DNT", "1");

            request.Method = "POST";

            // Post body
            var bytes = Encoding.ASCII.GetBytes(body);
            request.ContentLength = bytes.Length;
            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(bytes, 0, bytes.Length);
            }

            // Receive Response
            using ((HttpWebResponse)request.GetResponse())
            {
            }
        }

        /// <summary>
        ///     The login request.
        /// </summary>
        /// <param name="body">
        ///     The body.
        /// </param>
        /// <param name="post">
        ///     The post.
        /// </param>
        private static void LoginRequest(string body = "", bool post = false)
        {
            var request = (HttpWebRequest)WebRequest.Create($"{CommentSite}login.php");
            request.UserAgent = UserAgent;
            request.CookieContainer = Cookies;
            request.ContentType = "application/x-www-form-urlencoded";
            request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            request.Headers.Add("Accept-Encoding", "gzip, deflate");
            request.Headers.Add("Accept-Language", "en-US,en;q=0.8,nl;q=0.6");
            request.Headers.Add("DNT", "1");

            if (post)
            {
                request.Method = "POST";

                // Post body
                var bytes = Encoding.ASCII.GetBytes(body);
                request.ContentLength = bytes.Length;
                using (var requestStream = request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
            }

            // Receive Response
            using ((HttpWebResponse)request.GetResponse())
            {
            }
        }
    }
}