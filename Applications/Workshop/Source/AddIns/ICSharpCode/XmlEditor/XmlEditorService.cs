// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 5258 $</version>
// </file>

using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	public static class XmlEditorService
	{
        public const string XmlSchemaNamespace = "http://www.w3.org/2001/XMLSchema";

        private static Properties xmlEditorProperties;
        private static XmlEditorOptions options;
        private static XmlSchemaProvider registeredXmlSchemas;
        private static XmlSchemaFileAssociations schemaFileAssociations;
		
		public static XmlEditorOptions Options {
			get {
				if (options == null) {
					CreateXmlEditorOptions();
				}
				return options;
			}
		}
		
		public static bool ShowAttributesWhenFolded {
			get { return Options.ShowAttributesWhenFolded; }
			set { Options.ShowAttributesWhenFolded = value; }
		}
		
		public static bool ShowSchemaAnnotation {
			get { return Options.ShowSchemaAnnotation; }
			set { Options.ShowSchemaAnnotation = value; }
		}
		
		public static XmlSchemaFileAssociations SchemaFileAssociations {
			get {
				if (schemaFileAssociations == null) {
					CreateXmlSchemaFileAssociations();
				}
				return schemaFileAssociations;
			}
		}

        public static XmlSchemaProvider RegisteredSchemas
        {
            get
            {
                if (registeredXmlSchemas == null)
                {
                    CreateRegisteredXmlSchemas();
                    registeredXmlSchemas.ReadSchemas();
                    LogRegisteredSchemaErrorsAsWarnings();
                }

                return registeredXmlSchemas;
            }
        }

        /// <summary>
        /// Determines whether the specified namespace is actually the W3C namespace for
        /// XSD files.
        /// </summary>
        public static bool IsXmlSchemaNamespace(string schemaNamespace)
        {
            return schemaNamespace == XmlSchemaNamespace;
        }

        private static void CreateXmlEditorOptions()
        {
            CreateXmlEditorProperties();
            options = new XmlEditorOptions(xmlEditorProperties);
        }

        private static void CreateXmlEditorProperties()
        {
            xmlEditorProperties = PropertyService.Get(XmlEditorOptions.OptionsProperty, 
                new Properties());
        }

        private static void CreateXmlSchemaFileAssociations()
		{
			CreateXmlEditorProperties();
			schemaFileAssociations = new XmlSchemaFileAssociations(xmlEditorProperties, 
                new XmlSchemaFileAssociationList(), RegisteredSchemas.Schemas);
		}

        private static void CreateRegisteredXmlSchemas()
		{
            IList<string> readOnlySchemaFolders = GetReadOnlySchemaFolders();
			string userDefinedSchemaFolder = GetUserDefinedSchemaFolder();
			FileSystem fileSystem = new FileSystem();
				
			registeredXmlSchemas = new XmlSchemaProvider(readOnlySchemaFolders, 
                userDefinedSchemaFolder, fileSystem);
		}

        private static IList<string> GetReadOnlySchemaFolders()
		{
			List<string> folders = new List<string>();
			folders.Add(RuntimeEnvironment.GetRuntimeDirectory());
            folders.Add(GetDefaultSchemaFolder());

			return folders;
		}

        private static string GetUserDefinedSchemaFolder()
		{
			return Path.Combine(PropertyService.ConfigDirectory, "schemas");
		}

        private static string GetDefaultSchemaFolder()
		{
			return Path.Combine(PropertyService.DataDirectory, "schemas");
		}
		
		private static void LogRegisteredSchemaErrorsAsWarnings()
		{
			foreach (XmlSchemaError error in registeredXmlSchemas.GetSchemaErrors()) {
				if (error.HasException) {
					LoggingService.Warn(error.Message, error.Exception);
				} else {
					LoggingService.Warn(error.Message);
				}
			}
		}
	}
}
