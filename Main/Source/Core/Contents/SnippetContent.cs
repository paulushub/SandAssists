using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public sealed class SnippetContent : BuildContent<SnippetItem, SnippetContent>
    {
        #region Private Fields

        private string _contentsFile;

        #endregion

        #region Constructors and Destructor

        public SnippetContent()
        {
        }

        public SnippetContent(string contentFile)
        {
            _contentsFile = contentFile;
        }

        public SnippetContent(SnippetContent source)
            : base(source)
        {
            _contentsFile = source._contentsFile;
        }

        #endregion

        #region Public Properties

        public override bool IsEmpty
        {
            get
            {
                if (String.IsNullOrEmpty(_contentsFile) == false)
                {
                    return false;
                }

                return base.IsEmpty;
            }
        }

        public string ContentsFile
        {
            get
            {
                return _contentsFile;
            }
            set
            {
                _contentsFile = value;
            }
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

        public override SnippetContent Clone()
        {
            SnippetContent content = new SnippetContent(this);

            this.Clone(content);
            if (_contentsFile != null)
            {
                content._contentsFile = String.Copy(_contentsFile);
            }

            return content;
        }

        #endregion
    }
}
