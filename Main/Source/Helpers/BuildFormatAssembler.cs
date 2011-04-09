using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    public abstract class BuildFormatAssembler : BuildObject
    {
        #region Private Fields

        private BuildGroup    _group;
        private BuildFormat   _format;
        private BuildContext  _context;
        private BuildSettings _settings;

        #endregion

        #region Constructors and Destrutor

        protected BuildFormatAssembler(BuildContext context)
        {
            BuildExceptions.NotNull(context, "context");

            _context = context;
        }

        #endregion

        #region Public Properties

        public bool IsInitialized
        {
            get
            {
                return (_group != null && _format != null && _settings != null);
            }
            protected set
            {   
                if (!value)
                {
                    _group    = null;
                    _format   = null;
                    _settings = null;
                }
            }
        }

        public BuildGroup Group
        {
            get 
            { 
                return _group; 
            }
        }

        public BuildFormat Format
        {
            get 
            { 
                return _format; 
            }
        }

        public BuildContext Context
        {
            get
            {
                return _context;
            }
        }

        public BuildSettings Settings
        {
            get 
            { 
                return _settings; 
            }
        }

        #endregion

        #region Public Methods

        public virtual void Initialize(BuildFormat format, 
            BuildSettings settings, BuildGroup group)
        {
            BuildExceptions.NotNull(format,   "format");
            BuildExceptions.NotNull(settings, "group");
            BuildExceptions.NotNull(group,    "group");

            if (this.IsInitialized)
            {
                return;
            }

            _group    = group;
            _format   = format;
            _settings = settings;
        }

        public virtual void Uninitialize()
        {
            _group    = null;
            _format   = null;
            _settings = null;
        }

        public abstract void WriteAssembler(XmlWriter writer);

        #endregion
    }
}
