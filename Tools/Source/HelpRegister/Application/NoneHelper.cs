using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Sandcastle.HelpRegister
{
    public class NoneHelper : MarshalByRefObject, IRegistrationHelper
    {
        #region Constructors and Destructor

        public NoneHelper()
        {
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
        }

        public void Write(string text)
        {
        }

        public void WriteLine(COMException exception)
        {
        }

        public void WriteLine(Exception exception)
        {
        }

        public void WriteLine(string text)
        {
        }

        public void WriteLine()
        {
        }

        #endregion
    }
}
