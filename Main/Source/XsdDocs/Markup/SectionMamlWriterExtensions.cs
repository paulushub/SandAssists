using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;

using XsdDocumentation.Model;

namespace XsdDocumentation.Markup
{
    internal static class SectionMamlWriterExtensions
    {
        #region Section

        private static void WriteSection(this MamlWriter writer, string title, 
            string address, XmlNode sectionNode)
        {
            var contentNode = sectionNode.ChildNodes[0];
            var sectionsNode = sectionNode.ChildNodes.Count <= 1
                                ? null
                                : sectionNode.ChildNodes[1];

            writer.StartSection(title, address);
            writer.WriteRawContent(contentNode);
            if (sectionsNode != null)
                writer.WriteRawContent(sectionsNode);
            writer.EndSection();
        }

        #endregion

        #region Introduction

        public static void WriteIntroductionForSchemaSet(this MamlWriter writer, 
            Context context)
        {
            var documentationInfo = context.DocumentationManager.GetSchemaSetDocumentationInfo();

            writer.StartIntroduction();
            writer.WriteSummary(documentationInfo);
            //writer.StartParagraph();
            //writer.WriteToken("autoOutline");
            //writer.EndParagraph();
            writer.EndIntroduction();
        }

        public static void WriteIntroductionForNamespace(this MamlWriter writer, 
            Context context, string targetNamespace)
        {
            var documentationInfo = 
                context.DocumentationManager.GetNamespaceDocumentationInfo(targetNamespace);

            writer.StartIntroduction();
            writer.WriteSummary(documentationInfo);
            if (context.Configuration.IncludeAutoOutline)
            {
                writer.StartParagraph();
                writer.WriteToken("autoOutline");
                writer.EndParagraph();
            }
            writer.WriteObsoleteInfo(context, targetNamespace);
            writer.EndIntroduction();
        }

        public static void WriteIntroductionForOverview(this MamlWriter writer, 
            Context context, string namespaceUri)
        {
            writer.StartIntroduction();
            //writer.StartParagraph();
            //writer.WriteToken("autoOutline");
            //writer.EndParagraph();
            writer.StartParagraph();
            writer.WriteString("The ");
            writer.WriteNamespaceLink(context, namespaceUri);
            writer.WriteString(" namespace exposes the following members.");
            writer.EndParagraph();
            writer.EndIntroduction();
        }

        public static void WriteIntroductionForSchema(this MamlWriter writer, 
            Context context, XmlSchema schema)
        {
            var documentationInfo = 
                context.DocumentationManager.GetObjectDocumentationInfo(schema);

            writer.StartIntroduction();
            writer.WriteSummary(documentationInfo);
            if (context.Configuration.IncludeAutoOutline)
            {
                writer.StartParagraph();
                writer.WriteToken("autoOutline");
                writer.EndParagraph();
            }
            writer.WriteObsoleteInfo(context, schema);
            writer.WriteNamespaceInfo(context, schema.TargetNamespace);
            writer.EndIntroduction();
        }

        public static void WriteIntroductionForObject(this MamlWriter writer, 
            Context context, XmlSchemaObject obj)
        {
            DocumentationInfo documentationInfo = 
                context.DocumentationManager.GetObjectDocumentationInfo(obj);

            writer.StartIntroduction();
            writer.WriteSummary(documentationInfo);
            if (context.Configuration.IncludeAutoOutline)
            {
                XmlSchemaAttribute attribute;
                XmlSchemaElement element;
                XmlSchemaGroup group;
                XmlSchemaAttributeGroup attributeGroup;
                XmlSchemaSimpleType simpleType;
                XmlSchemaComplexType complexType;
      
                if (Casting.TryCast(obj, out element))
                {
                    bool isSimpleType = element.ElementSchemaType is XmlSchemaSimpleType;
                    if (!isSimpleType)
                    {
                        writer.StartParagraph();
                        writer.WriteToken("autoOutline");
                        writer.EndParagraph();

                        context.MoveToTopLink = true;
                    }
                }
                else if (Casting.TryCast(obj, out attribute))
                {   
                }
                else if (Casting.TryCast(obj, out group))
                {
                    XmlSchemaGroupBase compositor = group.Particle;
                    if (compositor != null)
                    {
                        XmlSchemaObjectCollection items = compositor.Items;
                        if (items != null && items.Count > 3)
                        {
                            writer.StartParagraph();
                            writer.WriteToken("autoOutline");
                            writer.EndParagraph();

                            context.MoveToTopLink = true;
                        }
                    }
                }
                else if (Casting.TryCast(obj, out attributeGroup))
                {
                }
                else if (Casting.TryCast(obj, out simpleType))
                {
                }
                else if (Casting.TryCast(obj, out complexType))
                {
                    if (complexType.ContentType != XmlSchemaContentType.Empty
                        && complexType.ContentType != XmlSchemaContentType.TextOnly)
                    {
                        writer.StartParagraph();
                        writer.WriteToken("autoOutline");
                        writer.EndParagraph();

                        context.MoveToTopLink = true;
                    }
                }
            }
            writer.WriteObsoleteInfo(context, obj);
            writer.WriteNamespaceAndSchemaInfo(context, obj);
            writer.EndIntroduction();
        }

