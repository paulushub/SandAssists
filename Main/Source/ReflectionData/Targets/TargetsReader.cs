// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Sandcastle.ReflectionData.Targets
{
    public static class TargetsReader
    {
        private static XmlReaderSettings _settings;

        public static Target ReadXml(string targetText)
        {
            if (String.IsNullOrEmpty(targetText))
            {
                return null;
            }
            if (_settings == null)
            {
                _settings = new XmlReaderSettings();
                _settings.CloseInput = true;
                _settings.ConformanceLevel = ConformanceLevel.Fragment;
            }

            Target target = null;

            StringReader textReader = new StringReader(targetText);
            XmlReader reader = XmlReader.Create(textReader, _settings);
            reader.MoveToContent();
            target = TargetsReader.ReadTarget(reader);
            reader.Close();

            return target;
        }

        public static Target ReadTarget(XmlReader reader)
        {
            if (reader == null || reader.NodeType != XmlNodeType.Element)
            {
                return null;
            }

            switch (reader.Name)
            {
                case "Target":
                    Target target = new Target();
                    target.ReadXml(reader);
                    return target;
                case "NamespaceTarget":
                    NamespaceTarget namespaceTarget = new NamespaceTarget();
                    namespaceTarget.ReadXml(reader);
                    return namespaceTarget;
                case "TypeTarget":
                    TypeTarget typeTarget = new TypeTarget();
                    typeTarget.ReadXml(reader);
                    return typeTarget;
                case "EnumerationTarget":
                    EnumerationTarget enumTarget = new EnumerationTarget();
                    enumTarget.ReadXml(reader);
                    return enumTarget;
                case "MemberTarget":
                    MemberTarget memberTarget = new MemberTarget();
                    memberTarget.ReadXml(reader);
                    return memberTarget;
                case "ConstructorTarget":
                    ConstructorTarget constructorTarget = new ConstructorTarget();
                    constructorTarget.ReadXml(reader);
                    return constructorTarget;
                case "ProcedureTarget":
                    ProcedureTarget procedTarget = new ProcedureTarget();
                    procedTarget.ReadXml(reader);
                    return procedTarget;
                case "EventTarget":
                    EventTarget eventTarget = new EventTarget();
                    eventTarget.ReadXml(reader);
                    return eventTarget;
                case "PropertyTarget":
                    PropertyTarget propertyTarget = new PropertyTarget();
                    propertyTarget.ReadXml(reader);
                    return propertyTarget;
                case "MethodTarget":
                    MethodTarget methodTarget = new MethodTarget();
                    methodTarget.ReadXml(reader);
                    return methodTarget;
            }

            return null;
        }

        public static MemberTarget ReadMemberTarget(XmlReader reader)
        {
            if (reader == null || reader.NodeType != XmlNodeType.Element)
            {
                return null;
            }

            switch (reader.Name)
            {
                case "MemberTarget":
                    MemberTarget memberTarget = new MemberTarget();
                    memberTarget.ReadXml(reader);
                    return memberTarget;
                case "ConstructorTarget":
                    ConstructorTarget constructorTarget = new ConstructorTarget();
                    constructorTarget.ReadXml(reader);
                    return constructorTarget;
                case "ProcedureTarget":
                    ProcedureTarget procedTarget = new ProcedureTarget();
                    procedTarget.ReadXml(reader);
                    return procedTarget;
                case "EventTarget":
                    EventTarget eventTarget = new EventTarget();
                    eventTarget.ReadXml(reader);
                    return eventTarget;
                case "PropertyTarget":
                    PropertyTarget propertyTarget = new PropertyTarget();
                    propertyTarget.ReadXml(reader);
                    return propertyTarget;
                case "MethodTarget":
                    MethodTarget methodTarget = new MethodTarget();
                    methodTarget.ReadXml(reader);
                    return methodTarget;
            }

            return null;
        }

        public static TypeTarget ReadTypeTarget(XmlReader reader)
        {
            if (reader == null || reader.NodeType != XmlNodeType.Element)
            {
                return null;
            }

            switch (reader.Name)
            {
                case "TypeTarget":
                    TypeTarget typeTarget = new TypeTarget();
                    typeTarget.ReadXml(reader);
                    return typeTarget;
                case "EnumerationTarget":
                    EnumerationTarget enumTarget = new EnumerationTarget();
                    enumTarget.ReadXml(reader);
                    return enumTarget;
            }

            return null;
        }

        public static ProcedureTarget ReadProcedureTarget(XmlReader reader)
        {
            if (reader == null || reader.NodeType != XmlNodeType.Element)
            {
                return null;
            }

            switch (reader.Name)
            {
                case "ProcedureTarget":
                    ProcedureTarget procedTarget = new ProcedureTarget();
                    procedTarget.ReadXml(reader);
                    return procedTarget;
                case "EventTarget":
                    EventTarget eventTarget = new EventTarget();
                    eventTarget.ReadXml(reader);
                    return eventTarget;
                case "PropertyTarget":
                    PropertyTarget propertyTarget = new PropertyTarget();
                    propertyTarget.ReadXml(reader);
                    return propertyTarget;
                case "MethodTarget":
                    MethodTarget methodTarget = new MethodTarget();
                    methodTarget.ReadXml(reader);
                    return methodTarget;
            }

            return null;
        }
    }
}
