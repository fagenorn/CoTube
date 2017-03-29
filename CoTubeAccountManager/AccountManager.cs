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
    using System.Collections.Generic;

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
        ///     Adds a new account to the account manager.
        /// </summary>
        /// <param name="account">
        ///     The account.
        /// </param>
        public static void CreateNewAccount(YAccount account)
        {
            Accounts.Add(account);
        }
    }
}