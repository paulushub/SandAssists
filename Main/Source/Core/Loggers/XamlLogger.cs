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

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CloseOutput = false;
            settings.Indent = true;
            settings.IndentChars = new string(' ', 4);
            settings.Encoding = this.Encoding;
            settings.OmitXmlDeclaration = true;

            _xmlWriter = XmlWriter.Create(this.BaseWriter, settings);
            _xmlWriter.WriteStartDocument();
            _xmlWriter.WriteDocType("html", "-//W3C//DTD XHTML 1.0 Transitional//EN", "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd", null);
            _xmlWriter.WriteStartElement("html", "http://www.w3.org/1999/xhtml"); // html
            _xmlWriter.WriteAttributeString("xml", "lang", "", "en");

            _xmlWriter.WriteStartElement("head");  // head
            _xmlWriter.WriteStartElement("title"); // title
            _xmlWriter.WriteString(logTitle == null ? String.Empty : logTitle);
            _xmlWriter.WriteFullEndElement();      // title

            _xmlWriter.WriteEndElement();          // head

            _xmlWriter.WriteStartElement("body");  // body
            // <div id="options">
            //    <a href="javascript:parentAccordion.pr(1)">Exand All</a> | 
            //    <a href="javascript:parentAccordion.pr(-1)">Collapse All</a>
            //</div>
            _xmlWriter.WriteStartElement("div");  // div
            _xmlWriter.WriteAttributeString("id", "options");

            _xmlWriter.WriteStartElement("a");    // a
            _xmlWriter.WriteAttributeString("href", "javascript:parentAccordion.pr(1)");
            _xmlWriter.WriteString("Expand All");
            _xmlWriter.WriteEndElement();         // a
            _xmlWriter.WriteString(" | ");
            _xmlWriter.WriteStartElement("a");    // a
            _xmlWriter.WriteAttributeString("href", "javascript:parentAccordion.pr(-1)");
            _xmlWriter.WriteString("Collapse All");
            _xmlWriter.WriteEndElement();         // a

            _xmlWriter.WriteEndElement();         // div

            _xmlWriter.WriteStartElement("ul");  // ul
            _xmlWriter.WriteAttributeString("id", "acc");
        }

        public override void Uninitialize()
        {
            if (_xmlWriter != null)
            {
                _xmlWriter.WriteFullEndElement();  // ul

                _xmlWriter.WriteFullEndElement();  // body
                _xmlWriter.WriteEndElement();      // html
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
                _xmlWriter.WriteStartElement("tr");  // tr
                _xmlWriter.WriteStartElement("td");  // td
                _xmlWriter.WriteAttributeString("class", "nl");
                _xmlWriter.WriteString(".");
                _xmlWriter.WriteEndElement();        // td
                _xmlWriter.WriteStartElement("td");  // td
                _xmlWriter.WriteAttributeString("class", "nl");
                _xmlWriter.WriteString("");
                _xmlWriter.WriteEndElement();        // td
                _xmlWriter.WriteEndElement();        // tr
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
                _xmlWriter.WriteStartElement("li");  // li

                _xmlWriter.WriteStartElement("h3");   // h3
                //_xmlWriter.WriteAttributeString("class", "msgTitle");
                _xmlWriter.WriteString(outputText);
                _xmlWriter.WriteEndElement();         // h3

                _xmlWriter.WriteStartElement("div");  // div
                _xmlWriter.WriteAttributeString("class", "acc-section");

                _xmlWriter.WriteStartElement("div");  // div
                _xmlWriter.WriteAttributeString("class", "acc-content");

                _xmlWriter.WriteStartElement("table");  // table
                _xmlWriter.WriteAttributeString("style", "width:100%");
                _xmlWriter.WriteAttributeString("cellspacing", "0");

                _xmlWriter.WriteStartElement("tr");  // tr
                _xmlWriter.WriteStartElement("th");  // th
                _xmlWriter.WriteAttributeString("style", "width:64px");
                _xmlWriter.WriteString("Level");
                _xmlWriter.WriteEndElement();        // th
                _xmlWriter.WriteStartElement("th");  // th
                _xmlWriter.WriteString("Message");
                _xmlWriter.WriteEndElement();        // th
                _xmlWriter.WriteEndElement();        // tr

                _startedSection = true;

                return;
            }
            if (level == BuildLoggerLevel.Ended)
            {
                _xmlWriter.WriteEndElement();         // table
                _xmlWriter.WriteEndElement();         // div
                _xmlWriter.WriteEndElement();         // div
                _xmlWriter.WriteRaw("<p>&nbsp;</p>");
                _xmlWriter.WriteEndElement();         // li

                _startedSection = false;

                return;
            }

            if (_startedSection)
            {
                _xmlWriter.WriteStartElement("tr");  // tr
                _xmlWriter.WriteStartElement("td");  // td
                if (level == BuildLoggerLevel.None)
                {
                    _xmlWriter.WriteAttributeString("class", "nl");
                    _xmlWriter.WriteString(".");
                }
                else
                {
                    _xmlWriter.WriteString(level.ToString());
                }
                _xmlWriter.WriteEndElement();        // td
                _xmlWriter.WriteStartElement("td");  // td
                if (level == BuildLoggerLevel.None)
                {
                    _xmlWriter.WriteAttributeString("class", "nl");
                }
                _xmlWriter.WriteString(outputText);
                _xmlWriter.WriteEndElement();        // td
                _xmlWriter.WriteEndElement();        // tr
            }
            else
            {
                _xmlWriter.WriteStartElement("li");  // li

                _xmlWriter.WriteStartElement("h3");   // h3
                _xmlWriter.WriteString("...");
                _xmlWriter.WriteEndElement();         // h3

                _xmlWriter.WriteStartElement("div");  // div
                _xmlWriter.WriteAttributeString("class", "acc-section");

                _xmlWriter.WriteStartElement("div");  // div
                _xmlWriter.WriteAttributeString("class", "acc-content");

                _xmlWriter.WriteStartElement("p");   // p
                _xmlWriter.WriteString(outputText);
                _xmlWriter.WriteEndElement();        // p

                _xmlWriter.WriteEndElement();         // div
                _xmlWriter.WriteEndElement();         // div
                _xmlWriter.WriteRaw("<p>&nbsp;</p>");
                _xmlWriter.WriteEndElement();         // li
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
