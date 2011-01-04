using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceDocument : BuildObject, IDisposable
    {
        #region Private Fields

        private bool _isModified;
        private bool _isEditing;
        private bool _isDocumentModified;

        private string _documentFile;
        private XmlDocument _document;

        private ReferenceDocumentType _documentType;

        [NonSerialized]
        private FileSystemWatcher _fileWatcher;

        #endregion

        #region Constructors and destructor

        public ReferenceDocument()
        {
            _documentType = ReferenceDocumentType.None;
        }

        public ReferenceDocument(string documentFile, 
            ReferenceDocumentType documentType)
        {
            BuildExceptions.PathMustExist(documentFile, "documentFile");

            _documentFile = documentFile;
            _documentType = documentType;
        }

        ~ReferenceDocument()
        {
            Dispose(false);
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return String.IsNullOrEmpty(_documentFile);
            }
        }

        public bool IsModified
        {
            get
            {
                return _isModified;
            }
        }

        public bool IsEditing
        {
            get
            {
                return _isEditing;
            }
        }

        public ReferenceDocumentType DocumentType
        {
            get
            {
                return _documentType;
            }
        }

        public XmlDocument Document
        {
            get
            {
                if (_document == null)
                {
                    this.Load();
                }

                return _document;
            }
        }

        public string DocumentFile
        {
            get
            {
                return _documentFile;
            }
        }

        #endregion

        #region Public Methods

        public bool BeginEdit(FileSystemWatcher fileWatcher)
        {
            if (_isEditing)
            {
                return _isEditing;
            }

            _isEditing          = true;
            _isDocumentModified = false;

            if (fileWatcher != null)
            {
                fileWatcher.Filter       = Path.GetFileName(_documentFile);
                fileWatcher.Path         = Path.GetDirectoryName(_documentFile);
                fileWatcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite;
                fileWatcher.IncludeSubdirectories = false;

                fileWatcher.Changed += new FileSystemEventHandler(OnFileChanged);

                // Save it to unsubscribe later...
                _fileWatcher = fileWatcher;

                // Begin watching the file for changes...
                _fileWatcher.EnableRaisingEvents = true;
            }

            return _isEditing;
        }

        public void EndEdit()
        {
            if (!_isEditing)
            {
                return;
            }

            if (_fileWatcher != null)
            {
                _fileWatcher.EnableRaisingEvents = false;
                _fileWatcher.Filter = String.Empty;

                _fileWatcher.Changed -= new FileSystemEventHandler(OnFileChanged);
                _fileWatcher = null;
            }

            if (_document != null)
            {
                if (_isDocumentModified)
                {
                    this.Save();
                }
            }

            _isEditing = false;
        }

        public void Accept(ReferenceVisitor visitor)
        {
            BuildExceptions.NotNull(visitor, "visitor");
            if (String.IsNullOrEmpty(_documentFile))
            {
                throw new BuildException(
                    "No XML Document is available for this operation.");
            }

            visitor.Visit(this);
        }

        public void Accept(ReferenceTocVisitor visitor)
        {
            BuildExceptions.NotNull(visitor, "visitor");
            if (String.IsNullOrEmpty(_documentFile))
            {
                throw new BuildException(
                    "No XML Document is available for this operation.");
            }

            visitor.Visit(this);
        }

        #endregion

        #region Public Static Methods

        public static string GetGuildFileName(string topicId)
        {
            if (String.IsNullOrEmpty(topicId))
            {
                return String.Empty;
            }

            HashAlgorithm md5 = HashAlgorithm.Create("MD5");
            byte[] input      = Encoding.UTF8.GetBytes(topicId);
            byte[] output     = md5.ComputeHash(input);
            Guid guid         = new Guid(output);

            return (guid.ToString());
        }

        public static string GetFriendlyFileName(string topicId)
        {
            if (String.IsNullOrEmpty(topicId))
            {
                return String.Empty;
            }

            string fileName = topicId.Replace(':', '_').Replace('<', '_').Replace('>', '_');

            if (fileName.IndexOf(".#ctor") != -1 && fileName.IndexOf("Overload") == -1)
            {
                fileName = "C_" + fileName.Substring(2);
                fileName = fileName.Replace(".#ctor", ".ctor");
            }
            else if (fileName.IndexOf(".#ctor") != -1 && fileName.IndexOf("Overload") != -1)
            {
                fileName = fileName.Replace("Overload", "O_T");
                fileName = fileName.Replace(".#ctor", ".ctor");
            }
            else if (fileName.IndexOf(".#cctor") != -1 && fileName.IndexOf("Overload") == -1)
            {
                fileName = "C_" + fileName.Substring(2);
                fileName = fileName.Replace(".#cctor", ".cctor");
            }
            else if (fileName.IndexOf(".#cctor") != -1 && 
                fileName.IndexOf("Overload") != -1)
            {
                fileName = fileName.Replace("Overload", "O_T");
                fileName = fileName.Replace(".#cctor", ".cctor");
            }
            else if (fileName.IndexOf("Overload") != -1)
            {
                fileName = fileName.Replace("Overload", "O_T");
            }

            fileName = fileName.Replace('.', '_').Replace('#', '_');

            int paramStart = fileName.IndexOf('(');
            if (paramStart != -1)
            {
                fileName = fileName.Substring(0, paramStart) + 
                    GenerateParametersCode(topicId.Substring(paramStart));
            }

            return fileName;
        }

        private static string GenerateParametersCode(string parameterSection)
        {
            int code = parameterSection.GetHashCode();

            int parameterCount = 1;

            for (int count = 0; count < parameterSection.Length; count += 1)
            {
                int c = (int)parameterSection[count];

                if (c == ',')
                    ++parameterCount;
            }

            // format as (# of parameters)_(semi-unique hex code)
            return string.Format("_{1}_{0:x8}", code, parameterCount);
        }

        #endregion

        #region Private Methods

        private void Load()
        {
            if (String.IsNullOrEmpty(_documentFile))
            {
                return;
            }

            _document = new XmlDocument();
            _document.Load(_documentFile);

            _document.NodeChanged  += new XmlNodeChangedEventHandler(OnDocumentChanged);
            _document.NodeRemoved  += new XmlNodeChangedEventHandler(OnDocumentChanged);
            _document.NodeInserted += new XmlNodeChangedEventHandler(OnDocumentChanged);
        }

        private void Save()
        {
            if (String.IsNullOrEmpty(_documentFile))
            {
                return;
            }

            if (_document == null)
            {
                return;
            }

            string backupFile = null;
            try
            {
                if (_fileWatcher != null)
                {
                    _fileWatcher.EnableRaisingEvents = false;
                }

                if (File.Exists(_documentFile))
                {
                    backupFile = Path.ChangeExtension(_documentFile, ".bak");
                    File.Move(_documentFile, backupFile);
                }

                _document.Save(_documentFile);

                if (_fileWatcher != null)
                {
                    _fileWatcher.EnableRaisingEvents = true;
                }

                if (backupFile != null && File.Exists(backupFile))
                {
                    File.Delete(backupFile);
                }

                _isDocumentModified = false;
            }
            catch
            {
                if (backupFile != null && File.Exists(backupFile))
                {
                    File.Move(backupFile, _documentFile);
                }

                throw;
            }
        }

        private void OnDocumentChanged(object sender, XmlNodeChangedEventArgs e)
        {
            _isModified         = true;
            _isDocumentModified = true;
        }

        private void OnFileChanged(object sender, FileSystemEventArgs e)
        {
            _isModified = true;

            if (_document != null)
            {
                // Reload the document...
                this.Load();
            }
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            _document = null;  
        }

        #endregion
    }
}
