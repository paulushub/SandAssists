using System;
using System.Xml;

namespace Sandcastle.Utilities
{
    [Serializable]
    public class ProcessInfo : BuildObject<ProcessInfo>
    {
        #region Private Fields

        private bool   _errorDialog;
        private bool   _waitForExit;
        private bool   _createNoWindow;
        private bool   _redirectError;
        private bool   _redirectOutput;
        private bool   _useShellExecute;

        private string _arguments;
        private string _verb;
        private string _workingDir;
        private string _domainName;
        private string _fileName;
        private string _userName;

        private string _userTag;

        #endregion

        #region Constructors and Destructor

        // Summary:
        //     Initializes a new instance of the ProcessInfo class
        //     without specifying a file name with which to start the process.
        public ProcessInfo()
            : this(null, null)
        {   
        }

        //
        // Summary:
        //     Initializes a new instance of the ProcessInfo class
        //     and specifies a file name such as an application or document with which to
        //     start the process.
        //
        // Parameters:
        //   fileName:
        //     An application or document with which to start a process.
        public ProcessInfo(string fileName)
            : this(fileName, null)
        {

        }

        //
        // Summary:
        //     Initializes a new instance of the ProcessInfo class,
        //     specifies an application file name with which to start the process, and specifies
        //     a set of command-line arguments to pass to the application.
        //
        // Parameters:
        //   fileName:
        //     An application with which to start a process.
        //
        //   arguments:
        //     Command-line arguments to pass to the application when the process starts.
        public ProcessInfo(string fileName, string arguments)
        {
            _redirectError   = true;
            _redirectOutput  = true;
            _useShellExecute = true;

            _verb            = String.Empty;
            _fileName        = fileName;
            _arguments       = arguments;
            _workingDir      = String.Empty;

            if (_fileName == null)
            {
                _fileName = String.Empty;
            }
            if (_arguments == null)
            {
                _arguments = String.Empty;
            }
        }

        public ProcessInfo(ProcessInfo source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        // Summary:
        //     Gets or sets the set of command-line arguments to use when starting the application.
        //
        // Returns:
        //     File type–specific arguments that the system can associate with the application
        //     specified in the ProcessInfo.FileName property. The
        //     default is an empty string (""). The maximum string length is 2,003 characters
        //     in .NET Framework applications and 488 characters in .NET Compact Framework
        //     applications.
        public string Arguments 
        { 
            get
            {
                return _arguments;
            }

            set
            {
                if (value == null)
                {
                    _arguments = String.Empty;
                }
                else
                {
                    _arguments = value;
                }
            }
        }

        //
        // Summary:
        //     Gets or sets a value indicating whether to start the process in a new window.
        //
        // Returns:
        //     true to start the process without creating a new window to contain it; otherwise,
        //     false. The default is false.
        public bool CreateNoWindow 
        {
            get
            {
                return _createNoWindow;
            }

            set
            {
                _createNoWindow = value;
            }
        }

        //
        // Summary:
        //     Gets or sets a value that identifies the domain to use when starting the
        //     process.
        //
        // Returns:
        //     The Active Directory domain to use when starting the process. The domain
        //     property is primarily of interest to users within enterprise environments
        //     that use Active Directory.
        public string Domain 
        {
            get
            {
                return _domainName;
            }

            set
            {
                _domainName = value;
            }
        }

        //
        // Summary:
        //     Gets or sets a value indicating whether an error dialog box is displayed
        //     to the user if the process cannot be started.
        //
        // Returns:
        //     true to display an error dialog box on the screen if the process cannot be
        //     started; otherwise, false.
        public bool ErrorDialog 
        { 
            get
            {
                return _errorDialog;
            }

            set
            {
                _errorDialog = value;
            }
        }

        //
        // Summary:
        //     Gets or sets the application or document to start.
        //
        // Returns:
        //     The name of the application to start, or the name of a document of a file
        //     type that is associated with an application and that has a default open action
        //     available to it. The default is an empty string ("").
        public string FileName 
        {
            get
            {
                return _fileName;
            }

            set
            {
                if (value == null)
                {
                    _fileName = String.Empty;
                }
                else
                {
                    _fileName = value;
                }
            }
        }

        //
        // Summary:
        //     Gets or sets a value that indicates whether the error output of an application
        //     is written to the Process.StandardError stream.
        //
        // Returns:
        //     true to write error output to Process.StandardError; otherwise,
        //     false.
        public bool RedirectError 
        {
            get
            {
                return _redirectError;
            }

            set
            {
                _redirectError = value;
            }
        }

        //
        // Summary:
        //     Gets or sets a value that indicates whether the output of an application
        //     is written to the Process.StandardOutput stream.
        //
        // Returns:
        //     true to write output to Process.StandardOutput; otherwise,
        //     false.
        public bool RedirectOutput 
        { 
            get
            {
                return _redirectOutput;
            }

            set
            {
                _redirectOutput = value;
            }
        }

        //
        // Summary:
        //     Gets or sets the user name to be used when starting the process.
        //
        // Returns:
        //     The user name to use when starting the process.
        public string UserName 
        {
            get
            {
                return _userName;
            }

            set
            {
                _userName = value;
            }
        }

        public string UserTag
        {
            get
            {
                return _userTag;
            }

            set
            {
                _userTag = value;
            }
        }

        //
        // Summary:
        //     Gets or sets a value indicating whether to use the operating system shell
        //     to start the process.
        //
        // Returns:
        //     true to use the shell when starting the process; otherwise, the process is
        //     created directly from the executable file. The default is true.
        public bool UseShellExecute 
        { 
            get
            {
                return _useShellExecute;
            }

            set
            {
                _useShellExecute = value;
            }
        }

        //
        // Summary:
        //     Gets or sets the verb to use when opening the application or document specified
        //     by the ProcessInfo.FileName property.
        //
        // Returns:
        //     The action to take with the file that the process opens. The default is an
        //     empty string ("").
        public string Verb 
        {
            get
            {
                return _verb;
            }

            set
            {
                if (value == null)
                {
                    _verb = String.Empty;
                }
                else
                {
                    _verb = value;
                }
            }
        }

        //
        // Summary:
        //     Gets or sets the initial directory for the process to be started.
        //
        // Returns:
        //     The fully qualified name of the directory that contains the process to be
        //     started. The default is an empty string ("").
        public string WorkingDirectory 
        {
            get
            {
                return _workingDir;
            }

            set
            {
                if (value == null)
                {
                    _workingDir = String.Empty;
                }
                else
                {
                    _workingDir = value;
                }
            }
        }

        //
        // Summary:
        //     Gets or sets a value instructing the Process to wait indefinitely for the associated process to exit.
        //
        // Returns:
        //     true to wait indefinitely for the process to end; otherwise,
        //     false. The default is false.
        public bool WaitForExit
        {
            get
            {
                return _waitForExit;
            }

            set
            {
                _waitForExit = value;
            }
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
        }

        public override void WriteXml(XmlWriter writer)
        {
        }

        #endregion

        #region ICloneable Members

        public override ProcessInfo Clone()
        {
            ProcessInfo style = new ProcessInfo(this);

            return style;
        }

        #endregion
    }
}
