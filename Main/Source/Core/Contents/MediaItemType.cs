﻿using System;

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
        Flash       = 3,
        /// <summary>
        /// Indicates the Microsoft Silverlight media type.
        /// </summary>
        Silverlight = 4,
        /// <summary>
        /// Indicates the YouType media type.
        /// </summary>
        YouTube     = 5,
        /// <summary>
        /// Indicates the Adobe Portable Document Format (PDF) media type.
        /// </summary>
        Pdf         = 6,
        /// <summary>
        /// Indicates the Microsoft <c>XML</c> Paper Specification format (XPS) media type.
        /// </summary>
        Xps         = 7
    }
}
