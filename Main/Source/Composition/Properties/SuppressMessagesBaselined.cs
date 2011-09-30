// -----------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
// -----------------------------------------------------------------------
using System;
using System.Diagnostics.CodeAnalysis;

// The following are untriaged violations, do not add to this list. Any explicitly suppressed violations
// should either be applied against the member or type itself, or if raised against a namespace, resource 
// or assembly, placed in SuppressMessages.cs.

// BUG: ConsiderPassingBaseTypesAsParameters does not take into account generic type inference
[module: SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Scope = "member", Target = "Sandcastle.Composition.CompositionServiceExtensions.#AddExport`1(Sandcastle.Composition.ICompositionService,Sandcastle.Composition.Export`1<!!0>)")]

// BUG: 441625 - Structure values uses culture-sensitive comparisons when it should be using ordinal comparisons
[module: SuppressMessage("Microsoft.Globalization", "CA1307:SpecifyStringComparison", MessageId = "System.String.CompareTo(System.String)", Scope = "member", Target = "System.Reflection.StructuredValues.ProjectionContext.#TryGetStructuralContractAdapter(System.Collections.Generic.IEnumerable`1<System.Reflection.MemberInfo>,System.Reflection.StructuredValues.StructuredValue,System.Type&,System.Reflection.StructuredValues.IStructureToContractAdapter&)")]

// DefineAccessorsForAttributeArguments and DoNotNestGenericTypesInMemberSignatures warnings are design issues that will be looked at during M2.
[module: SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Scope = "type", Target = "Sandcastle.Composition.ImportAttribute")]
[module: SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Scope = "type", Target = "Sandcastle.Composition.ImportManyAttribute")]
[module: SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Scope = "type", Target = "Sandcastle.Composition.ExportAttribute")]
[module: SuppressMessage("Microsoft.Design", "CA1019:DefineAccessorsForAttributeArguments", Scope = "type", Target = "Sandcastle.Composition.ContractTypeAttribute")]

// This will be resolved when we decide whether we are going to pass the constraint around as an Expression.
[module: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExport(System.Type,System.Type,System.Linq.Expressions.Expression`1<System.Func`2<Sandcastle.Composition.Primitives.ExportDefinition,System.Boolean>>,Sandcastle.Composition.RequiredCardinality)")]
[module: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExport`1(System.Linq.Expressions.Expression`1<System.Func`2<Sandcastle.Composition.Primitives.ExportDefinition,System.Boolean>>)")]
[module: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExport`1(System.Linq.Expressions.Expression`1<System.Func`2<Sandcastle.Composition.Primitives.ExportDefinition,System.Boolean>>,Sandcastle.Composition.RequiredCardinality)")]
[module: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExport`2(System.Linq.Expressions.Expression`1<System.Func`2<Sandcastle.Composition.Primitives.ExportDefinition,System.Boolean>>,Sandcastle.Composition.RequiredCardinality)")]
[module: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExports(System.Type,System.Type,System.Linq.Expressions.Expression`1<System.Func`2<Sandcastle.Composition.Primitives.ExportDefinition,System.Boolean>>)")]
[module: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExports`1(System.Linq.Expressions.Expression`1<System.Func`2<Sandcastle.Composition.Primitives.ExportDefinition,System.Boolean>>)")]
[module: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExports`2(System.Linq.Expressions.Expression`1<System.Func`2<Sandcastle.Composition.Primitives.ExportDefinition,System.Boolean>>)")]
[module: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExport(System.Type,System.Type,System.Linq.Expressions.Expression`1<System.Func`2<Sandcastle.Composition.Primitives.ExportDefinition,System.Boolean>>,Sandcastle.Composition.SingleExportCardinality)")]
[module: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExport`2(System.Linq.Expressions.Expression`1<System.Func`2<Sandcastle.Composition.Primitives.ExportDefinition,System.Boolean>>,Sandcastle.Composition.SingleExportCardinality)")]
[module: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Sandcastle.Composition.Hosting.ExportProvider.#GetExport`1(System.Linq.Expressions.Expression`1<System.Func`2<Sandcastle.Composition.Primitives.ExportDefinition,System.Boolean>>,Sandcastle.Composition.SingleExportCardinality)")]
[module: SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Scope = "member", Target = "Sandcastle.Composition.Primitives.ImportDefinition.#GetConstraint`1()")]
[module: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Sandcastle.Composition.Primitives.ComposablePartCatalog.#GetExports(Sandcastle.Composition.Primitives.ImportDefinition)")]
[module: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Sandcastle.Composition.Primitives.ImportDefinition.#Constraint")]
[module: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Sandcastle.Composition.Primitives.ImportDefinition.#.ctor(System.Linq.Expressions.Expression`1<System.Func`2<Sandcastle.Composition.Primitives.ExportDefinition,System.Boolean>>,System.String,Sandcastle.Composition.Primitives.ImportCardinality,System.Boolean,System.Boolean)")]

[module: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Sandcastle.Composition.ReflectionModel.ReflectionModelServices.#CreatePartDefinition(System.Lazy`1<System.Type>,System.Lazy`1<System.Collections.Generic.IEnumerable`1<Sandcastle.Composition.Primitives.ImportDefinition>>,System.Lazy`1<System.Collections.Generic.IEnumerable`1<Sandcastle.Composition.Primitives.ExportDefinition>>,System.Lazy`1<System.Collections.Generic.IDictionary`2<System.String,System.Object>>,Sandcastle.Composition.Primitives.ICompositionElement)")]
[module: SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Scope = "member", Target = "Sandcastle.Composition.ReflectionModel.ReflectionModelServices.#CreateExportDefinition(Sandcastle.Composition.ReflectionModel.LazyMemberInfo,System.String,System.Lazy`1<System.Collections.Generic.IDictionary`2<System.String,System.Object>>,Sandcastle.Composition.Primitives.ICompositionElement)")]

// When switching to CLR4 the security model has changed
[module: SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase", Scope = "member", Target = "Sandcastle.Composition.CompositionException.#GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)")]
[module: SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase", Scope = "member", Target = "Sandcastle.Composition.Primitives.ComposablePartException.#GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)")]
[module: SuppressMessage("Microsoft.Security", "CA2123:OverrideLinkDemandsShouldBeIdenticalToBase", Scope = "member", Target = "Sandcastle.Composition.CompositionError.#System.Runtime.Serialization.ISerializable.GetObjectData(System.Runtime.Serialization.SerializationInfo,System.Runtime.Serialization.StreamingContext)")]
