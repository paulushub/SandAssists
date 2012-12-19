using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components.Maths
{
    public abstract class MathFormatter : IDisposable
    {
        #region Private Fields

        private int      _inlineSize;
        private int      _inlineZoom;
        private int      _displayedSize;
        private int      _displayedZoom;

        private int      _namingCount;
        private bool     _hasPath;
        private bool     _isConceptual;
        private string   _namingPrefix;
        private string   _workingDir;
        private string[] _sizesLaTeX;

        private Type             _componentType;
        private MessageWriter    _messageWriter;
        private MathNamingMethod _namingMethod;

        private List<MathTeXCommand> _listCommands;
        private List<MathTeXPackage> _listPackages;

        #endregion

        #region Constructors and Destructor

        protected MathFormatter()
        {
            _inlineSize    = 10;
            _displayedSize = 10;
            _inlineZoom    = 2;
            _displayedZoom = 3;
            _namingPrefix  = "math";
            _namingMethod  = MathNamingMethod.Sequential;
            _sizesLaTeX    = new string[] { "\\tiny", "\\small", "\\normalsize",
                "\\large", "\\Large", "\\LARGE", "\\huge", "\\Huge"};
        }

        protected MathFormatter(Type componentType, MessageWriter messageWriter)
            : this()
        {
             if (componentType == null)
            {
                throw new ArgumentNullException("componentType",
                    "The component type cannot be null (or Nothing).");
            }
            if (messageWriter == null)
            {
                throw new ArgumentNullException("messageWriter",
                    "The message writer cannot be null (or Nothing).");
            }

            _componentType  = componentType;
            _messageWriter = messageWriter;
       }

        protected MathFormatter(Type componentType, MessageWriter messageWriter,
            XPathNavigator formatter) : this(componentType, messageWriter)
        {
            //<formatter format="LaTeX" type="MikTeX" baseSize="10">
            //    <style type="inline" baseSize="10" zoomLevel="2" />
            //    <style type="displayed" baseSize="10" zoomLevel="3" />
            //</formatter>
            if (formatter != null)
            {
                int baseSize = -1;
                string attribute = formatter.GetAttribute("baseSize", String.Empty);

                if (String.IsNullOrEmpty(attribute) == false)
                {
                    baseSize = Convert.ToInt32(attribute);
                    if (baseSize >= 10 && baseSize <= 12)
                    {
                        _inlineSize    = baseSize;
                        _displayedSize = baseSize;
                    }
                }

                XPathNodeIterator iterator = formatter.Select("style");
 
                foreach (XPathNavigator navigator in iterator)
                {
                    attribute = navigator.GetAttribute("type", String.Empty);
                    if (String.IsNullOrEmpty(attribute))
                    {
                        continue;
                    }

                    if (String.Equals(attribute, "inline",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        attribute = navigator.GetAttribute("baseSize", 
                            String.Empty);

                        if (String.IsNullOrEmpty(attribute) == false)
                        {
                            baseSize = Convert.ToInt32(attribute); 
                            if (baseSize >= 10 && baseSize <= 12)
                            {
                                _inlineSize = baseSize;
                            }
                        }

                        attribute = navigator.GetAttribute("zoomLevel", 
                            String.Empty);
                        if (String.IsNullOrEmpty(attribute) == false)
                        {
                            int zoomSize = Convert.ToInt32(attribute);
                            if (zoomSize >= 0)
                            {
                                _inlineZoom = zoomSize;
                            }
                        }
                    }
                    else if (String.Equals(attribute, "displayed",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        attribute = navigator.GetAttribute("baseSize", 
                            String.Empty);

                        if (String.IsNullOrEmpty(attribute) == false)
                        {
                            baseSize = Convert.ToInt32(attribute);
                            if (baseSize >= 10 && baseSize <= 12)
                            {
                                _displayedSize = baseSize;
                            }
                        }

                        attribute = navigator.GetAttribute("zoomLevel", 
                            String.Empty);
                        if (String.IsNullOrEmpty(attribute) == false)
                        {
                            int zoomSize = Convert.ToInt32(attribute);
                            if (zoomSize >= 0)
                            {
                                _displayedZoom = zoomSize;
                            }
                        }
                    }
                }
            }
        }

        ~MathFormatter()
        {
            this.Dispose(false);
        }

        #endregion

        #region Public Properties

        public bool HasPath
        {
            get
            {
                return _hasPath;
            }
        }

        public bool IsConceptual
        {
            get
            {
                return _isConceptual;
            }
        }

        public abstract string ImagePath
        {
            get;
        }

        public abstract string ImageFile
        {
            get;
        }

        public abstract string ImageExtension
        {
            get;
        }          

        public string WorkingDirectory
        {
            get
            {
                return _workingDir;
            }

            set
            {
                _workingDir = value;
                _hasPath = (String.IsNullOrEmpty(_workingDir) == false);

                if (_hasPath)
                {
                    _workingDir = Path.GetFullPath(_workingDir);
                    if (Directory.Exists(_workingDir) == false)
                    {
                        Directory.CreateDirectory(_workingDir);
                    }
                }
            }
        }

        public string NamingPrefix
        {
            get 
            { 
                return _namingPrefix; 
            }

            set 
            {
                _namingPrefix = (value == null) ? String.Empty : value; 
            }
        }

        public MathNamingMethod NamingMethod
        {
            get 
            { 
                return _namingMethod; 
            }

            set 
            { 
                _namingMethod = value; 
            }
        }

        public int InlineSize
        {
            get
            {
                return _inlineSize;
            }

            set
            {
                if (value >= 10 && value <= 12)
                {
                    _inlineSize = value;
                }
            }
        }

        public int InlineZoom
        {
            get
            {
                return _inlineZoom;
            }

            set
            {
                if (value >= 0)
                {
                    _inlineZoom = value;
                }
            }
        }

        public int DisplayedSize
        {
            get
            {
                return _displayedSize;
            }

            set
            {
                if (value >= 10 && value <= 12)
                {
                    _displayedSize = value;
                }
            }
        }

        public int DisplayedZoom
        {
            get
            {
                return _displayedZoom;
            }

            set
            {
                if (value >= 0)
                {
                    _displayedZoom = value;
                }
            }
        }

        public IList<MathTeXCommand> Commands
        {
            get
            {
                return _listCommands;
            }
        }

        public IList<MathTeXPackage> Packages
        {
            get
            {
                return _listPackages;
            }
        }

        #endregion

        #region Public Methods

        public virtual void BeginUpdate(string workingDirectory, bool isConceptual)
        {   
            _isConceptual = isConceptual;
            _workingDir   = workingDirectory;
            _hasPath      = (String.IsNullOrEmpty(_workingDir) == false);

            if (_hasPath)
            {
                _workingDir = Path.GetFullPath(_workingDir);
                if (Directory.Exists(_workingDir) == false)
                {
                    Directory.CreateDirectory(_workingDir);
                }
            }
        }

        public virtual void Update(XPathNavigator configuration)
        {   
            if (configuration == null)
            {
                return;
            }

            //<packages>
            //  <package use="amstext" />
            //  <package use="amsbsy, amscd" />
            //  <package use="noamsfonts" option="psamsfonts" />
            //</packages>
            XPathNodeIterator iterator = configuration.Select("packages/package");
            if (iterator != null && iterator.Count > 0)
            {
                _listPackages = new List<MathTeXPackage>(iterator.Count);

                foreach (XPathNavigator navigator in iterator)
                {
                    string attribute = navigator.GetAttribute("use",
                        String.Empty);
                    if (String.IsNullOrEmpty(attribute) == false)
                    {
                        MathTeXPackage package = new MathTeXPackage(attribute,
                            navigator.GetAttribute("options", String.Empty));

                        if (package.IsValid)
                        {
                            _listPackages.Add(package);
                        }
                    }
                }

                this.WriteMessage(MessageLevel.Info,
                    String.Format("Loaded {0} LaTeX packages", _listPackages.Count));
            }

            //<commands>
            //    <!-- For testing an example 
            //    <command name="" value="" />-->
            //    <command name="\quot" value="" arguments="2" />
            //    <command name="\exn" value="" arguments="1" />
            //</commands> 
            iterator = configuration.Select("commands/command");
            if (iterator != null && iterator.Count > 0)
            {
                _listCommands = new List<MathTeXCommand>(iterator.Count);

                foreach (XPathNavigator navigator in iterator)
                {
                    string attribute = navigator.GetAttribute("name",
                        String.Empty);
                    if (String.IsNullOrEmpty(attribute) == false)
                    {
                        MathTeXCommand command = new MathTeXCommand(attribute,
                            navigator.GetAttribute("value", String.Empty),
                            navigator.GetAttribute("arguments", String.Empty));

                        if (command.IsValid)
                        {
                            _listCommands.Add(command);
                        }
                    }
                }

                this.WriteMessage(MessageLevel.Info,
                    String.Format("Loaded {0} LaTeX commands", _listCommands.Count));
            }
        }

        public virtual void EndUpdate()
        {
        }

        public abstract bool Create(string equationText,
            bool isInline, bool isUser);

        #endregion

        #region Protected Methods

        protected string NextName()
        {
            if (_namingMethod == MathNamingMethod.Guid)
            {
                return Guid.NewGuid().ToString() + this.ImageExtension;
            }
            if (_namingMethod == MathNamingMethod.Random)
            {
                string tempFile = Path.GetRandomFileName().Replace(".", String.Empty);
                return tempFile + this.ImageExtension;
            }
            if (_namingMethod == MathNamingMethod.Sequential)
            {
                _namingCount++;
                if (_isConceptual)
                {
                    return String.Format("{0}{1:D4}{2}", _namingPrefix,
                        _namingCount, this.ImageExtension);
                }
                return String.Format("{0}{1:D3}{2}", _namingPrefix,
                    _namingCount, this.ImageExtension);
            }

            return Guid.NewGuid().ToString() + this.ImageExtension;
        }

        protected string GetSize(int index)
        {
            if ((_sizesLaTeX == null || _sizesLaTeX.Length == 0) ||
                (index < 0 || index >= _sizesLaTeX.Length))
            {
                return String.Empty;
            }

            return _sizesLaTeX[index];
        }

        protected void WriteMessage(MessageLevel level, string message)
        {
            if (String.IsNullOrEmpty(message))
            {
                return;
            }

            if (level != MessageLevel.Ignore && _messageWriter != null)
            {
                _messageWriter.Write(_componentType, level, message);
            }
        }

        protected void WriteMessage(MessageLevel level, Exception ex)
        {
            string message = ex.Message;
            if (String.IsNullOrEmpty(message))
            {
                this.WriteMessage(level, ex.ToString());
            }
            else
            {
                this.WriteMessage(level, String.Format("Exception({0}) - {1}",
                    ex.GetType().Name, message));
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            _componentType  = null;
            _messageWriter = null;
        }

        #endregion
    }
}
