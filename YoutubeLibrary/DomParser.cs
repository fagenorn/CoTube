// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomParser.cs" company="GMT">
//   Created by Fagenorn
// </copyright>
// <summary>
//   Defines the DomParser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YoutubeLibrary
{
    using System.Collections.Specialized;
    using System.Text.RegularExpressions;

    using HtmlAgilityPack;

    /// <summary>
    ///     The DOM parser.
    /// </summary>
    internal static class DomParser
    {
        /// <summary>
        ///     Get action from form id.
        /// </summary>
        /// <param name="htmlDocument">
        ///     The html document.
        /// </param>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GetActionFromFormId(this string htmlDocument, string id)
        {
            var document = new HtmlDocument();
            document.LoadHtml(htmlDocument);
            var form = document.GetElementbyId(id);
            return form.Attributes["action"].Value;
        }

        /// <summary>
        ///     The get comment id.
        /// </summary>
        /// <param name="commentResponse">
        ///     The comment response.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GetCommentId(this string commentResponse)
        {
            return RegexGetMatch("data-cid=\\\\\"(.*?)\\\\\"", commentResponse);
        }

        /// <summary>
        ///     Get video parameter.
        /// </summary>
        /// <param name="htmlDocument">
        ///     The html document.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GetCommentToken(this string htmlDocument)
        {
            return RegexGetMatch("\'COMMENTS_TOKEN\': \"(.*?)\",", htmlDocument);
        }

        /// <summary>
        ///     Get form values from id.
        /// </summary>
        /// <param name="htmlDocument">
        ///     The html document.
        /// </param>
        /// <param name="id">
        ///     The id.
        /// </param>
        /// <returns>
        ///     The <see cref="NameValueCollection" />.
        /// </returns>
        public static NameValueCollection GetFormValuesFromId(this string htmlDocument, string id)
        {
            var nvc = new NameValueCollection();
            HtmlNode.ElementsFlags.Remove("form");
            var document = new HtmlDocument();
            document.LoadHtml(htmlDocument);

            var form = document.GetElementbyId(id);

            foreach (var node in form.SelectNodes("//input"))
            {
                var valueAttribute = node.Attributes["value"];
                var nameAttribute = node.Attributes["name"];

                if (nameAttribute != null && valueAttribute != null)
                {
                    nvc.Add(nameAttribute.Value, valueAttribute.Value);
                }
            }

            return nvc;
        }

        /// <summary>
        ///     Get identity token.
        /// </summary>
        /// <param name="htmlDocument">
        ///     The html document.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GetIdentityToken(this string htmlDocument)
        {
            return RegexGetMatch("X-YouTube-Identity-Token\': \"(.*?)\"", htmlDocument);
        }

        /// <summary>
        ///     Get comment token.
        /// </summary>
        /// <param name="htmlDocument">
        ///     The html document.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GetSessionToken(this string htmlDocument)
        {
            return RegexGetMatch("\'XSRF_TOKEN\': \"(.*?)\",", htmlDocument);
        }

        /// <summary>
        ///     Get video code from url.
        /// </summary>
        /// <param name="url">
        ///     The url.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GetVideoCodeFromUrl(this string url)
        {
            return RegexGetMatch("v=(.*?)(&|$)", url);
        }

        /// <summary>
        ///     Get session token.
        /// </summary>
        /// <param name="htmlDocument">
        ///     The html document.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GetVideoParam(this string htmlDocument)
        {
            return RegexGetMatch("data-simplebox-params=\\\\\"(.*?)\\\\\"", htmlDocument);
        }

        /// <summary>
        ///     Regex get match.
        /// </summary>
        /// <param name="patern">
        ///     The pattern.
        /// </param>
        /// <param name="content">
        ///     The content.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        private static string RegexGetMatch(string patern, string content)
        {
            var regex = new Regex(patern);
            var v = regex.Match(content);
            return v.Groups[1].ToString();
        }
    }
}