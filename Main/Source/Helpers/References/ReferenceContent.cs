using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Contents;

namespace Sandcastle.References
{
    [Serializable]
    public class ReferenceContent : BuildContent<ReferenceItem, ReferenceContent>
    {
        #region Private Fields

        private DependencyContent _depencencies;

        #endregion

        #region Constructors and Destructor

        public ReferenceContent()
        {
            _depencencies = new DependencyContent();
        }

        public ReferenceContent(ReferenceContent source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public DependencyContent Dependencies
        {
            get
            {
                return _depencencies;
            }
        }

        #endregion

        #region Public Methods

        public void AddItem(string comments, string assembly)
        {
            if (String.IsNullOrEmpty(comments) && String.IsNullOrEmpty(assembly))
            {
                return;
            }

            this.Add(new ReferenceItem(comments, assembly));
        }

        public void AddDependency(string assembly)
        {
            if (String.IsNullOrEmpty(assembly))
            {
                return;
            }
            if (_depencencies == null)
            {
                _depencencies = new DependencyContent();
            }

            _depencencies.Add(new DependencyItem(
                Path.GetFileName(assembly), assembly));
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

        public override ReferenceContent Clone()
        {
            ReferenceContent content = new ReferenceContent(this);

            this.Clone(content);

            return content;
        }

        #endregion
    }
}
