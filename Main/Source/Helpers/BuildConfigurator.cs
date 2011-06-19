// <copyright>
// Portions of this class are based on the sources of Sandcastle shared content 
// component in the SharedContentComponent.cs, BuildComponentUtilities.cs and BuildContext.cs files.
// Copyright (c) Microsoft Corporation.  All rights reserved.
// <copyright>

using System;
using System.IO;
using System.Text;

using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Xml.Schema;
using System.Collections.Generic;

namespace Sandcastle
{
    /// <summary>
    /// This is an <see langword="abstract"/> base class for build tool configuration 
    /// file handlers, which provide editing and/or modifying operations for the
    /// configuration files.
    /// </summary>
    /// <remarks>
    /// The configurators provide an extensible framework for creating and/or modifying
    /// the various Sandcastle configuration files, using XML-based processing methods.
    /// </remarks>
    public abstract class BuildConfigurator : BuildObject, IDisposable
    {
        #region Private Fields

        private bool _isInitialized;
        private bool _warnIfNotFound;
        private bool _deleteIfNotFound;

        private BuildLogger _logger;

        // The shared content items
        private BuildProperties _dicContents;
        // The shared content elements
        private XPathExpression _pathSelector;
        private XPathExpression _itemSelector;
        private XPathExpression _parametersSelector;
        private XPathExpression _attributeSelector;
        private XmlNamespaceManager _namespaceManager;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildConfigurator"/> class.
        /// </summary>
        protected BuildConfigurator()
        {
            _warnIfNotFound     = true;

            _pathSelector       = XPathExpression.Compile("//IncludeItem | //IncludeItemAttribute");
            _itemSelector       = XPathExpression.Compile("string(@item)");
            _parametersSelector = XPathExpression.Compile("parameter");
            _attributeSelector  = XPathExpression.Compile("string(@name)");
        }

        /// <summary>
        /// This allows the <see cref="BuildConfigurator"/> instance to attempt to free 
        /// resources and perform other cleanup operations before the 
        /// <see cref="BuildConfigurator"/> instance is reclaimed by garbage collection.
        /// </summary>
        ~BuildConfigurator()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this build configuration task is 
        /// initialized and ready for the build process.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this build configuration task 
        /// is initialized; otherwise, it is <see langword="false"/>.
        /// </value>
        /// <seealso cref="BuildConfigurator.Initialize(BuildContext)"/>
        /// <seealso cref="BuildConfigurator.Uninitialize()"/>
        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
            protected set
            {
                _isInitialized = value;
            }
        }

        public int ContentCount
        {
            get
            {
                if (_dicContents != null)
                {
                    return _dicContents.Count;
                }

                return 0;
            }
        }

        public BuildLogger Logger
        {
            get
            {
                return _logger;
            }
        }
        
        public bool WarnIfNotFound
        {
            get 
            { 
                return _warnIfNotFound; 
            }
            set 
            { 
                _warnIfNotFound = value; 
            }
        }
        
        public bool DeleteIfNotFound
        {
            get 
            { 
                return _deleteIfNotFound; 
            }
            set 
            { 
                _deleteIfNotFound = value; 
            }
        }

        public virtual bool HasContents
        {
            get
            {
                return (_dicContents != null && _dicContents.Count != 0);
            }
        }

        public virtual string Prefix
        {
            get
            {
                return null;
            }
        }

