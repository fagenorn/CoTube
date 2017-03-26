// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UserAgent.cs" company="GMT">
//   Created by Fagenorn
// </copyright>
// <summary>
//   The user agent.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YoutubeLibrary
{
    /// <summary>
    ///     The user agent.
    /// </summary>
    internal static class UserAgent
    {
        /// <summary>
        ///     Generate a random browser user agent.
        /// </summary>
        /// <returns>
        ///     The <see cref="string" />.
        /// </returns>
        public static string GenerateUseragent()
        {
            return
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/57.0.2987.110 Safari/537.36";
        }
    }
}