        #endregion

        #region Type

        public static void WriteTypeSection(this MamlWriter writer, Context context, XmlSchemaElement element)
        {
            var isSimpleType = element.ElementSchemaType is XmlSchemaSimpleType;
            if (isSimpleType)
                return;

            // Test for empty contents...
            writer.StartFragment();
            writer.WriteTypeName(context.TopicManager, element.ElementSchemaType);
            writer.EndFragment();

            if (String.IsNullOrEmpty(writer.Framement))
            {
                return;
            }

            writer.StartSection("Type", "type");
            writer.StartParagraph();
            writer.WriteTypeName(context.TopicManager, element.ElementSchemaType);
            writer.EndParagraph();
            writer.EndSection();
        }

        #endregion

        #region Base Type

        public static void WriteBaseTypeSection(this MamlWriter writer, Context context, XmlSchemaComplexType complexType)
        {
            // Test for empty contents...
            writer.StartFragment();
            writer.WriteTypeName(context.TopicManager, complexType.BaseXmlSchemaType);
            writer.EndFragment();

            if (String.IsNullOrEmpty(writer.Framement))
            {
                return;
            }

            writer.StartSection("Base Type", "baseType");
            writer.StartParagraph();
            writer.WriteTypeName(context.TopicManager, complexType.BaseXmlSchemaType);
            writer.EndParagraph();
            writer.EndSection();
        }

        #endregion

        #region Parents

        public static void WriteParentsSection(this MamlWriter writer, Context context, IEnumerable<XmlSchemaObject> parents)
        {
            // Test for empty list...
            IList<ListItem> listItems = ListItemBuilder.Build(context, parents);
            if (listItems == null || listItems.Count == 0)
            {
                return;
            }

            writer.StartSection("Parents", "parents");
            writer.StartParagraph();
            writer.WriteList(context, listItems);
            writer.EndParagraph();

            writer.StartParagraph();
            if (context.MoveToTopLink && listItems.Count > 15)
            {
                // Create a link to the introduction, it is the top...
                writer.WriteLink("#introduction", "Top");
            }
            writer.EndParagraph();

            writer.EndSection();
        }

        #endregion

        #region Usages

        public static void WriteUsagesSection(this MamlWriter writer, Context context, IEnumerable<XmlSchemaObject> usages)
        {
            // Test for empty list...
            IList<ListItem> listItems = ListItemBuilder.Build(context, usages);
            if (listItems == null || listItems.Count == 0)
            {
                return;
            }

            writer.StartSection("Usages", "usages");
            writer.StartParagraph();
            writer.WriteList(context, listItems);
            writer.EndParagraph();

            writer.StartParagraph();
            if (context.MoveToTopLink && listItems.Count > 15)
            {
                // Create a link to the introduction, it is the top...
                writer.WriteLink("#introduction", "Top");
            }
            writer.EndParagraph();

            writer.EndSection();
        }

        #endregion

        #region Children

        public static void WriteChildrenSection(this MamlWriter writer, Context context, List<ChildEntry> children)
        {
            if (children == null || children.Count == 0)
                return;

            writer.StartSection("Children", "children");
            writer.WriteChildrenTable(context, children);

            writer.StartParagraph();
            if (context.MoveToTopLink)
            {
                // Create a link to the introduction, it is the top...
                writer.WriteLink("#introduction", "Top");
            }
            writer.EndParagraph();

            writer.EndSection();
        }

        #endregion

        #region Attributes

        public static void WriteAttributesSection(this MamlWriter writer, Context context, AttributeEntries attributeEntries)
        {
            if (attributeEntries.Attributes.Count == 0 && attributeEntries.AnyAttribute == null)
                return;

            writer.StartSection("Attributes", "attributes");
            writer.WriteAttributeTable(context, attributeEntries);

            writer.StartParagraph();
            if (context.MoveToTopLink)
            {
                // Create a link to the introduction, it is the top...
                writer.WriteLink("#introduction", "Top");
            }
            writer.EndParagraph();

            writer.EndSection();
        }

        #endregion

        #region Constraints

        public static void WriteConstraintsSection(this MamlWriter writer, Context context, XmlSchemaObjectCollection constraints)
        {
            if (!context.Configuration.DocumentConstraints)
                return;

            writer.StartSection("Constraints", "constraints");
            writer.WriteConstraintTable(context, constraints);
            writer.EndSection();
        }

