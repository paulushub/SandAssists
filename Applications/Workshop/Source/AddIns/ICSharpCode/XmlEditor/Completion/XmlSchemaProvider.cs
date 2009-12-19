// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 5304 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Keeps track of all the schemas that the XML Editor is aware of.
	/// </summary>
	public sealed class XmlSchemaProvider : IXmlSchemaCompletionFactory
	{
        private string userDefinedSchemaFolder;
        private List<string> readOnlySchemaFolders;
        private IFileSystem fileSystem;
        private IXmlSchemaCompletionFactory factory;
        private XmlSchemaCompletionCollection schemas;
        private List<XmlSchemaError> schemaErrors;

        public XmlSchemaProvider(IList<string> readOnlySchemaFolders, 
			string userDefinedSchemaFolder, IFileSystem fileSystem, 
			IXmlSchemaCompletionFactory factory)
		{
            this.readOnlySchemaFolders = new List<string>();
            this.schemas               = new XmlSchemaCompletionCollection();
            this.schemaErrors          = new List<XmlSchemaError>();

			this.readOnlySchemaFolders.AddRange(readOnlySchemaFolders);
			this.userDefinedSchemaFolder = userDefinedSchemaFolder;
			this.fileSystem = fileSystem;
			this.factory = factory;
		}

        public XmlSchemaProvider(IList<string> readOnlySchemaFolders, 
            string userDefinedSchemaFolder, IFileSystem fileSystem)
			: this(readOnlySchemaFolders, userDefinedSchemaFolder, fileSystem, null)
		{
			this.factory = this;
		}

        public event EventHandler UserDefinedSchemaAdded;
        public event EventHandler UserDefinedSchemaRemoved;
		
		public XmlSchemaCompletionCollection Schemas {
			get { return schemas; }
		}
		
		public bool SchemaExists(string namespaceUri)
		{
			return schemas.Contains(namespaceUri);
		}
		
		public void RemoveUserDefinedSchema(string namespaceUri)
		{
			XmlSchemaCompletion schema = schemas[namespaceUri];
			if (schema != null) {
				if (fileSystem.FileExists(schema.FileName)) {
					fileSystem.DeleteFile(schema.FileName);
				}
				schemas.Remove(schema);
				OnUserDefinedSchemaRemoved();
			}
		}

        public void AddUserDefinedSchema(XmlSchemaCompletion schema)
        {
            if (String.IsNullOrEmpty(userDefinedSchemaFolder))
            {
                return;
            }

            if (!fileSystem.DirectoryExists(userDefinedSchemaFolder))
            {
                fileSystem.CreateDirectory(userDefinedSchemaFolder);
            }

            string newSchemaDestinationFileName = GetUserDefinedSchemaDestination(schema);
            fileSystem.CopyFile(schema.FileName, newSchemaDestinationFileName);

            schema.FileName = newSchemaDestinationFileName;
            schemas.Add(schema);

            OnUserDefinedSchemaAdded();
        }

        public IList<XmlSchemaError> GetSchemaErrors()
		{
			return schemaErrors;
		}
		
		public void ReadSchemas()
		{
			foreach (string folder in readOnlySchemaFolders) {
				ReadSchemas(folder, "*.xsd", true);
			}
            if (!String.IsNullOrEmpty(userDefinedSchemaFolder))
            {
                ReadSchemas(userDefinedSchemaFolder, "*.*", false);
            }
		}

        public void ReadSchemas(string directory, string searchPattern, bool readOnly)
		{
			if (fileSystem.DirectoryExists(directory)) {
				foreach (string fileName in fileSystem.GetFilesInDirectory(directory, searchPattern)) {
					XmlSchemaCompletion schema = ReadSchema(fileName, readOnly);
					if (schema != null) {
						AddSchema(schema);
					}
				}
			}
		}

        private void OnUserDefinedSchemaRemoved()
        {
            if (UserDefinedSchemaRemoved != null)
            {
                UserDefinedSchemaRemoved(this, new EventArgs());
            }
        }

        private string GetUserDefinedSchemaDestination(XmlSchemaCompletion schema)
        {
            string fileName = Path.GetFileName(schema.FileName);
            return Path.Combine(userDefinedSchemaFolder, fileName);
        }

        private void OnUserDefinedSchemaAdded()
        {
            if (UserDefinedSchemaAdded != null)
            {
                UserDefinedSchemaAdded(this, new EventArgs());
            }
        }

        private XmlSchemaCompletion ReadSchema(string fileName, bool readOnly)
		{
			try {
				string baseUri = XmlSchemaCompletion.GetUri(fileName);
				XmlSchemaCompletion schema = factory.CreateSchemaCompletion(baseUri, fileName);
				schema.FileName = fileName;
				schema.ReadOnly = readOnly;
				return schema;
			} catch (Exception ex) {
				schemaErrors.Add(new XmlSchemaError("Unable to read schema '" + fileName + "'.", ex));
			}
			return null;
        }

        private void AddSchema(XmlSchemaCompletion schema)
        {
            if (schema.HasNamespaceUri)
            {
                schemas.Add(schema);
            }
            else
            {
                schemaErrors.Add(new XmlSchemaError(
                    "Ignoring schema with no namespace '" + schema.FileName + "'."));
            }
        }

        #region IXmlSchemaCompletionFactory Members

        XmlSchemaCompletion IXmlSchemaCompletionFactory.CreateSchemaCompletion(string baseUri, string fileName)
		{
			return new XmlSchemaCompletion(baseUri, fileName);
        }

        #endregion
	}
}
