using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

namespace Sandcastle.Steps
{
    public class StepHxsViewerStart : BuildStep
    {
        #region Private Fields

        private bool   _separateIndex;
        private string _registrar;
        private string _namespace;
        private string _titleId;
        private string _htmlHelpFile;
        private string _helpCollection;

        #endregion

        #region Constructors and Destructor

        public StepHxsViewerStart()
        {
            this.Message    = "Opening compiled HtmlHelp file";
        }

        public StepHxsViewerStart(string workingDir, string htmlHelpFile,
            string helpCollection)
            : base(workingDir)
        {
            _htmlHelpFile   = htmlHelpFile;
            _helpCollection = helpCollection;
            this.Message    = "Opening compiled HtmlHelp file";
        }

        #endregion

        #region Public Properties

        public string Registrar
        {
            get
            {
                return _registrar;
            }
            set
            {
                _registrar = value;
            }
        }

        public bool SeparateIndexFile
        {
            get
            {
                return _separateIndex;
            }
            set
            {
                _separateIndex = value;
            }
        }

        public string HelpNamespace
        {
            get
            {
                return _namespace;
            }
            set
            {
                if (value == null)
                {
                    _namespace = value;
                    return;
                }
                value = value.Trim();

                if (!String.IsNullOrEmpty(value))
                {
                    if (value.IndexOf(' ') >= 0)
                    {
                        throw new BuildException("The help file namespace cannot contain space.");
                    }
                }
                _namespace = value;
            }
        }

        public string HelpTitleId
        {
            get
            {
                return _titleId;
            }
            set
            {
                if (value == null)
                {
                    _titleId = value;
                    return;
                }
                value = value.Trim();

                if (!String.IsNullOrEmpty(value))
                {
                    if (value.IndexOf(' ') >= 0)
                    {
                        throw new BuildException("The help file title ID cannot contain space.");
                    }
                }
                _titleId = value;
            }
        }

        #endregion

        #region Protected Methods

        protected override bool MainExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;
            BuildSettings settings = context.Settings;

            //if (logger != null)
            //{
            //    logger.WriteLine("Opening compiled HtmlHelp file...",
            //        BuildLoggerLevel.Started);
            //}
            if (String.IsNullOrEmpty(_registrar) || 
                File.Exists(_registrar) == false)
            {
                if (logger != null)
                {
                    logger.WriteLine("The help file registration tool is not available.",
                        BuildLoggerLevel.Error);
                }
            }