        #endregion

        #region Content Type

        public static void WriteContentTypeSection(this MamlWriter writer, Context context, SimpleTypeStructureNode rootNode)
        {
            writer.StartSection("Content Type", "contentType");
            writer.WriteSimpleTypeStrucure(context, rootNode);
            writer.EndSection();
        }

        #endregion

        #region Remarks

        public static void WriteRemarksSectionForSchemaSet(this MamlWriter writer, Context context)
        {
            var documentationInfo = context.DocumentationManager.GetSchemaSetDocumentationInfo();
            writer.WriteRemarksSection(documentationInfo);
        }

        public static void WriteRemarksSectionForNamespace(this MamlWriter writer, Context context, string targetNamespace)
        {
            var documentationInfo = context.DocumentationManager.GetNamespaceDocumentationInfo(targetNamespace);
            writer.WriteRemarksSection(documentationInfo);
        }

        public static void WriteRemarksSectionForObject(this MamlWriter writer, Context context, XmlSchemaObject obj)
        {
            var documentationInfo = context.DocumentationManager.GetObjectDocumentationInfo(obj);
            writer.WriteRemarksSection(documentationInfo);
        }

        private static void WriteRemarksSection(this MamlWriter writer, DocumentationInfo documentationInfo)
        {
            if (documentationInfo == null || documentationInfo.RemarksNode == null)
                return;

            writer.WriteSection("Remarks", "remarks", documentationInfo.RemarksNode);
        }

        #endregion

        #region Examples

        public static void WriteExamplesSectionForSchemaSet(this MamlWriter writer, Context context)
        {
            var documentationInfo = context.DocumentationManager.GetSchemaSetDocumentationInfo();
            writer.WriteExamplesSection(documentationInfo);
        }

        public static void WriteExamplesSectionForNamespace(this MamlWriter writer, Context context, string targetNamespace)
        {
            var documentationInfo = context.DocumentationManager.GetNamespaceDocumentationInfo(targetNamespace);
            writer.WriteExamplesSection(documentationInfo);
        }

        public static void WriteExamplesSectionForObject(this MamlWriter writer, Context context, XmlSchemaObject obj)
        {
            var documentationInfo = context.DocumentationManager.GetObjectDocumentationInfo(obj);
            writer.WriteExamplesSection(documentationInfo);
        }

        private static void WriteExamplesSection(this MamlWriter writer, DocumentationInfo documentationInfo)
        {
            if (documentationInfo == null || documentationInfo.ExamplesNode == null)
                return;

            writer.WriteSection("Examples", "examples", documentationInfo.ExamplesNode);
        }

        #endregion

        #region Syntax

        public static void WriteSyntaxSection(this MamlWriter writer, Context context, XmlSchemaObject obj)
        {
            if (!context.Configuration.DocumentSyntax)
                return;

            string sourceCodeAbridged = context.SourceCodeManager.GetSourceCodeAbridged(obj);

            StringBuilder builder = new StringBuilder(sourceCodeAbridged);
            builder.Replace("xmlns=\"http://www.w3.org/2001/XMLSchema\"",
                String.Empty);
            builder.Replace("xmlns:xs=\"http://www.w3.org/2001/XMLSchema\"",
                String.Empty);
            builder.Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"",
                String.Empty);

            writer.StartSection("Syntax", "syntax");
            writer.WriteCode(builder.ToString(), "xml");

            writer.StartParagraph();
            if (context.MoveToTopLink)
            {
                // Create a link to the introduction, it is the top...
                writer.WriteLink("#introduction", "Top");
            }
            writer.EndParagraph();

            writer.EndSection();
        }

        #endregion

        #region Related Topics

        public static void WriteRelatedTopics(this MamlWriter writer, Context context, XmlSchemaObject obj)
        {
            writer.StartRelatedTopics();
            writer.WriteLinksForObject(obj, context);
            writer.EndRelatedTopics();
        }

        #endregion

        #region Jump Table Sections

        public static void WriteNamespacesSection(this MamlWriter writer, Context context, IEnumerable<string> namespaces)
        {
            writer.WriteJumpTableSection(context, namespaces, "Namespaces", "namespaces");
        }

        public static void WriteRootSchemasSection(this MamlWriter writer, Context context, IEnumerable<XmlSchemaObject> rootSchemas)
        {
            if (context.Configuration.DocumentRootSchemas && context.Configuration.DocumentSchemas)
            {
                writer.WriteJumpTableSection(context, rootSchemas, 
                    "Root Schemas", "rootSchemas");
            }
        }

        public static void WriteRootElementsSection(this MamlWriter writer, Context context, IEnumerable<XmlSchemaObject> rootElements)
        {
            if (context.Configuration.DocumentRootElements)
                writer.WriteJumpTableSection(context, rootElements, "Root Elements", "rootElements");
        }

