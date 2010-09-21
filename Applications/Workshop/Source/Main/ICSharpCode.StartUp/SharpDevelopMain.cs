// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3559 $</version>
// </file>

using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Configuration;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Sda;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop
{
    /// <summary>
    /// This Class is the Core main class, it starts the program.
    /// </summary>
    public static class SharpDevelopMain
    {
        static List<string> requestedFileList = new List<string>();
        static List<string> parameterList = new List<string>();

        static string[] commandLineArgs = null;

        public static string[] CommandLineArgs
        {
            get
            {
                return commandLineArgs;
            }
        }

        public static string[] GetParameterList()
        {
            return parameterList.ToArray();
        }

        public static string[] GetRequestedFileList()
        {
            return requestedFileList.ToArray();
        }

        public static void SetCommandLineArgs(string[] args)
        {
            requestedFileList.Clear();
            parameterList.Clear();

            foreach (string arg in args)
            {
                if (arg.Length == 0) continue;
                if (arg[0] == '-' || arg[0] == '/')
                {
                    int markerLength = 1;

                    if (arg.Length >= 2 && arg[0] == '-' && arg[1] == '-')
                    {
                        markerLength = 2;
                    }

                    parameterList.Add(arg.Substring(markerLength));
                }
                else
                {
                    requestedFileList.Add(arg);
                }
            }
        }

        /// <summary>
        /// Starts the core of SharpDevelop.
        /// </summary>
        [STAThread()]
        public static void Main(string[] args)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                Run(args);
                return;
            }
#endif
            // Do not use LoggingService here (see comment in Run(string[]))
            try
            {
                Run(args);
            }
            catch (Exception ex)
            {
                try
                {
                    HandleMainException(ex);
                }
                catch (Exception loadError)
                {
                    // HandleMainException can throw error when log4net is not found
                    MessageBox.Show(loadError.ToString(), "Critical error (Logging service defect?)");
                }
            }
        }

        static void HandleMainException(Exception ex)
        {
            LoggingService.Fatal(ex);
            try
            {
                Application.Run(new ExceptionBox(ex, "Unhandled exception terminated SharpDevelop", true));
            }
            catch
            {
                MessageBox.Show(ex.ToString(), "Critical error (cannot use ExceptionBox)");
            }
        }

        static void Run(string[] args)
        {
            // DO NOT USE LoggingService HERE!
            // LoggingService requires ICSharpCode.Core.dll and log4net.dll
            // When a method containing a call to LoggingService is JITted, the
            // libraries are loaded.
            // We want to show the SplashScreen while those libraries are loading, so
            // don't call LoggingService.

#if DEBUG
            Control.CheckForIllegalCrossThreadCalls = true;
#endif
            commandLineArgs = args;
            //bool noLogo = false;

            Application.SetCompatibleTextRenderingDefault(false);
            SharpDevelopMain.SetCommandLineArgs(args);

            //foreach (string parameter in SharpDevelopMain.GetParameterList())
            //{
            //    if ("nologo".Equals(parameter, StringComparison.OrdinalIgnoreCase))
            //        noLogo = true;
            //}

            RunApplication();
            //if (!noLogo) {  
            //    SplashScreenForm.ShowSplashScreen();
            //}
            //try {
            //    RunApplication();
            //} finally {
            //    if (SplashScreenForm.SplashScreen != null) {
            //        SplashScreenForm.SplashScreen.Dispose();
            //    }
            //}
        }

        static void RunApplication()
        {
            LoggingService.Info("Starting SandcastleWorkshop...");
            try
            {
                StartupSettings startup = new StartupSettings();
#if DEBUG
                startup.UseSharpDevelopErrorHandler = !Debugger.IsAttached;
#endif

                Assembly executingAsm = typeof(SharpDevelopMain).Assembly;
                startup.ApplicationRootPath = Path.GetDirectoryName(
                    executingAsm.Location);
                //startup.ApplicationRootPath = Path.Combine(Path.GetDirectoryName(
                //    executingAsm.Location), "..");
                startup.AllowUserAddIns = true;

                string configDirectory = ConfigurationManager.AppSettings["settingsPath"];
                if (String.IsNullOrEmpty(configDirectory))
                {
                    startup.ConfigDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                                                           "SandcastleAssist\\Workshop" + RevisionClass.MainVersion);
                }
                else
                {
                    startup.ConfigDirectory = Path.Combine(Path.GetDirectoryName(
                        executingAsm.Location), configDirectory);
                }

                startup.DomPersistencePath = ConfigurationManager.AppSettings["domPersistencePath"];
                if (string.IsNullOrEmpty(startup.DomPersistencePath))
                {
                    startup.DomPersistencePath = Path.Combine(Path.GetTempPath(), "Workshop" + RevisionClass.MainVersion);
#if DEBUG
                    startup.DomPersistencePath = Path.Combine(startup.DomPersistencePath, "Debug");
#endif
                }
                else if (startup.DomPersistencePath == "none")
                {
                    startup.DomPersistencePath = null;
                }

                startup.AddAddInsFromDirectory(Path.Combine(startup.ApplicationRootPath, "AddIns"));

                SharpDevelopHost host = new SharpDevelopHost(AppDomain.CurrentDomain, startup);

                string[] fileList = SharpDevelopMain.GetRequestedFileList();
                if (fileList.Length > 0)
                {
                    if (LoadFilesInPreviousInstance(fileList))
                    {
                        LoggingService.Info("Aborting startup, arguments will be handled by previous instance");
                        return;
                    }
                }

                //host.BeforeRunWorkbench += delegate {
                //    if (SplashScreenForm.SplashScreen != null) {
                //        SplashScreenForm.SplashScreen.BeginInvoke(new MethodInvoker(SplashScreenForm.SplashScreen.Dispose));
                //        SplashScreenForm.SplashScreen = null;
                //    }
                //};

                WorkbenchSettings workbenchSettings = new WorkbenchSettings();
                workbenchSettings.RunOnNewThread = false;
                for (int i = 0; i < fileList.Length; i++)
                {
                    workbenchSettings.InitialFileList.Add(fileList[i]);
                }
                host.RunWorkbench(workbenchSettings);
            }
            finally
            {
                LoggingService.Info("Leaving RunApplication()");
            }
        }

        static bool LoadFilesInPreviousInstance(string[] fileList)
        {
            try
            {
                foreach (string file in fileList)
                {
                    if (ProjectService.HasProjectLoader(file))
                    {
                        return false;
                    }
                }
                return SingleInstanceHelper.OpenFilesInPreviousInstance(fileList);
            }
            catch (Exception ex)
            {
                LoggingService.Error(ex);
                return false;
            }
        }
    }
}
