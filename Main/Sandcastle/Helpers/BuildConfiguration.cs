using System;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Configurations;

namespace Sandcastle
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class BuildConfiguration : BuildObject<BuildConfiguration>
    {
        #region Public Constant Fields

        /// <summary>
        /// Gets the name of the include configuration that is common to all build
        /// configurations.
        /// </summary>
        public const string IncludeDefault    = "Default";
        /// <summary>
        /// Gets the name of the include configuration that is specific to conceptual
        /// build configuration.
        /// </summary>
        public const string IncludeConceptual = "Conceptual";
        /// <summary>
        /// Gets the name of the include configuration that is specific to references 
        /// or API build configurations.
        /// </summary>
        public const string IncludeReferences = "References";

        #endregion

        #region Private Fields

        private IncludeContent _default;
        private IncludeContent _conceptual;
        private IncludeContent _references;

        #endregion

        #region Constructors and Destructor

        public BuildConfiguration()
        {
            _default    = new IncludeContent(IncludeDefault);
            _conceptual = new IncludeContent(IncludeConceptual);
            _references = new IncludeContent(IncludeReferences);
        }

        public BuildConfiguration(BuildConfiguration source)
            : base(source)
        {
            _default    = source._default;
            _conceptual = source._conceptual;
            _references = source._references;
        }

        #endregion

        #region Public Properties

        public IncludeItem this[int itemIndex]
        {
            get
            {
                return _default[itemIndex];
            }
        }

        public IncludeItem this[string itemName]
        {
            get
            {
                return _default[itemName];
            }
        }

        public IncludeItem this[string contentName, int itemIndex]
        {
            get
            {
                IncludeContent content = this.GetContent(contentName);
                if (content != null)
                {
                    return content[itemIndex];
                }

                return null;
            }
        }

        public IncludeItem this[string contentName, string itemName]
        {
            get
            {
                IncludeContent content = this.GetContent(contentName);
                if (content != null)
                {
                    return content[itemName];
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public IncludeContent GetContent(string contentName)
        {
            if (String.IsNullOrEmpty(contentName))
            {
                return _default;
            }
            if (String.Equals(contentName, IncludeDefault,
                StringComparison.CurrentCultureIgnoreCase))
            {
                return _default;
            }
            if (String.Equals(contentName, IncludeConceptual,
                StringComparison.CurrentCultureIgnoreCase))
            {
                return _conceptual;
            }
            if (String.Equals(contentName, IncludeReferences,
                StringComparison.CurrentCultureIgnoreCase))
            {
                return _references;
            }

            return null;
        }

        public void Add(string key, string value)
        {
            IncludeItem item = new IncludeItem(key, value);
            _default.Add(item);
        }

        public void Add(IncludeItem item)
        {
            _default.Add(item);
        }

        public void Add(string contentName, IncludeItem item)
        {
            IncludeContent content = this.GetContent(contentName);

            if (content != null)
            {
                content.Add(item);
            }
        }

        public void Add(IList<IncludeItem> items)
        {
            _default.Add(items);
        }

        public void Add(string contentName, IList<IncludeItem> items)
        {
            IncludeContent content = this.GetContent(contentName);

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
            IncludeContent content = this.GetContent(contentName);

            if (content != null)
            {
                content.Remove(index);
            }
        }

        public void Remove(IncludeItem item)
        {
            _default.Remove(item);
        }

        public void Remove(string contentName, IncludeItem item)
        {
            IncludeContent content = this.GetContent(contentName);

            if (content != null)
            {
                content.Remove(item);
            }
        }

        public bool Contains(IncludeItem item)
        {
            return _default.Contains(item);
        }

        public bool Contains(string contentName, IncludeItem item)
        {
            IncludeContent content = this.GetContent(contentName);

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
            IncludeContent content = this.GetContent(contentName); 

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

        #region ICloneable Members

        public override BuildConfiguration Clone()
        {
            BuildConfiguration config = new BuildConfiguration(this);

            if (_default != null)
            {
                config._default = _default.Clone();
            }
            if (_conceptual != null)
            {
                config._conceptual = _conceptual.Clone();
            }
            if (_references != null)
            {
                config._references = _references.Clone();
            }

            return config;
        }

        #endregion
    }
}
