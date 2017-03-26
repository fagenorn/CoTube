﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommentResponse.cs" company="GMT">
//   Created by Fagenorn
// </copyright>
// <summary>
//   The comment response.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YoutubeLibrary.Response
{
    /// <summary>
    ///     The comment response.
    /// </summary>
    public class CommentResponse
    {
        /// <summary>
        ///     Gets or sets the comment id.
        /// </summary>
        public string CommentId { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether response was a success.
        /// </summary>
        public bool Success { get; set; }
    }
}