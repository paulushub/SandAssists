// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;

namespace Sandcastle.ReflectionData.References
{
    [Serializable]
    public enum ReferenceType
    {
        None                            = 0,
        Invalid                         = 1,
        Namespace                       = 2,
        ExtensionMethod                 = 3,

        SimpleMember                    = 4,
        SpecializedMember               = 5,
        SpecializedMemberWithParameters = 6,

        SimpleType                      = 7,
        SpecializedType                 = 8,
        ArrayType                       = 9, 
        ReferenceType                   = 10,
        PointerType                     = 11,

        IndexedTemplate                 = 12, 
        NamedTemplate                   = 13,
        TypeTemplate                    = 14,
        MethodTemplate                  = 15
    }
}
