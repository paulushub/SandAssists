// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using Sandcastle.Composition.Primitives;

namespace Sandcastle.Composition
{
    internal interface IAttributedImport
    {
        string ContractName { get; }
        Type ContractType { get; }
        bool AllowRecomposition { get; }
        CreationPolicy RequiredCreationPolicy { get; }
        ImportCardinality Cardinality { get; }
    }
}