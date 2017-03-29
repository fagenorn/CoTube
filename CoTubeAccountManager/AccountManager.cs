// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountManager.cs" company="GMT">
//   Created by Fagenorn
// </copyright>
// <summary>
//   Defines the AccountManager type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoTubeAccountManager
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UsefullUtilitiesLibrary;

    using YoutubeLibrary;

    /// <summary>
    ///     The account manager.
    /// </summary>
    internal static class AccountManager
    {
        /// <summary>
        ///     Gets the accounts.
        /// </summary>
        private static List<YAccount> Accounts { get; } = new List<YAccount>();

        /// <summary>
        ///     Gets the comments list.
        /// </summary>
        private static List<string> Comments { get; } = new List<string>();

        /// <summary>
        ///     Gets the URLs to comment on.
        /// </summary>
        private static List<string> Urls { get; } = new List<string>();

        /// <summary>
        ///     Add new comment.
        /// </summary>
        /// <param name="comment">
        ///     The comment. (spin-tax supported)
        /// </param>
        public static void AddNewComment(string comment)
        {
            Comments.Add(comment);
        }

        /// <summary>
        ///     Add a YouTube URL to comment on.
        /// </summary>
        /// <param name="url">
        ///     The url.
        /// </param>
        public static void AddYoutubeUrl(string url)
        {
            Urls.Add(url);
        }

        /// <summary>
        ///     Adds a new account to the account manager.
        /// </summary>
        /// <param name="account">
        ///     The account.
        /// </param>
        public static void CreateNewAccount(YAccount account)
        {
            Accounts.Add(account);
        }

        /// <summary>
        ///     Start the commenting process.
        /// </summary>
        public static void StartCommentingProcess()
        {
            var toCommentList = Urls;
            foreach (var account in Accounts)
            {
                account.Login();
                if (!account.IsLoggedIn())
                {
                    continue;
                }

                for (var i = 0; i < 3; i++)
                {
                    if (toCommentList.Count == 0)
                    {
                        continue;
                    }

                    var urlToComment = toCommentList.RandomItem();
                    var comment = Comments.RandomItem();
                    var commentResponse = account.Comment(urlToComment, comment.SpinIt());
                    if (commentResponse.Success)
                    {
                        SubmitCommentId(commentResponse.CommentId);
                    }
                    toCommentList.FirstOrDefault(x => x = comment)
                }
            }
        }

        private static void SubmitCommentId(string commentId)
        {
            throw new NotImplementedException();
        }
    }
}