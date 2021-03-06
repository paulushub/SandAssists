// Copyright (c) Microsoft Corporation.  All rights reserved.
//

using System;
using System.Collections.Generic;

using System.Compiler;

namespace Microsoft.Ddue.Tools.Reflection {

    public static class ReflectionUtilities {

        public static Event[] GetImplementedEvents(Event trigger) {
            List < Event > list = new List < Event >();

            // get the adder
            Method adder = trigger.HandlerAdder;

            // get interface methods corresponding to this adder
            Method[] implementedAdders = GetImplementedMethods(adder);

            // get the events corresponding to the implemented adders
            foreach (Method implementedAdder in implementedAdders) {
                Event implementedTrigger = GetEventFromAdder(implementedAdder);
                if (implementedTrigger != null) list.Add(implementedTrigger);
            }

            return (list.ToArray());

        }

        public static Method[] GetImplementedMethods(Method method) {
            List < Method > list = new List < Method >();

            // Explicit implementations
            MethodList explicitImplementations = method.ImplementedInterfaceMethods;
            if (explicitImplementations != null) {
                for (int i = 0; i < explicitImplementations.Count; i++) {
                    Method explicitImplementation = explicitImplementations[i];
                    list.Add(explicitImplementation);
                }
            }

            // Implicit implementations
            MethodList implicitImplementations = method.ImplicitlyImplementedInterfaceMethods;
            if (implicitImplementations != null) {
                for (int i = 0; i < implicitImplementations.Count; i++) {
                    Method implicitImplementation = implicitImplementations[i];
                    list.Add(implicitImplementation);
                }
            }

            return (list.ToArray());
        }

        public static Property[] GetImplementedProperties(Property property) {
            List < Property > list = new List < Property >();

            // get an accessor
            Method accessor = property.Getter;
            if (accessor == null) accessor = property.Setter;
            if (accessor == null) return (new Property[0]);

            // get the interface methods corresponding to this accessor
            Method[] methods = GetImplementedMethods(accessor);

            // look for properties corresponding to these methods
            for (int i = 0; i < methods.Length; i++) {
                Method method = methods[i];
                Property entry = GetPropertyFromAccessor(method);
                if (entry != null) list.Add(entry);
            }

            return (list.ToArray());
        }

        public static Namespace GetNamespace(TypeNode type) {
            if (type.DeclaringType != null) {
                return (GetNamespace(type.DeclaringType));
            } else {
                return (new Namespace(type.Namespace));
            }
        }

        public static Member GetTemplateMember(Member member) {

            if (member == null) throw new ArgumentNullException("member");

            // if the containing type isn't generic, the member is the template member
            TypeNode type = member.DeclaringType;
            if (!type.IsGeneric) return (member);

            // if the containing type isn't specialized, the member is the template member
            if (!IsSpecialized(type)) return (member);

            // get the template type, and look for members with the same name
            TypeNode template = ReflectionUtilities.GetTemplateType(member.DeclaringType);
            Identifier name = member.Name;
            MemberList candidates = template.GetMembersNamed(name);

            // if no candidates, say so (this shouldn't happen)
            if (candidates.Count == 0) throw new InvalidOperationException("No members in the template had the name found in the specialization. This is not possible, but apparently it happened.");

            // if only one candidate, return it
            if (candidates.Count == 1) return (candidates[0]);

            // multiple candidates, so now we need to compare parameters
            ParameterList parameters = GetParameters(member);

            for (int i = 0; i < candidates.Count; i++) {
                Member candidate = candidates[i];

                // candidate must be same kind of node
                if (candidate.NodeType != member.NodeType) continue;

                // if parameters match, this is the one
                if (ParametersMatch(parameters, GetParameters(candidate))) return (candidate);

            }

            Console.WriteLine(member.DeclaringType.FullName);
            Console.WriteLine(member.FullName);
            throw new InvalidOperationException("No members in the template matched the parameters of the specialization. This is not possible.");
        }

