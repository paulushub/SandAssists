using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Builders.Contents
{
    [Serializable]
    public sealed class DdueXmlMetadata : DdueXmlObject<DdueXmlMetadata>
    {
        #region Private Fields

        private string _topicType;
        private string _runningHeaderText;

        private List<DdueXmlMetaItem> _listItems;

        #endregion

        #region Constructors and Destructor

        public DdueXmlMetadata()
        {
            _listItems = new List<DdueXmlMetaItem>();
        }

        public DdueXmlMetadata(string topicType, string runningHeaderText)
        {
            if (topicType == null)
            {
                throw new ArgumentNullException("topicType");
            }
            if (topicType.Length == 0)
            {
                throw new ArgumentException("topicType");
            }
            if (runningHeaderText == null)
            {
                throw new ArgumentNullException("runningHeaderText");
            }
            if (runningHeaderText.Length == 0)
            {
                throw new ArgumentException("runningHeaderText");
            }

            _topicType         = topicType;
            _runningHeaderText = runningHeaderText;
            _listItems         = new List<DdueXmlMetaItem>();
        }

        public DdueXmlMetadata(DdueXmlMetadata source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public string TopicType
        {
            get
            {
                return _topicType;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _topicType = value;
                }
            }
        }

        public string RunningHeaderText
        {
            get
            {
                return _runningHeaderText;
            }
            set
            {
                if (!String.IsNullOrEmpty(value))
                {
                    _runningHeaderText = value;
                }
            }
        }

        public DdueXmlMetaItem this[int index]
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

        public int ItemCount
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

        public IList<DdueXmlMetaItem> Items
        {
            get
            {
                return _listItems;
            }
        }

        #endregion

        #region Public Methods

        public void AddItem(string id, string name, string analysisProperty,
            string contentSet, string apProperty, string catalog,
            string valueId, string text)
        {
            DdueXmlMetaItem item = new DdueXmlMetaItem(id, name, 
                analysisProperty, contentSet, apProperty, catalog, 
                valueId, text);

            _listItems.Add(item);
        }

        public void AddItems()
        {
            // These values or options are how they affect the output is not
            // well-known. Until, the meaning is made public, we use the 
            // defaults as provided in the sample files...
            this.AddItem("4b91e890-68ce-4966-b685-75eacf1b596e", 
                "Catalog Container", "False", "False", "False", "True",  
                "b8052700-9597-436d-a02e-e9f139e28af8", 
                "System_Default_Catalog");
            this.AddItem("8ff54c74-d745-4a73-b058-023e47952242", 
                "CommunityContent", "False", "False", "False", "False", 
                "fe7d4cf5-877c-459c-a38e-0d9ce9796ab7", "0");
            this.AddItem("3785f925-c5d5-4d0b-9dd6-fd1d27a9e5c5", 
                "Content Set Container", "False", "True", "False", "False", 
                "c02fe60b-26c3-48db-9548-1b1cde9322df", 
                "System_Default_Content_Set");
            this.AddItem("9bf5165a-6803-4225-8a17-88d45ddc58ff", "DevLang", 
                "False", "False", "False", "False", 
                "95b32abd-126d-4071-900c-c2a0e23ae271", "aspx");
            this.AddItem("9bf5165a-6803-4225-8a17-88d45ddc58ff", "DevLang", 
                "False", "False", "False", "False", 
                "26ae1a47-ce90-4c67-9f71-ae3a566ccad0", "C++");
            this.AddItem("9bf5165a-6803-4225-8a17-88d45ddc58ff", "DevLang", 
                "False", "False", "False", "False", 
                "af7ca145-38b8-4e13-b414-ac79f10c1922", "CSharp");
            this.AddItem("9bf5165a-6803-4225-8a17-88d45ddc58ff", "DevLang", 
                "False", "False", "False", "False", 
                "ce1c8d7b-1d73-438a-a55c-f2d883e702f8", "jsharp");
            this.AddItem("9bf5165a-6803-4225-8a17-88d45ddc58ff", "DevLang", 
                "False", "False", "False", "False", 
                "96f36197-4b00-4c4d-b445-6512301ccb88", "VB");
            this.AddItem("80d89f21-9447-4d7d-a071-d6b488143d19", "ShippedIn", 
                "False", "False", "False", "False", 
                "3c19c0af-50f7-44e9-8b85-e16dcbcb1401", "vs.90");
        }

        public void Read(XmlReader reader)
        {
        }

        public void Write(XmlWriter writer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            // Write the runningHeaderText tag...
            writer.WriteStartElement("runningHeaderText");
            writer.WriteAttributeString("uscid", _runningHeaderText);
            writer.WriteEndElement();

            // Write the topicType tag...
            writer.WriteStartElement("topicType");
            writer.WriteAttributeString("id", _topicType);
            writer.WriteEndElement();

            // Finally, write the attribute tag...
            if (_listItems == null)
            {
                return;
            }

            int itemCount = _listItems.Count;
            for (int i = 0; i < itemCount; i++)
            {
                DdueXmlMetaItem metaItem = _listItems[i];
                if (metaItem == null)
                {
                    continue;
                }

                metaItem.Write(writer);
            }
        }

        #endregion

        #region ICloneable Members

        public override DdueXmlMetadata Clone()
        {
            DdueXmlMetadata metadata = new DdueXmlMetadata(this);

            return metadata;
        }

        #endregion
    }
}
