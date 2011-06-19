using System;
using System.IO;
using System.Xml;

namespace Sandcastle.Loggers
{
    /// <summary>
    /// This logger records all the relevant build events, information, warnings, 
    /// and errors in an XML text format.  
    /// </summary>
    public class XmlLogger : BuildLogger
    {
        #region Public Fields

        public const string LoggerName     = "Sandcastle.Loggers.XmlLogger";

        public const string LoggerFileName = "XmlLogFile.xml";

        #endregion

        #region Private Fields

        private bool _startedSection;
        private XmlWriter _xmlWriter;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="XmlLogger"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="XmlLogger"/> class
        /// to the default properties or values.
        /// </summary>
        public XmlLogger()
            : this(LoggerFileName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlLogger"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="XmlLogger"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="XmlLogger"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public XmlLogger(string logFile)
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
        /// <c>Sandcastle.Loggers.XmlLogger</c>.
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

            _startedSection = false;

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CloseOutput  = false;
            settings.Indent       = true;
            settings.IndentChars  = "   ";
            settings.Encoding     = this.Encoding;
            settings.OmitXmlDeclaration = false;

            _xmlWriter = XmlWriter.Create(this.BaseWriter, settings);
            _xmlWriter.WriteStartDocument();
            _xmlWriter.WriteStartElement("messages");  // messages
        }                                                      

        public override void Uninitialize()
        {
            if (_xmlWriter != null)
            {
                _xmlWriter.WriteEndElement();          // messages
                _xmlWriter.WriteEndDocument();

                _xmlWriter.Close();
                _xmlWriter = null;
            }

            base.Uninitialize();
        }

        public override void WriteLine()
        {
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
            if (level == BuildLoggerLevel.None)
            {
                return;
            }
            if (level == BuildLoggerLevel.Started)
            {
                _startedSection = true;

                _xmlWriter.WriteStartElement("category");  // category
                _xmlWriter.WriteAttributeString("title", outputText);
                return;
            }
            if (level == BuildLoggerLevel.Ended)
            {
                _startedSection = false;
                _xmlWriter.WriteEndElement();              // category
                return;
            }

            if (_startedSection)
            {   
                _xmlWriter.WriteStartElement("message");  // message
                _xmlWriter.WriteAttributeString("level", level.ToString());
                _xmlWriter.WriteString(outputText);
                _xmlWriter.WriteEndElement();             // message
            }
            else
            {   
                _xmlWriter.WriteStartElement("category"); // category

                _xmlWriter.WriteStartElement("message");  // message
                _xmlWriter.WriteAttributeString("level", level.ToString());
                _xmlWriter.WriteString(outputText);
                _xmlWriter.WriteEndElement();             // message

                _xmlWriter.WriteEndElement();             // category
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
