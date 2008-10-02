using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies the target attributes of the links, mainly to external targets
    /// such as the MSDN.
    /// <para>
    /// This is the same as the predefined HTML target attribute, which defined whether
    /// to open the link page in a separate browser window, or in the current browser 
    /// window.
    /// </para>
    /// </summary>
    [Serializable]
    public enum BuildLinkTargetType
    {
        /// <summary>
        /// Indicates loading the page inot a new browser window with no name.
        /// <para>
        /// This is equivalent to the HTML link target: <c>_blank</c>.
        /// </para>
        /// </summary>
        Blank  = 0,
        /// <summary>
        /// Indicates loading the page into the current browser window.
        /// <para>
        /// This is equivalent to the HTML link target: <c>_self</c>.
        /// </para>
        /// <note>
        /// If it is in a frame or frameset it will load within that frame. 
        /// </note>
        /// </summary>
        Self   = 1,
        /// <summary>
        /// Indicates loading the page into the frame that is superior to the frame
        /// the hyperlink is in.
        /// <para>
        /// <para>
        /// This is equivalent to the HTML link target: <c>_parent</c>.
        /// </note>
        /// If there is no frameset defined, this is equivalent to the 
        /// <see cref="OutputLinkTarget.Self"/> target.
        /// </note>
        /// </summary>
        Parent = 2,
        /// <summary>
        /// Indicates cancelling all frames, and loading the page in full browser window.
        /// <para>
        /// This is equivalent to the HTML link target: <c>_top</c>.
        /// </para>
        /// <note>
        /// If there is no frameset defined, this is equivalent to the 
        /// <see cref="OutputLinkTarget.Self"/> target.
        /// </note>
        /// </summary>
        Top    = 3
    }
}
