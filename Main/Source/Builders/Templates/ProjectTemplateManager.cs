using System;
using System.IO;
using System.Collections.Generic;

namespace Sandcastle.Builders.Templates
{
    public sealed class ProjectTemplateManager : BuildObject
    {
        #region Private Fields

        private static ProjectTemplateManager _manager;

        private bool _isInitialized;
        private BuildMultiMap<string, ProjectTemplate> _templates;
        private Dictionary<string, bool> _registeredPaths;

        #endregion

        #region Constructors and Destructor

        private ProjectTemplateManager()
        {
            _templates       = new BuildMultiMap<string, ProjectTemplate>(
                StringComparer.OrdinalIgnoreCase);
            _registeredPaths = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region Public Properties

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

        public IList<ProjectTemplate> this[string category]
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

        public static ProjectTemplateManager Manager
        {
            get
            {
                if (_manager == null)
                {
                    _manager = new ProjectTemplateManager();
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

            // We use the search pattern: *.sapt
            string searchPattern = "*" + TemplateConstants.ProjectExt;
            foreach (KeyValuePair<string, bool> pair in _registeredPaths)
            {
                // The key is the path and the value indicates whether a recursive search...
                string[] templateFiles = Directory.GetFiles(pair.Key, searchPattern, 
                    pair.Value ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);

                if (templateFiles != null && templateFiles.Length != 0)
                {
                    foreach (string templateFile in templateFiles)
                    {
                        ProjectTemplate template = new ProjectTemplate(templateFile);
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
            _templates = new BuildMultiMap<string, ProjectTemplate>(StringComparer.OrdinalIgnoreCase);

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
