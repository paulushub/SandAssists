using System;
using System.IO;
using System.Web;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.Others;

namespace Sandcastle.Components
{
    public sealed class ReferencePreTransComponent : PreTransComponent
    {
        #region Private Fields

        // For the auto-documentations...
        private bool   _applyAutoDocument;

        private bool   _documentWarning;
        private bool   _documentConstructors;
        private bool   _documentDisposeMethods;

        private string _autoDocContructor;
        private string _autoDocStaticConstructor;

        private string _autoDocDisposeMethod;
        private string _autoDocParamDisposeMethod;
        private string _autoDocDisposeMethodParam;

        // For the missing tags comments...
        private bool   _applyMissingTags;

        private bool   _missingLog;
        private bool   _missingLogInXml;
        private bool   _missingWarning;
        private bool   _missingIndicate;

        private bool   _rootTags;
        private bool   _includeTargetTags;
        private bool   _namespaceTags;
        private bool   _parameterTags;
        private bool   _remarkTags;
        private bool   _returnTags;
        private bool   _summaryTags;
        private bool   _typeParameterTags;
        private bool   _valueTags;
        private bool   _exceptionTags;

        private string _missingTagMessage;
        private string _missingParamTagMessage;
        private string _missingExceptionTextMessage;
        private string _missingIncludeTargetMessage;

        private string _missingLogFile;

        private StreamWriter _textWriter;
        private XmlWriter    _xmlWriter;

        #endregion

        #region Constructors and Destructor

