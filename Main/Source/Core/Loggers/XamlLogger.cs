using System;
using System.IO;
using System.Xml;
using System.Text;

namespace Sandcastle.Loggers
{
    public class XamlLogger : BuildLogger
    {
        #region Public Fields

        public const string LoggerName     = "Sandcastle.Loggers.XamlLogger";

        public const string LoggerFileName = "XamlLogFile.xaml";

        #endregion

        #region Private Fields

        private bool _startedSection;
        private XmlWriter _xmlWriter;

        #endregion

        #region Constructors and Destructor

        public XamlLogger()
            : this(LoggerFileName)
        {
        }

        public XamlLogger(string logFile)
            : base(logFile)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name of this build logger.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the unique name of this
        /// build logger implementation. This will always return 
        /// <c>Sandcastle.Loggers.XamlLogger</c>.
        /// </value>
        public override string Name
        {
            get
            {
                return LoggerName;
            }
        }

        #endregion

        #region Protected Properties

        protected override bool IsFileLogging
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(string logWorkingDir, string logTitle)
        {
            base.Initialize(logWorkingDir, logTitle);

            XmlWriterSettings settings  = new XmlWriterSettings();
            settings.CloseOutput        = false;
            settings.Indent             = true;
            settings.IndentChars        = new string(' ', 4);
            settings.Encoding           = this.Encoding;
            settings.OmitXmlDeclaration = true;

            _xmlWriter = XmlWriter.Create(this.BaseWriter, settings);
            _xmlWriter.WriteStartDocument();
            _xmlWriter.WriteStartElement("FlowDocument", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"); // FlowDocument
            _xmlWriter.WriteAttributeString("xmlns", "x", String.Empty,
                "http://schemas.microsoft.com/winfx/2006/xaml");
            _xmlWriter.WriteAttributeString("PageWidth",   "720pt");
            _xmlWriter.WriteAttributeString("ColumnWidth", "720pt");
            _xmlWriter.WriteAttributeString("IsOptimalParagraphEnabled", "True");
            _xmlWriter.WriteAttributeString("IsHyphenationEnabled", "True");
            _xmlWriter.WriteAttributeString("FontFamily", "Trebuchet MS, Tahoma, Verdana");
            _xmlWriter.WriteAttributeString("LineHeight", "1");
            _xmlWriter.WriteAttributeString("TextAlignment", "Left");
            _xmlWriter.WriteAttributeString("PagePadding", "12,0,12,0");
            _xmlWriter.WriteAttributeString("AllowDrop", "False");
            _xmlWriter.WriteAttributeString("NumberSubstitution.CultureSource", "User");

            _xmlWriter.WriteStartElement("FlowDocument.Resources"); // FlowDocument.Resources
            _xmlWriter.WriteStartElement("Style"); // Style
            _xmlWriter.WriteAttributeString("TargetType", "{x:Type Paragraph}");
            
            _xmlWriter.WriteStartElement("Setter");   // Setter
            _xmlWriter.WriteAttributeString("Property", "Margin");
            _xmlWriter.WriteAttributeString("Value", "3");
            _xmlWriter.WriteEndElement();             // Setter
            
            _xmlWriter.WriteEndElement();          // Style     
            _xmlWriter.WriteEndElement();                           // FlowDocument.Resources 

            _xmlWriter.WriteStartElement("Section");  // Section
            _xmlWriter.WriteAttributeString("Name", "Title");

            _xmlWriter.WriteStartElement("Paragraph"); // Paragraph
            _xmlWriter.WriteAttributeString("Foreground", "RoyalBlue");
            _xmlWriter.WriteAttributeString("FontSize", "28");
            _xmlWriter.WriteAttributeString("KeepTogether", "True");
            this.WriteBold(logTitle == null ? String.Empty : logTitle);
            _xmlWriter.WriteEndElement();              // Paragraph

            _xmlWriter.WriteEndElement();             // Section
        }

        public override void Uninitialize()
        {
            if (_xmlWriter != null)
            {
                _xmlWriter.WriteEndElement();      // FlowDocument
                _xmlWriter.WriteEndDocument();

                _xmlWriter.Close();
                _xmlWriter = null;
            }

            base.Uninitialize();
        }

        public override void WriteLine()
        {
            if (_xmlWriter == null)
            {
                return;
            }

            if (_startedSection)
            {
                _xmlWriter.WriteStartElement("TableRow");  // TableRow
                _xmlWriter.WriteAttributeString("Background", "AliceBlue");
                _xmlWriter.WriteStartElement("TableCell");  // TableCell
                _xmlWriter.WriteAttributeString("BorderBrush", "RoyalBlue");
                _xmlWriter.WriteAttributeString("BorderThickness", "1");
                this.WriteParaRun(String.Empty);
                _xmlWriter.WriteEndElement();        // TableCell
                _xmlWriter.WriteStartElement("TableCell");  // TableCell
                _xmlWriter.WriteAttributeString("BorderBrush", "RoyalBlue");
                _xmlWriter.WriteAttributeString("BorderThickness", "1");
                this.WriteParaRun(String.Empty);
                _xmlWriter.WriteEndElement();        // TableCell
                _xmlWriter.WriteEndElement();        // TableRow
            }
        }

        public override void Write(Exception ex, BuildLoggerLevel level)
        {
            if (ex == null)
            {
                return;
            }

            this.Write(ex.ToString(), level);
        }

        public override void Write(string outputText, BuildLoggerLevel level)
        {
            if (String.IsNullOrEmpty(outputText))
            {
                return;
            }

            this.WriteLine(outputText, level);
        }

        public override void WriteLine(Exception ex, BuildLoggerLevel level)
        {
            if (ex == null)
            {
                return;
            }

            this.WriteLine(ex.ToString(), level);
        }

        public override void WriteLine(string outputText, BuildLoggerLevel level)
        {
            if (_xmlWriter == null)
            {
                return;
            }
            if (level == BuildLoggerLevel.Started)
            {
                // Begin a section...
                _xmlWriter.WriteStartElement("Section");  // Section
                _xmlWriter.WriteAttributeString("Margin", "3 16 3 3");

                // Add a section title...
                _xmlWriter.WriteStartElement("Paragraph"); // Paragraph
                _xmlWriter.WriteAttributeString("Foreground", "RoyalBlue");
                _xmlWriter.WriteAttributeString("FontSize", "24");
                _xmlWriter.WriteAttributeString("KeepTogether", "True");
                this.WriteBold(outputText);
                _xmlWriter.WriteEndElement();              // Paragraph

                // Start a table...
                _xmlWriter.WriteStartElement("Table");  // Table
                _xmlWriter.WriteAttributeString("BorderBrush", "RoyalBlue");
                _xmlWriter.WriteAttributeString("BorderThickness", "1");
                _xmlWriter.WriteAttributeString("CellSpacing", "0");

                // The table columns...
                _xmlWriter.WriteStartElement("Table.Columns");  // Table.Columns
                _xmlWriter.WriteStartElement("TableColumn");  // TableColumn
                _xmlWriter.WriteAttributeString("Width", "60pt");
                _xmlWriter.WriteEndElement();                 // TableColumn
                _xmlWriter.WriteStartElement("TableColumn");  // TableColumn
                _xmlWriter.WriteAttributeString("Width", "Auto");
                _xmlWriter.WriteEndElement();                 // TableColumn
                _xmlWriter.WriteEndElement();                   // Table.Columns

                // The table header...
                _xmlWriter.WriteStartElement("TableRowGroup");  // TableRowGroup
                _xmlWriter.WriteAttributeString("Background", "RoyalBlue");
                _xmlWriter.WriteAttributeString("Foreground", "White");
                _xmlWriter.WriteAttributeString("FontWeight", "Bold");
                _xmlWriter.WriteAttributeString("FontSize",   "16");
                _xmlWriter.WriteStartElement("TableRow");  // TableRow

                _xmlWriter.WriteStartElement("TableCell");  // TableCell
                _xmlWriter.WriteAttributeString("BorderBrush", "RoyalBlue");
                _xmlWriter.WriteAttributeString("BorderThickness", "1");
                this.WriteParaRun("Level");
                _xmlWriter.WriteEndElement();               // TableCell

                _xmlWriter.WriteStartElement("TableCell");  // TableCell
                _xmlWriter.WriteAttributeString("BorderBrush", "RoyalBlue");
                _xmlWriter.WriteAttributeString("BorderThickness", "1");
                this.WriteParaRun("Message");
                _xmlWriter.WriteEndElement();               // TableCell

                _xmlWriter.WriteEndElement();              // TableRow
                _xmlWriter.WriteEndElement();        // TableRowGroup

                // Start the content group...
                _xmlWriter.WriteStartElement("TableRowGroup");  // TableRowGroup

                _startedSection = true;

                return;
            }
            if (level == BuildLoggerLevel.Ended)
            {
                _xmlWriter.WriteEndElement();                   // TableRowGroup
                _xmlWriter.WriteEndElement();         // Table

                _xmlWriter.WriteEndElement();         // Section

                _startedSection = false;

                return;
            }

            if (_startedSection)
            {
                _xmlWriter.WriteStartElement("TableRow");  // TableRow
                _xmlWriter.WriteStartElement("TableCell");  // TableCell
                if (level == BuildLoggerLevel.Warn)
                {
                    _xmlWriter.WriteAttributeString("Background", "Goldenrod");
                    _xmlWriter.WriteAttributeString("Foreground", "White");
                }
                else if (level == BuildLoggerLevel.Error)
                {
                    _xmlWriter.WriteAttributeString("Background", "Tomato");
                    _xmlWriter.WriteAttributeString("Foreground", "White");
                }
                _xmlWriter.WriteAttributeString("BorderBrush", "RoyalBlue");
                _xmlWriter.WriteAttributeString("BorderThickness", "1");
                if (level == BuildLoggerLevel.None)
                {
                    this.WriteParaRun(String.Empty);
                }
                else
                {
                    this.WriteParaRun(level.ToString());
                }
                _xmlWriter.WriteEndElement();        // TableCell
                _xmlWriter.WriteStartElement("TableCell");  // TableCell
                _xmlWriter.WriteAttributeString("BorderBrush", "RoyalBlue");
                _xmlWriter.WriteAttributeString("BorderThickness", "1");
                this.WriteParaRun(outputText);
                _xmlWriter.WriteEndElement();        // TableCell
                _xmlWriter.WriteEndElement();        // TableRow
            }
            else
            {
                if (level == BuildLoggerLevel.None)
                {
                    this.WriteParaRun(outputText);
                }
                else
                {
                    this.WriteParaRun(level.ToString() + ": " + outputText);
                }
            }
        }

        #endregion

        #region Protected Methods

        protected override void Write(string outputText)
        {
        }

        protected override void WriteLine(string outputText)
        {
        }

        #endregion

        #region Private Methods

        private void WriteLinkBreak()
        {
            _xmlWriter.WriteStartElement("LineBreak"); // LineBreak
            _xmlWriter.WriteEndElement();              // LineBreak
        }

        private void WriteRun(string runText)
        {
            _xmlWriter.WriteStartElement("Run"); // Run
            _xmlWriter.WriteString(runText);
            _xmlWriter.WriteEndElement();        // Run
        }

        private void WriteBold(string boldText)
        {
            _xmlWriter.WriteStartElement("Bold"); // Bold
            _xmlWriter.WriteString(boldText);
            _xmlWriter.WriteEndElement();        // Bold
        }

        private void WriteParaLinkBreak()
        {
            _xmlWriter.WriteStartElement("Paragraph"); // Paragraph
            _xmlWriter.WriteStartElement("LineBreak"); // LineBreak
            _xmlWriter.WriteEndElement();              // LineBreak
            _xmlWriter.WriteEndElement();              // Paragraph
        }

        private void WriteParaRun(string runText)
        {
            _xmlWriter.WriteStartElement("Paragraph"); // Paragraph
            _xmlWriter.WriteStartElement("Run"); // Run
            _xmlWriter.WriteString(runText);
            _xmlWriter.WriteEndElement();        // Run
            _xmlWriter.WriteEndElement();              // Paragraph
        }

        private void WriteParaBold(string boldText)
        {
            _xmlWriter.WriteStartElement("Paragraph"); // Paragraph
            _xmlWriter.WriteStartElement("Bold"); // Bold
            _xmlWriter.WriteString(boldText);
            _xmlWriter.WriteEndElement();        // Bold
            _xmlWriter.WriteEndElement();              // Paragraph
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (_xmlWriter != null)
            {
                _xmlWriter.Close();
                _xmlWriter = null;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
