using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle
{
    public abstract class BuildPathResolver : BuildObject
    {
        #region Private Fields

        private bool    _isInitialized;
        private string  _resolverId;
        internal string _basePath;

        private static object _synchObject = new object();

        private static Stack<BuildPathResolver> _resolvers = 
            new Stack<BuildPathResolver>();

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildPathProvider"/> class
        /// to the default properties or values.
        /// </summary>
        protected BuildPathResolver()
        {
            _resolverId = Guid.NewGuid().ToString();
        }

        protected BuildPathResolver(string resolverId)
            : this()
        {
            if (!String.IsNullOrEmpty(resolverId))
            {
                _resolverId = resolverId;
            }
        }

        #endregion

        #region Public Properties

        public string Id
        {
            get
            {
                return _resolverId;
            }
        }

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }
            protected set
            {
                _isInitialized = value;
            }
        }

        public string BasePath
        {
            get
            {
                return _basePath;
            }
        }

        public static object SyncRoot 
        { 
            get
            {
                return _synchObject;
            }
        }

        public static BuildPathResolver Resolver
        {
            get
            {
                if (_resolvers.Count == 0)
                {   
                    _resolvers.Push(SystemPathResolver.Create());
                }

                return _resolvers.Peek();
            }
        }

        public static object Push(BuildPathResolver resolver)
        {
            BuildExceptions.NotNull(resolver, "resolver");

            lock (_synchObject)
            {
                _resolvers.Push(resolver);

                return _synchObject;
            }
        }

        public static BuildPathResolver Pop()
        {
            if (!Monitor.TryEnter(_synchObject))
            {
                // If is locked by the user...
                if (_resolvers.Count != 0)
                {
                    return _resolvers.Pop();
                }  
            }

            // We have locked it...
            BuildPathResolver resolver = null;
            if (_resolvers.Count != 0)
            {
                resolver = _resolvers.Pop();
            }
            Monitor.Exit(_synchObject);

            return resolver;
        }

        public static void Clear()
        {
            lock (_synchObject)
            {
                _resolvers.Clear();
            }
        }

        #endregion

        #region Public Methods

        public virtual void Initialize(string basePath)
        {
            BuildExceptions.PathMustExist(basePath, "basePath");

            _basePath      = basePath;
            // Check the required condition, which works for all cases...
            if (!_basePath.EndsWith("\\"))
            {
                _basePath += "\\";
            }

            _isInitialized = true;
        }

        public virtual void Uninitialize()
        {
            _basePath       = null;
            _isInitialized = false;
        }

        public abstract string ResolveAbsolute(string relativePath);

        public abstract string ResolveRelative(string absolutePath);

        public static BuildPathResolver Create(string basePath)
        {
            if (String.IsNullOrEmpty(basePath))
            {
                basePath = Environment.CurrentDirectory;
            }

            SystemPathResolver resolver = new SystemPathResolver();
            resolver.Initialize(basePath);

            return resolver;
        }

        public static BuildPathResolver Create(string basePath,
            string resolverId)
        {
            if (String.IsNullOrEmpty(basePath))
            {
                basePath = Environment.CurrentDirectory;
            }

            SystemPathResolver resolver = new SystemPathResolver(resolverId);
            resolver.Initialize(basePath);

            return resolver;
        }

        #endregion

        #region SystemPathResolver Class

        private sealed class SystemPathResolver : BuildPathResolver
        {
            #region Private Fields

            #endregion

            #region Constructors and Destructor

            public SystemPathResolver()
            {
            }

            public SystemPathResolver(string resolverId)
                : base(resolverId)
            {
            }

            #endregion

            #region Public Methods

            public override void Initialize(string basePath)
            {
                base.Initialize(basePath);
            }

            public override void Uninitialize()
            {
                base.Uninitialize();
            }

            public override string ResolveAbsolute(string relativePath)
            {
                BuildExceptions.NotNullNotEmpty(relativePath, "relativePath");

                if (!this.IsInitialized)
                {
                    throw new BuildException("The path resolver is not initialized.");
                }

                relativePath = Environment.ExpandEnvironmentVariables(relativePath);
                if (Path.IsPathRooted(relativePath))
                {
                    return relativePath;
                }

                // This is the most reliable so far on tests...
                return Path.GetFullPath(Path.Combine(_basePath, relativePath));
            }

            public override string ResolveRelative(string absolutePath)
            {
                BuildExceptions.NotNullNotEmpty(absolutePath, "absolutePath");

                if (!this.IsInitialized)
                {
                    throw new BuildException("The path resolver is not initialized.");
                }

                absolutePath = Environment.ExpandEnvironmentVariables(absolutePath);
                if (!Path.IsPathRooted(absolutePath))
                {
                    return absolutePath;
                }

                return PathUtils.GetRelativePath(_basePath, absolutePath);
            }

            public static SystemPathResolver Create()
            {
                SystemPathResolver resolver = new SystemPathResolver();
                resolver.Initialize(Environment.CurrentDirectory);

                return resolver;
            }

            #endregion
        }

        #endregion
    }
}
