// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Sandcastle.ReflectionData.References
{
    public enum ReferenceLinkType
    {
        None         = 0,
        Self         = 1,
        Local        = 2,
        Index        = 3,
        LocalOrIndex = 4,
        Msdn         = 5,
        Id           = 6
    }
}
