using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

namespace Sandcastle.HelpRegister
{
    public partial class MainForm : Form
    {
        private int _result;
        private RegistrationOptions _cmdOptions;

        public MainForm()
        {
            InitializeComponent();
        }

        public int Result
        {
            get
            {
                return _result;
            }
        }

        public void SetOptions(RegistrationOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException("options",
                    "The options parameter cannot be null (or Nothing).");
            }

            _cmdOptions = options;
        }

        public void ProgressText(string message)
        {   
            if (String.IsNullOrEmpty(message))
            {
                return;
            }

            lblMessage.Text = message;
        }

        public void ProgressException(Exception exception)
        {   
            if (exception == null)
            {
                return;
            }

            lblMessage.Text = exception.Message;
        }

        private void OnLoad(object sender, EventArgs e)
        {
            _result = -1;

            loadingCircle.Enabled           = false;
            loadingCircle.OuterCircleRadius = 30;
            loadingCircle.InnerCircleRadius = 24;
            loadingCircle.NumberSpoke       = 16;
        }

        private void OnClosing(object sender, FormClosingEventArgs e)
        {
        }

        private void OnShown(object sender, EventArgs e)
        {
            if (this.CanRun())
            {
                this.RunTask();
            }
        }

        private void OnRetry(object sender, EventArgs e)
        {
            if (this.CanRun())
            {
                this.RunTask();
            }
        }

        private bool CanRun()
        {
            bool registerIt = false;

            try
            {
                listBox.BeginUpdate();
                listBox.Items.Clear();

                Process[] dxProcesses = Process.GetProcessesByName("dexplore");
                if (dxProcesses != null && dxProcesses.Length != 0)
                {
                    int processCount = dxProcesses.Length;
                    for (int i = 0; i < processCount; i++)
                    {
                        Process process = dxProcesses[i];

                        listBox.Items.Add(process.MainWindowTitle);
                    }

                    registerIt = false;
                }
                else
                {
                    registerIt = true;
                }

                listBox.EndUpdate();
            }
            catch
            {
                registerIt = false;
            }

            grpVS.Enabled         = !registerIt;
            loadingCircle.Enabled = registerIt;

            return registerIt;
        }

        private void RunTask()
        {
            grpVS.Enabled         = false;
            loadingCircle.Enabled = true;
            loadingCircle.Active  = true;

            FormsHelper helper = new FormsHelper(this, _cmdOptions);

            backgroundWorker.RunWorkerAsync(helper);
        }

        private void OnDoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker worker = sender as BackgroundWorker;

                FormsHelper helper = e.Argument as FormsHelper;
                if (helper == null)
                {
                    e.Result = -1;

                    return;
                }

                RegistrationOptions options = helper.Options;
                if (options == null)
                {
                    e.Result = -1;

                    return;
                }

                RegistrationHandler register = new RegistrationHandler();

                e.Result = register.Run(options, helper);
            }
            catch
            {
                e.Result = -1;

                return;
            }
        }

        private void OnRunWorkerCompleted(object sender, 
            RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                _result = Convert.ToInt32(e.Result);
            }

            this.Close();
        }
    }
}
