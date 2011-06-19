using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Loggers;

namespace Sandcastle
{
    [Serializable]
    public sealed class BuildLogging : BuildOptions<BuildLogging>
    {
        #region Public Fields

        public const string TagName                = "loggingOptions";
        /// <summary>
        /// 
        /// </summary>
        public const string DefaultOutputDirectory = "Logging";

        #endregion 

        #region Private Fields

        private bool   _useFile;
        private bool   _keepFile;
        private string _fileName;

        private BuildList<string>    _loggers;
        private BuildDirectoryPath   _outputPath;  
        private BuildLoggerVerbosity _verbosity;

        #endregion

        #region Constructor and Destructor

        public BuildLogging()
        {
            _useFile   = true;
            _keepFile  = true;
            _fileName  = "HelpBuild.log";  
            _verbosity = BuildLoggerVerbosity.Minimal;
            _loggers   = new BuildList<string>();   
        }

        public BuildLogging(BuildLogging source)
            : base(source)
        {
            _useFile   = source._useFile;
            _keepFile  = source._keepFile;
            _fileName  = source._fileName;
            _verbosity = source._verbosity;
            _loggers   = source._loggers;
            _outputPath = source._outputPath;
        }

        #endregion

        #region Public Properties

        public bool UseFile
        {
            get
            {
                return _useFile;
            }
            set
            {
                _useFile = value;
            }
        }

        public bool KeepFile
        {
            get
            {
                return _keepFile;
            }
            set
            {
                _keepFile = value;
            }
        }

        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                if (Path.IsPathRooted(value))
                {
                    _fileName = Path.GetFileName(value);
                }
                else
                {
                    _fileName = value;
                } 
            }
        }

        /// <summary>
        /// Gets or sets the level of detail to show in the build log.
        /// </summary>
        /// <value>
        /// An enumeration of the type <see cref="BuildLoggerVerbosity"/> specifying 
        /// the level of detail. The default is <see cref="BuildLoggerVerbosity.Minimal"/>.
        /// </value>
        public BuildLoggerVerbosity Verbosity
        {
            get
            {
                return _verbosity;
            }
            set
            {
                _verbosity = value;
            }
        }

        public IList<string> Loggers
        {
            get
            {
                return _loggers;
            }
        }

        public BuildDirectoryPath OutputPath
        {
            get
            {
                return _outputPath;
            }
            set
            {
                _outputPath = value;
            }
        }

        #endregion

        #region Public Methods

        public bool AddLogger(string loggerName)
        {
            if (String.IsNullOrEmpty(loggerName))
            {
                return false;
            }

            switch (loggerName)
            {
                case NoneLogger.LoggerName:
                case ConsoleLogger.LoggerName:
                case FileLogger.LoggerName:
                case HtmlLogger.LoggerName:
                case XmlLogger.LoggerName:
                case XamlLogger.LoggerName:
                case BuildLoggers.LoggerName:
                    if (!_loggers.Contains(loggerName))
                    {
                        _loggers.Add(loggerName);
                        return true;
                    }
                    break;
            }
            if (loggerName.StartsWith("Sandcastle.CustomLoggers") && 
                !_loggers.Contains(loggerName))
            {
                _loggers.Add(loggerName);
                return true;
            }

            return false;
        }

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        #endregion

        #region Private Methods

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a XML format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the XML attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string nodeText = reader.GetAttribute("verbosity");
            if (!String.IsNullOrEmpty(nodeText))
            {
                _verbosity = (BuildLoggerVerbosity)Enum.Parse(
                    typeof(BuildLoggerVerbosity), nodeText, true);
            }
            nodeText = reader.GetAttribute("useFile");
            if (!String.IsNullOrEmpty(nodeText))
            {
                _useFile = Convert.ToBoolean(nodeText);
            }
            nodeText = reader.GetAttribute("keepFile");
            if (!String.IsNullOrEmpty(nodeText))
            {
                _keepFile = Convert.ToBoolean(nodeText);
            }
            nodeText = reader.GetAttribute("fileName");
            if (!String.IsNullOrEmpty(nodeText))
            {
                _fileName = nodeText;
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "logger",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        nodeText = reader.GetAttribute("name");

                        if (!String.IsNullOrEmpty(nodeText))
                        {
                            _loggers.Add(nodeText);
                        }
                    }
                    else if (String.Equals(reader.Name, "location",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _outputPath = BuildDirectoryPath.ReadLocation(reader);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the XML format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The XML writer with which the XML format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            writer.WriteStartElement(TagName);  // start - logging
            writer.WriteAttributeString("verbosity", _verbosity.ToString());
            writer.WriteAttributeString("useFile",   _useFile.ToString());
            writer.WriteAttributeString("keepFile",  _keepFile.ToString());
            writer.WriteAttributeString("fileName",  _fileName);

            writer.WriteStartElement("location"); // location
            if (_outputPath != null)
            {
                _outputPath.WriteXml(writer);
            }
            writer.WriteEndElement();             // location

            writer.WriteStartElement("loggers");  // start - loggers
            if (_loggers != null && _loggers.Count != 0)
            {
                for (int i = 0; i < _loggers.Count; i++)
                {
                    writer.WriteStartElement("logger");  // start - logger
                    writer.WriteAttributeString("name", _loggers[i]);
                    writer.WriteEndElement();           // end - logger
                }
            }
            writer.WriteEndElement();             // end - loggers

            writer.WriteEndElement();           // end - logging
        }

        #endregion

        #region ICloneable Members

        public override BuildLogging Clone()
        {
            BuildLogging logging = new BuildLogging(this);

            if (_loggers != null)
            {
                logging._loggers = _loggers.Clone();
            }
            if (_fileName != null)
            {
                logging._fileName = String.Copy(_fileName);
            }
            if (_outputPath != null)
            {
                logging._outputPath = _outputPath.Clone();
            }

            return logging;
        }

        #endregion
    }
}