        public static TypeNode GetTemplateType(TypeNode type) {

            if (type == null) throw new ArgumentNullException("type");
            // Console.WriteLine(type.FullName);

            // only generic types have templates
            if (!type.IsGeneric) return (type);

            if (type.DeclaringType == null) {
                // if the type is not nested, life is simpler

                // if the type is not specified, the type is the template
                if (type.TemplateArguments == null) return (type);

                // otherwise, construct the template type identifier and use it to fetch the template type
                Module templateModule = type.DeclaringModule;
                Identifier name = new Identifier(String.Format("{0}`{1}", type.GetUnmangledNameWithoutTypeParameters(), type.TemplateArguments.Count));
                Identifier space = type.Namespace;
                TypeNode template = templateModule.GetType(space, name);
                return (template);
            } else {
                // if the type is nested, life is harder; we have to walk up the chain, constructing
                // un-specialized identifiers as we go, then walk back down the chain, fetching
                // template types as we go

                // create a stack to keep track of identifiers
                Stack < Identifier > identifiers = new Stack < Identifier >();

                // populate the stack with the identifiers of all the types up to the outermost type
                TypeNode current = type;
                while (true) {
                    int count = 0;
                    if ((current.TemplateArguments != null) && (current.TemplateArguments.Count > count)) count = current.TemplateArguments.Count;
                    if ((current.TemplateParameters != null) && (current.TemplateParameters.Count > count)) count = current.TemplateParameters.Count;
                    TypeNodeList arguments = current.TemplateParameters;
                    if (count == 0) {
                        identifiers.Push(new Identifier(current.GetUnmangledNameWithoutTypeParameters()));
                    } else {
                        identifiers.Push(new Identifier(String.Format("{0}`{1}", current.GetUnmangledNameWithoutTypeParameters(), count)));
                    }
                    // Console.WriteLine("U {0} {1}", identifiers.Peek(), CountArguments(current));
                    if (current.DeclaringType == null) break;
                    current = current.DeclaringType;
                }

                // fetch a TypeNode representing that outermost type
                Module module = current.DeclaringModule;
                Identifier space = current.Namespace;
                current = module.GetType(space, identifiers.Pop());

                // move down the stack to the inner type we want
                while (identifiers.Count > 0) {
                    current = (TypeNode)current.GetMembersNamed(identifiers.Pop())[0];
                    // Console.WriteLine("D {0} {1}", current.GetFullUnmangledNameWithTypeParameters(), CountArguments(current));
                }

                // whew, finally we've got it
                return (current);
            }

        }

        public static bool IsDefaultMember(Member member) {

            if (member == null) throw new ArgumentNullException("member");
            TypeNode type = member.DeclaringType;

            MemberList defaultMembers = type.DefaultMembers;
            for (int i = 0; i < defaultMembers.Count; i++) {
                Member defaultMember = defaultMembers[i];
                if (member == defaultMember) return (true);
            }
            return (false);

        }

        private static Event GetEventFromAdder(Method adder) {
            if (adder == null) throw new ArgumentNullException("adder");
            TypeNode type = adder.DeclaringType;
            MemberList members = type.Members;
            foreach (Member member in members) {
                if (member.NodeType != NodeType.Event) continue;
                Event trigger = member as Event;
                if (trigger.HandlerAdder == adder) return (trigger);
            }
            return (null);
        }

        private static ParameterList GetParameters(Member member) {
            Method method = member as Method;
            if (method != null) return (method.Parameters);

            Property property = member as Property;
            if (property != null) return (property.Parameters);

            return (new ParameterList());
        }

        private static Property GetPropertyFromAccessor(Method accessor) {
            if (accessor == null) throw new ArgumentNullException("accessor");
            TypeNode type = accessor.DeclaringType;
            MemberList members = type.Members;
            foreach (Member member in members) {
                if (member.NodeType != NodeType.Property) continue;
                Property property = member as Property;
                if (property.Getter == accessor) return (property);
                if (property.Setter == accessor) return (property);
            }
            return (null);
        }

        private static bool IsSpecialized(TypeNode type) {
            for (TypeNode t = type; t != null; t = t.DeclaringType) {
                TypeNodeList templates = t.TemplateArguments;
                if ((templates != null) && (templates.Count > 0)) return (true);
            }
            return (false);
        }

        // parameters1 should be fully specialized; parameters2 

        private static bool ParametersMatch(ParameterList parameters1, ParameterList parameters2) {

            if (parameters1.Count != parameters2.Count) return (false);

            for (int i = 0; i < parameters1.Count; i++) {
                TypeNode type1 = parameters1[i].Type;
                TypeNode type2 = parameters2[i].Type;

                // we can't determine the equivilence of template parameters; this is probably not good
                if (type1.IsTemplateParameter || type2.IsTemplateParameter) continue;

                // the node type must be the same; this is probably a fast check
                if (type1.NodeType != type2.NodeType) return (false);

                // if they are "normal" types, we will compare them
                // comparing arrays, pointers, etc. is dangerous, because the types they contian may be template parameters
                if ((type1.NodeType == NodeType.Class) || (type1.NodeType == NodeType.Struct) || (type1.NodeType == NodeType.Interface) ||
                    (type1.NodeType == NodeType.EnumNode) || (type1.NodeType == NodeType.DelegateNode)) {
                    type1 = GetTemplateType(type1);
                    type2 = GetTemplateType(type2);
                    if (!type2.IsStructurallyEquivalentTo(type1)) {
                        // Console.WriteLine("{0} !~ {1}", type1.FullName, type2.FullName);
                        return (false);
                    } else {
                        // Console.WriteLine("{0} ~ {1}", type1.FullName, type2.FullName);
                    }
                }

            }

            return (true);
        }

        private static bool TypeMatch(TypeNode type1, TypeNode type2) {

            // the two types must be of the same kind
            if (type1.NodeType != type2.NodeType) return (false);

            if (type1.NodeType == NodeType.ArrayType) {
                // they are arrays, so check elements
                ArrayType array1 = (ArrayType)type1;
                ArrayType array2 = (ArrayType)type2;
                return (TypeMatch(array1.ElementType, array2.ElementType));
            } else {
                // they are normal types
                return (type1.IsStructurallyEquivalentTo(type2));
            }
        }

    }

}
