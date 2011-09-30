using System;

namespace Sandcastle
{
    /// <summary>
    /// Specifies the kind of class of the build framework, 
    /// <see cref="BuildFramework"/>.
    /// </summary>
    [Serializable]
    public enum BuildFrameworkKind
    {
        /// <summary>
        /// Indicates unknown or unspecified framework.
        /// </summary>
        None        = 0,
        /// <summary>
        /// Indicates the standard .NET framework.
        /// </summary>
        DotNet      = 1,
        /// <summary>
        /// Indicates the Silverlight for the .NET framework.
        /// </summary>
        Silverlight = 2,
        /// <summary>
        /// Indicates the <see href="http://msdn.microsoft.com/en-us/library/gg597391.aspx">
        /// Portable Class Libraries for the .NET framework.
        /// </see>
        /// </summary>
        Portable    = 3,
        /// <summary>
        /// Indicates the Script# framework.
        /// <para>
        /// Visit the Script# website for more information, <see href="http://projects.nikhilk.net/ScriptSharp">ScriptSharp</see>
        /// </para>
        /// </summary>
        ScriptSharp = 4,
    }
}
