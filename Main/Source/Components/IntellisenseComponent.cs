// ------------------------------------------------------------------------------------------------
// <copyright file="IntellisenseComponent2.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation.
//      This source file is subject to the Microsoft Permissive License.
//      See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
//      All other rights reserved.
// </copyright>
// <summary>
// Contains code that extract XML comments files during the build process, in which case the 
// <inheritdoc/> tags are expanded and evaluated.
// </summary>
// <update>
// This is an extended version, which fixes the component since it is currently not usable.
// </update>
// ------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    public sealed class IntellisenseComponent : BuildComponentEx
    {
        #region Private Fields

        private string          _outputDirectory;

        private XPathExpression _rootExpression;
        private XPathExpression _enumExpression;
        private XPathExpression _enumApiExpression;
        private XPathExpression _memberSummaryExpression;

        private XPathExpression _assemblyExpression;

        private XPathExpression _summaryExpression;
        private XPathExpression _paramsExpression;
        private XPathExpression _paramContentExpression;
        private XPathExpression _templatesExpression;
        private XPathExpression _templateContentExpression;
        private XPathExpression _returnsExpression;
        private XPathExpression _exceptionExpression;
        private XPathExpression _exceptionCrefExpression;

        private Dictionary<string, XmlWriter>      _outputWriters; 
        private Dictionary<string, XPathNavigator> _userInputContents;

        #endregion

        #region Constructors and Destructor

        public IntellisenseComponent(BuildAssembler assembler, XPathNavigator configuration)
            : base(assembler, configuration)
        {
            _outputDirectory   = String.Empty;
            _outputWriters     = new Dictionary<string, XmlWriter>();

            XPathNavigator outputNode = configuration.SelectSingleNode("output");
            if (outputNode != null)
            {     
                string directoryValue = outputNode.GetAttribute("directory", String.Empty);
                if (!String.IsNullOrEmpty(directoryValue))
                {
                    _outputDirectory = Path.GetFullPath(
                        Environment.ExpandEnvironmentVariables(directoryValue));
                    if (!Directory.Exists(_outputDirectory))
                    {
                        WriteMessage(MessageLevel.Error, String.Format(
                            "The output directory '{0}' does not exist.", _outputDirectory));
                    }
                }
            }

            XPathNavigator expressionNode = configuration.SelectSingleNode("expressions");
            if (expressionNode != null)
            {  
                string root = expressionNode.GetAttribute("root", String.Empty);
                try
                {
                    _rootExpression = XPathExpression.Compile(root);
                }
                catch (XPathException)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The expression '{0}' is not a valid XPath expression.", root));
                }

                string assembly = expressionNode.GetAttribute("assembly", String.Empty);
                try
                {
                    _assemblyExpression = XPathExpression.Compile(assembly);
                }
                catch (XPathException)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The expression '{0}' is not a valid XPath expression.", assembly));
                }

                string summary = expressionNode.GetAttribute("summary", String.Empty);
                try
                {
                    _summaryExpression = XPathExpression.Compile(summary);
                }
                catch (XPathException)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The expression '{0}' is not a valid XPath expression.", summary));
                }

                string parameters = expressionNode.GetAttribute("parameters", String.Empty);
                try
                {
                    _paramsExpression = XPathExpression.Compile(parameters);
                }
                catch (XPathException)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The expression '{0}' is not a valid XPath expression.", parameters));
                }

                string parameterContent = expressionNode.GetAttribute("parameterContent", String.Empty);
                try
                {
                    _paramContentExpression = XPathExpression.Compile(parameterContent);
                }
                catch (XPathException)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The expression '{0}' is not a valid XPath expression.", parameterContent));
                }

                string templates = expressionNode.GetAttribute("templates", String.Empty);
                try
                {
                    _templatesExpression = XPathExpression.Compile(templates);
                }
                catch (XPathException)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The expression '{0}' is not a valid XPath expression.", templates));
                }

                string templateContent = expressionNode.GetAttribute("templateContent", String.Empty);
                try
                {
                    _templateContentExpression = XPathExpression.Compile(templateContent);
                }
                catch (XPathException)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The expression '{0}' is not a valid XPath expression.", templateContent));
                }

                string returns = expressionNode.GetAttribute("returns", String.Empty);
                try
                {
                    _returnsExpression = XPathExpression.Compile(returns);
                }
                catch (XPathException)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The expression '{0}' is not a valid XPath expression.", returns));
                }

                string exception = expressionNode.GetAttribute("exception", String.Empty);
                try
                {
                    _exceptionExpression = XPathExpression.Compile(exception);
                }
                catch (XPathException)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The expression '{0}' is not a valid XPath expression.", exception));
                }

                string exceptionCref = expressionNode.GetAttribute("exceptionCref", String.Empty);
                try
                {
                    _exceptionCrefExpression = XPathExpression.Compile(exceptionCref);
                }
                catch (XPathException)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The expression '{0}' is not a valid XPath expression.", exceptionCref));
                }

                string enumeration = expressionNode.GetAttribute("enumeration", String.Empty);
                try
                {
                    _enumExpression = XPathExpression.Compile(enumeration);
                }
                catch (XPathException)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The expression '{0}' is not a valid XPath expression.", enumeration));
                }

                string enumerationApi = expressionNode.GetAttribute("enumerationApi", String.Empty);
                try
                {
                    _enumApiExpression = XPathExpression.Compile(enumerationApi);
                }
                catch (XPathException)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The expression '{0}' is not a valid XPath expression.", enumerationApi));
                }

                string memberSummary = expressionNode.GetAttribute("memberSummary", String.Empty);
                try
                {
                    _memberSummaryExpression = XPathExpression.Compile(memberSummary);
                }
                catch (XPathException)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The expression '{0}' is not a valid XPath expression.", memberSummary));
                }  
            }
            else
            {
                // Provide the default expressions...
                _rootExpression             = XPathExpression.Compile("/*");
                _enumExpression             = XPathExpression.Compile("reference/elements/element");
                _enumApiExpression          = null;
                _memberSummaryExpression    = XPathExpression.Compile("summary");

                _assemblyExpression         = XPathExpression.Compile("string(//containers/library/@assembly)");

                _summaryExpression          = XPathExpression.Compile("comments/summary");
                _paramsExpression           = XPathExpression.Compile("comments/param");
                _paramContentExpression     = null;
                _templatesExpression        = XPathExpression.Compile("comments/typeparam");
                _templateContentExpression  = null;
                // NOTE: The "value" is not really needed by the IntelliSense, but we add it for completeness.
                _returnsExpression          = XPathExpression.Compile("comments/returns | comments/value");
                _exceptionExpression        = XPathExpression.Compile("comments/exception");
                _exceptionCrefExpression    = null;
            }

            // a way to get additional information into the IntelliSense file
            XPathNodeIterator inputNodes = configuration.Select("input");
            foreach (XPathNavigator inputNode in inputNodes)
            {
                string fileValue = inputNode.GetAttribute("file", String.Empty);
                if (!String.IsNullOrEmpty(fileValue))
                {
                    string file = Path.GetFullPath(Environment.ExpandEnvironmentVariables(fileValue));
                    ReadInputFile(file);
                }
            }
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string id)
        {     
            // only generate IntelliSense if id corresponds to an allowed IntelliSense ID
            if (id.Length < 2 || id[1] != ':') 
                return;

            if (!((id[0] == 'T') || (id[0] == 'M') || (id[0] == 'P') ||
                (id[0] == 'F') || (id[0] == 'E') || (id[0] == 'N')))
            {
                return;
            }

            if ((id[0] == 'N') && id.EndsWith("TocExcluded",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            XPathNavigator root = null;
            if (_rootExpression != null)
            {
                root = document.CreateNavigator().SelectSingleNode(_rootExpression);
            }
            else
            {
                root = document.CreateNavigator();
            }

            // get the assembly information
            string assembly = (string)root.Evaluate(_assemblyExpression);

            if (String.IsNullOrEmpty(assembly))
            {
                assembly = "namespaces";
            }

            // try/catch block for capturing errors
            try
            {  
                // get the writer for the assembly
                XmlWriter writer;
                if (!_outputWriters.TryGetValue(assembly, out writer))
                {   
                    // create a writer for the assembly
                    string name = Path.Combine(_outputDirectory, assembly + ".xml");

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;

                    try
                    {
                        writer = XmlWriter.Create(name, settings);
                    }
                    catch (IOException e)
                    {
                        WriteMessage(MessageLevel.Error, String.Format(
                            "An access error occurred while attempting to create the IntelliSense output file '{0}'. The error message is: {1}", name, e.Message));
                    }

                    _outputWriters.Add(assembly, writer);

                    // write out the initial data
                    writer.WriteStartDocument();
                    writer.WriteStartElement("doc");
                    //do not generate assembly nodes for namespace topics
                    if (assembly != "namespaces")
                    {
                        writer.WriteStartElement("assembly");
                        writer.WriteElementString("name", assembly);
                        writer.WriteEndElement();
                    }
                    writer.WriteStartElement("members");
                }

                writer.WriteStartElement("member");
                writer.WriteAttributeString("name", id);

                // summary
                WriteSummary(root, _summaryExpression, writer);

                // return value
                XPathNavigator returns = root.SelectSingleNode(_returnsExpression);
                if (returns != null)
                {
                    //writer.WriteStartElement("returns");
                    XmlReader reader = returns.ReadSubtree();

                    CopyContent(reader, writer);
                    reader.Close();

                    //writer.WriteEndElement();
                }

                // parameters
                XPathNodeIterator parameters = root.Select(_paramsExpression);
                foreach (XPathNavigator parameter in parameters)
                {
                    string name      = String.Empty;
                    XmlReader reader = null;
                    if (_paramContentExpression != null)
                    {
                        name = (string)parameter.GetAttribute("paramName", String.Empty);

                        XPathNavigator parameterContent = parameter.SelectSingleNode(_paramContentExpression);

                        if (parameterContent == null)
                            continue;

                        reader = parameterContent.ReadSubtree();
                    }
                    else
                    {
                        name   = (string)parameter.GetAttribute("name", String.Empty);
                        reader = parameter.ReadSubtree();
                    }

                    writer.WriteStartElement("param");
                    writer.WriteAttributeString("name", name);
                    CopyContentEx(reader, writer);
                    writer.WriteFullEndElement();

                    reader.Close();
                }

                // templates
                XPathNodeIterator templates = root.Select(_templatesExpression);
                foreach (XPathNavigator template in templates)
                {
                    string name      = String.Empty;
                    XmlReader reader = null;
                    if (_templateContentExpression != null)
                    {
                        name = (string)template.GetAttribute("paramName", String.Empty);

                        XPathNavigator templateContent = template.SelectSingleNode(_templateContentExpression);

                        if (templateContent == null)
                            continue;

                        reader = templateContent.ReadSubtree();
                    }
                    else
                    {
                        name   = (string)template.GetAttribute("name", String.Empty);
                        reader = template.ReadSubtree();
                    }

                    writer.WriteStartElement("typeparam");
                    writer.WriteAttributeString("name", name);
                    CopyContentEx(reader, writer);
                    writer.WriteFullEndElement();

                    reader.Close();
                }

                // exceptions
                XPathNodeIterator exceptions = root.Select(_exceptionExpression);
                foreach (XPathNavigator exception in exceptions)
                {
                    string cref = String.Empty;
                    if (_exceptionCrefExpression != null) //TODO: Do not know if such case exists
                    {
                        XPathNavigator exceptionCref = exception.SelectSingleNode(_exceptionCrefExpression);

                        if (exceptionCref == null)
                            continue;

                        cref = exceptionCref.GetAttribute("target", String.Empty);
                    }
                    else
                    {
                        cref = exception.GetAttribute("cref", String.Empty);
                    }

                    XmlReader reader = exception.ReadSubtree();

                    //writer.WriteStartElement("exception");
                    //writer.WriteAttributeString("cref", cref);
                    CopyContent(reader, writer);
                    //writer.WriteFullEndElement();

                    reader.Close();
                }

                // stored contents
                XPathNavigator input;
                if (_userInputContents != null && _userInputContents.TryGetValue(id, out input))
                {
                    XPathNodeIterator input_nodes = input.SelectChildren(XPathNodeType.Element);
                    foreach (XPathNavigator input_node in input_nodes)
                    {
                        input_node.WriteSubtree(writer);
                    }
                }

                writer.WriteFullEndElement();

                // enumeration members
                XPathNodeIterator enumerationIterator = root.Select(_enumExpression);

                foreach (XPathNavigator enumeration in enumerationIterator)
                {
                    string api = String.Empty;
                    if (_enumApiExpression != null) //TODO: Do not know if such case exists
                    {
                        XPathNavigator enumApi = enumeration.SelectSingleNode(_enumApiExpression);

                        if (enumApi == null)
                            continue;

                        api = enumApi.GetAttribute("target", String.Empty);
                    }
                    else
                    {
                        api = enumeration.GetAttribute("api", String.Empty);
                    }
                    writer.WriteStartElement("member");
                    writer.WriteAttributeString("name", api);

                    //summary
                    WriteSummary(enumeration, _memberSummaryExpression, writer);

                    writer.WriteFullEndElement();
                }
            }
            catch (IOException e)
            {
                WriteMessage(MessageLevel.Error, String.Format(
                    "An access error occurred while attempting to write IntelliSense data. The error message is: {0}", e.Message));
            }
            catch (XmlException e)
            {
                WriteMessage(MessageLevel.Error, String.Format(
                    "IntelliSense data was not valid XML. The error message is: {0}", e.Message));
            }
        }

        #endregion

        #region Private Methods

        // input content store

        private void ReadInputFile(string file)
        {
            try
            {
                if (_userInputContents == null)
                {
                    _userInputContents = new Dictionary<string, XPathNavigator>();
                }

                XPathDocument document = new XPathDocument(file);

                XPathNodeIterator memberNodes = document.CreateNavigator().Select("/metadata/topic[@id]");
                foreach (XPathNavigator memberNode in memberNodes)
                {
                    string id = memberNode.GetAttribute("id", String.Empty);
                    _userInputContents[id] = memberNode.Clone();
                }

                WriteMessage(MessageLevel.Info, String.Format(
                    "Read {0} input content nodes.", memberNodes.Count));

            }
            catch (XmlException e)
            {
                WriteMessage(MessageLevel.Error, String.Format(
                    "The input file '{0}' is not a well-formed XML file. The error message is: {1}", file, e.Message));
            }
            catch (IOException e)
            {
                WriteMessage(MessageLevel.Error, String.Format(
                    "An error occurred while attempting to access the fileThe input file '{0}'. The error message is: {1}", file, e.Message));
            }


        }
        // the action of the component

        private void WriteSummary(XPathNavigator node, XPathExpression expression, XmlWriter writer)
        {    
            XPathNavigator summary = node.SelectSingleNode(expression);
            if (summary != null)
            {
                //writer.WriteStartElement("summary");

                XmlReader reader = summary.ReadSubtree();

                CopyContent(reader, writer);
                reader.Close();

                //writer.WriteEndElement();
            }
            //else
            //{
            //}
        }

        private void CopyContent(XmlReader reader, XmlWriter writer)
        {
            reader.MoveToContent();

            writer.WriteNode(reader, true);
        }

        private void CopyContentEx(XmlReader reader, XmlWriter writer)
        {
            reader.MoveToContent();

            while (true)
            {      
                if (reader.NodeType == XmlNodeType.Text)
                {
                    writer.WriteString(reader.ReadString());
                }
                else if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.LocalName == "span" && (reader.GetAttribute("sdata", String.Empty) == "cer"))
                    {
                        writer.WriteStartElement("see");
                        writer.WriteAttributeString("cref", reader.GetAttribute("target", String.Empty));
                        writer.WriteEndElement();
                        reader.Skip();
                    }
                    else if (reader.LocalName == "span" && 
                        (reader.GetAttribute("sdata", String.Empty) == "paramReference"))
                    {
                        writer.WriteStartElement("paramref");
                        writer.WriteAttributeString("name", reader.ReadElementString());
                        writer.WriteEndElement();
                    }
                    else if (reader.LocalName == "span" && 
                        (reader.GetAttribute("sdata", String.Empty) == "link"))
                    {
                        writer.WriteString(reader.ReadElementString());
                    }
                    else if (reader.LocalName == "span" && 
                        (reader.GetAttribute("sdata", String.Empty) == "langKeyword"))
                    {
                        string keyword = reader.GetAttribute("value", String.Empty);
                        writer.WriteString(keyword);
                        reader.Skip();
                    }
                    else
                    {
                        if (!reader.Read())
                        {
                            break;
                        }
                    }
                }
                else
                {
                    if (!reader.Read())
                    {
                        break;
                    }
                }  
            }
        }

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_outputWriters != null)
                {
                    foreach (XmlWriter writer in _outputWriters.Values)
                    {
                        writer.WriteEndDocument();
                        writer.Close();
                    }

                    _outputWriters = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion
    }             
}