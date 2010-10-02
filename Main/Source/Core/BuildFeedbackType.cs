using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies the type of the feedback presented to the user of the help file.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The feedback allows the user of the documentation to send comments or 
    /// suggestions to the developer team, and where applicable, rating the content of
    /// the help page.
    /// </para>
    /// <para>
    /// Most of the feedback types are not supported by all the current style or 
    /// presentations. Where a specified feedback type is not supported, it will be
    /// changed to the <see cref="BuildFeedbackType.Simple"/>, which is supported by
    /// all the styles.
    /// </para>
    /// <para>
    /// A custom feedback type can be provided by modifying the shared contents 
    /// templates in the "Contents" folder of the help builder installation.
    /// </para>
    /// </remarks>
    /// <seealso cref="BuildFeedback"/>
    [Serializable]
    public enum BuildFeedbackType
    {
        /// <summary>
        /// Inidicates unknown or unspecified feedback type.
        /// <para>
        /// Supported by: All Styles.
        /// </para>
        /// </summary>
        None     = 0,
        /// <summary>
        /// Indicates a simple feedback type.
        /// <para>
        /// Supported by: All Styles.
        /// </para>
        /// </summary>
        Simple   = 1,
        /// <summary>
        /// Indicates a "standard" feedback type.
        /// <para>
        /// Supported by: Vs2005/8 and Whidbey Styles.
        /// </para>
        /// </summary>
        Standard = 2,
        /// <summary>
        /// Indicates a rating feedback type.
        /// <para>
        /// Supported by: Vs2005/8 and Whidbey Styles.
        /// </para>
        /// </summary>
        Rating   = 3,
        /// <summary>
        /// Indicates custom or user-defined feedback type.
        /// <para>
        /// Supported by: All Styles.
        /// </para>
        /// </summary>
        Custom   = 4
    }
}
