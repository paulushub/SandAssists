﻿// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using Sandcastle.Composition.Hosting;
using Sandcastle.Composition.Primitives;

namespace Sandcastle.Composition
{
    // Internal interface for providing access to the composition error
    // identifier for an exception or error that participates in composition.
    internal interface ICompositionError
    {
        CompositionErrorId Id
        {
            get;
        }

        ICompositionElement Element
        {
            get;
        }

        Exception InnerException
        {
            get;
        }
    } 
}
