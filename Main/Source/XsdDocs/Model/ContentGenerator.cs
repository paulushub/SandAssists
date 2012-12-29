using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

using XsdDocumentation.Markup;

namespace XsdDocumentation.Model
{
    public abstract class ContentGenerator
    {
        #region Private Fields

        private bool             _includeTopicTag;

        private Context          _context;  
        private Configuration    _configuration;
        private IMessageReporter _messageReporter;

        #endregion

        #region Construtors and Destructor

        protected ContentGenerator(IMessageReporter messageReporter, 
            Configuration configuration)
        {
            _includeTopicTag = true;

            _context         = new Context(messageReporter, configuration);
            _configuration   = configuration;
            _messageReporter = messageReporter;

            MediaItems = new List<MediaItem>();
            TopicFiles = new List<string>();
        }

        #endregion

        #region Public Properties

        public string ContentFile { get; protected set; }
        public string IndexFile { get; protected set; }
        public string TopicsFolder { get; protected set; }
        public string MediaFolder { get; protected set; }
        public IList<string> TopicFiles { get; protected set; }
        public IList<MediaItem> MediaItems { get; protected set; }

        public Context Context
        {
            get
            {
                return _context;
            }
        }

        public Configuration Configuration
        {
            get
            {
                return _configuration;
            }
        }

        public IMessageReporter MessageReporter
        {
            get
            {
                return _messageReporter;
            }
        }

