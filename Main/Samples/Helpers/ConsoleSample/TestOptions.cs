using System;
using System.IO;

using Sandcastle;
using Sandcastle.Formats;
using Sandcastle.Loggers;
using Sandcastle.Contents;

namespace ConsoleSample
{
    /// <summary>
    /// This creates the options and settings for the test documentations.
    /// </summary>
    class TestOptions
    {
        private bool _buildConceptual;
        private bool _buildReferences;

        private string _sampleDir;
        private string _workingDir;
        private string _sandAssistDir;

        private CustomTocType _tocType;

        private BuildSettings _settings;

        public TestOptions(BuildSettings settings)
        {
            BuildExceptions.NotNull(settings, "settings");

            _settings = settings;

            string allSamplesDir = @"..\Samples\Helpers\";

            _sampleDir     = Path.GetFullPath(allSamplesDir);
            _workingDir    = Path.Combine(_sampleDir, "HelpTest");
            _sandAssistDir = Environment.CurrentDirectory;

            _tocType       = CustomTocType.TopicRoot;
        }

        #region Public Properties

        public CustomTocType TocType
        {
            get
            {
                return _tocType;
            }
        }

        public bool BuildConceptual
        {
            get
            {
                return _buildConceptual;
            }
        }


        public bool BuildReferences
        {
            get
            {
                return _buildReferences;
            }
        }

        public string SampleDir
        {
            get
            {
                return _sampleDir;
            }
        }

        public string WorkingDir
        {
            get
            {
                return _workingDir;
            }
        }

        public string SandAssistDir
        {
            get
            {
                return _sandAssistDir;
            }
        }

        #endregion

        #region Create Method

        public void Create()
        {
            bool useCustomStyles = true;

            BuildStyleType styleType = BuildStyleType.ClassicWhite;

            _settings.WorkingDirectory = new BuildDirectoryPath(_workingDir);
            _settings.CleanIntermediate = false;
            _settings.ShowPreliminary = true;
            _settings.Style.StyleType = styleType;
            //_settings.SyntaxType = BuildSyntaxType.None;

            _settings.HeaderText = "Header: This is the header text.";
            _settings.FooterText = "Footer: This is the footer text.";

            BuildFeedback feedBack = _settings.Feedback;
            feedBack.CompanyName   = "Sandcastle Assist";
            feedBack.ProductName   = "Sandcastle Helpers";
            feedBack.EmailAddress  = "paulselormey@gmail.com";
            feedBack.FeedbackType  = BuildFeedbackType.None;
            feedBack.CopyrightText =
                "Copyright &#169; 2007-2008 Sandcastle Assist. All Rights Reserved.";
            feedBack.CopyrightLink = "http://www.codeplex.com/SandAssist";

            // Configure the logo image information...
            feedBack.LogoEnabled   = true; // show it...
            feedBack.LogoImage     = new BuildFilePath(Path.Combine(_sandAssistDir, "AssistLogo.jpg"));
            feedBack.LogoWidth     = 64;
            feedBack.LogoHeight    = 64;
            feedBack.LogoPadding   = 3;
            feedBack.LogoText      = "Sandcastle Assist";
            feedBack.LogoLink      = "http://www.codeplex.com/SandAssist";
            feedBack.LogoAlignment = BuildLogoAlignment.Center;
            feedBack.LogoPlacement = BuildLogoPlacement.Right;

            // Configure the logging, we add some loggers by their names...
            BuildLogging logging = _settings.Logging;
            //logging.Verbosity = BuildLoggerVerbosity.Detailed;
            //logging.AddLogger(XmlLogger.LoggerName);
            //logging.AddLogger(HtmlLogger.LoggerName);
            ///logging.AddLogger(XamlLogger.LoggerName);
            logging.AddLogger(ConsoleLogger.LoggerName);

            BuildStyle style = _settings.Style;
            // Add direct code snippet root folder...
            SnippetContent snippets = style.Snippets;
            snippets.Add(new SnippetItem(
                Path.Combine(_sampleDir, "SampleSnippets")));

            // Add some custom math packages and commands...
            MathPackageContent mathPackages = style.MathPackages;
            mathPackages.Add("picture", "calc");
            mathPackages.Add("xy", "all", "knot", "poly");

            MathCommandContent mathCommands = style.MathCommands;
            mathCommands.Add(@"\quot",
                @"\dfrac{\varphi \cdot X_{n, #1}}{\varphi_{#2} \times \varepsilon_{#1}}", 2);
            mathCommands.Add(@"\exn", @"(x+\varepsilon_{#1})^{#1}", 1);

            if (useCustomStyles)
            {
                string stylesDir = @"Presentations";
                stylesDir = Path.GetFullPath(stylesDir);
                if (Directory.Exists(stylesDir))
                {
                    _settings.Style.Directory = new BuildDirectoryPath(stylesDir);
                }
            }

            FormatChm chmFormat =
                _settings.Formats[BuildFormatType.HtmlHelp1] as FormatChm;
            if (chmFormat != null)
            {
                chmFormat.Enabled      = true;
                chmFormat.UseBinaryToc = false;
                chmFormat.Indent       = true;
            }

            //FormatHxs hxsFormat =
            //  _settings.Formats[BuildFormatType.HtmlHelp2] as FormatHxs;
            //if (hxsFormat != null)
            //{
            //    //hxsFormat.SeparateIndexFile = true;
            //    hxsFormat.Enabled = true;
            //    hxsFormat.Indent = true;
            //}

            //FormatMhv mhvFormat =
            //    _settings.Formats[BuildFormatType.HtmlHelp3] as FormatMhv;
            //if (mhvFormat != null)
            //{
            //    mhvFormat.Enabled = true;
            //    mhvFormat.Indent = true;
            //}

            //FormatWeb webFormat = 
            //    _settings.Formats[BuildFormatType.WebHelp] as FormatWeb;
            //if (webFormat != null)
            //{
            //    webFormat.Enabled = true;
            //    webFormat.Indent = true;
            //}

            //_settings.HelpName = "HelpRegister";
            //_settings.HelpTitle = "Sandcastle HelpRegister";
            _settings.HelpName  = "SandcastleHelpers";
            _settings.HelpTitle = "Sandcastle Helpers Test Sample";
        }

