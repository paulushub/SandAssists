using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.Formats
{
    [Serializable]
    public class FormatAspx : BuildFormat
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public FormatAspx()
        {
        }

        public FormatAspx(FormatAspx source)
            : base(source)
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value specifying the name of the this output format.
        /// </summary>
        /// <value>
        /// A <see cref="System.String"/> containing the name of the output format.
        /// This will always return "WebHelp".
        /// </value>
        public override string FormatName
        {
            get
            {
                return "WebHelp";
            }
        }

        public override string FormatExtension
        {
            get
            {
                return ".aspx";
            }
        }

        public override string OutputExtension
        {
            get
            {
                return ".aspx";
            }
        }

        public override bool IsCompilable
        {
            get
            {
                return false;
            }
        }

        public override BuildFormatType FormatType
        {
            get
            {
                return BuildFormatType.WebHelp;
            }
        }

        #endregion

        #region Public Methods

        public override BuildStep CreateStep(BuildContext context,
            BuildStage stage, string workingDir)
        {
            return null;
        }

        public override void WriteAssembler(BuildContext context,
            BuildGroup group, XmlWriter xmlWriter)
        {
            base.WriteAssembler(context, group, xmlWriter);
        }

        public override void Reset()
        {
            base.Reset();

            this.FormatFolder = "html3";
            this.OutputFolder = "WebHelp";
            this.ExternalLinkType = BuildLinkType.Msdn;
        }

        #endregion

        #region ICloneable Members

        public override BuildFormat Clone()
        {
            FormatAspx format = new FormatAspx(this);

            return format;
        }

        #endregion
    }
}
