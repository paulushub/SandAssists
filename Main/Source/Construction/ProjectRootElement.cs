//
// ProjectRootElement.cs
//
// Author:
//   Leszek Ciesielski (skolima@gmail.com)
//
// (C) 2011 Leszek Ciesielski
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 

using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using Sandcastle.Construction.Internal;
using Sandcastle.Construction.Evaluation;
using Sandcastle.Construction.Exceptions;

namespace Sandcastle.Construction
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class ProjectRootElement : ProjectContainerElement
    {
        #region Private Fields

        private string fullPath;
        private string directoryPath;
        private string toolsVersion;

        #endregion

        #region Constructors and Destructor

        internal ProjectRootElement(ProjectCollection projectCollection)
        {
            toolsVersion = "4.0";
        }

        #endregion

        #region Public Properties

        public override ProjectElementType ElementType
        {
            get
            {
                return ProjectElementType.Root;
            }
        }

        /// <summary>
        /// Gets or sets a condition for this element.
        /// </summary>
        /// <value>
        /// This is always <see langword="null"/>, since this element does not
        /// support the <c>Condition</c> attribute.
        /// </value>
        /// <exception cref="InvalidOperationException">
        /// If you set the value of this property, since it is not supported.
        /// </exception>
        public override string Condition
        {
            get
            {
                return null;
            }
            set
            {
                throw new InvalidOperationException(
                    "This element does not support the Condition attribute.");
            }
        }

        public string DefaultTargets 
        { 
            get; 
            set; 
        }

        public string FullPath
        {
            get 
            { 
                return fullPath; 
            }
            set
            {
                fullPath           = Path.GetFullPath(value);
                this.DirectoryPath = Path.GetDirectoryName(fullPath);
            }
        }

        public string DirectoryPath
        {
            get 
            { 
                return directoryPath ?? String.Empty; 
            }
            internal set 
            { 
                directoryPath = value; 
            }
        }

        public Encoding Encoding
        {
            get 
            { 
                return Encoding.UTF8; 
            }
        }

        public bool HasUnsavedChanges
        {
            get 
            { 
                return true; 
            }
        }

        public string InitialTargets 
        { 
            get; 
            set; 
        }

        public DateTime LastWriteTimeWhenRead
        {
            get 
            { 
                return DateTime.MinValue; 
            }
        }

        public DateTime TimeLastChanged
        {
            get 
            { 
                return DateTime.Now; 
            }
        }

        public string ToolsVersion
        {
            get 
            { 
                return toolsVersion; 
            }
            set 
            { 
                toolsVersion = value; 
            }
        }

        public int Version
        {
            get 
            { 
                return 0; 
            }
        }

        public string RawXml
        {
            get
            {
                using (var writer = new StringWriter(CultureInfo.InvariantCulture))
                {
                    Save(writer);

                    return writer.ToString();
                }
            }
        }

        public ICollection<ProjectPropertyElement> Properties
        {
            get
            {
                return new ReadOnlyCollection<ProjectPropertyElement>(
                  new FilteredEnumerable<ProjectElement, ProjectPropertyElement>(
                      this.AllChildren, ProjectElementType.Property));
            }
        }

        public ICollection<ProjectChooseElement> ChooseElements
        {
            get
            {
                return new ReadOnlyCollection<ProjectChooseElement>(
                  new FilteredEnumerable<ProjectElement, ProjectChooseElement>(
                      this.Children, ProjectElementType.Choose));
            }
        }

        public ICollection<ProjectImportGroupElement> ImportGroups
        {
            get
            {
                return new ReadOnlyCollection<ProjectImportGroupElement>(
                  new FilteredEnumerable<ProjectElement, ProjectImportGroupElement>(
                      this.Children, ProjectElementType.ImportGroup));
            }
        }

        public ICollection<ProjectImportGroupElement> ImportGroupsReversed
        {
            get
            {
                return new ReadOnlyCollection<ProjectImportGroupElement>(
                  new FilteredEnumerable<ProjectElement, ProjectImportGroupElement>(
                      this.ChildrenReversed, ProjectElementType.ImportGroup));
            }
        }

        public ICollection<ProjectImportElement> Imports
        {
            get
            {
                return new ReadOnlyCollection<ProjectImportElement>(
                  new FilteredEnumerable<ProjectElement, ProjectImportElement>(
                      this.AllChildren, ProjectElementType.Import));
            }
        }

        public ICollection<ProjectItemDefinitionGroupElement> ItemDefinitionGroups
        {
            get
            {
                return new ReadOnlyCollection<ProjectItemDefinitionGroupElement>(
                  new FilteredEnumerable<ProjectElement, ProjectItemDefinitionGroupElement>(
                      this.Children, ProjectElementType.ItemDefinitionGroup));
            }
        }

        public ICollection<ProjectItemDefinitionGroupElement> ItemDefinitionGroupsReversed
        {
            get
            {
                return new ReadOnlyCollection<ProjectItemDefinitionGroupElement>(
                  new FilteredEnumerable<ProjectElement, ProjectItemDefinitionGroupElement>(
                      this.ChildrenReversed, ProjectElementType.ItemDefinitionGroup));
            }
        }

        public ICollection<ProjectItemDefinitionElement> ItemDefinitions
        {
            get
            {
                return new ReadOnlyCollection<ProjectItemDefinitionElement>(
                  new FilteredEnumerable<ProjectElement, ProjectItemDefinitionElement>(
                      this.AllChildren, ProjectElementType.ItemDefinition));
            }
        }

        public ICollection<ProjectItemGroupElement> ItemGroups
        {
            get
            {
                return new ReadOnlyCollection<ProjectItemGroupElement>(
                  new FilteredEnumerable<ProjectElement, ProjectItemGroupElement>(
                      this.Children, ProjectElementType.ItemGroup));
            }
        }

        public ICollection<ProjectItemGroupElement> ItemGroupsReversed
        {
            get
            {
                return new ReadOnlyCollection<ProjectItemGroupElement>(
                  new FilteredEnumerable<ProjectElement, ProjectItemGroupElement>(
                      this.ChildrenReversed, ProjectElementType.ItemGroup));
            }
        }

        public ICollection<ProjectItemElement> Items
        {
            get
            {
                return new ReadOnlyCollection<ProjectItemElement>(
                  new FilteredEnumerable<ProjectElement, ProjectItemElement>(
                      this.AllChildren, ProjectElementType.Item));
            }
        }

        public ICollection<ProjectPropertyGroupElement> PropertyGroups
        {
            get
            {
                return new ReadOnlyCollection<ProjectPropertyGroupElement>(
                  new FilteredEnumerable<ProjectElement, ProjectPropertyGroupElement>(
                      this.Children, ProjectElementType.PropertyGroup));
            }
        }

        public ICollection<ProjectPropertyGroupElement> PropertyGroupsReversed
        {
            get
            {
                return new ReadOnlyCollection<ProjectPropertyGroupElement>(
                  new FilteredEnumerable<ProjectElement, ProjectPropertyGroupElement>(
                      this.ChildrenReversed, ProjectElementType.PropertyGroup));
            }
        }

        public ICollection<ProjectTargetElement> Targets
        {
            get
            {
                return new ReadOnlyCollection<ProjectTargetElement>(
                  new FilteredEnumerable<ProjectElement, ProjectTargetElement>(
                      this.Children, ProjectElementType.Target));
            }
        }

        public ICollection<ProjectUsingTaskElement> UsingTasks
        {
            get
            {
                return new ReadOnlyCollection<ProjectUsingTaskElement>(
                  new FilteredEnumerable<ProjectElement, ProjectUsingTaskElement>(
                      this.Children, ProjectElementType.UsingTask));
            }
        }

        #endregion

        #region Internal Properties

        protected override string XmlName
        {
            get { return "Project"; }
        }

        #endregion

        #region Public Methods

        #region Add Methods

        public ProjectImportElement AddImport(string project)
        {
            var import = CreateImportElement(project);
            AppendChild(import);
            return import;
        }

        public ProjectImportGroupElement AddImportGroup()
        {
            var importGroup = CreateImportGroupElement();
            AppendChild(importGroup);
            return importGroup;
        }

        public ProjectItemElement AddItem(string itemType, string include)
        {
            return AddItem(itemType, include, null);
        }

        public ProjectItemElement AddItem(string itemType, string include,
                                           IEnumerable<KeyValuePair<string, string>> metadata)
        {
            var itemGroup = ItemGroups.
                    Where(p => string.IsNullOrEmpty(p.Condition)
                           && p.Items.Where(s => s.ItemType.Equals(itemType,
                                    StringComparison.OrdinalIgnoreCase)).FirstOrDefault() != null).
                            FirstOrDefault();
            if (itemGroup == null)
                itemGroup = AddItemGroup();
            return itemGroup.AddItem(itemType, include, metadata);
        }

        public ProjectItemDefinitionElement AddItemDefinition(string itemType)
        {
            var itemDefGroup = ItemDefinitionGroups.
                    Where(p => string.IsNullOrEmpty(p.Condition)
                           && p.ItemDefinitions.Where(s => s.ItemType.Equals(itemType,
                                    StringComparison.OrdinalIgnoreCase)).FirstOrDefault() != null).
                            FirstOrDefault();
            if (itemDefGroup == null)
                itemDefGroup = AddItemDefinitionGroup();
            return itemDefGroup.AddItemDefinition(itemType);
        }

        public ProjectItemDefinitionGroupElement AddItemDefinitionGroup()
        {
            var itemDefGroup = CreateItemDefinitionGroupElement();
            ProjectContainerElement last = ItemDefinitionGroupsReversed.FirstOrDefault();
            if (last == null)
                last = PropertyGroupsReversed.FirstOrDefault();
            InsertAfterChild(itemDefGroup, last);
            return itemDefGroup;
        }

        public ProjectItemGroupElement AddItemGroup()
        {
            var itemGroup = CreateItemGroupElement();
            ProjectContainerElement last = ItemGroupsReversed.FirstOrDefault();
            if (last == null)
                last = PropertyGroupsReversed.FirstOrDefault();
            InsertAfterChild(itemGroup, last);
            return itemGroup;
        }

        public ProjectPropertyElement AddProperty(string name, string value)
        {
            ProjectPropertyGroupElement parentGroup = null;
            foreach (var propertyGroup in PropertyGroups)
            {
                if (string.IsNullOrEmpty(propertyGroup.Condition))
                {
                    if (parentGroup == null)
                        parentGroup = propertyGroup;
                    var property = propertyGroup.Properties.
                            Where(p => string.IsNullOrEmpty(p.Condition)
                                   && p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)).
                                    FirstOrDefault();
                    if (property != null)
                    {
                        property.Value = value;
                        return property;
                    }
                }
            }
            if (parentGroup == null)
                parentGroup = AddPropertyGroup();
            return parentGroup.AddProperty(name, value);
        }

        public ProjectPropertyGroupElement AddPropertyGroup()
        {
            var propGroup = CreatePropertyGroupElement();
            var last = PropertyGroupsReversed.FirstOrDefault();
            InsertAfterChild(propGroup, last);
            return propGroup;
        }

        public ProjectTargetElement AddTarget(string name)
        {
            var target = CreateTargetElement(name);
            AppendChild(target);
            return target;
        }

        public ProjectUsingTaskElement AddUsingTask(string name, string assemblyFile, string assemblyName)
        {
            var usingTask = CreateUsingTaskElement(name, assemblyFile, assemblyName);
            AppendChild(usingTask);
            return usingTask;
        }

        #endregion

        #region Create Methods

        public ProjectChooseElement CreateChooseElement()
        {
            return new ProjectChooseElement(this);
        }

        public ProjectImportElement CreateImportElement(string project)
        {
            return new ProjectImportElement(project, this);
        }

        public ProjectImportGroupElement CreateImportGroupElement()
        {
            return new ProjectImportGroupElement(this);
        }

        public ProjectItemDefinitionElement CreateItemDefinitionElement(
            string itemType)
        {
            return new ProjectItemDefinitionElement(itemType, this);
        }

        public ProjectItemDefinitionGroupElement CreateItemDefinitionGroupElement()
        {
            return new ProjectItemDefinitionGroupElement(this);
        }

        public ProjectItemElement CreateItemElement(string itemType)
        {
            return new ProjectItemElement(itemType, this);
        }

        public ProjectItemElement CreateItemElement(string itemType, 
            string include)
        {
            ProjectItemElement item = CreateItemElement(itemType);
            item.Include = include;
            return item;
        }

        public ProjectItemGroupElement CreateItemGroupElement()
        {
            return new ProjectItemGroupElement(this);
        }

        public ProjectMetadataElement CreateMetadataElement(string name)
        {
            return new ProjectMetadataElement(name, this);
        }

        public ProjectMetadataElement CreateMetadataElement(string name, 
            string unevaluatedValue)
        {
            ProjectMetadataElement metadata = CreateMetadataElement(name);
            metadata.Value = unevaluatedValue;
            return metadata;
        }

        public ProjectOnErrorElement CreateOnErrorElement(string executeTargets)
        {
            return new ProjectOnErrorElement(executeTargets, this);
        }

        public ProjectOtherwiseElement CreateOtherwiseElement()
        {
            return new ProjectOtherwiseElement(this);
        }

        public ProjectOutputElement CreateOutputElement(string taskParameter, 
            string itemType, string propertyName)
        {
            return new ProjectOutputElement(taskParameter, itemType, 
                propertyName, this);
        }

        public ProjectExtensionsElement CreateProjectExtensionsElement()
        {
            return new ProjectExtensionsElement(this);
        }

        public ProjectPropertyElement CreatePropertyElement(string name)
        {
            return new ProjectPropertyElement(name, this);
        }

        public ProjectPropertyGroupElement CreatePropertyGroupElement()
        {
            return new ProjectPropertyGroupElement(this);
        }

        public ProjectTargetElement CreateTargetElement(string name)
        {
            return new ProjectTargetElement(name, this);
        }

        public ProjectTaskElement CreateTaskElement(string name)
        {
            return new ProjectTaskElement(name, this);
        }

        public ProjectUsingTaskBodyElement CreateUsingTaskBodyElement(
            string evaluate, string body)
        {
            return new ProjectUsingTaskBodyElement(evaluate, body, this);
        }

        public ProjectUsingTaskElement CreateUsingTaskElement(string taskName, 
            string assemblyFile, string assemblyName)
        {
            return new ProjectUsingTaskElement(taskName, assemblyFile, 
                assemblyName, this);
        }

        public ProjectUsingTaskParameterElement CreateUsingTaskParameterElement(
            string name, string output, string required, string parameterType)
        {
            return new ProjectUsingTaskParameterElement(name, output, 
                required, parameterType, this);
        }

        public ProjectUsingTaskParameterGroupElement CreateUsingTaskParameterGroupElement()
        {
            return new ProjectUsingTaskParameterGroupElement(this);
        }

        public ProjectWhenElement CreateWhenElement(string condition)
        {
            ProjectWhenElement whenElement = new ProjectWhenElement(this);
            whenElement.Condition = condition;

            return whenElement;
        }

        #endregion

        #region Load/Save Methods

        public static ProjectRootElement Open(string path)
        {
            return Open(path, ProjectCollection.GlobalProjectCollection);
        }

        public static ProjectRootElement Open(string path, ProjectCollection projectCollection)
        {
            var result = Create(path, projectCollection);
            using (var reader = XmlReader.Create(path))
                result.ReadXml(reader);
            return result;
        }

        public void Save()
        {
            Save(Encoding);
        }

        public void Save(Encoding saveEncoding)
        {
            using (var writer = new StreamWriter(File.Create(FullPath), saveEncoding))
            {
                Save(writer);
            }
        }

        public void Save(string path)
        {
            Save(path, Encoding);
        }

        public void Save(TextWriter writer)
        {
            using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings
            {
                Indent = true,
                NewLineChars = "\r\n",
                OmitXmlDeclaration = true,
            }))
            {
                WriteXml(xmlWriter);
            }
        }

        public void Save(string path, Encoding encoding)
        {
            FullPath = path;
            Save(encoding);
        }

        #endregion

        #endregion

        #region Public Static Methods

        public static ProjectRootElement Create()
        {
            return Create(ProjectCollection.GlobalProjectCollection);
        }

        public static ProjectRootElement Create(ProjectCollection projectCollection)
        {
            return new ProjectRootElement(projectCollection);
        }

        public static ProjectRootElement Create(string path)
        {
            return Create(path, ProjectCollection.GlobalProjectCollection);
        }

        public static ProjectRootElement Create(XmlReader xmlReader)
        {
            return Create(xmlReader, ProjectCollection.GlobalProjectCollection);
        }

        public static ProjectRootElement Create(string path, ProjectCollection projectCollection)
        {
            var result = Create(projectCollection);
            result.FullPath = path;
            return result;
        }

        public static ProjectRootElement Create(XmlReader xmlReader, ProjectCollection projectCollection)
        {
            // yes, this should create en empty project
            var result = Create(projectCollection);
            return result;
        }

        public static ProjectRootElement TryOpen(string path)
        {
            return TryOpen(path, ProjectCollection.GlobalProjectCollection);
        }

        public static ProjectRootElement TryOpen(string path, ProjectCollection projectCollection)
        {
            // this should be non-null only if the project is already cached
            // and caching is not yet implemented
            return null;
        }

        #endregion

        #region Private Methods

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
            try
            {
                base.ReadXml(reader);
            }
            catch (XmlException ex)
            {
                throw new InvalidProjectFileException(FullPath, ex.LineNumber, ex.LinePosition, 0, 0,
                        ex.Message, null, null, null);
            }
        }

        protected override ProjectElement CreateChildElement(string name)
        {
            switch (name)
            {
                case "PropertyGroup":
                    var prop = CreatePropertyGroupElement();
                    AppendChild(prop);
                    return prop;
                case "ItemGroup":
                    var item = CreateItemGroupElement();
                    AppendChild(item);
                    return item;
                case "ImportGroup":
                    var import = CreateImportGroupElement();
                    AppendChild(import);
                    return import;
                case "Import":
                    return AddImport(null);
                case "Target":
                    return AddTarget(null);
                case "ItemDefinitionGroup":
                    var def = CreateItemDefinitionGroupElement();
                    AppendChild(def);
                    return def;
                case "UsingTask":
                    return AddUsingTask(null, null, null);
                case "Choose":
                    var choose = CreateChooseElement();
                    AppendChild(choose);
                    return choose;
                case "ProjectExtensions":
                    var ext = CreateProjectExtensionsElement();
                    AppendChild(ext);
                    return ext;
                default:
                    throw new InvalidProjectFileException(String.Format(
                            "Child \"{0}\" is not a known node type.", name));
            }
        }

        protected override void ReadXmlAttribute(XmlReader reader,
            string name, string value)
        {
            switch (name)
            {
                case "ToolsVersion":
                    this.ToolsVersion = value;
                    break;
                case "DefaultTargets":
                    this.DefaultTargets = value;
                    break;
                case "InitialTargets":
                    this.InitialTargets = value;
                    break;
                default:
                    base.ReadXmlAttribute(reader, name, value);
                    break;
            }
        }

        public override void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement(XmlName, 
                "http://schemas.microsoft.com/developer/msbuild/2003");
            this.WriteXmlValue(writer);
            writer.WriteEndElement();
        }

        protected override void WriteXmlValue(XmlWriter writer)
        {
            this.WriteXmlAttribute(writer, "ToolsVersion", ToolsVersion);
            this.WriteXmlAttribute(writer, "DefaultTargets", DefaultTargets);
            this.WriteXmlAttribute(writer, "InitialTargets", InitialTargets);

            base.WriteXmlValue(writer);
        }

        #endregion
    }
}