        #endregion

        #region Run Method

        public bool Run()
        {
            //Console.Title = "Sandcastle Test Builder - " + _settings.Style.StyleType;
            //Console.SetWindowSize(Console.LargestWindowWidth - 20,
            //    Console.LargestWindowHeight - 20);
            //Console.BackgroundColor = ConsoleColor.DarkCyan;
            //Console.ForegroundColor = ConsoleColor.White;
            //Console.Clear();

            Console.WriteLine(
                "******** This is Sandcastle Test Sample *************");
            Console.WriteLine();
            Console.WriteLine("The Output Style: " + _settings.Style.StyleType);
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(
                "Please selection the build option: typing 0, 1, 2 or 3");
            Console.WriteLine("\t0 -- Quit or Exit");
            Console.WriteLine("\t1 -- Build Reference Help");
            Console.WriteLine("\t2 -- Build Conceptual Help");
            Console.WriteLine("\t3 -- Build both Conceptual and Reference Help");
            Console.ForegroundColor = ConsoleColor.White;

            Console.Write("\t\t?>>>");
            bool isInvalid = true;
            int buildType = -1;
            while (isInvalid)
            {
                string textRead = Console.ReadLine();
                int option = -1;
                if (String.IsNullOrEmpty(textRead) == false)
                {
                    try
                    {
                        option = Convert.ToInt32(textRead);
                        isInvalid = (option < 0 || option > 3);
                    }
                    catch
                    {
                        isInvalid = true;
                    }
                }

                if (isInvalid)
                {
                    Console.Write("\t\t?>>>");
                }
                else
                {
                    buildType = option;
                    Console.Clear();
                }
            }
            if (buildType <= 0)  // quit or exit
            {
                return false;
            }
            else if (buildType == 1) // reference build
            {
                _buildReferences = true;
                _buildConceptual = false;
            }
            else if (buildType == 2) // conceptual build
            {
                _buildReferences = false;
                _buildConceptual = true;
            }
            else if (buildType == 3) // reference and conceptual builds
            {
                _buildReferences = true;
                _buildConceptual = true;
            }
            else
            {
                return false;
            }

            _settings.BuildReferences = _buildReferences;
            _settings.BuildConceptual = _buildConceptual;

            return true;
        }

        #endregion
    }
}
