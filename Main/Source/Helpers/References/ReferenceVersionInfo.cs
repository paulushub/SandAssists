using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Utilities;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceVersionInfo : BuildObject<ReferenceVersionInfo>
    {
        #region Public Fields

        public const string TagName = "versionInfo";

        #endregion

        #region Private Fields

        private bool   _ripOldApis;
        private bool   _platformFilters;
        private string _versionId;
        private string _versionLabel;
        private string _platformId;
        private string _platformTitle;
        private BuildKeyedList<ReferenceVersionSource>  _listSources;
        private BuildKeyedList<ReferenceVersionRelated> _listRelated;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceVersionInfo"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceVersionInfo"/> class
        /// to the default properties or values.
        /// </summary>
        public ReferenceVersionInfo()
        {
            _ripOldApis  = true;
            _platformFilters = true;
            _versionId   = String.Format("VerId{0:x}", Guid.NewGuid().ToString().GetHashCode());
            _platformId  = String.Format("Plat{0:x}", Guid.NewGuid().ToString().GetHashCode());
            _listSources = new BuildKeyedList<ReferenceVersionSource>();
            _listRelated = new BuildKeyedList<ReferenceVersionRelated>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceVersionInfo"/> class
        /// with properties from the specified source, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceVersionInfo"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceVersionInfo(ReferenceVersionInfo source)
            : base(source)
        {
            _versionId       = source._versionId;
            _versionLabel    = source._versionLabel;
            _platformTitle   = source._platformTitle;
            _platformId      = source._platformId;
            _platformFilters = source._platformFilters;
            _ripOldApis      = source._ripOldApis;
            _listSources     = source._listSources;
            _listRelated     = source._listRelated;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_versionLabel) || String.IsNullOrEmpty(_platformTitle))
                {
                    return true;
                }

                if (_listSources == null || _listSources.Count == 0)
                {
                    return true;
                }

                // For a valid or non-empty version information, there must be
                // at least one valid version source...
                for (int i = 0; i < _listSources.Count; i++)
                {
                    ReferenceVersionSource source = _listSources[i];

                    if (source != null && source.IsValid)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public string PlatformId
        {
            get
            {
                return _platformId;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (!String.IsNullOrEmpty(value))
                {
                    _platformId = value;
                }
            }
        }

        public string PlatformTitle
        {
            get
            {
                return _platformTitle;
            }
            set
            {
                if (value != null)
                {
                    _platformTitle = value.Trim();
                }
                else
                {
                    _platformTitle = String.Empty;
                }
            }
        }

        public bool PlatformFilters
        {
            get
            {
                return _platformFilters;
            }
            set
            {
                _platformFilters = value;
            }
        }

        public string VersionId
        {
            get
            {
                return _versionId;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (!String.IsNullOrEmpty(value))
                {
                    _versionId = value;
                }
            }
        }

        public string VersionLabel
        {
            get
            {
                return _versionLabel;
            }
            set
            {
                if (value != null)
                {
                    _versionLabel = value.Trim();
                }
                else
                {
                    _versionLabel = String.Empty;
                }
            }
        }

        public bool RipOldApis
        {
            get
            {
                return _ripOldApis;
            }
            set
            {
                _ripOldApis = value;
            }
        }

        public int Count
        {
            get
            {
                if (_listSources != null)
                {
                    return _listSources.Count;
                }

                return 0;
            }
        }

        public ReferenceVersionSource this[int index]
        {
            get
            {
                if (_listSources != null)
                {
                    return _listSources[index];
                }

                return null;
            }
        }

        public ReferenceVersionSource this[string sourceId]
        {
            get
            {
                if (String.IsNullOrEmpty(sourceId))
                {
                    return null;
                }

                if (_listSources != null)
                {
                    return _listSources[sourceId];
                }

                return null;
            }
        }

        public IBuildNamedList<ReferenceVersionSource> Sources
        {
            get
            {
                return _listSources;
            }
        }

        public IBuildNamedList<ReferenceVersionRelated> RelatedVersions
        {
            get
            {
                return _listRelated;
            }
        }

        /// <summary>
        /// Gets the name of the <c>XML</c> tag name, under which this object is stored.
        /// </summary>
        /// <value>
        /// A string containing the <c>XML</c> tag name of this object. 
        /// <para>
        /// For the <see cref="ReferenceVersionInfo"/> class instance, this property is 
        /// <see cref="ReferenceVersionInfo.TagName"/>.
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

        #region Source Methods

        public void AddSource(ReferenceVersionSource source)
        {
            BuildExceptions.NotNull(source, "source");

            if (_listSources == null)
            {
                _listSources = new BuildKeyedList<ReferenceVersionSource>();
            }

            _listSources.Add(source);
        }

        public void AddSource(IList<ReferenceVersionSource> sources)
        {
            BuildExceptions.NotNull(sources, "sources");

            int sourceCount = sources.Count;
            if (sourceCount == 0)
            {
                return;
            }

            for (int i = 0; i < sourceCount; i++)
            {
                this.AddSource(sources[i]);
            }
        }

        public void RemoveSource(int index)
        {
            if (_listSources == null || _listSources.Count == 0)
            {
                return;
            }

            _listSources.RemoveAt(index);
        }

        public void RemoveSource(ReferenceVersionSource source)
        {
            BuildExceptions.NotNull(source, "source");

            if (_listSources == null || _listSources.Count == 0)
            {
                return;
            }

            _listSources.Remove(source);
        }

        public bool ContainsSource(ReferenceVersionSource source)
        {
            if (source == null || _listSources == null || _listSources.Count == 0)
            {
                return false;
            }

            return _listSources.Contains(source);
        }

        public void ClearSource()
        {
            if (_listSources == null || _listSources.Count == 0)
            {
                return;
            }

            _listSources.Clear();
        }

        #endregion

        #region Related Versions Methods

        public void AddRelated(ReferenceVersionRelated relatedVersion)
        {
            BuildExceptions.NotNull(relatedVersion, "relatedVersion");

            if (_listRelated == null)
            {
                _listRelated = new BuildKeyedList<ReferenceVersionRelated>();
            }

            _listRelated.Add(relatedVersion);
        }

        public void AddRelated(IList<ReferenceVersionRelated> relatedVersions)
        {
            BuildExceptions.NotNull(relatedVersions, "relatedVersions");

            int relatedCount = relatedVersions.Count;
            if (relatedCount == 0)
            {
                return;
            }

            for (int i = 0; i < relatedCount; i++)
            {
                this.AddRelated(relatedVersions[i]);
            }
        }

        public void RemoveRelated(int index)
        {
            if (_listRelated == null || _listRelated.Count == 0)
            {
                return;
            }

            _listRelated.RemoveAt(index);
        }

        public void RemoveRelated(ReferenceVersionRelated relatedVersion)
        {
            BuildExceptions.NotNull(relatedVersion, "relatedVersion");

            if (_listRelated == null || _listRelated.Count == 0)
            {
                return;
            }

            _listRelated.Remove(relatedVersion);
        }

        public bool ContainsRelated(ReferenceVersionRelated relatedVersion)
        {
            if (relatedVersion == null || _listRelated == null || _listRelated.Count == 0)
            {
                return false;
            }

            return _listRelated.Contains(relatedVersion);
        }

        public void ClearRelated()
        {
            if (_listRelated == null || _listRelated.Count == 0)
            {
                return;
            }

            _listRelated.Clear();
        }

        #endregion

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

            if (_listSources == null)
            {
                _listSources = new BuildKeyedList<ReferenceVersionSource>();
            }
            if (_listRelated == null)
            {
                _listRelated = new BuildKeyedList<ReferenceVersionRelated>();
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "propertyGroup",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        while (reader.Read())
                        {
                            if (reader.NodeType == XmlNodeType.Element)
                            {
                                if (String.Equals(reader.Name, "property",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    string tempText = null;
                                    switch (reader.GetAttribute("name").ToLower())
                                    {
                                        case "platformid":
                                            _platformId = reader.ReadString();
                                            break;
                                        case "platformtitle":
                                            _platformTitle = reader.ReadString();
                                            break;
                                        case "platformfilters":
                                            tempText = reader.ReadString();
                                            if (!String.IsNullOrEmpty(tempText))
                                            {
                                                _platformFilters = Convert.ToBoolean(tempText);
                                            }
                                            break;
                                        case "versionid":
                                            _versionId = reader.ReadString();
                                            break;
                                        case "versionlabel":
                                            _versionLabel = reader.ReadString();
                                            break;
                                        case "ripoldapis":
                                            tempText = reader.ReadString();
                                            if (!String.IsNullOrEmpty(tempText))
                                            {
                                                _ripOldApis = Convert.ToBoolean(tempText);
                                            }
                                            break;
                                        default:
                                            // Should normally not reach here...
                                            throw new NotImplementedException(reader.GetAttribute("name"));
                                    }
                                }
                            }
                            else if (reader.NodeType == XmlNodeType.EndElement)
                            {
                                if (String.Equals(reader.Name, "propertyGroup",
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    break;
                                }
                            }
                        }
                    }
                    else if (String.Equals(reader.Name, ReferenceSource.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        ReferenceVersionSource versionSource = 
                            new ReferenceVersionSource();

                        versionSource.ReadXml(reader);

                        _listSources.Add(versionSource);
                    }
                    else if (String.Equals(reader.Name, ReferenceVersionRelated.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        ReferenceVersionRelated relatedVersion = 
                            new ReferenceVersionRelated();

                        relatedVersion.ReadXml(reader);

                        _listRelated.Add(relatedVersion);
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

            if (this.IsEmpty)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - TagName

            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name",            "Reference");
            writer.WritePropertyElement("PlatformId",      _platformId);
            writer.WritePropertyElement("PlatformTitle",   _platformTitle);
            writer.WritePropertyElement("PlatformFilters", _platformFilters);
            writer.WritePropertyElement("VersionId",       _versionId);
            writer.WritePropertyElement("VersionLabel",    _versionLabel);
            writer.WritePropertyElement("RipOldApis",      _ripOldApis);
            writer.WriteEndElement();                   // end - propertyGroup

            writer.WriteStartElement("contentSources");  // start - contentSources
            if (_listSources != null && _listSources.Count != 0)
            {
                for (int i = 0; i < _listSources.Count; i++)
                {
                    _listSources[i].WriteXml(writer);
                }
            }
            writer.WriteEndElement();                    // end - contentSources

            writer.WriteStartElement("relatedVersions");  // start - relatedVersions
            if (_listRelated != null && _listRelated.Count != 0)
            {
                for (int i = 0; i < _listRelated.Count; i++)
                {
                    _listRelated[i].WriteXml(writer);
                }
            }
            writer.WriteEndElement();                     // end - relatedVersions

            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// This creates a new directory copier object that is a deep copy of the 
        /// current instance.
        /// </summary>
        /// <returns>
        /// A new directory copier object that is a deep copy of this instance.
        /// </returns>
        /// <remarks>
        /// This is deep cloning of the members of this directory copier object. 
        /// If you need just a copy, use the copy constructor to create a new 
        /// instance.
        /// </remarks>
        public override ReferenceVersionInfo Clone()
        {
            ReferenceVersionInfo versionInfo = new ReferenceVersionInfo(this);

            if (_listSources != null)
            {
                versionInfo._listSources = _listSources.Clone();
            }
            if (_listRelated != null)
            {
                versionInfo._listRelated = _listRelated.Clone();
            }

            if (_versionId != null)
            {
                versionInfo._versionId = String.Copy(_versionId);
            }
            if (_versionLabel != null)
            {
                versionInfo._versionLabel = String.Copy(_versionLabel);
            }
            if (_platformId != null)
            {
                versionInfo._platformId = String.Copy(_platformId);
            }
            if (_platformTitle != null)
            {
                versionInfo._platformTitle = String.Copy(_platformTitle);
            }

            return versionInfo;
        }

        #endregion
    }
}
