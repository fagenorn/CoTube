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
            UpvoteManager.Login("x9.102934@gmail.com", "9NazD802jz");
            Assert.IsTrue(UpvoteManager.IsLoggedIn);
            UpvoteManager.SubmitUpvoteRequest(
                "https://www.youtube.com/watch?v=O42rOvv-1bk&lc=z13agph4zrbdcbp0q23yuzqbnuvpwdmux",
                15);
        }
    }
}