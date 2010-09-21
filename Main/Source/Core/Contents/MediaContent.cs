using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Contents
{
    [Serializable]
    public class MediaContent : BuildContent<MediaItem, MediaContent>
    {
        #region Private Fields

        private string _contentsName;
        private string _contentsPath;
        private string _contentsFile;
        private string _outputBase;
        private string _outputPath;
        private string _outputLink;

        #endregion

        #region Constructors and Destructor

        public MediaContent() 
            : this((string)null)
        {
        }

        public MediaContent(string contentsFile)
        {
            _contentsName = String.Empty;
            if (String.IsNullOrEmpty(contentsFile) == false)
            {
                _contentsPath = Path.GetDirectoryName(contentsFile);
            }
            _contentsFile = contentsFile;

            _outputPath   = "string('media')";
            _outputLink   = "media";
            _outputBase   = @".\Output";
        }

        public MediaContent(MediaContent source)
            : base(source)
        {
            _contentsName = source._contentsName;
            _contentsPath = source._contentsPath;
            _contentsFile = source._contentsFile;
            _outputPath   = source._outputPath;
            _outputLink   = source._outputLink;
            _outputBase   = source._outputBase;
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

        public string ContentsName
        {
            get 
            { 
                return _contentsName; 
            }
            set 
            { 
                _contentsName = value; 
            }
        }
        
        public string ContentsPath
        {
            get 
            { 
                return _contentsPath; 
            }
            set 
            { 
                _contentsPath = value; 
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

        public string OutputBase
        {
            get 
            {
                return _outputBase; 
            }
            set 
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    _outputBase = value; 
                }
            }
        }

        public string OutputPath
        {
            get
            {
                return _outputPath;
            }
            set
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    _outputPath = value;
                }
            }
        }

        public string OutputLink
        {
            get 
            { 
                return _outputLink; 
            }
            set 
            {
                if (String.IsNullOrEmpty(value) == false)
                {
                    _outputLink = value; 
                }
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

        public override MediaContent Clone()
        {
            MediaContent content = new MediaContent(this);
            
            this.Clone(content);

            if (_contentsName != null)
            {
                content._contentsName = String.Copy(_contentsName);
            }
            if (_contentsPath != null)
            {
                content._contentsPath = String.Copy(_contentsPath);
            }
            if (_contentsFile != null)
            {
                content._contentsFile = String.Copy(_contentsFile);
            }
            if (_outputBase != null)
            {
                content._outputBase = String.Copy(_outputBase);
            }
            if (_outputPath != null)
            {
                content._outputPath = String.Copy(_outputPath);
            }
            if (_outputLink != null)
            {
                content._outputLink = String.Copy(_outputLink);
            }

            return content;
        }

        #endregion
    }
}
