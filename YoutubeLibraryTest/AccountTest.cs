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
            const string VideoUrl = "https://www.youtube.com/watch?v=xt544bCPqAw";
            account.Login();
            var response = account.Comment(VideoUrl, "LoL, I can't stop listening.");

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
        ///     Set up various account details.
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
        }
    }
}