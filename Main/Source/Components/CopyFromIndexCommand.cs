// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.Indexed;

namespace Sandcastle.Components
{
    public sealed class CopyFromIndexCommand
    {
        #region Private Fields

        private String target;
        private String attribute;
        private String ignoreCase;

        private MessageLevel missingEntry; 
        private MessageLevel missingSource;
        private MessageLevel missingTarget;

        private XPathExpression key; 
        private XPathExpression source;
        private IndexedDocumentSource cache;

        #endregion

        #region Constructors and Destructor

        private CopyFromIndexCommand()
        {
            missingEntry  = MessageLevel.Ignore; 
            missingSource = MessageLevel.Ignore;
            missingTarget = MessageLevel.Ignore;
        }

        public CopyFromIndexCommand(IndexedDocumentSource sourceIndex, 
            string keyXPath, string sourceXPath, string targetXPath, 
            string attributeValue, string ignoreCaseValue) : this()
        {
            BuildComponentExceptions.NotNull(sourceIndex, "sourceIndex");

            this.cache = sourceIndex;

            if (String.IsNullOrEmpty(keyXPath))
            {
                key = XPathExpression.Compile("string($key)");
            }
            else
            {
                key = XPathExpression.Compile(keyXPath);
            }

            source     = XPathExpression.Compile(sourceXPath);
            target     = targetXPath;
            attribute  = attributeValue;
            ignoreCase = ignoreCaseValue;
        }

        #endregion

        #region Public Properties

        public IndexedDocumentSource Index
        {
            get
            {
                return cache;
            }
        }

        public XPathExpression Key
        {
            get
            {
                return key;
            }
        }

        public XPathExpression Source
        {
            get
            {
                return source;
            }
        }

        public String Target
        {
            get
            {
                return target;
            }
        }

        public String Attribute
        {
            get
            {
                return attribute;
            }
        }

        public String IgnoreCase
        {
            get
            {
                return ignoreCase;
            }
        }

        public MessageLevel MissingEntry
        {
            get
            {
                return missingEntry;
            }
            set
            {
                missingEntry = value;
            }
        }

        public MessageLevel MissingSource
        {
            get
            {
                return missingSource;
            }
            set
            {
                missingSource = value;
            }
        }

        public MessageLevel MissingTarget
        {
            get
            {
                return missingTarget;
            }
            set
            {
                missingTarget = value;
            }
        }

        #endregion 
    }
}
