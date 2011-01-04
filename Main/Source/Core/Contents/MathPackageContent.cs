using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class MathPackageContent : BuildContent<MathPackageItem, MathPackageContent>
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public MathPackageContent()
        {
        }

        public MathPackageContent(MathPackageContent source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        public void Add(string use)
        {   
            this.Add(new MathPackageItem(use));
        }

        public void Add(string use, string option)
        {
            this.Add(new MathPackageItem(use, option));
        }

        public void Add(string use, params string[] options)
        {
            this.Add(new MathPackageItem(use, options));
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

        public override MathPackageContent Clone()
        {
            MathPackageContent content = new MathPackageContent(this);

            this.Clone(content);

            return content;
        }

        #endregion
    }
}
