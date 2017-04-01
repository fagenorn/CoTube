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
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    using CoTubeAccountManager;

    using MahApps.Metro.Controls.Dialogs;

    using UsefullUtilitiesLibrary;

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
        }

        /// <summary>
        ///     Gets or sets the account manager.
        /// </summary>
        public AccountManager AccountManager { get; set; } = new AccountManager();

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

        private void ImportFileAccount(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ImportFileComment(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void ImportFileUrl(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
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

                textButton.Text = "Stop";
                AccountManager.CancellationTokenSource.Dispose();
                AccountManager.CancellationTokenSource = new CancellationTokenSource();
                try
                {
                    await Task.Factory.StartNew(
                                                this.AccountManager.StartCommentingProcess,
                                                AccountManager.CancellationTokenSource.Token);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }

                textButton.Text = "Start";
            }
            else
            {
                AccountManager.CancellationTokenSource.Cancel();
            }
        }
    }
}