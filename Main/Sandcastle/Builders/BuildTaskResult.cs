using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Builders
{
    [Serializable]
    public sealed class BuildTaskResult : BuildObject<BuildTaskResult>
    {
        #region Public Fields

        public static readonly BuildTaskResult Success = new BuildTaskResult(true);
        public static readonly BuildTaskResult Failure = new BuildTaskResult(false);

        #endregion

        #region Private Fields

        private bool      _isSuccess;
        private Exception _exception;

        #endregion

        #region Constructors and Destructor

        public BuildTaskResult()
        {
        }

        public BuildTaskResult(bool success)
        {
            _isSuccess = success;
        }

        public BuildTaskResult(Exception exception)
        {
            _exception = exception;
        }

        public BuildTaskResult(BuildTaskResult source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        public bool IsSuccess
        {
            get
            {
                return _isSuccess;
            }
        }

        public bool IsException
        {
            get
            {
                return (_exception != null);
            }
        }

        public Exception Exception
        {
            get
            {
                return _exception;
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

        public override BuildTaskResult Clone()
        {
            BuildTaskResult style = new BuildTaskResult(this);

            return style;
        }

        #endregion
    }
}
