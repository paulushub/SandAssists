using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using Sandcastle.Configurations;

namespace Sandcastle.Builders
{
    public class BuildConfigReader : MarshalByRefObject, IDisposable
    {
        #region Constructors and Destructor

        public BuildConfigReader()
        {
        }

        ~BuildConfigReader()
        {
            Dispose(false);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }

        #endregion
    }
}
