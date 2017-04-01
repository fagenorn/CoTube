// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Credentials.cs" company="GMT">
//   Created by Fagenorn
// </copyright>
// <summary>
//   The credentials.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YoutubeLibrary
{
    /// <summary>
    ///     The credentials.
    /// </summary>
    public class Credentials
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Credentials" /> class.
        /// </summary>
        /// <param name="username">
        ///     The username.
        /// </param>
        /// <param name="password">
        ///     The password.
        /// </param>
        public Credentials(string username, string password)
        {
            this.Username = username;
            this.Password = password;
        }

        /// <summary>
        ///     Gets the password.
        /// </summary>
        public string Password { get; }

        /// <summary>
        ///     Gets the username.
        /// </summary>
        public string Username { get; }
    }
}