        public ReferencePreTransComponent(BuildAssembler assembler, 
            XPathNavigator configuration) : base(assembler, configuration)
        {
            try
            {
                ParseAutoDocument(configuration);
                ParseMissingsTags(configuration);
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
            if (_applyAutoDocument || _applyMissingTags)
            {
                XPathNavigator navigator = document.CreateNavigator();

                if (_applyAutoDocument)
                {
                    this.ApplyAutoDocument(navigator, key);
                }

                if (_applyMissingTags)
                {
                    this.ApplyMissingTags(navigator, key);
                }
            }
        }

        #endregion

        #region Protected Methods

        #endregion

        #region Private Methods

        #region ApplyAutoDocument Method

        private void ApplyAutoDocument(XPathNavigator documentNavigator, string key)
        {
            if (_documentConstructors)
            {
                if ((key.StartsWith("Overload", StringComparison.Ordinal)  &&
                    (key.IndexOf(".ctor", StringComparison.Ordinal) != -1  ||
                    key.IndexOf(".#ctor", StringComparison.Ordinal) != -1)))
                {
                    // The format: Overload:TestLibrary.DClass.#ctor

                    // Extract: M:TestLibrary.DClass.#ctor
                    string targetKey = "M:" + key.Substring(key.IndexOf(':') + 1);

                    targetKey = targetKey.Replace("..ctor", ".#ctor");

                    string ctorExpression = "document/reference/elements/element[starts-with(@api, '" 
                        + targetKey + "')]";

                    XPathNodeIterator iterator = documentNavigator.Select(ctorExpression);
                    if (iterator != null && iterator.Count != 0)
                    {
                        foreach (XPathNavigator targetNavigator in iterator)
                        {
                            this.ProcessDocumentContructor(targetNavigator, targetKey);
                        } 
                    }
                }
                else if (key.StartsWith("AllMembers", StringComparison.Ordinal))
                {
                    // The format: AllMembers.T:TestLibrary.DClass

                    // Extract: M:TestLibrary.DClass
                    string baseKey = key.Substring(key.IndexOf(':') + 1);
                    string targetKey = "M:" + baseKey;

                    if (key.IndexOf(".ctor", StringComparison.Ordinal) == -1 &&
                        key.IndexOf(".#ctor", StringComparison.Ordinal) == -1)
                    {
                        targetKey += ".#ctor";
                    }
                    else
                    {
                        targetKey = targetKey.Replace("..ctor", ".#ctor");
                    }

                    string ctorExpression = "document/reference/elements/element[starts-with(@api, '" 
                        + targetKey + "')]";

                    XPathNodeIterator iterator = documentNavigator.Select(ctorExpression);
                    if (iterator != null && iterator.Count != 0)
                    {
                        foreach (XPathNavigator targetNavigator in iterator)
                        {
                            this.ProcessDocumentContructor(targetNavigator, targetKey);
                        } 
                    }
                    else
                    {
                        // Try the format: Overload:TestLibrary.DClass.#ctor/element...
                        ctorExpression = "document/reference/elements/element/element[starts-with(@api, '"
                            + targetKey + "')]";
                        iterator = documentNavigator.Select(ctorExpression);
                        if (iterator != null && iterator.Count != 0)
                        {
                            foreach (XPathNavigator targetNavigator in iterator)
                            {
                                this.ProcessDocumentContructor(targetNavigator, targetKey);
                            }
                        }
                    }
                }
                else if ((key[0] == 'M' && key[1] == ':') &&
                    (key.IndexOf(".ctor", StringComparison.Ordinal) != -1 ||
                    key.IndexOf(".#ctor", StringComparison.Ordinal) != -1))
                {
                    // The format: M:TestLibrary.DClass.#ctor(..ctor)

                    string targetKey = key.Replace("..ctor", ".#ctor");

                    XPathNavigator targetNavigator = documentNavigator.SelectSingleNode("document/comments");
                    if (targetNavigator != null)
                    {
                        this.ProcessDocumentContructor(targetNavigator, targetKey);
                    }
                }
            }

            if (_documentDisposeMethods)
            {
                if (key.StartsWith("AllMembers", StringComparison.Ordinal) ||
                    key.StartsWith("Methods", StringComparison.Ordinal))
                {
                    string targetKey = "M:" + key.Substring(key.IndexOf(':') + 1);

                    if (!key.EndsWith(".Dispose", StringComparison.Ordinal))
                    {
                        targetKey += ".Dispose";
                    }

                    string disposeExpression = "document/reference/elements/element[@api='" 
                        + targetKey + "']";
                    XPathNodeIterator iterator = documentNavigator.Select(disposeExpression);
                    if (iterator != null && iterator.Count != 0)
                    {
                        foreach (XPathNavigator targetNavigator in iterator)
                        {
                            this.ProcessDocumentDispose(documentNavigator, targetNavigator, targetKey);
                        }
                    }
                    else
                    {
                        disposeExpression = "document/reference/elements/element/element[@api='"
                            + targetKey + "']";
                        iterator = documentNavigator.Select(disposeExpression);
                        if (iterator != null && iterator.Count != 0)
                        {
                            foreach (XPathNavigator targetNavigator in iterator)
                            {
                                this.ProcessDocumentDispose(documentNavigator, targetNavigator, targetKey);
                            }
                        }
                    }
            
                    targetKey += "(System.Boolean)";
                    disposeExpression = "document/reference/elements/element[@api='" 
                        + targetKey + "']";
                    iterator = documentNavigator.Select(disposeExpression);
                    if (iterator != null && iterator.Count != 0)
                    {
                        foreach (XPathNavigator targetNavigator in iterator)
                        {
                            this.ProcessDocumentDispose(documentNavigator, targetNavigator, targetKey);
                        }
                    }
                    else
                    {
                        disposeExpression = "document/reference/elements/element/element[@api='"
                            + targetKey + "']";
                        iterator = documentNavigator.Select(disposeExpression);
                        if (iterator != null && iterator.Count != 0)
                        {
                            foreach (XPathNavigator targetNavigator in iterator)
                            {
                                this.ProcessDocumentDispose(documentNavigator, targetNavigator, targetKey);
                            }
                        }
                    }
                }
                else if (key.StartsWith("Overload", StringComparison.Ordinal)  &&
                    key.EndsWith(".Dispose", StringComparison.Ordinal))
                {
                    string targetKey = "M:" + key.Substring(key.IndexOf(':') + 1);

                    string disposeExpression = "document/reference/elements/element[@api='" 
                        + targetKey + "']";
                    XPathNodeIterator iterator = documentNavigator.Select(disposeExpression);
                    if (iterator != null && iterator.Count != 0)
                    {
                        foreach (XPathNavigator targetNavigator in iterator)
                        {
                            this.ProcessDocumentDispose(documentNavigator, targetNavigator, targetKey);
                        }
                    }
            
                    targetKey += "(System.Boolean)";
                    disposeExpression = "document/reference/elements/element[@api='" 
                        + targetKey + "']";
                    iterator = documentNavigator.Select(disposeExpression);
                    if (iterator != null && iterator.Count != 0)
                    {
                        foreach (XPathNavigator targetNavigator in iterator)
                        {
                            this.ProcessDocumentDispose(documentNavigator, targetNavigator, targetKey);
                        }
                    }
                }
                else if ((key[0] == 'M' && key[1] == ':') &&
                    (key.EndsWith(".Dispose", StringComparison.Ordinal) ||
                    key.EndsWith(".Dispose(System.Boolean)", StringComparison.Ordinal)))
                {
                    string targetKey = key;

                    XPathNavigator targetNavigator = documentNavigator.SelectSingleNode("document/comments");
                    this.ProcessDocumentDispose(documentNavigator, targetNavigator, targetKey);
                }
            }
        }

        #endregion

        #region ApplyMissingTags Method

        private void ApplyMissingTags(XPathNavigator documentNavigator, string key)
        {
            if (key[0] == 'R')
            {
                // 1. Handle the summaries...
                if (_rootTags && _summaryTags)
                {
                    XPathNavigator commentsNavigator = documentNavigator.SelectSingleNode("document/comments");
                    if (commentsNavigator != null)
                    {
                        this.ProcessMissingTag(documentNavigator, commentsNavigator, "summary", key, key);
                    }
                }
                if (_namespaceTags && _summaryTags)
                {
                    string textExpression = "document/reference/elements/element";

                    XPathNodeIterator iterator = documentNavigator.Select(textExpression);
                    if (iterator != null && iterator.Count != 0)
                    {
                        foreach (XPathNavigator targetNavigator in iterator)
                        {
                            this.ProcessMissingTag(documentNavigator, targetNavigator, "summary",
                                targetNavigator.GetAttribute("api", String.Empty), key);
                        }
                    }
                }
            }
            else if (key[0] == 'N')
            {
                if (_summaryTags)
                {
                    string textExpression = "document/reference/elements/element";

                    XPathNodeIterator iterator = documentNavigator.Select(textExpression);
                    if (iterator != null && iterator.Count != 0)
                    {
                        foreach (XPathNavigator targetNavigator in iterator)
                        {
                            this.ProcessMissingTag(documentNavigator, targetNavigator, "summary",
                                targetNavigator.GetAttribute("api", String.Empty), key);
                        }
                    }

                    if (_namespaceTags)
                    {
                        XPathNavigator commentsNavigator = documentNavigator.SelectSingleNode(
                            "document/comments");
                        if (commentsNavigator != null && !key.EndsWith("TocExcluded", 
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.ProcessMissingTag(documentNavigator, commentsNavigator, "summary", key, key);
                        }
                    }
                }
            }
            else if (key.StartsWith("Overload", StringComparison.Ordinal))
            {                   
                if (_summaryTags)
                {   
                    // Extract: M:TestLibrary.DClass.AMethod
                    string targetKey = "M:" + key.Substring(key.IndexOf(':') + 1);

                    string textExpression = "document/reference/elements/element[starts-with(@api, '"
                        + targetKey + "')]";
                    //string textExpression = "document/reference/elements/element";

                    XPathNodeIterator iterator = documentNavigator.Select(textExpression);
                    if (iterator != null && iterator.Count != 0)
                    {
                        foreach (XPathNavigator targetNavigator in iterator)
                        {
                            this.ProcessMissingTag(documentNavigator, targetNavigator, "summary",
                                targetNavigator.GetAttribute("api", String.Empty), key);
                        }
                    }
                }
            }
            else if (key.StartsWith("Properties", StringComparison.Ordinal))
            {
                if (_summaryTags)
                {   
                    // Extract: P:TestLibrary.DClass.AProperty
                    string targetKey = "P:" + key.Substring(key.IndexOf(':') + 1);

                    string textExpression = "document/reference/elements/element[starts-with(@api, '"
                        + targetKey + "')]";
                    //string textExpression = "document/reference/elements/element";

                    XPathNodeIterator iterator = documentNavigator.Select(textExpression);
                    if (iterator != null && iterator.Count != 0)
                    {
                        foreach (XPathNavigator targetNavigator in iterator)
                        {
                            this.ProcessMissingTag(documentNavigator, targetNavigator, "summary",
                                targetNavigator.GetAttribute("api", String.Empty), key);
                        }
                    }
                }
            }
            else if (key.StartsWith("AllMembers", StringComparison.Ordinal) ||
                    key.StartsWith("Methods", StringComparison.Ordinal))
            {
                if (_summaryTags)
                {   
                    // Extract: M:TestLibrary.DClass
                    string baseKey = key.Substring(key.IndexOf(':') + 1);
                    string targetKey = "M:" + baseKey;

                    string textExpression = "document/reference/elements/element[starts-with(@api, '"
                        + targetKey + "')]";

                    //string textExpression = "document/reference/elements/element";
                    XPathNodeIterator iterator = documentNavigator.Select(textExpression);
                    if (iterator != null && iterator.Count != 0)
                    {
                        foreach (XPathNavigator targetNavigator in iterator)
                        {
                            this.ProcessMissingTag(documentNavigator, targetNavigator, "summary",
                                targetNavigator.GetAttribute("api", String.Empty), key);
                        }
                    }
                    // Try the format: Overload:TestLibrary.DClass.#ctor/element...
                    textExpression = "document/reference/elements/element/element[starts-with(@api, '"
                        + targetKey + "')]";
                    //textExpression = "document/reference/elements/element/element";
                    iterator = documentNavigator.Select(textExpression);
                    if (iterator != null && iterator.Count != 0)
                    {
                        foreach (XPathNavigator targetNavigator in iterator)
                        {
                            this.ProcessMissingTag(documentNavigator, targetNavigator, "summary",
                                targetNavigator.GetAttribute("api", String.Empty), key);
                        }
                    }

                    // Extract: P:TestLibrary.DClass, but not for "Methods"...
                    if (!key.StartsWith("Methods", StringComparison.Ordinal))
                    {
                        targetKey = "P:" + baseKey;

                        textExpression = "document/reference/elements/element[starts-with(@api, '"
                            + targetKey + "')]";
                        iterator = documentNavigator.Select(textExpression);
                        if (iterator != null && iterator.Count != 0)
                        {
                            foreach (XPathNavigator targetNavigator in iterator)
                            {
                                this.ProcessMissingTag(documentNavigator, targetNavigator, "summary",
                                    targetNavigator.GetAttribute("api", String.Empty), key);
                            }
                        }
                    }
                }
            }
            else // Finally, for the individual pages...
            {
                XPathNavigator commentsNavigator = documentNavigator.SelectSingleNode("document/comments");
                if (commentsNavigator == null)
                {
                    return;
                }

                // 1. Handle the summaries...
                if (_summaryTags)
                {
                    this.ProcessMissingTag(documentNavigator, commentsNavigator, "summary", key, key);
                }
                // 2. Handle the include targets...
                if (_includeTargetTags)
                {
                    this.ProcessMissingIncludeTag(documentNavigator, commentsNavigator, key);
                }
                // 3. Handle the remarks for all...
                if (_remarkTags && key[0] != 'N') // Only namespaces do not have remarks...
                {
                    this.ProcessMissingTag(documentNavigator, commentsNavigator, "remarks", key, key);
                }

                // 4. Handle the properties, methods and types (classes, structures and enumerations)...
                if (key[0] == 'P')
                {
                    if (_valueTags)
                    {
                        this.ProcessMissingTag(documentNavigator, commentsNavigator, "value", key, key);
                    }
                    if (_exceptionTags)
                    {
                        this.ProcessMissingExceptionTag(documentNavigator, commentsNavigator, key);
                    }
                }
                else if (key[0] == 'M')
                {
                    if (_typeParameterTags)
                    {
                        XPathNodeIterator iterator = documentNavigator.Select(
                            "document/reference/templates/template");
                        if (iterator != null && iterator.Count != 0)
                        {
                            foreach (XPathNavigator targetNavigator in iterator)
                            {
                                this.ProcessMissingTag(documentNavigator, commentsNavigator, "typeparam",
                                    targetNavigator.GetAttribute("name", String.Empty), key);
                            }
                        }
                    }
                    if (_parameterTags)
                    {
                        XPathNodeIterator iterator = documentNavigator.Select(
                            "document/reference/parameters/parameter");
                        if (iterator != null && iterator.Count != 0) // the method must have parameters...
                        {
                            foreach (XPathNavigator targetNavigator in iterator)
                            {
                                this.ProcessMissingTag(documentNavigator, commentsNavigator, "param",
                                    targetNavigator.GetAttribute("name", String.Empty), key);
                            }
                        }
                    }
                    if (_returnTags)
                    {
                        XPathNavigator returnsNavigator = documentNavigator.SelectSingleNode(
                            "document/reference/returns");
                        if (returnsNavigator != null)  // the method must have a return...
                        {
                            this.ProcessMissingTag(documentNavigator, 
                                commentsNavigator, "returns", key, key);
                        }
                    }
                    if (_exceptionTags)
                    {
                        this.ProcessMissingExceptionTag(documentNavigator,
                            commentsNavigator, key);
                    }
                }
                else if (key[0] == 'T')
                {
                    if (_typeParameterTags)
                    {
                        XPathNodeIterator iterator = documentNavigator.Select(
                            "document/reference/templates/template");
                        if (iterator != null && iterator.Count != 0)
                        {
                            foreach (XPathNavigator targetNavigator in iterator)
                            {
                                this.ProcessMissingTag(documentNavigator, targetNavigator, "typeparam",
                                    targetNavigator.GetAttribute("name", String.Empty), key);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region ProcessMissingTag Method

        private void ProcessMissingTag(XPathNavigator documentNavigator, 
            XPathNavigator commentsNavigator, string targetName, 
            string targetKey, string documentKey)
        {
            string tagText = String.Empty;
            XPathNavigator targetTag = commentsNavigator.SelectSingleNode(targetName);

            if (targetTag == null)
            {
                commentsNavigator.AppendChildElement(commentsNavigator.Prefix, targetName, 
                    commentsNavigator.NamespaceURI, String.Empty);
                targetTag = commentsNavigator.SelectSingleNode(targetName);

                if (documentKey != null && documentKey.Length > 1 && 
                    documentKey[0] == 'M')
                {
                    if (targetName.Equals("param", StringComparison.Ordinal) ||
                        targetName.Equals("typeparam", StringComparison.Ordinal))
                    {
                        targetTag.CreateAttribute(targetTag.Prefix,
                            "name", targetTag.NamespaceURI, targetKey);
                    }
                }
            }
            else
            {
                tagText = targetTag.InnerXml;
                if (!String.IsNullOrEmpty(tagText))
                {
                    tagText = tagText.Trim();
                }
            }

            if (tagText != null && tagText.Length != 0)
            {
                return;
            }

            if (_missingWarning) // warn, if requested...
            {
                if (targetName.Equals("param", StringComparison.Ordinal))
                {
                    this.WriteMessage(MessageLevel.Warn, String.Format(
                        "Missing tag, <{0}>, documentation for {1} in {2}.", 
                        targetName, targetKey, documentKey));
                }
                else if (targetName.Equals("typeparam", StringComparison.Ordinal))
                {
                    this.WriteMessage(MessageLevel.Warn, String.Format(
                        "Missing tag, <{0}>, documentation for {1} with {2}.",
                        targetName, targetKey, documentKey));
                }
                else
                {
                    this.WriteMessage(MessageLevel.Warn, String.Format(
                        "Missing tag, <{0}>, documentation for {1}.", targetName, targetKey));
                }
            }
            if (_missingLog)     // log, if requested...
            {
                if (_missingLogInXml && _xmlWriter != null)
                {
                    _xmlWriter.WriteStartElement("missingTag");
                    _xmlWriter.WriteAttributeString("tag", targetName);
                    if (targetName.Equals("param", StringComparison.Ordinal))
                    {
                        _xmlWriter.WriteAttributeString("in", documentKey);
                    }
                    else if (targetName.Equals("typeparam", StringComparison.Ordinal))
                    {
                        _xmlWriter.WriteAttributeString("with", documentKey);
                    }
                    _xmlWriter.WriteString(targetKey);
                    _xmlWriter.WriteEndElement();
                }
                else if (_textWriter != null)
                {
                    if (targetName.Equals("param", StringComparison.Ordinal))
                    {
                        _textWriter.WriteLine(String.Format("Missing tag, <{0}>, documentation for {1} in {2}.",
                            targetName, targetKey, documentKey));
                    }
                    else if (targetName.Equals("typeparam", StringComparison.Ordinal))
                    {
                        _textWriter.WriteLine(String.Format("Missing tag, <{0}>, documentation for {1} with {2}.",
                            targetName, targetKey, documentKey));
                    }
                    else
                    {
                        _textWriter.WriteLine(String.Format("Missing tag, <{0}>, documentation for {1}.",
                            targetName, targetKey));
                    }
                }
            }
            if (!_missingIndicate)  // only write the document flags of the missing tags if requested...
            {
                return;
            }

            switch (targetName)
            {
                case "summary":
                case "remarks":
                case "returns":
                case "value":
                    targetTag.InnerXml = String.Format(_missingTagMessage, targetName, 
                        HttpUtility.HtmlEncode(targetKey));
                    break;
                case "param":
                case "typeparam":
                    targetTag.InnerXml = String.Format(_missingParamTagMessage, targetName,
                        HttpUtility.HtmlEncode(targetKey));
                    break;
                default:
                    throw new InvalidOperationException(
                        "There is not support for the tag: " + targetName);
            }
        }

        #endregion

        #region ProcessMissingExceptionTag Method

        private void ProcessMissingExceptionTag(XPathNavigator documentNavigator, 
            XPathNavigator commentsNavigator, string targetKey)
        {
            string targetName = "exception";
            XPathNavigator targetTag = commentsNavigator.SelectSingleNode(targetName);

            if (targetTag == null)
            {
                return;
            }

            string crefValue = targetTag.GetAttribute("cref", String.Empty);
            if (!String.IsNullOrEmpty(crefValue))
            {
                crefValue = crefValue.Trim();
                if (String.Equals(crefValue, "!:", StringComparison.Ordinal))
                {
                    crefValue = String.Empty;
                }
            }

            string tagText = targetTag.InnerXml;
            if (!String.IsNullOrEmpty(tagText))
            {
                tagText = tagText.Trim();
            }

            if (!String.IsNullOrEmpty(crefValue) && !String.IsNullOrEmpty(tagText))
            {
                return;
            }
            
            if (_missingWarning) // warn, if requested...
            {
                if (String.IsNullOrEmpty(crefValue))
                {
                    this.WriteMessage(MessageLevel.Warn, String.Format(
                        "Missing tag, <exception cref=\"?\"/>, 'cref' attribute for {0}.", targetKey));
                }
                if (String.IsNullOrEmpty(tagText))
                {
                    this.WriteMessage(MessageLevel.Warn, String.Format(
                        "Missing tag, <exception cref=\"{0}\">, documentation for {1}.", 
                        crefValue == null ? String.Empty : crefValue, targetKey));
                }
            }
            if (_missingLog)     // log, if requested...
            {
                if (_missingLogInXml && _xmlWriter != null)
                {
                    if (String.IsNullOrEmpty(crefValue))
                    {
                        _xmlWriter.WriteStartElement("missingTag");
                        _xmlWriter.WriteAttributeString("tag", "exception");
                        _xmlWriter.WriteAttributeString("attribute", "cref='???'");
                        _xmlWriter.WriteString(targetKey);
                        _xmlWriter.WriteEndElement();
                    }
                    if (String.IsNullOrEmpty(tagText))
                    {
                        _xmlWriter.WriteStartElement("missingTag");
                        _xmlWriter.WriteAttributeString("tag", String.Format(
                            "<exception cref=\"{0}\">", String.IsNullOrEmpty(crefValue) ? "?" : crefValue));
                        _xmlWriter.WriteString(targetKey);
                        _xmlWriter.WriteEndElement();
                    }
                }
                else if (_textWriter != null)
                {
                    if (String.IsNullOrEmpty(crefValue))
                    {
                        _textWriter.WriteLine(String.Format(
                            "Missing tag, <exception cref=\"?\"/>, 'cref' attribute for {0}.", targetKey));
                    }
                    if (String.IsNullOrEmpty(tagText))
                    {
                        _textWriter.WriteLine(String.Format(
                            "Missing tag, <exception cref=\"{0}\">, documentation for {1}.",
                            crefValue == null ? String.Empty : crefValue, targetKey));
                    }
                }
            }
            if (_missingIndicate)  // only write the document flags of the missing tags if requested...
            {
                if (String.IsNullOrEmpty(tagText))
                {
                    targetTag.InnerXml = String.Format(_missingExceptionTextMessage,
                        HttpUtility.HtmlEncode(String.IsNullOrEmpty(crefValue) ? "?" : crefValue),
                        HttpUtility.HtmlEncode(targetKey));
                }
            }
       }

        #endregion

        #region ProcessMissingIncludeTag Method

        private void ProcessMissingIncludeTag(XPathNavigator documentNavigator, 
            XPathNavigator commentsNavigator, string targetKey)
        {
            XPathNodeIterator includeIterator = commentsNavigator.Select("include");
            if (includeIterator == null || includeIterator.Count == 0)
            {
                return;
            }

            XPathNavigator targetTag = null;
            StringBuilder builder    = null;
            if (_missingIndicate)  // only write the document flags of the missing tags if requested...
            {
                builder = new StringBuilder();

                // We do have some includes, which could define any tag (summary, param, remarks...).
                // For quicker attention, we append the information to the summary tag...
                string targetName = "summary";

                targetTag = commentsNavigator.SelectSingleNode(targetName);

                if (targetTag == null)
                {
                    commentsNavigator.AppendChildElement(commentsNavigator.Prefix, targetName,
                        commentsNavigator.NamespaceURI, String.Empty);
                    targetTag = commentsNavigator.SelectSingleNode(targetName);
                }
                else
                {
                    builder.Append(targetTag.InnerXml);
                }
            }

            foreach (XPathNavigator includeNavigator in includeIterator)
            {
                string includeFile  = includeNavigator.GetAttribute("file", String.Empty);
                string includeXPath = includeNavigator.GetAttribute("path", String.Empty);

                if (_missingWarning) // warn, if requested...
                {
                    this.WriteMessage(MessageLevel.Warn, String.Format(
                        "Missing tag, <include file='{0}' path='{1}'/>, documentation for {2}.", includeFile, 
                        includeXPath, targetKey));
                }
                if (_missingLog)     // log, if requested...
                {
                    if (_missingLogInXml && _xmlWriter != null)
                    {
                        _xmlWriter.WriteStartElement("missingTag");
                        _xmlWriter.WriteAttributeString("tag", String.Format(
                            "[include file='{0}' path='{1}'], documentation for {2}.", includeFile, 
                            includeXPath, targetKey));
                        _xmlWriter.WriteString(targetKey);
                        _xmlWriter.WriteEndElement();
                    }
                    else if (_textWriter != null)
                    {
                        _textWriter.WriteLine(String.Format(
                            "Missing tag, <include file='{0}' path='{1}'/>, documentation for {2}.", 
                            includeFile, includeXPath, targetKey));
                    }
                }
                if (_missingIndicate)  // only write the document flags of the missing tags if requested...
                {
                    builder.AppendLine();
                    builder.AppendFormat(_missingIncludeTargetMessage,
                        HttpUtility.HtmlEncode(includeFile),
                        HttpUtility.HtmlEncode(includeXPath),
                        HttpUtility.HtmlEncode(targetKey));
                } 
            }

            if (targetTag != null && builder != null)
            {
                targetTag.InnerXml = builder.ToString();
            }
        }

        #endregion

        #region ProcessDocumentContructor Method

        private void ProcessDocumentContructor(XPathNavigator navigator, string targetKey)
        {
            string innerXml  = null;
            if (targetKey.Contains("#cctor")) // static constructor...
            {
                string className = HttpUtility.HtmlEncode(targetKey.Substring(2,
                    targetKey.IndexOf(".#cctor", StringComparison.Ordinal) - 2));
                innerXml = String.Format(_autoDocStaticConstructor, className);
            }
            else // all other constructors...
            {
                string className = HttpUtility.HtmlEncode(targetKey.Substring(2,
                    targetKey.IndexOf(".#ctor", StringComparison.Ordinal) - 2));
                innerXml = String.Format(_autoDocContructor, className);
            }

            if (_documentWarning)
            {
                this.WriteMessage(MessageLevel.Warn, 
                    "Automatically documenting constructor: " + targetKey);
            }

            XPathNavigator summaryTag = navigator.SelectSingleNode("summary");

            if (summaryTag == null)
            {
                navigator.AppendChildElement(navigator.Prefix, "summary", navigator.NamespaceURI,
                    String.Empty);
                if (navigator.MoveToChild("summary", String.Empty))
                {
                    navigator.InnerXml = innerXml;
                }

                return;
            }
            else
            {
                string tagText = summaryTag.InnerXml;
                if (!String.IsNullOrEmpty(tagText))
                {
                    tagText = tagText.Trim();
                }

                if (tagText != null && tagText.Length != 0)
                {
                    return;
                }

                summaryTag.InnerXml = innerXml;
            }
        }

        #endregion

        #region ProcessDocumentDispose Method

        private void ProcessDocumentDispose(XPathNavigator documentNavigator, 
            XPathNavigator targetNavigator, string targetKey)
        {
            string innerXml = null;
            bool hasParam   = false;
            if (targetKey.EndsWith(".Dispose(System.Boolean)", StringComparison.Ordinal))
            {
                hasParam = true;
                string className = HttpUtility.HtmlEncode(targetKey.Substring(2, targetKey.Length - 26));
                innerXml  = String.Format(_autoDocParamDisposeMethod, className);
            }
            else
            {
                string className = HttpUtility.HtmlEncode(targetKey.Substring(2, targetKey.Length - 10));
                innerXml  = String.Format(_autoDocDisposeMethod, className);
            }

            if (_documentWarning)
            {
                this.WriteMessage(MessageLevel.Warn,
                    "Automatically documenting dispose method: " + targetKey);
            }

            XPathNavigator summaryTag = targetNavigator.SelectSingleNode("summary");

            if (summaryTag == null)
            {
                // For the Dispose(System.Boolean), we also check and create the param summary...
                if (hasParam)
                {
                    if (String.Equals(targetNavigator.LocalName, "comments", StringComparison.Ordinal))
                    {
                        bool wasEmptyElement = targetNavigator.IsEmptyElement;

                        XPathNavigator paramNode = null;
                        targetNavigator.AppendChildElement(targetNavigator.Prefix, "summary", targetNavigator.NamespaceURI,
                            String.Empty);
                        if (wasEmptyElement)
                        {
                            targetNavigator.AppendChildElement(targetNavigator.Prefix, "param", targetNavigator.NamespaceURI,
                               String.Empty);
                            paramNode = targetNavigator.SelectSingleNode("param");
                            if (paramNode != null)
                            {
                                XPathNavigator paramNameNode = documentNavigator.SelectSingleNode(
                                    "document/reference/parameters/parameter");
                                if (paramNameNode != null)
                                {
                                    string paramName = paramNameNode.GetAttribute("name", String.Empty);
                                    if (!String.IsNullOrEmpty(paramName))
                                    {
                                        paramNode.CreateAttribute(paramNode.Prefix, "name",
                                            paramNode.NamespaceURI, paramName);
                                    }
                                }
                            }
                        }
                        else
                        {
                            paramNode = targetNavigator.SelectSingleNode("param");
                            if (paramNode == null)
                            {
                                targetNavigator.AppendChildElement(targetNavigator.Prefix, 
                                    "param", targetNavigator.NamespaceURI, String.Empty);
                                paramNode = targetNavigator.SelectSingleNode("param");
                                if (paramNode != null)
                                {
                                    XPathNavigator paramNameNode = documentNavigator.SelectSingleNode(
                                        "document/reference/parameters/parameter");
                                    if (paramNameNode != null)
                                    {
                                        string paramName = paramNameNode.GetAttribute("name", String.Empty);
                                        if (!String.IsNullOrEmpty(paramName))
                                        {
                                            paramNode.CreateAttribute(paramNode.Prefix, "name",
                                                paramNode.NamespaceURI, paramName);
                                        }
                                    }
                                }
                            }
                        }
                        XPathNavigator summaryNode = targetNavigator.SelectSingleNode("summary");
                        if (summaryNode != null)
                        {
                            summaryNode.InnerXml = innerXml;
                        }
                        if (paramNode == null)
                        {
                            paramNode = targetNavigator.SelectSingleNode("param");
                        }
                        if (paramNode != null)
                        {
                            string paramText = paramNode.InnerXml;
                            if (paramText != null)
                            {
                                paramText = paramText.Trim();
                            }
                            if (String.IsNullOrEmpty(paramText))
                            {
                                paramNode.InnerXml = _autoDocDisposeMethodParam;
                            }
                        }
                    }
                    else
                    {
                        targetNavigator.AppendChildElement(targetNavigator.Prefix, "summary", targetNavigator.NamespaceURI,
                            String.Empty);
                        if (targetNavigator.MoveToChild("summary", String.Empty))
                        {
                            targetNavigator.InnerXml = innerXml;
                        }
                    }
                }
                else
                {
                    targetNavigator.AppendChildElement(targetNavigator.Prefix, "summary", targetNavigator.NamespaceURI,
                        String.Empty);
                    if (targetNavigator.MoveToChild("summary", String.Empty))
                    {
                        targetNavigator.InnerXml = innerXml;
                    }
                }
            }
            else
            {
                string tagText = summaryTag.InnerXml;
                if (!String.IsNullOrEmpty(tagText))
                {
                    tagText = tagText.Trim();
                }

                if (String.IsNullOrEmpty(tagText))
                {
                    summaryTag.InnerXml = innerXml;
                }

                // If it has parameter, check for availability of the summary...
                if (hasParam)
                {
                    XPathNavigator paramNode = targetNavigator.SelectSingleNode("param");
                    if (paramNode == null)
                    {
                        targetNavigator.AppendChildElement(targetNavigator.Prefix, "param", targetNavigator.NamespaceURI,
                           String.Empty);
                        paramNode = targetNavigator.SelectSingleNode("param");
                        if (paramNode != null)
                        {
                            XPathNavigator paramNameNode = documentNavigator.SelectSingleNode(
                                "document/reference/parameters/parameter");
                            if (paramNameNode != null)
                            {
                                string paramName = paramNameNode.GetAttribute("name", String.Empty);
                                if (!String.IsNullOrEmpty(paramName))
                                {
                                    paramNode.CreateAttribute(paramNode.Prefix, "name",
                                        paramNode.NamespaceURI, paramName);
                                }
                            }
                        }
                    }

                    string paramText = paramNode.InnerXml;
                    if (paramText != null)
                    {
                        paramText = paramText.Trim();
                    }
                    if (String.IsNullOrEmpty(paramText))
                    {
                        paramNode.InnerXml = _autoDocDisposeMethodParam;
                    }
                }
            }
        }

        #endregion

        #region ParseMissingsTags Method

        private void ParseMissingsTags(XPathNavigator configuration)
        {
            //<missingTags enabled="true" warn="true" indicate="true" log="true" logXml="true" logFile="">
            //    <tags roots="true" includeTargets="true" namespaces="true" parameters="true" remarks="true" returns="true" summaries="true" typeParameters="true" values="true" exceptions="true"/>
            //    <MissingTag message=""/>
            //    <MissingParamTag message=""/>
            //    <MissingIncludeTarget message=""/>
            //    <MissingExceptionReference message=""/>
            //    <MissingExceptionText message=""/>
            //</missingTags> 

            _applyMissingTags            = false;

            _missingLog                  = true;
            _missingLogInXml             = true;
            _missingWarning              = true;
            _missingIndicate             = false;

            _rootTags                    = false;
            _includeTargetTags           = false;
            _namespaceTags               = true;
            _parameterTags               = true;
            _remarkTags                  = false;
            _returnTags                  = true;
            _summaryTags                 = true;
            _typeParameterTags           = true;
            _valueTags                   = false;
            _exceptionTags               = false;

            _missingLogFile              = null;

            _missingTagMessage           =
                "<p style=\"color:#ff4500;font-size:8pt;font-weight:bold;\">[Missing Tag]: The &lt;{0}&gt; documentation for '{1}'.</p>";
            _missingParamTagMessage      =
                "<p style=\"color:#ff4500;font-size:8pt;font-weight:bold;\">[Missing Tag]: The &lt;param name='{0}'/&gt; documentation for '{1}'.</p>";
            _missingIncludeTargetMessage =
                "<p style=\"color:#ff4500;font-size:8pt;font-weight:bold;\">[Missing Tag]: The &lt;include file='{0}' path='{1}'/&gt; target documentation for '{2}'.</p>";
            _missingExceptionTextMessage =
                "<p style=\"color:#ff4500;font-size:8pt;font-weight:bold;\">[Missing Tag]: The &lt;exception cref='{0}'/&gt; documentation for '{1}'.</p>";

            string tempText = String.Empty;
            BuildLocalizedContents localizedContents = BuildLocalizedContents.Instance;
            if (localizedContents != null && localizedContents.IsInitialized)
            {
                tempText = localizedContents["MissingTagMessage"];
                if (!String.IsNullOrEmpty(tempText))
                {
                    _missingTagMessage = tempText;
                }
                tempText = localizedContents["MissingParamTagMessage"];
                if (!String.IsNullOrEmpty(tempText))
                {
                    _missingParamTagMessage = tempText;
                }
                tempText = localizedContents["MissingIncludeTargetMessage"];
                if (!String.IsNullOrEmpty(tempText))
                {
                    _missingIncludeTargetMessage = tempText;
                }
            }

            XPathNavigator containerNavigator = configuration.SelectSingleNode("missingTags");
            if (containerNavigator == null)
            {
                return;
            }
            tempText = containerNavigator.GetAttribute("enabled", String.Empty);
            if (!String.IsNullOrEmpty(tempText))
            {
                _applyMissingTags = Convert.ToBoolean(tempText);
            }
            if (!_applyMissingTags)
            {
                return;
            }
            tempText = containerNavigator.GetAttribute("warn", String.Empty);
            if (!String.IsNullOrEmpty(tempText))
            {
                _missingWarning = Convert.ToBoolean(tempText);
            }
            tempText = containerNavigator.GetAttribute("indicate", String.Empty);
            if (!String.IsNullOrEmpty(tempText))
            {
                _missingIndicate = Convert.ToBoolean(tempText);
            }
            tempText = containerNavigator.GetAttribute("log", String.Empty);
            if (!String.IsNullOrEmpty(tempText))
            {
                _missingLog = Convert.ToBoolean(tempText);
            }
            tempText = containerNavigator.GetAttribute("logXml", String.Empty);
            if (!String.IsNullOrEmpty(tempText))
            {
                _missingLogInXml = Convert.ToBoolean(tempText);
            }
            tempText = containerNavigator.GetAttribute("logFile", String.Empty);
            if (!String.IsNullOrEmpty(tempText))
            {
                _missingLogFile = Path.GetFullPath(
                    Environment.ExpandEnvironmentVariables(tempText));
            }

            // <tags roots="true" includeTargets="true" namespaces="true" parameters="true" 
            //     remarks="true" returns="true" summaries="true" typeParameters="true" 
            //     values="true" exceptions="true"/>
            XPathNavigator navigator = containerNavigator.SelectSingleNode("tags");
            if (navigator != null)
            {
                tempText = navigator.GetAttribute("roots", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _rootTags = Convert.ToBoolean(tempText);
                }
                tempText = navigator.GetAttribute("includeTargets", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _includeTargetTags = Convert.ToBoolean(tempText);
                }
                tempText = navigator.GetAttribute("namespaces", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _namespaceTags = Convert.ToBoolean(tempText);
                }
                tempText = navigator.GetAttribute("parameters", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _parameterTags = Convert.ToBoolean(tempText);
                }
                tempText = navigator.GetAttribute("remarks", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _remarkTags = Convert.ToBoolean(tempText);
                }
                tempText = navigator.GetAttribute("returns", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _returnTags = Convert.ToBoolean(tempText);
                }
                tempText = navigator.GetAttribute("summaries", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _summaryTags = Convert.ToBoolean(tempText);
                }
                tempText = navigator.GetAttribute("typeParameters", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _typeParameterTags = Convert.ToBoolean(tempText);
                }
                tempText = navigator.GetAttribute("values", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _valueTags = Convert.ToBoolean(tempText);
                }
                tempText = navigator.GetAttribute("exceptions", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _exceptionTags = Convert.ToBoolean(tempText);
                }
            }

            // <MissingTag message=""/>
            // <MissingParamTag message=""/>
            // <MissingIncludeTarget message=""/>
            // <MissingExceptionReference message=""/>
            // <MissingExceptionText message=""/>
            navigator = containerNavigator.SelectSingleNode("MissingTag");
            if (navigator != null)
            {
                tempText = navigator.GetAttribute("message", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _missingTagMessage = tempText;
                }
            }
            navigator = containerNavigator.SelectSingleNode("MissingParamTag");
            if (navigator != null)
            {
                tempText = navigator.GetAttribute("message", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _missingParamTagMessage = tempText;
                }
            }
            navigator = containerNavigator.SelectSingleNode("MissingIncludeTarget");
            if (navigator != null)
            {
                tempText = navigator.GetAttribute("message", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _missingIncludeTargetMessage = tempText;
                }
            }
            navigator = containerNavigator.SelectSingleNode("MissingExceptionText");
            if (navigator != null)
            {
                tempText = navigator.GetAttribute("message", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _missingExceptionTextMessage = tempText;
                }
            }

            if (_rootTags == false && _includeTargetTags == false && _namespaceTags == false && 
                _parameterTags == false && _remarkTags == false && _returnTags == false && 
                _summaryTags == false && _typeParameterTags == false && _valueTags == false && 
                _exceptionTags == false)
            {
                _applyMissingTags = false;
            }

            if (_applyMissingTags && _missingLog && !String.IsNullOrEmpty(_missingLogFile))
            {
                _textWriter = new StreamWriter(_missingLogFile, false, Encoding.UTF8);
                if (_missingLogInXml)
                {
                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Encoding    = Encoding.UTF8;
                    settings.Indent      = true;
                    settings.CloseOutput = false;

                    _xmlWriter = XmlWriter.Create(_textWriter, settings);
                    _xmlWriter.WriteStartDocument();
                    _xmlWriter.WriteStartElement("missingTags");
                }                    
            }
        }

        #endregion

        #region ParseAutoDocument Method

        private void ParseAutoDocument(XPathNavigator configuration)
        {   
            _applyAutoDocument         = false;

            _documentWarning           = false;
            _documentConstructors      = false;
            _documentDisposeMethods    = false;

            _autoDocContructor         = "Initializes a new instance of the <see cref=\"T:{0}\"/> class.";
            _autoDocStaticConstructor  = "Initializes the static fields of the <see cref=\"T:{0}\"/> class.";

            _autoDocDisposeMethod      = "Releases all resources used by the <see cref=\"T:{0}\"/> object. ";
            _autoDocParamDisposeMethod = "Releases the unmanaged resources used by the <see cref=\"T:{0}\"/> object and optionally releases the managed resources.";
            _autoDocDisposeMethodParam = "This is <see langword=\"true\"/> to release both managed and unmanaged resources; <see langword=\"false\"/> to release only unmanaged resources.";

            XPathNavigator containerNavigator = configuration.SelectSingleNode("autoDocument");
            if (containerNavigator == null)
            {
                return;
            }

            string tempText = String.Empty;
            tempText = containerNavigator.GetAttribute("enabled", String.Empty);
            if (!String.IsNullOrEmpty(tempText))
            {
                _applyAutoDocument = Convert.ToBoolean(tempText);
                if (!_applyAutoDocument)
                {
                    return;
                }
            }
            BuildLocalizedContents localizedContents = BuildLocalizedContents.Instance;
            if (localizedContents != null && localizedContents.IsInitialized)
            {
                tempText = localizedContents["AutoDocContructor"];
                if (!String.IsNullOrEmpty(tempText) && tempText.Contains("T:{0}"))
                {
                    _autoDocContructor = tempText;
                }
                tempText = localizedContents["AutoDocStaticConstructor"];
                if (!String.IsNullOrEmpty(tempText) && tempText.Contains("T:{0}"))
                {
                    _autoDocStaticConstructor = tempText;
                }

                tempText = localizedContents["AutoDocDisposeMethod"];
                if (!String.IsNullOrEmpty(tempText) && tempText.Contains("T:{0}"))
                {
                    _autoDocDisposeMethod = tempText;
                }
                tempText = localizedContents["AutoDocParamDisposeMethod"];
                if (!String.IsNullOrEmpty(tempText) && tempText.Contains("T:{0}"))
                {
                    _autoDocParamDisposeMethod = tempText;
                }
                tempText = localizedContents["AutoDocDisposeMethodParam"];
                if (!String.IsNullOrEmpty(tempText))
                {
                    _autoDocDisposeMethodParam = tempText;
                }  
            }

            tempText = containerNavigator.GetAttribute("warn", String.Empty);
            if (!String.IsNullOrEmpty(tempText))
            {
                _documentWarning = Convert.ToBoolean(tempText);
            }
            if (containerNavigator.IsEmptyElement)
            {
                tempText = containerNavigator.GetAttribute("constructors", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _documentConstructors = Convert.ToBoolean(tempText);
                }
                tempText = containerNavigator.GetAttribute("disposeMethods", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _documentDisposeMethods = Convert.ToBoolean(tempText);
                }

                _applyAutoDocument = _documentConstructors || _documentDisposeMethods;

                return;
            }

            // For the constructors: static and non-static...
            XPathNavigator navigator = containerNavigator.SelectSingleNode("constructors");
            if (navigator != null)
            {
                tempText = navigator.GetAttribute("enabled", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _documentConstructors = Convert.ToBoolean(tempText);
                }

                if (_documentConstructors && !navigator.IsEmptyElement)
                {
                    // For the non-static constructor summary
                    XPathNavigator commentNavigator = navigator.SelectSingleNode("normalSummary");
                    if (commentNavigator != null)
                    {
                        tempText = navigator.GetAttribute("comment", String.Empty);
                        if (!String.IsNullOrEmpty(tempText) && tempText.Contains("T:{0}"))
                        {
                            _autoDocContructor = tempText;
                        }
                    }

                    // For the static constructor summary
                    commentNavigator = navigator.SelectSingleNode("staticSummary");
                    if (commentNavigator != null)
                    {
                        tempText = navigator.GetAttribute("comment", String.Empty);
                        if (!String.IsNullOrEmpty(tempText) && tempText.Contains("T:{0}"))
                        {
                            _autoDocStaticConstructor = tempText;
                        }                
                    }
                }
            }

            // For the dispose methods: with and without parameters...
            navigator = containerNavigator.SelectSingleNode("disposeMethods");
            if (navigator != null)
            {
                tempText = navigator.GetAttribute("enabled", String.Empty);
                if (!String.IsNullOrEmpty(tempText))
                {
                    _documentDisposeMethods   = Convert.ToBoolean(tempText);
                }

                if (_documentDisposeMethods && !navigator.IsEmptyElement)
                {
                    XPathNavigator commentNavigator = navigator.SelectSingleNode("withoutParamSummary");
                    if (commentNavigator != null)
                    {
                        tempText = navigator.GetAttribute("comment", String.Empty);
                        if (!String.IsNullOrEmpty(tempText) && tempText.Contains("T:{0}"))
                        {
                            _autoDocDisposeMethod = tempText;
                        }
                    }

                    commentNavigator = navigator.SelectSingleNode("withParamSummary");
                    if (commentNavigator != null)
                    {
                        tempText = navigator.GetAttribute("comment", String.Empty);
                        if (!String.IsNullOrEmpty(tempText) && tempText.Contains("T:{0}"))
                        {
                            _autoDocParamDisposeMethod = tempText;
                        }
                    }

                    commentNavigator = navigator.SelectSingleNode("boolParam");
                    if (commentNavigator != null)
                    {
                        tempText = navigator.GetAttribute("comment", String.Empty);
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            _autoDocDisposeMethodParam = tempText;
                        }
                    }
                }
            }

            _applyAutoDocument = _documentConstructors || _documentDisposeMethods;
        }

        #endregion

        #region Version Information Methods

        private void ParseVersionInfo(XPathNavigator configuration)
        {
            string reflectionFile = String.Empty;

            if (String.IsNullOrEmpty(reflectionFile) || 
                File.Exists(reflectionFile) == false)
            {
                return;
            }

            BuildComponentController controller = BuildComponentController.Controller;
            if (controller == null)
            {
                return;
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel = ConformanceLevel.Document;
            //settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;
            using (XmlReader reader = XmlReader.Create(reflectionFile, settings))
            {
                reader.MoveToContent();
                if (String.Equals(reader.Name, "versions"))
                {
                    XmlNodeType nodeType = XmlNodeType.None;
                    string nodeName = String.Empty;
                    while (reader.Read())
                    {
                        nodeType = reader.NodeType;
                        if (nodeType == XmlNodeType.Element &&
                            String.Equals(reader.Name, "version"))
                        {
                            string asmName     = reader.GetAttribute("assemblyName");
                            string asmVersion  = reader.GetAttribute("assemblyVersion");
                            string fileVersion = reader.GetAttribute("fileVersion");
                            if (!String.IsNullOrEmpty(asmName))
                            {
                                controller.AddVersion(new VersionInfo(asmName,
                                    asmVersion, fileVersion));
                            }
                        }
                        else if (nodeType == XmlNodeType.EndElement)
                        {
                            if (String.Equals(reader.Name, "versions"))
                            {
                                break;
                            }
                        }
                    }
                }
                else if (String.Equals(reader.Name, "reflection"))
                {
                    ParseAssemblies(reader, controller);
                }
            }
        }

        private void ParseAssemblies(XmlReader reader, BuildComponentController controller)
        {
            XmlNodeType nodeType = XmlNodeType.None;
            string nodeName = String.Empty;
            while (reader.Read())
            {
                nodeType = reader.NodeType;
                if (nodeType == XmlNodeType.Element)
                {
                    nodeName = reader.Name;
                    if (String.Equals(nodeName, "assembly"))
                    {
                        string asmName = reader.GetAttribute("name");
                        string asmVersion = String.Empty;
                        while (reader.Read())
                        {
                            nodeType = reader.NodeType;
                            if (nodeType == XmlNodeType.Element)
                            {
                                nodeName = reader.Name;
                                if (String.Equals(nodeName, "assemblydata"))
                                {
                                    asmVersion = reader.GetAttribute("version");
                                    reader.Skip();
                                }
                                else if (String.Equals(nodeName, "attributes"))
                                {
                                    string fileVersion = String.Empty;
                                    while (reader.Read())
                                    {
                                        nodeType = reader.NodeType;
                                        if (nodeType == XmlNodeType.Element &&
                                            String.Equals(reader.Name, "attribute"))
                                        {
                                            if (reader.ReadToDescendant("type") && String.Equals(
                                                reader.GetAttribute("api"), "T:System.Reflection.AssemblyFileVersionAttribute"))
                                            {
                                                if (reader.ReadToNextSibling("argument") &&
                                                    reader.ReadToDescendant("value"))
                                                {
                                                    fileVersion = reader.ReadString();
                                                }
                                                break;
                                            }
                                            else
                                            {
                                                reader.Skip();
                                            }
                                        }
                                        else if (nodeType == XmlNodeType.EndElement)
                                        {
                                            if (String.Equals(reader.Name, "attributes"))
                                            {
                                                break;
                                            }
                                        }
                                    }

                                    controller.AddVersion(new VersionInfo(asmName,
                                        asmVersion, fileVersion));

                                    break;
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                nodeName = reader.Name;
                                if (String.Equals(nodeName, "assembly") ||
                                    String.Equals(nodeName, "assemblies") ||
                                    String.Equals(nodeName, "apis"))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (String.Equals(nodeName, "apis"))
                    {
                        break;
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    nodeName = reader.Name;
                    if (String.Equals(nodeName, "assemblies") ||
                        String.Equals(nodeName, "apis"))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (_xmlWriter != null)
            {
                _xmlWriter.WriteEndElement();
                _xmlWriter.WriteEndDocument();
                _xmlWriter.Close();
                _xmlWriter = null;
            }
            if (_textWriter != null)
            {
                _textWriter.Close();
                _textWriter = null;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}
