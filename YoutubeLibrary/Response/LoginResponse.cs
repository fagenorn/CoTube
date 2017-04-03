// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LoginResponse.cs" company="GMT">
//   Created by Fagenorn
// </copyright>
// <summary>
//   Defines the LoginResponse type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YoutubeLibrary.Response
{
    /// <summary>
    ///     The login response.
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        ///     Gets or sets a value indicating whether challenge is required to login.
        /// </summary>
        public bool ChallengeRequired { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether login was a success.
        /// </summary>
        public bool Success { get; set; }
    }
}