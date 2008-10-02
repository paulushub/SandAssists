using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle
{
    [Serializable]
    public class BuildStyle : BuildObject<BuildStyle>
    {
        #region Public Constant Fields

        /// <summary>
        /// Gets the name of the shared content that is common to all build
        /// configurations.
        /// </summary>
        public const string SharedDefault    = "Default";
        /// <summary>
        /// Gets the name of the shared content that is specific to conceptual
        /// build configuration.
        /// </summary>
        public const string SharedConceptual = "Conceptual";
        /// <summary>
        /// Gets the name of the shared content that is specific to references or API
        /// build configurations.
        /// </summary>
        public const string SharedReferences = "References";

        #endregion

        #region Private Fields

        private string         _styleName;
        private string         _styleDir;
        private string         _stylePresent;
        private BuildStyleType _styleType;

        private SharedContent _default;
        private SharedContent _conceptual;
        private SharedContent _references;

        #endregion

        #region Constructor and Destructor

        public BuildStyle()
            : this(BuildStyleType.Vs2005)
        {
        }

        public BuildStyle(BuildStyleType type)
        {
            _styleType  = type;
            _default    = new SharedContent(SharedDefault, String.Empty);
            _conceptual = new SharedContent(SharedConceptual, String.Empty);
            _references = new SharedContent(SharedReferences, String.Empty);
        }

        public BuildStyle(string name, string directory, 
            BuildStyleType type) : this(type)
        {
            _styleName = name;
            _styleDir  = directory;
        }

        public BuildStyle(BuildStyle source)
            : base(source)
        {
            _styleDir     = source._styleDir;
            _styleName    = source._styleName;
            _stylePresent = source._stylePresent;
            _styleType    = source._styleType;

            _default    = source._default;
            _conceptual = source._conceptual;
            _references = source._references;
        }

        #endregion

        #region Public Properties

        public string Name
        {
            get 
            { 
                return _styleName; 
            }
            set 
            { 
                _styleName = value; 
            }
        }

        public string Directory
        {
            get 
            { 
                return _styleDir; 
            }
            set 
            { 
                _styleDir = value; 
            }
        }

        public string Presentation
        {
            get
            {
                return _stylePresent;
            }
            set
            {
                _stylePresent = value;
            }
        }

        public BuildStyleType StyleType
        {
            get 
            { 
                return _styleType; 
            }
            set 
            { 
                _styleType = value; 
            }
        }

        public SharedItem this[int itemIndex]
        {
            get
            {
                return _default[itemIndex];
            }
        }

        public SharedItem this[string itemName]
        {
            get
            {
                return _default[itemName];
            }
        }

        public SharedItem this[string contentName, int itemIndex]
        {
            get
            {
                SharedContent content = this.GetContent(contentName);
                if (content != null)
                {
                    return content[itemIndex];
                }

                return null;
            }
        }

        public SharedItem this[string contentName, string itemName]
        {
            get
            {
                SharedContent content = this.GetContent(contentName);
                if (content != null)
                {
                    return content[itemName];
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        #region SharedContent Methods

        public SharedContent GetContent(string contentName)
        {
            if (String.IsNullOrEmpty(contentName))
            {
                return _default;
            }
            if (String.Equals(contentName, SharedDefault,
                StringComparison.CurrentCultureIgnoreCase))
            {
                return _default;
            }
            if (String.Equals(contentName, SharedConceptual,
                StringComparison.CurrentCultureIgnoreCase))
            {
                return _conceptual;
            }
            if (String.Equals(contentName, SharedReferences,
                StringComparison.CurrentCultureIgnoreCase))
            {
                return _references;
            }

            return null;
        }

        public void Add(SharedItem item)
        {
            _default.Add(item);
        }

        public void Add(string contentName, SharedItem item)
        {
            SharedContent content = this.GetContent(contentName);

            if (content != null)
            {
                content.Add(item);
            }
        }

        public void Add(IList<SharedItem> items)
        {
            _default.Add(items);
        }

        public void Add(string contentName, IList<SharedItem> items)
        {
            SharedContent content = this.GetContent(contentName);

            if (content != null)
            {
                content.Add(items);
            }
        }

        public void Remove(int index)
        {
            _default.Remove(index);
        }

        public void Remove(string contentName, int index)
        {
            SharedContent content = this.GetContent(contentName);

            if (content != null)
            {
                content.Remove(index);
            }
        }

        public void Remove(SharedItem item)
        {
            _default.Remove(item);
        }

        public void Remove(string contentName, SharedItem item)
        {
            SharedContent content = this.GetContent(contentName);

            if (content != null)
            {
                content.Remove(item);
            }
        }

        public bool Contains(SharedItem item)
        {
            return _default.Contains(item);
        }

        public bool Contains(string contentName, SharedItem item)
        {
            SharedContent content = this.GetContent(contentName);

            if (content != null)
            {
                return content.Contains(item);
            }

            return false;
        }

        public void Clear()
        {
            if (_default.Count == 0)
            {
                return;
            }

            _default.Clear();
        }

        public void Clear(string contentName)
        {
            SharedContent content = this.GetContent(contentName);

            if (content != null && content.Count != 0)
            {
                content.Clear();
            }
        }

        public void ClearAll()
        {
            if (_default.Count != 0)
            {
                _default.Clear();
            }
            if (_conceptual.Count != 0)
            {
                _conceptual.Clear();
            }
            if (_references.Count != 0)
            {
                _references.Clear();
            }
        }

        #endregion

        #region GetSkeleton Method

        public virtual string GetSkeleton(BuildEngineType engineType)
        {
            //<data file="%DXROOT%\Presentation\Vs2005\transforms\skeleton_conceptual.xml" />
            //<data file="%DXROOT%\Presentation\vs2005\Transforms\skeleton.xml" />
            string path = _styleDir;
            if (String.IsNullOrEmpty(_styleDir) ||
                System.IO.Directory.Exists(_styleDir) == false)
            {
                path = "%DXROOT%";
            }
            path = Path.Combine(path, @"Presentation\");
            string skeleton = null;
            if (engineType == BuildEngineType.Conceptual)
            {
                skeleton = "skeleton_conceptual.xml";
            }
            else if (engineType == BuildEngineType.Reference)
            {
                skeleton = "skeleton.xml";
            }
            if (String.IsNullOrEmpty(skeleton))
            {
                return null;
            }
            BuildStyleType styleType = _styleType;
            if (styleType == BuildStyleType.Whidbey)
            {
                styleType = BuildStyleType.Vs2005;
            }

            return path + String.Format(@"{0}\Transforms\{1}", 
                styleType.ToString(), skeleton);
        }

        #endregion

        #region GetTransform Method

        public virtual string GetTransform(BuildEngineType engineType)
        {
            //<transform file="%DXROOT%\Presentation\Vs2005\transforms\main_conceptual.xsl">
            //<transform file="%DXROOT%\Presentation\vs2005\Transforms\main_sandcastle.xsl">
            string path = _styleDir;
            if (String.IsNullOrEmpty(_styleDir) ||
                System.IO.Directory.Exists(_styleDir) == false)
            {
                path = "%DXROOT%";
            }
            path = Path.Combine(path, @"Presentation\");
            string transform = null;
            if (engineType == BuildEngineType.Conceptual)
            {
                transform = "main_conceptual.xsl";
            }
            else if (engineType == BuildEngineType.Reference)
            {
                transform = "main_sandcastle.xsl";
            }
            if (String.IsNullOrEmpty(transform))
            {
                return null;
            }
            BuildStyleType styleType = _styleType;
            if (styleType == BuildStyleType.Whidbey)
            {
                styleType = BuildStyleType.Vs2005;
            }

            return path + String.Format(@"{0}\Transforms\{1}",
                styleType.ToString(), transform);
        }

        #endregion

        #region GetSharedContents Method

        public virtual IList<string> GetSharedContents(BuildEngineType engineType)
        {
            List<string> sharedContents = new List<string>();
            string path = _styleDir;
            if (String.IsNullOrEmpty(_styleDir) || 
                System.IO.Directory.Exists(_styleDir) == false)
            {
                path = "%DXROOT%";
            }
            path = Path.Combine(path, @"Presentation\");

            if (engineType == BuildEngineType.Conceptual)
            {   
                if (_styleType == BuildStyleType.Vs2005 ||
                    _styleType == BuildStyleType.Whidbey)
                {   
                    //<content file="%DXROOT%\Presentation\Vs2005\content\shared_content.xml" />
                    //<content file="%DXROOT%\Presentation\VS2005\content\feedBack_content.xml" />
                    //<content file="%DXROOT%\Presentation\Vs2005\content\conceptual_content.xml" />

                    sharedContents.Add(Path.Combine(path,
                        @"Vs2005\Content\shared_content.xml"));
                    sharedContents.Add(Path.Combine(path,
                        @"Vs2005\Content\feedBack_content.xml"));
                    sharedContents.Add(Path.Combine(path,
                        @"Vs2005\Content\conceptual_content.xml"));
                }
                else if (_styleType == BuildStyleType.Hana)
                {   
                    //<content file="%DXROOT%\Presentation\Hana\content\shared_content.xml" />
                    //<content file="%DXROOT%\Presentation\Hana\content\conceptual_content.xml" />
                    
                    sharedContents.Add(Path.Combine(path,
                        @"Hana\Content\shared_content.xml"));
                    sharedContents.Add(Path.Combine(path,
                        @"Hana\Content\conceptual_content.xml"));
                }
                else if (_styleType == BuildStyleType.Prototype)
                {
                    //<content file="%DXROOT%\Presentation\Prototype\content\shared_content.xml" />
                    //<content file="%DXROOT%\Presentation\Prototype\content\conceptual_content.xml" />

                    sharedContents.Add(Path.Combine(path,
                        @"Prototype\Content\shared_content.xml"));
                    sharedContents.Add(Path.Combine(path,
                        @"Prototype\Content\conceptual_content.xml"));
                }
            }
            else if (engineType == BuildEngineType.Reference)
            {
                if (_styleType == BuildStyleType.Vs2005 ||
                    _styleType == BuildStyleType.Whidbey)
                {
                    //<content file="%DXROOT%\Presentation\vs2005\content\shared_content.xml" />
                    //<content file="%DXROOT%\Presentation\vs2005\content\reference_content.xml" />
                    //<content file="%DXROOT%\Presentation\shared\content\syntax_content.xml" />
                    //<content file="%DXROOT%\Presentation\vs2005\content\feedback_content.xml" />

                    sharedContents.Add(Path.Combine(path,
                        @"Vs2005\Content\shared_content.xml"));
                    sharedContents.Add(Path.Combine(path,
                        @"Vs2005\Content\reference_content.xml"));
                    sharedContents.Add(Path.Combine(path,
                        @"Shared\Content\syntax_content.xml"));
                    sharedContents.Add(Path.Combine(path,
                        @"Vs2005\Content\feedBack_content.xml"));
                }
                else if (_styleType == BuildStyleType.Hana)
                {
                    //<content file="%DXROOT%\Presentation\hana\content\shared_content.xml" />
                    //<content file="%DXROOT%\Presentation\hana\content\reference_content.xml" />
                    //<content file="%DXROOT%\Presentation\shared\content\syntax_content.xml" />

                    sharedContents.Add(Path.Combine(path,
                        @"Hana\Content\shared_content.xml"));
                    sharedContents.Add(Path.Combine(path,
                        @"Hana\Content\reference_content.xml"));
                    sharedContents.Add(Path.Combine(path,
                        @"Shared\Content\syntax_content.xml"));
                }
                else if (_styleType == BuildStyleType.Prototype)
                {
                    //<content file="%DXROOT%\Presentation\Prototype\content\shared_content.xml" />
                    //<content file="%DXROOT%\Presentation\Prototype\content\reference_content.xml" />
                    //<content file="%DXROOT%\Presentation\Shared\content\syntax_content.xml" />

                    sharedContents.Add(Path.Combine(path,
                        @"Prototype\Content\shared_content.xml"));
                    sharedContents.Add(Path.Combine(path,
                        @"Prototype\Content\reference_content.xml"));
                    sharedContents.Add(Path.Combine(path,
                        @"Shared\Content\syntax_content.xml"));
                }
            }

            return sharedContents;
        }

        #endregion

        #endregion

        #region ICloneable Members

        public override BuildStyle Clone()
        {
            BuildStyle style = new BuildStyle(this);

            if (_default != null)
            {
                style._default = _default.Clone();
            }
            if (_conceptual != null)
            {
                style._conceptual = _conceptual.Clone();
            }
            if (_references != null)
            {
                style._references = _references.Clone();
            }

            return style;
        }

        #endregion
    }
}
