using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Globalization;
using System.Collections.Generic;

namespace Sandcastle.Formats
{
    [Serializable]
    public class FormatAjaxHtm : FormatHtm
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public FormatAjaxHtm()
        {
        }

        public FormatAjaxHtm(FormatAjaxHtm source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        public override BuildStep CreateStep(BuildContext context,
            BuildStage stage, string workingDir)
        {
            return base.CreateStep(context, stage, workingDir);
        }

        public override void WriteAssembler(BuildContext context,
            BuildGroup group, XmlWriter xmlWriter)
        {
            base.WriteAssembler(context, group, xmlWriter);
        }

        public override void Reset()
        {
            base.Reset();
        }

        #endregion

        #region Private Methods

        #endregion

        #region ICloneable Members

        public override BuildFormat Clone()
        {
            FormatAjaxHtm format = new FormatAjaxHtm(this);

            return format;
        }

        #endregion
    }
}
