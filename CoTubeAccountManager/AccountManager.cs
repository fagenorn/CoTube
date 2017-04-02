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
    using System.Threading;
    using System.Threading.Tasks;

    using UsefullUtilitiesLibrary;
    using UsefullUtilitiesLibrary.CustomList;

    using YoutubeLibrary;

    /// <summary>
    ///     The account manager.
    /// </summary>
    public class AccountManager
    {
        /// <summary>
        ///     Gets the accounts.
        /// </summary>
        public static ObservableCollectionExt<YAccount> Accounts { get; } =
            new ObservableCollectionExt<YAccount>(new List<YAccount>());

        /// <summary>
        ///     Gets or sets the cancellation token source.
        /// </summary>
        public static CancellationTokenSource CancellationTokenSource { get; set; } = new CancellationTokenSource();

        /// <summary>
        ///     Gets the comments list.
        /// </summary>
        public static ObservableCollectionExt<string> Comments { get; } = new ObservableCollectionExt<string>();

        /// <summary>
        ///     The lock.
        /// </summary>
        public static object Lock => new object();

        /// <summary>
        ///     Gets the log.
        /// </summary>
        public static ObservableCollectionExt<string> Log { get; } = new ObservableCollectionExt<string>();

        /// <summary>
        ///     Gets the URLs to comment on.
        /// </summary>
        public static ObservableCollectionExt<string> Urls { get; } = new ObservableCollectionExt<string>();

        /// <summary>
        ///     Gets or sets the max threads.
        /// </summary>
        public int MaxThreads { get; set; } = 5;

        /// <summary>
        ///     Gets or sets the panel password.
        /// </summary>
        public string PanelPassword { get; set; }

        /// <summary>
        ///     Gets or sets the panel up-vote amount.
        /// </summary>
        public int PanelUpvoteAmount { get; set; } = 15;

        /// <summary>
        ///     Gets or sets the panel username.
        /// </summary>
        public string PanelUsername { get; set; }

        /// <summary>
        ///     The delay between each comment in milliseconds.
        /// </summary>
        private static int DelayBetweenEachComment => 6000;

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
        ///     Add new comment range.
        /// </summary>
        /// <param name="comments">
        ///     The comments.
        /// </param>
        public static void AddNewCommentRange(IEnumerable<string> comments)
        {
            Comments.AddRange(comments);
        }

        /// <summary>
        ///     Add a YouTube URL to comment on.
        /// </summary>
        /// <param name="url">
        ///     The URL.
        /// </param>
        public static void AddYoutubeUrl(string url)
        {
            Urls.Add(url);
        }

        /// <summary>
        ///     Add YouTube URL range.
        /// </summary>
        /// <param name="urls">
        ///     The URLs.
        /// </param>
        public static void AddYoutubeUrlRange(IEnumerable<string> urls)
        {
            Urls.AddRange(urls);
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
        public void StartCommentingProcess()
        {
            var toCommentList = new List<string>(Urls.ToList());
            var options = new ParallelOptions
                              {
                                  CancellationToken = CancellationTokenSource.Token,
                                  MaxDegreeOfParallelism = this.MaxThreads
                              };
            Parallel.ForEach(
                             Accounts,
                             options,
                             account =>
                                 {
                                     CancellationTokenSource.Token.ThrowIfCancellationRequested();
                                     lock (Lock)
                                     {
                                         AddNewLog($"Logging in - {account.Email}");
                                     }

                                     try
                                     {
                                         account.Login();
                                         if (!account.IsLoggedIn())
                                         {
                                             return;
                                         }
                                     }
                                     catch (Exception)
                                     {
                                         AddNewLog($"Failed to login - {account.Email}");
                                         return;
                                     }

                                     for (var i = 0; i < 3; i++)
                                     {
                                         if (toCommentList.Count == 0)
                                         {
                                             continue;
                                         }

                                         var urlToComment = toCommentList.RandomItem();
                                         var comment = Comments.RandomItem();
                                         lock (Lock)
                                         {
                                             AddNewLog($"Commenting on {urlToComment} - {account.Email}");
                                         }

                                         try
                                         {
                                             var commentResponse = account.Comment(urlToComment, comment.SpinIt());
                                             if (commentResponse.Success)
                                             {
                                                 this.SubmitCommentId(commentResponse.CommentLink);
                                             }
                                         }
                                         catch (Exception)
                                         {
                                             AddNewLog($"Failed to Comment - {account.Email}");
                                             return;
                                         }

                                         lock (Lock)
                                         {
                                             toCommentList.Remove(urlToComment);
                                         }

                                         CancellationTokenSource.Token.WaitHandle.WaitOne(DelayBetweenEachComment);
                                     }
                                 });
        }

        /// <summary>
        ///     Add new log item.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        private static void AddNewLog(string item)
        {
            Log.Add(item);
            const int MaxLogAmount = 10;
            if (Log.Count > MaxLogAmount)
            {
                ListHelper.RemoveRange(Log, 0, Log.Count - 1 - MaxLogAmount);
            }
        }

        /// <summary>
        ///     Submit comment link to receive up-votes.
        /// </summary>
        /// <param name="commentLink">
        ///     The comment link.
        /// </param>
        private void SubmitCommentId(string commentLink)
        {
            if (string.IsNullOrWhiteSpace(this.PanelUsername))
            {
                return;
            }

            if (!UpvoteManager.IsLoggedIn)
            {
                UpvoteManager.Login(this.PanelUsername, this.PanelPassword);
            }

            UpvoteManager.SubmitUpvoteRequest(commentLink, this.PanelUpvoteAmount);
        }
    }
}