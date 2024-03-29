﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DomParser.cs" company="GMT">
//   Created by Fagenorn
// </copyright>
// <summary>
//   Defines the DomParser type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YoutubeLibrary
{
    using System;
    using System.Collections.Specialized;
    using System.Text;
    using System.Text.RegularExpressions;

    using HtmlAgilityPack;

    /// <summary>
    ///     The DOM parser.
    /// </summary>
    internal static class DomParser
    {
        /// <summary>
        ///     The random generator.
        /// </summary>
        private static readonly Random Random = new Random();

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
        ///     Get BotGuard code.
        /// </summary>
        /// <param name="htmlDocument">
        ///     The html document.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GetBotguardCode(this string htmlDocument)
        {
            const string Letters = "abcdefghijklmnopqrstuvwxyz";
            var sb = new StringBuilder();
            for (var index = 0; index < 30; index++)
            {
                sb.Append(Letters[Random.Next(Letters.Length - 1)]);
            }

            return "!v7ylvLkPAAMRAACZAdhZ5tDJmm9kt08Jc1HSFEdOqKIRufjYBS0+5y9BA6bUseOsjEm2TqTuu/IAJ07u+QLGNcPa5+" + sb;
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
        /// The get comment parameter.
        /// </summary>
        /// <param name="commentResponse">
        /// The comment response.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        public static string GetCommentParam(this string commentResponse)
        {
            return RegexGetMatch("data-simplebox-params=\\\\\"(.*?)\\\\\"", commentResponse);
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
        ///     The get BotGuard url.
        /// </summary>
        /// <param name="htmlDocument">
        ///     The html document.
        /// </param>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        // ReSharper disable once UnusedMember.Local
        private static string GetBotguardUrl(this string htmlDocument)
        {
            return RegexGetMatch("COMMENTS_BG_IU\', \\\\\"(.*?)\\\\\"\\)", htmlDocument);
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