﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Http.cs" company="GMT">
//   Created by Fagenorn
// </copyright>
// <summary>
//   Defines the Http type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YoutubeLibrary
{
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.Net;
    using System.Text;

    /// <summary>
    ///     The request method.
    /// </summary>
    public enum Method
    {
        /// <summary>
        ///     Get request.
        /// </summary>
        Get,

        /// <summary>
        ///     Post request.
        /// </summary>
        Post
    }

    /// <summary>
    ///     The http object.
    /// </summary>
    internal class Http
    {
        /// <summary>
        ///     The extra headers.
        /// </summary>
        private readonly NameValueCollection extraHeaders = new NameValueCollection();

        /// <summary>
        ///     The request.
        /// </summary>
        private HttpWebRequest request;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Http" /> class.
        /// </summary>
        /// <param name="proxy">
        ///     The proxy.
        /// </param>
        public Http(Proxy proxy)
        {
            this.Proxy = proxy;
        }

        /// <summary>
        ///     Gets the cookies.
        /// </summary>
        private CookieContainer Cookies { get; } = new CookieContainer();

        /// <summary>
        ///     Gets the proxy.
        /// </summary>
        private Proxy Proxy { get; }

        /// <summary>
        ///     Gets the user agent.
        /// </summary>
        private string Useragent { get; } = UserAgent.GenerateUseragent();

        /// <summary>
        ///     The read chunked response.
        /// </summary>
        /// <param name="response">
        ///     The response.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string ReadChunkedResponse(WebResponse response)
        {
            var sb = new StringBuilder();
            var buf = new byte[8192];
            var resStream = response.GetResponseStream();
            int count;
            do
            {
                Debug.Assert(resStream != null, "resStream != null");
                count = resStream.Read(buf, 0, buf.Length);
                if (count == 0)
                {
                    continue;
                }

                var tmpString = Encoding.ASCII.GetString(buf, 0, count);
                sb.Append(tmpString);
            }
            while (count > 0);
            return sb.ToString();
        }

        /// <summary>
        ///     Add header.
        /// </summary>
        /// <param name="name">
        ///     The name.
        /// </param>
        /// <param name="value">
        ///     The value.
        /// </param>
        public void AddHeader(string name, string value)
        {
            this.extraHeaders.Add(name, value);
        }

        /// <summary>
        ///     Make a simple YouTube request.
        /// </summary>
        /// <param name="url">
        ///     The url.
        /// </param>
        /// <param name="method">
        ///     The request method.
        /// </param>
        /// <param name="body">
        ///     The body, if method is set to Post.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string SimpleYoutubeRequest(string url, Method method = Method.Get, string body = "")
        {
            this.request = (HttpWebRequest)WebRequest.Create(url);
            this.SetupHeaders();

            if (method == Method.Post)
            {
                this.request.Method = "POST";
                var bytes = Encoding.ASCII.GetBytes(body);
                this.request.ContentLength = bytes.Length;
                using (var requestStream = this.request.GetRequestStream())
                {
                    requestStream.Write(bytes, 0, bytes.Length);
                }
            }

            using (var response = (HttpWebResponse)this.request.GetResponse())
            {
                return ReadChunkedResponse(response);
            }
        }

        /// <summary>
        ///     The simple YouTube request.
        /// </summary>
        /// <param name="url">
        ///     The url.
        /// </param>
        /// <param name="nameValueCollection">
        ///     The name value collection.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public string SimpleYoutubeRequest(string url, NameValueCollection nameValueCollection)
        {
            var stringBuilder = new StringBuilder();
            foreach (string key in nameValueCollection.Keys)
            {
                stringBuilder.AppendFormat(
                                           "{0}={1}&",
                                           WebUtility.UrlEncode(key),
                                           WebUtility.UrlEncode(nameValueCollection[key]));
            }

            stringBuilder.Length -= 1;

            return this.SimpleYoutubeRequest(url, Method.Post, stringBuilder.ToString());
        }

        /// <summary>
        ///     Setup request headers.
        /// </summary>
        private void SetupHeaders()
        {
            this.request.UserAgent = this.Useragent;
            this.request.CookieContainer = this.Cookies;
            this.request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            this.request.ContentType = "application/x-www-form-urlencoded";
            this.request.Headers.Add("Accept-Encoding", "gzip, deflate");
            this.request.Headers.Add("Accept-Language", "en-US,en;q=0.8,nl;q=0.6");
            this.request.Headers.Add("DNT", "1");
            foreach (string header in this.extraHeaders.Keys)
            {
                this.request.Headers.Add(header, this.extraHeaders[header]);
            }

            this.extraHeaders.Clear();
            this.request.AllowAutoRedirect = true;
            this.request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            this.request.Proxy = this.SetupProxy();
        }

        /// <summary>
        ///     Setup proxy.
        /// </summary>
        /// <returns>
        ///     The <see cref="IWebProxy" />.
        /// </returns>
        private IWebProxy SetupProxy()
        {
            if (!this.Proxy.HasProxy)
            {
                return WebRequest.DefaultWebProxy;
            }

            var webProxy = new WebProxy(this.Proxy.ProxyFormated, true);
            if (!this.Proxy.HasCredentials)
            {
                return webProxy;
            }

            var credentials = this.Proxy.Credentials;
            webProxy.Credentials = new NetworkCredential(credentials.Username, credentials.Password);
            return webProxy;
        }
    }
}