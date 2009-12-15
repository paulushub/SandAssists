using System;

namespace Sandcastle.Formats
{
    public sealed class FormatWebOptions
    {
        #region Private Fields

        private string _tocFile;
        private string _helpName;
        private string _helpTitle;
        private string _projectName;
        private string _htmlFolder;
        private string _htmlDirectory;
        private string _outputDirectory;
        private string _workingDirectory;

        #endregion

        #region Constructors and Destructor

        public FormatWebOptions()
        {
            _htmlFolder = "html";
        }

        #endregion

        #region Public Properties

        public string HelpName
        {
            get 
            { 
                return _helpName; 
            }
            set 
            {
                _helpName = value; 
            }
        }

        public string HelpTitle
        {
            get
            {
                return _helpTitle;
            }
            set
            {
                _helpTitle = value;
            }
        }

        public string HelpTocFile
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

        public string HtmlFolder
        {
            get 
            { 
                return _htmlFolder; 
            }
            set 
            { 
                _htmlFolder = value; 
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

        #endregion
    }
}
