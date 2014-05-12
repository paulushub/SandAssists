// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Sandcastle.ReflectionData.Targets
{
    [Serializable]
    public enum TargetType
    {
        None        = 0,
        Namespace   = 1,
        Type        = 2,
        Enumeration = 3,
        Member      = 4,
        Constructor = 5,
        Procedure   = 6,
        Event       = 7,
        Property    = 8,
        Method      = 9,
    }
}
