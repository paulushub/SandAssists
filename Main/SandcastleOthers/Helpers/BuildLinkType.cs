using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies the type of links, mainly the reference links, in a documentation.
    /// </summary>
    [Serializable]
    public enum BuildLinkType
    {
        /// <summary>
        /// Indicates no link.
        /// </summary>
        None         = 0,
        /// <summary>
        /// Indicates a link to the same document.
        /// </summary>
        Self         = 1,
        /// <summary>
        /// Indicates a local link, normally links in the same documentation collection,
        /// such as the HtmlHelp 1.x (.CHM) file.
        /// </summary>
        Local        = 2,
        /// <summary>
        /// Indicates an indexed link, normally links in the same documentation
        /// collection, such as the HtmlHelp 2.x (.HXS) file.
        /// </summary>
        Index        = 3,
        /// <summary>
        /// Indicates local or indexed link, the actual type is determined by the
        /// build link resolving component. If the link is found in the current list
        /// of cached links in the documentation, the local link is used; otherwise,
        /// the indexed link is used.
        /// <note type="important">
        /// Unless, there is a special need for this, please do not use this link type
        /// for performance reasons.
        /// </note>
        /// </summary>
        LocalOrIndex = 4,
        /// <summary>
        /// Indicates the MSDN links, used for linking to targets in the online MSDN 
        /// documentations.
        /// <para>
        /// This applies to both the HtmlHelp 1.x and HtmlHelp 2.x formats.
        /// </para>
        /// </summary>
        Msdn         = 5
    }
}
