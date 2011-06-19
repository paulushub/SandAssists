// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Sandcastle.ReflectionData
{
    [Serializable]
    public enum TargetType
    {
        None        = 0,
        Namespace   = 1,
        Type        = 2,
        Enumeration = 3,
        Member      = 4,
        Constructor = 5,
        Procedure   = 6,
        Event       = 7,
        Property    = 8,
        Method      = 9,
    }

    [Serializable]
    public class Target : IXmlSerializable
    {
        #region Private Fields

        internal string    id;
        internal string    container;
        internal string    file;

        internal ReferenceLinkType defaultType;
        [NonSerialized]
        private ReferenceLinkType type;

        #endregion

        #region Constructors and Destructor

        public Target()
        {
            id        = String.Empty;
            container = String.Empty;
            file      = String.Empty;
            type      = ReferenceLinkType.None;
       }

        #endregion

        #region Public Properties

        public virtual TargetType TargetType
        {
            get
            {
                return TargetType.None;
            }
        }

        public string Id
        {
            get
            {
                return id;
            }
        }

        public string Container
        {
            get
            {
                return container;
            }
        }

        public string File
        {
            get
            {
                return file;
            }
        }

        public ReferenceLinkType LinkType
        {
            get
            {
                return type;
            }
            set
            {
                type = value;
            }
        }

        public ReferenceLinkType DefaultLinkType
        {
            get
            {
                return defaultType;
            }
        }

        #endregion

        #region Public Methods

        public virtual void Add(TargetStorage targets)
        {
            targets.Add(this);
        }

        #endregion

        #region Protected Methods

        protected virtual void OnReadXml(XmlReader reader)
        {
            // This reads only the target node...
            if (!String.Equals(reader.Name, "Target",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            id        = reader.GetAttribute("id");
            container = reader.GetAttribute("container");
            file      = reader.GetAttribute("file");

            string typeText = reader.GetAttribute("type");
            if (!String.IsNullOrEmpty(typeText))
            {
                switch (typeText.ToLower())
                {
                    case "none":
                        type = ReferenceLinkType.None;
                        break;
                    case "self":
                        type = ReferenceLinkType.Self;
                        break;
                    case "local":
                        type = ReferenceLinkType.Local;
                        break;
                    case "index":
                        type = ReferenceLinkType.Index;
                        break;
                    case "localorindex":
                        type = ReferenceLinkType.LocalOrIndex;
                        break;
                    case "msdn":
                        type = ReferenceLinkType.Msdn;
                        break;
                    case "id":
                        type = ReferenceLinkType.Id;
                        break;
               }
            }
        }

        protected virtual void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Target");

            writer.WriteAttributeString("id", id);
            writer.WriteAttributeString("container", container);
            writer.WriteAttributeString("file", file);
            writer.WriteAttributeString("type", type.ToString());
            
            writer.WriteEndElement();
        }

        #endregion

        #region IXmlSerializable Members

        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        public virtual void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "Target", 
                StringComparison.OrdinalIgnoreCase))
            {
                this.OnReadXml(reader);
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "Target",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "Target",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "Target",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public virtual void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            this.OnWriteXml(writer);
        }

        #endregion
    }

    [Serializable]
    public sealed class NamespaceTarget : Target
    {
        #region Private Fields

        internal string name;

        #endregion

        #region Constructors and Destructor

        public NamespaceTarget()
        {
            name = String.Empty;
        }

        #endregion

        #region Public Properties

        public override TargetType TargetType
        {
            get
            {
                return TargetType.Namespace;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected override void OnReadXml(XmlReader reader)
        {
            // Give the base a change, just in case that is the target...
            base.OnReadXml(reader);

            // This reads only the target node...
            if (!String.Equals(reader.Name, "NamespaceTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            name = reader.GetAttribute("name");

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    // Read the base contents in...
                    if (String.Equals(reader.Name, "Target",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        base.OnReadXml(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "NamespaceTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("NamespaceTarget");
            writer.WriteAttributeString("name", name);

            // Write the base contents in...
            base.OnWriteXml(writer);

            writer.WriteEndElement(); 
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "NamespaceTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                this.OnReadXml(reader);
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "NamespaceTarget",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "NamespaceTarget",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "NamespaceTarget",
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

            this.OnWriteXml(writer);
        }

        #endregion
    }

    [Serializable]
    public class TypeTarget : Target
    {
        #region Private Fields

        internal string name;
        internal IList<string> templates;
        internal SimpleTypeReference containingType;
        internal NamespaceReference containingNamespace;

        #endregion

        #region Constructors and Destructor

        public TypeTarget()
        {
            name = String.Empty;
        }

        #endregion

        #region Public Properties

        public override TargetType TargetType
        {
            get
            {
                return TargetType.Type;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public NamespaceReference Namespace
        {
            get
            {
                return (containingNamespace);
            }
        }

        public SimpleTypeReference OuterType
        {
            get
            {
                return (containingType);
            }
        }

        public IList<string> Templates
        {
            get
            {
                return (templates);
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);

            // This reads only the target node...
            if (!String.Equals(reader.Name, "TypeTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            name      = reader.GetAttribute("name"); 
            templates = new List<string>();

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    // Read the base contents in...
                    if (String.Equals(reader.Name, "Target",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        base.OnReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, "NamespaceReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        containingNamespace = new NamespaceReference();
                        containingNamespace.ReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, "SimpleTypeReference",
                         StringComparison.OrdinalIgnoreCase))
                    {
                        containingType = new SimpleTypeReference();
                        containingType.ReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, "Templates",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        while (reader.Read())
                        {
                            nodeType = reader.NodeType;

                            if (nodeType == XmlNodeType.Element)
                            {
                                if (String.Equals(reader.Name, "Template",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    templates.Add(reader.GetAttribute("value"));
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "Templates",
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
                    if (String.Equals(reader.Name, "TypeTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("TypeTarget");

            writer.WriteAttributeString("name", name);

            // Write the base contents in...
            base.OnWriteXml(writer);

            if (containingNamespace != null)
            {
                containingNamespace.WriteXml(writer);
            }
            if (containingType != null)
            {
                containingType.WriteXml(writer);
            }
            if (templates != null && templates.Count != 0)
            {
                writer.WriteStartElement("Templates");
                for (int i = 0; i < templates.Count; i++)
                {
                    writer.WriteStartElement("Template");
                    writer.WriteAttributeString("value", templates[i]);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
           
            writer.WriteEndElement();
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "TypeTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                this.OnReadXml(reader);
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "TypeTarget",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "TypeTarget",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "TypeTarget",
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

            this.OnWriteXml(writer);
        }

        #endregion
    }

    [Serializable]
    public sealed class EnumerationTarget : TypeTarget
    {
        #region Private Fields

        internal IList<MemberTarget> elements;

        #endregion

        #region Constructors and Destructor

        public EnumerationTarget()
        {   
        }

        #endregion

        #region Public Properties

        public override TargetType TargetType
        {
            get
            {
                return TargetType.Enumeration;
            }
        }

        #endregion

        #region Public Methods

        public override void Add(TargetStorage storage)
        {
            base.Add(storage);

            foreach (MemberTarget element in elements)
            {
                element.Add(storage);
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);

            // This reads only the target node...
            if (!String.Equals(reader.Name, "EnumerationTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            elements = new List<MemberTarget>();

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    // Read the base contents in...
                    if (String.Equals(reader.Name, "TypeTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        base.OnReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, "Elements",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        while (reader.Read())
                        {
                            nodeType = reader.NodeType;

                            if (nodeType == XmlNodeType.Element)
                            {
                                if (reader.Name.IndexOf("Target", 0, StringComparison.OrdinalIgnoreCase) >= 0)
                                {
                                    MemberTarget target = TargetsReader.ReadMemberTarget(reader);
                                    if (target != null)
                                    {
                                        elements.Add(target);
                                    }
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "Elements",
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
                    if (String.Equals(reader.Name, "EnumerationTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("EnumerationTarget");

            // Write the base contents in...
            base.OnWriteXml(writer);

            if (elements != null && elements.Count != 0)
            {
                writer.WriteStartElement("Elements");

                for (int i = 0; i < elements.Count; i++)
                {
                    elements[i].WriteXml(writer);
                }

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "EnumerationTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                this.OnReadXml(reader);
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "EnumerationTarget",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "EnumerationTarget",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "EnumerationTarget",
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

            this.OnWriteXml(writer);
        }

        #endregion
    }

    [Serializable]
    public class MemberTarget : Target
    {
        #region Private Fields

        internal string name;
        internal string overload;
        internal SimpleTypeReference containingType;

        #endregion

        #region Constructors and Destructor

        public MemberTarget()
        {
            name     = String.Empty;
            overload = String.Empty;
        }

        #endregion

        #region Public Properties

        public override TargetType TargetType
        {
            get
            {
                return TargetType.Member;
            }
        }

        public string Name
        {
            get
            {
                return name;
            }
        }

        public TypeReference Type
        {
            get
            {
                return containingType;
            }
        }

        public string OverloadId
        {
            get
            {
                return overload;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);

            // This reads only the target node...
            if (!String.Equals(reader.Name, "MemberTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            name     = reader.GetAttribute("name");
            overload = reader.GetAttribute("overload");

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    // Read the base contents in...
                    if (String.Equals(reader.Name, "Target",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        base.OnReadXml(reader);
                    }
                    else if (String.Equals(reader.Name, "SimpleTypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        containingType = new SimpleTypeReference();
                        containingType.ReadXml(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "MemberTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("MemberTarget");
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("overload", overload);

            // Write the base contents in...
            base.OnWriteXml(writer);

            if (containingType != null)
            {
                containingType.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "MemberTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                this.OnReadXml(reader);
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "MemberTarget",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "MemberTarget",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "MemberTarget",
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

            this.OnWriteXml(writer);
        }

        #endregion
    }

    [Serializable]
    public sealed class ConstructorTarget : MemberTarget
    {
        #region Private Fields

        internal IList<Parameter> parameters;

        #endregion

        #region Constructors and Destructor

        public ConstructorTarget()
        {   
        }

        #endregion

        #region Public Properties

        public override TargetType TargetType
        {
            get
            {
                return TargetType.Constructor;
            }
        }

        public IList<Parameter> Parameters
        {
            get
            {
                return parameters;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);

            // This reads only the target node...
            if (!String.Equals(reader.Name, "ConstructorTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            parameters = new List<Parameter>();

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    // Read the base contents in...
                    if (String.Equals(reader.Name, "MemberTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        base.OnReadXml(reader);
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
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "ConstructorTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("ConstructorTarget");

            // Write the base contents in...
            base.OnWriteXml(writer);

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

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "ConstructorTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                this.OnReadXml(reader);
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "ConstructorTarget",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "ConstructorTarget",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "ConstructorTarget",
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

            this.OnWriteXml(writer);
        }

        #endregion
    }

    [Serializable]
    public class ProcedureTarget : MemberTarget
    {
        #region Private Fields

        internal bool conversionOperator;
        internal MemberReference explicitlyImplements;

        #endregion

        #region Constructors and Destructor

        public ProcedureTarget()
        {
        }

        #endregion

        #region Public Properties

        public override TargetType TargetType
        {
            get
            {
                return TargetType.Procedure;
            }
        }

        public bool ConversionOperator
        {
            get
            {
                return conversionOperator;
            }
        }

        public MemberReference ExplicitlyImplements
        {
            get
            {
                return explicitlyImplements;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);

            // This reads only the target node...
            if (!String.Equals(reader.Name, "ProcedureTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string tempText = reader.GetAttribute("conversionOperator");
            if (!String.IsNullOrEmpty(tempText))
            {
                conversionOperator = Boolean.Parse(tempText);
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    // Read the base contents in...
                    if (String.Equals(reader.Name, "MemberTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        base.OnReadXml(reader);
                    }
                    else if (reader.Name.IndexOf("Member", 0, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        explicitlyImplements = ReferencesReader.ReadMemberReference(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "ProcedureTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("ProcedureTarget");
            writer.WriteAttributeString("conversionOperator", conversionOperator.ToString());

            // Write the base contents in...
            base.OnWriteXml(writer);

            if (explicitlyImplements != null)
            {
                explicitlyImplements.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "ProcedureTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                this.OnReadXml(reader);
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "ProcedureTarget",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "ProcedureTarget",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "ProcedureTarget",
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

            this.OnWriteXml(writer);
        }

        #endregion
    }

    [Serializable]
    public sealed class EventTarget : ProcedureTarget
    {
        #region Constructors and Destructor

        public EventTarget()
        {   
        }

        #endregion  

        #region Public Properties

        public override TargetType TargetType
        {
            get
            {
                return TargetType.Event;
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);

            // This reads only the target node...
            if (!String.Equals(reader.Name, "EventTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    // Read the base contents in...
                    if (String.Equals(reader.Name, "ProcedureTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        base.OnReadXml(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "EventTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("EventTarget");

            // Write the base contents in...
            base.OnWriteXml(writer);

            writer.WriteEndElement();
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "EventTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                this.OnReadXml(reader);
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "EventTarget",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "EventTarget",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "EventTarget",
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

            this.OnWriteXml(writer);
        }

        #endregion
    }

    [Serializable]
    public sealed class PropertyTarget : ProcedureTarget
    {
        #region Private Fields

        internal IList<Parameter> parameters;
        internal TypeReference returnType;

        #endregion

        #region Constructors and Destructor

        public PropertyTarget()
        {   
        }

        #endregion

        #region Public Properties

        public override TargetType TargetType
        {
            get
            {
                return TargetType.Property;
            }
        }

        public IList<Parameter> Parameters
        {
            get
            {
                return parameters;
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);

            // This reads only the target node...
            if (!String.Equals(reader.Name, "PropertyTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            parameters = new List<Parameter>();

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    // Read the base contents in...
                    if (String.Equals(reader.Name, "ProcedureTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        base.OnReadXml(reader);
                    }
                    else if (reader.Name.EndsWith("TypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        returnType = ReferencesReader.ReadTypeReference(reader);
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
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "PropertyTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("PropertyTarget");

            // Write the base contents in...
            base.OnWriteXml(writer);

            if (parameters != null && parameters.Count != 0)
            {
                writer.WriteStartElement("Parameters");

                for (int i = 0; i < parameters.Count; i++)
                {
                    parameters[i].WriteXml(writer);
                }

                writer.WriteEndElement();
            }
            if (returnType != null)
            {
                returnType.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "PropertyTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                this.OnReadXml(reader);
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "PropertyTarget",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "PropertyTarget",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "PropertyTarget",
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

            this.OnWriteXml(writer);
        }

        #endregion
    }

    [Serializable]
    public sealed class MethodTarget : ProcedureTarget
    {
        #region Private Fields

        internal IList<Parameter> parameters;
        internal TypeReference    returnType;
        internal IList<string>    templates;

        // property to hold specialized template arguments (used with extension methods)
        internal IList<TypeReference> templateArgs;

        #endregion

        #region Constructors and Destructor

        public MethodTarget()
        {   
        }

        #endregion

        #region Public Properties

        public override TargetType TargetType
        {
            get
            {
                return TargetType.Method;
            }
        }

        public IList<Parameter> Parameters
        {
            get
            {
                return parameters;
            }
        }

        public IList<string> Templates
        {
            get
            {
                return templates;
            }
        }

        public IList<TypeReference> TemplateArgs
        {
            get
            {
                return templateArgs;
            }
        }

        #endregion

        #region Protected Methods

        protected override void OnReadXml(XmlReader reader)
        {
            base.OnReadXml(reader);

            // This reads only the target node...
            if (!String.Equals(reader.Name, "MethodTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            parameters   = new List<Parameter>();
            templateArgs = new List<TypeReference>();
            templates    = new List<string>();

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    // Read the base contents in...
                    if (String.Equals(reader.Name, "ProcedureTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        base.OnReadXml(reader);
                    }
                    else if (reader.Name.EndsWith("TypeReference", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        returnType = ReferencesReader.ReadTypeReference(reader);
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
                    else if (String.Equals(reader.Name, "Templates",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        while (reader.Read())
                        {
                            nodeType = reader.NodeType;

                            if (nodeType == XmlNodeType.Element)
                            {
                                if (String.Equals(reader.Name, "Template",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    templates.Add(reader.GetAttribute("value"));
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "Templates",
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
                    if (String.Equals(reader.Name, "MethodTarget",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        protected override void OnWriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("MethodTarget");

            // Write the base contents in...
            base.OnWriteXml(writer);

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
            if (templates != null && templates.Count != 0)
            {
                writer.WriteStartElement("Templates");

                for (int i = 0; i < templates.Count; i++)
                {
                    writer.WriteStartElement("Template");
                    writer.WriteAttributeString("value", templates[i]);
                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
            if (returnType != null)
            {
                returnType.WriteXml(writer);
            }

            writer.WriteEndElement();
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

            if (String.Equals(reader.Name, "MethodTarget",
                StringComparison.OrdinalIgnoreCase))
            {
                this.OnReadXml(reader);
            }
            else
            {
                XmlNodeType nodeType = XmlNodeType.None;
                while (reader.Read())
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element)
                    {
                        if (String.Equals(reader.Name, "MethodTarget",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            this.OnReadXml(reader);

                            if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "MethodTarget",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.EndElement)
                    {
                        if (String.Equals(reader.Name, "MethodTarget",
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

            this.OnWriteXml(writer);
        }

        #endregion
    }

    [Serializable]
    public sealed class Parameter : IXmlSerializable
    {
        #region Private Fields

        private string name;
        private TypeReference type;

        #endregion

        #region Constructors and Destructor

        internal Parameter()
        {
            name = String.Empty;
        }

        public Parameter(string name, TypeReference type)
        {
            this.name = name;
            this.type = type;
        }

        #endregion

        #region Public Properties

        public string Name
        {
            get
            {
                return name;
            }
        }

        public TypeReference Type
        {
            get
            {
                return type;
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
            if (reader == null)
            {
                return;
            }

            if (String.Equals(reader.Name, "Parameter",
                StringComparison.OrdinalIgnoreCase))
            {
                name = reader.GetAttribute("name");
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;

                if (nodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "Parameter",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        name = reader.GetAttribute("name");
                    }
                    else if (reader.Name.EndsWith("TypeReference",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        type = ReferencesReader.ReadTypeReference(reader);
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, "Parameter",
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

            writer.WriteStartElement("Parameter");
            writer.WriteAttributeString("name", name);

            if (type != null)
            {
                type.WriteXml(writer);
            }

            writer.WriteEndElement();
        }

        #endregion
    }
}
