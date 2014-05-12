using System;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

using Sandcastle.ReflectionData.References;
using Sandcastle.ReflectionData.Targets;

namespace Sandcastle.ReflectionData
{
    public sealed class TargetController
    {
        #region Private Fields

        private bool           _isInitialized;
        private bool           _isDataLoaded;

        private MemoryTargetStorage   _localStorage;
        private DatabaseTargetTextStorage _msdnStorage;

        private MessageWriter _messageWriter;

        private static TargetController _controller;

        #endregion

        #region Constructors and Destructor

        private TargetController()
        {
            _localStorage = new MemoryTargetStorage();
            _msdnStorage  = new DatabaseTargetTextStorage(true, false);
        }

        #endregion

        #region Public Properties

        public TargetStorage Local
        {
            get
            {
                return _localStorage;
            }
        }

        public TargetStorage Msdn
        {
            get
            {
                return _msdnStorage;
            }
        }

        public bool HasMsdnStorage
        {
            get
            {
                return (_msdnStorage != null);
            }
        }

        public static TargetController Controller
        {
            get
            {
                if (_controller == null)
                {
                    _controller = new TargetController();
                }

                return _controller;
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(BuildComponent component)
        {   
            if (_isInitialized)
            {
                return;
            }
            if (component == null)
            {
                throw new ArgumentNullException();
            }

            BuildAssembler assembler = component.BuildAssembler;
            if (assembler == null)
            {
                throw new ArgumentException();
            }

            _messageWriter = assembler.MessageWriter;
            _isInitialized  = (_messageWriter != null);
        }

        public void Uninitialize()
        {   
            try
            {
                if (_localStorage != null)
                {
                    _localStorage.Dispose();
                    _localStorage = null;
                }
                if (_msdnStorage != null)
                {
                    _msdnStorage.Dispose();
                    _msdnStorage = null;
                }
            }
            catch
            {              	
            }
        }

        public TargetCollection GetCollection(XPathNavigator configuration,
            out ReferenceLinkType localLink, out ReferenceLinkType msdnLink)
        {
            localLink = ReferenceLinkType.None;
            msdnLink  = ReferenceLinkType.None;

            XPathNodeIterator targetsNodes = configuration.Select("targets");
            if (targetsNodes == null || targetsNodes.Count == 0)
            {
                return new TargetCollections();
            }

            foreach (XPathNavigator targetsNode in targetsNodes)
            {
                // get target type
                string typeValue = targetsNode.GetAttribute("type", String.Empty);
                if (String.IsNullOrEmpty(typeValue))
                    WriteMessage(MessageLevel.Error, "Each targets element must have a type attribute that specifies which type of links to create.");

                ReferenceLinkType linkType = ReferenceLinkType.None;
                try
                {
                    linkType = (ReferenceLinkType)Enum.Parse(typeof(ReferenceLinkType), typeValue, true);
                }
                catch (ArgumentException)
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "'{0}' is not a supported reference link type.", typeValue));
                }

                // get base directory
                string baseValue = targetsNode.GetAttribute("base", String.Empty);

                // get file pattern
                string filesValue = targetsNode.GetAttribute("files", String.Empty);
                if (String.IsNullOrEmpty(filesValue))
                    WriteMessage(MessageLevel.Error, "Each targets element must have a files attribute specifying which target files to load.");

                // determine whether to search recursively
                bool recurse = false;
                string recurseValue = targetsNode.GetAttribute("recurse", String.Empty);
                if (!String.IsNullOrEmpty(recurseValue))
                {
                    if (String.Compare(recurseValue, Boolean.TrueString, true) == 0)
                    {
                        recurse = true;
                    }
                    else if (String.Compare(recurseValue, Boolean.FalseString, true) == 0)
                    {
                        recurse = false;
                    }
                    else
                    {
                        WriteMessage(MessageLevel.Error, String.Format(
                            "On the targets element, recurse='{0}' is not an allowed value.", recurseValue));
                    }
                }

                // turn baseValue and filesValue into directoryPath and filePattern
                string fullPath;
                if (String.IsNullOrEmpty(baseValue))
                {
                    fullPath = filesValue;
                }
                else
                {
                    fullPath = Path.Combine(baseValue, filesValue);
                }

                fullPath = Environment.ExpandEnvironmentVariables(fullPath);
                string directoryPath = Path.GetDirectoryName(fullPath);
                if (String.IsNullOrEmpty(directoryPath))
                    directoryPath = Environment.CurrentDirectory;
                string filePattern = Path.GetFileName(fullPath);

                bool isSystem = false;
                DataSource dataSource = null;

                // Verify that directory exists...
                if (!Directory.Exists(directoryPath))
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The targets directory '{0}' does not exist.", directoryPath));
                }
                else
                {
                    bool systemIsFound = false;
                    string systemValue = targetsNode.GetAttribute("system", String.Empty);
                    if (!String.IsNullOrEmpty(systemValue))
                    {
                        if (String.Compare(systemValue, Boolean.TrueString, true) == 0)
                        {
                            isSystem      = true;
                            systemIsFound = true;

                            if (msdnLink == ReferenceLinkType.None)
                            {
                                msdnLink = linkType;
                            }
                        }
                        else if (String.Compare(systemValue, Boolean.FalseString, true) == 0)
                        {
                            isSystem      = false;
                            systemIsFound = true;

                            if (localLink == ReferenceLinkType.None &&
                                linkType != ReferenceLinkType.Msdn)
                            {
                                localLink = linkType;
                            }
                        }
                        else
                        {
                            WriteMessage(MessageLevel.Error, String.Format(
                                "On the targets element, system='{0}' is not an allowed value.", systemValue));
                        }
                    }

                    // Retrieve the data source, if any...
                    XPathNavigator nodeDataSource = 
                        targetsNode.SelectSingleNode("source");
                    if (nodeDataSource != null)
                    {
                        dataSource = new DataSource(false, nodeDataSource);
                        if (dataSource.IsValid)
                        {   
                            // Currently, database is supported for systems only...
                            if (!dataSource.IsSystem && !dataSource.IsDatabase)
                            {
                                dataSource = null;
                            }
                            else
                            {   
                                if (String.IsNullOrEmpty(systemValue))
                                {
                                    isSystem = dataSource.IsSystem;
                                    systemIsFound = true;
                                }
                            }
                        }
                        else
                        {
                            dataSource = null;
                        }
                    }

                    if (String.IsNullOrEmpty(systemValue) && !systemIsFound)
                    {   
                        // Try doing it generally...
                        if (directoryPath.IndexOf(@"Sandcastle\Data\Reflection",
                            StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            isSystem = true;
                            if (msdnLink == ReferenceLinkType.None)
                            {
                                msdnLink = linkType;
                            }
                        }
                        else
                        {
                            if (localLink == ReferenceLinkType.None &&
                                linkType != ReferenceLinkType.Msdn)
                            {
                                localLink = linkType;
                            }
                        }
                    }
                }

                // If the database is not yet loaded, we load all...
                if (!_isDataLoaded)
                {   
                    if (isSystem || (dataSource != null && dataSource.IsSystem))
                    {
                        if (dataSource != null && dataSource.Exists)
                        {   
                            if (_msdnStorage.IsInitialize)
                            {
                                DatabaseTargetStorageEx databaseStorage =
                                    new DatabaseTargetStorageEx(true, false,
                                        dataSource.OutputDir);

                                _msdnStorage.AddStorage(databaseStorage);
                            }
                            else
                            {
                                _msdnStorage.Initialize(dataSource.OutputDir, false);
                            }
                        }
                        else
                        {
                            if (!_msdnStorage.Exists)
                            {
                                if (!_msdnStorage.IsInitialize)
                                {
                                    if (Directory.Exists(directoryPath))
                                    {
                                        _msdnStorage.Initialize(directoryPath, false);
                                    }
                                }

                                this.AddDatabaseTargets(directoryPath, filePattern,
                                    recurse, linkType);
                            }
                        }
                    }
                    else
                    {
                        if (linkType == ReferenceLinkType.Msdn)
                        {
                            if (!_msdnStorage.IsInitialize)
                            {
                                if (Directory.Exists(directoryPath))
                                {
                                    _msdnStorage.Initialize(directoryPath, false);
                                }
                            }

                            this.AddDatabaseTargets(directoryPath, filePattern, 
                                recurse, linkType);
                        }
                        else
                        {
                            this.AddTargets(directoryPath, filePattern, 
                                recurse, linkType);
                        }
                    }
                }
            }

