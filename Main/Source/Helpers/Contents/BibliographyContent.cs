using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public class BibliographyContent : BuildContent<BibliographyItem, BibliographyContent>
    {
        #region Private Fields

        private string _contentsFile;
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
            _contentsFile = source._contentsFile;
        }

        #endregion

        #region Public Properties

        public override bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_contentsFile) == false)
                {
                    return false;
                }

                return base.IsEmpty;
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
