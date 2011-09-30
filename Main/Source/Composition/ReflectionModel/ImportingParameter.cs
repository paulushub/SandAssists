// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using Sandcastle.Composition.Primitives;

namespace Sandcastle.Composition.ReflectionModel
{
    internal class ImportingParameter : ImportingItem
    {
        public ImportingParameter(ContractBasedImportDefinition definition, ImportType importType)
            : base(definition, importType)
        {
        }
    }
}
