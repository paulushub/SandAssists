using System;
using System.Collections.Generic;

namespace Sandcastle
{
    [Serializable]  
    public abstract class BuildGroupTocInfo : BuildTocInfo
    {
        #region Private Fields

        private bool _exclude;
        private string _tocFile;

        #endregion

        #region Constructors and Destructor

        protected BuildGroupTocInfo()
            : this(Guid.NewGuid().ToString())
        {
        }

        protected BuildGroupTocInfo(string name)
            : base(name)
        {
        }

        protected BuildGroupTocInfo(string name, string tocFile)
            : base(name)
        {
            BuildExceptions.NotNull(tocFile, "tocFile");

            _tocFile = tocFile;
        }

        protected BuildGroupTocInfo(BuildGroupTocInfo source)
            : base(source)
        {
            _exclude = source._exclude;
            _tocFile = source._tocFile;
        }

        #endregion

        #region Public Properties

        public override bool IsEmpty
        {
            get
            {
                if (base.IsEmpty)
                {
                    return true;
                }

                return String.IsNullOrEmpty(_tocFile);
            }
        }

        public abstract bool IsRooted
        {
            get;
        }

        public string TocFile
        {
            get
            {
                return _tocFile;
            }
            set
            {
                _tocFile = value;
            }
        }

        public bool Exclude
        {
            get
            {
                return _exclude;
            }
            set
            {
                _exclude = value;
            }
        }

        public abstract bool IsLoaded
        {
            get;
        }

        public abstract int Count
        {
            get;
        }

        public abstract BuildTopicTocInfo this[int index]
        {
            get;
        }

        public abstract BuildTopicTocInfo this[string name]
        {
            get;
        }

        public abstract IList<BuildTopicTocInfo> Items
        {
            get;
        }

        #endregion

        #region Public Methods

        public virtual void Add(BuildTopicTocInfo item)
        {
        }

        public virtual void AddRange(IList<BuildTopicTocInfo> items)
        {
        }

        public virtual void AddRange(BuildGroupTocInfo groupTocInfo)
        {
        }

        public virtual void Insert(int index, BuildTopicTocInfo item)
        {
        }

        public virtual void InsertRange(int index, IList<BuildTopicTocInfo> items)
        {
        }

        public virtual void InsertRange(int index, BuildGroupTocInfo groupTocInfo)
        {
        }

        public virtual void Replace(BuildTopicTocInfo itemOut, BuildTopicTocInfo itemIn)
        {
        }

        public virtual void Replace(BuildTopicTocInfo itemOut,
            IList<BuildTopicTocInfo> items)
        {
        }

        public virtual void Replace(BuildTopicTocInfo itemOut, 
            BuildGroupTocInfo groupTocInfo)
        {
        }

        public virtual void Remove(int itemIndex)
        {
        }

        public virtual void Remove(BuildTopicTocInfo item)
        {
        }

        public virtual int IndexOf(BuildTopicTocInfo item)
        {
            return -1;
        }

        public virtual void Clear()
        {
        }

        #endregion
    }
}
