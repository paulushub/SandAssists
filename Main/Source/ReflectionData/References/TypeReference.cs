// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Sandcastle.ReflectionData.References
{
    [Serializable]
    public abstract class TypeReference : Reference
    {
        #region Constructors and Destructor

        protected TypeReference()
        {
        }

        #endregion
    }
}