            if (!_isDataLoaded)
            {
                _isDataLoaded = true;
            }

            return new TargetCollections(_localStorage, _msdnStorage, 
                localLink, msdnLink);
        }

        #endregion

        #region Private Methods

        private void WriteMessage(MessageLevel level, string message)
        {
            if (level == MessageLevel.Ignore || _messageWriter == null) 
                return;

            _messageWriter.Write(this.GetType(), level, message);
        }

        #region AddTargets Methods

        private void AddTargets(string directory, string filePattern, 
            bool recurse, ReferenceLinkType type)
        {
            // add the specified targets from the directory
            WriteMessage(MessageLevel.Info, String.Format(
                "Searching directory '{0}' for targets files of the form '{1}'.",
                directory, filePattern));


            string[] files = Directory.GetFiles(directory, filePattern);
            foreach (string file in files)
            {
                AddTargets(file, type);
            }
            if (recurse)
            {
                string[] subdirectories = Directory.GetDirectories(directory);
                foreach (string subdirectory in subdirectories)
                {
                    AddTargets(subdirectory, filePattern, recurse, type);
                }
            }
        }

        private void AddTargets(string file, ReferenceLinkType type)
        {
            try
            {
                XPathDocument document = new XPathDocument(file);
                // This will only load into the memory...
                TargetCollectionXmlUtilities.AddTargets(_localStorage, document.CreateNavigator(), type);
            }
            catch (XmlSchemaException e)
            {
                WriteMessage(MessageLevel.Error, String.Format(
                    "The reference targets file '{0}' is not valid. The error message is: {1}",
                    file, BuildComponentUtilities.GetExceptionMessage(e)));
            }
            catch (XmlException e)
            {
                WriteMessage(MessageLevel.Error, String.Format(
                    "The reference targets file '{0}' is not well-formed XML. The error message is: {1}",
                    file, BuildComponentUtilities.GetExceptionMessage(e)));
            }
            catch (IOException e)
            {
                WriteMessage(MessageLevel.Error, String.Format(
                    "An access error occurred while opening the reference targets file '{0}'. The error message is: {1}",
                    file, BuildComponentUtilities.GetExceptionMessage(e)));
            }
        }

