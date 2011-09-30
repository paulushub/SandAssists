﻿// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Sandcastle.Composition.Hosting;
using Sandcastle.Composition.Primitives;
using Sandcastle.Composition.ReflectionModel;
using System.Globalization;
using System.Reflection;
using Microsoft.Internal;

namespace Sandcastle.Composition.ReflectionModel
{
    internal abstract class ReflectionImportDefinition : ContractBasedImportDefinition, ICompositionElement
    {
        private readonly ICompositionElement _origin;

        public ReflectionImportDefinition(
            string contractName, 
            string requiredTypeIdentity,
            IEnumerable<string> requiredMetadata,
            ImportCardinality cardinality, 
            bool isRecomposable, 
            bool isPrerequisite, 
            CreationPolicy requiredCreationPolicy,
            ICompositionElement origin) 
            : base(contractName, requiredTypeIdentity, requiredMetadata, cardinality, isRecomposable, isPrerequisite, requiredCreationPolicy)
        {
            this._origin = origin;
        }

        string ICompositionElement.DisplayName
        {
            get { return this.GetDisplayName(); }
        }

        ICompositionElement ICompositionElement.Origin
        {
            get { return this._origin; }
        }

        public override string ToString()
        {
            return this.GetDisplayName();
        }

        public abstract ImportingItem ToImportingItem();

        protected abstract string GetDisplayName();
    }
}
