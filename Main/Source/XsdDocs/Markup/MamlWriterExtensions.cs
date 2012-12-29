using System;
using System.Linq;
using System.Xml.Schema;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using XsdDocumentation.Model;

namespace XsdDocumentation.Markup
{
    internal static class MamlWriterExtensions
    {
        private const string NonBlockingSpaceEntityName = "&#160;";
        //private const string XmlTopicType = "3272D745-2FFC-48C4-9E9D-CF2B2B784D5F";

        public static void WriteIndent(this MamlWriter writer, int level)
        {
            if (level <= 0)
            {
                return;
            }

            //writer.StartMarkup();
            writer.WriteStartElement("phrase");
            for (var i = 0; i < level * 6; i++)
                writer.WriteRaw(NonBlockingSpaceEntityName);
            //writer.EndMarkup();
            writer.WriteEndElement();
        }

        //public static void WriteTopicLink(this MamlWriter writer, Topic topic)
        //{
        //    //var url = String.Format(CultureInfo.InvariantCulture, "/html/{0}.htm", topic.Id);
        //    //writer.WriteStartElement("a", Namespaces.Maml);
        //    //writer.WriteAttributeString("href", url);
        //    //writer.WriteString(topic.LinkTitle);
        //    //writer.WriteEndElement();
        //    writer.WriteLink(topic.Id, topic.LinkTitle);
        //}

        public static void StartArtItem(this MamlWriter writer, ArtItem artItem)
        {
            //writer.StartMarkup();
            //writer.WriteStartElement("nobr", String.Empty);
            //writer.WriteStartElement("artLink");
            //writer.WriteAttributeString("target", artItem.Id);
            //writer.WriteEndElement();
            //writer.WriteRaw(NonBlockingSpaceEntityName);

            writer.WriteStartElement("notLocalizable"); //notLocalizable
            writer.WriteAttributeString("address", writer.GetNextNobrAddress());

            // Write the inline-image...
            writer.WriteMediaLinkInline(artItem.Id);
            // Write a white space...
            //writer.WriteStartElement("phrase"); //phrase
            //writer.WriteRaw(NonBlockingSpaceEntityName);
            //writer.WriteEndElement();           //phrase
        }

        public static void StartArtItem(this MamlWriter writer,
            ArtItem artItem, int indentLevel)
        {
            //writer.StartMarkup();
            //writer.WriteStartElement("nobr", String.Empty);
            //writer.WriteStartElement("artLink");
            //writer.WriteAttributeString("target", artItem.Id);
            //writer.WriteEndElement();
            //writer.WriteRaw(NonBlockingSpaceEntityName);

            writer.WriteStartElement("notLocalizable"); //notLocalizable
            writer.WriteAttributeString("address", writer.GetNextNobrAddress());

            if (indentLevel > 0)
            {
                writer.WriteIndent(indentLevel);
            }

            // Write the inline-image...
            writer.WriteMediaLinkInline(artItem.Id);
            // Write a white space...
            //writer.WriteStartElement("phrase"); //phrase
            //writer.WriteRaw(NonBlockingSpaceEntityName);
            //writer.WriteEndElement();           //phrase
        }

        public static void EndArtItem(this MamlWriter writer)
        {
            //writer.WriteEndElement(); // nobr
            //writer.EndMarkup();

            writer.WriteEndElement();                   //notLocalizable
        }

        public static void WriteArtItemWithText(this MamlWriter writer, 
            ArtItem artItem, string text)
        {
            //writer.StartHtmlArtItem(artItem);
            //writer.WriteString(text);
            //writer.EndHtmlArtItem();
            writer.WriteStartElement("notLocalizable"); //notLocalizable
            writer.WriteAttributeString("address", writer.GetNextNobrAddress());

            // Write the inline-image...
            writer.WriteMediaLinkInline(artItem.Id);
            // Write a white space...
            //writer.WriteStartElement("phrase"); //phrase
            //writer.WriteRaw(NonBlockingSpaceEntityName);
            //writer.WriteEndElement();           //phrase
            // Write the line...
            writer.WriteString(text);

            writer.WriteEndElement();                   //notLocalizable
        }

