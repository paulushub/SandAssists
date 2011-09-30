﻿// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
#if !SILVERLIGHT

using System;
using System.Collections.ObjectModel;
using Sandcastle.Composition.Primitives;
using Sandcastle.Composition.ReflectionModel;
using System.Linq;
using System.Reflection;
using Microsoft.Internal;
using Microsoft.Internal.Collections;

namespace Sandcastle.Composition.Hosting
{
    partial class DirectoryCatalog
    {
        internal class DirectoryCatalogDebuggerProxy
        {
            private readonly DirectoryCatalog _catalog;

            public DirectoryCatalogDebuggerProxy(DirectoryCatalog catalog)
            {
                Requires.NotNull(catalog, "catalog");

                this._catalog = catalog;
            }

            public ReadOnlyCollection<Assembly> Assemblies
            {
                get
                {
                    return this._catalog._assemblyCatalogs.Values.Select(catalog => catalog.Assembly)
                                                                 .ToReadOnlyCollection();
                }
            }

            public string SearchPattern
            {
                get { return this._catalog.SearchPattern; }
            }

            public string Path
            {
                get { return this._catalog._path; }
            }

            public string FullPath
            {
                get { return this._catalog._fullPath; }
            }

            public ReadOnlyCollection<string> LoadedFiles
            {
                get { return this._catalog._loadedFiles; }
            }

            public ReadOnlyCollection<ComposablePartDefinition> Parts
            {
                // NOTE: This shouldn't be cached, so that on every query of
                // the current value of the underlying catalog is respected.
                // We use ReadOnlyCollection as arrays do not have the 
                // appropriate debugger display attributes applied to them.
                get { return this._catalog.Parts.ToReadOnlyCollection(); }
            }
        }
    }
}

#endif