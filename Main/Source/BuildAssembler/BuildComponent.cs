// Copyright © Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Ddue.Tools
{        
    public abstract class BuildComponent : IDisposable
    {
        #region Private Fields

        private Type           _thisType;

        private BuildAssembler _assembler;

        // shared data
        private static Dictionary<string, object> _data = 
            new Dictionary<string, object>();

        #endregion

        #region Constructors and Destructor

        protected BuildComponent(BuildAssembler assembler, 
            XPathNavigator configuration)
        {
            if (assembler == null)
            {
                throw new ArgumentNullException("assembler",
                    "The assembler is required and cannot be null (or Nothing).");
            }
            if (configuration == null)
            {
                throw new ArgumentNullException("configuration",
                    "The configuration is required and cannot be null (or Nothing).");
            }

            _assembler = assembler;

            this.WriteMessage(MessageLevel.Info, "Instantiating component.");
        }

        #endregion

        #region Public Properties

        public BuildAssembler BuildAssembler
        {
            get
            {
                return _assembler;
            }
        }

        #endregion

        #region Protected Properties

        //private MessageHandler handler;

        protected static Dictionary<string, object> Data
        {
            get
            {
                return _data;
            }
        }

        #endregion

        #region Public Methods

        public abstract void Apply(XmlDocument document, string key);

        public virtual void Apply(XmlDocument document)
        {
            this.Apply(document, null);
        }

        #endregion

        #region Protected Methods

        // component messaging facility

        protected virtual void OnComponentEvent(EventArgs e)
        {
            if (_thisType == null)
            {
                _thisType = this.GetType();
            }

            _assembler.RaiseComponentEvent(_thisType, e);
        }

        protected virtual void WriteMessage(MessageLevel level, string message)
        {
            if (level == MessageLevel.Ignore)
                return;

            MessageWriter writer = _assembler.MessageWriter;
            if (writer != null)
            {
                if (_thisType == null)
                {
                    _thisType = this.GetType();
                }

                writer.Write(_thisType, level, message);
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            this.Dispose(true);

            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
