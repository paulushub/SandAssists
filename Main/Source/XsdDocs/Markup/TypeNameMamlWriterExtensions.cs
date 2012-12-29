using System;
using System.Xml.Schema;

using XsdDocumentation.Model;

namespace XsdDocumentation.Markup
{
    internal static class TypeNameMamlWriterExtensions
    {
        public static void WriteTypeName(this MamlWriter writer, TopicManager topicManager, 
            XmlSchemaType schemaType)
        {
            if (schemaType == null || schemaType == XmlSchemaType.GetBuiltInComplexType(XmlTypeCode.Item))
                return;

            XmlSchemaSimpleType simpleType;
            XmlSchemaComplexType complexType;

            if (Casting.TryCast(schemaType, out simpleType))
            {
                if (simpleType.QualifiedName.IsEmpty)
                    return;

                writer.WriteImageWithTopicLink(topicManager, ArtItem.SimpleType, simpleType);
                return;
            }

            if (Casting.TryCast(schemaType, out complexType))
            {
                if (!complexType.QualifiedName.IsEmpty)
                {
                    writer.WriteImageWithTopicLink(topicManager, ArtItem.ComplexType, complexType);
                    return;
                }

                var baseType = complexType.BaseXmlSchemaType;
                if (baseType == null || complexType.ContentModel == null)
                    return;

                var isExtension = complexType.ContentModel.Content is XmlSchemaComplexContentExtension ||
                                  complexType.ContentModel.Content is XmlSchemaSimpleContentExtension;

                var artItem = isExtension
                                ? ArtItem.Extension
                                : ArtItem.Restriction;

                writer.WriteImageWithTopicLink(topicManager, artItem, baseType);
                return;
            }

            throw ExceptionBuilder.UnexpectedSchemaObjectType(schemaType);
        }

        private static void WriteImageWithTopicLink(this MamlWriter writer, TopicManager topicManager, ArtItem artItem, XmlSchemaType type)
        {
            var topic = topicManager.GetTopic(type);
            if (topic != null)
            {
                writer.WriteArtItemWithTopicLink(artItem, topic);
            }
            else
            {
                writer.WriteArtItemWithText(artItem, type.QualifiedName.Name);
            }
        }
    }
}