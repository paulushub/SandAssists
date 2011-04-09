using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Sandcastle
{
    /// <summary>
    /// This is a generic or base documenter. A documenter builds or generates a
    /// documentation using contents defined by various build groups.
    /// </summary>
    /// <remarks>
    /// The generic documenter is independent of any content or project file format, 
    /// and will build contents or content locations loaded into memory.
    /// </remarks>
    [Serializable]
    public class BuildDocumenter : BuildObject<BuildDocumenter>
    {
        #region Private Fields

        private BuildSettings              _settings;
        private BuildKeyedList<BuildGroup> _listGroups;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildDocumenter"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildDocumenter"/> class
        /// to the default properties or values.
        /// </summary>
        public BuildDocumenter()
        {
            _listGroups = new BuildKeyedList<BuildGroup>();
            _settings   = new BuildSettings();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildDocumenter"/> class with
        /// the specified list of build groups to be initially added to 
        /// this documenter.
        /// </summary>
        /// <param name="groups">
        /// A list, <see cref="IList{T}"/>, specifying the build groups 
        /// <see cref="BuildGroup"/> to be initially added to this documenter.
        /// </param>
        public BuildDocumenter(IList<BuildGroup> groups)
            : this()
        {
            if (groups != null && groups.Count != 0)
            {
                _listGroups.Add(groups);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildDocumenter"/> class
        /// with properties from the specified source, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildDocumenter"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public BuildDocumenter(BuildDocumenter source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public bool IsEmpty
        {
            get
            {
                return (_listGroups == null || _listGroups.Count == 0);
            }
        }

        public BuildSettings Settings
        {
            get
            {
                return _settings;
            }
        }

        public int Count
        {
            get
            {
                if (_listGroups != null)
                {
                    return _listGroups.Count;
                }

                return 0;
            }
        }

        public BuildGroup this[int index]
        {
            get
            {
                if (_listGroups != null)
                {
                    return _listGroups[index];
                }

                return null;
            }
        }

        public BuildGroup this[string groupName]
        {
            get
            {
                if (String.IsNullOrEmpty(groupName))
                {
                    return null;
                }

                if (_listGroups != null)
                {
                    return _listGroups[groupName];
                }

                return null;
            }
        }

        public IList<BuildGroup> Groups
        {
            get
            {
                if (_listGroups != null)
                {
                    return new ReadOnlyCollection<BuildGroup>(_listGroups);
                }

                return null;
            }
        }

        #endregion

        #region Public Methods

        public virtual bool Load(string contentsPath)
        {
            return true;
        }

        public virtual bool Save(string contentsPath)
        {
            return true;
        }

        #region Group Methods

        public void Add(BuildGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            if (_listGroups == null)
            {
                _listGroups = new BuildKeyedList<BuildGroup>();
            }

            _listGroups.Add(group);
        }

        public void Add(IList<BuildGroup> groups)
        {
            BuildExceptions.NotNull(groups, "groups");

            int groupCount = groups.Count;
            if (groupCount == 0)
            {
                return;
            }

            for (int i = 0; i < groupCount; i++)
            {
                this.Add(groups[i]);
            }
        }

        public void Remove(int index)
        {
            if (_listGroups == null || _listGroups.Count == 0)
            {
                return;
            }

            _listGroups.RemoveAt(index);
        }

        public void Remove(BuildGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            if (_listGroups == null || _listGroups.Count == 0)
            {
                return;
            }

            _listGroups.Remove(group);
        }

        public bool Contains(BuildGroup group)
        {
            if (group == null || _listGroups == null || _listGroups.Count == 0)
            {
                return false;
            }

            return _listGroups.Contains(group);
        }

        public void Clear()
        {
            if (_listGroups == null || _listGroups.Count == 0)
            {
                return;
            }

            _listGroups.Clear();
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
        public override BuildDocumenter Clone()
        {
            BuildDocumenter documenter = new BuildDocumenter(this);

            if (_settings != null)
            {
                documenter._settings = _settings.Clone();
            }
            if (_listGroups != null)
            {
                documenter._listGroups = _listGroups.Clone();
            }

            return documenter;
        }

        #endregion
    }
}
