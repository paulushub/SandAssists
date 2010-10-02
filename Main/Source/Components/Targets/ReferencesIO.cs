using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Sandcastle.Components.Targets
{
    public static class ReferencesReader
    {
        private static XmlReaderSettings _settings;

        public static Reference Read(string referenceText)
        {
            if (String.IsNullOrEmpty(referenceText))
            {
                return null;
            }
            if (_settings == null)
            {
                _settings = new XmlReaderSettings();
                _settings.ConformanceLevel = ConformanceLevel.Fragment;
            }

            Reference reference = null;

            StringReader textReader = new StringReader(referenceText);
            using (XmlReader reader = XmlReader.Create(textReader, _settings))
            {
                reader.MoveToContent();
                Debug.Assert(reader.NodeType == XmlNodeType.Element);
                reference = ReferencesReader.ReadReference(reader);
            }

            return reference;
        }

        public static Reference ReadReference(XmlReader reader)
        {
            if (reader == null || reader.NodeType != XmlNodeType.Element)
            {
                return null;
            }

            switch (reader.Name)
            {
                case "InvalidReference":
                    InvalidReference invalidReference = new InvalidReference();
                    invalidReference.ReadXml(reader);
                    return invalidReference;
                case "NamespaceReference":
                    NamespaceReference namespaceReference = new NamespaceReference();
                    namespaceReference.ReadXml(reader);
                    return namespaceReference;
                case "ExtensionMethodReference":
                    ExtensionMethodReference extensionMethodReference = new ExtensionMethodReference();
                    extensionMethodReference.ReadXml(reader);
                    return extensionMethodReference;

                // For the MemberReference(s)...
                case "SimpleMemberReference":
                    SimpleMemberReference simpleMemberReference = new SimpleMemberReference();
                    simpleMemberReference.ReadXml(reader);
                    return simpleMemberReference;
                case "SpecializedMemberReference":
                    SpecializedMemberReference specializedMemberReference = new SpecializedMemberReference();
                    specializedMemberReference.ReadXml(reader);
                    return specializedMemberReference;
                case "SpecializedMemberWithParametersReference":
                    SpecializedMemberWithParametersReference specializedMemberWithParametersReference
                        = new SpecializedMemberWithParametersReference();
                    specializedMemberWithParametersReference.ReadXml(reader);
                    return specializedMemberWithParametersReference;

                // For the TypeReference(s)...
                case "SimpleTypeReference":
                    SimpleTypeReference simpleTypeReference = new SimpleTypeReference();
                    simpleTypeReference.ReadXml(reader);
                    return simpleTypeReference;
                case "SpecializedTypeReference":
                    SpecializedTypeReference specializedTypeReference = new SpecializedTypeReference();
                    specializedTypeReference.ReadXml(reader);
                    return specializedTypeReference;
                case "ArrayTypeReference":
                    ArrayTypeReference arrayTypeReference = new ArrayTypeReference();
                    arrayTypeReference.ReadXml(reader);
                    return arrayTypeReference;
                case "ReferenceTypeReference":
                    ReferenceTypeReference referenceTypeReference = new ReferenceTypeReference();
                    referenceTypeReference.ReadXml(reader);
                    return referenceTypeReference;
                case "PointerTypeReference":
                    PointerTypeReference pointerTypeReference = new PointerTypeReference();
                    pointerTypeReference.ReadXml(reader);
                    return pointerTypeReference;
                // For the TemplateTypeReference(s)...
                case "IndexedTemplateTypeReference":
                    IndexedTemplateTypeReference indexedTemplateTypeReference = 
                        new IndexedTemplateTypeReference();
                    indexedTemplateTypeReference.ReadXml(reader);
                    return indexedTemplateTypeReference;
                case "NamedTemplateTypeReference":
                    NamedTemplateTypeReference namedTemplateTypeReference = new NamedTemplateTypeReference();
                    namedTemplateTypeReference.ReadXml(reader);
                    return namedTemplateTypeReference;
                case "TypeTemplateTypeReference":
                    TypeTemplateTypeReference typeTemplateTypeReference = new TypeTemplateTypeReference();
                    typeTemplateTypeReference.ReadXml(reader);
                    return typeTemplateTypeReference;
                case "MethodTemplateTypeReference":
                    MethodTemplateTypeReference methodTemplateTypeReference = new MethodTemplateTypeReference();
                    methodTemplateTypeReference.ReadXml(reader);
                    return methodTemplateTypeReference;
            }

            return null;
        }

        public static TypeReference ReadTypeReference(XmlReader reader)
        {
            if (reader == null || reader.NodeType != XmlNodeType.Element)
            {
                return null;
            }

            switch (reader.Name)
            {
                // For the TypeReference(s)...
                case "SimpleTypeReference":
                    SimpleTypeReference simpleTypeReference = new SimpleTypeReference();
                    simpleTypeReference.ReadXml(reader);
                    return simpleTypeReference;
                case "SpecializedTypeReference":
                    SpecializedTypeReference specializedTypeReference = new SpecializedTypeReference();
                    specializedTypeReference.ReadXml(reader);
                    return specializedTypeReference;
                case "ArrayTypeReference":
                    ArrayTypeReference arrayTypeReference = new ArrayTypeReference();
                    arrayTypeReference.ReadXml(reader);
                    return arrayTypeReference;
                case "ReferenceTypeReference":
                    ReferenceTypeReference referenceTypeReference = new ReferenceTypeReference();
                    referenceTypeReference.ReadXml(reader);
                    return referenceTypeReference;
                case "PointerTypeReference":
                    PointerTypeReference pointerTypeReference = new PointerTypeReference();
                    pointerTypeReference.ReadXml(reader);
                    return pointerTypeReference;
                // For the TemplateTypeReference(s)...
                case "IndexedTemplateTypeReference":
                    IndexedTemplateTypeReference indexedTemplateTypeReference =
                        new IndexedTemplateTypeReference();
                    indexedTemplateTypeReference.ReadXml(reader);
                    return indexedTemplateTypeReference;
                case "NamedTemplateTypeReference":
                    NamedTemplateTypeReference namedTemplateTypeReference = new NamedTemplateTypeReference();
                    namedTemplateTypeReference.ReadXml(reader);
                    return namedTemplateTypeReference;
                case "TypeTemplateTypeReference":
                    TypeTemplateTypeReference typeTemplateTypeReference = new TypeTemplateTypeReference();
                    typeTemplateTypeReference.ReadXml(reader);
                    return typeTemplateTypeReference;
                case "MethodTemplateTypeReference":
                    MethodTemplateTypeReference methodTemplateTypeReference = new MethodTemplateTypeReference();
                    methodTemplateTypeReference.ReadXml(reader);
                    return methodTemplateTypeReference;
            }

            return null;
        }

        public static MemberReference ReadMemberReference(XmlReader reader)
        {
            if (reader == null || reader.NodeType != XmlNodeType.Element)
            {
                return null;
            }

            switch (reader.Name)
            {
                // For the MemberReference(s)...
                case "SimpleMemberReference":
                    SimpleMemberReference simpleMemberReference = new SimpleMemberReference();
                    simpleMemberReference.ReadXml(reader);
                    return simpleMemberReference;
                case "SpecializedMemberReference":
                    SpecializedMemberReference specializedMemberReference = new SpecializedMemberReference();
                    specializedMemberReference.ReadXml(reader);
                    return specializedMemberReference;
                case "SpecializedMemberWithParametersReference":
                    SpecializedMemberWithParametersReference specializedMemberWithParametersReference
                        = new SpecializedMemberWithParametersReference();
                    specializedMemberWithParametersReference.ReadXml(reader);
                    return specializedMemberWithParametersReference;
            }

            return null;
        }
    }

    public static class ReferencesWriter
    {
        private static XmlWriterSettings _settings;

        public static string Write(Reference reference)
        {
            if (reference == null)
            {
                return String.Empty;
            }

            if (_settings == null)
            {
                _settings = new XmlWriterSettings();
                _settings.Indent             = false;
                _settings.OmitXmlDeclaration = true;
                _settings.ConformanceLevel   = ConformanceLevel.Fragment;
            }

            StringBuilder builder   = new StringBuilder();
            StringWriter textWriter = new StringWriter(builder);

            using (XmlWriter writer = XmlWriter.Create(textWriter, _settings))
            {
                reference.WriteXml(writer);
            }

            return builder.ToString();
        }
    }
}
