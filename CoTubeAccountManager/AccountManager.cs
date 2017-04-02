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
        ///     Gets or sets the delay between each comment.
        /// </summary>
        public static int DelayBetweenEachComment { get; set; } = 10000;

        /// <summary>
        ///     The lock.
        /// </summary>
        public static object Lock => new object();

        /// <summary>
        ///     Gets the log.
        /// </summary>
        public static ObservableCollectionExt<string> Log { get; } = new ObservableCollectionExt<string>();

        /// <summary>
        ///     Gets the replies.
        /// </summary>
        public static ObservableCollectionExt<string> Replies { get; } = new ObservableCollectionExt<string>();

        /// <summary>
        ///     Gets the URLs to comment on.
        /// </summary>
        public static ObservableCollectionExt<string> Urls { get; } = new ObservableCollectionExt<string>();

        /// <summary>
        ///     Gets or sets a value indicating whether automatic restart.
        /// </summary>
        public bool AutomaticRestarter { get; set; } = false;

        /// <summary>
        ///     Gets or sets the automatic restart timeout in minutes. Time before restarting process.
        /// </summary>
        public int AutomaticRestarterTimeout { get; set; } = 1;

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
        ///     Add new log item.
        /// </summary>
        /// <param name="item">
        ///     The item.
        /// </param>
        public static void AddNewLog(string item)
        {
            Log.Add(item);
            const int MaxLogAmount = 10;
            if (Log.Count > MaxLogAmount)
            {
                ListHelper.RemoveRange(Log, 0, Log.Count - 1 - MaxLogAmount);
            }
        }

        /// <summary>
        ///     Add new reply.
        /// </summary>
        /// <param name="reply">
        ///     The reply.
        /// </param>
        public static void AddNewReply(string reply)
        {
            Replies.Add(reply);
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
            var options2 = new ParallelOptions { MaxDegreeOfParallelism = this.MaxThreads };
            Parallel.ForEach(
                             Accounts,
                             options2,
                             account =>
                                 {
                                     try
                                     {
                                         lock (Lock)
                                         {
                                             AddNewLog($"Logging in - {account.Email}");
                                         }

                                         account.Login();
                                     }
                                     catch (Exception)
                                     {
                                         AddNewLog($"Failed to login - {account.Email}");
                                     }
                                 });

            Parallel.ForEach(
                             Accounts,
                             options,
                             account =>
                                 {
                                     CancellationTokenSource.Token.ThrowIfCancellationRequested();

                                     // Start Commenting on 3 Random Videos
                                     for (var i = 0; i < 3; i++)
                                     {
                                         if (toCommentList.Count == 0)
                                         {
                                             continue;
                                         }

                                         if (!account.IsLoggedIn())
                                         {
                                             return;
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

                                             // Reply 5 times to comment with random accounts
                                             for (var j = 0; j < 5; j++)
                                             {
                                                 var replyAccount = Accounts.RandomItem();
                                                 var reply = Replies.RandomItem();
                                                 replyAccount.Reply(
                                                                    urlToComment,
                                                                    commentResponse.Parameter,
                                                                    reply.SpinIt());
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