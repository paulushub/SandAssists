using System;
using System.IO;
using System.Collections.Generic;

namespace Sandcastle.Builders.Templates
{
    /// <summary>
    /// Manages the file templates, <see cref="FileTemplate"/>, registered to be used by
    /// the file creation dialog.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This is a <c>Singleton</c> an instance can be access from the <see cref="FileTemplateManager.Manager"/>
    /// property.
    /// </para>
    /// <para>
    /// The list of file templates are available by category, and the available categories can
    /// be accessed from the <see cref="FileTemplateManager.Categories"/> property.
    /// </para>
    /// <example>
    /// <para>
    /// The following illustrates the use for the file template manager.
    /// </para>
    /// <code lang="c#">
    /// <![CDATA[
    /// using System;
    /// using System.Collections.Generic;
    /// 
    /// using Sandcastle;
    /// using Sandcastle.Builders.Templates;
    /// 
    /// namespace TemplateTests
    /// {
    ///     class Program
    ///     {
    ///         static void Main(string[] args)
    ///         {
    ///             // 1. Get the file template manager...
    ///             FileTemplateManager templateManager = FileTemplateManager.Manager;
    ///             // 2. Register file templates, specifying the template directories...
    ///             templateManager.Register(@"A:\Templates\Files", true);
    ///             // 3. Initialize the manager to load the templates...
    ///             templateManager.Initialize();
    /// 
    ///             // 4. Get templates, for instance in the 'TestTemplates' category...
    ///             IList<FileTemplate> testTemplates = templateManager["TestTemplates"];
    /// 
    ///             // 5. Process the templates...
    /// 
    ///             // 6. Uninitialize the manager to unload the templates...
    ///             templateManager.Uninitialize();
    ///         }
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// </example>
    /// </remarks>
    public sealed class FileTemplateManager : BuildObject
    {
        #region Private Fields

        private static FileTemplateManager _manager;

        private bool _isInitialized;
        private BuildMultiMap<string, FileTemplate> _templates;
        private Dictionary<string, bool> _registeredPaths;

        #endregion

        #region Constructors and Destructor

        private FileTemplateManager()
        {
            _templates = new BuildMultiMap<string, FileTemplate>(StringComparer.OrdinalIgnoreCase);
            _registeredPaths = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether this file template manager is initialized.
        /// </summary>
        /// <value>
        /// If this property is <see langword="true"/>, this file template manager is
        /// successfully initialized; otherwise, it is not initialized.
        /// </value>
        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
        }

        public IEnumerable<string> RegisteredPaths
        {
            get
            {
                return _registeredPaths.Keys;
            }
        }        

        public IEnumerable<string> Categories
        {
            get
            {
                return _templates.Keys;
            }
        }

        public IList<FileTemplate> this[string category]
        {
            get
            {
                if (!String.IsNullOrEmpty(category))
                {
                    return _templates[category];
                }

                return null;
            }
        }

        public static FileTemplateManager Manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = new FileTemplateManager();
                }

                return _manager;
            }
        }

        #endregion

        #region Public Methods

        public void Initialize()
        {
            // If already initialized or no path is registered, we stop here...
            if (_isInitialized || _registeredPaths.Count == 0)
            {
                return;
            }

            // We use the search pattern: *.saft
            string searchPattern = "*" + TemplateConstants.FileExt;
            foreach (KeyValuePair<string, bool> pair in _registeredPaths)
            {
                // The key is the path and the value indicates whether a recursive search...
                string[] templateFiles = Directory.GetFiles(pair.Key, searchPattern, 
                    pair.Value ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                if (templateFiles != null && templateFiles.Length != 0)
                {
                    foreach (string templateFile in templateFiles)
                    {
                        FileTemplate template = new FileTemplate(templateFile);
                        template.Load();

                        if (!template.IsEmpty)
                        {
                            string category = template.Configuration.Category;
                            if (!String.IsNullOrEmpty(category))
                            {
                                _templates.Add(category, template);
                            }
                        }
                    }
                }
            }

            _isInitialized = true;
        }

        public void Uninitialize()
        {
            if (!_isInitialized)
            {
                return;
            }

            // Discard all the current templates...
            _templates = new BuildMultiMap<string, FileTemplate>(StringComparer.OrdinalIgnoreCase);

            _isInitialized = false;
        }

        public bool Register(string path, bool recursive)
        {
            BuildExceptions.PathMustExist(path, "path");
  
            // To make matching easier, we terminate all path with backslash...
            if (!path.EndsWith("\\"))  
            {
                path += "\\";
            }

            if (_registeredPaths.ContainsKey(path))
            {
                return false;
            }

            _registeredPaths.Add(path, recursive);

            return true;
        }

        public bool Unregister(string path)
        {
            BuildExceptions.NotNullNotEmpty(path, "path");

            // To make matching easier, we terminate all path with backslash...
            if (!path.EndsWith("\\"))
            {
                path += "\\";
            }

            return _registeredPaths.Remove(path);
        }

        #endregion
    }
}
