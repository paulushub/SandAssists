using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

using Microsoft.Win32;
using Sandcastle.Utilities;

namespace Sandcastle
{
    [Serializable]
    public sealed class BuildFramework : BuildObject<BuildFramework>
    {
        #region Private Fields

        private string             _assemblyDir;  
        private Version            _version;
        private IList<string>      _commentDirs;
        private BuildFrameworkType _frameworkType;

        #endregion

        #region Constructor and Destructor

        public BuildFramework()
        {
            _frameworkType = BuildFrameworkType.Framework20;
            _version       = new Version(2, 0, 50727, 1433);
            _assemblyDir   = Environment.ExpandEnvironmentVariables(
                            @"%SystemRoot%\Microsoft.NET\Framework\v" + _version.ToString(3));

            _commentDirs = new List<string>();
            _commentDirs.Add(_assemblyDir);

            // Check if F# 2.0 is installed...
            string fSharpDir = Path.Combine(PathUtils.ProgramFiles32,
                @"Reference Assemblies\Microsoft\FSharp\2.0\Runtime\v2.0");

            if (Directory.Exists(fSharpDir))
            {
                _commentDirs.Add(fSharpDir);
            }
        }

        public BuildFramework(BuildFrameworkType type, IList<string> commentDirs, 
            Version version) : this()
        {
            if (type == BuildFrameworkType.Null ||
                type == BuildFrameworkType.None)
            {
                throw new ArgumentException("The framework type must be valid.");
            }

            BuildExceptions.NotNull(version, "version");
            BuildExceptions.NotNull(commentDirs, "commentDirs");

            _version       = version;
            _commentDirs   = commentDirs;
            _frameworkType = type;
        }

        public BuildFramework(BuildFrameworkType type, string assemblyDir, 
            IList<string> commentDirs, Version version)
            : this(type, commentDirs, version)
        {
            BuildExceptions.NotNullNotEmpty(assemblyDir, "assemblyDir");

            _assemblyDir   = assemblyDir;
        }

        public BuildFramework(BuildFramework source)
            : base(source)
        {
            _version       = source._version;
            _assemblyDir   = source._assemblyDir;
            _commentDirs   = source._commentDirs;
            _frameworkType = source._frameworkType;
        }

        #endregion

        #region Public Properties

        public BuildFrameworkType FrameworkType
        {
            get
            {
                return _frameworkType;
            }
        }

        public string Folder
        {
            get 
            { 
                if (_version != null)
                {
                    switch (_version.Major)
                    {
                        case 1:
                            return "v" + _version.ToString(3);
                        case 2:
                            return "v" + _version.ToString(3);
                        case 3:
                            Version version = BuildFrameworks.GetVersion(2,
                                _frameworkType.IsSilverlight);
                            if (version == null)
                            {
                                return "v2.0.50727"; // not expected...
                            }
                            return "v" + _version.ToString(3);
                        case 4:
                            return "v" + _version.ToString(3);
                    }
                }

                return String.Empty; 
            }
        }

        public Version Version
        {
            get 
            { 
                return _version; 
            }
        }

        public string AssemblyDir
        {
            get
            {
                return _assemblyDir;
            }
        }

        public IList<string> CommentDirs
        {
            get
            {
                return _commentDirs;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region ICloneable Members

        public override BuildFramework Clone()
        {
            BuildFramework framework = new BuildFramework(this);

            return framework;
        }

        #endregion
    }
}
