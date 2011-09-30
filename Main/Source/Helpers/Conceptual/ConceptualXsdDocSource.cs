using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Collections.Generic;

using XsdDocumentation;
using XsdDocumentation.Model;
using XsdDocumentation.Markup;

using Sandcastle.Contents;
using Sandcastle.Utilities;

namespace Sandcastle.Conceptual
{
    /// <summary>
    /// A conceptual documentation content source for <c>XML</c> schema.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This uses the <see href="http://xsddoc.codeplex.com/">XML Schema Documenter</see>
    /// library to document <c>XML</c> schema files (or <c>XSD</c>).
    /// </para>
    /// <para>
    /// The <c>XML Schema Documenter</c> library is created by <c>Immo Landwerth</c>.
    /// </para>
    /// <para>
    /// The documentations are mainly extracted from the <c>XML</c> schema 
    /// <see href="http://www.w3.org/TR/xmlschema-0/#CommVers">Annotations</see>,
    /// but external <c>XML</c> document files are also supported.
    /// </para>
    /// </remarks>
    [Serializable]
    public sealed class ConceptualXsdDocSource : ConceptualSource
    {
        #region Public Static Fields

        public const string SourceName =
            "Sandcastle.Conceptual.ConceptualXsdDocSource";

        #endregion

        #region Private Fields

        private bool   _documentConstraints;
        private bool   _documentRootElements;
        private bool   _documentRootSchemas;
        private bool   _documentSchemas;
        private bool   _documentSyntax;
        private bool   _namespaceContainer;
        private bool   _schemaSetContainer;
        private bool   _undocumentedElementInheritsTypeDoc;
        private bool   _undocumentedAttributeInheritsTypeDoc;
        private bool   _includeLinkUriInKeywordK;
        private bool   _includeAutoOutline;
        private bool   _includeMoveToTop;
        private string _schemaSetTitle;

        private BuildProperties          _namespaceRenaming;

