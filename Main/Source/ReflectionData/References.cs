// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Sandcastle.ReflectionData
{
    [Serializable]
    public enum ReferenceType
    {
        None                            = 0,
        Invalid                         = 1,
        Namespace                       = 2,
        ExtensionMethod                 = 3,

        SimpleMember                    = 4,
        SpecializedMember               = 5,
        SpecializedMemberWithParameters = 6,

        SimpleType                      = 7,
        SpecializedType                 = 8,
        ArrayType                       = 9, 
        ReferenceType                   = 10,
        PointerType                     = 11,

        IndexedTemplate                 = 12, 
        NamedTemplate                   = 13,
        TypeTemplate                    = 14,
        MethodTemplate                  = 15
    }


    public enum ReferenceLinkType
    {
        None         = 0,
        Self         = 1,
        Local        = 2,
        Index        = 3,
        LocalOrIndex = 4,
        Msdn         = 5,
        Id           = 6
    }

    [Serializable]
    public abstract class Reference : IXmlSerializable
    {
        #region Constructors and Destructor

        protected Reference()
        {
        }

        #endregion

        #region Public Properties

        public abstract ReferenceType ReferenceType
        {
            get;
        }

        #endregion

        #region IXmlSerializable Members

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
        }

        public virtual void WriteXml(XmlWriter writer)
        {
        }

        #endregion
    }

    [Serializable]
    public sealed class NamespaceReference : Reference
    {
        #region Private Fields

        private string namespaceId;

        #endregion

        #region Constructors and Destructor

        internal NamespaceReference()
        {
            this.namespaceId = String.Empty;
        }

        public NamespaceReference(string id)
        {
            this.namespaceId = id;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.Namespace;
            }
        }

        public string Id
        {
            get
            {
                return (namespaceId);
            }
        }

        #endregion

        #region Public Methods

        public Target Resolve(TargetCollection targets)
        {
            return (targets[namespaceId]);
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "NamespaceReference", StringComparison.OrdinalIgnoreCase))
            {
                namespaceId = reader.GetAttribute("id");
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "NamespaceReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            namespaceId = reader.GetAttribute("id");
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "NamespaceReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("NamespaceReference");
            writer.WriteAttributeString("id", namespaceId);
            writer.WriteEndElement();
        }

        #endregion
    }

    /// <summary>
    /// Contains the information to generate the display string for an extension method link
    /// </summary>
    [Serializable]
    public sealed class ExtensionMethodReference : Reference
    {
        #region Private Fields

        private string               methodName;
        private IList<Parameter>     parameters;
        private IList<TypeReference> templateArgs;

        #endregion

        #region Constructors and Destructor

        internal ExtensionMethodReference()
        {
        }

        public ExtensionMethodReference(string methodName, IList<Parameter> parameters,
            IList<TypeReference> templateArgs)
        {
            if (methodName == null)
                throw new ArgumentNullException("methodName");

            this.methodName   = methodName;
            this.parameters   = parameters;
            this.templateArgs = templateArgs;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.ExtensionMethod;
            }
        }

        public string Name
        {
            get
            {
                return (methodName);
            }
        }

        public IList<Parameter> Parameters
        {
            get
            {
                return (parameters);
            }
        }

        public IList<TypeReference> TemplateArgs
        {
            get
            {
                return (templateArgs);
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "ExtensionMethodReference",
                StringComparison.OrdinalIgnoreCase))
            {
                methodName = reader.GetAttribute("methodName");
            }

            parameters   = new List<Parameter>();
            templateArgs = new List<TypeReference>();

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "ExtensionMethodReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        methodName = reader.GetAttribute("methodName");
                    }
                    else if (String.Equals(reader.Name, "Parameters",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        while (reader.Read())
                        {
                            nodeType = reader.NodeType;

                            if (nodeType == XmlNodeType.Element)
                            {
                                if (String.Equals(reader.Name, "Parameter",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    Parameter parameter = new Parameter();
                                    parameter.ReadXml(reader);

                                    parameters.Add(parameter);
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "Parameters",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (String.Equals(reader.Name, "TemplateArgs",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        while (reader.Read())
                        {
                            nodeType = reader.NodeType;

                            if (nodeType == XmlNodeType.Element)
                            {
                                if (reader.Name.EndsWith("TypeReference",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    TypeReference typeRef = ReferencesReader.ReadTypeReference(reader);
                                    if (typeRef != null)
                                    {
                                        templateArgs.Add(typeRef);
                                    }
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "TemplateArgs",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "ExtensionMethodReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("ExtensionMethodReference");
            if (methodName == null)
            {
                methodName = String.Empty;
            }
            writer.WriteAttributeString("methodName", methodName);

            if (parameters != null && parameters.Count != 0)
            {
                writer.WriteStartElement("Parameters");
                for (int i = 0; i < parameters.Count; i++)
                {
                    parameters[i].WriteXml(writer);
                }
                writer.WriteEndElement();
            }
            if (templateArgs != null && templateArgs.Count != 0)
            {
                writer.WriteStartElement("TemplateArgs");
                for (int i = 0; i < templateArgs.Count; i++)
                {
                    templateArgs[i].WriteXml(writer);
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        #endregion
    }

    [Serializable]
    public sealed class InvalidReference : Reference
    {
        #region Private Fields

        private string id;

        #endregion

        #region Constructors and Destructor

        internal InvalidReference()
        {
            this.id = String.Empty;
        }

        public InvalidReference(string id)
        {
            this.id = id;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.Invalid;
            }
        }

        public String Id
        {
            get
            {
                return (id);
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "InvalidReference", StringComparison.OrdinalIgnoreCase))
            {
                id = reader.GetAttribute("id");
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "InvalidReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            id = reader.GetAttribute("id");
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "InvalidReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("InvalidReference");
            writer.WriteAttributeString("id", id);
            writer.WriteEndElement();
        }

        #endregion
    }

    [Serializable]
    public abstract class TypeReference : Reference 
    { 
        #region Constructors and Destructor

        protected TypeReference()
        {
        }

        #endregion
    }
    
    [Serializable]
    public sealed class SimpleTypeReference : TypeReference
    {
        #region Private Fields

        private string typeId;

        #endregion

        #region Constructors and Destructor

        internal SimpleTypeReference()
        {
            this.typeId = String.Empty;
        }

        public SimpleTypeReference(string id)
        {
            this.typeId = id;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.SimpleType;
            }
        }

        public string Id
        {
            get
            {
                return (typeId);
            }
        }

        #endregion

        #region Public Methods

        public Target Resolve(TargetCollection targets)
        {
            return (targets[typeId]);
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "SimpleTypeReference", StringComparison.OrdinalIgnoreCase))
            {
                typeId = reader.GetAttribute("id");
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "SimpleTypeReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            typeId = reader.GetAttribute("id");
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "SimpleTypeReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("SimpleTypeReference");
            writer.WriteAttributeString("id", typeId);
            writer.WriteEndElement();
        }

        #endregion
    }

    [Serializable]
    public sealed class SpecializedTypeReference : TypeReference
    {
        #region Private Fields

        private IList<Specialization> specializations;

        #endregion

        #region Constructors and Destructor

        internal SpecializedTypeReference()
        {
        }

        public SpecializedTypeReference(IList<Specialization> specializations)
        {
            if (specializations == null)
                throw new ArgumentNullException("specializations");

            this.specializations = specializations;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.SpecializedType;
            }
        }

        public IList<Specialization> Specializations
        {
            get
            {
                return (specializations);
            }
        }

        #endregion

        #region Public Methods

        public IDictionary<IndexedTemplateTypeReference, TypeReference> GetSpecializationDictionary()
        {
            Dictionary<IndexedTemplateTypeReference, TypeReference> dictionary = 
                new Dictionary<IndexedTemplateTypeReference, TypeReference>();
            foreach (Specialization specialization in specializations)
            {
                IList<TypeReference> arguments = specialization.arguments;
                for (int index = 0; index < arguments.Count; index++)
                {
                    IndexedTemplateTypeReference template = 
                        new IndexedTemplateTypeReference(specialization.TemplateType.Id, index);
                    dictionary.Add(template, arguments[index]);
                }
            }

            return (dictionary);
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            specializations = new List<Specialization>();

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "Specialization",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        Specialization item = new Specialization();
                        item.ReadXml(reader);

                        specializations.Add(item);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "SpecializedTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("SpecializedTypeReference");
            if (specializations != null && specializations.Count != 0)
            {
                for (int i = 0; i < specializations.Count; i++)
                {
                    specializations[i].WriteXml(writer);
                }
            }
            writer.WriteEndElement();
        }

        #endregion
    }

    [Serializable]
    public sealed class ArrayTypeReference : TypeReference
    {
        #region Private Fields

        private int rank;
        private TypeReference elementType;

        #endregion

        #region Constructors and Destructor

        internal ArrayTypeReference()
        {
        }

        public ArrayTypeReference(TypeReference elementType, int rank)
        {
            if (elementType == null)
                throw new ArgumentNullException("elementType");
            if (rank <= 0)
                throw new ArgumentOutOfRangeException("rank");

            this.elementType = elementType;
            this.rank = rank;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.ArrayType;
            }
        }

        public int Rank
        {
            get
            {
                return (rank);
            }
        }

        public TypeReference ElementType
        {
            get
            {
                return (elementType);
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            bool readContent = false;
            if (String.Equals(reader.Name, "ArrayTypeReference",
                StringComparison.OrdinalIgnoreCase))
            {
                string tempText = reader.GetAttribute("rank");
                if (!String.IsNullOrEmpty(tempText))
                {
                    rank = Int32.Parse(tempText);
                }

                readContent = true;
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "ArrayTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        if (readContent)
                        {
                            // It must be a nested ArrayTypeReference type...
                            elementType = ReferencesReader.ReadTypeReference(reader);
                        }
                        else
                        {
                            string tempText = reader.GetAttribute("rank");
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                rank = Int32.Parse(tempText);
                            }
                            readContent = true;
                        }
                    }
                    else if (reader.Name.EndsWith("TypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        elementType = ReferencesReader.ReadTypeReference(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "ArrayTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("ArrayTypeReference");
            writer.WriteAttributeString("rank", rank.ToString());

            if (elementType != null)
            {
                elementType.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion
    }

    [Serializable]
    public sealed class ReferenceTypeReference : TypeReference
    {
        #region Private Fields

        private TypeReference referredToType;

        #endregion

        #region Constructors and Destructor

        internal ReferenceTypeReference()
        {
        }

        public ReferenceTypeReference(TypeReference referredToType)
        {
            if (referredToType == null)
                throw new ArgumentNullException("referedToType");

            this.referredToType = referredToType;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.ReferenceType;
            }
        }

        public TypeReference ReferedToType
        {
            get
            {
                return (referredToType);
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (reader.Name.EndsWith("TypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        referredToType = ReferencesReader.ReadTypeReference(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "ReferenceTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("ReferenceTypeReference");

            if (referredToType != null)
            {
                referredToType.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion
    }

    [Serializable]
    public sealed class PointerTypeReference : TypeReference
    {
        #region Private Fields

        private TypeReference pointedToType;

        #endregion

        #region Constructors and Destructor

        internal PointerTypeReference()
        {
        }

        public PointerTypeReference(TypeReference pointedToType)
        {
            if (pointedToType == null)
                throw new ArgumentNullException("pointedToType");

            this.pointedToType = pointedToType;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.PointerType;
            }
        }

        public TypeReference PointedToType
        {
            get
            {
                return (pointedToType);
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (reader.Name.EndsWith("TypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        pointedToType = ReferencesReader.ReadTypeReference(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "PointerTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("PointerTypeReference");

            if (pointedToType != null)
            {
                pointedToType.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion
    }
    
    [Serializable]
    public abstract class TemplateTypeReference : TypeReference 
    { 
        #region Constructors and Destructor

        protected TemplateTypeReference()
        {
        }

        #endregion
    }

    [Serializable]
    public sealed class IndexedTemplateTypeReference : TemplateTypeReference
    {
        #region Private Fields

        private string templateId;
        private int index;

        #endregion

        #region Constructors and Destructor

        internal IndexedTemplateTypeReference()
        {
        }

        public IndexedTemplateTypeReference(string templateId, int index)
        {
            if (templateId == null)
                throw new ArgumentNullException("templateId");
            if (index < 0)
                throw new ArgumentOutOfRangeException("index");

            this.templateId = templateId;
            this.index      = index;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.IndexedTemplate;
            }
        }

        public string TemplateId
        {
            get
            {
                return (templateId);
            }
        }

        public int Index
        {
            get
            {
                return (index);
            }
        }

        #endregion

        #region Public Methods

        public override int GetHashCode()
        {
            return (index ^ templateId.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            IndexedTemplateTypeReference other = obj as IndexedTemplateTypeReference;
            if (other == null) return (false);
            if ((this.index == other.index) && (this.templateId == other.templateId))
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "IndexedTemplateTypeReference", 
                StringComparison.OrdinalIgnoreCase))
            {
                templateId      = reader.GetAttribute("id");
                string tempText = reader.GetAttribute("index");
                if (!String.IsNullOrEmpty(tempText))
                {
                    index = Int32.Parse(tempText);
                }
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;

                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "IndexedTemplateTypeReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            templateId      = reader.GetAttribute("id");
                            string tempText = reader.GetAttribute("index");
                            if (!String.IsNullOrEmpty(tempText))
                            {
                                index = Int32.Parse(tempText);
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "IndexedTemplateTypeReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("IndexedTemplateTypeReference");
            writer.WriteAttributeString("id", templateId);
            writer.WriteAttributeString("index", index.ToString());
            writer.WriteEndElement();
        }

        #endregion
    }

    [Serializable]
    public sealed class NamedTemplateTypeReference : TemplateTypeReference
    {
        #region Private Fields

        private string name;

        #endregion

        #region Constructors and Destructor

        internal NamedTemplateTypeReference()
        {
            this.name = String.Empty;
        }

        public NamedTemplateTypeReference(string name)
        {
            this.name = name;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.NamedTemplate;
            }
        }

        public string Name
        {
            get
            {
                return (name);
            }
        }

        #endregion

        #region Public Methods

        #endregion 

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "NamedTemplateTypeReference", StringComparison.OrdinalIgnoreCase))
            {
                name = reader.GetAttribute("name");
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "NamedTemplateTypeReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            name = reader.GetAttribute("name");
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "NamedTemplateTypeReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("NamedTemplateTypeReference");
            writer.WriteAttributeString("name", name);
            writer.WriteEndElement();
        }

        #endregion
    }

    [Serializable]
    public sealed class TypeTemplateTypeReference : TemplateTypeReference
    {
        #region Private Fields

        private int position;
        private SimpleTypeReference template;

        #endregion

        #region Constructors and Destructor

        internal TypeTemplateTypeReference()
        {
        }

        public TypeTemplateTypeReference(SimpleTypeReference template, int position)
        {
            if (template == null)
                throw new ArgumentNullException("template");
            if (position < 0)
                throw new ArgumentOutOfRangeException("position");

            this.template = template;
            this.position = position;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.TypeTemplate;
            }
        }

        public SimpleTypeReference TemplateType
        {
            get
            {
                return (template);
            }
        }

        public int Position
        {
            get
            {
                return (position);
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "TypeTemplateTypeReference", 
                StringComparison.OrdinalIgnoreCase))
            {
                string tempText = reader.GetAttribute("position");
                if (!String.IsNullOrEmpty(tempText))
                {
                    position = Int32.Parse(tempText);
                }
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "TypeTemplateTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string tempText = reader.GetAttribute("position");
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            position = Int32.Parse(tempText);
                        }
                    }
                    else if (String.Equals(reader.Name, "SimpleTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        template = new SimpleTypeReference();
                        template.ReadXml(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "TypeTemplateTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("TypeTemplateTypeReference");
            writer.WriteAttributeString("position", position.ToString());

            if (template != null)
            {
                template.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion
    }

    [Serializable]
    public sealed class MethodTemplateTypeReference : TemplateTypeReference
    {
        #region Private Fields

        private int position;  
        private MemberReference template;

        #endregion

        #region Constructors and Destructor

        internal MethodTemplateTypeReference()
        {
        }

        public MethodTemplateTypeReference(MemberReference template, int position)
        {
            this.template = template;
            this.position = position;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.MethodTemplate;
            }
        }

        public MemberReference TemplateMethod
        {
            get
            {
                return (template);
            }
        }

        public int Position
        {
            get
            {
                return (position);
            }
        }

        #endregion

        #region Public Methods

        #endregion 

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "MethodTemplateTypeReference",
                StringComparison.OrdinalIgnoreCase))
            {
                string tempText = reader.GetAttribute("position");
                if (!String.IsNullOrEmpty(tempText))
                {
                    position = Int32.Parse(tempText);
                }
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "MethodTemplateTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        string tempText = reader.GetAttribute("position");
                        if (!String.IsNullOrEmpty(tempText))
                        {
                            position = Int32.Parse(tempText);
                        }
                    }
                    else if (reader.Name.IndexOf("Member", 0, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        template = ReferencesReader.ReadMemberReference(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "MethodTemplateTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("MethodTemplateTypeReference");
            writer.WriteAttributeString("position", position.ToString());

            if (template != null)
            {
                template.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion
    }

    [Serializable]
    public abstract class MemberReference : Reference
    {
        #region Constructors and Destructor

        protected MemberReference()
        {
        }

        #endregion
    }
                               
    [Serializable]
    public sealed class SimpleMemberReference : MemberReference
    {
        #region Private Fields

        private string memberId;

        #endregion

        #region Constructors and Destructor

        internal SimpleMemberReference()
        {
            this.memberId = String.Empty;
        }

        public SimpleMemberReference(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");

            this.memberId = id;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.SimpleMember;
            }
        }

        public string Id
        {
            get
            {
                return (memberId);
            }
        }

        public Target Resolve(TargetCollection targets)
        {
            return (targets[memberId]);
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "SimpleMemberReference", StringComparison.OrdinalIgnoreCase))
            {
                memberId = reader.GetAttribute("id");
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "SimpleMemberReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            memberId = reader.GetAttribute("id");
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "SimpleMemberReference",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("SimpleMemberReference");
            writer.WriteAttributeString("id", memberId);
            writer.WriteEndElement();
        }

        #endregion
    }

    [Serializable]
    public sealed class SpecializedMemberReference : MemberReference
    {
        #region Private Fields

        private SimpleMemberReference    member; 
        private SpecializedTypeReference type;

        #endregion

        #region Constructors and Destructor

        internal SpecializedMemberReference()
        {
        }

        public SpecializedMemberReference(SimpleMemberReference member, SpecializedTypeReference type)
        {
            if (member == null)
                throw new ArgumentNullException("member");
            if (type == null)
                throw new ArgumentNullException("type");

            this.member = member;
            this.type   = type;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.SpecializedMember;
            }
        }

        public SimpleMemberReference TemplateMember
        {
            get
            {
                return (member);
            }
        }

        public SpecializedTypeReference SpecializedType
        {
            get
            {
                return (type);
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null || reader.IsEmptyElement)
            {
                return;
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "SimpleMemberReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        member = new SimpleMemberReference();
                        member.ReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, "SpecializedTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        type = new SpecializedTypeReference();
                        type.ReadXml(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "SpecializedMemberReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("SpecializedMemberReference");

            if (member != null)
            {
                member.WriteXml(writer);
            }
            if (type != null)
            {
                type.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion
    }

    [Serializable]
    public sealed class SpecializedMemberWithParametersReference : MemberReference
    {
        #region Private Fields

        private string prefix;
        private string member;        
        private SpecializedTypeReference type;
        private IList<TypeReference> parameters;

        #endregion

        #region Constructors and Destructor

        internal SpecializedMemberWithParametersReference()
        {
            this.prefix     = String.Empty;
            this.member     = String.Empty;
        }

        public SpecializedMemberWithParametersReference(string prefix, SpecializedTypeReference type,
            string member, IList<TypeReference> parameters)
        {
            if (type == null) 
                throw new ArgumentNullException("type");
            if (parameters == null) 
                throw new ArgumentNullException("parameters");

            this.prefix     = prefix;
            this.type       = type;
            this.member     = member;
            this.parameters = parameters;
        }

        #endregion

        #region Public Properties

        public override ReferenceType ReferenceType
        {
            get
            {
                return ReferenceType.SpecializedMemberWithParameters;
            }
        }

        public string Prefix
        {
            get
            {
                return prefix;
            }
        }

        public SpecializedTypeReference SpecializedType
        {
            get
            {
                return type;
            }
        }

        public string MemberName
        {
            get
            {
                return member;
            }
        }

        public IList<TypeReference> ParameterTypes
        {
            get
            {
                return parameters;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "SpecializedMemberWithParametersReference",
                StringComparison.OrdinalIgnoreCase))
            {
                prefix = reader.GetAttribute("prefix");
                member = reader.GetAttribute("member");
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            parameters = new List<TypeReference>();

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "SpecializedMemberWithParametersReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        prefix = reader.GetAttribute("prefix");
                        member = reader.GetAttribute("member");
                    }
                    else if (String.Equals(reader.Name, "SpecializedTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        type = new SpecializedTypeReference();
                        type.ReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, "Parameters",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        while (reader.Read())
                        {
                            nodeType = reader.NodeType;

                            if (nodeType == XmlNodeType.Element)
                            {
                                if (reader.Name.EndsWith("TypeReference",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    TypeReference typeRef = ReferencesReader.ReadTypeReference(reader);
                                    if (typeRef != null)
                                    {
                                        parameters.Add(typeRef);
                                    }
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "Parameters",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "SpecializedMemberWithParametersReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("SpecializedMemberWithParametersReference");
            writer.WriteAttributeString("prefix", prefix);
            writer.WriteAttributeString("member", member);

            if (type != null)
            {
                type.WriteXml(writer);
            }
            if (parameters != null && parameters.Count != 0)
            {
                writer.WriteStartElement("Parameters");
                for (int i = 0; i < parameters.Count; i++)
                {
                    parameters[i].WriteXml(writer);
                }
                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        #endregion
    }

    [Serializable]
    public sealed class Specialization : IXmlSerializable
    {
        #region Private Fields

        private SimpleTypeReference template;
        public IList<TypeReference> arguments;

        #endregion

        #region Constructors and Destructor

        internal Specialization()
        {
        }

        public Specialization(SimpleTypeReference template, IList<TypeReference> arguments)
        {
            if (template == null)
                throw new ArgumentNullException("template");
            if (arguments == null)
                throw new ArgumentNullException("arguments");

            this.template  = template;
            this.arguments = arguments;
        }

        #endregion

        #region Public Properties

        public SimpleTypeReference TemplateType
        {
            get
            {
                return template;
            }
        }

        public IList<TypeReference> Arguments
        {
            get
            {
                return arguments;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region IXmlSerializable Members

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader == null || reader.IsEmptyElement)
            {
                return;
            }

            arguments = new List<TypeReference>();

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "SimpleTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        template = new SimpleTypeReference();
                        template.ReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, "Arguments",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        while (reader.Read())
                        {
                            nodeType = reader.NodeType;

                            if (nodeType == XmlNodeType.Element)
                            {
                                if (reader.Name.EndsWith("TypeReference",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    TypeReference typeRef = ReferencesReader.ReadTypeReference(reader);
                                    if (typeRef != null)
                                    {
                                        arguments.Add(typeRef);
                                    }
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "Arguments",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "Specialization",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            writer.WriteStartElement("Specialization");
            if (template != null)
            {
                template.WriteXml(writer);
            }
            if (arguments != null && arguments.Count != 0)
            {
                writer.WriteStartElement("Arguments");
                for (int i = 0; i < arguments.Count; i++)
                {
                    arguments[i].WriteXml(writer);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        #endregion
    }
}
