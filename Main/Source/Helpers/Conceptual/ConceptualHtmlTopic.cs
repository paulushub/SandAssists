using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Sandcastle.Contents;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualHtmlTopic : ConceptualItem
    {
        #region Private Fields

        private BuildKeyedList<ConceptualItem> _listItems;

        #endregion

        #region Constructors and Destructor

        public ConceptualHtmlTopic()
        {
        }

        public ConceptualHtmlTopic(BuildFilePath filePath, string topicTitle, 
            string topicId) : base(filePath, topicTitle, topicId)
        {
        }

        public ConceptualHtmlTopic(ConceptualHtmlTopic source)
            : base(source)
        {
            _listItems = source._listItems;
        }

        #endregion

        #region Public Properties

        public override ConceptualItemType ItemType
        {
            get
            {
                return ConceptualItemType.Html;
            }
        }

        public override int ItemCount
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

        public override ConceptualItem this[int index]
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems[index];
                }

                return null;
            }
        }

        public override ConceptualItem this[string topicId, bool recursive]
        {
            get
            {
                if (_listItems != null)
                {
                    return _listItems[topicId];
                }

                return null;
            }
        }

        public override IList<ConceptualItem> Items
        {
            get
            {
                if (_listItems != null)
                {
                    return new ReadOnlyCollection<ConceptualItem>(_listItems);
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public override bool CreateTopic(string ddueXmlDir, string ddueCompDir,
            string ddueHtmlDir)
        {
            if (!base.CreateTopic(ddueXmlDir, ddueCompDir, ddueHtmlDir))
            {
                return false;
            }

            if (_listItems != null && _listItems.Count > 0)
            {
                int itemCount = _listItems.Count;
                for (int i = 0; i < itemCount; i++)
                {
                    ConceptualItem item = _listItems[i];
                    if (item.Visible)
                    {
                        item.CreateTopic(ddueXmlDir, ddueCompDir, ddueHtmlDir);
                    }
                }
            }

            return true;
        }

        public override void Add(ConceptualItem item)
        {
            BuildExceptions.NotNull(item, "item");

            if (_listItems == null)
            {
                _listItems = new BuildKeyedList<ConceptualItem>();
            }

            Debug.Assert(this.Content != null);
            item.Content = this.Content;

            _listItems.Add(item);
        }

        public override void Insert(int index, ConceptualItem item)
        {
            BuildExceptions.NotNull(item, "item");

            Debug.Assert(this.Content != null);
            item.Content = this.Content;

            _listItems.Insert(index, item);
        }

        public override void Remove(int itemIndex)
        {
            if (_listItems == null || _listItems.Count == 0)
            {
                return;
            }

            _listItems.RemoveAt(itemIndex);
        }

        public override void Remove(ConceptualItem item)
        {
            if (item == null)
            {
                return;
            }
            if (_listItems == null || _listItems.Count == 0)
            {
                return;
            }

            _listItems.Remove(item);
        }

        public override void Clear()
        {
            if (_listItems != null)
            {
                _listItems = new BuildKeyedList<ConceptualItem>();
            }
        }

        #endregion

        #region Protected Methods

        protected override bool OnCreateTopic(string ddueXmlDir,
            string ddueCompDir, string ddueHtmlDir)
        {
            if (!Directory.Exists(ddueHtmlDir))
            {
                Directory.CreateDirectory(ddueHtmlDir);
            }

            BuildFilePath filePath = this.FilePath;
            if (filePath == null || !filePath.Exists)
            {
                return false;
            }

            string topicId = this.TopicId;
            if (String.IsNullOrEmpty(topicId))
            {
                return false;
            }

            string documentPath = Path.Combine(ddueHtmlDir, topicId + ".htm");
            if (File.Exists(documentPath))
            {
                File.SetAttributes(documentPath, FileAttributes.Normal);
                File.Delete(documentPath);
            }

            File.Copy(filePath, documentPath, true);
            File.SetAttributes(documentPath, FileAttributes.Normal);
   
            return true;
        }

        #endregion

        #region ICloneable Members

        public override ConceptualItem Clone()
        {
            ConceptualHtmlTopic item = new ConceptualHtmlTopic(this);
            if (_listItems != null)
            {
                item._listItems = _listItems.Clone();
            }

            return item;
        }

        #endregion
    }
}
