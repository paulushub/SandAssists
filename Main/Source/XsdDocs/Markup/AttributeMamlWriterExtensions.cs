using System;
using System.Linq;
using System.Xml.Schema;

using XsdDocumentation.Model;

namespace XsdDocumentation.Markup
{
    internal static class AttributeMamlWriterExtensions
    {
        #region AttributeTypeNameWriter

        private sealed class AttributeTypeNameWriter : XmlSchemaSetVisitor
        {
            private Context _context;
            private MamlWriter _writer;

            public AttributeTypeNameWriter(MamlWriter writer, Context context)
            {
                _writer = writer;
                _context = context;
            }

            protected override void Visit(XmlSchemaSimpleType type)
            {
                if (type.QualifiedName.IsEmpty)
                    Traverse(type.Content);
                else
                {
                    var topic = _context.TopicManager.GetTopic(type);
                    if (topic != null)
                    {
                        _writer.WriteArtItemWithTopicLink(ArtItem.SimpleType, topic);
                    }
                    else
                    {
                        _writer.WriteArtItemWithText(ArtItem.SimpleType, type.QualifiedName.Name);
                    }
                }
            }

            protected override void Visit(XmlSchemaSimpleTypeList list)
            {
                if (list.BaseItemType.QualifiedName.IsEmpty)
                {
                    _writer.WriteArtItemWithText(ArtItem.List, "List");
                }
                else
                {
                    var topic = _context.TopicManager.GetTopic(list.BaseItemType);
                    if (topic != null)
                    {
                        _writer.WriteArtItemWithTopicLink(ArtItem.List, topic);
                    }
                    else
                    {
                        _writer.WriteArtItemWithText(ArtItem.List, list.BaseItemType.QualifiedName.Name);
                    }
                }
            }

            protected override void Visit(XmlSchemaSimpleTypeRestriction restriction)
            {
                var typeArtItem = ArtItem.Restriction;

                var baseType = _context.SchemaSetManager.SchemaSet.ResolveType(restriction.BaseType, restriction.BaseTypeName);

                if (baseType != null && baseType.QualifiedName.IsEmpty)
                {
                    _writer.WriteArtItemWithText(typeArtItem, "Restriction");
                }
                else if (baseType == null)
                {
                    _writer.WriteArtItemWithText(typeArtItem, restriction.BaseTypeName.Name);
                }
                else
                {
                    var topic = _context.TopicManager.GetTopic(baseType);
                    if (topic != null)
                    {
                        _writer.WriteArtItemWithTopicLink(typeArtItem, topic);
                    }
                    else
                    {
                        _writer.WriteArtItemWithText(typeArtItem, baseType.QualifiedName.Name);
                    }
                }
            }

            protected override void Visit(XmlSchemaSimpleTypeUnion union)
            {
                foreach (var baseMemberType in union.BaseMemberTypes)
                {
                    if (!baseMemberType.QualifiedName.IsEmpty)
                        continue;

                    _writer.WriteArtItemWithText(ArtItem.Union, "Union");
                    return;
                }

                _writer.StartArtItem(ArtItem.Union);

                var isFirst = true;
                foreach (var baseMemberType in union.BaseMemberTypes)
                {
                    if (isFirst)
                    {
                        _writer.WriteString(", ");
                        isFirst = false;
                    }

                    var topic = _context.TopicManager.GetTopic(baseMemberType);
                    if (topic != null)
                    {
                        _writer.WriteTopicLink(topic);
                    }
                    else
                    {
                        _writer.WriteString(baseMemberType.QualifiedName.Name);
                    }
                }

                _writer.EndArtItem();
            }
        }

        #endregion

