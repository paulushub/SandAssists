// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Net;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;
using System.Web.Services.Protocols;

namespace Sandcastle.ReflectionData
{
    public sealed class MemoryMsdnResolver : TargetMsdnResolver
    {
        #region Private Fields

        private Dictionary<string, string> cachedMsdnUrls;

        #endregion

        #region Constructors and Destructor

        public MemoryMsdnResolver()
        {
            cachedMsdnUrls = new Dictionary<string, string>();
        }

        #endregion

        #region Public Properties

        public override bool IsDisabled
        {
            get
            {
                return base.IsDisabled;
            }
        }

        public override string this[string id]
        {
            get
            {
                if (cachedMsdnUrls.ContainsKey(id))
                {
                    return cachedMsdnUrls[id];
                }

                string url = base.GetUrl(id);
                if (!String.IsNullOrEmpty(url))
                {
                    cachedMsdnUrls[id] = url;

                    return url;
                }

                return String.Empty;
            }
        }

        #endregion

        #region Public Methods

        public override string GetUrl(string id)
        {
            if (cachedMsdnUrls.ContainsKey(id))
            {
                return cachedMsdnUrls[id];
            }

            string url = base.GetUrl(id);
            if (!String.IsNullOrEmpty(url))
            {
                cachedMsdnUrls[id] = url;
            }

            return url;
        }

        #endregion
    }
}
