using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace XsdDocumentation.Model
{
    public sealed class SimpleTypeStructureNode
    {
        public SimpleTypeStructureNode()
        {
            Children = new List<SimpleTypeStructureNode>();
        }

        public SimpleTypeStructureNodeType NodeType { get; set; }
        public XmlSchemaObject Node { get; set; }
        public List<SimpleTypeStructureNode> Children { get; private set; }

        public static bool GetIsSingleRow(SimpleTypeStructureNode node)
        {
            switch (node.NodeType)
            {
                case SimpleTypeStructureNodeType.Any:
                case SimpleTypeStructureNodeType.Mixed:
                case SimpleTypeStructureNodeType.NamedType:
                case SimpleTypeStructureNodeType.List:
                case SimpleTypeStructureNodeType.Union:
                    break;
                default:
                    return false;
            }
            foreach (var child in node.Children)
            {
                switch (child.NodeType)
                {
                    case SimpleTypeStructureNodeType.Union:
                    case SimpleTypeStructureNodeType.List:
                    case SimpleTypeStructureNodeType.Restriction:
                        return false;
                }
            }

            return true;
        }
    }
}