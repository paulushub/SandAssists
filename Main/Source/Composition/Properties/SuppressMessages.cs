// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Diagnostics.CodeAnalysis;

[module: SuppressMessage("Microsoft.MSInternal", "CA900:AptcaAssembliesShouldBeReviewed",
                        Justification = "Waiting for FxCop team to add our assembly to the whitelist")]


[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "System.Lazy`2", 
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.AttributedModelServices",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.ChangeRejectedException",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.CompositionContractMismatchException",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.CompositionError",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.CompositionException",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.CreationPolicy",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.ExportAttribute",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.ExportMetadataAttribute",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.ImportAttribute",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.ImportCardinalityMismatchException",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.ImportingConstructorAttribute",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.ImportManyAttribute",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.InheritedExportAttribute", 
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.MetadataAttributeAttribute",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.PartCreationPolicyAttribute",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.PartMetadataAttribute",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.PartNotDiscoverableAttribute",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Hosting.AdaptingExportProvider",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Hosting.AggregateCatalog",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Hosting.AggregateExportProvider",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Hosting.AssemblyCatalog",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Hosting.CatalogExportProvider",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Hosting.ComposablePartCatalogChangeEventArgs",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Hosting.ComposablePartExportProvider",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Hosting.CompositionBatch",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Hosting.CompositionConstants",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Hosting.CompositionContainer",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Hosting.AtomicComposition",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Hosting.DirectoryCatalog",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Primitives.ExportedDelegate",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Hosting.ExportProvider",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Hosting.ExportsChangeEventArgs",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Hosting.ImportEngine",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Hosting.TypeCatalog",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Primitives.Export",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Primitives.ExportDefinition",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Primitives.ComposablePart",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Primitives.ComposablePartCatalog",
                        Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Primitives.ComposablePartDefinition", Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Primitives.ComposablePartException", Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Primitives.ContractBasedImportDefinition", Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Primitives.ImportCardinality", Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.Primitives.ImportDefinition", Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.ReflectionModel.LazyMemberInfo", Justification = "Razzle mscorlib is not APTCA")]
[module: SuppressMessage("Microsoft.Security", "CA2117:AptcaTypesShouldOnlyExtendAptcaBaseTypes", Scope = "type", Target = "Sandcastle.Composition.ReflectionModel.ReflectionModelServices", Justification = "Razzle mscorlib is not APTCA")]


[module: SuppressMessage("Microsoft.MSInternal", "CA905:SystemAndMicrosoftNamespacesRequireApproval", Scope = "namespace", Target = "Sandcastle.Composition",
                        Justification = "Approved by Framework")]

[module: SuppressMessage("Microsoft.MSInternal", "CA905:SystemNamespacesRequireApproval", Scope = "namespace", Target = "Sandcastle.Composition.Caching",
                        Justification = "Approved by Framework")]

[module: SuppressMessage("Microsoft.MSInternal", "CA905:SystemNamespacesRequireApproval", Scope = "namespace", Target = "Sandcastle.Composition.Hosting")]

[module: SuppressMessage("Microsoft.MSInternal", "CA905:SystemNamespacesRequireApproval", Scope = "namespace", Target = "Sandcastle.Composition.Primitives")]
[module: SuppressMessage("Microsoft.MSInternal", "CA905:SystemNamespacesRequireApproval", Scope = "namespace", Target = "Sandcastle.Composition.ReflectionModel")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Sandcastle.Composition.ReflectionModel")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Windows")]

// BUG: DDB 90145 - GenericMethodsShouldProvideTypeParameter should ignore methods that returns T (Code Analysis bug)
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExports`2(System.String)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExports`2(System.Linq.Expressions.Expression`1<System.Func`2<Sandcastle.Composition.Primitives.ExportDefinition,System.Boolean>>)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExports`2()")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExports`1(System.String)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExports`1(System.Linq.Expressions.Expression`1<System.Func`2<Sandcastle.Composition.Primitives.ExportDefinition,System.Boolean>>)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExports`1()")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExportedValues`1(System.String)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExportedValues`1()")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExportedValueOrDefault`1(System.String)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExportedValueOrDefault`1()")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExportedValue`1(System.String)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExportedValue`1()")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExport`2(System.String)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExport`2(System.Linq.Expressions.Expression`1<System.Func`2<Sandcastle.Composition.Primitives.ExportDefinition,System.Boolean>>)")]
[module: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures",  Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExport`2(System.Linq.Expressions.Expression`1<System.Func`2<Sandcastle.Composition.Primitives.ExportDefinition,System.Boolean>>)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExport`2()")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExport`1(System.String)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExport`1(System.Linq.Expressions.Expression`1<System.Func`2<Sandcastle.Composition.Primitives.ExportDefinition,System.Boolean>>)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExport`1()")]

#if SILVERLIGHT

// BUG: Dev10 - 434542 ImplementStandardExceptionConstructors fires on Silverlight exceptions even though serialization is not supported on that Framework (Code Analysis bug)
[module: SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Scope = "type", Target = "Sandcastle.Composition.CompositionException")]
[module: SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Scope = "type", Target = "Sandcastle.Composition.CardinalityMismatchException")]
[module: SuppressMessage("Microsoft.Design", "CA1032:ImplementStandardExceptionConstructors", Scope = "type", Target = "Microsoft.Internal.Assumes+InternalErrorException")]

// Code Analysis bugs
[module: SuppressMessage("Microsoft.Usage", "CA2235:MarkAllNonSerializableFields", Scope = "member", Target = "Sandcastle.Composition.CompositionException.#_message")]
[module: SuppressMessage("Microsoft.Usage", "CA2235:MarkAllNonSerializableFields", Scope = "member", Target = "Sandcastle.Composition.ComposablePartException.#_id")]
[module: SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Scope="type", Target="Sandcastle.Composition.ICompositionError")]
[module: SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Scope="type", Target="Sandcastle.Composition.IAttributedImport")]
[module: SuppressMessage("Microsoft.Design", "CA1064:ExceptionsShouldBePublic", Scope="type", Target="Sandcastle.Composition.ReflectionModel.IReflectionPartCreationInfo")]
#endif

// All of these will go away when more types in Advanced and Primitives, and Tuple is removed from System.
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "System.Threading")]
[module: SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Sandcastle.Composition.Primitives")]

// These warnings are deliberate design decision. ICompositionElement is an advanced type and we don't want to dirty the API by make these members public
[module: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Sandcastle.Composition.Hosting.AssemblyCatalog.#Sandcastle.Composition.Primitives.ICompositionElement.DisplayName")]
[module: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Sandcastle.Composition.Hosting.AssemblyCatalog.#Sandcastle.Composition.Primitives.ICompositionElement.Origin")]
[module: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Sandcastle.Composition.Hosting.DirectoryCatalog.#Sandcastle.Composition.Primitives.ICompositionElement.DisplayName")]
[module: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Sandcastle.Composition.Hosting.DirectoryCatalog.#Sandcastle.Composition.Primitives.ICompositionElement.Origin")]
[module: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Sandcastle.Composition.Hosting.TypeCatalog.#Sandcastle.Composition.Primitives.ICompositionElement.DisplayName")]
[module: SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Sandcastle.Composition.Hosting.TypeCatalog.#Sandcastle.Composition.Primitives.ICompositionElement.Origin")]
