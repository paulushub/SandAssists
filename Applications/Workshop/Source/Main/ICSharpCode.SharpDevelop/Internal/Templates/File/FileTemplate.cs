﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3685 $</version>
// </file>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
    public sealed class TemplateProperty
	{
		string name;
		string localizedName;
		string type;
		string category;
		string description;
		string defaultValue;
        private string readOnly;
		
		public string Name {
			get {
				return name;
			}
		}
		
		public string LocalizedName {
			get {
				return localizedName;
			}
		}
		
		public string Type {
			get {
				return type;
			}
		}
		
		public string Category {
			get {
				return category;
			}
		}
		
		public string Description {
			get {
				return description;
			}
		}
		
		public string DefaultValue {
			get {
				return defaultValue;
			}
		}

        public bool IsReadOnly
        {
            get
            {
                if (!String.IsNullOrEmpty(readOnly))
                {
                    return readOnly.Equals("true", StringComparison.OrdinalIgnoreCase);
                }

                return false;
            }
        }
		
		public TemplateProperty(XmlElement propertyElement)
		{
			name         = propertyElement.GetAttribute("name");
			localizedName= propertyElement.GetAttribute("localizedName");
			type         = propertyElement.GetAttribute("type");
			category     = propertyElement.GetAttribute("category");
			description  = propertyElement.GetAttribute("description");
			defaultValue = propertyElement.GetAttribute("defaultValue");
            readOnly     = propertyElement.GetAttribute("readonly");
		}
	}
	
	public sealed class TemplateType
	{
		string    name;
		Hashtable pairs = new Hashtable();
		
		public string Name {
			get {
				return name;
			}
		}
		
		public Hashtable Pairs {
			get {
				return pairs;
			}
		}
		
		public TemplateType(XmlElement enumType)
		{
			name = enumType.GetAttribute("name");
			foreach (XmlElement node in enumType.ChildNodes) {
				pairs[node.GetAttribute("name")] = node.GetAttribute("value");
			}
		}
	}
	
	/// <summary>
	/// This class defines and holds the new file templates.
	/// </summary>
    public sealed class FileTemplate : IComparable
	{
		public static List<FileTemplate> FileTemplates = new List<FileTemplate>();
		
		private string author;
        private string name;
        private string category;
        private string languagename;
        private string icon;
        private string description;
        private string wizardpath;
        private string defaultName;
        private string subcategory;

        private bool newFileDialogVisible;

        private List<FileDescriptionTemplate> files;
        private List<TemplateProperty> properties;
        private List<TemplateType> customTypes;
        private List<ReferenceProjectItem> requiredAssemblyReferences;

        private XmlElement fileoptions;

        private FileTemplate()
        {
            newFileDialogVisible = true;
            files = new List<FileDescriptionTemplate>();
            properties = new List<TemplateProperty>();
            customTypes = new List<TemplateType>();
            requiredAssemblyReferences = new List<ReferenceProjectItem>();
        }
		
		public FileTemplate(string filename)
            : this()
		{
			XmlDocument doc = new XmlDocument();
			doc.Load(filename);
			
			author = doc.DocumentElement.GetAttribute("author");
			
			XmlElement config = doc.DocumentElement["Config"];
			name         = config.GetAttribute("name");
			icon         = config.GetAttribute("icon");
			category     = config.GetAttribute("category");
			defaultName  = config.GetAttribute("defaultname");
			languagename = config.GetAttribute("language");
			
			if (config.HasAttribute("subcategory")) {
				subcategory = config.GetAttribute("subcategory");
			}

			string newFileDialogVisibleAttr  = config.GetAttribute("newfiledialogvisible");
			if (newFileDialogVisibleAttr != null && newFileDialogVisibleAttr.Length != 0) {
				if (newFileDialogVisibleAttr.Equals("false", StringComparison.OrdinalIgnoreCase))
					newFileDialogVisible = false;
			}

			if (doc.DocumentElement["Description"] != null) {
				description  = doc.DocumentElement["Description"].InnerText;
			}
			
			if (config["Wizard"] != null) {
				wizardpath = config["Wizard"].Attributes["path"].InnerText;
			}
			
			if (doc.DocumentElement["Properties"] != null) {
				XmlNodeList propertyList = doc.DocumentElement["Properties"].SelectNodes("Property");
				foreach (XmlElement propertyElement in propertyList) {
					properties.Add(new TemplateProperty(propertyElement));
				}
			}
			
			if (doc.DocumentElement["Types"] != null) {
				XmlNodeList typeList = doc.DocumentElement["Types"].SelectNodes("Type");
				foreach (XmlElement typeElement in typeList) {
					customTypes.Add(new TemplateType(typeElement));
				}
			}
			
			if (doc.DocumentElement["References"] != null) 
            {
				XmlNodeList references = doc.DocumentElement["References"].SelectNodes("Reference");
				foreach (XmlElement reference in references)
                {
					if (!reference.HasAttribute("include"))
						throw new InvalidDataException("Reference without 'include' attribute!");
					ReferenceProjectItem item = new ReferenceProjectItem(null, reference.GetAttribute("include"));
                    string hintPath = reference.GetAttribute("hintPath");
                    if (!String.IsNullOrEmpty(hintPath))
                    {
                        hintPath = StringParser.Parse(hintPath);
                    }
					item.SetMetadata("HintPath", hintPath);
                    string localCopy = reference.GetAttribute("copyLocal");
                    // We check for false, the default is true...
                    if (!String.IsNullOrEmpty(localCopy) && localCopy.Equals("false",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        item.CopyLocal = false;
                    }
					requiredAssemblyReferences.Add(item);
				}
			}
			
			fileoptions = doc.DocumentElement["AdditionalOptions"];
			
			doc.DocumentElement.SetAttribute("fileName", filename); // used for template loading warnings
			
			// load the files
			XmlElement elements  = doc.DocumentElement["Files"];
            XmlNodeList nodes = elements.ChildNodes;
			foreach (XmlNode filenode in nodes) {
				if (filenode is XmlElement) {
					this.files.Add(new FileDescriptionTemplate((XmlElement)filenode, 
                        Path.GetDirectoryName(filename)));
				}
			}
		}

        static FileTemplate()
        {
            UpdateTemplates();
        }
		
		int IComparable.CompareTo(object other)
		{
			FileTemplate pt = other as FileTemplate;
			if (pt == null) return -1;
			int res = category.CompareTo(pt.category);
			if (res != 0) return res;
			return name.CompareTo(pt.name);
		}
		
		public string Author {
			get {
				return author;
			}
		}
		public string Name {
			get {
				return name;
			}
		}
		public string Category {
			get {
				return category;
			}
		}
		public string Subcategory {
			get {
				return subcategory;
			}
		}
		public string LanguageName {
			get {
				return languagename;
			}
		}
		public string Icon {
			get {
				return icon;
			}
		}
		public string Description {
			get {
				return description;
			}
		}
		public string WizardPath {
			get {
				return wizardpath;
			}
		}
		public string DefaultName {
			get {
				return defaultName;
			}
		}
		public XmlElement Fileoptions {
			get {
				return fileoptions;
			}
		}
		public bool NewFileDialogVisible {
			get {
				return newFileDialogVisible;
			}
		}
		
		public List<FileDescriptionTemplate> FileDescriptionTemplates {
			get {
				return files;
			}
		}
		
		public List<TemplateProperty> Properties {
			get {
				return properties;
			}
		}
		
		public List<TemplateType> CustomTypes {
			get {
				return customTypes;
			}
		}
		
		public List<ReferenceProjectItem> RequiredAssemblyReferences {
			get { return requiredAssemblyReferences; }
		}
		
		public bool HasProperties {
			get {
				return properties != null && properties.Count > 0;
			}
		}
		
		public static void UpdateTemplates()
		{
			string dataTemplateDir = FileUtility.Combine(PropertyService.DataDirectory, "templates", "file");
			List<string> files = FileUtility.SearchDirectory(dataTemplateDir, "*.xft");
			foreach (string templateDirectory in AddInTree.BuildItems<string>(ProjectTemplate.TemplatePath, null, false)) {
				if (!Directory.Exists(templateDirectory))
					Directory.CreateDirectory(templateDirectory);
				files.AddRange(FileUtility.SearchDirectory(templateDirectory, "*.xft"));
			}
			FileTemplates.Clear();
			foreach (string file in files) {
				try {
					FileTemplates.Add(new FileTemplate(file));
				} catch (XmlException ex) {
					MessageService.ShowError("Error loading template file " + file + ":\n" + ex.Message);
				} catch (TemplateLoadException ex) {
					MessageService.ShowError("Error loading template file " + file + ":\n" + ex.ToString());
				} catch(Exception e) {
					MessageService.ShowError(e, "Error loading template file " + file + ".");
				}
			}
			FileTemplates.Sort();
		}
	}
}