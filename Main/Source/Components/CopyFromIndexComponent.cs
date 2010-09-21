// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Text;
using System.Reflection;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.Indexed;

namespace Sandcastle.Components
{
    public sealed class CopyFromIndexComponent : BuildComponentEx
    {
        #region Private Fields

        // a context in which to evaluate XPath expressions
        private CustomContext              _context;

        // List of copy components
        private List<CopyComponent>        _copyComponents;

        // what to copy
        private List<CopyFromIndexCommand> _copyCommands;

        private IndexedDocumentController  _documentController;

        #endregion

        #region Constructors and Destructor

        public CopyFromIndexComponent(BuildAssembler assembler, XPathNavigator configuration)
            : base(assembler, configuration)
        {
            _copyComponents   = new List<CopyComponent>();
            _copyCommands = new List<CopyFromIndexCommand>();
            _context      = new CustomContext();

            // set up the context
            XPathNodeIterator contextNodes = configuration.Select("context");
            foreach (XPathNavigator contextNode in contextNodes)
            {
                string prefix = contextNode.GetAttribute("prefix", String.Empty);
                string name   = contextNode.GetAttribute("name", String.Empty);
                _context.AddNamespace(prefix, name);
            }

            // set up the indices
            _documentController = new IndexedDocumentController(this, 
                configuration, _context, Data);

            // get the copy commands
            XPathNodeIterator copyNodes = configuration.Select("copy");
            foreach (XPathNavigator copyNode in copyNodes)
            {
                string sourceName = copyNode.GetAttribute("name", String.Empty);
                if (String.IsNullOrEmpty(sourceName))
                    throw new BuildComponentException(
                        "Each copy command must specify an index to copy from.");

                string keyXPath = copyNode.GetAttribute("key", String.Empty);

                string sourceXPath = copyNode.GetAttribute("source", String.Empty);
                if (String.IsNullOrEmpty(sourceXPath))
                    throw new BuildComponentException(
                        "When instantiating a CopyFromDirectory component, you must specify a source xpath format using the source attribute.");

                string targetXPath = copyNode.GetAttribute("target", String.Empty);
                if (String.IsNullOrEmpty(targetXPath))
                    throw new BuildComponentException(
                        "When instantiating a CopyFromDirectory component, you must specify a target xpath format using the target attribute.");

                string attributeValue     = copyNode.GetAttribute("attribute", String.Empty);

                string ignoreCaseValue    = copyNode.GetAttribute("ignoreCase", String.Empty);

                string missingEntryValue  = copyNode.GetAttribute("missing-entry", String.Empty);
                string missingSourceValue = copyNode.GetAttribute("missing-source", String.Empty);
                string missingTargetValue = copyNode.GetAttribute("missing-target", String.Empty);

                // Note that sourceController != _documentController
                IndexedDocumentController sourceController =
                    (IndexedDocumentController)Data[sourceName];
                CopyFromIndexCommand copyCommand = new CopyFromIndexCommand(
                    sourceController[sourceName], keyXPath, sourceXPath, 
                    targetXPath, attributeValue, ignoreCaseValue);
                if (!String.IsNullOrEmpty(missingEntryValue))
                {
                    try
                    {
                        copyCommand.MissingEntry = (MessageLevel)Enum.Parse(
                            typeof(MessageLevel), missingEntryValue, true);
                    }
                    catch (ArgumentException)
                    {
                        WriteMessage(MessageLevel.Error, String.Format("'{0}' is not a message level.", missingEntryValue));
                    }
                }
                if (!String.IsNullOrEmpty(missingSourceValue))
                {
                    try
                    {
                        copyCommand.MissingSource = (MessageLevel)Enum.Parse(
                            typeof(MessageLevel), missingSourceValue, true);
                    }
                    catch (ArgumentException)
                    {
                        WriteMessage(MessageLevel.Error, String.Format(
                            "'{0}' is not a message level.", missingSourceValue));
                    }
                }
                if (!String.IsNullOrEmpty(missingTargetValue))
                {
                    try
                    {
                        copyCommand.MissingTarget = (MessageLevel)Enum.Parse(
                            typeof(MessageLevel), missingTargetValue, true);
                    }
                    catch (ArgumentException)
                    {
                        WriteMessage(MessageLevel.Error, String.Format(
                            "'{0}' is not a message level.", missingTargetValue));
                    }
                }

                _copyCommands.Add(copyCommand);
            }

            XPathNodeIterator componentNodes = configuration.Select("components/component");
            foreach (XPathNavigator componentNode in componentNodes)
            {                      
                // get the data to load the component
                string assemblyPath = componentNode.GetAttribute(
                    "assembly", String.Empty);
                if (String.IsNullOrEmpty(assemblyPath)) 
                    WriteMessage(MessageLevel.Error, "Each component element must have an assembly attribute.");
                string typeName = componentNode.GetAttribute("type", 
                    String.Empty);
                if (String.IsNullOrEmpty(typeName)) 
                    WriteMessage(MessageLevel.Error, "Each component element must have a type attribute.");

                // expand environment variables in the path
                assemblyPath = Environment.ExpandEnvironmentVariables(assemblyPath);

                try
                {
                    Assembly assembly = Assembly.LoadFrom(assemblyPath);
                    CopyComponent component = (CopyComponent)assembly.CreateInstance(typeName, false, BindingFlags.Public | BindingFlags.Instance, null, new Object[2] { componentNode.Clone(), Data }, null, null);

                    if (component == null)
                    {
                        WriteMessage(MessageLevel.Error, String.Format(
                            "The type '{0}' does not exist in the assembly '{1}'.", typeName, assemblyPath));
                    }
                    else
                    {
                        _copyComponents.Add(component);
                    }

                }
                catch (IOException e)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "A file access error occurred while attempting to load the build component '{0}'. The error message is: {1}", assemblyPath, e.Message));
                }
                catch (BadImageFormatException e)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "A syntax generator assembly '{0}' is invalid. The error message is: {1}.", assemblyPath, e.Message));
                }
                catch (TypeLoadException e)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The type '{0}' does not exist in the assembly '{1}'. The error message is: {2}", typeName, assemblyPath, e.Message));
                }
                catch (MissingMethodException e)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The type '{0}' in the assembly '{1}' does not have an appropriate constructor. The error message is: {2}", typeName, assemblyPath, e.Message));
                }
                catch (TargetInvocationException e)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "An error occurred while attempting to instantiate the type '{0}' in the assembly '{1}'. The error message is: {2}", typeName, assemblyPath, e.InnerException.Message));
                }
                catch (InvalidCastException)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The type '{0}' in the assembly '{1}' is not a SyntaxGenerator.", typeName, assemblyPath));
                }
            }

            WriteMessage(MessageLevel.Info, String.Format(
                "Loaded {0} copy components.", _copyComponents.Count));
        }

        #endregion

        #region Public Properties

        public IndexedDocumentController Controller
        {
            get
            {
                return _documentController;
            }
        }

        public IList<CopyFromIndexCommand> CopyCommands
        {
            get
            {
                return _copyCommands;
            }
        }

        #endregion

        #region Public Methods

        // the actual work of the component

        public override void Apply(XmlDocument document, string key)
        {
            // set the key in the XPath context
            _context["key"] = key;

            // perform each copy action
            foreach (CopyFromIndexCommand copyCommand in _copyCommands)
            {
                // get the source comment
                XPathExpression keyExpression = copyCommand.Key.Clone();
                keyExpression.SetContext(_context);
                string keyValue = (string)document.CreateNavigator().Evaluate(keyExpression);

                XPathNavigator data = copyCommand.Index.GetContent(keyValue);

                if (data == null && copyCommand.IgnoreCase == "true")
                    data = copyCommand.Index.GetContent(keyValue.ToLower());

                // notify if no entry
                if (data == null)
                {
                    WriteMessage(copyCommand.MissingEntry, String.Format(
                        "No index entry found for key '{0}'.", keyValue));
                    continue;
                }

                // get the target node
                String targetXPath = copyCommand.Target.Clone().ToString();
                XPathExpression target_expression = XPathExpression.Compile(
                    String.Format(targetXPath, keyValue));
                target_expression.SetContext(_context);

                XPathNavigator target = document.CreateNavigator().SelectSingleNode(target_expression);

                // notify if no target found
                if (target == null)
                {
                    WriteMessage(copyCommand.MissingTarget, String.Format("Target node '{0}' not found.", target_expression.Expression));
                    continue;
                }

                // get the source nodes
                XPathExpression sourceExpression = copyCommand.Source.Clone();
                sourceExpression.SetContext(_context);
                XPathNodeIterator sources = data.CreateNavigator().Select(sourceExpression);

                // append the source nodes to the target node
                int sourceCount = 0;
                foreach (XPathNavigator source in sources)
                {
                    sourceCount++;

                    // If attribute=true, add the source attributes to current target. 
                    // Otherwise append source as a child element to target
                    if (copyCommand.Attribute == "true" && source.HasAttributes)
                    {
                        string source_name = source.LocalName;
                        XmlWriter attributes = target.CreateAttributes();

                        source.MoveToFirstAttribute();
                        string attrFirst = target.GetAttribute(string.Format("{0}_{1}", source_name, source.Name), string.Empty);
                        if (string.IsNullOrEmpty(attrFirst)) attributes.WriteAttributeString(string.Format("{0}_{1}", source_name, source.Name), source.Value);

                        while (source.MoveToNextAttribute())
                        {
                            string attrNext = target.GetAttribute(string.Format("{0}_{1}", source_name, source.Name), string.Empty);
                            if (string.IsNullOrEmpty(attrNext)) attributes.WriteAttributeString(string.Format("{0}_{1}", source_name, source.Name), source.Value);
                        }
                        attributes.Close();
                    }
                    else target.AppendChild(source);
                }

                // notify if no source found
                if (sourceCount == 0)
                {
                    WriteMessage(copyCommand.MissingSource, String.Format("Source node '{0}' not found.", sourceExpression.Expression));
                }

                foreach (CopyComponent component in _copyComponents)
                {
                    component.Apply(document, key);
                }
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (_documentController != null)
            {
                _documentController.Dispose();
                _documentController = null;
            }
        }

        #endregion
    }
}