        public static void WriteArtItemWithTopicLink(this MamlWriter writer, 
            ArtItem artItem, Topic topic)
        {
            //writer.StartHtmlArtItem(artItem);
            //writer.WriteHtmlTopicLink(topic);
            //writer.EndHtmlArtItem();
            writer.WriteStartElement("notLocalizable"); //notLocalizable
            writer.WriteAttributeString("address", writer.GetNextNobrAddress());

            // Write the inline-image...
            writer.WriteMediaLinkInline(artItem.Id);
            // Write a white space...
            //writer.WriteStartElement("phrase"); //phrase
            //writer.WriteRaw(NonBlockingSpaceEntityName);
            //writer.WriteEndElement();           //phrase
            // Write the line...
            writer.WriteLink(topic.Id, topic.LinkTitle);

            writer.WriteEndElement();                   //notLocalizable
        }

        public static void WriteArtItemWithTopicLink(this MamlWriter writer, 
            ArtItem artItem, Topic topic, string text)
        {
            //writer.StartHtmlArtItem(artItem);
            //writer.WriteHtmlTopicLink(topic);
            //writer.EndHtmlArtItem();
            writer.WriteStartElement("notLocalizable"); //notLocalizable
            writer.WriteAttributeString("address", writer.GetNextNobrAddress());

            // Write the inline-image...
            writer.WriteMediaLinkInline(artItem.Id);
            // Write a white space...
            //writer.WriteStartElement("phrase"); //phrase
            //writer.WriteRaw(NonBlockingSpaceEntityName);
            //writer.WriteEndElement();           //phrase
            // Write the line...
            writer.WriteLink(topic.Id, text);

            writer.WriteEndElement();                   //notLocalizable
        }

        public static void WriteArtItemWithText(this MamlWriter writer, 
            ArtItem artItem, string text, int indentLevel)
        {
            //writer.StartHtmlArtItem(artItem);
            //writer.WriteString(text);
            //writer.EndHtmlArtItem();
            writer.WriteStartElement("notLocalizable"); //notLocalizable
            writer.WriteAttributeString("address", writer.GetNextNobrAddress());

            if (indentLevel > 0)
            {
                writer.WriteIndent(indentLevel);
            }

            // Write the inline-image...
            writer.WriteMediaLinkInline(artItem.Id);
            // Write a white space...
            //writer.WriteStartElement("phrase"); //phrase
            //writer.WriteRaw(NonBlockingSpaceEntityName);
            //writer.WriteEndElement();           //phrase
            // Write the line...
            writer.WriteString(text);

            writer.WriteEndElement();                   //notLocalizable
        }

        public static void WriteArtItemWithTopicLink(this MamlWriter writer, 
            ArtItem artItem, Topic topic, int indentLevel)
        {
            //writer.StartHtmlArtItem(artItem);
            //writer.WriteHtmlTopicLink(topic);
            //writer.EndHtmlArtItem();
            writer.WriteStartElement("notLocalizable"); //notLocalizable
            writer.WriteAttributeString("address", writer.GetNextNobrAddress());

            if (indentLevel > 0)
            {
                writer.WriteIndent(indentLevel);
            }

            // Write the inline-image...
            writer.WriteMediaLinkInline(artItem.Id);
            // Write a white space...
            //writer.WriteStartElement("phrase"); //phrase
            //writer.WriteRaw(NonBlockingSpaceEntityName);
            //writer.WriteEndElement();           //phrase
            // Write the line...
            writer.WriteLink(topic.Id, topic.LinkTitle);

            writer.WriteEndElement();                   //notLocalizable
        }

        //private static void WriteNamespaceLink(this MamlWriter writer, Context context, string namespaceUri)
        //{
        //    var topic = context.TopicManager.GetNamespaceTopic(namespaceUri);
        //    if (topic == null)
        //        writer.WriteString(namespaceUri ?? "Empty");
        //    else
        //        writer.WriteLink(topic.Id, topic.LinkTitle);
        //        //writer.WriteHtmlTopicLink(topic);
        //}

        private static void WriteSchemaLink(this MamlWriter writer, Context context, XmlSchemaObject obj)
        {
            var topic = context.TopicManager.GetTopic(obj.GetSchema());
            if (topic == null)
                writer.WriteString(obj.GetSchemaName());
            else
                writer.WriteLink(topic.Id, topic.LinkTitle);
                //writer.WriteHtmlTopicLink(topic);
        }

        public static void WriteArtItem(this MamlWriter writer, ArtItem artItem)
        {
            writer.WriteMediaLink(artItem.Id);
        }

        public static void WriteArtItemInline(this MamlWriter writer, ArtItem artItem)
        {
            writer.WriteMediaLinkInline(artItem.Id);
        }

        public static void WriteTopicLink(this MamlWriter writer, Topic topic)
        {
            writer.WriteLink(topic.Id, topic.LinkTitle);
        }