        public bool IncludeTopicTag
        {
            get
            {
                return _includeTopicTag;
            }
            protected set
            {
                _includeTopicTag = value;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Generate()
        {
            TopicsFolder = Path.Combine(_context.Configuration.OutputFolderPath, 
                "xsdTopics");
            ContentFile  = Path.Combine(TopicsFolder, "xsd.content");
            IndexFile    = Path.Combine(TopicsFolder, "xsd.index");
            MediaFolder  = Path.Combine(_context.Configuration.OutputFolderPath, 
                "xsdMedia");
            
            GenerateIndex();
            GenerateContentFile();
            GenerateTopicFiles();
            GenerateMediaFiles();
        }

        #endregion

        #region Protected Methods

        protected virtual void GenerateIndex()
        {
            TopicIndex topicIndex = new TopicIndex();
            topicIndex.Load(_context.TopicManager);
            topicIndex.Save(IndexFile);
        }

        protected virtual void GenerateTopicFiles()
        {
            Directory.CreateDirectory(TopicsFolder);

            GenerateTopicFiles(_context.TopicManager.Topics);
        }

        protected abstract void GenerateMediaFiles();
        protected abstract void GenerateContentFile();
        protected abstract string GetAbsoluteFileName(string topicsFolder,
            Topic topic);

        #endregion

        #region Private Methods

        private void GenerateTopicFiles(IEnumerable<Topic> topics)
        {
            foreach (var topic in topics)
            {
                topic.FileName = GetAbsoluteFileName(TopicsFolder, topic);
                TopicFiles.Add(topic.FileName);

                switch (topic.TopicType)
                {
                    case TopicType.SchemaSet:
                        GenerateSchemaSetTopic(topic);
                        break;
                    case TopicType.Namespace:
                        GenerateNamespaceTopic(topic);
                        break;
                    case TopicType.Schema:
                        GenerateSchemaTopic(topic);
                        break;
                    case TopicType.Element:
                        GenerateElementTopic(topic);
                        break;
                    case TopicType.Attribute:
                        GenerateAttributeTopic(topic);
                        break;
                    case TopicType.AttributeGroup:
                        GenerateAttributeGroup(topic);
                        break;
                    case TopicType.Group:
                        GenerateGroupTopic(topic);
                        break;
                    case TopicType.SimpleType:
                        GenerateSimpleTypeTopic(topic);
                        break;
                    case TopicType.ComplexType:
                        GenerateComplexTypeTopic(topic);
                        break;
                    case TopicType.RootSchemasSection:
                    case TopicType.RootElementsSection:
                    case TopicType.SchemasSection:
                    case TopicType.ElementsSection:
                    case TopicType.AttributesSection:
                    case TopicType.AttributeGroupsSection:
                    case TopicType.GroupsSection:
                    case TopicType.SimpleTypesSection:
                    case TopicType.ComplexTypesSection:
                        GenerateOverviewTopic(topic);
                        break;
                    default:
                        throw ExceptionBuilder.UnhandledCaseLabel(topic.TopicType);
                }

                GenerateTopicFiles(topic.Children);
            }
        }

        private void GenerateSchemaSetTopic(Topic topic)
        {
            if (_context.Configuration.NamespaceContainer)
            {
                using (var stream = File.Create(topic.FileName))
                using (var writer = new MamlWriter(stream))
                {
                    if (_includeTopicTag)
                    {
                        writer.StartTopic(topic.Id);
                    }
                    else
                    {
                        writer.StartDocument();
                    }
                    writer.WriteIntroductionForSchemaSet(_context);
                    writer.WriteRemarksSectionForSchemaSet(_context);
                    writer.WriteExamplesSectionForSchemaSet(_context);
                    writer.WriteNamespacesSection(_context, 
                        _context.SchemaSetManager.GetNamespaces());
                    if (_includeTopicTag)
                    {
                        writer.EndTopic();
                    }
                    else
                    {
                        writer.EndDocument();
                    }
                }
            }
            else
            {
                var contentFinder = new NamespaceContentFinder(_context.SchemaSetManager, topic.Namespace);
                contentFinder.Traverse(_context.SchemaSetManager.SchemaSet);

                using (var stream = File.Create(topic.FileName))
                using (var writer = new MamlWriter(stream))
                {
                    if (_includeTopicTag)
                    {
                        writer.StartTopic(topic.Id);
                    }
                    else
                    {
                        writer.StartDocument();
                    }
                    writer.WriteIntroductionForSchemaSet(_context);
                    writer.WriteRemarksSectionForSchemaSet(_context);
                    writer.WriteExamplesSectionForSchemaSet(_context);
                    writer.WriteRootSchemasSection(_context, _context.SchemaSetManager.GetNamespaceRootSchemas(topic.Namespace));
                    writer.WriteRootElementsSection(_context, _context.SchemaSetManager.GetNamespaceRootElements(topic.Namespace));
                    writer.WriteSchemasSection(_context, contentFinder.Schemas);
                    writer.WriteElementsSection(_context, contentFinder.Elements);
                    writer.WriteAttributesSection(_context, contentFinder.Attributes);
                    writer.WriteGroupsSection(_context, contentFinder.Groups);
                    writer.WriteAttributeGroupsSection(_context, contentFinder.AttributeGroups);
                    writer.WriteSimpleTypesSection(_context, contentFinder.SimpleTypes);
                    writer.WriteComplexTypesSection(_context, contentFinder.ComplexTypes);
                    if (_includeTopicTag)
                    {
                        writer.EndTopic();
                    }
                    else
                    {
                        writer.EndDocument();
                    }
                }
            }
        }

        private void GenerateNamespaceTopic(Topic topic)
        {
            var contentFinder = new NamespaceContentFinder(_context.SchemaSetManager, topic.Namespace);
            contentFinder.Traverse(_context.SchemaSetManager.SchemaSet);

            using (var stream = File.Create(topic.FileName))
            using (var writer = new MamlWriter(stream))
            {
                if (_includeTopicTag)
                {
                    writer.StartTopic(topic.Id);
                }
                else
                {
                    writer.StartDocument();
                }

                if (_configuration.IncludeMoveToTop)
                {
                    _context.MoveToTopLink = true;
                }

                writer.WriteIntroductionForNamespace(_context, topic.Namespace);
                writer.WriteRemarksSectionForNamespace(_context, topic.Namespace);
                writer.WriteExamplesSectionForNamespace(_context, topic.Namespace);
                writer.WriteRootSchemasSection(_context, _context.SchemaSetManager.GetNamespaceRootSchemas(topic.Namespace));
                writer.WriteRootElementsSection(_context, _context.SchemaSetManager.GetNamespaceRootElements(topic.Namespace));
                writer.WriteSchemasSection(_context, contentFinder.Schemas);
                writer.WriteElementsSection(_context, contentFinder.Elements);
                writer.WriteAttributesSection(_context, contentFinder.Attributes);
                writer.WriteGroupsSection(_context, contentFinder.Groups);
                writer.WriteAttributeGroupsSection(_context, contentFinder.AttributeGroups);
                writer.WriteSimpleTypesSection(_context, contentFinder.SimpleTypes);
                writer.WriteComplexTypesSection(_context, contentFinder.ComplexTypes);

                _context.MoveToTopLink = false;

                if (_includeTopicTag)
                {
                    writer.EndTopic();
                }
                else
                {
                    writer.EndDocument();
                }
            }
        }

        private void GenerateSchemaTopic(Topic topic)
        {
            var schema = (XmlSchema)topic.SchemaObject;

            var contentFinder = new SchemaContentFinder(schema);
            contentFinder.Traverse(schema);

            using (var stream = File.Create(topic.FileName))
            using (var writer = new MamlWriter(stream))
            {
                if (_includeTopicTag)
                {
                    writer.StartTopic(topic.Id);
                }
                else
                {
                    writer.StartDocument();
                }

                if (_configuration.IncludeMoveToTop)
                {
                    _context.MoveToTopLink = true;
                }

                writer.WriteIntroductionForSchema(_context, schema);
                writer.WriteRemarksSectionForObject(_context, schema);
                writer.WriteExamplesSectionForObject(_context, schema);
                writer.WriteElementsSection(_context, contentFinder.Elements);
                writer.WriteAttributesSection(_context, contentFinder.Attributes);
                writer.WriteGroupsSection(_context, contentFinder.Groups);
                writer.WriteAttributeGroupsSection(_context, contentFinder.AttributeGroups);
                writer.WriteSimpleTypesSection(_context, contentFinder.SimpleTypes);
                writer.WriteComplexTypesSection(_context, contentFinder.ComplexTypes);

                _context.MoveToTopLink = false;
                
                if (_includeTopicTag)
                {
                    writer.EndTopic();
                }
                else
                {
                    writer.EndDocument();
                }
            }
        }

        private void GenerateOverviewTopic(Topic topic)
        {
            var contentFinder = new NamespaceContentFinder(_context.SchemaSetManager, topic.Namespace);
            contentFinder.Traverse(_context.SchemaSetManager.SchemaSet);

            using (var stream = File.Create(topic.FileName))
            using (var writer = new MamlWriter(stream))
            {
                if (_includeTopicTag)
                {
                    writer.StartTopic(topic.Id);
                }
                else
                {
                    writer.StartDocument();
                }
                writer.WriteIntroductionForOverview(_context, topic.Namespace);

                switch (topic.TopicType)
                {
                    case TopicType.RootSchemasSection:
                        writer.WriteRootSchemasSection(_context, _context.SchemaSetManager.GetNamespaceRootSchemas(topic.Namespace));
                        break;
                    case TopicType.RootElementsSection:
                        writer.WriteRootElementsSection(_context, _context.SchemaSetManager.GetNamespaceRootElements(topic.Namespace));
                        break;
                    case TopicType.SchemasSection:
                        writer.WriteSchemasSection(_context, contentFinder.Schemas);
                        break;
                    case TopicType.ElementsSection:
                        writer.WriteElementsSection(_context, contentFinder.Elements);
                        break;
                    case TopicType.AttributesSection:
                        writer.WriteAttributesSection(_context, contentFinder.Attributes);
                        break;
                    case TopicType.AttributeGroupsSection:
                        writer.WriteAttributeGroupsSection(_context, contentFinder.AttributeGroups);
                        break;
                    case TopicType.GroupsSection:
                        writer.WriteGroupsSection(_context, contentFinder.Groups);
                        break;
                    case TopicType.SimpleTypesSection:
                        writer.WriteSimpleTypesSection(_context, contentFinder.SimpleTypes);
                        break;
                    case TopicType.ComplexTypesSection:
                        writer.WriteComplexTypesSection(_context, contentFinder.ComplexTypes);
                        break;
                    default:
                        throw ExceptionBuilder.UnhandledCaseLabel(topic.TopicType);
                }

                _context.MoveToTopLink = false;

                if (_includeTopicTag)
                {
                    writer.EndTopic();
                }
                else
                {
                    writer.EndDocument();
                }
            }
        }

        private void GenerateElementTopic(Topic topic)
        {
            var element = (XmlSchemaElement)topic.SchemaObject;
            var parents = _context.SchemaSetManager.GetObjectParents(element);
            var simpleTypeStructureRoot = _context.SchemaSetManager.GetSimpleTypeStructure(element.ElementSchemaType);
            var children = _context.SchemaSetManager.GetChildren(element);
            var attributeEntries = _context.SchemaSetManager.GetAttributeEntries(element);
            var constraints = element.Constraints;

            using (var stream = File.Create(topic.FileName))
            using (var writer = new MamlWriter(stream))
            {
                if (_includeTopicTag)
                {
                    writer.StartTopic(topic.Id);
                }
                else
                {
                    writer.StartDocument();
                }
                writer.WriteIntroductionForObject(_context, element);
                writer.WriteTypeSection(_context, element);
                writer.WriteContentTypeSection(_context, simpleTypeStructureRoot);
                writer.WriteParentsSection(_context, parents);
                writer.WriteChildrenSection(_context, children);
                writer.WriteAttributesSection(_context, attributeEntries);
                writer.WriteConstraintsSection(_context, constraints);
                writer.WriteRemarksSectionForObject(_context, element);
                writer.WriteExamplesSectionForObject(_context, element);
                writer.WriteSyntaxSection(_context, element);
                writer.WriteRelatedTopics(_context, element);

                _context.MoveToTopLink = false;

                if (_includeTopicTag)
                {
                    writer.EndTopic();
                }
                else
                {
                    writer.EndDocument();
                }
            }
        }

        private void GenerateAttributeTopic(Topic topic)
        {
            var attribute = (XmlSchemaAttribute)topic.SchemaObject;
            var parents = _context.SchemaSetManager.GetObjectParents(attribute);
            var simpleTypeStructureRoot = _context.SchemaSetManager.GetSimpleTypeStructure(attribute.AttributeSchemaType);

            using (var stream = File.Create(topic.FileName))
            using (var writer = new MamlWriter(stream))
            {
                if (_includeTopicTag)
                {
                    writer.StartTopic(topic.Id);
                }
                else
                {
                    writer.StartDocument();
                }
                writer.WriteIntroductionForObject(_context, attribute);
                writer.WriteContentTypeSection(_context, simpleTypeStructureRoot);
                writer.WriteParentsSection(_context, parents);
                writer.WriteRemarksSectionForObject(_context, attribute);
                writer.WriteExamplesSectionForObject(_context, attribute);
                writer.WriteSyntaxSection(_context, attribute);
                writer.WriteRelatedTopics(_context, attribute);

                _context.MoveToTopLink = false;

                if (_includeTopicTag)
                {
                    writer.EndTopic();
                }
                else
                {
                    writer.EndDocument();
                }
            }
        }

        private void GenerateGroupTopic(Topic topic)
        {
            var group = (XmlSchemaGroup)topic.SchemaObject;
            var parents = _context.SchemaSetManager.GetObjectParents(group);
            var children = _context.SchemaSetManager.GetChildren(group);

            using (var stream = File.Create(topic.FileName))
            using (var writer = new MamlWriter(stream))
            {
                if (_includeTopicTag)
                {
                    writer.StartTopic(topic.Id);
                }
                else
                {
                    writer.StartDocument();
                }
                writer.WriteIntroductionForObject(_context, group);
                writer.WriteUsagesSection(_context, parents);
                writer.WriteChildrenSection(_context, children);
                writer.WriteRemarksSectionForObject(_context, group);
                writer.WriteExamplesSectionForObject(_context, group);
                writer.WriteSyntaxSection(_context, group);
                writer.WriteRelatedTopics(_context, group);

                _context.MoveToTopLink = false;

                if (_includeTopicTag)
                {
                    writer.EndTopic();
                }
                else
                {
                    writer.EndDocument();
                }
            }
        }

        private void GenerateAttributeGroup(Topic topic)
        {
            var attributeGroup = (XmlSchemaAttributeGroup)topic.SchemaObject;
            var usages = _context.SchemaSetManager.GetObjectParents(attributeGroup);
            var attributeEntries = _context.SchemaSetManager.GetAttributeEntries(attributeGroup);

            using (var stream = File.Create(topic.FileName))
            using (var writer = new MamlWriter(stream))
            {
                if (_includeTopicTag)
                {
                    writer.StartTopic(topic.Id);
                }
                else
                {
                    writer.StartDocument();
                }
                writer.WriteIntroductionForObject(_context, attributeGroup);
                writer.WriteUsagesSection(_context, usages);
                writer.WriteAttributesSection(_context, attributeEntries);
                writer.WriteRemarksSectionForObject(_context, attributeGroup);
                writer.WriteExamplesSectionForObject(_context, attributeGroup);
                writer.WriteSyntaxSection(_context, attributeGroup);
                writer.WriteRelatedTopics(_context, attributeGroup);

                _context.MoveToTopLink = false;

                if (_includeTopicTag)
                {
                    writer.EndTopic();
                }
                else
                {
                    writer.EndDocument();
                }
            }
        }

        private void GenerateSimpleTypeTopic(Topic topic)
        {
            var simpleType = (XmlSchemaSimpleType)topic.SchemaObject;
            var usages = _context.SchemaSetManager.GetTypeUsages(simpleType);
            var simpleTypeStructureRoot = _context.SchemaSetManager.GetSimpleTypeStructure(simpleType.Content);

            using (var stream = File.Create(topic.FileName))
            using (var writer = new MamlWriter(stream))
            {
                if (_includeTopicTag)
                {
                    writer.StartTopic(topic.Id);
                }
                else
                {
                    writer.StartDocument();
                }
                writer.WriteIntroductionForObject(_context, simpleType);
                writer.WriteContentTypeSection(_context, simpleTypeStructureRoot);
                writer.WriteUsagesSection(_context, usages);
                writer.WriteRemarksSectionForObject(_context, simpleType);
                writer.WriteExamplesSectionForObject(_context, simpleType);
                writer.WriteSyntaxSection(_context, simpleType);
                writer.WriteRelatedTopics(_context, simpleType);

                _context.MoveToTopLink = false;

                if (_includeTopicTag)
                {
                    writer.EndTopic();
                }
                else
                {
                    writer.EndDocument();
                }
            }
        }

        private void GenerateComplexTypeTopic(Topic topic)
        {
            var complexType = (XmlSchemaComplexType)topic.SchemaObject;
            var usages = _context.SchemaSetManager.GetTypeUsages(complexType);
            var simpleTypeStructureRoot = _context.SchemaSetManager.GetSimpleTypeStructure(complexType);
            var children = _context.SchemaSetManager.GetChildren(complexType);
            var attributeEntries = _context.SchemaSetManager.GetAttributeEntries(complexType);

            using (var stream = File.Create(topic.FileName))
            using (var writer = new MamlWriter(stream))
            {
                if (_includeTopicTag)
                {
                    writer.StartTopic(topic.Id);
                }
                else
                {
                    writer.StartDocument();
                }
                writer.WriteIntroductionForObject(_context, complexType);
                writer.WriteBaseTypeSection(_context, complexType);
                writer.WriteContentTypeSection(_context, simpleTypeStructureRoot);
                writer.WriteUsagesSection(_context, usages);
                writer.WriteChildrenSection(_context, children);
                writer.WriteAttributesSection(_context, attributeEntries);
                writer.WriteRemarksSectionForObject(_context, complexType);
                writer.WriteExamplesSectionForObject(_context, complexType);
                writer.WriteSyntaxSection(_context, complexType);
                writer.WriteRelatedTopics(_context, complexType);

                _context.MoveToTopLink = false;

                if (_includeTopicTag)
                {
                    writer.EndTopic();
                }
                else
                {
                    writer.EndDocument();
                }
            }
        }

        #endregion
    }
}