using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components.Maths
{
    public sealed class MathMikTeXFormatter : MathFormatter
    {
        #region Private Fields

        private bool   _transparent;

        private string _inputFile;
        private string _outpuFile;
        private string _texArguments;
        private string _dvipngFormats;

        private string _imageFile;
        private string _imagePath;
        private string _imageExt;

        private StringBuilder _inlineBuilder;
        private StringBuilder _displayedBuilder;

        #endregion

        #region Constructors and Destructor

        public MathMikTeXFormatter()
        {
            Initialize();
        }

        public MathMikTeXFormatter(Type componentType, MessageHandler messageHandler)
            : base(componentType, messageHandler)
        {
            Initialize();
        }

        public MathMikTeXFormatter(Type componentType, MessageHandler messageHandler,
            XPathNavigator formatter) : base(componentType, messageHandler, formatter)
        {
            Initialize();
        }

        #endregion

        #region Public Properties

        public override string ImageExtension
        {
            get
            {
                return _imageExt;
            }
        }

        public override string ImageFile
        {
            get
            {
                return _imageFile;
            }
        }

        public override string ImagePath
        {
            get
            {
                return _imagePath;
            }
        }

        public bool Transparent
        {
            get
            {
                return _transparent;
            }
        }

        #endregion

        #region Public Methods

        public override void BeginUpdate(string workingDirectory, bool isConceptual)
        {
            base.BeginUpdate(workingDirectory, isConceptual);

            _inputFile = Path.Combine(workingDirectory, "MikTeXFile.tex");

            _inlineBuilder.AppendLine("\\documentclass[10pt]{article}");
            _inlineBuilder.AppendLine("\\usepackage{amsmath, amsfonts, amssymb, latexsym}");
            _inlineBuilder.AppendLine("\\usepackage[mathscr]{eucal}");

            _displayedBuilder.AppendLine("\\documentclass[10pt]{article}");
            _displayedBuilder.AppendLine("\\usepackage{amsmath, amsfonts, amssymb, latexsym}");
            _displayedBuilder.AppendLine("\\usepackage[mathscr]{eucal}");

            //_texArguments = String.Format(
            //    "-quiet -interaction=nonstopmode -enable-installer {0}", 
            //    _inputFile);
            _texArguments = String.Format(
                "-quiet -interaction=nonstopmode -enable-installer \"{0}\"",
                _inputFile);
            if (_transparent)
            {
                _dvipngFormats =
                    "-q* -T tight --gamma 2.5 -bg Transparent -Q 6 -o {0} {1}";
            }
            else
            {
                _dvipngFormats =
                    "-q* -T tight --gamma 2.5 -Q 6 -o {0} {1}";
            }
            //_dvipngFormats =
            //    "-q* -T tight --gamma 2.5 -bg Transparent -Q 6 -o \"{0}\" \"{1}\"";
        }

        public override void Update(XPathNavigator configuration)
        {
            base.Update(configuration);

            IList<MathTeXPackage> listPackages = this.Packages;
            if (listPackages != null && listPackages.Count > 0)
            {
                int itemCount = listPackages.Count;

                for (int i = 0; i < itemCount; i++)
                {
                    MathTeXPackage package = listPackages[i];

                    package.ToString(_displayedBuilder);
                }
            }

            IList<MathTeXCommand> listCommands = this.Commands;
            if (listCommands != null && listCommands.Count > 0)
            {
                int itemCount = listCommands.Count;

                for (int i = 0; i < itemCount; i++)
                {
                    MathTeXCommand command = listCommands[i];

                    command.ToString(_displayedBuilder);
                }
            }

            _inlineBuilder.AppendLine("\\begin{document}");
            _inlineBuilder.AppendLine("\\pagestyle{empty}");

            _displayedBuilder.AppendLine("\\begin{document}");
            _displayedBuilder.AppendLine("\\pagestyle{empty}");
        }

        public override void EndUpdate()
        {
            base.EndUpdate();
        }

        public override bool Create(string equationText, 
            bool isInline, bool isUser)
        {
            if (String.IsNullOrEmpty(equationText))
            {
                return false;
            }
            _imageFile = null;
            _imagePath = null;

            string fileName = base.NextName();
            _imageFile = fileName;
            if (this.HasPath)
            {
                fileName = Path.Combine(this.WorkingDirectory, fileName);
            }

            //equationText = equationText.Trim();
            equationText = equationText.Replace("\\textstyle", String.Empty);
            equationText = equationText.Replace("\\displaystyle", String.Empty);
            equationText = equationText.Replace("\\parstyle", String.Empty);

            string mathText = equationText;
            bool opResult = false;
            if (isInline)
            {
                mathText = "\\textstyle " + equationText;
                opResult = CreateInline(mathText, _imageFile);
            }
            else
            {
                if (!isUser)
                {
                    mathText = "\\displaystyle " + equationText;
                }
                opResult = CreateDisplayed(mathText, isUser, _imageFile);
            }

            if (opResult)
            {
                _imagePath = fileName;
            }

            return opResult;
        }

        #endregion

        #region Private Methods

        private bool CreateInline(string equationText, string fileName)
        {
            if (_inlineBuilder == null || _inlineBuilder.Length == 0)
            {
                return false;
            }

            StreamWriter writer = new StreamWriter(_inputFile, false);

            writer.WriteLine(_inlineBuilder.ToString());  // force a new line.

            string zoomSize = this.GetSize(this.InlineZoom);
            if (String.IsNullOrEmpty(zoomSize) == false)
            {   
                writer.WriteLine(zoomSize);
            }
            writer.WriteLine("\\begin{align*}");
            writer.WriteLine(equationText);
            writer.WriteLine("\\end{align*}");
            writer.WriteLine("\\end{document}");
            writer.Close();
            writer = null;

            Process process = new Process();

            ProcessStartInfo startInfo = process.StartInfo;

            startInfo.FileName = @"latex.exe";
            startInfo.UseShellExecute = false;
            startInfo.Arguments = _texArguments;
            startInfo.WorkingDirectory = this.WorkingDirectory;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;

            // Add the event handler to receive the console output...
            //process.ErrorDataReceived  += new DataReceivedEventHandler(
            //    OnErrorDataReceived);
            //process.OutputDataReceived += new DataReceivedEventHandler(
            //    OnOutputDataReceived);

            // Now, start the LaTeX build process...
            process.Start();

            // Start the asynchronous read of the output stream
            //process.BeginErrorReadLine();
            //process.BeginOutputReadLine();

            // We must wait for the process to complete...
            process.WaitForExit();

            bool createResult = false;

            if (process.ExitCode == 0)
            {
                startInfo.FileName  = @"dvipng.exe";
                startInfo.Arguments = String.Format(_dvipngFormats, 
                    fileName, _outpuFile);

                // Now, start the Dvipng process...
                process.Start();

                // We must wait for the process to complete...
                process.WaitForExit();

                int exitCode = process.ExitCode;

                createResult = (exitCode == 0);
            }

            process.Close();

            return createResult;
        }

        private bool CreateDisplayed(string equationText, bool isUser, string fileName)
        {
            StreamWriter writer = new StreamWriter(_inputFile, false);

            writer.WriteLine(_displayedBuilder.ToString());  // force a new line.

            string zoomSize = this.GetSize(this.DisplayedZoom);
            if (String.IsNullOrEmpty(zoomSize) == false)
            {
                writer.WriteLine(zoomSize);
            }

            if (isUser)
            {
                writer.WriteLine(equationText);
            }
            else
            {
                writer.WriteLine("\\begin{align*}");
                writer.WriteLine(equationText);
                writer.WriteLine("\\end{align*}");
            }
            writer.WriteLine("\\end{document}");
            writer.Close();
            writer = null;

            Process process = new Process();

            ProcessStartInfo startInfo = process.StartInfo;

            startInfo.FileName = @"latex.exe";
            startInfo.UseShellExecute = false;
            startInfo.Arguments = _texArguments;
            startInfo.WorkingDirectory = this.WorkingDirectory;
            startInfo.RedirectStandardError = true;
            startInfo.RedirectStandardOutput = true;

            // Add the event handler to receive the console output...
            //process.ErrorDataReceived += new DataReceivedEventHandler(
            //    OnErrorDataReceived);
            //process.OutputDataReceived += new DataReceivedEventHandler(
            //    OnOutputDataReceived);

            // Now, start the LaTeX build process...
            process.Start();

            //// Start the asynchronous read of the output stream
            //process.BeginErrorReadLine();
            //process.BeginOutputReadLine();

            // We must wait for the process to complete...
            process.WaitForExit();

            bool createResult = false;

            if (process.ExitCode == 0)
            {
                startInfo.FileName  = @"dvipng.exe";
                startInfo.Arguments = String.Format(_dvipngFormats, 
                    fileName, _outpuFile);

                // Now, start the Dvipng process...
                process.Start();
                // We must wait for the process to complete...
                process.WaitForExit();

                int exitCode = process.ExitCode;

                createResult = (exitCode == 0);
            }

            process.Close();

            return createResult;
        }

        private void Initialize()
        {
            _transparent = true;
            //_logFile   = "MikTeXFile.log";
            //_otherFile = "MikTeXFile.aux";
            _inputFile = "MikTeXFile.tex";
            _outpuFile = "MikTeXFile.dvi";
            _imageExt  = ".png";

            _inlineBuilder    = new StringBuilder();
            _displayedBuilder = new StringBuilder();
        }

        #region Process Event Handlers

        private void OnErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            string lineReceived = e.Data;

            if (lineReceived == null)
            {
                return;
            }

            // Dvipng seems to be using the error standard output for most information...
            this.WriteMessage(MessageLevel.Warn, lineReceived);
        }

        private void OnOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            string lineReceived = e.Data;

            if (lineReceived == null)
            {
                return;
            }

            this.WriteMessage(MessageLevel.Info, lineReceived);
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
