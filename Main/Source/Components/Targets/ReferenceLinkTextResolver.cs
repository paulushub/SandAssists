// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Sandcastle.Components.Targets
{
    // ***** Link text writing logic *****

    [Flags]
    public enum ReferenceLinkDisplayOptions
    {
        ShowContainer  = 1,
        ShowTemplates  = 2,
        ShowParameters = 4,

        Default        = 6
    }

    public sealed class ReferenceLinkTextResolver
    {
        #region Private Fields

        private TargetCollection _targets;

        #endregion

        #region Constructors and Destructor

        public ReferenceLinkTextResolver(TargetCollection targets)
        {
            _targets = targets;
        }

        #endregion

        #region Public Methods

        public void WriteTarget(Target target, ReferenceLinkDisplayOptions options, XmlWriter writer)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (writer == null)
                throw new ArgumentNullException("writer");

            switch (target.TargetType)
            {
                case TargetType.Namespace:
                    WriteNamespaceTarget((NamespaceTarget)target, options, writer);
                    return;
                // For the TypeTarget...
                case TargetType.Type:
                case TargetType.Enumeration:
                    WriteTypeTarget((TypeTarget)target, options, writer);
                    return;
                // For the MemberTarget...
                case TargetType.Member:
                case TargetType.Constructor:
                case TargetType.Procedure:
                case TargetType.Event:
                case TargetType.Property:
                case TargetType.Method:
                    WriteMemberTarget((MemberTarget)target, options, writer);
                    return;
            }

            throw new InvalidOperationException();
        }

        public void WriteNamespaceTarget(NamespaceTarget space, ReferenceLinkDisplayOptions options, XmlWriter writer)
        {
            if (space == null)
                throw new ArgumentNullException("target");
            if (writer == null)
                throw new ArgumentNullException("writer");

            writer.WriteString(space.Name);
        }

        public void WriteTypeTarget(TypeTarget type, ReferenceLinkDisplayOptions options, XmlWriter writer)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (writer == null)
                throw new ArgumentNullException("writer");

            WriteTypeTarget(type, options, true, writer);
        }

        public void WriteMemberTarget(MemberTarget target, ReferenceLinkDisplayOptions options, XmlWriter writer)
        {
            WriteMemberTarget(target, options, writer, null);
        }

        public void WriteReference(Reference reference, ReferenceLinkDisplayOptions options, XmlWriter writer)
        {
            if (reference == null)
                throw new ArgumentNullException("reference");
            if (writer == null)
                throw new ArgumentNullException("writer");

            NamespaceReference space = reference as NamespaceReference;
            if (space != null)
            {
                WriteNamespace(space, options, writer);
                return;
            }

            TypeReference type = reference as TypeReference;
            if (type != null)
            {
                WriteType(type, options, writer);
                return;
            }

            MemberReference member = reference as MemberReference;
            if (member != null)
            {
                WriteMember(member, options, writer);
                return;
            }

            ExtensionMethodReference extMethod = reference as ExtensionMethodReference;
            if (extMethod != null)
            {
                WriteExtensionMethod(extMethod, options, writer);
                return;
            }

            InvalidReference invalid = reference as InvalidReference;
            if (invalid != null)
            {
                WriteInvalid(invalid, options, writer);
                return;
            }

            throw new InvalidOperationException();
        }

        public void WriteNamespace(NamespaceReference spaceReference, ReferenceLinkDisplayOptions options,
            XmlWriter writer)
        {
            if (spaceReference == null)
                throw new ArgumentNullException("spaceReference");
            if (writer == null)
                throw new ArgumentNullException("writer");

            NamespaceTarget spaceTarget = spaceReference.Resolve(_targets) as NamespaceTarget;
            if (spaceTarget != null)
            {
                WriteNamespaceTarget(spaceTarget, options, writer);
            }
            else
            {
                ReferenceTextUtilities.WriteNamespaceReference(spaceReference, options, writer);
            }
        }

        public void WriteType(TypeReference type, ReferenceLinkDisplayOptions options, XmlWriter writer)
        {
            WriteType(type, options, writer, null);
        }

        public void WriteSimpleType(SimpleTypeReference simple, ReferenceLinkDisplayOptions options, XmlWriter writer)
        {
            WriteSimpleType(simple, options, true, writer);
        }

        public void WriteExtensionMethod(ExtensionMethodReference extMethod, ReferenceLinkDisplayOptions options,
            XmlWriter writer)
        {
            if (extMethod == null)
                throw new ArgumentNullException("extMethod");
            if (writer == null)
                throw new ArgumentNullException("writer");

            // write the unqualified method name
            writer.WriteString(extMethod.Name);

            // if this is a generic method, write any template params or args
            if (extMethod.TemplateArgs != null && extMethod.TemplateArgs.Count > 0)
            {
                WriteTemplateArguments(extMethod.TemplateArgs, writer);
            }

            // write parameters
            if ((options & ReferenceLinkDisplayOptions.ShowParameters) > 0)
            {
                IList<Parameter> parameters = extMethod.Parameters;
                WriteMethodParameters(extMethod.Parameters, writer);
            }
        }

        public void WriteMember(MemberReference member, ReferenceLinkDisplayOptions options, XmlWriter writer)
        {
            if (member == null)
                throw new ArgumentNullException("member");
            if (writer == null)
                throw new ArgumentNullException("writer");

            switch (member.ReferenceType)
            {
                case ReferenceType.SimpleMember:
                    WriteSimpleMember((SimpleMemberReference)member, options, writer);
                    return;
                case ReferenceType.SpecializedMember:
                    WriteSpecializedMember((SpecializedMemberReference)member, options, writer);
                    return;
                case ReferenceType.SpecializedMemberWithParameters:
                    WriteSpecializedMemberWithParameters((SpecializedMemberWithParametersReference)member,
                        options, writer);
                    return;
            }

            throw new InvalidOperationException();
        }

        #endregion

        #region Private Methods

        private void WriteTypeTarget(TypeTarget type, ReferenceLinkDisplayOptions options, bool showOuterType,
            XmlWriter writer)
        {
            // write namespace, if containers are requested
            if ((options & ReferenceLinkDisplayOptions.ShowContainer) > 0)
            {
                WriteNamespace(type.Namespace, ReferenceLinkDisplayOptions.Default, writer);
                WriteSeperator(writer);
            }

            // write outer type, if one exists
            if (showOuterType && (type.OuterType != null))
            {
                WriteSimpleType(type.OuterType, ReferenceLinkDisplayOptions.Default, writer);
                WriteSeperator(writer);
            }

            // write the type name
            writer.WriteString(type.Name);

            // write if template parameters, if they exist and we are requested
            if ((options & ReferenceLinkDisplayOptions.ShowTemplates) > 0)
            {
                WriteTemplateParameters(type.Templates, writer);
            }
        }

        private void WriteMemberTarget(MemberTarget target, ReferenceLinkDisplayOptions options, XmlWriter writer,
            IDictionary<IndexedTemplateTypeReference, TypeReference> dictionary)
        {
            if (target == null)
                throw new ArgumentNullException("target");
            if (writer == null)
                throw new ArgumentNullException("writer");

            if ((options & ReferenceLinkDisplayOptions.ShowContainer) > 0)
            {
                TypeReference type = target.Type;
                WriteType(type, options & ~ReferenceLinkDisplayOptions.ShowContainer, writer);
                if (target.TargetType == TargetType.Method)
                {
                    MethodTarget methodTarget = (MethodTarget)target;
                    if (methodTarget.conversionOperator)
                    {
                        writer.WriteString(" ");
                    }
                    else
                    {
                        WriteSeperator(writer);
                    }
                }
                else
                {
                    WriteSeperator(writer);
                }
            }

            switch (target.TargetType)
            {
                case TargetType.Method:
                    // special logic for writing methods
                    WriteMethod((MethodTarget)target, options, writer, dictionary);
                    return;
                case TargetType.Property:
                    // special logic for writing properties
                    WriteProperty((PropertyTarget)target, options, writer);
                    return;
                case TargetType.Constructor:
                    // special logic for writing constructors
                    WriteConstructor((ConstructorTarget)target, options, writer);
                    return;
                case TargetType.Event:
                    // special logic for writing events
                    WriteEvent((EventTarget)target, options, writer);
                    return;
            }

            // by default, just write name
            writer.WriteString(target.Name);
        }

        private void WriteType(TypeReference type, ReferenceLinkDisplayOptions options, XmlWriter writer,
            IDictionary<IndexedTemplateTypeReference, TypeReference> dictionary)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (writer == null)
                throw new ArgumentNullException("writer");

            switch (type.ReferenceType)
            {
                case ReferenceType.SimpleType:
                    WriteSimpleType((SimpleTypeReference)type, options, writer);
                    return;
                case ReferenceType.SpecializedType:
                    WriteSpecializedType((SpecializedTypeReference)type, options, writer);
                    return;
                case ReferenceType.ArrayType:
                    WriteArrayType((ArrayTypeReference)type, options, writer, dictionary);
                    return;
                case ReferenceType.ReferenceType:
                    WriteReferenceType((ReferenceTypeReference)type, options, writer, dictionary);
                    return;
                case ReferenceType.PointerType:
                    WritePointerType((PointerTypeReference)type, options, writer, dictionary);
                    return;
                case ReferenceType.TypeTemplate:
                case ReferenceType.IndexedTemplate:
                case ReferenceType.NamedTemplate:
                case ReferenceType.MethodTemplate:
                    WriteTemplateType((TemplateTypeReference)type, options, writer, dictionary);
                    return;
            }

            throw new InvalidOperationException("Unknown type reference type");
        }

        private void WriteSimpleType(SimpleTypeReference simple, ReferenceLinkDisplayOptions options,
            bool showOuterType, XmlWriter writer)
        {
            TypeTarget type = simple.Resolve(_targets) as TypeTarget;
            if (type != null)
            {
                WriteTypeTarget(type, options, showOuterType, writer);
            }
            else
            {
                ReferenceTextUtilities.WriteSimpleTypeReference(simple, options, writer);
            }
        }

        private static void WriteTemplateParameters(IList<string> templates, XmlWriter writer)
        {
            if (templates.Count == 0)
                return;

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "languageSpecificText");
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "cs");
            writer.WriteString("<");
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "vb");
            writer.WriteString("(Of ");
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "cpp");
            writer.WriteString("<");
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "nu");
            writer.WriteString("(");
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "fs");
            writer.WriteString("<'");
            writer.WriteEndElement();

            writer.WriteEndElement();

            for (int i = 0; i < templates.Count; i++)
            {
                if (i > 0) writer.WriteString(", ");
                writer.WriteString(templates[i]);
            }

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "languageSpecificText");
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "cs");
            writer.WriteString(">");
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "vb");
            writer.WriteString(")");
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "cpp");
            writer.WriteString(">");
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "nu");
            writer.WriteString(")");
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "fs");
            writer.WriteString(">");
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        private void WriteSpecializedType(SpecializedTypeReference special, 
            ReferenceLinkDisplayOptions options, XmlWriter writer)
        {
            IList<Specialization> specializations = special.Specializations;
            for (int i = 0; i < specializations.Count; i++)
            {
                if (i == 0)
                {
                    WriteSpecialization(specializations[0], options, writer);
                }
                else
                {
                    WriteSeperator(writer);
                    WriteSpecialization(specializations[i], options & ~ReferenceLinkDisplayOptions.ShowContainer, writer);
                }
            }
        }

        private void WriteSpecialization(Specialization specialization, 
            ReferenceLinkDisplayOptions options, XmlWriter writer)
        {
            // write the type itself (without outer types, because those will be written be other calls to this routine)
            WriteSimpleType(specialization.TemplateType, (options & ~ReferenceLinkDisplayOptions.ShowTemplates),
                false, writer);

            // then write the template arguments
            WriteTemplateArguments(specialization.arguments, writer);
        }

        private void WriteTemplateArguments(IList<TypeReference> specialization, XmlWriter writer)
        {
            if (specialization.Count == 0)
                return;

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "languageSpecificText");
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "cs");
            writer.WriteString("<");
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "vb");
            writer.WriteString("(Of ");
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "cpp");
            writer.WriteString("<");
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "fs");
            writer.WriteString("<'");
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "nu");
            writer.WriteString("(");
            writer.WriteEndElement();

            writer.WriteEndElement();

            for (int i = 0; i < specialization.Count; i++)
            {
                if (i > 0) writer.WriteString(", ");
                WriteType(specialization[i], ReferenceLinkDisplayOptions.Default, writer);
            }

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "languageSpecificText");
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "cs");
            writer.WriteString(">");
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "vb");
            writer.WriteString(")");
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "cpp");
            writer.WriteString(">");
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "fs");
            writer.WriteString(">");
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "nu");
            writer.WriteString(")");
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void WriteArrayType(ArrayTypeReference reference, ReferenceLinkDisplayOptions options,
            XmlWriter writer, IDictionary<IndexedTemplateTypeReference, TypeReference> dictionary)
        {
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "languageSpecificText");
            // C++ array notation (left)
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "cpp");
            writer.WriteString("array<");
            writer.WriteEndElement();
            writer.WriteEndElement(); // end of <span class="languageSpecificText"> element

            // the underlying type
            WriteType(reference.ElementType, options, writer, dictionary);

            // C++ array notation (right)
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "languageSpecificText");
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "cpp");
            if (reference.Rank > 1)
            {
                writer.WriteString("," + reference.Rank.ToString());
            }
            writer.WriteString(">");
            writer.WriteEndElement();

            // C# array notation
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "cs");
            writer.WriteString("[");
            for (int i = 1; i < reference.Rank; i++) { writer.WriteString(","); }
            writer.WriteString("]");
            writer.WriteEndElement();

            // VB array notation
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "vb");
            writer.WriteString("(");
            for (int i = 1; i < reference.Rank; i++) { writer.WriteString(","); }
            writer.WriteString(")");
            writer.WriteEndElement();

            // neutral array notation
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "nu");
            writer.WriteString("[");
            for (int i = 1; i < reference.Rank; i++) { writer.WriteString(","); }
            writer.WriteString("]");
            writer.WriteEndElement();

            // F# array notation
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "fs");
            writer.WriteString("[");
            for (int i = 1; i < reference.Rank; i++) { writer.WriteString(","); }
            writer.WriteString("]");
            writer.WriteEndElement(); 

            writer.WriteEndElement(); // end of <span class="languageSpecificText"> element
        }

        private static void WriteSeperator(XmlWriter writer)
        {
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "languageSpecificText");
            // C# separator
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "cs");
            writer.WriteString(".");
            writer.WriteEndElement();

            // VB separator
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "vb");
            writer.WriteString(".");
            writer.WriteEndElement();

            // C++ separator
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "cpp");
            writer.WriteString("::");
            writer.WriteEndElement();

            // neutral separator
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "nu");
            writer.WriteString(".");
            writer.WriteEndElement();

            // F# separator
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "fs");
            writer.WriteString(".");
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        private void WritePointerType(PointerTypeReference pointer, ReferenceLinkDisplayOptions options,
            XmlWriter writer, IDictionary<IndexedTemplateTypeReference, TypeReference> dictionary)
        {
            WriteType(pointer.PointedToType, options, writer, dictionary);
            writer.WriteString("*");
        }

        private void WriteReferenceType(ReferenceTypeReference reference, ReferenceLinkDisplayOptions options,
            XmlWriter writer, IDictionary<IndexedTemplateTypeReference, TypeReference> dictionary)
        {
            WriteType(reference.ReferedToType, options, writer, dictionary);

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "languageSpecificText");
            // add % in C++
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "cpp");
            writer.WriteString("%");
            writer.WriteEndElement();
            writer.WriteEndElement();
        }

        private void WriteTemplateType(TemplateTypeReference template, ReferenceLinkDisplayOptions options,
            XmlWriter writer)
        {
            WriteTemplateType(template, options, writer, null);
        }

        private void WriteTemplateType(TemplateTypeReference template, ReferenceLinkDisplayOptions options,
            XmlWriter writer, IDictionary<IndexedTemplateTypeReference, TypeReference> dictionary)
        {
            switch (template.ReferenceType)
            {
                case ReferenceType.NamedTemplate:
                    // if we have the name, just write it
                    NamedTemplateTypeReference namedTemplate = (NamedTemplateTypeReference)template;
                    writer.WriteString(namedTemplate.Name);
                    return;

                case ReferenceType.IndexedTemplate:
                    IndexedTemplateTypeReference indexedTemplate = (IndexedTemplateTypeReference)template;
                    if ((dictionary != null) && (dictionary.ContainsKey(indexedTemplate)))
                    {
                        WriteType(dictionary[indexedTemplate], options, writer);
                    }
                    else
                    {
                        writer.WriteString(GetTemplateName(indexedTemplate.TemplateId, indexedTemplate.Index));
                    }
                    return;

                case ReferenceType.TypeTemplate:
                    TypeTemplateTypeReference typeTemplate = (TypeTemplateTypeReference)template;

                    TypeReference value = null;
                    if (dictionary != null)
                    {
                        IndexedTemplateTypeReference key = new IndexedTemplateTypeReference(
                            typeTemplate.TemplateType.Id, typeTemplate.Position);
                        if (dictionary.ContainsKey(key)) 
                            value = dictionary[key];
                    }

                    if (value == null)
                    {
                        writer.WriteString(GetTypeTemplateName(typeTemplate.TemplateType, 
                            typeTemplate.Position));
                    }
                    else
                    {
                        WriteType(value, options, writer);
                    }
                    return;
            }

            throw new InvalidOperationException();
        }

        private string GetTemplateName(string templateId, int position)
        {
            Target target = _targets[templateId];

            if (target == null)
            {
                return ("UTT");
            }
            else
            {
                TargetType targetType = target.TargetType;
                if (targetType == TargetType.Type || targetType == TargetType.Enumeration)
                {
                    TypeTarget type = (TypeTarget)target;
                    IList<string> templates = type.Templates;
                    if (templates.Count > position)
                    {
                        return (templates[position]);
                    }
                    else
                    {
                        return ("UTT");
                    }
                }

                if (targetType == TargetType.Method)
                {
                    MethodTarget method = (MethodTarget)target;
                    IList<string> templates = method.Templates;
                    if (templates.Count > position)
                    {
                        return (templates[position]);
                    }
                    else
                    {
                        return ("UTT");
                    }
                }

                return ("UTT");
            }
        }

        private string GetTypeTemplateName(SimpleTypeReference type, int position)
        {
            TypeTarget target = type.Resolve(_targets) as TypeTarget;
            if (target != null)
            {
                IList<string> templates = target.Templates;
                if (templates.Count > position)
                {
                    return (templates[position]);
                }
                else if (target.OuterType != null)
                {
                    return (GetTypeTemplateName(target.OuterType, position));
                }
                else
                {
                    return ("UTT");
                }
            }
            else
            {
                throw new InvalidOperationException(String.Format("Unknown type reference '{0}'", type.Id));
            }
        }

        private void WriteSpecializedMember(SpecializedMemberReference member, ReferenceLinkDisplayOptions options,
            XmlWriter writer)
        {
            if ((options & ReferenceLinkDisplayOptions.ShowContainer) > 0)
            {
                WriteType(member.SpecializedType, options & ~ReferenceLinkDisplayOptions.ShowContainer, writer);
                WriteSeperator(writer);
            }

            IDictionary<IndexedTemplateTypeReference, TypeReference> dictionary
                = member.SpecializedType.GetSpecializationDictionary();
            WriteSimpleMember(member.TemplateMember, options & ~ReferenceLinkDisplayOptions.ShowContainer,
                writer, dictionary);
        }

        private void WriteSimpleMember(SimpleMemberReference member, ReferenceLinkDisplayOptions options, XmlWriter writer)
        {
            WriteSimpleMember(member, options, writer, null);
        }

        private void WriteSimpleMember(SimpleMemberReference member, ReferenceLinkDisplayOptions options,
            XmlWriter writer, IDictionary<IndexedTemplateTypeReference, TypeReference> dictionary)
        {
            MemberTarget target = member.Resolve(_targets) as MemberTarget;
            if (target != null)
            {
                WriteMemberTarget(target, options, writer, dictionary);
            }
            else
            {
                ReferenceTextUtilities.WriteSimpleMemberReference(member, options, writer, this);
                //throw new InvalidOperationException(String.Format("Unknown member target '{0}'", member.Id));
            }
        }

        private void WriteProcedureName(ProcedureTarget target, ReferenceLinkDisplayOptions options, XmlWriter writer)
        {
            MemberReference implements = target.ExplicitlyImplements;
            if (implements == null)
            {
                if (target.conversionOperator)
                {
                    WriteConversionOperator(target, writer);
                }
                else
                {
                    writer.WriteString(target.Name);
                }
            }
            else
            {
                WriteMember(implements, ReferenceLinkDisplayOptions.ShowContainer, writer);
            }
        }

        private void WriteMethod(MethodTarget target, ReferenceLinkDisplayOptions options, XmlWriter writer,
            IDictionary<IndexedTemplateTypeReference, TypeReference> dictionary)
        {
            WriteProcedureName(target, options, writer);

            if ((options & ReferenceLinkDisplayOptions.ShowTemplates) > 0)
            {
                // if this is a generic method, write any template params or args
                if (target.TemplateArgs != null && target.TemplateArgs.Count > 0)
                {
                    WriteTemplateArguments(target.TemplateArgs, writer);
                }
            }

            if ((options & ReferenceLinkDisplayOptions.ShowParameters) > 0)
            {
                IList<Parameter> parameters = target.Parameters;

                if (target.ConversionOperator)
                {
                    TypeReference returns = target.returnType;
                    WriteConversionOperatorParameters(parameters, returns, writer, dictionary);
                }
                else
                {
                    WriteMethodParameters(parameters, writer, dictionary);
                }
            }
        }

        private void WriteConversionOperator(ProcedureTarget target, XmlWriter writer)
        {
            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "languageSpecificText");

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "cs");
            writer.WriteString(target.Name);
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "vb");
            if (target.name == "Explicit")
            {
                writer.WriteString("Narrowing");
            }
            else if (target.name == "Implicit")
            {
                writer.WriteString("Widening");
            }
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "cpp");
            writer.WriteString(target.name);
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "nu");
            writer.WriteString(target.name);
            writer.WriteEndElement();

            writer.WriteStartElement("span");
            writer.WriteAttributeString("class", "fs");
            writer.WriteString(target.name);
            writer.WriteEndElement();

            writer.WriteEndElement();
        }

        internal void WriteMethodParameters(IList<Parameter> parameters, XmlWriter writer)
        {
            WriteMethodParameters(parameters, writer, null);
        }    

        private void WriteMethodParameters(IList<Parameter> parameters, XmlWriter writer,
            IDictionary<IndexedTemplateTypeReference, TypeReference> dictionary)
        {
            if (parameters.Count > 0)
            {
                writer.WriteString("(");

                // show parameters
                // we need to deal with type template substitutions!
                for (int i = 0; i < parameters.Count; i++)
                {
                    if (i > 0) writer.WriteString(", ");
                    WriteType(parameters[i].Type, ReferenceLinkDisplayOptions.Default, writer, dictionary);
                }

                writer.WriteString(")");
            }
            else
            {
                writer.WriteStartElement("span");
                writer.WriteAttributeString("class", "languageSpecificText");
                // when there are no parameters, VB shows no parenthesis

                writer.WriteStartElement("span");
                writer.WriteAttributeString("class", "cs");
                writer.WriteString("()");
                writer.WriteEndElement();

                writer.WriteStartElement("span");
                writer.WriteAttributeString("class", "cpp");
                writer.WriteString("()");
                writer.WriteEndElement();

                writer.WriteStartElement("span");
                writer.WriteAttributeString("class", "nu");
                writer.WriteString("()");
                writer.WriteEndElement();

                writer.WriteStartElement("span");
                writer.WriteAttributeString("class", "fs");
                writer.WriteString("()");
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        private void WriteConversionOperatorParameters(IList<Parameter> parameters, TypeReference returns,
            XmlWriter writer, IDictionary<IndexedTemplateTypeReference, TypeReference> dictionary)
        {
            if (parameters.Count > 0 || returns != null)
                writer.WriteString("(");

            if (parameters.Count > 0)
                WriteType(parameters[0].Type, ReferenceLinkDisplayOptions.Default, writer, dictionary);

            if (parameters.Count > 0 && returns != null)
                writer.WriteString(" to ");

            if (returns != null)
                WriteType(returns, ReferenceLinkDisplayOptions.Default, writer, dictionary);

            if (parameters.Count > 0 || returns != null)
                writer.WriteString(")");

            if (parameters.Count == 0 && returns == null)
            {
                writer.WriteStartElement("span");
                writer.WriteAttributeString("class", "languageSpecificText");
                // when there are no parameters, VB shows no parenthesis

                writer.WriteStartElement("span");
                writer.WriteAttributeString("class", "cs");
                writer.WriteString("()");
                writer.WriteEndElement();

                writer.WriteStartElement("span");
                writer.WriteAttributeString("class", "cpp");
                writer.WriteString("()");
                writer.WriteEndElement();

                writer.WriteStartElement("span");
                writer.WriteAttributeString("class", "nu");
                writer.WriteString("()");
                writer.WriteEndElement();

                writer.WriteStartElement("span");
                writer.WriteAttributeString("class", "fs");
                writer.WriteString("()");
                writer.WriteEndElement();

                writer.WriteEndElement();
            }
        }

        private void WriteProperty(PropertyTarget target, ReferenceLinkDisplayOptions options, XmlWriter writer)
        {
            WriteProcedureName(target, options, writer);

            if ((options & ReferenceLinkDisplayOptions.ShowParameters) > 0)
            {    
                IList<Parameter> parameters = target.Parameters;

                // VB only shows parenthesis when there are parameters
                if (parameters.Count > 0)
                {
                    writer.WriteStartElement("span");
                    writer.WriteAttributeString("class", "languageSpecificText");
                    writer.WriteStartElement("span");
                    writer.WriteAttributeString("class", "cs");
                    writer.WriteString("[");
                    writer.WriteEndElement();

                    writer.WriteStartElement("span");
                    writer.WriteAttributeString("class", "vb");
                    writer.WriteString("(");
                    writer.WriteEndElement();

                    writer.WriteStartElement("span");
                    writer.WriteAttributeString("class", "cpp");
                    writer.WriteString("[");
                    writer.WriteEndElement();

                    writer.WriteStartElement("span");
                    writer.WriteAttributeString("class", "nu");
                    writer.WriteString("(");
                    writer.WriteEndElement();

                    writer.WriteStartElement("span");
                    writer.WriteAttributeString("class", "fs");
                    writer.WriteString(" ");
                    writer.WriteEndElement();

                    writer.WriteEndElement();

                    // show parameters
                    // we need to deal with type template substitutions!
                    for (int i = 0; i < parameters.Count; i++)
                    {
                        if (i > 0) writer.WriteString(", ");
                        WriteType(parameters[i].Type, ReferenceLinkDisplayOptions.Default, writer);
                    }

                    writer.WriteStartElement("span");
                    writer.WriteAttributeString("class", "languageSpecificText");
                    writer.WriteStartElement("span");
                    writer.WriteAttributeString("class", "cs");
                    writer.WriteString("]");
                    writer.WriteEndElement();

                    writer.WriteStartElement("span");
                    writer.WriteAttributeString("class", "vb");
                    writer.WriteString(")");
                    writer.WriteEndElement();

                    writer.WriteStartElement("span");
                    writer.WriteAttributeString("class", "cpp");
                    writer.WriteString("]");
                    writer.WriteEndElement();

                    writer.WriteStartElement("span");
                    writer.WriteAttributeString("class", "nu");
                    writer.WriteString(")");
                    writer.WriteEndElement();

                    writer.WriteStartElement("span");
                    writer.WriteAttributeString("class", "fs");
                    writer.WriteString(" ");
                    writer.WriteEndElement();

                    writer.WriteEndElement();
                }
            }
        }

        private void WriteEvent(EventTarget trigger, ReferenceLinkDisplayOptions options, XmlWriter writer)
        {
            WriteProcedureName(trigger, options, writer);
        }

        private void WriteConstructor(ConstructorTarget constructor, ReferenceLinkDisplayOptions options, XmlWriter writer)
        {
            WriteType(constructor.Type, options & ~ReferenceLinkDisplayOptions.ShowContainer, writer);

            if ((options & ReferenceLinkDisplayOptions.ShowParameters) > 0)
            {
                IList<Parameter> parameters = constructor.Parameters;
                WriteMethodParameters(parameters, writer);
            }  
        }

        private void WriteSpecializedMemberWithParameters(SpecializedMemberWithParametersReference ugly,
            ReferenceLinkDisplayOptions options, XmlWriter writer)
        {
            if ((options & ReferenceLinkDisplayOptions.ShowContainer) > 0)
            {
                WriteSpecializedType(ugly.SpecializedType, options & ~ReferenceLinkDisplayOptions.ShowContainer, writer);
                WriteSeperator(writer);
            }

            writer.WriteString(ugly.MemberName);

            if ((options & ReferenceLinkDisplayOptions.ShowParameters) > 0)
            {       
                writer.WriteString("(");

                IList<TypeReference> parameterTypes = ugly.ParameterTypes;
                for (int i = 0; i < parameterTypes.Count; i++)
                {
                    if (i > 0) 
                        writer.WriteString(", ");

                    WriteType(parameterTypes[i], ReferenceLinkDisplayOptions.Default, writer);
                }

                writer.WriteString(")");
            }    
        }

        private static void WriteInvalid(InvalidReference invalid, ReferenceLinkDisplayOptions options, XmlWriter writer)
        {
            writer.WriteString("[" + invalid.Id + "]");
        }

        #endregion
    }
}