            if (String.IsNullOrEmpty(_htmlHelpFile) ||
                File.Exists(_htmlHelpFile) == false)
            {
                if (logger != null)
                {
                    logger.WriteLine("The help file is not available.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            // There are three possible installed directories of the Help 2.x viewer,
            // depending on the edition of the Visual Studio installed.
            // We start with the latest edition of the visual studio...
            bool foundViewer = false;
            string viewerPath = null;
            string specialFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.CommonProgramFiles);
            // VS.NET 2008: C:\Program Files\Common Files\Microsoft Shared\Help 9
            viewerPath  = Path.Combine(specialFolder, @"Microsoft Shared\Help 9\dexplore.exe");
            foundViewer = File.Exists(viewerPath);
            if (foundViewer == false)
            {
                // VS.NET 2005: C:\Program Files\Common Files\Microsoft Shared\Help 8
                viewerPath = Path.Combine(specialFolder, @"Microsoft Shared\Help 8\dexplore.exe");
                foundViewer = File.Exists(viewerPath);

                // VS.NET 2003: C:\Program Files\Common Files\Microsoft Shared\Help
                if (foundViewer == false)
                {
                    viewerPath = Path.Combine(specialFolder, @"Microsoft Shared\Help\dexplore.exe");
                    foundViewer = File.Exists(viewerPath);
                }
            }
            if (foundViewer == false)
            {
                if (logger != null)
                {
                    logger.WriteLine("The help file viewer is not available.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            string helpFile = Path.GetFileName(_htmlHelpFile);
            string helpPath = Path.GetDirectoryName(_htmlHelpFile);

            string helpNamespace = _namespace;
            if (helpNamespace != null)
            {
                helpNamespace = helpNamespace.Trim();
            }
            if (String.IsNullOrEmpty(helpNamespace))
            {
                helpNamespace = "SandcastleHelpers";
            }
            string helpTitleId = _titleId;
            if (helpTitleId != null)
            {
                helpTitleId = helpTitleId.Trim();
            }
            if (String.IsNullOrEmpty(helpTitleId))
            {
                helpTitleId = Path.GetFileNameWithoutExtension(_htmlHelpFile);
            }

            //Usage: HxReg [switches] | HxReg <Help filename .HxS>
            //     -n <namespace>
            //     -i <title ID>
            //     -c <collection Name .HxC | .HxS>
            //     -d <namespace description>
            //     -s <Help filename .HxS>
            //     -x <Help Index filename .HxI>
            //     -q <Help Collection Combined FTS filename .HxQ>
            //     -t <Help Collection Combined Attribute Index filename .HxR>
            //     -l <language ID>
            //     -a <alias>
            //     -f <filename listing HxReg commands>
            //     -r Remove a namespace, Help title, or alias
            //
            //EXAMPLES
            //To register a namespace:
            //     HxReg -n <namespace> -c <collection filename> -d <namespace description>
            //To register a Help file:
            //     HxReg -n <namespace> -i <title id> -s <HxS filename>

            // For now we will run the following...
            // HxReg -n <namespace> -c HelpFile.HxS  
            // HxReg -n <namespace> -i <title ID> -s HelpFile.HxS
            // 
            // Also: http://msdn.microsoft.com/en-us/library/bb165615(VS.80).aspx
            try
            {
                string arguments = null;
                if (String.IsNullOrEmpty(_helpCollection) == false &&
                    File.Exists(_helpCollection))
                {
                    string helpColl = Path.GetFileName(_helpCollection);
                    arguments = String.Format(" -n {0} -c {1} -d \"Sandcastle Documentations\"",
                        helpNamespace, helpColl);
                }
                else
                {
                    arguments = String.Format(" -n {0} -c {1} -d \"Sandcastle Documentations\"",
                        helpNamespace, helpFile);
                }

                Process hxregProcess = new Process();
                ProcessStartInfo hxregInfo = hxregProcess.StartInfo;
                hxregInfo.FileName = _registrar;
                hxregInfo.Arguments = arguments;
                hxregInfo.WorkingDirectory = helpPath;
                hxregInfo.WindowStyle = ProcessWindowStyle.Hidden;
                hxregInfo.ErrorDialog = false;

                if (logger != null)
                {
                    logger.WriteLine(
                        "Registering the Help 2.x namespace: " + helpNamespace, 
                        BuildLoggerLevel.Info);
                }

                hxregProcess.Start();

                hxregProcess.WaitForExit();

                if (hxregProcess.ExitCode != 0)
                {
                    if (logger != null)
                    {
                        logger.WriteLine(
                            "Registration of the Help 2.x namespace failed.", 
                            BuildLoggerLevel.Error);
                    }
                    hxregProcess.Close();

                    return false;
                }
                else
                {
                    if (logger != null)
                    {
                        logger.WriteLine(
                            "Registration of the Help 2.x namespace successful.", 
                            BuildLoggerLevel.Info);
                    }
                }
                arguments = String.Format(" -n {0} -i {1} -s {2}",
                    helpNamespace, helpTitleId, helpFile);
                hxregInfo.Arguments = arguments;

                if (logger != null)
                {
                    logger.WriteLine(
                        "Registering the Help 2.x unique identifier: " + helpTitleId, 
                        BuildLoggerLevel.Info);
                }

                hxregProcess.Start();

                hxregProcess.WaitForExit();

                if (hxregProcess.ExitCode != 0)
                {
                    if (logger != null)
                    {
                        logger.WriteLine(
                            "Registration of the Help 2.x unique identifier failed.", 
                            BuildLoggerLevel.Error);
                    }
                    hxregProcess.Close();

                    return false;
                }
                else
                {
                    if (logger != null)
                    {
                        logger.WriteLine(
                            "Registration of the Help 2.x unique identifier successful.", 
                            BuildLoggerLevel.Info);
                    }
                }

                if (_separateIndex)
                {
                    string indexFile = Path.ChangeExtension(_htmlHelpFile, ".HxI");
                    if (File.Exists(indexFile))
                    {
                        indexFile = Path.GetFileName(indexFile);
                    }
                    arguments = String.Format(" -n {0} -i {1} -s {2} -x {3}",
                        helpNamespace, helpTitleId, helpFile, indexFile);
                    hxregInfo.Arguments = arguments;

                    if (logger != null)
                    {
                        logger.WriteLine(
                            "Registering the Help 2.x index file: " + indexFile,
                            BuildLoggerLevel.Info);
                    }

                    hxregProcess.Start();

                    hxregProcess.WaitForExit();

                    if (hxregProcess.ExitCode != 0)
                    {
                        if (logger != null)
                        {
                            logger.WriteLine(
                                "Registration of the Help 2.x index file failed.",
                                BuildLoggerLevel.Error);
                        }
                        hxregProcess.Close();

                        return false;
                    }
                    else
                    {
                        if (logger != null)
                        {
                            logger.WriteLine(
                                "Registration of the Help 2.x index file successful.",
                                BuildLoggerLevel.Info);
                        }
                    }
                }    
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }

                return false;
            }

            if (logger != null)
            {
                logger.WriteLine("Opening: " + _htmlHelpFile, BuildLoggerLevel.Info);
            }

            bool isStarted = false;
            try
            {
                CultureInfo culture = settings.CultureInfo;
                int lcid = 1033;
                if (culture != null)
                {
                    lcid = culture.LCID;
                }

                // dexplore.exe /helpcol ms-help://<namespace>/<title ID> /LCID <locale ID> /LaunchNamedUrlTopic DefaultPage
                string runOptions = String.Format(
                    " /helpcol \"ms-help://{0}/{1}\" /LCID {2} /LaunchNamedUrlTopic DefaultPage", 
                    helpNamespace, helpTitleId, lcid);
                Process process = Process.Start(viewerPath, runOptions);

                isStarted = (process != null);
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            //if (logger != null)
            //{
            //    logger.WriteLine("Opening compiled HtmlHelp file.",
            //        BuildLoggerLevel.Ended);
            //}

            return isStarted;
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
