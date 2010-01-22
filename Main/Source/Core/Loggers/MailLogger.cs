using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Loggers
{
    /// <summary>
    /// This logger records all the relevant build events, information, warnings, 
    /// and errors in various formats, and mailing the record to a specified address.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The following file formats are currently supported:
    /// </para>
    /// <list type="number">
    /// <item>
    /// <term>Plain Text Format: </term>
    /// <description>
    /// A simple text formatting of the messages.
    /// </description>
    /// </item>
    /// <item>
    /// <term>HTML Format: </term>
    /// <description>
    /// A standard HyperText Markup Language (HTML) format with collapsible regions of the essages.
    /// </description>
    /// </item>
    /// <item>
    /// <term>XML Format: </term>
    /// <description>
    /// A simple Extensible Markup Language (XML) text formatting of the messages.
    /// </description>
    /// </item>
    /// </list>
    /// <para>
    /// The following file formats will be supported in the later releases:
    /// </para>
    /// <list type="number">
    /// <item>
    /// <term>Rich-Text Format: </term>
    /// <description>
    /// A simple rich-text (RTF) formatting of the messages.
    /// </description>
    /// </item>
    /// <item>
    /// <term>PDF Format: </term>
    /// <description>
    /// The Adobe Portable Document Format (PDF) formatting of the essages.
    /// </description>
    /// </item>
    /// <item>
    /// <term>XPS Format: </term>
    /// <description>
    /// The Microsoft XML Paper Specification formatting of the messages.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    public class MailLogger : BuildLogger
    {
        #region Public Static Fields

        public const string TextFormat = "PlainText";
        public const string HtmlFormat = "HtmlText";
        public const string XmlFormat  = "XmlText";

        #endregion

        #region Private Fields

        private string _logFormat;

        #endregion

        #region Constructors and Destructor

        public MailLogger()
        {
            _logFormat = MailLogger.TextFormat;
        }

        public MailLogger(string logFile)
            : base(logFile)
        {
            _logFormat = MailLogger.TextFormat;
        }

        #endregion

        #region Public Properties

        public string LogFormat
        {
            get 
            { 
                return _logFormat; 
            }
            set 
            { 
                if (!String.IsNullOrEmpty(value))
                {
                    _logFormat = value; 
                }
            }
        }

        #endregion

        #region Public Methods

        public override void WriteLine()
        {
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
    }
}
