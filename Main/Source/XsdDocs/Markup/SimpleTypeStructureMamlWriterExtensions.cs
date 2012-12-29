using System;
using System.Collections.Generic;
using System.Xml.Schema;

using XsdDocumentation.Model;

namespace XsdDocumentation.Markup
{
    internal static class SimpleTypeStructureMamlWriterExtensions
    {
        public static void WriteSimpleTypeStrucure(this MamlWriter writer, 
            Context context, SimpleTypeStructureNode root)
        {
            if (root == null || root.Children.Count == 0)
                return;

            if (root.Children.Count == 1)
            {
                var node = root.Children[0];
                var isSingleRow = SimpleTypeStructureNode.GetIsSingleRow(node);
                if (isSingleRow)
                {
                    writer.StartParagraph();
                    writer.WriteSingle(context.TopicManager, node, 0);
                    writer.EndParagraph();

                    return;
                }
            }

            writer.StartTable();

            writer.StartTableHeader();
            writer.StartTableRow();

            writer.WriteRowEntry("Item"); 
            
            //writer.WriteRowEntry("Facet Value");
            writer.StartTableRowEntry();
            writer.StartParagraph();
            writer.WriteStartElement("notLocalizable"); //notLocalizable
            writer.WriteAttributeString("address", writer.GetNextNobrAddress());
            writer.WriteString("Facet Value");
            writer.WriteEndElement();                   //notLocalizable
            writer.EndParagraph();
            writer.EndTableRowEntry();
            
            writer.WriteRowEntry("Description");

            writer.EndTableRow();
            writer.EndTableHeader();

            writer.WriteNodes(context, root.Children, 0);

            writer.EndTable();
        }

        private static void WriteNodes(this MamlWriter writer, Context context, 
            IEnumerable<SimpleTypeStructureNode> children, int level)
        {
            foreach (var childEntry in children)
            {
                if (childEntry.NodeType == SimpleTypeStructureNodeType.NamedType)
                    continue;

                writer.StartTableRow();

                var isSingleRow = SimpleTypeStructureNode.GetIsSingleRow(childEntry);
                if (isSingleRow)
                    writer.WriteSingleItemAndFacet(level, context.TopicManager, childEntry);
                else
                    writer.WriteConstructorItemAndFacet(level, context.TopicManager, childEntry);

                writer.StartTableRowEntry();
                writer.StartParagraph();
                writer.WriteSummaryForObject(context, childEntry.Node);
                writer.EndParagraph();
                writer.EndTableRowEntry();

                writer.EndTableRow();

                if (!isSingleRow)
                    writer.WriteNodes(context, childEntry.Children, level + 1);
            }
        }

        private static void WriteSingle(this MamlWriter writer, TopicManager topicManager, 
            SimpleTypeStructureNode node, int indentLevel)
        {
            switch (node.NodeType)
            {
                case SimpleTypeStructureNodeType.Any:
                    writer.WriteIndent(indentLevel);
                    writer.WriteString("Any");
                    break;
                case SimpleTypeStructureNodeType.Mixed:
                    writer.WriteIndent(indentLevel);
                    writer.WriteString("Mixed");
                    break;
                case SimpleTypeStructureNodeType.NamedType:
                    writer.WriteIndent(indentLevel);
                    writer.WriteTypeName(topicManager, (XmlSchemaType)node.Node);
                    break;
                case SimpleTypeStructureNodeType.List:
                    writer.StartArtItem(ArtItem.List, indentLevel);
                    writer.WriteTypeNamesMarkup(topicManager, node.Children);
                    writer.EndArtItem();
                    break;
                case SimpleTypeStructureNodeType.Union:
                    writer.StartArtItem(ArtItem.Union, indentLevel);
                    writer.WriteTypeNamesMarkup(topicManager, node.Children);
                    writer.EndArtItem();
                    break;
                default:
                    throw ExceptionBuilder.UnhandledCaseLabel(node.NodeType);
            }
        }

        private static void WriteSingleItemAndFacet(this MamlWriter writer, 
            int level, TopicManager topicManager, SimpleTypeStructureNode node)
        {
            writer.StartTableRowEntry();
            writer.StartParagraph();
            //writer.WriteHtmlIndent(level);
            writer.WriteSingle(topicManager, node, level);
            writer.EndParagraph();
            writer.EndTableRowEntry();

            writer.StartTableRowEntry();
            writer.WriteParagraph(String.Empty);
            writer.EndTableRowEntry();
        }