        //private static void WriteTopicLinkWithReferenceMarker(this MamlWriter writer, Topic rootItemTopic)
        //{
        //    writer.WriteLink(rootItemTopic.Id, rootItemTopic.LinkTitle, XmlTopicType);
        //}

        public static void WriteNamespaceLink(this MamlWriter writer, Context context, string namespaceUri)
        {
            var topic = context.TopicManager.GetNamespaceTopic(namespaceUri);
            if (topic == null)
                writer.WriteString(namespaceUri ?? "Empty");
            else
                writer.WriteTopicLink(topic);
        }

        public static void WriteSummary(this MamlWriter writer, DocumentationInfo documentationInfo)
        {
            if (documentationInfo != null && documentationInfo.SummaryNode != null)
                writer.WriteRawContent(documentationInfo.SummaryNode);
        }

        public static void WriteSummaryForObject(this MamlWriter writer, Context context, XmlSchemaObject obj)
        {
            var documentationInfo = context.DocumentationManager.GetObjectDocumentationInfo(obj);
            writer.WriteSummary(documentationInfo);
        }

        public static void WriteNamespaceInfo(this MamlWriter writer, 
            Context context, string namespaceUri)
        {
            writer.StartParagraph();
            //writer.StartMarkup();
            //writer.WriteRaw("<b>Namespace:</b> ");
            writer.WriteBold("Namespace: ", true);
            writer.WriteNamespaceLink(context, namespaceUri);
            //writer.WriteRaw("<br/>");
            //writer.EndMarkup();
            writer.EndParagraph();
        }

        public static void WriteNamespaceAndSchemaInfo(this MamlWriter writer, Context context, XmlSchemaObject obj)
        {
            var targetNamespace = obj.GetSchema().TargetNamespace;

            //writer.StartParagraph();
            //writer.StartMarkup();
            //writer.WriteRaw("<b>Namespace:</b> ");
            //writer.WriteBold("Namespace:", true);
            //writer.WriteHtmlNamespaceLink(context, targetNamespace);
            //writer.WriteRaw("<br/>");
            //writer.WriteRaw("<b>Schema:</b> ");
            //writer.WriteHtmlSchemaLink(context, obj);
            //writer.WriteRaw("<br/>");
            //writer.EndMarkup();
            //writer.EndParagraph();

            writer.StartParagraph();
            writer.WriteBold("Namespace: ", true);
            writer.WriteNamespaceLink(context, targetNamespace);
            //writer.EndParagraph();
            //writer.WriteRaw(NonBlockingSpaceEntityName);
            writer.WriteToken("lineBreak");
            //writer.WriteRaw(NonBlockingSpaceEntityName);
            //writer.StartParagraph();
            writer.WriteBold("Schema: ", true);
            writer.WriteSchemaLink(context, obj);
            writer.EndParagraph();
        }

        public static void WriteLinksForObject(this MamlWriter writer, 
            XmlSchemaObject obj, Context context)
        {
            var root = obj.GetRoot();
            if (root != null && root != obj)
            {
                var rootItemTopic = context.TopicManager.GetTopic(root);
                if (rootItemTopic != null)
                    writer.WriteTopicLink(rootItemTopic);
            }

            var targetNamespace = obj.GetSchema().TargetNamespace;
            var targetNamespaceTopic = context.TopicManager.GetNamespaceTopic(targetNamespace);
            if (targetNamespaceTopic != null)
                writer.WriteTopicLink(targetNamespaceTopic);

            var info = context.DocumentationManager.GetObjectDocumentationInfo(obj);
            if (info != null && info.RelatedTopicsNode != null)
                writer.WriteRawContent(info.RelatedTopicsNode);
        }

