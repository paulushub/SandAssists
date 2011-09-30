﻿// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Globalization;
using System.Reflection;
using Microsoft.Internal;
using Sandcastle.Composition.ReflectionModel;
using System.Collections.Generic;
using Sandcastle.Composition.Primitives;
using System.Threading;

namespace Sandcastle.Composition.ReflectionModel
{
    internal class ReflectionParameterImportDefinition : ReflectionImportDefinition
    {
        private Lazy<ParameterInfo> _importingLazyParameter;

        public ReflectionParameterImportDefinition(
            Lazy<ParameterInfo> importingLazyParameter,
            string contractName, 
            string requiredTypeIdentity,
            IEnumerable<string> requiredMetadata,
            ImportCardinality cardinality, 
            CreationPolicy requiredCreationPolicy,
            ICompositionElement origin) 
            : base(contractName, requiredTypeIdentity, requiredMetadata, cardinality, false, true, requiredCreationPolicy, origin)
        {
            Assumes.NotNull(importingLazyParameter);

            this._importingLazyParameter = importingLazyParameter;
        }

        public override ImportingItem ToImportingItem()
        {
            return new ImportingParameter(this, new ImportType(this.ImportingLazyParameter.GetNotNullValue("parameter").ParameterType, this.Cardinality));
        }

        public Lazy<ParameterInfo> ImportingLazyParameter
        {
            get { return this._importingLazyParameter; }
        }

        protected override string GetDisplayName()
        {
            ParameterInfo parameter = this.ImportingLazyParameter.GetNotNullValue("parameter");
            return string.Format(
                CultureInfo.CurrentCulture,
                "{0} (Parameter=\"{1}\", ContractName=\"{2}\")",  // NOLOC
                parameter.Member.GetDisplayName(),
                parameter.Name,                
                this.ContractName);
        }
    }
}
