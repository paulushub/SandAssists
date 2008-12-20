using System;
using System.Text;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using Microsoft.Win32;

namespace Sandcastle.HelpRegister
{
    class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AllocConsole();
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool FreeConsole();
        [DllImport("kernel32", SetLastError = true)]
        static extern bool AttachConsole(int dwProcessId);
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd,
            out int lpdwProcessId);        

        static int Main(string[] args)
        {
            // 1. Parse the commandline options to determine the requested task...
            RegistrationOptions options = new RegistrationOptions();
            bool argValid = options.Parse(args);

            // 2. Get a pointer to the forground window.  The idea here is that                
            // If the user is starting our application from an existing console                
            // shell, that shell will be the uppermost window.  We'll get it                
            // and attach to it.                
            // Uses this idea from, Jeffrey Knight, since it fits our model instead
            // of the recommended ATTACH_PARENT_PROCESS (DWORD)-1 parameter
            bool startedInConsole = false;
            IntPtr ptr    = GetForegroundWindow();
            int processId = -1;
            GetWindowThreadProcessId(ptr, out processId);
            Process process = Process.GetProcessById(processId);

            startedInConsole = process != null && String.Equals(process.ProcessName, 
                "cmd", StringComparison.InvariantCultureIgnoreCase);

            // 3. If the command option is invalid, or we just needed to show help...
            int returnValue = -1;
            if (!argValid || options.ShowHelp)
            {
                IRegistrationHelper helper = null;

                bool consoleSuccess = false;
                if (startedInConsole)
                {
                    consoleSuccess = CreateConsole(startedInConsole, process);
                    helper = new ConsoleHelper();
                }
                else
                {
                    Application.EnableVisualStyles();
                    Application.SetCompatibleTextRenderingDefault(false);

                    helper = new FormsHelper();
                }
                if (helper != null)
                {
                    helper.DisplayHelp();
                }
                if (consoleSuccess)
                {
                    FreeConsole();
                }

                return returnValue;
            }

            // 4. If requested to open the help file, process it...
            if (options.IsViewer)
            {
                bool consoleSuccess = false;
                IRegistrationHelper helper = null;

                try
                {
                    if (startedInConsole)
                    {
                        consoleSuccess = CreateConsole(startedInConsole, process);
                        helper = new ConsoleHelper();
                    }
                    else
                    {
                        helper = new NoneHelper();
                    }

                    RegistrationHandler register = new RegistrationHandler();

                    returnValue = register.Run(options, helper);
                }
                catch (Exception ex)
                {
                    if (startedInConsole)
                    {   
                        if (helper != null)
                        {
                            helper.WriteLine(ex);
                        }
                    }
                    else
                    {
                        MessageBox.Show(ex.ToString());
                    }
                }
                if (consoleSuccess)
                {
                    FreeConsole();
                }

                return returnValue;
            }

            // If started from the console, we will ignore any GUI request...
            if (startedInConsole && !options.IsConsole)
            {
                options.SetMode(startedInConsole);
            }

            if (options.IsConsole)
            {
                bool consoleSuccess = false;
                try
                {
                    consoleSuccess = CreateConsole(startedInConsole, process);

                    if (consoleSuccess)
                    {
                        ConsoleHelper helper = new ConsoleHelper();

                        RegistrationHandler register = new RegistrationHandler();

                        returnValue = register.Run(options, helper);
                    }
                }
                catch
                {
                    FormsHelper helper = new FormsHelper();
                    helper.DisplayHelp();
                }                       
                finally
                {
                    if (consoleSuccess)
                    {
                        FreeConsole();
                    }
                }
            }
            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                MainForm mainForm = new MainForm();

                mainForm.SetOptions(options);

                Application.Run(mainForm);

                returnValue = mainForm.Result;
            }

            return returnValue;
        }

        static bool CreateConsole(bool startedInConsole, Process process)
        {
            bool consoleSuccess = false;
            // If the uppermost window a cmd process...
            if (startedInConsole)
            {
                consoleSuccess = AttachConsole(process.Id);
                if (!consoleSuccess)
                {
                    consoleSuccess = AllocConsole();
                    if (consoleSuccess)
                    {
                        Console.Title = "Help 2.0 Registration";
                    }
                }
            }
            else
            {
                consoleSuccess = AllocConsole();
                if (consoleSuccess)
                {
                    Console.Title = "Help 2.0 Registration";
                }
            }

            return consoleSuccess;
        }
    }
}
