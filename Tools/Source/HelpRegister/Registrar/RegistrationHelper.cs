using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Sandcastle.HelpRegister
{
    public sealed class RegistrationHelper : IRegistrationHelper
    {
        #region Private Fields

        private static RegistrationHelper _handler = new RegistrationHelper();
        private bool _ignoreExceptions;
        private List<IRegistrationHelper> _listHandlers;

        #endregion

        #region Constructors and Destructor

        private RegistrationHelper()
        {
            _ignoreExceptions = true;
            _listHandlers     = new List<IRegistrationHelper>();
        }

        #endregion

        #region Public Properties

        public bool IgnoreExceptions
        {
            get 
            { 
                return _ignoreExceptions; 
            }
            set 
            { 
                _ignoreExceptions = value; 
            }
        }

        public IEnumerator<IRegistrationHelper> Helpers
        {
            get 
            {
                if (_listHandlers != null)
                {
                    return _listHandlers.GetEnumerator();
                }

                return null; 
            }
        }

        #endregion

        #region Public Static Properties

        public static RegistrationHelper Instance
        {
            get
            {
                return _handler;
            }
        }  

        #endregion

        #region IRegistrationHelper Members

        public void CloseViewers()
        {
            if (_listHandlers == null || _listHandlers.Count == 0)
            {
                return;
            }
            int itemCount = _listHandlers.Count;
            for (int i = 0; i < itemCount; i++)
            {
                IRegistrationHelper handler = _listHandlers[i];
                if (handler != null)
                {
                    handler.CloseViewers();
                }
            }
        }

        public void DisplayLogo()
        {
        }

        public void DisplayHelp()
        {
        }

        public void WriteLine()
        {
            if (_listHandlers == null || _listHandlers.Count == 0)
            {
                return;
            }
            int itemCount = _listHandlers.Count;
            for (int i = 0; i < itemCount; i++)
            {
                IRegistrationHelper handler = _listHandlers[i];
                if (handler != null)
                {
                    handler.WriteLine();
                }
            }
        }

        public void Write(string text)
        {
            if (String.IsNullOrEmpty(text) == true || 
                _listHandlers == null || _listHandlers.Count == 0)
            {
                return;
            }
            int itemCount = _listHandlers.Count;
            for (int i = 0; i < itemCount; i++)
            {
                IRegistrationHelper handler = _listHandlers[i];
                if (handler != null)
                {
                    handler.Write(text);
                }
            }
        }

        public void WriteLine(string text)
        {
            if (_listHandlers == null || _listHandlers.Count == 0)
            {
                return;
            }
            int itemCount = _listHandlers.Count;
            for (int i = 0; i < itemCount; i++)
            {
                IRegistrationHelper handler = _listHandlers[i];
                if (handler != null)
                {
                    if (String.IsNullOrEmpty(text))
                    {
                        handler.WriteLine();
                    }
                    else
                    {
                        handler.WriteLine(text);
                    }
                }
            }
        }

        public void WriteLine(Exception exception)
        {
            if (_ignoreExceptions || exception == null || 
                _listHandlers == null || _listHandlers.Count == 0)
            {
                return;
            }
            int itemCount = _listHandlers.Count;
            for (int i = 0; i < itemCount; i++)
            {
                IRegistrationHelper handler = _listHandlers[i];
                if (handler != null)
                {
                    handler.WriteLine();
                }
            }
        }

        public void WriteLine(COMException exception)
        {
            if (_ignoreExceptions || exception == null || 
                _listHandlers == null || _listHandlers.Count == 0)
            {
                return;
            }
            int itemCount = _listHandlers.Count;
            for (int i = 0; i < itemCount; i++)
            {
                IRegistrationHelper handler = _listHandlers[i];
                if (handler != null)
                {
                    handler.WriteLine(exception);
                }
            }
        }

        #endregion

        #region Public Methods

        public void Add(IRegistrationHelper handler)
        {
            if (handler == null || _listHandlers == null)
            {
                return;
            }

            _listHandlers.Add(handler);
        }

        public void Remove(IRegistrationHelper handler)
        {
            if (handler == null || _listHandlers == null)
            {
                return;
            }

            _listHandlers.Remove(handler);
        }

        #endregion
    }
}