        public static void WriteAttributeTable(this MamlWriter writer, Context context, AttributeEntries attributeEntries)
        {
            if (attributeEntries.Attributes.Count == 0 && attributeEntries.AnyAttribute == null)
                return;

            writer.StartTable();
            writer.StartTableHeader();
            writer.StartTableRow();

            writer.WriteRowEntry("Name");  
            writer.WriteRowEntry("Type");  
            writer.WriteRowEntry("Required");
            writer.WriteRowEntry("Description");

            writer.EndTableRow();
            writer.EndTableHeader();

            var sortedAttributes = from a in attributeEntries.Attributes
                                   orderby a.QualifiedName.Name
                                   select a;

            foreach (var attribute in sortedAttributes)
            {
                writer.StartTableRow();

                writer.StartTableRowEntry();
                writer.StartParagraph();
                writer.WriteAttributeTopicLink(context.TopicManager, attribute, 
                    false, 0);
                writer.EndParagraph();
                writer.EndTableRowEntry();

                writer.StartTableRowEntry();
                writer.StartParagraph();
                writer.WriteType(context, attribute.AttributeSchemaType);
                writer.EndParagraph();
                writer.EndTableRowEntry();

                writer.StartTableRowEntry();
                writer.StartParagraph();
                writer.WriteRequiredText(attribute.Use);
                writer.EndParagraph();
                writer.EndTableRowEntry();

                writer.StartTableRowEntry();
                writer.StartParagraph();
                writer.WriteSummaryForObject(context, attribute);
                writer.EndParagraph();
                writer.EndTableRowEntry();

                writer.EndTableRow();
            }

            if (attributeEntries.AnyAttribute != null)
            {
                writer.StartTableRow();

                writer.StartTableRowEntry();
                writer.StartParagraph();
                writer.WriteArtItemWithText(ArtItem.AnyAttribute, "Any");
                writer.EndParagraph();
                writer.EndTableRowEntry();

                writer.StartTableRowEntry();
                writer.WriteParagraph(String.Empty);
                writer.EndTableRowEntry();

                writer.StartTableRowEntry();
                writer.WriteParagraph(String.Empty);
                writer.EndTableRowEntry();

                writer.StartTableRowEntry();
                writer.StartParagraph();
                writer.WriteSummaryForObject(context, attributeEntries.AnyAttribute);
                writer.EndParagraph();
                writer.EndTableRowEntry();

                writer.EndTableRow();
            }

            var sortedExtensionAttributes = from a in attributeEntries.ExtensionAttributes
                                            orderby a.QualifiedName.Name
                                            select a;
            foreach (var attribute in sortedExtensionAttributes)
            {
                writer.StartTableRow();

                writer.StartTableRowEntry();
                writer.StartParagraph();
                //writer.WriteHtmlIndent(1);
                writer.WriteAttributeTopicLink(context.TopicManager, attribute, 
                    true, 1);
                writer.EndParagraph();
                writer.EndTableRowEntry();

                writer.StartTableRowEntry();
                writer.StartParagraph();
                writer.WriteType(context, attribute.AttributeSchemaType);
                writer.EndParagraph();
                writer.EndTableRowEntry();

                writer.StartTableRowEntry();
                writer.StartParagraph();
                writer.WriteRequiredText(attribute.Use);
                writer.EndParagraph();
                writer.EndTableRowEntry();

                writer.StartTableRowEntry();
                writer.StartParagraph();
                writer.WriteSummaryForObject(context, attribute);
                writer.EndParagraph();
                writer.EndTableRowEntry();

                writer.EndTableRow();
            }

            writer.EndTable();
        }

        private static void WriteAttributeTopicLink(this MamlWriter writer, 
            TopicManager topicManager, XmlSchemaAttribute attribute, 
            bool isExtension, int indent)
        {
            var artItem = attribute.RefName.IsEmpty && !isExtension
                            ? ArtItem.Attribute
                            : ArtItem.AttributeRef;

            var topic = topicManager.GetTopic(attribute);
            if (topic != null)
            {
                writer.WriteArtItemWithTopicLink(artItem, topic, indent);
            }
            else
            {
                writer.WriteArtItemWithText(artItem, 
                    attribute.QualifiedName.Name, indent);
            }
        }

        private static void WriteRequiredText(this MamlWriter writer, 
            XmlSchemaUse use)
        {
            switch (use)
            {
                case XmlSchemaUse.None:
                case XmlSchemaUse.Optional:
                    break;
                case XmlSchemaUse.Required:
                    writer.WriteString("Yes");
                    break;
                default:
                    throw ExceptionBuilder.UnhandledCaseLabel(use);
            }
        }

        private static void WriteType(this MamlWriter writer, Context context, 
            XmlSchemaSimpleType type)
        {
            var typeNameWriter = new AttributeTypeNameWriter(writer, context);
            typeNameWriter.Traverse(type);
        }
    }
}