// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="GMT">
//   Created by Fagenorn
// </copyright>
// <summary>
//   Defines the Constants type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace YoutubeLibrary
{
    /// <summary>
    ///     The constants.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        ///     The login URL.
        /// </summary>
        public static string LoginUrl
            =>
                "https://accounts.google.com/ServiceLogin?continue=https%3A%2F%2Fwww.youtube.com%2Fsignin%3Ffeature%3Dsign_in_button%26hl%3Den%26app%3Ddesktop%26next%3D%252F%26action_handle_signin%3Dtrue&hl=en&service=youtube&passive=true&uilel=3"
        ;

        /// <summary>
        ///     The post comment url.
        /// </summary>
        public static string PostCommentUrl => "https://www.youtube.com/comment_service_ajax?action_create_comment=1";

        /// <summary>
        ///     The video watch url.
        /// </summary>
        public static string VideoWatchUrl
            =>
                "https://www.youtube.com/watch_fragments_ajax?v={0}&tr=time&distiller=1&ctoken={1}&frags=comments&spf=load"
        ;
    }
}