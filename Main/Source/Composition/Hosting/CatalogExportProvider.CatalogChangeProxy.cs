﻿// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Sandcastle.Composition.Primitives;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace Sandcastle.Composition.Hosting
{
    public partial class CatalogExportProvider : ExportProvider, IDisposable
    {
        private class CatalogChangeProxy : ComposablePartCatalog
        {
            private ComposablePartCatalog _originalCatalog;
            private List<ComposablePartDefinition> _addedParts;
            private HashSet<ComposablePartDefinition> _removedParts;

            public CatalogChangeProxy(ComposablePartCatalog originalCatalog,
                IEnumerable<ComposablePartDefinition> addedParts,
                IEnumerable<ComposablePartDefinition> removedParts)
            {
                this._originalCatalog = originalCatalog;
                this._addedParts = new List<ComposablePartDefinition>(addedParts);
                this._removedParts = new HashSet<ComposablePartDefinition>(removedParts);
            }

            public override IQueryable<ComposablePartDefinition> Parts
            {
                get
                {
                    return this._originalCatalog.Parts.Concat(this._addedParts).Except(this._removedParts);
                }
            }

            public override IEnumerable<Tuple<ComposablePartDefinition, ExportDefinition>> GetExports(
                ImportDefinition import)
            {
                var originalExports = this._originalCatalog.GetExports(import);
                var trimmedExports = originalExports.Where(partAndExport =>
                    !this._removedParts.Contains(partAndExport.Item1));

                var addedExports = new List<Tuple<ComposablePartDefinition, ExportDefinition>>();
                foreach (var part in this._addedParts)
                {
                    foreach (var export in part.ExportDefinitions)
                    {
                        if (import.IsConstraintSatisfiedBy(export))
                        {
                            addedExports.Add(new Tuple<ComposablePartDefinition, ExportDefinition>(part, export));
                        }
                    }
                }
                return trimmedExports.Concat(addedExports);
            }
        }
    }
}