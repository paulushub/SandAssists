// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;

namespace Sandcastle.Composition
{
    internal enum ExportCardinalityCheckResult : int
    {
        Match,
        NoExports,
        TooManyExports
    }
}
