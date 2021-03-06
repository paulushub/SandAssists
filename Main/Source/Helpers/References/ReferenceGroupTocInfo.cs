﻿using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceGroupTocInfo : BuildGroupTocInfo
    {
        #region Public Fields

        public const string TagName = "topics";

        #endregion

        #region Private Fields

        private bool _isLoaded;

        private BuildKeyedList<BuildTopicTocInfo> _listNamespaces;          
        private BuildKeyedList<BuildTopicTocInfo> _listTopics;

        #endregion

        #region Constructors and Destructor

        public ReferenceGroupTocInfo()
            : this(Guid.NewGuid().ToString(), String.Empty)
        {
        }

        public ReferenceGroupTocInfo(string name, string tocFile)
            : base(name, tocFile)
        {
        }

        public ReferenceGroupTocInfo(ReferenceGroupTocInfo source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public override BuildTocInfoType ItemType
        {
            get
            {
                return BuildTocInfoType.Reference;
            }
        }

        public override bool IsRooted
        {
            get
            {
                return (this.RootTopicInfo != null);
            }
        }

        public override bool IsEmpty
        {
            get
            {
                return base.IsEmpty;
            }
        }

        public override bool IsLoaded
        {
            get
            {
                return _isLoaded;
            }
        }

        public override int Count
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

        public override BuildTopicTocInfo this[int index]
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

        public override BuildTopicTocInfo this[string name]
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

        public BuildTopicTocInfo RootTopicInfo
        {
            get
            {
                if (_listTopics != null && _listTopics.Count == 1)
                {
                    BuildTopicTocInfo topicInfo = _listTopics[0];
                    if (String.IsNullOrEmpty(topicInfo.Container))
                    {
                        return topicInfo;
                    }
                }

                return null;
            }
        }

        public override IList<BuildTopicTocInfo> Items
        {
            get
            {
                return _listTopics;
            }
        }

        public BuildKeyedList<BuildTopicTocInfo> Namespaces
        {
            get
            {
                return _listNamespaces;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="ReferenceGroupTocInfo"/> class instance, this 
        /// property is <see cref="ReferenceGroupTocInfo.TagName"/>.
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
            if (_isLoaded)
            {
                return;
            }
            string tocFile = this.TocFile;
            if (String.IsNullOrEmpty(tocFile) || !File.Exists(tocFile))
            {
                return;
            }

            XmlReader reader = null;
            try
            {
                _listTopics     = new BuildKeyedList<BuildTopicTocInfo>();
                _listNamespaces = new BuildKeyedList<BuildTopicTocInfo>();

                XmlReaderSettings settings = new XmlReaderSettings();

                settings.IgnoreComments = true;
                settings.IgnoreWhitespace = true;
                settings.IgnoreProcessingInstructions = true;

                reader = XmlReader.Create(tocFile, settings);
                reader.MoveToContent();
                this.ReadXml(reader);

                _isLoaded = true;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
            }
        }

        public override void Save()
        {
            if (!_isLoaded)
            {
                return;
            }
            string tocFile = this.TocFile;
            if (String.IsNullOrEmpty(tocFile) || !File.Exists(tocFile))
            {
                return;
            }

            XmlWriter writer = null;
            try
            {
                XmlWriterSettings writerSettings  = new XmlWriterSettings();
                writerSettings.Indent             = true;
                writerSettings.OmitXmlDeclaration = false;

                writer = XmlWriter.Create(tocFile, writerSettings);

                this.WriteXml(writer);

                writer.Close();
                writer = null;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (writer != null)
                {
                    writer.Close();
                    writer = null;
                }
            }
        }

        public override void Unload()
        {
            _listTopics     = null;
            _listNamespaces = null;

            _isLoaded       = false;
        }

        public override BuildTopicTocInfo Find(string name, bool recursive)
        {
            if (_listTopics == null || _listTopics.Count == 0)
            {
                return null;
            }
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

        public override void Add(BuildTopicTocInfo item)
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

        public override void AddRange(IList<BuildTopicTocInfo> items)
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

        public override void AddRange(BuildGroupTocInfo groupTocInfo)
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

        public override void Insert(int index, BuildTopicTocInfo item)
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

        public override void InsertRange(int index, IList<BuildTopicTocInfo> items)
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

        public override void InsertRange(int index, BuildGroupTocInfo groupTocInfo)
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

        public override void Replace(BuildTopicTocInfo itemOut, 
            BuildTopicTocInfo itemIn)
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

        public override void Replace(BuildTopicTocInfo itemOut,
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

        public override void Replace(BuildTopicTocInfo itemOut, 
            BuildGroupTocInfo groupTocInfo)
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

        public override void Remove(int itemIndex)
        {
            if (_listTopics == null || _listTopics.Count == 0)
            {
                return;
            }

            _listTopics.RemoveAt(itemIndex);
        }

        public override void Remove(BuildTopicTocInfo item)
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

        public override int IndexOf(BuildTopicTocInfo item)
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

        public override void Clear()
        {
            if (_listTopics != null)
            {
                _listTopics = new BuildKeyedList<BuildTopicTocInfo>();
            }
        }

        #endregion

        #region Private Methods

        private void ReadTopic(XmlReader reader, BuildTopicTocInfo parent)
        {
            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;
                if (nodeType == XmlNodeType.Element && String.Equals(reader.Name, "topic",
                    StringComparison.OrdinalIgnoreCase))
                {
                    string topicId = reader.GetAttribute("id");
                    string topicFile = reader.GetAttribute("file");
                    string topicContainer = reader.GetAttribute("project");
                    if (!String.IsNullOrEmpty(topicId) &&
                        !String.IsNullOrEmpty(topicFile))
                    {
                        BuildTopicTocInfo topic = new BuildTopicTocInfo(
                            topicId, topicFile, parent);
                        topic.Container = topicContainer;

                        if (String.Equals(topicContainer, "_Namespaces",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            _listNamespaces.Add(topic);
                        }

                        parent.Add(topic);

                        if (!reader.IsEmptyElement)
                        {
                            // Read the sub-topics...
                            this.ReadTopic(reader, topic);
                        }
                    }
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

            if (reader.IsEmptyElement)
            {
                return;
            }

            this.Clear();

            XmlNodeType nodeType = XmlNodeType.None;
            while (reader.Read())
            {
                nodeType = reader.NodeType;
                if (nodeType == XmlNodeType.Element && String.Equals(
                    reader.Name, BuildTopicTocInfo.TagName,
                    StringComparison.OrdinalIgnoreCase))
                {
                    // Read the top-level topics...
                    string topicId = reader.GetAttribute("id");
                    string topicFile = reader.GetAttribute("file");
                    string topicContainer = reader.GetAttribute("project");
                    if (!String.IsNullOrEmpty(topicId) &&
                        !String.IsNullOrEmpty(topicFile))
                    {
                        BuildTopicTocInfo topic = new BuildTopicTocInfo(
                            topicId, topicFile, null);
                        topic.Container = topicContainer;

                        if (String.Equals(topicContainer, "_Namespaces",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            _listNamespaces.Add(topic);
                        }

                        this.Add(topic);

                        if (!reader.IsEmptyElement)
                        {
                            // Read the sub-topics...
                            this.ReadTopic(reader, topic);
                        }
                    }
                }
                else if (nodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName,
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

            writer.WriteStartElement(TagName);

            if (_listTopics != null && _listTopics.Count != 0)
            {
                for (int i = 0; i < _listTopics.Count; i++)
                {
                    _listTopics[i].WriteXml(writer);
                }
            }

            writer.WriteEndElement();
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
            ReferenceGroupTocInfo other = obj as ReferenceGroupTocInfo;
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
            if (this.TocFile != null)
            {
                hashCode ^= this.TocFile.GetHashCode();
            }

            return hashCode;
        }

        #endregion

        #region ICloneable Members

        public override BuildTocInfo Clone()
        {
            ReferenceGroupTocInfo item = new ReferenceGroupTocInfo(this);
            if (this.Name != null)
            {
                item.Name = String.Copy(this.Name);
            }
            if (this.TocFile != null)
            {
                item.TocFile = String.Copy(this.TocFile);
            }

            return item;
        }

        #endregion
    }
}
