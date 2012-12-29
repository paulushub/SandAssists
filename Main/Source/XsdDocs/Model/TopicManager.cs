using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace XsdDocumentation.Model
{
    public sealed class TopicManager : Manager
    {
        private Dictionary<string, Topic>          _topicUriIndex;
        private Dictionary<string, Topic>          _namespaceTopics;
        private Dictionary<XmlSchemaObject, Topic> _topicDictionary;

        public TopicManager(Context context)
            : base(context)
        {
            _namespaceTopics = new Dictionary<string, Topic>();
            _topicDictionary = new Dictionary<XmlSchemaObject, Topic>();
            _topicUriIndex   = new Dictionary<string, Topic>();
        }

        public override void Initialize()
        {
            var topicBuilder = new TopicBuilder(Context.SchemaSetManager, _namespaceTopics, _topicDictionary);
            topicBuilder.Traverse(Context.SchemaSetManager.SchemaSet);
            Topics = topicBuilder.GetRootTopics();

            RemoveNamespaceContainerIfConfigured();
            RemoveSchemasIfConfigured();
            SortNamespaces();
            InsertNamespaceOverviewTopics();
            InsertNamespaceRootTopics();
            InsertSchemaSetTopic();
            SetTopicIds();
            SetKeywords(Topics);
            GenerateTopicUriIndex(Topics);
        }

        public Topic GetNamespaceTopic(string targetNamespace)
        {
            Topic result;
            _namespaceTopics.TryGetValue(targetNamespace ?? String.Empty, out result);
            return result;
        }

        public Topic GetTopicByUri(string uri)
        {
            Topic result;
            _topicUriIndex.TryGetValue(uri, out result);
            return result;
        }

        public Topic GetTopic(XmlSchemaObject obj)
        {
            XmlSchemaElement element;
            XmlSchemaAttribute attribute;

            if (Casting.TryCast(obj, out element))
            {
                if (!element.RefName.IsEmpty)
                    obj = Context.SchemaSetManager.SchemaSet.GlobalElements[element.RefName];
            }
            else if (Casting.TryCast(obj, out attribute))
            {
                if (!attribute.RefName.IsEmpty)
                    obj = Context.SchemaSetManager.SchemaSet.GlobalAttributes[attribute.RefName];
            }

            Topic result;
            _topicDictionary.TryGetValue(obj, out result);
            return result;
        }

        public List<Topic> Topics { get; private set; }

        private void RemoveNamespaceContainerIfConfigured()
        {
            var namespaceTopicCount = Topics.Count;
            var shouldRemoveNamespaceTopic = !Context.Configuration.NamespaceContainer;
            var canRemoveNamespaceTopic = (namespaceTopicCount == 1);
            var removeNamespaceTopic = shouldRemoveNamespaceTopic && canRemoveNamespaceTopic;

            if (removeNamespaceTopic)
            {
                var singleNamespaceTopic = Topics[0];
                Topics.Clear();
                Topics.AddRange(singleNamespaceTopic.Children);
                _namespaceTopics.Remove(singleNamespaceTopic.Namespace ?? String.Empty);
            }

            Context.Configuration.NamespaceContainer = !removeNamespaceTopic;
        }

        private void RemoveSchemasIfConfigured()
        {
            if (Context.Configuration.DocumentSchemas)
                return;

            if (!Context.Configuration.NamespaceContainer)
            {
                RemoveSchemas(Topics);
            }
            else
            {
                foreach (var namespaceTopic in Topics)
                    RemoveSchemas(namespaceTopic.Children);
            }
        }

        private void RemoveSchemas(IList<Topic> topics)
        {
            for (var i = topics.Count - 1; i >= 0; i--)
            {
                var topic = topics[i];
                if (topic.TopicType != TopicType.Schema)
                    continue;

                topics.RemoveAt(i);
                _topicDictionary.Remove(topic.SchemaObject);
            }
        }

        private void SortNamespaces()
        {
            if (Context.Configuration.NamespaceContainer)
                Topics.Sort((x, y) => x.LinkTitle.CompareTo(y.LinkTitle));
        }

        private void InsertNamespaceOverviewTopics()
        {
            if (Context.Configuration.NamespaceContainer)
            {
                foreach (var namespaceTopic in Topics)
                    CreateNamespaceOverviewTopics(namespaceTopic.Namespace, namespaceTopic.Children);
            }
            else if (Topics.Count > 0)
            {
                var firstEntry = Topics[0];
                CreateNamespaceOverviewTopics(firstEntry.Namespace, Topics);
            }
        }

        private static void CreateNamespaceOverviewTopics(string namespaceUri, List<Topic> namespaceChildTopics)
        {
            var groupedChildren = from child in namespaceChildTopics
                                  group child by child.TopicType
                                      into g
                                      orderby GetTopicTypeSortOrderKey(g.Key)
                                      select g;

            var overviewTopics = new List<Topic>();

            foreach (var grouping in groupedChildren)
            {
                var overviewTopicType = GetOverviewTopicType(grouping.Key);
                var overviewLinkUri = GetOverviewTopicLinkUri(namespaceUri, overviewTopicType);
                var overviewTopicTitle = GetOverviewTopicTitle(overviewTopicType);
                var overviewTopic = new Topic
                                    {
                                        Title = overviewTopicTitle,
                                        LinkTitle = overviewTopicTitle,
                                        LinkUri = overviewLinkUri,
                                        TopicType = overviewTopicType,
                                        Namespace = namespaceUri
                                    };

                var sortedSubTopics = from subTopic in grouping
                                      orderby subTopic.Title
                                      select subTopic;
                overviewTopic.Children.AddRange(sortedSubTopics);
                overviewTopics.Add(overviewTopic);
            }

            namespaceChildTopics.Clear();
            namespaceChildTopics.AddRange(overviewTopics);
        }

        private void InsertNamespaceRootTopics()
        {
            if (Context.Configuration.NamespaceContainer)
            {
                foreach (var namespaceTopic in Topics)
                    InsertNamespaceRootTopics(namespaceTopic.Namespace, namespaceTopic.Children);
            }
            else if (Topics.Count > 0)
            {
                var firstEntry = Topics[0];
                InsertNamespaceRootTopics(firstEntry.Namespace, Topics);
            }
        }

        private void InsertNamespaceRootTopics(string namespaceUri, IList<Topic> namespaceChildren)
        {
            if (Context.Configuration.DocumentRootElements)
            {
                var rootElements = Context.SchemaSetManager.GetNamespaceRootElements(namespaceUri);
                InsertNamespaceRootTopic(namespaceUri, namespaceChildren, rootElements, TopicType.RootElementsSection);
            }

            if (Context.Configuration.DocumentRootSchemas && Context.Configuration.DocumentSchemas)
            {
                var rootSchemas = Context.SchemaSetManager.GetNamespaceRootSchemas(namespaceUri);
                InsertNamespaceRootTopic(namespaceUri, namespaceChildren, rootSchemas, TopicType.RootSchemasSection);
            }
        }

        private static void InsertNamespaceRootTopic(string namespaceUri, IList<Topic> namespaceChildren, IEnumerable<XmlSchemaObject> rootItems, TopicType topicType)
        {
            var rootElements = rootItems.ToList();
            if (rootElements.Count == 0)
                return;

            var overviewTopicTitle = GetOverviewTopicTitle(topicType);
            var rootElementsTopic = new Topic
                                    {
                                        Title = overviewTopicTitle,
                                        LinkTitle = overviewTopicTitle,
                                        LinkUri = GetOverviewTopicLinkUri(namespaceUri, topicType),
                                        TopicType = topicType,
                                        Namespace = namespaceUri
                                    };
            namespaceChildren.Insert(0, rootElementsTopic);
        }

        private void InsertSchemaSetTopic()
        {
            if (!Context.Configuration.SchemaSetContainer)
                return;

            var title = String.IsNullOrEmpty(Context.Configuration.SchemaSetTitle)
                            ? "Schema Set"
                            : Context.Configuration.SchemaSetTitle;

            var schemaSetTopic = new Topic
                                 {
                                     Title = title,
                                     LinkTitle = title,
                                     LinkUri = "##SchemaSet",
                                     TopicType = TopicType.SchemaSet,
                                 };

            schemaSetTopic.Children.AddRange(Topics);
            Topics.Clear();
            Topics.Add(schemaSetTopic);
        }

        private static int GetTopicTypeSortOrderKey(TopicType topicType)
        {
            switch (topicType)
            {
                case TopicType.Schema:
                    return 0;
                case TopicType.Element:
                    return 1;
                case TopicType.Attribute:
                    return 2;
                case TopicType.AttributeGroup:
                    return 3;
                case TopicType.Group:
                    return 4;
                case TopicType.SimpleType:
                    return 5;
                case TopicType.ComplexType:
                    return 6;
                default:
                    throw ExceptionBuilder.UnhandledCaseLabel(topicType);
            }
        }

        private static TopicType GetOverviewTopicType(TopicType topicType)
        {
            switch (topicType)
            {
                case TopicType.Schema:
                    return TopicType.SchemasSection;
                case TopicType.Element:
                    return TopicType.ElementsSection;
                case TopicType.Attribute:
                    return TopicType.AttributesSection;
                case TopicType.AttributeGroup:
                    return TopicType.AttributeGroupsSection;
                case TopicType.Group:
                    return TopicType.GroupsSection;
                case TopicType.SimpleType:
                    return TopicType.SimpleTypesSection;
                case TopicType.ComplexType:
                    return TopicType.ComplexTypesSection;
                default:
                    throw ExceptionBuilder.UnhandledCaseLabel(topicType);
            }
        }

        private static string GetOverviewTopicLinkUri(string namespaceUri, TopicType type)
        {
            switch (type)
            {
                case TopicType.RootSchemasSection:
                    return String.Format(CultureInfo.InvariantCulture, "{0}##RootSchemas", namespaceUri);
                case TopicType.RootElementsSection:
                    return String.Format(CultureInfo.InvariantCulture, "{0}##RootElements", namespaceUri);
                case TopicType.SchemasSection:
                    return String.Format(CultureInfo.InvariantCulture, "{0}##Schemas", namespaceUri);
                case TopicType.ElementsSection:
                    return String.Format(CultureInfo.InvariantCulture, "{0}##Elements", namespaceUri);
                case TopicType.AttributesSection:
                    return String.Format(CultureInfo.InvariantCulture, "{0}##Attributes", namespaceUri);
                case TopicType.AttributeGroupsSection:
                    return String.Format(CultureInfo.InvariantCulture, "{0}##AttributeGroups", namespaceUri);
                case TopicType.GroupsSection:
                    return String.Format(CultureInfo.InvariantCulture, "{0}##Groups", namespaceUri);
                case TopicType.SimpleTypesSection:
                    return String.Format(CultureInfo.InvariantCulture, "{0}##SimpleTypes", namespaceUri);
                case TopicType.ComplexTypesSection:
                    return String.Format(CultureInfo.InvariantCulture, "{0}##ComplexTypes", namespaceUri);
                default:
                    throw ExceptionBuilder.UnhandledCaseLabel(type);
            }
        }

        private static string GetOverviewTopicTitle(TopicType topicType)
        {
            switch (topicType)
            {
                case TopicType.RootSchemasSection:
                    return "Root Schemas";
                case TopicType.RootElementsSection:
                    return "Root Elements";
                case TopicType.SchemasSection:
                    return "Schemas";
                case TopicType.ElementsSection:
                    return "Elements";
                case TopicType.AttributesSection:
                    return "Attributes";
                case TopicType.AttributeGroupsSection:
                    return "Attribute Groups";
                case TopicType.GroupsSection:
                    return "Groups";
                case TopicType.SimpleTypesSection:
                    return "Simple Types";
                case TopicType.ComplexTypesSection:
                    return "Complex Types";
                default:
                    throw ExceptionBuilder.UnhandledCaseLabel(topicType);
            }
        }

        private void SetTopicIds()
        {
            var guidsInUse = new HashSet<Guid>();
            using (var md5 = HashAlgorithm.Create("MD5"))
                SetTopicIds(Topics, md5, guidsInUse);
        }

        private static void SetTopicIds(IEnumerable<Topic> topics, HashAlgorithm algorithm, HashSet<Guid> guidsInUse)
        {
            foreach (var topic in topics)
            {
                var input = Encoding.UTF8.GetBytes(topic.LinkUri);
                var output = algorithm.ComputeHash(input);
                var guid = new Guid(output);
                while (!guidsInUse.Add(guid))
                    guid = Guid.NewGuid();
                topic.Id = guid.ToString();

                SetTopicIds(topic.Children, algorithm, guidsInUse);
            }
        }

        private void SetKeywords(IEnumerable<Topic> topics)
        {
            foreach (var topic in topics)
            {
                switch (topic.TopicType)
                {
                    case TopicType.Namespace:
                        AddKeywordK(topic, topic.Title);
                        AddKeywordF(topic, topic.Namespace);
                        break;
                    case TopicType.Schema:
                        AddKeywordK(topic, topic.Title);
                        break;
                    case TopicType.Element:
                        {
                            AddKeywordK(topic, topic.Title);
                            var element = (XmlSchemaElement)topic.SchemaObject;
                            AddKeywordF(topic, element.QualifiedName);
                            break;
                        }
                    case TopicType.Attribute:
                        {
                            AddKeywordK(topic, topic.Title);
                            var attribute = (XmlSchemaAttribute)topic.SchemaObject;
                            AddKeywordF(topic, attribute.QualifiedName);
                            break;
                        }
                    case TopicType.AttributeGroup:
                        AddKeywordK(topic, topic.Title);
                        break;
                    case TopicType.Group:
                        AddKeywordK(topic, topic.Title);
                        break;
                    case TopicType.SimpleType:
                        AddKeywordK(topic, topic.Title);
                        break;
                    case TopicType.ComplexType:
                        AddKeywordK(topic, topic.Title);
                        break;
                }

                if (Context.Configuration.IncludeLinkUriInKeywordK)
                    AddKeywordK(topic, topic.LinkUri);

                SetKeywords(topic.Children);
            }
        }

        private static void AddKeywordK(Topic topic, string keyword)
        {
            topic.KeywordsK.Add(keyword);
        }

        private static void AddKeywordF(Topic topic, string keyword)
        {
            topic.KeywordsF.Add(keyword);
        }

        private static void AddKeywordF(Topic topic, XmlQualifiedName qualifiedName)
        {
            if (String.IsNullOrEmpty(qualifiedName.Namespace))
                return;

            var keyword = String.Format("{0}#{1}", qualifiedName.Namespace, qualifiedName.Name);
            AddKeywordF(topic, keyword);
        }

        private void GenerateTopicUriIndex(IEnumerable<Topic> topics)
        {
            foreach (var topic in topics)
            {
                AddTopicUriToIndex(topic, topic.LinkUri);
                AddTopicUriToIndex(topic, topic.LinkIdUri);

                GenerateTopicUriIndex(topic.Children);
            }
        }

        private void AddTopicUriToIndex(Topic topic, string uri)
        {
            if (String.IsNullOrEmpty(uri))
                return;

            if (!_topicUriIndex.ContainsKey(uri))
                _topicUriIndex.Add(uri, topic);
        }
    }
}