        private static void WriteTypeNamesMarkup(this MamlWriter writer, 
            TopicManager topicManager, IEnumerable<SimpleTypeStructureNode> children)
        {
            var isFirst = true;
            foreach (var node in children)
            {
                if (node.NodeType != SimpleTypeStructureNodeType.NamedType)
                    continue;

                if (isFirst)
                    isFirst = false;
                else
                    writer.WriteString(", ");

                var simpleType = (XmlSchemaSimpleType)node.Node;
                var topic = topicManager.GetTopic(simpleType);

                if (topic != null)
                    writer.WriteTopicLink(topic);
                else
                    writer.WriteString(simpleType.QualifiedName.Name);
            }
        }

        private static void WriteConstructorItemAndFacet(this MamlWriter writer, 
            int level, TopicManager topicManager, SimpleTypeStructureNode node)
        {
            switch (node.NodeType)
            {
                case SimpleTypeStructureNodeType.Restriction:
                    writer.WriteConstructor(level, topicManager, node, ArtItem.Restriction, "Restriction");
                    break;
                case SimpleTypeStructureNodeType.List:
                    writer.WriteConstructor(level, topicManager, node, ArtItem.List, "List");
                    break;
                case SimpleTypeStructureNodeType.Union:
                    writer.WriteConstructor(level, topicManager, node, ArtItem.Union, "Union");
                    break;
                case SimpleTypeStructureNodeType.FacetEnumeration:
                    writer.WriteFacet(level, node, "Enumeration");
                    break;
                case SimpleTypeStructureNodeType.FacetMaxExclusive:
                    writer.WriteFacet(level, node, "Max Exclusive");
                    break;
                case SimpleTypeStructureNodeType.FacetMaxInclusive:
                    writer.WriteFacet(level, node, "Max Inclusive");
                    break;
                case SimpleTypeStructureNodeType.FacetMinExclusive:
                    writer.WriteFacet(level, node, "Min Exclusive");
                    break;
                case SimpleTypeStructureNodeType.FacetMinInclusive:
                    writer.WriteFacet(level, node, "Min Inclusive");
                    break;
                case SimpleTypeStructureNodeType.FacetFractionDigits:
                    writer.WriteFacet(level, node, "Fraction Digits");
                    break;
                case SimpleTypeStructureNodeType.FacetLength:
                    writer.WriteFacet(level, node, "Length");
                    break;
                case SimpleTypeStructureNodeType.FacetMaxLength:
                    writer.WriteFacet(level, node, "Max Length");
                    break;
                case SimpleTypeStructureNodeType.FacetMinLength:
                    writer.WriteFacet(level, node, "Min Length");
                    break;
                case SimpleTypeStructureNodeType.FacetTotalDigits:
                    writer.WriteFacet(level, node, "Total Digits");
                    break;
                case SimpleTypeStructureNodeType.FacetPattern:
                    writer.WriteFacet(level, node, "Pattern");
                    break;
                case SimpleTypeStructureNodeType.FacetWhiteSpace:
                    writer.WriteFacet(level, node, "White Space");
                    break;
                default:
                    throw ExceptionBuilder.UnhandledCaseLabel(node.NodeType);
            }
        }

        private static void WriteConstructor(this MamlWriter writer, int level, TopicManager topicManager, 
            SimpleTypeStructureNode node, ArtItem constructionArtItem, string constructName)
        {
            writer.StartTableRowEntry();
            writer.StartParagraph();
            //writer.WriteHtmlIndent(level);
            writer.StartArtItem(constructionArtItem, level);
            if (ContainsNamedTypes(node.Children))
                writer.WriteTypeNamesMarkup(topicManager, node.Children);
            else
                writer.WriteString(constructName);
            writer.EndArtItem();
            writer.EndParagraph();
            writer.EndTableRowEntry();
            
            writer.StartTableRowEntry();
            writer.WriteParagraph(String.Empty);
            writer.EndTableRowEntry();
        }

        private static bool ContainsNamedTypes(IEnumerable<SimpleTypeStructureNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.NodeType == SimpleTypeStructureNodeType.NamedType)
                    return true;
            }

            return false;
        }

        private static void WriteFacet(this MamlWriter writer, int level, 
            SimpleTypeStructureNode node, string facetType)
        {
            var facetValue = ((XmlSchemaFacet)node.Node).Value;

            writer.StartTableRowEntry();
            writer.StartParagraph();
            //writer.WriteHtmlIndent(level);
            writer.WriteArtItemWithText(ArtItem.Facet, facetType, level);
            writer.EndParagraph();
            writer.EndTableRowEntry();

            writer.WriteRowEntry(facetValue);
        }
    }
}