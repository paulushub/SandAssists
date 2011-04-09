using System;
using System.IO;
using System.Xml;
using System.Text;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualRevisionFilter : ConceptualFilter
    {
        #region Private Fields

        private int _revNumber;

        #endregion

        #region Constructors and Destructor

        public ConceptualRevisionFilter()
        {
            _revNumber = -1;
        }

        public ConceptualRevisionFilter(string name)
            : base(name)
        {
            _revNumber = -1;
        }

        public ConceptualRevisionFilter(string name, int revisionNumber)
            : base(name)
        {
            _revNumber = revisionNumber;
        }

        public ConceptualRevisionFilter(ConceptualRevisionFilter source)
            : base(source)
        {
            _revNumber = source._revNumber;
        }

        #endregion

        #region Public Properties

        public override bool IsValid
        {
            get
            {
                return (_revNumber >= 0);
            }
        }

        public int RevisionNumber
        {
            get
            {
                return _revNumber;
            }

            set
            {
                _revNumber = value;
            }
        }

        #endregion

        #region Public Methods

        public override bool Filter(ConceptualItem item)
        {
            BuildExceptions.NotNull(item, "item");
            if (_revNumber < 0)
            {
                return false;
            }

            bool isFiltered = (item.TopicRevisions == _revNumber);

            return this.Inverse ? !isFiltered : isFiltered;
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

        public override ConceptualFilter Clone()
        {
            ConceptualRevisionFilter filter = new ConceptualRevisionFilter(this);

            return filter;
        }

        #endregion
    }
}