        #endregion

        #region AddDatabaseTargets Methods

        private void AddDatabaseTargets(string directory, string filePattern, 
            bool recurse, ReferenceLinkType type)
        {
            // add the specified targets from the directory
            WriteMessage(MessageLevel.Info, String.Format(
                "Searching directory '{0}' for targets files of the form '{1}'.",
                directory, filePattern));


            string[] files = Directory.GetFiles(directory, filePattern);
            foreach (string file in files)
            {
                AddDatabaseTargets(file, type);
            }
            if (recurse)
            {
                string[] subdirectories = Directory.GetDirectories(directory);
                foreach (string subdirectory in subdirectories)
                {
                    AddDatabaseTargets(subdirectory, filePattern, recurse, type);
                }
            }
        }

        private void AddDatabaseTargets(string file, ReferenceLinkType type)
        {
            try
            {
                XPathDocument document = new XPathDocument(file);
                // This will load into the memory, if the database does not
                // exists...

                TargetCollectionXmlUtilities.AddTargets(_msdnStorage,
                    document.CreateNavigator(), type);
            }
            catch (XmlSchemaException e)
            {
                WriteMessage(MessageLevel.Error, String.Format(
                    "The reference targets file '{0}' is not valid. The error message is: {1}",
                    file, BuildComponentUtilities.GetExceptionMessage(e)));
            }
            catch (XmlException e)
            {
                WriteMessage(MessageLevel.Error, String.Format(
                    "The reference targets file '{0}' is not well-formed XML. The error message is: {1}",
                    file, BuildComponentUtilities.GetExceptionMessage(e)));
            }
            catch (IOException e)
            {
                WriteMessage(MessageLevel.Error, String.Format(
                    "An access error occurred while opening the reference targets file '{0}'. The error message is: {1}",
                    file, BuildComponentUtilities.GetExceptionMessage(e)));
            }
        }

        #endregion

        #endregion
    }
}
