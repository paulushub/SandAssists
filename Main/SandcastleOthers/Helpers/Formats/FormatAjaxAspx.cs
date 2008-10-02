using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Formats
{
    [Serializable]
    public class FormatAjaxAspx : FormatAspx
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public FormatAjaxAspx()
        {
        }

        public FormatAjaxAspx(FormatAjaxAspx source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        public override BuildStep CreateStep(BuildEngine engine,
            BuildStepType stepType, string workingDir)
        {
            return base.CreateStep(engine, stepType, workingDir);
        }

        public override void WriteAssembler(BuildEngine engine,
            BuildGroup group, XmlWriter xmlWriter)
        {
            base.WriteAssembler(engine, group, xmlWriter);
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
            FormatAjaxAspx format = new FormatAjaxAspx(this);

            return format;
        }

        #endregion
    }
}
