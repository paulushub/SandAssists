using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class BibliographyContent : BuildContent<BibliographyItem, BibliographyContent>
    {
        #region Private Fields

        private bool   _isLoaded;

        private string _contentsFile;
        [NonSerialized]
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public BibliographyContent()
            : base(new BuildKeyedList<BibliographyItem>())
        {
            BuildKeyedList<BibliographyItem> keyedList =
                this.List as BuildKeyedList<BibliographyItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }
        }

        public BibliographyContent(string contentsFile)
            : this()
        {
            _contentsFile = contentsFile;
        }

        public BibliographyContent(BibliographyContent source)
            : base(source)
        {
            _isLoaded     = source._isLoaded;
            _dicItems     = source._dicItems;
            _contentsFile = source._contentsFile;
        }

        #endregion

        #region Public Properties

        public override bool IsEmpty
        {
            get
            {
                if (!String.IsNullOrEmpty(_contentsFile))
                {
                    return false;
                }

                return base.IsEmpty;
            }
        }

        public bool IsLoaded
        {
            get
            {
                if (String.IsNullOrEmpty(_contentsFile))
                {
                    return false;
                }

                return _isLoaded;
            }
        }

        public string ContentsFile
        {
            get
            {
                return _contentsFile;
            }
            set
            {
                _contentsFile = value;
            }
        }

        public BibliographyItem this[string itemName]
        {
            get
            {
                if (String.IsNullOrEmpty(itemName))
                {
                    return null;
                }

                int curIndex = -1;
                if (_dicItems != null &&
                    _dicItems.TryGetValue(itemName, out curIndex))
                {
                    return this[curIndex];
                }

                return null;
            }
        }

        public override bool IsKeyed
        {
            get
            {
                return true;
            }
        }

        #endregion

        #region Public Method

        public void Load()
        {
            this.Load(_contentsFile);
        }

        public void Load(string contentFile)
        {
            BuildExceptions.PathMustExist(contentFile, "contentFile");

            _isLoaded = true;
        }

        public void Save()
        {
            this.Save(_contentsFile);
        }

        public void Save(string contentFile)
        {
            BuildExceptions.NotNullNotEmpty(contentFile, "contentFile");
        }

        public void Add(string contentFile)
        {
            BuildExceptions.NotNullNotEmpty(contentFile, "contentFile");
        }

        public override void Add(BibliographyItem item)
        {
            if (item != null && !String.IsNullOrEmpty(item.Name))
            {   
                if (_dicItems.ContainsKey(item.Name))
                {
                    this.Insert(_dicItems[item.Name], item);
                }
                else
                {
                    base.Add(item);
                }
            }
        }

        public bool Contains(string itemName)
        {
            if (String.IsNullOrEmpty(itemName) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return false;
            }

            return _dicItems.ContainsKey(itemName);
        }

        public int IndexOf(string itemName)
        {
            if (String.IsNullOrEmpty(itemName) ||
                _dicItems == null || _dicItems.Count == 0)
            {
                return -1;
            }

            if (_dicItems.ContainsKey(itemName))
            {
                return _dicItems[itemName];
            }

            return -1;
        }

        public bool Remove(string itemName)
        {
            int itemIndex = this.IndexOf(itemName);
            if (itemIndex < 0)
            {
                return false;
            }

            if (_dicItems.Remove(itemName))
            {
                base.Remove(itemIndex);

                return true;
            }

            return false;
        }

        public override bool Remove(BibliographyItem item)
        {
            if (base.Remove(item))
            {
                if (_dicItems != null && _dicItems.Count != 0)
                {
                    _dicItems.Remove(item.Name);
                }

                return true;
            }

            return false;
        }

        public override void Clear()
        {
            if (_dicItems != null && _dicItems.Count != 0)
            {
                _dicItems.Clear();
            }

            base.Clear();
        }

        #endregion

        #region IXmlSerializable Members

        public override void ReadXml(XmlReader reader)
        {
        }

        public override void WriteXml(XmlWriter writer)
        {
        }

        #endregion

        #region ICloneable Members

        public override BibliographyContent Clone()
        {
            BibliographyContent content = new BibliographyContent(this);

            if (_contentsFile != null)
            {
                content._contentsFile = String.Copy(_contentsFile);
            }

            this.Clone(content, new BuildKeyedList<BibliographyItem>());

            return content;
        }

        #endregion
    }
}
