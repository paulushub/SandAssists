// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2751 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public sealed class LayoutConfiguration
	{
		public static readonly List<LayoutConfiguration> Layouts = new List<LayoutConfiguration>();
        public static string[] DefaultLayouts = new string[] {
			"Default",
			"Debug",
			"Plain"
		};

        private const string configFile         = "LayoutConfig.xml";
        private const string DataLayoutSubPath  = "resources/layouts";
        private const string DefaultLayoutName  = "Default";

        private static string currentLayoutName = DefaultLayoutName;
		
        private string name;
        private string fileName;
        private string displayName;

        private bool readOnly;
        private bool custom;

        private LayoutConfiguration()
        {
        }

        private LayoutConfiguration(XmlElement el, bool custom)
        {
            this.name     = el.GetAttribute("name");
            this.fileName = el.GetAttribute("file");
            this.readOnly = Boolean.Parse(el.GetAttribute("readonly"));
            this.custom   = custom;
        }

        public static event EventHandler LayoutChanged;

		public bool Custom {
			get {
				return custom;
			}
			set {
				custom = value;
			}
		}
		
		public string FileName {
			get {
				return fileName;
			}
			set {
				fileName = value;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public string DisplayName {
			get {
				return displayName == null ? Name : displayName;
			}
			set {
				displayName = value;
			}
		}
		
		public bool ReadOnly {
			get {
				return readOnly;
			}
			set {
				readOnly = value;
			}
		}
		
		public static LayoutConfiguration CreateCustom(string name)
		{
			LayoutConfiguration l = new LayoutConfiguration();
			l.name = name;
			l.fileName = Path.GetRandomFileName() + ".xml";
			File.Copy(Path.Combine(Path.Combine(PropertyService.DataDirectory, DataLayoutSubPath), "Default.xml"),
			          Path.Combine(Path.Combine(PropertyService.ConfigDirectory, "layouts"), l.fileName));
			l.custom = true;
			Layouts.Add(l);
			return l;
		}
		
		public override string ToString()
		{
            return this.DisplayName;
		}
		
		public static string CurrentLayoutName {
			get {
				return currentLayoutName;
			}
			set {
				if (WorkbenchSingleton.InvokeRequired)
					throw new InvalidOperationException("Invoke required");
				if (value != CurrentLayoutName) {
					currentLayoutName = value;
					WorkbenchSingleton.Workbench.WorkbenchLayout.LoadConfiguration();
					OnLayoutChanged(EventArgs.Empty);
				}
			}
		}
		
		public static void ReloadDefaultLayout()
		{
			currentLayoutName = DefaultLayoutName;
			WorkbenchSingleton.Workbench.WorkbenchLayout.LoadConfiguration();
			OnLayoutChanged(EventArgs.Empty);
		}
		
		public static string CurrentLayoutFileName {
			get {
				string configPath = Path.Combine(PropertyService.ConfigDirectory, "layouts");
				LayoutConfiguration current = CurrentLayout;
				if (current != null) {
					return Path.Combine(configPath, current.FileName);
				}
				return null;
			}
		}
		
		public static string CurrentLayoutTemplateFileName {
			get {
				string dataPath = Path.Combine(PropertyService.DataDirectory, DataLayoutSubPath);
				LayoutConfiguration current = CurrentLayout;
				if (current != null) {
					return Path.Combine(dataPath, current.FileName);
				}
				return null;
			}
		}
		
		public static LayoutConfiguration CurrentLayout {
			get {
				foreach (LayoutConfiguration config in Layouts) {
					if (config.name == CurrentLayoutName) {
						return config;
					}
				}
				return null;
			}
		}
		
		public static LayoutConfiguration GetLayout(string name)
		{
			foreach (LayoutConfiguration config in Layouts) {
				if (config.Name == name) {
					return config;
				}
			}
			return null;
		}
		
		public static void SaveCustomLayoutConfiguration()
		{
			string configPath = Path.Combine(PropertyService.ConfigDirectory, "layouts");
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = System.Text.Encoding.UTF8;
            using (XmlWriter w = XmlWriter.Create(
                Path.Combine(configPath, configFile), settings))
            {
				//w.Formatting = Formatting.Indented;
				w.WriteStartElement("LayoutConfig");
				foreach (LayoutConfiguration lc in Layouts) {
					if (lc.custom) {
						w.WriteStartElement("Layout");
						w.WriteAttributeString("name", lc.name);
						w.WriteAttributeString("file", lc.fileName);
						w.WriteAttributeString("readonly", lc.readOnly.ToString());
						w.WriteEndElement();
					}
				}
				w.WriteEndElement();
			}
		}

        internal static void LoadLayoutConfiguration()
        {
            Layouts.Clear();
            string configPath = Path.Combine(PropertyService.ConfigDirectory, "layouts");
            if (File.Exists(Path.Combine(configPath, configFile)))
            {
                LoadLayoutConfiguration(Path.Combine(configPath, configFile), true);
            }
            string dataPath = Path.Combine(PropertyService.DataDirectory, DataLayoutSubPath);
            if (File.Exists(Path.Combine(dataPath, configFile)))
            {
                LoadLayoutConfiguration(Path.Combine(dataPath, configFile), false);
            }
        }

        private static void LoadLayoutConfiguration(string layoutConfig, bool custom)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(layoutConfig);

            foreach (XmlElement el in doc.DocumentElement.ChildNodes)
            {
                Layouts.Add(new LayoutConfiguration(el, custom));
            }
        }

        private static void OnLayoutChanged(EventArgs e)
		{
			if (LayoutChanged != null) {
				LayoutChanged(null, e);
			}
		}
	}
}