        public static void WriteList(this MamlWriter writer, 
            Context context, IList<ListItem> listItems)
        {
            int itemCount = listItems.Count;
            if (itemCount > 1)
            {
                writer.WriteStartElement("phrase"); //phrase
                writer.WriteString("|");
                writer.WriteEndElement();           //phrase
            }    

            if (itemCount == 1)
            {
                ListItem item = listItems[0];
                writer.WriteArtItemWithTopicLink(item.ArtItem, item.Topic);
            }
            else if (itemCount == 2)
            {
                ListItem item1 = listItems[0];
                ListItem item2 = listItems[1];
                Topic topic1 = item1.Topic;
                Topic topic2 = item2.Topic;

                if (String.Equals(topic1.LinkTitle, topic2.LinkTitle, 
                    StringComparison.Ordinal))
                {
                    writer.WriteQualifiedListItem(context, item1, topic1);

                    writer.WriteStartElement("phrase"); //phrase
                    writer.WriteString(" |");
                    writer.WriteEndElement();           //phrase

                    writer.WriteQualifiedListItem(context, item2, topic2);
                }
                else
                {
                    writer.WriteArtItemWithTopicLink(item1.ArtItem, topic1);
                   
                    writer.WriteStartElement("phrase"); //phrase
                    writer.WriteString(" |");
                    writer.WriteEndElement();           //phrase

                    writer.WriteArtItemWithTopicLink(item2.ArtItem, topic2);
                }                  
            }
            else
            {
                MultiMap<string, ListItem> listMap = new MultiMap<string, ListItem>();

                for (int i = 0; i < itemCount; i++)
                {
                    ListItem item = listItems[i];

                    listMap.Add(item.Topic.LinkTitle, item);
                }

                if (listMap.IsMultiValue)
                {
                    writer.WriteQualifiedList(context, listMap);
                }
                else
                {   
                    for (int i = 0; i < itemCount; i++)
                    {
                        ListItem item = listItems[i];
                        // Write a separator...
                        if (i > 0)
                        {   
                            writer.WriteStartElement("phrase"); //phrase
                            writer.WriteString(" |");
                            writer.WriteEndElement();           //phrase
                        }

                        writer.WriteArtItemWithTopicLink(item.ArtItem, item.Topic);
                    }
                }
            }
            if (itemCount > 1)
            {
                writer.WriteStartElement("phrase"); //phrase
                writer.WriteString(" |");
                writer.WriteEndElement();           //phrase
            }
        }

        private static void WriteQualifiedList(this MamlWriter writer,
            Context context, MultiMap<string, ListItem> listMap)
        {
            int index = 0;

            IEnumerable<string> keys = listMap.Keys;
            foreach (string key in keys)
            {
                IList<ListItem> qualifiedItems = listMap[key];

                if (qualifiedItems.Count == 1)
                {
                    ListItem item = qualifiedItems[0];
                    // Write a separator...
                    if (index > 0)
                    {
                        writer.WriteStartElement("phrase"); //phrase
                        writer.WriteString(" |");
                        writer.WriteEndElement();           //phrase
                    }

                    writer.WriteArtItemWithTopicLink(item.ArtItem, item.Topic);

                    index++;
                }
                else
                {
                    MultiMap<string, ListItem> listMapNext = new MultiMap<string, ListItem>();

                    for (int i = 0; i < qualifiedItems.Count; i++)
                    {
                        ListItem item = qualifiedItems[i];

                        listMapNext.Add(GetQualifiedListItem(
                            context, item, item.Topic), item);
                    } 

                    IEnumerable<string> keysNext = listMapNext.Keys;
                    foreach (string keyNext in keysNext)
                    {
                        IList<ListItem> qualifiedItemsNext = listMapNext[keyNext];

                        if (qualifiedItemsNext.Count == 1)
                        {
                            ListItem itemNext = qualifiedItemsNext[0];
                            // Write a separator...
                            if (index > 0)
                            {
                                writer.WriteStartElement("phrase"); //phrase
                                writer.WriteString(" |");
                                writer.WriteEndElement();           //phrase
                            }

                            writer.WriteQualifiedListItem(context, itemNext, itemNext.Topic);

                            index++;
                        }
                        else
                        {
                            for (int i = 0; i < qualifiedItemsNext.Count; i++)
                            {
                                ListItem itemNext = qualifiedItemsNext[i];
                                // Write a separator...
                                if (index > 0)
                                {
                                    writer.WriteStartElement("phrase"); //phrase
                                    writer.WriteString(" |");
                                    writer.WriteEndElement();           //phrase
                                }

                                writer.WriteQualifiedListItem(context, itemNext,
                                    itemNext.Topic, keyNext);

                                index++;
                            }
                        }
                    }
         
                    //for (int i = 0; i < qualifiedItems.Count; i++)
                    //{
                    //    ListItem item = qualifiedItems[i];
                    //    // Write a separator...
                    //    if (index > 0)
                    //    {
                    //        writer.WriteStartElement("phrase"); //phrase
                    //        writer.WriteString(" |");
                    //        writer.WriteEndElement();           //phrase
                    //    }

                    //    //writer.WriteArtItemWithTopicLink(item.ArtItem, item.Topic);
                    //    writer.WriteQualifiedListItem(context, item, item.Topic);

                    //    index++;
                    //}
                }
            }
        }

