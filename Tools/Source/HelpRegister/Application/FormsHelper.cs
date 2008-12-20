using System;
using System.Text;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Sandcastle.HelpRegister
{
    public class FormsHelper : MarshalByRefObject, IRegistrationHelper
    {
        #region Private Fields

        private delegate void TextMessageHandler(string message);
        private delegate void ExceptionMessageHandler(Exception exception);

        private RegistrationOptions _options;
        private MainForm _mainForm;

        #endregion

        #region Constructors and Destructor

        public FormsHelper()
        {
        }

        public FormsHelper(MainForm mainform, RegistrationOptions options)
        {
            _mainForm = mainform;
            _options  = options;
        }

        #endregion

        #region Public Properties

        public RegistrationOptions Options
        {
            get
            {
                return _options;
            }
        }

        #endregion

        #region IRegistrationHelper Members

        public void CloseViewers()
        {
        }

        public void DisplayLogo()
        {
        }

        public void DisplayHelp()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string helpText = String.Format(ResourcesHelper.GetString(
                "RegisterToolCommandLineOptions"), asm.GetName().Name);

            MessageBox.Show(helpText, "HelpRegister", 
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public void Write(string text)
        {
            if (_mainForm == null)
            {
                return;
            }

            if (_mainForm.InvokeRequired)
            {
                TextMessageHandler handler = new TextMessageHandler(
                    _mainForm.ProgressText);

                _mainForm.Invoke(handler, new object[] { text });
            }
            else
            {
                _mainForm.ProgressText(text);
            }
        }

        public void WriteLine(Exception exception)
        {
            if (_mainForm == null)
            {
                return;
            }

            if (_mainForm.InvokeRequired)
            {
                ExceptionMessageHandler handler = new ExceptionMessageHandler(
                    _mainForm.ProgressException);

                _mainForm.Invoke(handler, new object[] { exception });
            }
            else
            {
                _mainForm.ProgressException(exception);
            }
        }

        public void WriteLine(COMException exception)
        {
            if (_mainForm == null)
            {
                return;
            }

            if (_mainForm.InvokeRequired)
            {
                ExceptionMessageHandler handler = new ExceptionMessageHandler(
                    _mainForm.ProgressException);

                _mainForm.Invoke(handler, new object[] { exception });
            }
            else
            {
                _mainForm.ProgressException(exception);
            }
        }

        public void WriteLine(string text)
        {
            if (_mainForm == null)
            {
                return;
            }

            if (_mainForm.InvokeRequired)
            {
                TextMessageHandler handler = new TextMessageHandler(
                    _mainForm.ProgressText);

                _mainForm.Invoke(handler, new object[] { text });
            }
            else
            {
                _mainForm.ProgressText(text);
            }
        }

        public void WriteLine()
        {
        }

        #endregion
    }
}
