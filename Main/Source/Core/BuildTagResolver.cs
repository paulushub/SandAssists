using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    public abstract class BuildTagResolver : BuildObject
    {
        #region Constructors and Destructor

        protected BuildTagResolver()
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        public abstract string Resolve(string inputText);
        public abstract string Resolve(string inputText, 
            IDictionary<string, string> customTags);

        #endregion

        #region SystemTagResolver Class

        private abstract class SystemTagResolver : BuildTagResolver
        {
            public SystemTagResolver()
            {   
            }
        }

        #endregion
    }
}
