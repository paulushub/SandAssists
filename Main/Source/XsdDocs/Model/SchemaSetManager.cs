using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Schema;
using System.Linq;

namespace XsdDocumentation.Model
{
    public sealed class SchemaSetManager : Manager
    {
        private HashSet<string> _schemaDependencyFiles;
        private XmlSchemaObject[] _emptyObjectList = new XmlSchemaObject[0];
        private Dictionary<XmlSchemaObject, HashSet<XmlSchemaObject>> _objectParents;
        private Dictionary<XmlSchemaType, HashSet<XmlSchemaObject>> _typeUsages;
        private Dictionary<string, List<XmlSchemaObject>> _namespaceRootSchemas;
        private Dictionary<string, List<XmlSchemaObject>> _namespaceRootElements;
        private Dictionary<XmlSchemaAny, List<XmlSchemaObject>> _extensionElements = new Dictionary<XmlSchemaAny, List<XmlSchemaObject>>();
        private Dictionary<XmlSchemaAnyAttribute, List<XmlSchemaObject>> _extensionAttributes = new Dictionary<XmlSchemaAnyAttribute, List<XmlSchemaObject>>();

        public SchemaSetManager(Context context)
            : base(context)
        {
        }

        public XmlSchemaSet SchemaSet { get; private set; }

        public override void Initialize()
        {
            var schemaFileNames = Context.Configuration.SchemaFileNames;
            var schemaDependencyFileNames = Context.Configuration.SchemaDependencyFileNames;
            var allFileNames = schemaFileNames.Union(schemaDependencyFileNames,
                StringComparer.OrdinalIgnoreCase);

            try
            {
                SchemaSet = XmlSchemaSetBuilder.Build(allFileNames);
            }
            catch (Exception ex)
            {
                throw ExceptionBuilder.ErrorBuildingSchemaSet(ex);
            }

            _schemaDependencyFiles = new HashSet<string>(schemaDependencyFileNames, StringComparer.OrdinalIgnoreCase);

            _objectParents = new Dictionary<XmlSchemaObject, HashSet<XmlSchemaObject>>();
            var parentFinderNew = new ParentFinder(SchemaSet, _objectParents);
            parentFinderNew.Traverse(SchemaSet);

            _typeUsages = new Dictionary<XmlSchemaType, HashSet<XmlSchemaObject>>();
            var typeUsageFinder = new TypeUsageFinder(SchemaSet, _typeUsages);
            typeUsageFinder.Traverse(SchemaSet);

            var rootSchemaFinder = new NamespaceRootSchemaFinder(_schemaDependencyFiles);
            rootSchemaFinder.Traverse(SchemaSet);
            _namespaceRootSchemas = rootSchemaFinder.GetRootSchemas();

            var rootElementFinder = new NamespaceRootElementFinder(SchemaSet);
            rootElementFinder.Traverse(SchemaSet);
            _namespaceRootElements = rootElementFinder.GetRootElements();

            RemoveDependencySchemaObjects(_namespaceRootSchemas);
            RemoveDependencySchemaObjects(_namespaceRootElements);
        }

        private void RemoveDependencySchemaObjects(Dictionary<string, List<XmlSchemaObject>> namespaeRootObjects)
        {
            foreach (var pair in namespaeRootObjects)
            {
                var rootObjects = pair.Value;
                for (var i = rootObjects.Count - 1; i >= 0; i--)
                {
                    var rootObject = rootObjects[i];
                    if (IsDependencySchema(rootObject.GetSchema()))
                        rootObjects.RemoveAt(i);
                }
            }
        }

        public bool IsDependencySchema(XmlSchema schema)
        {
            return _schemaDependencyFiles.Contains(schema.GetLocalPath());
        }

        public ICollection<XmlSchemaObject> GetObjectParents(XmlSchemaObject obj)
        {
            HashSet<XmlSchemaObject> parents;
            if (!_objectParents.TryGetValue(obj, out parents))
                return _emptyObjectList;

            return parents;
        }

        public ICollection<XmlSchemaObject> GetTypeUsages(XmlSchemaType type)
        {
            HashSet<XmlSchemaObject> usages;
            if (!_typeUsages.TryGetValue(type, out usages))
                return _emptyObjectList;

            return usages;
        }

        public ICollection<string> GetNamespaces()
        {
            return _namespaceRootSchemas.Keys;
        }

        public ICollection<XmlSchemaObject> GetNamespaceRootSchemas(string targetNamespace)
        {
            List<XmlSchemaObject> rootSchemas;
            if (!_namespaceRootSchemas.TryGetValue(targetNamespace ?? String.Empty, out rootSchemas))
                return _emptyObjectList;

            return rootSchemas;
        }

        public ICollection<XmlSchemaObject> GetNamespaceRootElements(string targetNamespace)
        {
            List<XmlSchemaObject> rootElements;
            if (!_namespaceRootElements.TryGetValue(targetNamespace ?? String.Empty, out rootElements))
                return _emptyObjectList;

            return rootElements;
        }