        private static void WriteQualifiedListItem(this MamlWriter writer,
            Context context, ListItem item, Topic topic)
        {
            string parentName = GetSchemaObjectName(context, topic.SchemaObject);

            if (String.IsNullOrEmpty(parentName))
            {
                writer.WriteArtItemWithTopicLink(item.ArtItem, topic);
            }
            else
            {
                writer.WriteArtItemWithTopicLink(item.ArtItem, topic,
                    parentName + "\\" + topic.LinkTitle);
            }
        }

        private static void WriteQualifiedListItem(this MamlWriter writer,
            Context context, ListItem item, Topic topic, string text)
        {
            string parentName = GetSchemaObjectName(context, 
                topic.SchemaObject.Parent);

            if (String.IsNullOrEmpty(parentName))
            {
                writer.WriteArtItemWithTopicLink(item.ArtItem, topic, text);
            }
            else
            {
                writer.WriteArtItemWithTopicLink(item.ArtItem, topic,
                    parentName + "\\" + text);
            }
        }

        private static string GetQualifiedListItem(Context context, ListItem item, Topic topic)
        {
            string parentName = GetSchemaObjectName(context, topic.SchemaObject);

            if (String.IsNullOrEmpty(parentName))
            {
                return topic.LinkTitle;
            }
            else
            {
                return parentName + "\\" + topic.LinkTitle;
            }
        }

        private static string GetSchemaObjectName(Context context, 
            XmlSchemaObject schemaObject)
        {
            XmlSchema schema;
            XmlSchemaAttribute attribute;
            XmlSchemaElement element;
            XmlSchemaGroup group;
            XmlSchemaAttributeGroup attributeGroup;
            XmlSchemaSimpleType simpleType;
            XmlSchemaComplexType complexType;
            XmlSchemaGroupBase groupBase;
            XmlSchemaGroupRef groupRef;

            XmlSchemaContent content;

            if (Casting.TryCast(schemaObject, out attribute))
            {
                var parents = context.SchemaSetManager.GetObjectParents(attribute);

                IList<ListItem> listItems = ListItemBuilder.Build(context, parents);
                if (listItems != null && listItems.Count == 1)
                {
                    ListItem parentItem = listItems[0];

                    return parentItem.Topic.LinkTitle;
                }

                //var simpleTypeStructureRoot = 
                //    context.SchemaSetManager.GetSimpleTypeStructure(
                //    attribute.AttributeSchemaType);
                //if (simpleTypeStructureRoot != null)
                //{
                //    if (simpleTypeStructureRoot.Children.Count == 1)
                //    {
                //        var node = simpleTypeStructureRoot.Children[0];
                //        var isSingleRow = SimpleTypeStructureNode.GetIsSingleRow(node);
                //        if (isSingleRow && node.NodeType == SimpleTypeStructureNodeType.NamedType)
                //        {
                //            XmlSchemaType schemaType = (XmlSchemaType)node.Node;

                //            return GetTypeName(context.TopicManager, schemaType);
                //        }
                //    }   
                //}

                return attribute.AttributeSchemaType.Name;
            }
            
            if (Casting.TryCast(schemaObject, out element))
            {
                var parents = context.SchemaSetManager.GetObjectParents(element);

                IList<ListItem> listItems = ListItemBuilder.Build(context, parents);
                if (listItems != null && listItems.Count == 1)
                {
                    ListItem parentItem = listItems[0];

                    return parentItem.Topic.LinkTitle;
                }

                //var simpleTypeStructureRoot =
                //    context.SchemaSetManager.GetSimpleTypeStructure(
                //    element.ElementSchemaType);

                //if (simpleTypeStructureRoot != null)
                //{
                //    if (simpleTypeStructureRoot.Children.Count == 1)
                //    {
                //        var node = simpleTypeStructureRoot.Children[0];
                //        var isSingleRow = SimpleTypeStructureNode.GetIsSingleRow(node);
                //        if (isSingleRow && node.NodeType == SimpleTypeStructureNodeType.NamedType)
                //        {
                //            XmlSchemaType schemaType = (XmlSchemaType)node.Node;

                //            return GetTypeName(context.TopicManager, schemaType);
                //        }
                //    }
                //}

                return element.ElementSchemaType.Name;
            }

            if (Casting.TryCast(schemaObject, out schema))
            {
                return String.Empty;
            }

            if (Casting.TryCast(schemaObject, out group))
            {
                return group.QualifiedName.Name;
            }
            
            if (Casting.TryCast(schemaObject, out attributeGroup))
            {
                return attributeGroup.QualifiedName.Name;
            }
            if (Casting.TryCast(schemaObject, out simpleType))
            {
                return simpleType.QualifiedName.Name;
            }
            
            if (Casting.TryCast(schemaObject, out complexType))
            {
                return complexType.QualifiedName.Name;
            }

            if (Casting.TryCast(schemaObject, out groupBase))
            {
                return GetSchemaObjectName(context, groupBase.Parent);
            }

            if (Casting.TryCast(schemaObject, out groupRef))
            {
                group = context.SchemaSetManager.SchemaSet.ResolveGroup(groupRef);

                if (group != null)
                {
                    return group.QualifiedName.Name;
                }
            }

            if (Casting.TryCast(schemaObject, out content))
            {
                XmlSchemaSimpleContentExtension simpleContentExtension;
                XmlSchemaSimpleContentRestriction simpleContentRestriction;
                XmlSchemaComplexContentExtension complexContentExtension;
                XmlSchemaComplexContentRestriction complexContentRestriction;

                if (Casting.TryCast(content, out simpleContentExtension))
                {
                    return simpleContentExtension.BaseTypeName.Name;
                }
                else if (Casting.TryCast(content, out simpleContentRestriction))
                {
                    return simpleContentRestriction.BaseTypeName.Name;
                }
                else if (Casting.TryCast(content, out complexContentExtension))
                {
                    return complexContentExtension.BaseTypeName.Name;
                }
                else if (Casting.TryCast(content, out complexContentRestriction))
                {
                    return complexContentRestriction.BaseTypeName.Name;
                }
            }

            //if (schemaObject.Parent != null)
            //{
            //    return GetSchemaObjectName(context, schemaObject.Parent);
            //}

            throw ExceptionBuilder.UnexpectedSchemaObjectType(schemaObject);
        }