        private BuildFilePath            _transformFileName;
        private BuildList<BuildFilePath> _schemaFileNames;
        private BuildList<BuildFilePath> _schemaDependencyFileNames;
        private BuildList<BuildFilePath> _externalDocumentFileNames;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ConceptualXsdDocSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualXsdDocSource"/> class
        /// with the default parameters.
        /// </summary>
        public ConceptualXsdDocSource()
        {
            _schemaFileNames           = new BuildList<BuildFilePath>();
            _schemaDependencyFileNames = new BuildList<BuildFilePath>();
            _externalDocumentFileNames = new BuildList<BuildFilePath>();

            _namespaceRenaming         = new BuildProperties();

            _includeMoveToTop          = true;
            _includeAutoOutline        = true;
            _documentRootSchemas       = true;
            _documentRootElements      = true;
            _documentConstraints       = true;
            _documentSyntax            = true;
            _documentSchemas           = true;
            _namespaceContainer        = true;

            _undocumentedElementInheritsTypeDoc = true;
            _undocumentedAttributeInheritsTypeDoc = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConceptualXsdDocSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ConceptualXsdDocSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ConceptualXsdDocSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ConceptualXsdDocSource(ConceptualXsdDocSource source)
            : base(source)
        {
            _documentConstraints      = source._documentConstraints;
            _documentRootElements     = source._documentRootElements;
            _documentRootSchemas      = source._documentRootSchemas;
            _documentSchemas          = source._documentSchemas;
            _documentSyntax           = source._documentSyntax;
            _namespaceContainer       = source._namespaceContainer;
            _schemaSetContainer       = source._schemaSetContainer;
            _includeLinkUriInKeywordK = source._includeLinkUriInKeywordK;
            _includeAutoOutline       = source._includeAutoOutline;
            _includeMoveToTop         = source._includeMoveToTop;
            _schemaSetTitle           = source._schemaSetTitle;

            _undocumentedElementInheritsTypeDoc   = source._undocumentedElementInheritsTypeDoc;
            _undocumentedAttributeInheritsTypeDoc = source._undocumentedAttributeInheritsTypeDoc;

            _namespaceRenaming         = source._namespaceRenaming;
            _transformFileName         = source._transformFileName;
            _schemaFileNames           = source._schemaFileNames;
            _schemaDependencyFileNames = source._schemaDependencyFileNames;
            _externalDocumentFileNames = source._externalDocumentFileNames;
       }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name of this documentation content source.
        /// </summary>
        /// <value>
        /// A string containing the unique name of this documentation content
        /// source. The value is <see cref="ConceptualXsdDocSource.SourceName"/>.
        /// </value>
        public override string Name
        {
            get
            {
                return ConceptualXsdDocSource.SourceName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this content source is valid.
        /// </summary>
        /// <value>
        /// This is <see langword="true"/> if this content source is valid;
        /// otherwise, it is <see langword="false"/>.
        /// </value>
        public override bool IsValid
        {
            get
            {
                IList<BuildFilePath> schemaFiles = this.SchemaFileNames;
                if (schemaFiles == null || schemaFiles.Count == 0)
                {
                    return false;
                }
                for (int i = 0; i < schemaFiles.Count; i++)
                {
                    BuildFilePath filePath = schemaFiles[i];
                    if (filePath == null || !filePath.Exists)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to document the root schemas.
        /// </summary>
        /// <value>
        /// If <see langword="true"/>, the root schemas get their own entry 
        /// in the table of contents; otherwise, this is not included. The default
        /// is <see langword="true"/>.
        /// </value>
        /// <remarks>                
        /// A root schema is a schema that is not referenced by any other 
        /// schema. Documenting schemas can be useful if you have a large 
        /// schema set because it documents which schema files should be referenced.
        /// </remarks>
        public bool DocumentRootSchemas 
        { 
            get
            {
                return _documentRootSchemas;
            }
            set
            {
                _documentRootSchemas = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to document the root elements.
        /// </summary>
        /// <value>
        /// If <see langword="true"/>, the root elements get their own entry 
        /// in the table of contents; otherwise, this is not included. The 
        /// default value is <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// A root element is an element that is not referenced by an other 
        /// element except itself. Documenting roots elements can be useful 
        /// if you have a large schema set because it documents what are the 
        /// possible document elements.
        /// <note type="caution">         
        /// This requires the property <see cref="DocumentSchemas"/> be set
        /// to <see langword="true"/>.
        /// </note>
        /// </remarks>
        public bool DocumentRootElements 
        { 
            get
            {
                return _documentRootElements;
            }
            set
            {
                _documentRootElements = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to document the schema
        /// element constraints.
        /// </summary>
        /// <value>                
        /// If this property is <see langword="true"/>, the element constraints 
        /// <c>xs:unique</c>, <c>xs:key</c>, and <c>xs:keyref</c> are documented. 
        /// The default is <see langword="true"/>.
        /// </value>
        public bool DocumentConstraints 
        { 
            get
            {
                return _documentConstraints;
            }
            set
            {
                _documentConstraints = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to document the list of schemas.
        /// </summary>
        /// <value>            
        /// If this property is <see langword="true"/>, the schemas get their own entry in the 
        /// table of contents. The default value is <see langword="true"/>.
        /// </value>
        public bool DocumentSchemas 
        { 
            get
            {
                return _documentSchemas;
            }
            set
            {
                _documentSchemas = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to include the schema 
        /// element declaration as it appears in the file but without the annotations.
        /// </summary>
        /// <value>          
        /// If this property is <see langword="true"/>, every element, 
        /// attribute, group, attribute group, complex type, and simple type 
        /// topic contains a section showing their XSD declaration. The default
        /// value is <see langword="true"/>.
        /// </value>
        public bool DocumentSyntax 
        { 
            get
            {
                return _documentSyntax;
            }
            set
            {
                _documentSyntax = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether attributes without 
        /// documentation inherit the documentation from the associated simple type.
        /// </summary>
        /// <value>         
        /// If property is <see langword="true"/>, attributes without documentation 
        /// inherit the documentation from the associated simple type. The 
        /// default value is <see langword="true"/>.
        /// </value>
        public bool UndocumentedAttributeInheritsTypeDoc 
        { 
            get
            {
                return _undocumentedAttributeInheritsTypeDoc;
            }
            set
            {
                _undocumentedAttributeInheritsTypeDoc = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether elements without 
        /// documentation inherit the documentation from the associated 
        /// simple or complex type.  
        /// </summary>
        /// <value>               
        /// If property is <see langword="true"/>, elements without documentation 
        /// inherit the documentation from the associated simple type or 
        /// complex type. The default value is <see langword="true"/>.
        /// </value>
        public bool UndocumentedElementInheritsTypeDoc 
        { 
            get
            {
                return _undocumentedElementInheritsTypeDoc;
            }
            set
            {
                _undocumentedElementInheritsTypeDoc = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether to create a new container topic 
        /// that represents the whole schema set.
        /// </summary>
        /// <value> 
        /// If this property is <see cref="true"/>, all <c>XML</c> schema namespaces 
        /// will be summarized under this container topic; otherwise, all 
        /// namespaces will be listed as root entries in the table of contents.
        /// The default value is <see langword="false"/>.
        /// </value>
        public bool SchemaSetContainer 
        { 
            get
            {
                return _schemaSetContainer;
            }
            set
            {
                _schemaSetContainer = value;
            }
        }

        /// <summary>
        /// Gets or sets the title of the schema set container.
        /// </summary>
        /// <value>
        /// A string containing the schema set container title. The default is
        /// <c>Schema Set</c>.
        /// </value>      
        /// <remarks>
        /// This requires the <see cref="SchemaSetContainer"/> property to be
        /// set to <see langword="true"/>.
        /// </remarks>
        public string SchemaSetTitle 
        { 
            get
            {
                return _schemaSetTitle;
            }
            set
            {
                _schemaSetTitle = value;
            }
        }
        
        /// <summary>
        /// Gets or sets a value indicating whether to create namespace table
        /// of contents as container for namespace contents.
        /// </summary>    
        /// <value>
        /// If property is <see langword="true"/>, a namespace table of 
        /// content entry will be created as the container of the namespace 
        /// contents in the documented schema set; otherwise, the namespace 
        /// contents are listed directly. The default value is <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// This setting is ignored if the schema set contains more than one 
        /// namespace, in which case the namespace will always have a table 
        /// of content entry.
        /// </remarks>
        public bool NamespaceContainer 
        { 
            get
            {
                return _namespaceContainer;
            }
            set
            {
                _namespaceContainer = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether <c>XML</c> entity links such as 
        /// <c>http://schemas.example.org#E/anElement/@aAttribute</c> are 
        /// included in the <c>K</c> keyword index.
        /// </summary>
        /// <value>           
        /// If this property is <see langword="true"/>, the <c>XML</c> entity
        /// links are included in the keyword index. The default value is 
        /// <see langword="false"/>.
        /// </value>
        public bool IncludeLinkUriInKeywordK 
        { 
            get
            {
                return _includeLinkUriInKeywordK;
            }
            set
            {
                _includeLinkUriInKeywordK = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the automatic outlining
        /// of sections support is included in the generated topics.
        /// </summary>
        /// <value>
        /// If this property is <see langword="true"/>, the automatic outlining
        /// support is included. The default is <see langword="true"/>.
        /// </value>
        /// <remarks>
        /// The automatic outline of sections and sub-sections is an inline
        /// table of contents, which
        /// <list type="bullet">
        /// <item>
        /// <description>
        /// Provides an outline of the topic.
        /// </description>
        /// </item>
        /// <item>
        /// <description>
        /// Easy navigation, by providing links to sections and sub-sections
        /// of the topic.
        /// </description>
        /// </item>
        /// </list>       
        /// </remarks>
        public bool IncludeAutoOutline 
        { 
            get
            {
                return _includeAutoOutline;
            }
            set
            {
                _includeAutoOutline = value;
            }
        }
                  
        /// <summary>
        /// Gets or sets a value indicating whether a link to navigate to the
        /// top of the topic is included.
        /// </summary>
        /// <value>             
        /// If this is <see langword="true"/>, links are included to move to
        /// the top of the topic from sections of the topic. The default value
        /// is <see langword="true"/>.
        /// </value>
        public bool IncludeMoveToTop 
        { 
            get
            {
                return _includeMoveToTop;
            }
            set
            {
                _includeMoveToTop = value;
            }
        }

        /// <summary>
        /// Gets or sets the path of the <c>XSLT</c> transform file that can
        /// be used to transform custom inline <c>XSD</c> documentation into a
        /// format supported by this content source.
        /// </summary>
        /// <value>
        /// The path of the <c>XSLT</c> transform file, which will transform
        /// the sources to the <c>MAML</c> format used by the content source. 
        /// The default value is <see langword="null"/>.
        /// </value>
        /// <remarks>              
        /// If the annotations of the <c>XSD</c> files contain custom formatting
        /// and styling elements, for instance using <c>HTML</c> tags, you can
        /// provide an <c>XSLT</c> transform file to transform the files.
        /// </remarks>
        public BuildFilePath TransformFileName 
        { 
            get
            {
                return _transformFileName;
            }
            set
            {
                _transformFileName = value;
            }
        }

        /// <summary>
        /// Gets or sets the list of <c>XML</c> schema files to be documented.
        /// </summary>
        /// <value>
        /// A list of all the <c>XML</c> schema files from which to extract the
        /// annotations.
        /// </value>
        public IList<BuildFilePath> SchemaFileNames 
        { 
            get
            {
                return _schemaFileNames;
            }
            set
            {
                if (value != null)
                {
                    _schemaFileNames = new BuildList<BuildFilePath>(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the list of <c>XML</c> schema files that are needed
        /// or used by the <c>XML</c> schema set but should not be documented.
        /// </summary>
        /// <value>
        /// A list of dependency files used to resolve dependency elements
        /// used by the main schema files.
        /// </value>
        /// <remarks>
        /// These files are used to resolve the dependencies of the target
        /// <c>XML</c> schema files.
        /// </remarks>
        public IList<BuildFilePath> SchemaDependencyFileNames 
        { 
            get
            {
                return _schemaDependencyFileNames;
            }
            set
            {
                if (value != null)
                {
                    _schemaDependencyFileNames = 
                        new BuildList<BuildFilePath>(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the list of <c>XML</c> files that contain the external
        /// documentation for the schemas.
        /// </summary>
        /// <value>
        /// A list specifying all the external documentation files for the
        /// schemas.
        /// </value>
        public IList<BuildFilePath> ExternalDocumentFileNames 
        { 
            get
            {
                return _externalDocumentFileNames;
            }
            set
            {
                if (value != null)
                {
                    _externalDocumentFileNames = 
                        new BuildList<BuildFilePath>(value);
                }
            }
        }

        /// <summary>
        /// Gets or sets the mapping of <c>XML</c> namespaces to descriptive
        /// names of the namespaces. The descriptive names could be shorter
        /// and more readable.
        /// </summary>
        /// <value>
        /// A dictionary providing the mapping of the namespaces to the
        /// descriptive names.
        /// </value>
        public IDictionary<string, string> NamespaceRenaming 
        { 
            get
            {
                return _namespaceRenaming;
            }
            set
            {
                if (value != null)
                {
                    _namespaceRenaming = new BuildProperties();
                    foreach (KeyValuePair<string, string> pair in value)
                    {
                        if (!String.IsNullOrEmpty(pair.Key) && 
                            !String.IsNullOrEmpty(pair.Value))
                        {
                            _namespaceRenaming.Add(pair);
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public override ConceptualContent Create(BuildGroupContext groupContext)
        {
            BuildExceptions.NotNull(groupContext, "groupContext");

            BuildContext context = groupContext.Context;
            BuildLogger logger   = null;
            if (context != null)
            {
                logger = context.Logger;
            }

            if (!this.IsInitialized)
            {
                throw new BuildException(String.Format(
                    "The content source '{0}' is not yet initialized.", this.Title));
            }
            if (!this.IsValid)
            {
                if (logger != null)
                {
                    logger.WriteLine(String.Format(
                        "The content group source '{0}' is invalid.", this.Title),
                        BuildLoggerLevel.Warn);
                }

                return null;
            }

            Stopwatch watch = new Stopwatch();
            watch.Start();

            BuildGroup group = groupContext.Group;

            if (logger != null)
            {
                logger.WriteLine("Begin Generating XSD Topics: " + group.Name,
                    BuildLoggerLevel.Info);
            }

            XsdMessageReporter messageReporter = new XsdMessageReporter(logger);

            Configuration configuration = this.CreateConfiguration(context);
            XsdContentGenerator contentGenerator = new XsdContentGenerator(
                messageReporter, configuration);

            contentGenerator.Initialize(this.SourceContext, groupContext);

            contentGenerator.Generate();      
            ConceptualContent content = contentGenerator.Content;

            contentGenerator.Uninitialize();

            watch.Stop();

            if (logger != null)
            {
                string message = String.Format(
                    "A total of {0} conceptual XSD topics generated in {1}.",
                    contentGenerator.TopicCount, watch.Elapsed);

                logger.WriteLine(message, BuildLoggerLevel.Info);
                logger.WriteLine("Completed Generating XSD Topics: " + group.Name, 
                    BuildLoggerLevel.Info);
            }

            return content;
        }

        #endregion

        #region Private Methods

        #region CreateConfiguration Method

        private static IList<string> ExpandFiles(IList<BuildFilePath> filePaths)
        {
            List<string> result = new List<string>();
            if (filePaths == null || filePaths.Count == 0)
            {
                return result;
            }

            foreach (BuildFilePath path in filePaths)
            {
                if (path != null && path.Exists)
                {
                    result.Add(path.Path);
                }
            }

            return result;
        }

        private Configuration CreateConfiguration(BuildContext context)
        {
            Configuration config = new Configuration();
             
            config.OutputFolderPath     = context.WorkingDirectory;
            config.DocumentRootSchemas  = _documentRootSchemas;
            config.DocumentRootElements = _documentRootElements;
            config.DocumentConstraints  = _documentConstraints;
            config.DocumentSchemas      = _documentSchemas;
            config.DocumentSyntax       = _documentSyntax;
            config.UseTypeDocumentationForUndocumentedAttributes
                = _undocumentedAttributeInheritsTypeDoc;
            config.UseTypeDocumentationForUndocumentedElements
                = _undocumentedElementInheritsTypeDoc;
            config.SchemaSetContainer   = _schemaSetContainer;
            config.SchemaSetTitle       = _schemaSetTitle;
            config.NamespaceContainer   = _namespaceContainer;
            config.NamespaceRenaming    = _namespaceRenaming;

            config.IncludeLinkUriInKeywordK = _includeLinkUriInKeywordK;

            if (_transformFileName != null && _transformFileName.Exists)
            {
                config.AnnotationTransformFileName = _transformFileName.Path;
            }
            else
            {
                string transformFile = Path.Combine(context.Settings.SandAssistDirectory,
                    @"Transforms\Xsd\AnnotationTranform.xslt");
                if (File.Exists(transformFile))
                {
                    config.AnnotationTransformFileName = transformFile;
                }
            }
            config.SchemaFileNames = ExpandFiles(_schemaFileNames);
            config.SchemaDependencyFileNames = ExpandFiles(_schemaDependencyFileNames);
            config.DocFileNames = ExpandFiles(_externalDocumentFileNames);

            config.IncludeAutoOutline = _includeAutoOutline;
            config.IncludeMoveToTop   = _includeMoveToTop;

            return config;
        }

        #endregion

        #region ReadXmlGeneral Method

        private void ReadXmlGeneral(XmlReader reader)
        {
            string startElement = reader.Name;
            if (!String.Equals(startElement, "propertyGroup",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new BuildException(String.Format(
                    "ReadXmlGeneral: The current element is '{0}' not the expected 'propertyGroup'.",
                    startElement));
            }

            Debug.Assert(String.Equals(reader.GetAttribute("name"), "General"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "property",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string tempText = null;
                        switch (reader.GetAttribute("name").ToLower())
                        {
                            case "title":
                                this.Title = reader.ReadString();
                                break;
                            case "documentconstraints":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _documentConstraints = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "documentrootelements":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _documentRootElements = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "documentrootschemas":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _documentRootSchemas = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "documentschemas":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _documentSchemas = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "documentsyntax":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _documentSyntax = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "namespacecontainer":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _namespaceContainer = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "schemasetcontainer":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _schemaSetContainer = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "undocumentedelementinheritstypedoc":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _undocumentedElementInheritsTypeDoc = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "undocumentedattributeinheritstypedoc":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _undocumentedAttributeInheritsTypeDoc = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "includelinkuriinkeywordk":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _includeLinkUriInKeywordK = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "includeautooutline":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _includeAutoOutline = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "includemovetotop":
                                tempText = reader.ReadString();
                                if (!String.IsNullOrEmpty(tempText))
                                {
                                    _includeMoveToTop = Convert.ToBoolean(tempText);
                                }
                                break;
                            case "schemasettitle":
                                _schemaSetTitle = reader.ReadString();
                                break;
                            default:
                                // Should normally not reach here...
                                throw new NotImplementedException(reader.GetAttribute("name"));
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadXmlNamespaceRenaming Method

        private void ReadXmlNamespaceRenaming(XmlReader reader)
        {
            string startElement = reader.Name;
            if (!String.Equals(startElement, "namespaceRenaming",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new BuildException(String.Format(
                    "ReadXmlNamespaceRenaming: The current element is '{0}' not the expected 'namespaceRenaming'.",
                    startElement));
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_namespaceRenaming == null || _namespaceRenaming.Count != 0)
            {
                _namespaceRenaming = new BuildProperties();
            }

            //<namespaceRenaming>
            //    <namespace name="http://schemas.com/locale" title="Localization Schema" />
            //    <namespace name="http://schemas.com/others" title="Database Schema" />
            //</namespaceRenaming>

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "namespace",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string name  = reader.GetAttribute("name");
                        string title = reader.GetAttribute("title");

                        if (!String.IsNullOrEmpty(name) && 
                            !String.IsNullOrEmpty(title))
                        {
                            // It should be unique...
                            _namespaceRenaming.Add(name, title);
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadXmlSchemaFilePaths Method

        private void ReadXmlSchemaFilePaths(XmlReader reader)
        {
            string startElement = reader.Name;
            if (!String.Equals(startElement, "schemaFilePaths",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new BuildException(String.Format(
                    "ReadXmlSchemaFilePaths: The current element is '{0}' not the expected 'schemaFilePaths'.",
                    startElement));
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_schemaFileNames == null || _schemaFileNames.Count != 0)
            {
                _schemaFileNames = new BuildList<BuildFilePath>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, BuildFilePath.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        BuildFilePath filePath = new BuildFilePath();
                        filePath.ReadXml(reader);

                        if (filePath.IsValid)
                        {
                            _schemaFileNames.Add(filePath);
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadXmlSchemaDependencyFilePaths Method

        private void ReadXmlSchemaDependencyFilePaths(XmlReader reader)
        {
            string startElement = reader.Name;
            if (!String.Equals(startElement, "schemaDependencyFilePaths",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new BuildException(String.Format(
                    "ReadXmlSchemaDependencyFilePaths: The current element is '{0}' not the expected 'schemaDependencyFilePaths'.",
                    startElement));
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_schemaDependencyFileNames == null ||
                _schemaDependencyFileNames.Count != 0)
            {
                _schemaDependencyFileNames = new BuildList<BuildFilePath>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, BuildFilePath.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        BuildFilePath filePath = new BuildFilePath();
                        filePath.ReadXml(reader);

                        if (filePath.IsValid)
                        {
                            _schemaDependencyFileNames.Add(filePath);
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadXmlExternalDocumentFilePaths Method

        private void ReadXmlExternalDocumentFilePaths(XmlReader reader)
        {
            string startElement = reader.Name;
            if (!String.Equals(startElement, "externalDocumentFilePaths",
                StringComparison.OrdinalIgnoreCase))
            {
                throw new BuildException(String.Format(
                    "ReadXmlExternalDocumentFilePaths: The current element is '{0}' not the expected 'externalDocumentFilePaths'.",
                    startElement));
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_externalDocumentFileNames == null ||
                _externalDocumentFileNames.Count != 0)
            {
                _externalDocumentFileNames = new BuildList<BuildFilePath>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, BuildFilePath.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        BuildFilePath filePath = new BuildFilePath();
                        filePath.ReadXml(reader);

                        if (filePath.IsValid)
                        {
                            _externalDocumentFileNames.Add(filePath);
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(false, String.Format(
                    "The element name '{0}' does not match the expected '{1}'.",
                    reader.Name, TagName));
                return;
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "propertyGroup",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadXmlGeneral(reader);
                    }
                    else if (String.Equals(reader.Name, "namespaceRenaming",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadXmlNamespaceRenaming(reader);
                    }
                    else if (String.Equals(reader.Name, "transformFilePath",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        if (!reader.IsEmptyElement)
                        {
                            _transformFileName = BuildFilePath.ReadLocation(reader);
                        }
                    }
                    else if (String.Equals(reader.Name, "schemaFilePaths",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadXmlSchemaFilePaths(reader);
                    }
                    else if (String.Equals(reader.Name, "schemaDependencyFilePaths",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadXmlSchemaDependencyFilePaths(reader);
                    }
                    else if (String.Equals(reader.Name, "externalDocumentFilePaths",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadXmlExternalDocumentFilePaths(reader);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            if (!this.IsValid)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - TagName
            writer.WriteAttributeString("name", this.Name);

            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name", "General");
            writer.WritePropertyElement("Title",               this.Title);
            writer.WritePropertyElement("DocumentConstraints", _documentConstraints);
            writer.WritePropertyElement("DocumentRootElements", _documentRootElements);
            writer.WritePropertyElement("DocumentRootSchemas", _documentRootSchemas);
            writer.WritePropertyElement("DocumentSchemas", _documentSchemas);
            writer.WritePropertyElement("DocumentSyntax", _documentSyntax);
            writer.WritePropertyElement("NamespaceContainer", _namespaceContainer);
            writer.WritePropertyElement("SchemaSetContainer", _schemaSetContainer);
            writer.WritePropertyElement("UndocumentedElementInheritsTypeDoc", _undocumentedElementInheritsTypeDoc);
            writer.WritePropertyElement("UndocumentedAttributeInheritsTypeDoc", _undocumentedAttributeInheritsTypeDoc);
            writer.WritePropertyElement("IncludeLinkUriInKeywordK", _includeLinkUriInKeywordK);
            writer.WritePropertyElement("IncludeAutoOutline", _includeAutoOutline);
            writer.WritePropertyElement("IncludeMoveToTop", _includeMoveToTop);
            writer.WritePropertyElement("SchemaSetTitle", _schemaSetTitle);
            writer.WriteEndElement();                   // end - propertyGroup

            writer.WriteStartElement("namespaceRenaming"); // start - namespaceRenaming
            if (_namespaceRenaming != null)
            {
                foreach (KeyValuePair<string, string> pair in _namespaceRenaming)
                {
                    writer.WriteStartElement("namespace");
                    writer.WriteAttributeString("name",  pair.Key);
                    writer.WriteAttributeString("title", pair.Value);
                    writer.WriteEndElement();
                }
            }
            writer.WriteEndElement();                      // end - namespaceRenaming

            writer.WriteStartElement("transformFilePath");
            if (_transformFileName != null)
            {
                _transformFileName.WriteXml(writer);
            }
            writer.WriteEndElement();

            writer.WriteStartElement("schemaFilePaths");
            if (_schemaFileNames != null && _schemaFileNames.Count != 0)
            {
                for (int i = 0; i < _schemaFileNames.Count; i++)
                {
                    _schemaFileNames[i].WriteXml(writer);
                }
            }
            writer.WriteEndElement();

            writer.WriteStartElement("schemaDependencyFilePaths");
            if (_schemaDependencyFileNames != null && _schemaDependencyFileNames.Count != 0)
            {
                for (int i = 0; i < _schemaDependencyFileNames.Count; i++)
                {
                    _schemaDependencyFileNames[i].WriteXml(writer);
                }
            }
            writer.WriteEndElement();

            writer.WriteStartElement("externalDocumentFilePaths");
            if (_externalDocumentFileNames != null && _externalDocumentFileNames.Count != 0)
            {
                for (int i = 0; i < _externalDocumentFileNames.Count; i++)
                {
                    _externalDocumentFileNames[i].WriteXml(writer);
                }
            }
            writer.WriteEndElement();  

            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region ICloneable Members

        public override ConceptualSource Clone()
        {
            ConceptualXsdDocSource source = new ConceptualXsdDocSource(this);
            if (this.Title != null)
            {
                source.Title = String.Copy(this.Title);
            }
            if (_schemaSetTitle != null)
            {
                source._schemaSetTitle = String.Copy(_schemaSetTitle);
            }
            if (_namespaceRenaming != null)
            {
                source._namespaceRenaming = _namespaceRenaming.Clone();
            }
            if (_transformFileName != null)
            {
                source._transformFileName = _transformFileName.Clone();
            }
            if (_schemaFileNames != null)
            {
                source._schemaFileNames = _schemaFileNames.Clone();
            }
            if (_schemaDependencyFileNames != null)
            {
                source._schemaDependencyFileNames = _schemaDependencyFileNames.Clone();
            }
            if (_externalDocumentFileNames != null)
            {
                source._externalDocumentFileNames = _externalDocumentFileNames.Clone();
            }

            return source;
        }

        #endregion

        #region XsdContentGenerator Class

        private sealed class XsdContentGenerator : ContentGenerator, IDisposable
        {
            #region Private Fields

            private int                _topicCount;

            private string             _ddueXmlDir;
            private string             _ddueCompDir;
            private string             _workingDir;

            private XmlWriter          _tocWriter;
            private XmlWriter          _indexWriter;
            private XmlWriter          _manifestWriter;
            private XmlWriter          _metadataWriter; 

            private XmlWriterSettings  _settings;

            private BuildGroupContext  _groupContext;
            private BuildSourceContext _sourceContext;

            private ConceptualContent  _content;

            private BuildPathResolver  _resolver;

            #endregion

            #region Constructors and Destructor

            public XsdContentGenerator(IMessageReporter messageReporter, Configuration configuration)
                : base(messageReporter, configuration)
            {
            }

            ~XsdContentGenerator()
            {
                this.Dispose(false);
            }

            #endregion

            #region Public Properties

            public ConceptualContent Content
            {
                get
                {
                    return _content;
                }
            }

            public int TopicCount
            {
                get
                {
                    return _topicCount;
                }
            }

            #endregion

            #region Public Methods

            public void Initialize(BuildSourceContext sourceContext, 
                BuildGroupContext groupContext)
            {
                BuildExceptions.NotNull(sourceContext, "sourceContext");
                BuildExceptions.NotNull(groupContext,  "groupContext");

                _topicCount    = 0;

                _sourceContext = sourceContext;
                _groupContext  = groupContext;

                // This content requires the link text to work as expected...
                _groupContext["$ShowLinkText"]       = Boolean.TrueString;
                _groupContext["$LinkResolverPrefix"] = "xsd";
                _groupContext["$LinkResolverName"]   = "http://schemas.xsddoc.codeplex.com/schemaDoc/2009/3";
                _groupContext["$LinkResolverValue"]  = "/*//xsd:xmlEntityReference";
                _groupContext["$LinkResolverTopicType"] = "3272D745-2FFC-48C4-9E9D-CF2B2B784D5F";

                Configuration configuration = this.Configuration;

                _workingDir  = configuration.OutputFolderPath;

                _ddueXmlDir  = Path.Combine(_workingDir, groupContext["$DdueXmlDir"]);
                _ddueCompDir = Path.Combine(_workingDir, groupContext["$DdueXmlCompDir"]);

                if (!Directory.Exists(_ddueXmlDir))
                {
                    Directory.CreateDirectory(_ddueXmlDir);
                }
                if (!Directory.Exists(_ddueCompDir))
                {
                    Directory.CreateDirectory(_ddueCompDir);
                }

                string manifestFile = Path.Combine(_workingDir,
                    groupContext["$ManifestFile"]);
                string metadataFile = Path.Combine(_workingDir,
                    groupContext["$MetadataFile"]);
                string tocFile = Path.Combine(_workingDir,
                    groupContext["$TocFile"]);

                this.TopicsFolder = _sourceContext.TopicsDir;
                this.ContentFile = _sourceContext.TopicsFile;
                this.IndexFile = Path.Combine(_workingDir,
                    _groupContext["$IndexFile"]);
                this.MediaFolder = _sourceContext.MediaDir;

                _settings = new XmlWriterSettings();
                _settings.Indent   = true;
                _settings.Encoding = Encoding.UTF8;

                _manifestWriter = XmlWriter.Create(manifestFile, _settings);
                _manifestWriter.WriteStartDocument();
                _manifestWriter.WriteStartElement("topics"); // start - topics

                _metadataWriter = XmlWriter.Create(metadataFile, _settings);
                _metadataWriter.WriteStartDocument();
                _metadataWriter.WriteStartElement("metadata"); // start - metadata
                _metadataWriter.WriteAttributeString("fileAssetGuid", 
                    Guid.NewGuid().ToString());
                _metadataWriter.WriteAttributeString("assetTypeId", "ContentMetadata");

                _tocWriter = XmlWriter.Create(tocFile, _settings);
                _tocWriter.WriteStartDocument();
                _tocWriter.WriteStartElement("topics"); // start - topics

                _indexWriter = XmlWriter.Create(this.IndexFile, _settings);
                _indexWriter.WriteStartDocument();
                _indexWriter.WriteStartElement("topics"); // start - topics
            }

            public void Uninitialize()
            {   
                if (_manifestWriter != null)
                {
                    _manifestWriter.WriteEndElement();       // end - topics
                    _manifestWriter.WriteEndDocument();

                    _manifestWriter.Close();
                    _manifestWriter = null;
                }
                if (_metadataWriter != null)
                {
                    _metadataWriter.WriteEndElement();       // end - metadata
                    _metadataWriter.WriteEndDocument();

                    _metadataWriter.Close();
                    _metadataWriter = null;
                }
                if (_tocWriter != null)
                {
                    _tocWriter.WriteEndElement();       // end - topics
                    _tocWriter.WriteEndDocument();
                    _tocWriter.Close();
                    _tocWriter = null;
                }
                if (_indexWriter != null)
                {
                    _indexWriter.WriteEndElement();       // end - topics
                    _indexWriter.WriteEndDocument();
                    _indexWriter.Close();
                    _indexWriter = null;
                }
            }
                 
            public override void Generate()
            {
                //this.GenerateIndex();
                this.GenerateContentFile();
                this.GenerateTopicFiles();
                this.GenerateMediaFiles();
            }

            #endregion

            #region Protected Methods

            protected override void GenerateMediaFiles()
            {
                string mediaSourceDir = Path.Combine(
                    _groupContext.Context.Settings.SandAssistDirectory,
                    @"Media\Xsd\");

                string mediaDestDir = this.MediaFolder;
                if (!Directory.Exists(mediaDestDir))
                {
                    Directory.CreateDirectory(mediaDestDir);
                }

                string mediaFile = _sourceContext.MediaFile;
                BuildPathResolver resolver = BuildPathResolver.Create(
                    Path.GetDirectoryName(mediaFile));

                BuildFilePath mediaFilePath = new BuildFilePath(mediaFile);
                BuildDirectoryPath mediaDir = new BuildDirectoryPath(mediaDestDir);

                using (XmlWriter writer = XmlWriter.Create(mediaFile, _settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement(MediaContent.TagName);
                    writer.WriteAttributeString("id", Guid.NewGuid().ToString());
                    writer.WriteAttributeString("version", "1.0");

                    writer.WriteStartElement("location");
                    if (!mediaDir.IsDirectoryOf(mediaFilePath))
                    {
                        writer.WriteStartElement(BuildDirectoryPath.TagName);
                        writer.WriteAttributeString("value",
                            resolver.ResolveRelative(mediaDir.Path));
                        writer.WriteEndElement();
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("items");  // start - items
                    foreach (var artItem in ArtItem.ArtItems)
                    {
                        string sourceFile = Path.Combine(mediaSourceDir, artItem.FileName);
                        string destinationFile = Path.Combine(mediaDestDir, artItem.FileName);
                        File.Copy(sourceFile, destinationFile, true);
                        File.SetAttributes(destinationFile, FileAttributes.Normal);

                        writer.WriteStartElement("item");  // start - item
                        writer.WriteAttributeString("id", artItem.Id);
                        
                        writer.WriteStartElement("image"); // start - image
                        writer.WriteAttributeString("file",
                            resolver.ResolveRelative(destinationFile));

                        writer.WriteElementString("altText", artItem.AlternateText);
                        
                        writer.WriteEndElement();          // end - image                        
                        writer.WriteEndElement();          // end - item
                    }
                    writer.WriteEndElement();           // end - items

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }

            protected override void GenerateContentFile()
            {
                IList<Topic> listTopics = this.Context.TopicManager.Topics;
                if (listTopics == null || listTopics.Count == 0)
                {
                    return;
                }

                string topicsFile = this.ContentFile;

                string fileDirectory = Path.GetDirectoryName(topicsFile);
                if (!Directory.Exists(fileDirectory))
                    Directory.CreateDirectory(fileDirectory);

                if (!File.Exists(topicsFile))
                {
                    // ConceptualContent requires the file to exist, create
                    // a temporal XML file...
                    File.WriteAllText(topicsFile, 
                        "<?xml version=\"1.0\" encoding=\"utf-8\"?>", Encoding.UTF8);
                }

                _content = new ConceptualContent(topicsFile, this.TopicsFolder);

                BuildFilePath contentFile     = _content.ContentFile;
                BuildDirectoryPath contentDir = _content.ContentDir;

                _resolver = BuildPathResolver.Create(fileDirectory);

                using (XmlWriter writer = XmlWriter.Create(topicsFile, _settings))
                {
                    writer.WriteStartDocument();
                    writer.WriteStartElement(ConceptualContent.TagName);
                    writer.WriteAttributeString("version", "1.0.0.0");

                    writer.WriteStartElement("location"); // location
                    if (contentDir != null &&
                        !contentDir.IsDirectoryOf(contentFile))
                    {
                        writer.WriteStartElement(BuildDirectoryPath.TagName);
                        writer.WriteAttributeString("value",
                            _resolver.ResolveRelative(contentDir.Path));
                        writer.WriteEndElement();             
                    }
                    writer.WriteEndElement();             // location

                    writer.WriteStartElement("propertyGroup");
                    writer.WriteAttributeString("name", "General");
                    writer.WritePropertyElement("Id",   _content.Id);
                    writer.WritePropertyElement("CompanionFiles",
                        Convert.ToBoolean(_content.CompanionFiles));
                    writer.WriteEndElement();

                    writer.WriteStartElement("topics");
                    writer.WriteAttributeString("default", listTopics[0].Id);
                    for (int i = 0; i < listTopics.Count; i++)
                    {
                        this.WriteTopic(writer, listTopics[i]);
                    }
                    writer.WriteEndElement();

                    writer.WriteStartElement("relatedTopics");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                    writer.WriteEndDocument();
                }
            }

            protected override string GetAbsoluteFileName(string topicsFolder,
                Topic topic)
            {
                return Path.Combine(topicsFolder,
                    Path.ChangeExtension(topic.Id, ".xml"));
            }

            #endregion

            #region Private Methods

            private void WriteTopic(XmlWriter writer, Topic topic)
            {
                // Signal the beginning of a topic...
                this.OnBeginTopic(topic);

                // Signal the writing of a topic...
                this.OnWriteTopic(topic);

                // Write the current topic...
                writer.WriteStartElement(ConceptualItem.TagName); // start - topic
                writer.WriteAttributeString("id", topic.Id);
                writer.WriteAttributeString("type", "Topic");
                writer.WriteAttributeString("visible", Boolean.TrueString);
                writer.WriteAttributeString("revision", "1");
                writer.WriteTextElement("title", topic.Title);
                writer.WriteStartElement(BuildFilePath.TagName);
                writer.WriteAttributeString("value",
                    _resolver.ResolveRelative(GetAbsoluteFileName(this.TopicsFolder, topic)));
                writer.WriteEndElement();             

                writer.WriteStartElement("excludes");
                writer.WriteEndElement();

                WriteMetadata(writer, topic, false);

                // Write the sub-topics
                IList<Topic> listTopics = topic.Children;

                for (int i = 0; i < listTopics.Count; i++)
                {
                    this.WriteTopic(writer, listTopics[i]);
                }

                writer.WriteEndElement();                         // end - topic

                // Signal the end of a topic...
                this.OnEndTopic(topic);
            }

            private void OnBeginTopic(Topic topic)
            {
                // For the TOC...
                _tocWriter.WriteStartElement("topic");
                _tocWriter.WriteAttributeString("id",   topic.Id);
                _tocWriter.WriteAttributeString("file", topic.Id);  
            }

            private void OnWriteTopic(Topic topic)
            {
                _topicCount++;

                // For the manifest...
                _manifestWriter.WriteStartElement("topic");
                _manifestWriter.WriteAttributeString("id", topic.Id);
                _manifestWriter.WriteEndElement();

                // For the metadata...
                _metadataWriter.WriteStartElement("topic");
                _metadataWriter.WriteAttributeString("id", topic.Id);
                _metadataWriter.WriteAttributeString("revisionNumber", "1");

                _metadataWriter.WriteStartElement("item");
                _metadataWriter.WriteAttributeString("id", "PBM_FileVersion");
                _metadataWriter.WriteString("1.0.0.0");
                _metadataWriter.WriteEndElement();

                _metadataWriter.WriteStartElement("title");
                _metadataWriter.WriteString(topic.Title);
                _metadataWriter.WriteEndElement();

                if (!String.IsNullOrEmpty(topic.TocTitle))
                {
                    _metadataWriter.WriteStartElement("tableOfContentsTitle");
                    _metadataWriter.WriteString(topic.TocTitle);
                    _metadataWriter.WriteEndElement();
                }

                _metadataWriter.WriteStartElement("runningHeaderText");
                _metadataWriter.WriteAttributeString("uscid", "runningHeaderText");
                _metadataWriter.WriteEndElement();

                _metadataWriter.WriteStartElement("topicType");
                _metadataWriter.WriteAttributeString("id", "3272D745-2FFC-48C4-9E9D-CF2B2B784D5F");
                _metadataWriter.WriteEndElement();

                _metadataWriter.WriteEndElement();

                // Write the index file for the topic...
                this.OnWriteIndex(topic);
 
                // Write the companion file for the topic...
                this.OnWriteMetadata(topic);
           }

            private void OnEndTopic(Topic topic)
            {
                // For the TOC...
                _tocWriter.WriteEndElement();
            }

            private void OnWriteIndex(Topic topic)
            {
                if (topic.LinkUri != null)
                {
                    _indexWriter.WriteStartElement("topic");
                    _indexWriter.WriteAttributeString("id", topic.Id);
                    _indexWriter.WriteAttributeString("linkText", topic.LinkTitle);
                    _indexWriter.WriteAttributeString("uri", topic.LinkUri);
                    _indexWriter.WriteEndElement();
                }
                if (topic.LinkIdUri != null)
                {
                    _indexWriter.WriteStartElement("topic");
                    _indexWriter.WriteAttributeString("id", topic.Id);
                    _indexWriter.WriteAttributeString("linkText", topic.LinkTitle);
                    _indexWriter.WriteAttributeString("uri", topic.LinkIdUri);
                    _indexWriter.WriteEndElement();
                }
            }

            private void OnWriteMetadata(Topic topic)
            {
                string companionPath = Path.Combine(_ddueCompDir,
                    topic.Id + ".cmp.xml");

                using (XmlWriter writer = XmlWriter.Create(companionPath, 
                    _settings))
                {
                    writer.WriteStartDocument();

                    WriteMetadata(writer, topic, true);

                    writer.WriteEndDocument();
                }    
            }

            private static void WriteMetadata(XmlWriter writer, Topic topic,
                bool isStandalone)
            {
                writer.WriteStartElement("metadata"); // start - metadata

                // We include identifiers in the standalone metadata...
                if (isStandalone)
                {   
                    writer.WriteAttributeString("fileAssetGuid", topic.Id);
                    writer.WriteAttributeString("assetTypeId", "CompanionFile");

                    // We include the topic tag in only standalone metadata...
                    writer.WriteStartElement("topic"); // start - topic
                    writer.WriteAttributeString("id", topic.Id);

                    writer.WriteElementString("title", topic.Title);
                }

                if (!String.IsNullOrEmpty(topic.TocTitle))
                {
                    writer.WriteElementString("tableOfContentsTitle",
                        topic.TocTitle);
                }
                if (!String.IsNullOrEmpty(topic.LinkTitle))
                {
                    writer.WriteElementString("linkText", topic.LinkTitle);
                }

                // Write the attributes...
                // TODO--PAUL

                // Write the keywords...
                if (topic.KeywordsK.Count != 0 || topic.KeywordsF.Count != 0)
                {
                    if (!isStandalone)
                    {
                        writer.WriteStartElement("keywords"); // start - keywords
                    }

                    WriteKeywords(writer, topic.KeywordsK, "K");
                    WriteKeywords(writer, topic.KeywordsF, "F");

                    if (!isStandalone)
                    {
                        writer.WriteEndElement();             // end - keywords
                    }
                }

                if (isStandalone)
                {   
                    writer.WriteEndElement();          // end - topic   
                }
                writer.WriteEndElement();             // end - metadata
            }

            private static void WriteKeywords(XmlWriter writer,
                IList<string> listTerms, string keyIndex)
            {
                if (listTerms == null || listTerms.Count == 0)
                {
                    return;
                }

                writer.WriteStartElement("keyword");
                writer.WriteAttributeString("index", keyIndex);
                writer.WriteString(listTerms[0]);

                if (listTerms.Count > 1)
                {
                    for (int j = 1; j < listTerms.Count; j++)
                    {
                        writer.WriteStartElement("keyword");
                        writer.WriteAttributeString("index", keyIndex);
                        writer.WriteString(listTerms[j]);
                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
            }

            #endregion

            #region IDisposable Members

            public void Dispose()
            {
                this.Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (_manifestWriter != null)
                {
                    _manifestWriter.Close();
                    _manifestWriter = null;
                }
                if (_metadataWriter != null)
                {
                    _metadataWriter.Close();
                    _metadataWriter = null;
                }
                if (_tocWriter != null)
                {
                    _tocWriter.Close();
                    _tocWriter = null;
                }
                if (_indexWriter != null)
                {
                    _indexWriter.Close();
                    _indexWriter = null;
                }
            }

            #endregion
        }

        #endregion

        #region XsdMessageReporter Class

        private sealed class XsdMessageReporter : IMessageReporter
        {
            private BuildLogger _logger;

            public XsdMessageReporter(BuildLogger logger)
            {
                _logger = logger;
            }

            #region IMessageReporter Members

            public void ReportWarning(string warningCode, string message)
            {
                if (_logger == null || String.IsNullOrEmpty(message))
                {
                    return;
                }

                _logger.WriteLine(message, BuildLoggerLevel.Warn);
            }

            #endregion
        }

        #endregion
    }
}
