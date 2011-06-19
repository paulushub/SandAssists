using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceVersionInfo : BuildObject<ReferenceVersionInfo>
    {
        #region Private Fields

        private bool   _ripOldApis;
        private string _sourceId;
        private string _label;
        private string _title;
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
            _sourceId    = "Ver" + Guid.NewGuid().ToString().Replace("-", String.Empty);
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
            _label       = source._label;
            _title       = source._title;
            _sourceId    = source._sourceId;
            _ripOldApis  = source._ripOldApis;
            _listSources = source._listSources;
            _listRelated = source._listRelated;
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_label) || String.IsNullOrEmpty(_title))
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

        public string Id
        {
            get
            {
                return _sourceId;
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != null)
                {
                    _title = value.Trim();
                }
                else
                {
                    _title = String.Empty;
                }
            }
        }

        public string VersionLabel
        {
            get
            {
                return _label;
            }
            set
            {
                if (value != null)
                {
                    _label = value.Trim();
                }
                else
                {
                    _label = String.Empty;
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
        /// This reads and sets its state or attributes stored in a XML format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the XML attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the XML format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The XML writer with which the XML format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
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
            ReferenceVersionInfo documenter = new ReferenceVersionInfo(this);

            if (_listSources != null)
            {
                documenter._listSources = _listSources.Clone();
            }

            return documenter;
        }

        #endregion
    }
}
