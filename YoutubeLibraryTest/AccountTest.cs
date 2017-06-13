// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AccountTest.cs" company="GMT">
//   Created by Fagenorn
// </copyright>
// <summary>
//   Defines the AccountTest type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YoutubeLibraryTest
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using YoutubeLibrary;

    /// <summary>
    ///     The account test.
    /// </summary>
    [TestClass]
    public class AccountTest
    {
        /// <summary>
        ///     Comment on video.
        /// </summary>
        [TestMethod]
        public void CommentOnVideo()
        {
            const string Email = "donaldducktrumpmaker@gmail.com";
            const string Password = "celementoni";
            var account = new YAccount(Email, Password);
            const string VideoUrl = "https://www.youtube.com/watch?v=O42rOvv-1bk";
            account.Login();
            var response = account.Comment(VideoUrl, "LoL, I can't stop laughing.");

            Assert.IsTrue(response.Success);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.CommentId));
        }

        /// <summary>
        ///     Login into the account successfully.
        /// </summary>
        [TestMethod]
        public void LoginIntoAccountSuccess()
        {
            const string Email = "stephenmcclellan36@gmail.com";
            const string Password = "KuCobof23%";
            var account = new YAccount(Email, Password);
            Assert.IsFalse(account.IsLoggedIn());
            account.Login();
            Assert.IsTrue(account.IsLoggedIn());
        }

        /// <summary>
        /// Reply to comment.
        /// </summary>
        [TestMethod]
        public void ReplyToComment()
        {
            const string Email = "donaldducktrumpmaker@gmail.com";
            const string Password = "celementoni";
            var account = new YAccount(Email, Password);
            const string VideoUrl = "https://www.youtube.com/watch?v=O42rOvv-1bk";
            account.Login();
            var response = account.Comment(VideoUrl, "Elise is pretty cool");
            Assert.IsTrue(response.Success);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.CommentId));
            response = account.Reply(VideoUrl, response.Parameter, ":P");
            Assert.IsTrue(response.Success);
            Assert.IsFalse(string.IsNullOrWhiteSpace(response.CommentId));
        }

        /// <summary>
        ///     Set up various account details.
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
        }
    }
}