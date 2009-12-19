using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.XmlEditor;
using ICSharpCode.TextEditor;

namespace Sandcastle.Workshop.Bindings
{
    public static class MamlEditorService
    {
        private static bool _isInitialized;
        private static XmlSchemaProvider _schemaProvider;

        public static bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        public static XmlSchemaProvider RegisteredSchemas
        {
            get
            {
                if (!_isInitialized)
                {
                    Initialize();
                }

                return _schemaProvider;
            }
        }

        public static void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }

            List<string> schemaFolders = new List<string>();
            string sourceDir = Environment.ExpandEnvironmentVariables("%DXROOT%");
            if (!String.IsNullOrEmpty(sourceDir))
            {
                sourceDir = FileUtility.StripEndBackSlash(Path.GetFullPath(sourceDir));
                if (Directory.Exists(sourceDir))
                {                       
                    sourceDir = Path.Combine(sourceDir, @"Schemas\Authoring");

                    if (Directory.Exists(sourceDir))
                    {
                        schemaFolders.Add(sourceDir);
                    }
                }
            }

            if (schemaFolders.Count == 0)
            {
                schemaFolders.Add(GetDefaultSchemaFolder());
            }

            string userFolder = GetUserDefinedSchemaFolder();
            FileSystem fileSystem = new FileSystem();

            _schemaProvider = new XmlSchemaProvider(schemaFolders, userFolder, fileSystem);
            _schemaProvider.ReadSchemas(sourceDir, "*.xsd", true);

            _isInitialized = true;
        }

        public static void Uninitialize()
        {
            if (!_isInitialized)
            {
                return;
            }

            _schemaProvider = null;
            _isInitialized  = false;
        }

        private static string GetUserDefinedSchemaFolder()
        {
            return String.Empty;
            //return Path.Combine(PropertyService.ConfigDirectory, "Schemas");
        }

        private static string GetDefaultSchemaFolder()
        {
            return Path.Combine(PropertyService.DataDirectory, @"Schemas\Sandcastle");
        }
    }
}
