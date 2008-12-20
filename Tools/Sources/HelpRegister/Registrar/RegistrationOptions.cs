//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace Sandcastle.HelpRegister
{
    [Serializable]
    public sealed class RegistrationOptions
    {
        #region Private Fields

        private int    _argCount;
        private bool   _isConsole;
        private bool   _showLogo;
        private bool   _showHelp;

        private bool   _beQuiet;
        private bool   _viewHelp;
        private bool   _errorCodes;
        private string _actionParam;
        private string _xmlFilename;
        private string _xpathSequence;

        #endregion

        #region Constructors and Destructor

        public RegistrationOptions()
        {
            _isConsole     = true;
            _showLogo      = true;
            _actionParam   = String.Empty;
            _xmlFilename   = String.Empty;
            _xpathSequence = String.Empty;
        }

        #endregion

        #region Public Properties

        public int Count
        {
            get
            {
                return _argCount;
            }
        }

        public bool IsConsole
        {
            get
            {
                return _isConsole;
            }
        }

        public bool ShowLogo
        {
            get
            {
                return _showLogo;
            }
        }

        public bool ShowHelp
        {
            get 
            {
                if (_actionParam == "/?" || _actionParam == "-?")
                {
                    return true;
                }

                return _showHelp; 
            }
        }

        public bool ViewHelp
        {
            get
            {
                return _viewHelp;
            }
        }

        public bool IsQuiet
        {
            get 
            { 
                return _beQuiet; 
            }
        }

        public bool IsViewer
        {
            get
            {
                if (String.IsNullOrEmpty(_actionParam))
                {
                    return false;
                }

                return (_actionParam == "/v" || _actionParam == "-v");
            }
        }

        public bool ErrorCodes
        {
            get 
            { 
                return _errorCodes; 
            }
        }

        public string ActionParam
        {
            get 
            { 
                return _actionParam; 
            }
        }

        public string FileName
        {
            get 
            { 
                return _xmlFilename; 
            }
        }

        public string XPathSequence
        {
            get 
            { 
                return _xpathSequence; 
            }
        }

        #endregion

        #region Public Methods

        public void SetMode(bool isConsole)
        {
            _isConsole = isConsole;
        }

        public bool Parse(string[] args)
        {
            int itemCount = args.Length;

            _argCount = itemCount;

            if (itemCount == 0)
            {
                return false;
            }

            _actionParam = args[0].ToLower(CultureInfo.InvariantCulture);
            if (_actionParam == "/?" || _actionParam == "-?")
            {
                _showHelp = true;

                return false;
            }
            _xmlFilename = args[itemCount - 1];
            if (!String.IsNullOrEmpty(_xmlFilename) && !String.IsNullOrEmpty(
                Path.GetExtension(_xmlFilename)))
            {   
                // it contains the configuration file...
                itemCount--;
            }
            else
            {
                _xmlFilename = null;
            }

            for (int i = 1; i < itemCount; i++)
            {
                string arg = args[i];

                if (String.IsNullOrEmpty(arg))
                {
                    continue;
                }

                if ('-' == arg[0] || '/' == arg[0])
                {
                    string parameter = arg.Substring(1);

                    if (String.Equals("nologo", parameter,
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        _showLogo = false;
                    }
                    if ("?" == parameter || String.Equals("help", parameter,
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        _showHelp = true;
                    }
                    if (String.Equals("quiet", parameter, 
                        StringComparison.InvariantCultureIgnoreCase) || 
                        "q" == parameter)
                    {
                        _beQuiet = true;
                    }
                    if (parameter.StartsWith("xpath:",
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        _xpathSequence = parameter.Substring(6);
                    }
                    if (String.Equals("useerrorcodes", parameter, 
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        _errorCodes = true;
                    }
                    if (String.Equals("gui", parameter,
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        _isConsole = false;
                    }
                    if (String.Equals("view", parameter,
                        StringComparison.InvariantCultureIgnoreCase))
                    {
                        _viewHelp = true;
                    }
                }
            }

            return true;
        }

        #endregion
    }
}