        public virtual string Namespace
        {
            get
            {
                return null;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Initialize(BuildLogger logger)
        {
            _logger        = logger;

            _isInitialized = true;
        }

        public virtual void Uninitialize()
        {
            _logger        = null;
            _dicContents   = null;

            _isInitialized = false;
        }

        public virtual XmlNamespaceManager GetNamespaceManager(XmlNameTable nameTable)
        {
            if (_namespaceManager == null || 
                _namespaceManager.NameTable != nameTable)
            {
                string configPrefix = this.Prefix;
                string configNamespace = this.Namespace;
                if (!String.IsNullOrEmpty(configPrefix) && 
                    !String.IsNullOrEmpty(configNamespace))
                {
                    _namespaceManager = new XmlNamespaceManager(nameTable);
                    _namespaceManager.AddNamespace(String.Empty, configNamespace);
                    _namespaceManager.AddNamespace(configPrefix, configNamespace);
                }
            }

            return _namespaceManager;
        }

        public void AddContentItem(string itemName, string itemValue)
        {
            BuildExceptions.NotNullNotEmpty(itemName, "itemName");

            if (itemValue == null)
            {
                itemValue = String.Empty;
            }

            if (_dicContents == null)
            {
                _dicContents = new BuildProperties();
            }

            _dicContents[itemName] = itemValue;
        }

        public void ClearContents()
        {
            if (_dicContents != null)
            {
                _dicContents = new BuildProperties();
            }
        }

        public void ApplyContents(XmlDocument document)
        {
            if (this.HasContents == false)
            {
                return;
            }

            XmlNamespaceManager nsManager = 
                this.GetNamespaceManager(document.NameTable);
            if (nsManager != null)
            {
                if (_pathSelector != null)
                {
                    _pathSelector.SetContext(nsManager);
                }
                if (_itemSelector != null)
                {
                    _itemSelector.SetContext(nsManager);
                }
                if (_parametersSelector != null)
                {
                    _parametersSelector.SetContext(nsManager);
                }
                if (_attributeSelector != null)
                {
                    _attributeSelector.SetContext(nsManager);
                }
            }

            ResolveContent(document, document.CreateNavigator());
        }

        public void LoadContents(string wildcardPath)
        {
            string sharedContentFiles = Environment.ExpandEnvironmentVariables(
                wildcardPath);
            if (String.IsNullOrEmpty(sharedContentFiles))
                LogMessage(BuildLoggerLevel.Error,
                    "The content/@file attribute specifies an empty string.");

            LogMessage(BuildLoggerLevel.Info, String.Format(
                "Searching for files that match '{0}'.", sharedContentFiles));

            string directoryPart = Path.GetDirectoryName(sharedContentFiles);
            if (String.IsNullOrEmpty(directoryPart))
                directoryPart = Environment.CurrentDirectory;

            directoryPart = Path.GetFullPath(directoryPart);
            string filePart = Path.GetFileName(sharedContentFiles);
            string[] files = Directory.GetFiles(directoryPart, filePart);
            
            foreach (string file in files)
                LoadContent(file);

            LogMessage(BuildLoggerLevel.Info, String.Format(
                "Found {0} files in {1}.", files.Length, sharedContentFiles));
        }

        public void LoadContent(string contentFile)
        {
            if (_dicContents == null)
            {
                _dicContents = new BuildProperties();
            }

            LogMessage(BuildLoggerLevel.Info, String.Format(
                "Loading shared content file '{0}'.", contentFile));

            XmlReader reader = null;
            try
            {
                reader = XmlReader.Create(contentFile);
                reader.MoveToContent();
                while (!reader.EOF)
                {
                    if ((reader.NodeType == XmlNodeType.Element) &&
                        (reader.Name == "item"))
                    {

                        string key = reader.GetAttribute("id");
                        string value = reader.ReadInnerXml();
                        _dicContents[key] = value;
                    }
                    else
                    {
                        reader.Read();
                    }
                }
            }
            catch (IOException ex)
            {
                LogMessage(BuildLoggerLevel.Error, String.Format(
                    "The shared content file '{0}' could not be opened. The error message is: {1}",
                    contentFile, GetExceptionMessage(ex)));
            }
            catch (XmlException ex)
            {
                LogMessage(BuildLoggerLevel.Error, String.Format(
                    "The shared content file '{0}' is not well-formed. The error message is: {1}",
                    contentFile, GetExceptionMessage(ex)));
            }
            catch (XmlSchemaException ex)
            {
                LogMessage(BuildLoggerLevel.Error, String.Format(
                    "The shared content file '{0}' is not valid. The error message is: {1}",
                    contentFile, GetExceptionMessage(ex)));
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
        }

        #endregion

        #region Protected Methods

        // look up shared content
        protected virtual string GetContent(string key, string[] parameters)
        {   
            string value = String.Empty;
            if (_dicContents != null && _dicContents.TryGetValue(key, out value))
            {
                if (parameters != null && parameters.Length != 0)
                {
                    try
                    {
                        value = String.Format(value, parameters);
                    }
                    catch
                    {
                        LogMessage(BuildLoggerLevel.Error, String.Format(
                            "The shared content item '{0}' could not be formatted with {1} parameters.",
                            key, parameters.Length));
                    }
                }
            }

            return value;
        }

        protected void LogMessage(BuildLoggerLevel level, string message)
        {
            if (_logger != null)
            {
                _logger.WriteLine(message, level);
            }
        }

        #endregion

        #region Private Methods

        private void ResolveContent(XmlDocument document, XPathNavigator start)
        {
            if (_pathSelector == null)
            {
                return;
            }

            // find all such elements
            XPathNodeIterator nodeIterator = start.Select(_pathSelector);
            if (nodeIterator == null || nodeIterator.Count == 0)
            {
                return;
            }

            // convert to an array so as not to cause an error when manipulating the document
            XPathNavigator[] nodes = ToArray(nodeIterator);

            // process each element
            int nodeCount = nodes.Length;
            for (int i = 0; i < nodeCount; i++)
            {
                XPathNavigator node = nodes[i];
                // get the key
                string item = node.Evaluate(_itemSelector).ToString();

                // check for missing key
                if (String.IsNullOrEmpty(item))
                {
                    LogMessage(BuildLoggerLevel.Warn,
                        "A shared content element did not specify an item.");
                }
                else
                {
                    // extract parameters
                    List<string> parameters = new List<string>();
                    XPathNodeIterator parameter_nodes = node.Select(_parametersSelector);
                    foreach (XPathNavigator parameter_node in parameter_nodes)
                    {
                        string parameter = GetInnerXml(parameter_node);
                        parameters.Add(parameter);
                    }

                    // get the content
                    string content = GetContent(item, parameters.ToArray());

                    // check for missing content
                    if (content == null)
                    {
                        if (_warnIfNotFound)
                        {
                            LogMessage(BuildLoggerLevel.Warn, String.Format(
                                "Missing shared content item. Tag:'{0}'; Id:'{1}'.",
                                node.LocalName, item));
                        }
                        if (_deleteIfNotFound)
                        {
                            node.DeleteSelf();
                        }
                    }
                    else
                    {
                        // store the content in a document fragment
                        XmlDocumentFragment fragment = document.CreateDocumentFragment();
                        fragment.InnerXml = content;

                        // resolve any shared content in the fragment
                        ResolveContent(document, fragment.CreateNavigator());

                        // look for an attribute name
                        string attribute = node.Evaluate(_attributeSelector).ToString();

                        // insert the resolved content
                        if (String.IsNullOrEmpty(attribute))
                        {
                            // as mixed content
                            XmlWriter writer = node.InsertAfter();
                            fragment.WriteTo(writer);
                            writer.Close();
                        }
                        else
                        {
                            // as an attribute
                            XPathNavigator parent = node.CreateNavigator();
                            parent.MoveToParent();
                            parent.CreateAttribute(String.Empty, attribute,
                                String.Empty, fragment.InnerText);
                        }
                    }
                }

                // keep a reference to the parent element
                XPathNavigator parentElement = node.CreateNavigator();
                parentElement.MoveToParent();

                // remove the node
                node.DeleteSelf();

                // if there is no content left in the parent element, make sure it is self-closing
                if (!parentElement.HasChildren && !parentElement.IsEmptyElement)
                {
                    //If 'node' was already the root then we will have a blank node now and 
                    //doing an InsertAfter() will throw an exception.
                    if (parentElement.Name.Length > 0)
                    {
                        // create a new element
                        XmlWriter attributeWriter = parentElement.InsertAfter();
                        attributeWriter.WriteStartElement(parentElement.Prefix,
                            parentElement.LocalName, parentElement.NamespaceURI);

                        // copy attributes to it
                        XmlReader attributeReader = parentElement.ReadSubtree();
                        attributeReader.Read();
                        attributeWriter.WriteAttributes(attributeReader, false);
                        attributeReader.Close();

                        // close it
                        attributeWriter.WriteEndElement();
                        attributeWriter.Close();

                        // delete the old element
                        parentElement.DeleteSelf();
                    }
                    else
                    {
                        //if we are inside a tag such as title, removing the content will make it in the
                        //form of <title /> which is not allowed in html. 
                        //Since this usually means there is a problem with the shared content or the transforms
                        //leading up to this we will just report the error here.
                        LogMessage(BuildLoggerLevel.Error, "Error replacing item.");
                    }
                }
            }
        }

        #endregion

        #region Private Static Methods

        private static XPathNavigator[] ToArray(XPathNodeIterator iterator)
        {
            XPathNavigator[] result = new XPathNavigator[iterator.Count];
            int itemCount = result.Length;
            for (int i = 0; i < itemCount; i++)
            {
                iterator.MoveNext();
                result[i] = iterator.Current.Clone();
            }

            return result;
        }

        private static string GetExceptionMessage(Exception ex)
        {
            string message = ex.Message;

            XmlException xmlE = ex as XmlException;
            if (xmlE != null)
            {
                message = String.Format(
                    "{0} (LineNumber: {1}; LinePosition: {2}; SourceUri: '{3}')",
                    message, xmlE.LineNumber, xmlE.LinePosition, xmlE.SourceUri);
            }
            else
            {
                XsltException xslE = ex as XsltException;
                if (xslE != null)
                {
                    message = String.Format(
                        "{0} (LineNumber: {1}; LinePosition: {2}; SourceUri: '{3}')",
                        message, xslE.LineNumber, xslE.LinePosition, xslE.SourceUri);
                }
            }

            if (ex.InnerException != null)
                message = String.Format("{0} {1}", message,
                    GetExceptionMessage(ex.InnerException));

            return message;
        }

        // get InnerXml without changing the spacing
        private static string GetInnerXml(XPathNavigator node)
        {
            // check for null argument, and clone so we don't change input
            if (node == null)
                throw new ArgumentNullException("node");
            XPathNavigator current = node.Clone();

            // create appropriate settings for the output writer
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.ConformanceLevel = ConformanceLevel.Fragment;
            settings.OmitXmlDeclaration = true;

            // construct a writer for our output
            StringBuilder builder = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(builder, settings);

            // write the output
            bool writing = current.MoveToFirstChild();
            while (writing)
            {
                current.WriteSubtree(writer);
                writing = current.MoveToNext();
            }

            // finish up and return the result
            writer.Close();

            return builder.ToString();
        }

        #endregion

        #region IDisposable Members

        /// <overloads>
        /// This performs build configuration tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </overloads>
        /// <summary>
        /// This performs build configuration tasks associated with freeing, releasing, or 
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);            
        }

        /// <summary>
        /// This cleans up any resources being used.
        /// </summary>
        /// <param name="disposing">
        /// This is <see langword="true"/> if managed resources should be 
        /// disposed; otherwise, <see langword="false"/>.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }    

    public sealed class CustomContext : XsltContext
    {            
        // variable control
        private Dictionary<string, IXsltContextVariable> variables = new Dictionary<string, IXsltContextVariable>();

        public CustomContext() 
            : base() 
        { 
        }

        public string this[string variable]
        {
            get
            {
                return (variables[variable].Evaluate(this).ToString());
            }
            set
            {
                variables[variable] = new CustomVariable(value);
            }
        }

        public bool ClearVariable(string name)
        {
            return (variables.Remove(name));
        }

        public void ClearVariables()
        {
            variables.Clear();
        }

        // Implementation of XsltContext methods

        public override IXsltContextVariable ResolveVariable(
            string prefix, string name)
        {
            return (variables[name]);
        }

        public override IXsltContextFunction ResolveFunction(
            string prefix, string name, XPathResultType[] argumentTypes)
        {
            throw new NotImplementedException();
        }

        public override int CompareDocument(string baseUri, string nextBaseUri)
        {
            return (0);
        }

        public override bool Whitespace
        {
            get
            {
                return (true);
            }
        }

        public override bool PreserveWhitespace(XPathNavigator node)
        {
            return (true);
        } 
    }        

    internal sealed class CustomVariable : IXsltContextVariable
    {
        private string value;

        public CustomVariable(string value)
        {
            this.value = value;
        }

        public bool IsLocal
        {
            get
            {
                return (false);
            }
        }

        public bool IsParam
        {
            get
            {
                return (false);
            }
        }

        public XPathResultType VariableType
        {
            get
            {
                return (XPathResultType.String);
            }
        }

        public Object Evaluate(XsltContext context)
        {
            return (value);
        }  
    }      
}
