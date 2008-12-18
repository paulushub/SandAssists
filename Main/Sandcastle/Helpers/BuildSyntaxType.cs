using System;

namespace Sandcastle
{
    /// <summary>
    /// This specifies the languages to include in the syntax declaraction section.
    /// </summary>
    [Serializable, Flags]
    public enum BuildSyntaxType
    {
        /// <summary>
        /// Indicates including no language syntax.
        /// </summary>
        None        = 0,
        /// <summary>
        /// Indicates including the C# language syntax.
        /// </summary>
        CSharp      = 1,
        /// <summary>
        /// Indicates including the VB.NET language syntax.
        /// </summary>
        VisualBasic = 2,
        /// <summary>
        /// Indicates including the C++/CLI language syntax.
        /// </summary>
        CPlusPlus   = 4,
        /// <summary>
        /// Indicates including the J# language syntax.
        /// </summary>
        JSharp      = 8,
        /// <summary>
        /// Indicates including the JScript.NET language syntax.
        /// </summary>
        JScript     = 16,
        /// <summary>
        /// Indicates including the standard JavaScript language syntax.
        /// </summary>
        JavaScript  = 32,
        /// <summary>
        /// Indicates including the XAML language syntax.
        /// </summary>
        Xaml        = 64,
        /// <summary>
        /// Indicates including the .NET standard languages (C#, VB.NET and C++/CLI) syntax.
        /// </summary>
        Standard    = CSharp | VisualBasic | CPlusPlus,
        /// <summary>
        /// Indicates including the all the supported languages (and the usages) syntax.
        /// </summary>
        All         = CSharp | VisualBasic | CPlusPlus | JSharp | JScript | JavaScript | Xaml
    }
}
