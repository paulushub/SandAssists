using System;

namespace Sandcastle.Contents
{
    /// <summary>
    /// Specifies the content type or format of the shared item text. 
    /// </summary>
    [Serializable]
    public enum SharedItemFormat
    {
        /// <summary>
        /// Indicates unknown or unspecified text format.
        /// </summary>
        None        = 0,
        /// <summary>
        /// Indicates a plain text format.
        /// </summary>
        PlainText   = 1,
        /// <summary>
        /// Indicates a markup text format, such as the HTML or XML.
        /// </summary>
        MarkupText  = 2,
        /// <summary>
        /// Indicates a encoded plain text format, normally the HTML escaped format.
        /// </summary>
        EncodedText = 3
    }
}
