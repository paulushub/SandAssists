using System;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    public abstract class BuilderComponent : BuildComponent
    {
        #region Constructors and Destructor

        protected BuilderComponent(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
        {
        }

        ~BuilderComponent()
        {
            Dispose(false);
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected void WriteMessage(MessageLevel level, Exception ex)
        {
            string message = ex.Message;
            if (message == null)
            {
                message = ex.ToString();
            }
            this.WriteMessage(level,  message);
            //this.WriteMessage(level, String.Format("Exception({0}) - {1}",
            //    ex.GetType().FullName, message));
        }

        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

            base.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {   
        }

        #endregion
    }
}
