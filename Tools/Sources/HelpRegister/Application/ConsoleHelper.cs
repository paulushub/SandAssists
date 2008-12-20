using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Sandcastle.HelpRegister
{
    public class ConsoleHelper : MarshalByRefObject, IRegistrationHelper
    {
        #region Constructors and Destructor

        public ConsoleHelper()
        {
        }

        #endregion

        #region IRegistrationHelper Members

        public void CloseViewers()
        {
            string[] invalidProcesses = new string[] 
            { 
                "dexplore.exe", "sharpdevelop.exe" 
            };
            ApplicationHelpers.KillProcesses(invalidProcesses);
        }

        public void DisplayLogo()
        {   
            Assembly asm = Assembly.GetExecutingAssembly();
            this.WriteLine();
            this.WriteLine(String.Format("Help 2.0 Registration Utility v{0}",
                asm.GetName().Version.ToString()));
            this.WriteLine(
                "Copyright (c) 2005 - 2008 Mathias Simmack. All rights reserved.");
            this.WriteLine();
        }

        public void DisplayHelp()
        {   
            Assembly asm = Assembly.GetExecutingAssembly();
            this.WriteLine();
            this.WriteLine(String.Format(ResourcesHelper.GetString(
                "RegisterToolCommandLineOptions"), asm.GetName().Name));
            this.WriteLine();
        }

        public void Write(string text)
        {
            Console.Write(text);
        }

        public void WriteLine(COMException exception)
        {
            if (exception == null)
            {
                return;
            }
            Console.WriteLine(exception.ToString());
        }

        public void WriteLine(Exception exception)
        {
            if (exception == null)
            {
                return;
            }
            Console.WriteLine(exception.ToString());
        }

        public void WriteLine(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine(text);
            }
        }

        public void WriteLine()
        {
            Console.WriteLine();
        }

        #endregion
    }
}
