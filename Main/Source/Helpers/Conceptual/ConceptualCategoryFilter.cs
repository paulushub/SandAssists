using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle.Conceptual
{
    [Serializable]
    public sealed class ConceptualCategoryFilter : ConceptualFilter
    {
        #region Private Fields

        private CategoryItem _category;

        #endregion

        #region Constructors and Destructor

        public ConceptualCategoryFilter()
        {
        }

        public ConceptualCategoryFilter(string name)
            : base(name)
        {
        }

        public ConceptualCategoryFilter(string name, CategoryItem category)
            : base(name)
        {
            _category = category;
        }

        public ConceptualCategoryFilter(ConceptualCategoryFilter source)
            : base(source)
        {
            _category = source._category;
        }

        #endregion

        #region Public Properties

        public override bool IsValid
        {
            get
            {
                return (_category != null && _category.IsValid);
            }
        }

        public CategoryItem Category
        {
            get
            {
                return _category;
            }

            set
            {
                _category = value;
            }
        }

        #endregion

        #region Public Methods

        public override bool Filter(ConceptualItem item)
        {
            BuildExceptions.NotNull(item, "item");

            if (_category.Enabled && _category.IsValid)
            {
                HashSet<string> excludes = item.ExcludesInternal;
                if (excludes == null || excludes.Count == 0)
                {
                    return false;
                }

                return excludes.Contains(_category.Name);
            }

            return false;
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
            ConceptualCategoryFilter filter = new ConceptualCategoryFilter(this);

            if (_category != null)
            {
                filter._category = _category.Clone();
            }

            return filter;
        }

        #endregion
    }
}
