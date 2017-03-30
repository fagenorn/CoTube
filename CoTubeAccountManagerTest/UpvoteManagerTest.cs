// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UpvoteManagerTest.cs" company="GMT">
//   Created by Fagenorn
// </copyright>
// <summary>
//   The up-vote manager test.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CoTubeAccountManagerTest
{
    using CoTubeAccountManager;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    ///     The up-vote manager test.
    /// </summary>
    [TestClass]
    public class UpvoteManagerTest
    {
        /// <summary>
        ///     Login into the panel and post YouTube comment url.
        /// </summary>
        [TestMethod]
        public void LoginIntoPanelAndPostYouTubeUrl()
        {
            Assert.IsFalse(UpvoteManager.IsLoggedIn);
            UpvoteManager.Login("mydbenouazzane@gmail.com", "eo1k87b7DY");
            Assert.IsTrue(UpvoteManager.IsLoggedIn);
            UpvoteManager.SubmitUpvoteRequest(
                                              "https://www.youtube.com/watch?v=xt544bCPqAw&lc=z122vtbghzubdzcg004cflzi1sriifzr3pw0k",
                                              20);
        }
    }
}