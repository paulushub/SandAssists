using System;

namespace Sandcastle.Contents
{
    /// <summary>
    /// This specifies the type of the media defined by a media item, 
    /// <see cref="MediaItem"/>.
    /// </summary>
    [Serializable]
    public enum MediaItemType
    {
        /// <summary>
        /// Indicates unspecified or unknown media type.
        /// </summary>
        None        = 0,
        /// <summary>
        /// Indicates the image/picture media type.
        /// </summary>
        Image       = 1,
        /// <summary>
        /// Indicates the video or movie media type.
        /// </summary>
        Video       = 2,
        /// <summary>
        /// Indicates the Adobe Flash media type.
        /// </summary>
        Flash       = 4,
        /// <summary>
        /// Indicates the Microsoft Silverlight media type.
        /// </summary>
        Silverlight = 5,
        /// <summary>
        /// Indicates the image/picture gallery media collection type.
        /// </summary>
        Gallery     = 6
    }
}
