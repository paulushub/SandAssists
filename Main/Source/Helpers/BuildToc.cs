using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Sandcastle.Contents;

namespace Sandcastle
{
    /// <summary>
    /// This provides contents and interfaces for customizing the table of content of 
    /// the documentation.
    /// </summary>
    /// <remarks>
    /// You can use this class to create the table of content customization based on
    /// the currently supported procedure or extend this class to provide your own
    /// table of content processing.
    /// </remarks>
    [Serializable]
    public class BuildToc : BuildOptions<BuildToc>
    {
        #region Public Static Fields

        /// <summary>
        /// 
        /// </summary>
        public const string HelpToc         = "HelpToc.xml";
        public const string HierarchicalToc = "HierarchicalToc.xml";

        #endregion

        #region Private Fields

        private BuildList<TocContent> _listItems;

        #endregion

        #region Constructor and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildToc"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildToc"/> class
        /// to the default properties or values.
        /// </summary>
        public BuildToc()
        {
            _listItems = new BuildList<TocContent>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildToc"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildToc"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildToc"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public BuildToc(BuildToc source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public virtual int Count
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems.Count;
                }

                return 0;
            }
        }

        public virtual TocContent this[int index]
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems[index];
                }

                return null;
            }
            set
            {
                if (value != null)
                {
                    _listItems[index] = value;
                }
            }
        }

        public virtual IList<TocContent> Items
        {
            get
            {
                if (_listItems != null)
                {
                    return new ReadOnlyCollection<TocContent>(_listItems);
                }

                return null;
            }
        }

        public virtual bool IsEmpty
        {
            get
            {
                return (_listItems == null || _listItems.Count == 0);
            }
        }

        #endregion

        #region Public Methods

        public override void Initialize(BuildContext context)
        {
            base.Initialize(context);
        }

        public override void Uninitialize()
        {
            base.Uninitialize();
        }

        public virtual bool Merge(BuildContext context)
        {
            return true;
        }

        public virtual void Add(TocContent item)
        {
            BuildExceptions.NotNull(item, "item");

            _listItems.Add(item);
        }

        public virtual void Add(IList<TocContent> items)
        {
            BuildExceptions.NotNull(items, "items");

            int itemCount = items.Count;
            for (int i = 0; i < itemCount; i++)
            {
                this.Add(items[i]);
            }
        }

        public virtual void Remove(int index)
        {
            if (_listItems.Count == 0)
            {
                return;
            }

            _listItems.RemoveAt(index);
        }

        public virtual void Remove(TocContent item)
        {
            BuildExceptions.NotNull(item, "item");

            if (_listItems.Count == 0)
            {
                return;
            }

            _listItems.Remove(item);
        }

        public virtual bool Contains(TocContent item)
        {
            if (item == null || _listItems.Count == 0)
            {
                return false;
            }

            return _listItems.Contains(item);
        }

        public virtual void Clear()
        {
            if (_listItems.Count == 0)
            {
                return;
            }

            _listItems.Clear();
        }

        #endregion

        #region ICloneable Members

        /// <overloads>
        /// This creates a new build custom table of content that is a deep copy of 
        /// the current instance.
        /// </overloads>
        /// <summary>
        /// This creates a new build custom table of content that is a deep copy of 
        /// the current instance.
        /// </summary>
        /// <returns>
        /// A new build custom table of content that is a deep copy of this instance.
        /// </returns>
        /// <remarks>
        /// This is deep cloning of the members of this build custom table of content. 
        /// If you need just a copy, use the copy constructor to create a new instance.
        /// </remarks>
        public override BuildToc Clone()
        {
            BuildToc helpToc = new BuildToc(this);
            int itemCount = _listItems.Count;

            for (int i = 0; i < itemCount; i++)
            {
                helpToc._listItems.Add(_listItems[i].Clone());
            }

            return helpToc;
        }

        #endregion
    }
}
