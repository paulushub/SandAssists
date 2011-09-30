using System;

namespace Sandcastle.Construction.ProjectSections
{
    /// <summary>
    /// Specifies the type of the standard <c>MSBuild</c> project.
    /// </summary>
    [Serializable]
    public enum StandardProjectType
    {
        /// <summary>
        /// Indicates an unknown or unspecified project type.
        /// </summary>
        None    = 0,
        /// <summary>
        /// Indicates the <c>Visual C#</c> project type.
        /// </summary>
        CsProj  = 1,
        /// <summary>
        /// <para>
        /// Indicates the <c>Visual Basic .NET</c> project type.
        /// </para>
        /// <para>
        /// Unique to this project type is that the documentation file is fixed and
        /// not subject to user modifications, and it is defined relative to the
        /// output directory, not the project file directory.
        /// </para>
        /// </summary>
        VbProj  = 2,
        /// <summary>
        /// <para>
        /// Indicates the <c>Visual J#</c> project type.
        /// </para>
        /// <para>
        /// This is not supported in <c>VS.NET 2008</c> or later.
        /// </para>
        /// </summary>
        VjsProj = 3,
        /// <summary>
        /// Indicates the <c>Visual F#</c> project type.
        /// </summary>
        FsProj  = 4,
        /// <summary>
        /// <para>
        /// Indicates the <c>Iron Python</c> project type.
        /// </para>
        /// <para>
        /// This does not support <c>XML</c> documentations, but could be used as
        /// reference project.
        /// </para>
        /// </summary>
        PyProj  = 5,
        /// <summary>
        /// <para>
        /// Indicates the <c>Iron Ruby</c> project type.
        /// </para>
        /// <para>
        /// This does not support <c>XML</c> documentations, but could be used as
        /// reference project.
        /// </para>
        /// </summary>
        RbProj  = 6
    }
}