        public static void WriteSchemasSection(this MamlWriter writer, Context context, IEnumerable<XmlSchemaObject> schemas)
        {
            if (context.Configuration.DocumentSchemas)
                writer.WriteJumpTableSection(context, schemas, "Schemas", "schemas");
        }

        public static void WriteElementsSection(this MamlWriter writer, Context context, IEnumerable<XmlSchemaObject> elements)
        {
            writer.WriteJumpTableSection(context, elements, "Elements", "elements");
        }

        public static void WriteAttributesSection(this MamlWriter writer, Context context, IEnumerable<XmlSchemaObject> attributes)
        {
            writer.WriteJumpTableSection(context, attributes, "Attributes", "attributes");
        }

        public static void WriteGroupsSection(this MamlWriter writer, Context context, IEnumerable<XmlSchemaObject> groups)
        {
            writer.WriteJumpTableSection(context, groups, "Groups", "groups");
        }

        public static void WriteAttributeGroupsSection(this MamlWriter writer, Context context, IEnumerable<XmlSchemaObject> groups)
        {
            writer.WriteJumpTableSection(context, groups, "Attribute Groups", "attributeGroups");
        }

        public static void WriteSimpleTypesSection(this MamlWriter writer, Context context, IEnumerable<XmlSchemaObject> types)
        {
            writer.WriteJumpTableSection(context, types, "Simple Types", "simpleTypes");
        }

        public static void WriteComplexTypesSection(this MamlWriter writer, Context context, IEnumerable<XmlSchemaObject> types)
        {
            writer.WriteJumpTableSection(context, types, "Complex Types", "complexTypes");
        }

        private static void WriteJumpTableSection(this MamlWriter writer, Context context, 
            IEnumerable<string> namespaces, string title, string address)
        {
            var listItems = ListItemBuilder.Build(context, namespaces);
            writer.WriteJumpTableSection(context, listItems, title, address);
        }

        private static void WriteJumpTableSection(this MamlWriter writer, Context context, 
            IEnumerable<XmlSchemaObject> rows, string title, string address)
        {
            var listItems = ListItemBuilder.Build(context, rows);
            writer.WriteJumpTableSection(context, listItems, title, address);
        }

        private static void WriteJumpTableSection(this MamlWriter writer, 
            Context context, ICollection<ListItem> listItems, 
            string title, string address)
        {
            if (listItems.Count == 0)
                return;

            writer.StartSection(title, address);

            writer.StartTable();

            writer.StartTableHeader();
            writer.StartTableRow();

            //writer.WriteRowEntry(String.Empty);
            writer.StartTableRowEntry();
            writer.StartParagraph();
            writer.WriteToken("iconColumn");
            writer.EndParagraph();
            writer.EndTableRowEntry();

            if (title.EndsWith("s", StringComparison.Ordinal))
            {                    
                if (title.IndexOf(' ') < 0)  // if it is a single word...
                {
                    writer.WriteRowEntry(title.Remove(title.Length - 1)); 
                }
                else
                {
                    // Write non-breaking title text...
                    writer.StartTableRowEntry();
                    writer.StartParagraph();
                    writer.WriteStartElement("notLocalizable"); //notLocalizable
                    writer.WriteAttributeString("address", writer.GetNextNobrAddress());
                    writer.WriteString(title.Remove(title.Length - 1));
                    writer.WriteEndElement();                   //notLocalizable
                    writer.EndParagraph();
                    writer.EndTableRowEntry();
                }
            }
            else
            {
                writer.WriteRowEntry("Element"); 
            }
            writer.WriteRowEntry("Description");

            writer.EndTableRow();
            writer.EndTableHeader();

            foreach (var listItem in listItems)
            {
                writer.StartTableRow();

                writer.StartTableRowEntry();
                writer.StartParagraph();
                writer.WriteArtItemInline(listItem.ArtItem);
                writer.EndParagraph();
                writer.EndTableRowEntry();

                writer.StartTableRowEntry();
                writer.StartParagraph();
                writer.WriteTopicLink(listItem.Topic);
                writer.EndParagraph();
                writer.EndTableRowEntry();

                writer.StartTableRowEntry();
                writer.StartParagraph();
                writer.WriteRaw(listItem.SummaryMarkup);
                writer.EndParagraph();
                writer.EndTableRowEntry();

                writer.EndTableRow();
            }

            writer.EndTable();

            writer.StartParagraph();
            if (context.MoveToTopLink)
            {
                // Create a link to the introduction, it is the top...
                writer.WriteLink("#introduction", "Top");
            }
            writer.EndParagraph();

            writer.EndSection();
        }

        #endregion
    }
}