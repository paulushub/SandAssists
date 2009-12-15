// <copyright>
// Portions of this file are based on the sources of Sandcastle ChmBuilder.exe tool. 
// Copyright (c) Microsoft Corporation.  All rights reserved.
// <copyright>

using System;

namespace Sandcastle.Formats
{
    internal sealed class FormatChmOptions
    {
        #region Private Fields

        private int    _langId;
        private bool   _includeMetadata;

        private string _tocFile;
        private string _tocStyle;
        private string _configFile;
        private string _projectName;
        private string _htmlDirectory;
        private string _outputDirectory;
        private string _workingDirectory;

        #endregion

        #region Constructors and Destructor

        public FormatChmOptions()
        {
            _langId  = 1033;
            _tocFile = String.Empty;
        }

        #endregion

        #region Public Properties

        public string ConfigFile
        {
            get 
            { 
                return _configFile; 
            }
            set 
            { 
                _configFile = value; 
            }
        }

        public string HtmlDirectory
        {
            get 
            { 
                return _htmlDirectory; 
            }
            set 
            { 
                _htmlDirectory = value; 
            }
        }

        public int LangID
        {
            get 
            { 
                return _langId; 
            }
            set 
            { 
                _langId = value; 
            }
        }

        public bool Metadata
        {
            get 
            { 
                return _includeMetadata; 
            }
            set 
            { 
                _includeMetadata = value; 
            }
        }

        public string OutputDirectory
        {
            get 
            { 
                return _outputDirectory; 
            }
            set 
            { 
                _outputDirectory = value; 
            }
        }

        public string WorkingDirectory
        {
            get
            {
                return _workingDirectory;
            }
            set
            {
                _workingDirectory = value;
            }
        }

        public string ProjectName
        {
            get 
            { 
                return _projectName; 
            }
            set 
            { 
                _projectName = value; 
            }
        }

        public string TocFile
        {
            get 
            { 
                return _tocFile; 
            }
            set 
            { 
                _tocFile = value; 
            }
        }

        public string TocStyle
        {
            get 
            { 
                return _tocStyle; 
            }
            set 
            { 
                _tocStyle = value; 
            }
        }

        #endregion
    }
}