        public List<ChildEntry> GetChildren(XmlSchemaElement element)
        {
            var childrenFinder = new ChildrenFinder(this);
            childrenFinder.Traverse(element);
            return childrenFinder.GetChildren();
        }

        public List<ChildEntry> GetChildren(XmlSchemaGroup group)
        {
            var childrenFinder = new ChildrenFinder(this);
            childrenFinder.Traverse(group);
            return childrenFinder.GetChildren();
        }

        public List<ChildEntry> GetChildren(XmlSchemaComplexType complexType)
        {
            var childrenFinder = new ChildrenFinder(this);
            childrenFinder.Traverse(complexType);
            return childrenFinder.GetChildren();
        }

        public AttributeEntries GetAttributeEntries(XmlSchemaElement element)
        {
            var attributeFinder = new AttributeFinder(this);
            attributeFinder.Traverse(element);
            return attributeFinder.GetAttributeEntries();
        }

        public AttributeEntries GetAttributeEntries(XmlSchemaAttributeGroup attributeGroup)
        {
            var attributeFinder = new AttributeFinder(this);
            attributeFinder.Traverse(attributeGroup);
            return attributeFinder.GetAttributeEntries();
        }

        public AttributeEntries GetAttributeEntries(XmlSchemaComplexType complexType)
        {
            var attributeFinder = new AttributeFinder(this);
            attributeFinder.Traverse(complexType);
            return attributeFinder.GetAttributeEntries();
        }

        public SimpleTypeStructureNode GetSimpleTypeStructure(XmlSchemaType schemaType)
        {
            var simpleTypeStructureBuilder = new SimpleTypeStructureBuilder(SchemaSet);
            simpleTypeStructureBuilder.Traverse(schemaType);
            return simpleTypeStructureBuilder.GetRoot();
        }

        public SimpleTypeStructureNode GetSimpleTypeStructure(XmlSchemaSimpleTypeContent content)
        {
            var simpleTypeStructureBuilder = new SimpleTypeStructureBuilder(SchemaSet);
            simpleTypeStructureBuilder.Traverse(content);
            return simpleTypeStructureBuilder.GetRoot();
        }

        public ICollection<XmlSchemaObject> GetExtensionElements(XmlSchemaAny parent)
        {
            List<XmlSchemaObject> elements;
            if (!_extensionElements.TryGetValue(parent, out elements))
                return _emptyObjectList;

            return elements;
        }

        public ICollection<XmlSchemaObject> GetExtensionAttributes(XmlSchemaAnyAttribute parent)
        {
            List<XmlSchemaObject> attributes;
            if (!_extensionAttributes.TryGetValue(parent, out attributes))
                return _emptyObjectList;

            return attributes;
        }

        public void RegisterExtension(XmlSchemaObject parent, XmlSchemaObject extension)
        {
            XmlSchemaElement element;
            XmlSchemaAttribute attribute;

            if (Casting.TryCast(extension, out element))
                RegisterExtensionElement(parent, element);
            else if (Casting.TryCast(extension, out attribute))
                RegisterExtensionAttribute(parent, attribute);
            else
                Context.ProblemReporter.InvalidExtensionType(extension);
        }

        private void RegisterExtensionElement(XmlSchemaObject parent, XmlSchemaElement element)
        {
            XmlSchemaAny parentAny;
            XmlSchemaElement parentElement;
            XmlSchemaGroup parentGroup;
            XmlSchemaComplexType parentComplexType;

            if (Casting.TryCast(parent, out parentAny))
            {
                RegisterExtensionElement(parentAny, element);
            }
            else if (Casting.TryCast(parent, out parentElement))
            {
                var targets = GetAnyElements(parentElement);
                if (targets.Count == 0)
                    Context.ProblemReporter.ParentOffersNoExtension(parentElement, element);
                else
                    RegisterExtensionElements(targets, element);
            }
            else if (Casting.TryCast(parent, out parentGroup))
            {
                var targets = GetAnyElements(parentGroup);
                if (targets.Count == 0)
                    Context.ProblemReporter.ParentOffersNoExtension(parentGroup, element);
                else
                    RegisterExtensionElements(targets, element);
            }
            else if (Casting.TryCast(parent, out parentComplexType))
            {
                var targets = GetAnyElements(parentComplexType);
                if (targets.Count == 0)
                    Context.ProblemReporter.ParentOffersNoExtension(parentComplexType, element);
                else
                    RegisterExtensionElements(targets, element);
            }
            else
            {
                Context.ProblemReporter.InvalidParentForExtension(parent, element);
                return;
            }
        }

