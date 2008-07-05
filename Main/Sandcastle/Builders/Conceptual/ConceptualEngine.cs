using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Utilities;
using Sandcastle.Builders.Tasks;

namespace Sandcastle.Builders.Conceptual
{
    public class ConceptualEngine : BuildEngine
    {
        #region Private Fields

        private bool              _isInitialized;
        private List<BuildTask>   _listTasks;
        private ConceptualContext _context;

        #endregion

        #region Constructors and Destructor

        public ConceptualEngine()
        {
            _context     = new ConceptualContext();
            _listTasks   = new List<BuildTask>();
        }

        #endregion

        #region Public Properties

        public override bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        public ConceptualContext ConceptualContext
        {
            get
            {
                return _context;
            }
        }

        public override BuildContext Context
        {
            get
            {
                return _context;
            }
        }

        public override IList<BuildTask> Tasks
        {
            get
            {
                return _listTasks;
            }
        }

        #endregion

        #region Public Methods

        #region Initialize Method

        public override void Initialize(BuildContext context)
        {
            this.Initialize((ConceptualContext)context);
        }

        public void Initialize(ConceptualContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context",
                    "The context object cannot be null (or Nothing).");
            }

            _isInitialized = false;
            _context       = null;

            if (this.LoggerCount == 0)
            {
                string logFile = Path.Combine(context.WorkingDirectory, @"Build.log");
                logFile = Path.GetFullPath(logFile);
                if (File.Exists(logFile))
                {
                    try
                    {
                        File.Delete(logFile);
                        this.AddLogger(new HelpConsoleLogger(logFile));
                    }
                    catch
                    {
                    }
                }
                else
                {
                    this.AddLogger(new HelpConsoleLogger(logFile));
                }
            }

            this.Logger.Initialize();

            if (context.ItemsLoaded == false)
            {
                if (context.LoadItems(this.Logger) == false)
                {
                    return;
                }
            }

            if (_listTasks == null || 
                (_listTasks != null && _listTasks.Count != 0))
            {
                _listTasks = new List<BuildTask>();
            }

            _context = context;

            // 1. Create the conceptual contents building task
            TaskConceptualContents taskContents = new TaskConceptualContents(this);
            taskContents.Initialize(null);
            _listTasks.Add(taskContents);

            // 2---TODO
            ProcessInfo startInfo = new ProcessInfo();

            string batchFilePath = null;
            string batchFileName = "Project.bat";
            if (Path.IsPathRooted(batchFileName))
            {
                batchFilePath = batchFileName;
            }
            else
            {
                batchFilePath = Path.Combine(_context.WorkingDirectory, batchFileName);
            }
            if (!File.Exists(batchFilePath))
            {
                return;
            }
            batchFilePath = Path.GetFullPath(batchFilePath);
            startInfo.FileName = batchFilePath;
            startInfo.UseShellExecute = false;
            startInfo.WorkingDirectory = Path.GetDirectoryName(batchFilePath);
            startInfo.RedirectOutput = true;

            TaskRunProcess taskRun = new TaskRunProcess(this, startInfo);
            taskRun.Initialize(null);
            _listTasks.Add(taskRun);  

            // ...Create the task of cleaning up the intermediates
            TaskConceptualCleanUp taskCleanUp = new TaskConceptualCleanUp(this);
            taskCleanUp.Initialize(null);
            _listTasks.Add(taskCleanUp);

            _isInitialized = true;
        }

        #endregion

        #region Build Method

        public override bool Build()
        {
            if (_isInitialized == false)
            {
                return false;
            }

            HelpLogger logger = this.Logger;
            try
            {
                int taskCount = _listTasks.Count;
                if (taskCount == 0)
                {
                    return false;
                }

                for (int i = 0; i < taskCount; i++)
                {
                    BuildTask buildTask = _listTasks[i];
                    if (buildTask != null && buildTask.IsInitialized)
                    {
                        buildTask.Execute();
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                logger.WriteLine(ex, HelpLoggerLevel.Error);

                return false;
            }
        }

        public bool Build(string batchFileName)
        {
            HelpLogger logger = this.Logger;

            if (String.IsNullOrEmpty(batchFileName))
            {
                return false;
            }     
            if (String.IsNullOrEmpty(_context.WorkingDirectory) ||
                !Directory.Exists(_context.WorkingDirectory))
            {
                return false;
            }

            string batchFilePath = null;
            if (Path.IsPathRooted(batchFileName))
            {
                batchFilePath = batchFileName;
            }
            else
            {
                batchFilePath = Path.Combine(_context.WorkingDirectory, batchFileName);
            }
            if (!File.Exists(batchFilePath))
            {
                return false;
            }
            batchFilePath = Path.GetFullPath(batchFilePath);

            string outputFile = Path.Combine(_context.WorkingDirectory, @"Help\Manual.chm");
            if (File.Exists(outputFile))
            {
                try
                {
                    File.Delete(outputFile);
                }
                catch
                {
                    bool bFound = false;
                    Process[] hhProcesses = Process.GetProcessesByName("hh");
                    if (hhProcesses != null && hhProcesses.Length != 0)
                    {
                        int processCount = hhProcesses.Length;
                        for (int i = 0; i < processCount; i++)
                        {   
                            Process compiledHelp = hhProcesses[i];

                            // In a typical GUI we could tell what the title of the help window,
                            // and compare it with that of the process.
                            if (String.Equals(compiledHelp.MainWindowTitle, "Manual",
                                StringComparison.CurrentCultureIgnoreCase))
                            {
                                compiledHelp.CloseMainWindow();
                                compiledHelp.Close();

                                bFound = true;

                                break;
                            }
                        }
                    }

                    if (!bFound)
                    {
                        logger.WriteLine(
                            "Please close the Help file, and try again.",
                            HelpLoggerLevel.Error);

                        return false;
                    }
                }
            }

            try
            {
                Process process = new Process();

                ProcessStartInfo startInfo = process.StartInfo;

                startInfo.FileName               = batchFilePath;
                startInfo.UseShellExecute        = false;
                startInfo.WorkingDirectory       = Path.GetDirectoryName(batchFilePath);
                startInfo.RedirectStandardOutput = true;

                // Add the event handler to receive the console output...
                process.OutputDataReceived += new DataReceivedEventHandler(OnOutputDataReceived);

                // Now, start the process - there will still not be output till...
                process.Start();

                // Start the asynchronous read of the output stream
                process.BeginOutputReadLine();

                // We must wait for the process to complete...
                process.WaitForExit();

                if (File.Exists(outputFile))
                {
                    Process.Start(outputFile);
                }

                process.Close();

                return true;
            }
            catch (Exception ex)
            {
                logger.WriteLine(ex);

                return false;
            }
        }

        #endregion

        #region Uninitialize Method

        public override void Uninitialize()
        {
            if (_listTasks != null)
            {
                int taskCount = _listTasks.Count;

                for (int i = 0; i < taskCount; i++)
                {
                    BuildTask buildTask = _listTasks[i];
                    if (buildTask != null)
                    {
                        buildTask.Uninitialize();
                        buildTask.Dispose();
                    }
                }

                _listTasks = null;
            }

            this.Logger.Uninitialize();
 
            _isInitialized = false;
        }

        #endregion

        #endregion

        #region Private Methods

        #region OnOutputDataReceived Method

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            HelpLogger logger = this.Logger;

            string lineReceived = e.Data;

            if (lineReceived == null)
            {
                return;
            }

            logger.WriteLine(lineReceived, HelpLoggerLevel.None);
        }

        #endregion

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
