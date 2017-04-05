// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MainWindow.xaml.cs" company="GMT">
//   Created by Fagenorn
// </copyright>
// <summary>
//   Interaction logic for MainWindow.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoTube
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    using CoTubeAccountManager;

    using FluentScheduler;

    using MahApps.Metro.Controls.Dialogs;

    using UsefullUtilitiesLibrary;
    using UsefullUtilitiesLibrary.FileManipulation;

    using YoutubeLibrary;

    /// <summary>
    ///     The main window.
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        public MainWindow()
        {
            this.InitializeComponent();
            BindingOperations.EnableCollectionSynchronization(AccountManager.Log, AccountManager.Lock);
            JobManager.Initialize(new Registry());
        }

        /// <summary>
        ///     The status.
        /// </summary>
        private enum Status
        {
            /// <summary>
            ///     The start.
            /// </summary>
            Start,

            /// <summary>
            ///     The stop.
            /// </summary>
            Stop
        }

        /// <summary>
        ///     Gets or sets the account manager.
        /// </summary>
        public AccountManager AccountManager { get; set; } = new AccountManager();

        /// <summary>
        ///     Gets or sets a value indicating whether account manager is busy.
        /// </summary>
        private bool IsBusy { get; set; }

        /// <summary>
        ///     Add a new account.
        /// </summary>
        /// <param name="account">
        ///     The account.
        /// </param>
        public void AddNewAccount(YAccount account)
        {
            AccountManager.CreateNewAccount(account);
        }

        /// <summary>
        ///     Add new account button is clicked.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private async void AddNewAccountClick(object sender, RoutedEventArgs e)
        {
            var logindialogsettings = new LoginDialogSettings
                                          {
                                              AnimateHide = false,
                                              AffirmativeButtonText = "Next",
                                              UsernameWatermark = "Email",
                                              NegativeButtonVisibility = Visibility.Visible,
                                              EnablePasswordPreview = true
                                          };
            var loginData = await this.ShowLoginAsync(
                                                      "Add a new account",
                                                      "Enter the credentials of the account you wish to add.",
                                                      logindialogsettings);

            var username = loginData?.Username;
            var password = loginData?.Password;
            if (string.IsNullOrWhiteSpace(username))
            {
                // Check if cancel pressed
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                // Check if cancel pressed
                return;
            }

            Proxy tempProxy = null;
            var mds = new MetroDialogSettings { AnimateHide = false, AnimateShow = false };

            // Don't animate hide and show
            var proxy = await this.ShowInputAsync(
                                                  "Add Account",
                                                  "Enter the proxy for the account. (IP:Port) Leave blank for none.",
                                                  mds);
            if (proxy == null)
            {
                // Check if cancel pressed
                return;
            }

            if (!string.IsNullOrWhiteSpace(proxy) && proxy.Contains(":"))
            {
                var credentialsProxy = proxy.Split(':');
                var lds = new LoginDialogSettings
                              {
                                  AnimateHide = false,
                                  AnimateShow = false,
                                  EnablePasswordPreview = true,
                                  AffirmativeButtonText = "Next",
                                  NegativeButtonVisibility = Visibility.Visible
                              };

                // Don't animate hide and show
                var proxyLogin = await this.ShowLoginAsync(
                                                           "Add Account",
                                                           "Enter the credentials of your proxy, or leave blank if not applicable.",
                                                           lds);
                if (proxyLogin == null)
                {
                    return;
                }

                if (!string.IsNullOrWhiteSpace(proxyLogin.Password) && !string.IsNullOrWhiteSpace(proxyLogin.Username))
                {
                    var credentials = new Credentials(proxyLogin.Username, proxyLogin.Password);
                    tempProxy = new Proxy(credentialsProxy[0], credentialsProxy[1], credentials);
                }
                else
                {
                    tempProxy = new Proxy(credentialsProxy[0], credentialsProxy[1]);
                }
            }

            var account = new YAccount(username, password);
            if (tempProxy != null)
            {
                account.Proxy = tempProxy;
            }

            this.AddNewAccount(account);
        }

        /// <summary>
        ///     Add new comment button is clicked.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private async void AddNewCommentClick(object sender, RoutedEventArgs e)
        {
            var comment = await this.ShowInputAsync("Add Comment", "Enter the comment. Spintax supported.");
            if (string.IsNullOrWhiteSpace(comment))
            {
                return;
            }

            AccountManager.AddNewComment(comment);
        }

        /// <summary>
        ///     Add new reply button is  clicked.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private async void AddNewReplyClick(object sender, RoutedEventArgs e)
        {
            var reply = await this.ShowInputAsync("Add Reply", "Enter the reply. Spintax supported.");
            if (string.IsNullOrWhiteSpace(reply))
            {
                return;
            }

            AccountManager.AddNewReply(reply);
        }

        /// <summary>
        ///     Add new url button is clicked.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private async void AddNewUrlClick(object sender, RoutedEventArgs e)
        {
            var url = await this.ShowInputAsync("Add URL", "Enter the UR.");
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            url = url.ConvertValidHttpsUrl();
            if (url.IsValidUriString())
            {
                AccountManager.AddYoutubeUrl(url);
            }
        }

        /// <summary>
        ///     Deletes accounts.
        /// </summary>
        private void DeleteAccounts()
        {
            var selectedItemsList = this.AccountsGrid.SelectedItems.Cast<YAccount>().ToList();
            if (selectedItemsList.Count == 0)
            {
                return;
            }

            AccountManager.Accounts.RemoveAll(
                                              x =>
                                                  selectedItemsList.Any(
                                                                        y =>
                                                                            y.Email.ToLower().Trim()
                                                                            == x.Email.ToLower().Trim()));
        }

        /// <summary>
        ///     The delete accounts click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void DeleteAccountsClick(object sender, RoutedEventArgs e)
        {
            this.DeleteAccounts();
        }

        /// <summary>
        ///     Delete comments.
        /// </summary>
        private void DeleteComments()
        {
            var selectedItemsList = this.CommentsGrid.SelectedItems.Cast<string>().ToList();
            if (selectedItemsList.Count == 0)
            {
                return;
            }

            AccountManager.Comments.RemoveAll(x => selectedItemsList.Any(y => y.ToLower().Trim() == x.ToLower().Trim()));
        }

        /// <summary>
        ///     Delete comments click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void DeleteCommentsClick(object sender, RoutedEventArgs e)
        {
            this.DeleteComments();
        }

        /// <summary>
        ///     Delete replies.
        /// </summary>
        private void DeleteReplies()
        {
            var selectedItemsList = this.RepliesGrid.SelectedItems.Cast<string>().ToList();
            if (selectedItemsList.Count == 0)
            {
                return;
            }

            AccountManager.Replies.RemoveAll(x => selectedItemsList.Any(y => y.ToLower().Trim() == x.ToLower().Trim()));
        }

        /// <summary>
        ///     Delete replies button is click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void DeleteRepliesClick(object sender, RoutedEventArgs e)
        {
            this.DeleteReplies();
        }

        /// <summary>
        ///     Delete URLs.
        /// </summary>
        private void DeleteUrls()
        {
            var selectedItemsList = this.UrlsGrid.SelectedItems.Cast<string>().ToList();
            if (selectedItemsList.Count == 0)
            {
                return;
            }

            AccountManager.Urls.RemoveAll(x => selectedItemsList.Any(y => y.ToLower().Trim() == x.ToLower().Trim()));
        }

        /// <summary>
        ///     Delete URLs click.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void DeleteUrlsClick(object sender, RoutedEventArgs e)
        {
            this.DeleteUrls();
        }

        /// <summary>
        ///     Context menu opening.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        /// <exception cref="ArgumentOutOfRangeException">
        ///     Status type not handled
        /// </exception>
        private void GridContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var grid = (DataGrid)sender;
            var cm = grid.ContextMenu;
            if (cm == null)
            {
                return;
            }

            var selectedAmount = grid.SelectedItems.Count;
            foreach (var item in cm.Items)
            {
                var menuItem = item as MenuItem;
                if (menuItem == null)
                {
                    continue;
                }

                var status = (DataGridHelper.WhenEnabled)menuItem.GetValue(DataGridHelper.MenuEnabledWhenProperty);
                switch (status)
                {
                    case DataGridHelper.WhenEnabled.Always:
                        menuItem.IsEnabled = true;
                        break;
                    case DataGridHelper.WhenEnabled.One:
                        menuItem.IsEnabled = selectedAmount == 1;
                        break;
                    case DataGridHelper.WhenEnabled.Zero:
                        menuItem.IsEnabled = selectedAmount <= 0;
                        break;
                    case DataGridHelper.WhenEnabled.OneOrMore:
                        menuItem.IsEnabled = selectedAmount != 0;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        /// <summary>
        ///     Import an account file .
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void ImportFileAccount(object sender, RoutedEventArgs e)
        {
            var path = FileDialogHelper.OpenTextFile();
            if (path == null)
            {
                return;
            }

            var list = FileDialogHelper.ReadPerLine(path);
            var parsedList = FileDialogHelper.ParseUserFile(list);
            foreach (var accountProxy in parsedList)
            {
                Proxy tempProxy = null;
                if (accountProxy.ProxyPass != null)
                {
                    tempProxy = new Proxy(accountProxy.Ip, accountProxy.Port);
                }
                else if (accountProxy.Ip != null)
                {
                    tempProxy = new Proxy(
                                          accountProxy.Ip,
                                          accountProxy.Port,
                                          new Credentials(accountProxy.ProxyUser, accountProxy.ProxyPass));
                }

                var account = new YAccount(accountProxy.Username, accountProxy.Password);
                if (tempProxy != null)
                {
                    account.Proxy = tempProxy;
                }

                this.AddNewAccount(account);
            }
        }

        /// <summary>
        ///     Import a comment file.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void ImportFileComment(object sender, RoutedEventArgs e)
        {
            var path = FileDialogHelper.OpenTextFile();
            if (path == null)
            {
                return;
            }

            var list = FileDialogHelper.ReadPerLine(path);
            foreach (var line in list)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                AccountManager.AddNewComment(line);
            }
        }

        /// <summary>
        ///     Import file reply.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void ImportFileReply(object sender, RoutedEventArgs e)
        {
            var path = FileDialogHelper.OpenTextFile();
            if (path == null)
            {
                return;
            }

            var list = FileDialogHelper.ReadPerLine(path);
            foreach (var line in list)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                AccountManager.AddNewReply(line);
            }
        }

        /// <summary>
        ///     Import a url file .
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void ImportFileUrl(object sender, RoutedEventArgs e)
        {
            var path = FileDialogHelper.OpenTextFile();
            if (path == null)
            {
                return;
            }

            var list = FileDialogHelper.ReadPerLine(path);
            foreach (var url in list)
            {
                if (string.IsNullOrWhiteSpace(url))
                {
                    continue;
                }

                var urlFormated = url.ConvertValidHttpsUrl();
                if (urlFormated.IsValidUriString())
                {
                    AccountManager.AddYoutubeUrl(urlFormated);
                }
            }
        }

        /// <summary>
        ///     Start account manager.
        /// </summary>
        /// <param name="status">
        ///     The status.
        /// </param>
        /// <returns>
        ///     The <see cref="Task" />.
        /// </returns>
        private async Task StartAccountManager(Status status)
        {
            if (status == Status.Start)
            {
                if (this.IsBusy)
                {
                    AccountManager.AddNewLog($"Can't Start - Automatic Scheduler Is Running.");
                    return;
                }

                if (AccountManager.Accounts.Count == 0)
                {
                    // No Accounts
                    await this.ShowMessageAsync("No Accounts", "You need to have atleast one account.");
                    return;
                }

                if (AccountManager.Urls.Count == 0)
                {
                    // No Urls
                    await this.ShowMessageAsync("No Urls", "You need to have atleast one url.");
                    return;
                }

                if (AccountManager.Comments.Count == 0)
                {
                    // No Comments
                    await this.ShowMessageAsync("No Comments", "You need to have atleast one comment.");
                    return;
                }

                if (AccountManager.Replies.Count == 0)
                {
                    // No Replies
                    await this.ShowMessageAsync("No Replies", "You need to have atleast one reply.");
                    return;
                }

                this.IsBusy = true;
                AccountManager.CancellationTokenSource.Dispose();
                AccountManager.CancellationTokenSource = new CancellationTokenSource();
                try
                {
                    await Task.Factory.StartNew(
                                                this.AccountManager.StartCommentingProcess,
                                                AccountManager.CancellationTokenSource.Token);
                    if (!this.AccountManager.AutomaticRestarter)
                    {
                        return;
                    }

                    JobManager.AddJob(
                                      async () => await this.StartAccountManager(Status.Start),
                                      s =>
                                          s.NonReentrant()
                                           .ToRunOnceIn(this.AccountManager.AutomaticRestarterTimeout)
                                           .Minutes());
                    AccountManager.AddNewLog(
                                             $"Automatic Scheduler - Next run in {this.AccountManager.AutomaticRestarterTimeout} minute(s)");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
                finally
                {
                    this.IsBusy = false;
                }
            }
            else
            {
                AccountManager.CancellationTokenSource.Cancel();
            }
        }

        /// <summary>
        ///     Start account manager button is clicked.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private async void StartAccountManagerClick(object sender, RoutedEventArgs e)
        {
            var textButton = (TextBlock)((Button)sender).Content;
            if (textButton.Text == "Start")
            {
                textButton.Text = "Stop";
                await this.StartAccountManager(Status.Start);
                textButton.Text = "Start";
            }
            else
            {
                await this.StartAccountManager(Status.Stop);
            }
        }
    }
}