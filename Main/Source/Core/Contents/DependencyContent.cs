using System;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class DependencyContent : BuildContent<DependencyItem, DependencyContent>
    {
        #region Public Fields

        public const string TagName = "dependencyContent";

        #endregion

        #region Private Fields

        private BuildList<BuildDirectoryPath> _paths;

        [NonSerialized]
        private IDictionary<string, int> _dicItems;

        #endregion

        #region Constructors and Destructor

        public DependencyContent()
            : base(new BuildKeyedList<DependencyItem>())
        {
            BuildKeyedList<DependencyItem> keyedList =
                this.List as BuildKeyedList<DependencyItem>;

            if (keyedList != null)
            {
                _dicItems = keyedList.Dictionary;
            }

            _paths = new BuildList<BuildDirectoryPath>();
        }

        public DependencyContent(DependencyContent source)
            : base(source)
        {
            _paths    = source._paths;
            _dicItems = source._dicItems;
        }

        #endregion

        #region Public Properties

        public override bool IsEmpty
        {
            get
            {
                if (_paths != null && _paths.Count != 0)
                {
                    for (int i = 0; i < _paths.Count; i++)
                    {
                        BuildDirectoryPath path = _paths[i];
                        if (path != null && path.Exists)
                        {
                            return false;
                        }
                    }
                }

                return base.IsEmpty;
            }
        }

        public DependencyItem this[string itemName]
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

        public IList<BuildDirectoryPath> Paths
        {
            get
            {
                return _paths;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="DependencyContent"/> class instance, this property is 
        /// <see cref="DependencyContent.TagName"/>.
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

        #region Public Method

        public void AddDirectory(string assemblyDir)
        {
            if (!String.IsNullOrEmpty(assemblyDir))
            {
                this.AddDirectory(new BuildDirectoryPath(assemblyDir));
            }
        }

        public void AddDirectory(BuildDirectoryPath path)
        {
            BuildExceptions.NotNull(path, "path");

            if (_paths == null)
            {
                _paths = new BuildList<BuildDirectoryPath>();
            }

            _paths.Add(path);
        }

        public void AddItem(string assemblyName)
        {
            if (!String.IsNullOrEmpty(assemblyName))
            {
                this.Add(new DependencyItem(assemblyName));
            }
        }

        public override void Add(DependencyItem item)
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

        public override bool Remove(DependencyItem item)
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
            if (_paths == null)
            {
                _paths = new BuildList<BuildDirectoryPath>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "paths",
                        StringComparison.OrdinalIgnoreCase) &&
                        !reader.IsEmptyElement)
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                if (String.Equals(reader.Name, BuildDirectoryPath.TagName,
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    BuildDirectoryPath path = new BuildDirectoryPath();
                                    path.ReadXml(reader);

                                    this.AddDirectory(path);
                                }
                            }
                            else if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "paths",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (String.Equals(reader.Name, "items",
                        StringComparison.OrdinalIgnoreCase) &&
                        !reader.IsEmptyElement)
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                if (String.Equals(reader.Name, DependencyItem.TagName,
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    DependencyItem item = new DependencyItem();
                                    item.Content = this;
                                    item.ReadXml(reader);

                                    this.Add(item);
                                }
                            }
                            else if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "items",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    } 
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
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

            if (!this.IsEmpty)
            {   
                writer.WriteStartElement("paths");    // start: paths
                for (int i = 0; i < _paths.Count; i++)
                {
                    _paths[i].WriteXml(writer);
                }
                writer.WriteEndElement();             // end: paths

                writer.WriteStartElement("items");    // start: items
                for (int i = 0; i < this.Count; i++)
                {
                    this[i].WriteXml(writer);
                }
                writer.WriteEndElement();             // end: items
            }   

            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        public override DependencyContent Clone()
        {
            DependencyContent content = new DependencyContent(this);

            if (_paths != null)
            {
                content._paths = _paths.Clone();
            }

            this.Clone(content, new BuildKeyedList<DependencyItem>());

            return content;
        }

        #endregion
    }
}
