// Copyright (C) Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

using Sandcastle.ReflectionData;

namespace Sandcastle.Components
{
    using Target           = Sandcastle.ReflectionData.Target;
    using Reference        = Sandcastle.ReflectionData.Reference;
    using MemberTarget     = Sandcastle.ReflectionData.MemberTarget;
    using MethodTarget     = Sandcastle.ReflectionData.MethodTarget;
    using TargetCollection = Sandcastle.ReflectionData.TargetCollection;

    public sealed class ReferenceLinkComponent : LinkComponent
    {
        #region Private Fields

        private static XPathExpression referenceLinkExpression = XPathExpression.Compile("//referenceLink");

        private string                    _linkTarget;

        // WebDocs target url formatting
        private string                    _hrefFormat; 
        private XPathExpression           _baseUrl;

        private XmlWriterSettings         _writerSettings;

        // target information storage
        private TargetCollection          _targets;  
        // msdn resolver
        private TargetMsdnController      _msdnResolver;
        private ReferenceLinkTextResolver _linkResolver; 

        #endregion

        #region Constructors and Destructor

        public ReferenceLinkComponent(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
        {
            _linkTarget = "_blank";
            _hrefFormat = "{0}.htm";

            TargetController controller = TargetController.Controller;
            Debug.Assert(controller != null);
            controller.Initialize(this);

            ReferenceLinkType localLink = ReferenceLinkType.None;
            ReferenceLinkType msdnLink = ReferenceLinkType.None;

            _targets = controller.GetCollection(configuration, out localLink, out msdnLink);
            Debug.Assert(_targets != null);
            _linkResolver = new ReferenceLinkTextResolver(_targets);

            if (msdnLink == ReferenceLinkType.Msdn && _msdnResolver == null)
            {
                WriteMessage(MessageLevel.Info, "Creating MSDN URL resolver.");
                _msdnResolver = TargetMsdnController.Controller;
            }

            XPathNavigator optionsNode = 
                configuration.SelectSingleNode("options");
            if (optionsNode == null)
            {
                optionsNode = configuration;
            }

            // base-url is an xpath expression applied against the current document to pick up the save location of the
            // document. If specified, local links will be made relative to the base-url.
            string baseUrlValue = optionsNode.GetAttribute("base-url", String.Empty);
            if (!String.IsNullOrEmpty(baseUrlValue))
                _baseUrl = XPathExpression.Compile(baseUrlValue);

            // url-format is a string format that is used to format the value of local href attributes. The default is
            // "{0}.htm" for backwards compatibility.
            string hrefFormatValue = optionsNode.GetAttribute("href-format", String.Empty);
            if (!String.IsNullOrEmpty(hrefFormatValue))
                _hrefFormat = hrefFormatValue;

            // the container XPath can be replaced; this is useful
            string containerValue = optionsNode.GetAttribute("container", String.Empty);
            if (!String.IsNullOrEmpty(containerValue))
                TargetCollectionXmlUtilities.ContainerExpression = containerValue;

            string localeValue = optionsNode.GetAttribute("locale", String.Empty);
            if (!String.IsNullOrEmpty(localeValue) && _msdnResolver != null)
                _msdnResolver.Locale = localeValue;

            string targetValue = optionsNode.GetAttribute("linkTarget", String.Empty);
            if (!String.IsNullOrEmpty(targetValue))
                _linkTarget = targetValue;

            this.WriteMessage(MessageLevel.Info, String.Format(
                "Loaded {0} reference targets.", _targets.Count));

            _writerSettings = new XmlWriterSettings();
            _writerSettings.ConformanceLevel = ConformanceLevel.Fragment;
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
            XPathNodeIterator linkIterator = document.CreateNavigator().Select(referenceLinkExpression);
            if (linkIterator == null || linkIterator.Count == 0)
            {
                return;
            }

            XPathNavigator[] linkNodes = BuildComponentUtilities.ConvertNodeIteratorToArray(linkIterator);

            foreach (XPathNavigator linkNode in linkNodes)
            {
                // extract link information
                ReferenceLinkInfo link = ReferenceLinkInfo.Create(linkNode);

                if (link == null)
                {
                    WriteMessage(MessageLevel.Warn, "Invalid referenceLink element.");
                }
                else
                {    
                    // determine target, link type, and display options
                    string targetId = link.Target;
                    ReferenceLinkDisplayOptions options = link.DisplayOptions;
                    ReferenceLinkType type = ReferenceLinkType.None;

                    Target target = _targets[targetId];
                    if (target == null)
                    {
                        // no such target known; set link type to none and warn
                        type = ReferenceLinkType.None;
                        WriteMessage(MessageLevel.Warn, String.Format(
                            "Unknown reference link target '{0}'.", targetId));
                    }
                    else
                    {
                        // if overload is preferred and found, change targetId and make link options hide parameters
                        if (link.PreferOverload)
                        {
                            bool isConversionOperator = false;

                            TargetType targetType = target.TargetType;

                            MemberTarget member = null;
                            if (targetType == TargetType.Method)
                            {
                                MethodTarget method = (MethodTarget)target;
                                isConversionOperator = method.ConversionOperator;
                                member = method;  // a method is a member...
                            }
                            else if (targetType == TargetType.Member || targetType == TargetType.Constructor ||
                                targetType == TargetType.Procedure || targetType == TargetType.Event ||
                                targetType == TargetType.Property)
                            {
                                member = (MemberTarget)target;
                            }

                            // if conversion operator is found, always link to individual topic.
                            if ((member != null) && (!String.IsNullOrEmpty(member.OverloadId)) && !isConversionOperator)
                            {
                                Target overloadTarget = _targets[member.OverloadId];
                                if (overloadTarget != null)
                                {
                                    target = overloadTarget;
                                    targetId = overloadTarget.Id;
                                }
                            }

                            // if individual conversion operator is found, always display parameters.
                            if (isConversionOperator && member != null &&
                                (!string.IsNullOrEmpty(member.OverloadId)))
                            {
                                options = options | ReferenceLinkDisplayOptions.ShowParameters;
                            }
                            else
                            {
                                options = options & ~ReferenceLinkDisplayOptions.ShowParameters;
                            }
                        }

                        // get stored link type
                        type = target.DefaultLinkType;

                        // if link type is local or index, determine which
                        if (type == ReferenceLinkType.LocalOrIndex)
                        {
                            if ((key != null) && _targets.Contains(key) &&
                                (target.Container == _targets[key].Container))
                            {
                                type = ReferenceLinkType.Local;
                            }
                            else
                            {
                                type = ReferenceLinkType.Index;
                            }
                        }
                    }

                    // links to this page are not live
                    if (targetId == key)
                    {
                        type = ReferenceLinkType.Self;
                    }
                    else if ((target != null) && (key != null) && _targets.Contains(key) &&
                        (target.File == _targets[key].File))
                    {
                        type = ReferenceLinkType.Self;
                    }

                    // get msdn endpoint, if needed
                    string msdnUrl = null;
                    if (type == ReferenceLinkType.Msdn)
                    {
                        if ((_msdnResolver == null) || (_msdnResolver.IsDisabled))
                        {
                            // no msdn resolver
                        }
                        else
                        {
                            msdnUrl = _msdnResolver[targetId];
                            if (String.IsNullOrEmpty(msdnUrl))
                            {
                                WriteMessage(MessageLevel.Warn, String.Format(
                                    "MSDN URL not found for target '{0}'.", targetId));
                            }
                        }

                        if (String.IsNullOrEmpty(msdnUrl))
                            type = ReferenceLinkType.None;
                    }

                    // write opening link tag and target info
                    XmlWriter writer = linkNode.InsertAfter();
                    switch (type)
                    {
                        case ReferenceLinkType.None:
                            writer.WriteStartElement("span");
                            writer.WriteAttributeString("class", "nolink");
                            break;
                        case ReferenceLinkType.Self:
                            writer.WriteStartElement("span");
                            writer.WriteAttributeString("class", "selflink");
                            break;
                        case ReferenceLinkType.Local:
                            // format link with prefix and/or postfix
                            string href = String.Format(_hrefFormat, target.File);

                            // make link relative, if we have a baseUrl
                            if (_baseUrl != null)
                                href = BuildComponentUtilities.GetRelativePath(href,
                                    BuildComponentUtilities.EvalXPathExpr(document, _baseUrl, "key", key));

                            writer.WriteStartElement("a");
                            writer.WriteAttributeString("href", href);
                            break;
                        case ReferenceLinkType.Index:
                            writer.WriteStartElement("mshelp", "link", "http://msdn.microsoft.com/mshelp");
                            writer.WriteAttributeString("keywords", targetId);
                            writer.WriteAttributeString("tabindex", "0");
                            break;
                        case ReferenceLinkType.Msdn:
                            writer.WriteStartElement("a");
                            writer.WriteAttributeString("href", msdnUrl);
                            writer.WriteAttributeString("target", _linkTarget);
                            break;
                        case ReferenceLinkType.Id:
                            string xhelp = String.Format("ms-xhelp://?Id={0}", targetId);
                            xhelp = xhelp.Replace("#", "%23");
                            writer.WriteStartElement("a");
                            writer.WriteAttributeString("href", xhelp);
                            break;
                    }

                    // write the link text
                    if (String.IsNullOrEmpty(link.DisplayTarget))
                    {
                        if (link.Contents == null)
                        {
                            if (target != null)
                            {
                                _linkResolver.WriteTarget(target, options, writer);
                            }
                            else
                            {
                                Reference reference = ReferenceTextUtilities.CreateReference(targetId);

                                if (reference.ReferenceType == ReferenceType.Invalid)
                                    WriteMessage(MessageLevel.Warn, String.Format(
                                        "Invalid reference link target '{0}'.", targetId));

                                _linkResolver.WriteReference(reference, options, writer);
                            }
                        }
                        else
                        {
                            // write contents to writer
                            link.Contents.WriteSubtree(writer);
                        }
                    }
                    else
                    {
                        if ((String.Compare(link.DisplayTarget, "content", true) == 0) && (link.Contents != null))
                        {
                            // Use the contents as an XML representation of the display target
                            Reference reference = TargetCollectionXmlUtilities.CreateReference(link.Contents);

                            _linkResolver.WriteReference(reference, options, writer);
                        }
                        if ((String.Compare(link.DisplayTarget, "format", true) == 0) && (link.Contents != null))
                        {
                            // Use the contents as a format string for the display target
                            string format = link.Contents.OuterXml;

                            string input = null;
                            StringWriter textStore = new StringWriter();
                            try
                            {
                                XmlWriter xmlStore = XmlWriter.Create(textStore, _writerSettings);
                                try
                                {
                                    if (target != null)
                                    {
                                        _linkResolver.WriteTarget(target, options, xmlStore);
                                    }
                                    else
                                    {
                                        Reference reference = ReferenceTextUtilities.CreateReference(targetId);
                                        _linkResolver.WriteReference(reference, options, xmlStore);
                                    }
                                }
                                finally
                                {
                                    xmlStore.Close();
                                }
                                input = textStore.ToString();
                            }
                            finally
                            {
                                textStore.Close();
                            }

                            string output = String.Format(format, input);

                            XmlDocumentFragment fragment = document.CreateDocumentFragment();
                            fragment.InnerXml = output;
                            fragment.WriteTo(writer);

                            //writer.WriteRaw(output);
                        }
                        else if ((String.Compare(link.DisplayTarget, "extension", true) == 0) && (link.Contents != null))
                        {
                            Reference extMethodReference = TargetCollectionXmlUtilities.CreateExtensionMethodReference(link.Contents);
                            _linkResolver.WriteReference(extMethodReference, options, writer);
                        }
                        else
                        {
                            // Use the display target value as a CER for the display target

                            ReferenceTextUtilities.SetGenericContext(key);
                            Reference reference = ReferenceTextUtilities.CreateReference(link.DisplayTarget);

                            _linkResolver.WriteReference(reference, options, writer);
                        }
                    }

                    // write the closing link tag
                    writer.WriteEndElement();
                    writer.Close();
                }

                // delete the original tag
                linkNode.DeleteSelf();
            }
        }

        #endregion

        #region Protected Methods

        #endregion
    }
}
