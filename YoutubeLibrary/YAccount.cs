// --------------------------------------------------------------------------------------------------------------------
// <copyright file="YAccount.cs" company="GMT">
//   Created by Fagenorn
// </copyright>
// <summary>
//   The y account.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YoutubeLibrary
{
    using System.Collections.Specialized;
    using System.Net;

    using YoutubeLibrary.Response;

    /// <summary>
    ///     The YouTube account.
    /// </summary>
    public class YAccount
    {
        /// <summary>
        ///     The http object.
        /// </summary>
        private readonly Http http = new Http();

        /// <summary>
        ///     Initializes a new instance of the <see cref="YAccount" /> class.
        /// </summary>
        /// <param name="email">
        ///     The email.
        /// </param>
        /// <param name="password">
        ///     The password.
        /// </param>
        public YAccount(string email, string password)
        {
            this.Email = email;
            this.Password = password;
        }

        /// <summary>
        ///     Gets the email.
        /// </summary>
        public string Email { get; }

        /// <summary>
        ///     Gets the password.
        /// </summary>
        public string Password { get; }

        /// <summary>
        ///     Comment on a video.
        /// </summary>
        /// <param name="videoUrl">
        ///     The video url.
        /// </param>
        /// <param name="comment">
        ///     The comment.
        /// </param>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public CommentResponse Comment(string videoUrl, string comment)
        {
            var videoCode = videoUrl.GetVideoCodeFromUrl();
            var videoResp = this.http.SimpleYoutubeRequest(videoUrl);
            var sessionToken = videoResp.GetSessionToken();
            var commentToken = WebUtility.UrlEncode(videoResp.GetCommentToken());

            var identityTokenHeader = videoResp.GetIdentityToken();
            this.http.AddHeader("X-Youtube-Identity-Token", identityTokenHeader);

            var watchResponse =
                this.http.SimpleYoutubeRequest(
                                               string.Format(Constants.VideoWatchUrl, videoCode, commentToken),
                                               Method.Post,
                                               $"session_token={sessionToken}");
            var videoParam = watchResponse.GetVideoParam();
            watchResponse.GetBotguardCode();

            var body = new NameValueCollection
                           {
                               { "content", comment },
                               { "params", videoParam },
                               { "bgr", watchResponse.GetBotguardCode() },
                               { "session_token", sessionToken }
                           };

            var commentId = this.http.SimpleYoutubeRequest(Constants.PostCommentUrl, body).GetCommentId();
            return new CommentResponse { Success = true, CommentId = commentId };
        }

        /// <summary>
        ///     Checks if user is logged in by requesting YouTube.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool IsLoggedIn()
        {
            var response = this.http.SimpleYoutubeRequest("https://www.youtube.com/");
            return response.Contains(this.Email);
        }

        /// <summary>
        ///     Login into the account.
        /// </summary>
        /// <returns>
        ///     The <see cref="bool" />.
        /// </returns>
        public bool Login()
        {
            var emailResponse = this.http.SimpleYoutubeRequest(Constants.LoginUrl);

            var inputs = emailResponse.GetFormValuesFromId("gaia_loginform");
            inputs["Email"] = this.Email;
            var passwordResponse = this.http.SimpleYoutubeRequest(
                                                                  emailResponse.GetActionFromFormId("gaia_loginform"),
                                                                  inputs);

            inputs = passwordResponse.GetFormValuesFromId("gaia_loginform");
            inputs.Add("Passwd", this.Password);
            this.http.SimpleYoutubeRequest(passwordResponse.GetActionFromFormId("gaia_loginform"), inputs);
            return true;
        }
    }
}