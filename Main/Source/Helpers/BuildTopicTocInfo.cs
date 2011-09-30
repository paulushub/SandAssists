using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle
{
    [Serializable]
    public sealed class BuildTopicTocInfo : BuildTocInfo
    {
        #region Public Fields

        public const string TagName = "topic";

        #endregion

        #region Private Fields

        private string _source;
        private string _container;
        private BuildTopicTocInfo _parent;
        private BuildKeyedList<BuildTopicTocInfo> _listTopics;

        #endregion

        #region Constructors and Destructor

        public BuildTopicTocInfo()
            : this(Guid.NewGuid().ToString(), String.Empty, null)
        {
        }

        public BuildTopicTocInfo(string name, string source, BuildTopicTocInfo parent)
            : base(name)
        {
            if (String.IsNullOrEmpty(source))
            {
                source = Guid.NewGuid().ToString();
            }

            _source     = source;
            _parent     = parent;
        }

        public BuildTopicTocInfo(BuildTopicTocInfo source)
            : base(source)
        {
            _source     = source._source;
            _container  = source._container;
            _parent     = source._parent;
            _listTopics = source._listTopics;
        }

        #endregion

        #region Public Properties

        public override BuildTocInfoType ItemType
        {
            get
            {
                return BuildTocInfoType.Topic;
            }
        }

        public override bool IsEmpty
        {
            get
            {
                if (base.IsEmpty)
                {
                    return true;
                }

                return String.IsNullOrEmpty(_source);
            }
        }

        public string Source
        {
            get
            {
                return _source;
            }
            set
            {
                if (value == null)
                {
                    _source = String.Empty;
                }
                else
                {
                    _source = value.Trim();
                }
            }
        }

        public string Container
        {
            get
            {
                return _container;
            }
            set
            {
                _container = value;
            }
        }

        public int Count
        {
            get
            {
                if (_listTopics != null)
                {
                    return _listTopics.Count;
                }

                return 0;
            }
        }

        public BuildTopicTocInfo Parent
        {
            get
            {
                return _parent;
            }
        }

        public BuildTopicTocInfo this[int index]
        {
            get
            {
                if (_listTopics != null && (index >= 0 && index < _listTopics.Count))
                {
                    return _listTopics[index];
                }

                return null;
            }
        }

        public BuildTopicTocInfo this[string name]
        {
            get
            {
                if (_listTopics != null && _listTopics.Count != 0)
                {
                    return _listTopics[name];
                }

                return null;
            }
        }

        public IList<BuildTopicTocInfo> Items
        {
            get
            {
                if (_listTopics != null && _listTopics.Count != 0)
                {
                    return _listTopics; 
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="BuildToc"/> class instance, this property is 
        /// <see cref="BuildToc.TagName"/>.
        /// </para>
        /// </value>
        public override string XmlTagName
        {
            get
            {
                return TagName;
            }
        }

        #endregion

        #region Public Methods

        public override void Load()
        {   
        }

        public override void Save()
        {
        }

        public override void Unload()
        {   
        }

        public override BuildTopicTocInfo Find(string name, bool recursive)
        {
            if (String.Equals(name, this.Name, StringComparison.OrdinalIgnoreCase))
            {
                return this;
            }
            if (_listTopics != null && _listTopics.Count != 0)
            {
                BuildTopicTocInfo foundTopic = this[name];
                if (foundTopic != null)
                {
                    return foundTopic;
                }

                if (recursive)
                {
                    for (int i = 0; i < _listTopics.Count; i++)
                    {
                        foundTopic = _listTopics[i].Find(name, recursive);
                        if (foundTopic != null)
                        {
                            break;
                        }
                    }
                }

                return foundTopic;
            }

            return null;
        }

        public void Add(BuildTopicTocInfo item)
        {
            if (item == null)
            {
                return;
            }

            if (_listTopics == null)
            {
                _listTopics = new BuildKeyedList<BuildTopicTocInfo>();
            }

            _listTopics.Add(item);
        }

        public void AddRange(IList<BuildTopicTocInfo> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }
            if (_listTopics == null)
            {
                _listTopics = new BuildKeyedList<BuildTopicTocInfo>();
            }

            _listTopics.Add(items);
        }

        public void AddRange(BuildGroupTocInfo groupTocInfo)
        {
            if (groupTocInfo == null || groupTocInfo.Count == 0)
            {
                return;
            }
            if (_listTopics == null)
            {
                _listTopics = new BuildKeyedList<BuildTopicTocInfo>();
            }

            _listTopics.Add(groupTocInfo.Items);
        }

        public void Insert(int index, BuildTopicTocInfo item)
        {
            if (item == null)
            {
                return;
            }

            if (_listTopics == null)
            {
                _listTopics = new BuildKeyedList<BuildTopicTocInfo>();
            }

            _listTopics.Insert(index, item);
        }

        public void InsertRange(int index, IList<BuildTopicTocInfo> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }
            if (_listTopics == null)
            {
                _listTopics = new BuildKeyedList<BuildTopicTocInfo>();
            }

            _listTopics.Insert(index, items);
        }

        public void InsertRange(int index, BuildGroupTocInfo groupTocInfo)
        {
            if (groupTocInfo == null || groupTocInfo.Count == 0)
            {
                return;
            }
            if (_listTopics == null)
            {
                _listTopics = new BuildKeyedList<BuildTopicTocInfo>();
            }

            _listTopics.Insert(index, groupTocInfo.Items);
        }

        public void Replace(BuildTopicTocInfo itemOut, BuildTopicTocInfo itemIn)
        {
            if (itemIn == null || itemOut == null || _listTopics == null)
            {
                return;
            }

            int index = this.IndexOf(itemOut);
            if (index < 0)
            {
                BuildTopicTocInfo itemParent = itemOut.Parent;
                if (itemParent != null)
                {
                    itemParent.Replace(itemOut, itemIn);
                }
                return;
            }

            _listTopics.RemoveAt(index);
            _listTopics.Insert(index, itemIn);
        }

        public void Replace(BuildTopicTocInfo itemOut, 
            IList<BuildTopicTocInfo> items)
        {
            if (itemOut == null || items == null || items.Count == 0
                || _listTopics == null)
            {
                return;
            }

            int index = this.IndexOf(itemOut);
            if (index < 0)
            {
                BuildTopicTocInfo itemParent = itemOut.Parent;
                if (itemParent != null)
                {
                    itemParent.Replace(itemOut, items);
                }
                return;
            }

            _listTopics.RemoveAt(index);  
            _listTopics.Insert(index, items);
        }

        public void Replace(BuildTopicTocInfo itemOut, BuildGroupTocInfo groupTocInfo)
        {
            if (itemOut == null || groupTocInfo == null || groupTocInfo.Count == 0
                || _listTopics == null)
            {
                return;
            }

            int index = this.IndexOf(itemOut);
            if (index < 0)
            {
                BuildTopicTocInfo itemParent = itemOut.Parent;
                if (itemParent != null)
                {
                    itemParent.Replace(itemOut, groupTocInfo);
                }
                return;
            }

            _listTopics.RemoveAt(index);
            _listTopics.Insert(index, groupTocInfo.Items);
        }

        public void Remove(int itemIndex)
        {
            if (_listTopics == null || _listTopics.Count == 0)
            {
                return;
            }

            _listTopics.RemoveAt(itemIndex);
        }

        public void Remove(BuildTopicTocInfo item)
        {
            if (item == null)
            {
                return;
            }
            if (_listTopics == null || _listTopics.Count == 0)
            {
                return;
            }

            if (!_listTopics.Remove(item))
            {
                BuildTopicTocInfo parent = item.Parent;
                if (parent != null)
                {
                    parent.Remove(item);
                }
            }
        }

        public int IndexOf(BuildTopicTocInfo item)
        {
            if (item == null)
            {
                return -1;
            }

            if (_listTopics != null && _listTopics.Count != 0)
            {
                return _listTopics.IndexOf(item);
            }

            return -1;
        }

        public void Clear()
        {
            if (_listTopics != null)
            {
                _listTopics = new BuildKeyedList<BuildTopicTocInfo>();
            }
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);

            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }
            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(false, String.Format(
                    "The element name '{0}' does not match the expected '{1}'.",
                    reader.Name, TagName));
                return;
            }

            string topicId   = reader.GetAttribute("id");
            string topicFile = reader.GetAttribute("file");
            if (!String.IsNullOrEmpty(topicId) &&
                !String.IsNullOrEmpty(topicFile))
            {
                this.Name  = topicId;
                _source    = topicFile;
                _container = reader.GetAttribute("project");
            }
            else
            {
                return;
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;
                if (nodeType == XmlNodeType.Element && String.Equals(reader.Name, "topic",
                    StringComparison.OrdinalIgnoreCase))
                {
                    BuildTopicTocInfo topic = new BuildTopicTocInfo(
                        Guid.NewGuid().ToString(), String.Empty, this);

                    topic.ReadXml(reader);

                    this.Add(topic);
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, BuildTopicTocInfo.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            if (this.IsEmpty)
            {
                return;
            }

            // <topic id="" project="" file="">
            writer.WriteStartElement(TagName);  // start: topic
            writer.WriteAttributeString("id", this.Name);
            if (_container != null)  // the container (or project) can be empty for root...
            {
                writer.WriteAttributeString("project", _container);
            }
            writer.WriteAttributeString("file", _source);

            if (_listTopics != null && _listTopics.Count != 0)
            {
                for (int i = 0; i < _listTopics.Count; i++)
                {
                    _listTopics[i].WriteXml(writer);
                }
            }     

            writer.WriteEndElement();           // end: topic
        }

        #endregion

        #region IEquatable<T> Members

        public override bool Equals(BuildTocInfo other)
        {
            if (other == null)
            {
                return false;
            }
            if (!String.Equals(this.Name, other.Name))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            BuildTopicTocInfo other = obj as BuildTopicTocInfo;
            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int hashCode = 53;
            if (this.Name != null)
            {
                hashCode ^= this.Name.GetHashCode();
            }
            if (_source != null)
            {
                hashCode ^= _source.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override BuildTocInfo Clone()
        {
            BuildTopicTocInfo item = new BuildTopicTocInfo(this);
            if (this.Name != null)
            {
                item.Name = String.Copy(this.Name);
            }
            if (_source != null)
            {
                item._source = String.Copy(_source);
            }

            return item;
        }

        #endregion
    }
}