        private void RegisterExtensionAttribute(XmlSchemaObject parent, XmlSchemaAttribute attribute)
        {
            XmlSchemaAnyAttribute parentAnyAttribute;
            XmlSchemaElement parentElement;
            XmlSchemaAttributeGroup parentAttributeGroup;
            XmlSchemaComplexType parentComplexType;

            if (Casting.TryCast(parent, out parentAnyAttribute))
            {
                RegisterExtensionAttribute(parentAnyAttribute, (XmlSchemaObject)attribute);
            }
            else if (Casting.TryCast(parent, out parentElement))
            {
                parentAnyAttribute = GetAnyAttribute(parentElement);
                if (parentAnyAttribute == null)
                    Context.ProblemReporter.ParentOffersNoExtension(parentElement, attribute);
                else
                    RegisterExtensionAttribute(parentAnyAttribute, (XmlSchemaObject)attribute);
            }
            else if (Casting.TryCast(parent, out parentAttributeGroup))
            {
                parentAnyAttribute = GetAnyAttribute(parentAttributeGroup);
                if (parentAnyAttribute == null)
                    Context.ProblemReporter.ParentOffersNoExtension(parentAttributeGroup, attribute);
                else
                    RegisterExtensionAttribute(parentAnyAttribute, (XmlSchemaObject)attribute);
            }
            else if (Casting.TryCast(parent, out parentComplexType))
            {
                parentAnyAttribute = GetAnyAttribute(parentComplexType);
                if (parentAnyAttribute == null)
                    Context.ProblemReporter.ParentOffersNoExtension(parentComplexType, attribute);
                else
                    RegisterExtensionAttribute(parentAnyAttribute, (XmlSchemaObject)attribute);
            }
            else
            {
                Context.ProblemReporter.InvalidParentForExtension(parent, attribute);
            }
        }

        private void RegisterExtensionElements(IEnumerable<XmlSchemaAny> targets, XmlSchemaElement element)
        {
            foreach (var target in targets)
                RegisterExtensionElement(target, element);
        }

        private void RegisterExtensionElement(XmlSchemaAny parent, XmlSchemaElement element)
        {
            // Just there to require the argument to be of type XmlSchemaElement.
            Debug.Assert(element.Name != null);

            // Add object to parent's extension elements.

            List<XmlSchemaObject> elements;
            if (!_extensionElements.TryGetValue(parent, out elements))
            {
                elements = new List<XmlSchemaObject>();
                _extensionElements.Add(parent, elements);
            }

            elements.Add(element);

            // Add parent to object's parents.

            RegisterParent(element, parent);
        }

        private void RegisterExtensionAttribute(XmlSchemaAnyAttribute parent, XmlSchemaObject obj)
        {
            // Add object to parent's extension attributes.

            List<XmlSchemaObject> attributes;
            if (!_extensionAttributes.TryGetValue(parent, out attributes))
            {
                attributes = new List<XmlSchemaObject>();
                _extensionAttributes.Add(parent, attributes);
            }

            attributes.Add(obj);

            // Add parent to object's parents.

            RegisterParent(obj, parent);
        }

        private void RegisterParent(XmlSchemaObject obj, XmlSchemaObject parent)
        {
            XmlSchemaElement parentElement;
            do
            {
                parentElement = parent as XmlSchemaElement;
                parent = parent.Parent;
            }
            while (parentElement == null && parent != null);

            Debug.Assert(parentElement != null);

            HashSet<XmlSchemaObject> parents;
            if (!_objectParents.TryGetValue(obj, out parents))
            {
                parents = new HashSet<XmlSchemaObject>();
                _objectParents.Add(obj, parents);
            }

            parents.Add(parentElement);
        }

        private List<XmlSchemaAny> GetAnyElements(XmlSchemaElement element)
        {
            var children = GetChildren(element);
            var result = new List<XmlSchemaAny>();
            if (children != null)
                GetAnyElements(result, children);
            return result;
        }

        private List<XmlSchemaAny> GetAnyElements(XmlSchemaGroup group)
        {
            var children = GetChildren(group);
            var result = new List<XmlSchemaAny>();
            if (children != null)
                GetAnyElements(result, children);
            return result;
        }

        private List<XmlSchemaAny> GetAnyElements(XmlSchemaComplexType type)
        {
            var children = GetChildren(type);
            var result = new List<XmlSchemaAny>();
            if (children != null)
                GetAnyElements(result, children);
            return result;
        }

        private static void GetAnyElements(ICollection<XmlSchemaAny> target, IEnumerable<ChildEntry> children)
        {
            foreach (var child in children)
            {
                if (child.ChildType == ChildType.Any)
                    target.Add((XmlSchemaAny)child.Particle);

                GetAnyElements(target, child.Children);
            }
        }

        private XmlSchemaAnyAttribute GetAnyAttribute(XmlSchemaElement element)
        {
            var entries = GetAttributeEntries(element);
            return entries.AnyAttribute;
        }

        private XmlSchemaAnyAttribute GetAnyAttribute(XmlSchemaAttributeGroup group)
        {
            var entries = GetAttributeEntries(group);
            return entries.AnyAttribute;
        }

        private XmlSchemaAnyAttribute GetAnyAttribute(XmlSchemaComplexType type)
        {
            var entries = GetAttributeEntries(type);
            return entries.AnyAttribute;
        }
    }
}