        public static string GetTypeName(TopicManager topicManager,
            XmlSchemaType schemaType)
        {
            if (schemaType == null || schemaType == XmlSchemaType.GetBuiltInComplexType(XmlTypeCode.Item))
                return String.Empty;

            XmlSchemaSimpleType simpleType;
            XmlSchemaComplexType complexType;

            if (Casting.TryCast(schemaType, out simpleType))
            {
                if (simpleType.QualifiedName.IsEmpty)
                    return String.Empty;

                return GetTopicLinkTitle(topicManager, ArtItem.SimpleType, simpleType);
            }

            if (Casting.TryCast(schemaType, out complexType))
            {
                if (!complexType.QualifiedName.IsEmpty)
                {
                    return GetTopicLinkTitle(topicManager, 
                        ArtItem.ComplexType, complexType);
                }

                var baseType = complexType.BaseXmlSchemaType;
                if (baseType == null || complexType.ContentModel == null)
                    return String.Empty;

                var isExtension = complexType.ContentModel.Content is XmlSchemaComplexContentExtension ||
                                  complexType.ContentModel.Content is XmlSchemaSimpleContentExtension;

                var artItem = isExtension
                                ? ArtItem.Extension
                                : ArtItem.Restriction;

                return GetTopicLinkTitle(topicManager, artItem, baseType);
            }

            throw ExceptionBuilder.UnexpectedSchemaObjectType(schemaType);
        }

        private static string GetTopicLinkTitle(TopicManager topicManager, 
            ArtItem artItem, XmlSchemaType type)
        {
            var topic = topicManager.GetTopic(type);
            if (topic != null)
            {
                return topic.LinkTitle;
            }
            else
            {
                return type.QualifiedName.Name;
            }
        }

        //public static void WriteList(this MamlWriter writer, Context context, IEnumerable<XmlSchemaObject> schemaObjects)
        //{
        //    IList<ListItem> listItems = ListItemBuilder.Build(context, schemaObjects);

        //    int itemCount = listItems.Count;
        //    for (int i = 0; i < itemCount; i++)
        //    {
        //        ListItem item = listItems[i];
        //        //if (i == 0)
        //        //{
        //        //    writer.WriteString(" ");
        //        //}

        //        // Write a separator...
        //        if (i > 0)
        //        {   
        //            writer.WriteStartElement("phrase"); //phrase
        //            writer.WriteString(" |");
        //            writer.WriteEndElement();           //phrase
        //        }

        //        writer.WriteHtmlArtItemWithTopicLink(item.ArtItem, item.Topic);
        //    }
        //}
    }
}