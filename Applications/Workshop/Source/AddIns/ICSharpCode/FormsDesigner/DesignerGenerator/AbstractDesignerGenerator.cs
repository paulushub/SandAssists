// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 4888 $</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;
using ICSharpCode.SharpDevelop.Refactoring;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;
using ReflectionLayer = ICSharpCode.SharpDevelop.Dom.ReflectionLayer;

namespace ICSharpCode.FormsDesigner
{
	public abstract class AbstractDesignerGenerator : IDesignerGenerator2
	{
		/// <summary>The currently open part of the class being designed.</summary>
		IClass currentClassPart;
		/// <summary>The complete class being designed.</summary>
		IClass  completeClass;
		/// <summary>The class part containing the designer code.</summary>
		IClass  formClass;
		IMethod initializeComponents;
		
		FormsDesignerViewContent viewContent;
		CodeDomProvider provider;
		
		public CodeDomProvider CodeDomProvider {
			get {
				if (this.provider == null) {
					this.provider = this.CreateCodeProvider();
				}
				return this.provider;
			}
		}
		
		public FormsDesignerViewContent ViewContent {
			get {
				return viewContent;
			}
		}
		
		/// <summary>
		/// Gets the part of the designed class that is open in the source code editor which the designer view is attached to.
		/// </summary>
		protected IClass CurrentClassPart {
			get { return this.currentClassPart; }
			set { this.currentClassPart = value; }
		}
		
		public void Attach(FormsDesignerViewContent viewContent)
		{
			this.viewContent = viewContent;
		}
		
		public void Detach()
		{
			this.viewContent = null;
		}
		
		public IEnumerable<OpenedFile> GetSourceFiles(out OpenedFile designerCodeFile)
		{
			// get new initialize components
			ParseInformation info = ParserService.ParseFile(this.viewContent.PrimaryFileName, this.viewContent.PrimaryFileContent, false);
			ICompilationUnit cu = info.BestCompilationUnit;
			foreach (IClass c in cu.Classes) {
				if (FormsDesignerSecondaryDisplayBinding.BaseClassIsFormOrControl(c)) {
					this.currentClassPart = c;
					this.initializeComponents = FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(c);
					if (this.initializeComponents != null) {
						string designerFileName = this.initializeComponents.DeclaringType.CompilationUnit.FileName;
						if (designerFileName != null) {
							
							designerCodeFile = FileService.GetOrCreateOpenedFile(designerFileName);
							
							CompoundClass compound = c.GetCompoundClass() as CompoundClass;
							if (compound == null) {
								return new [] {designerCodeFile};
							} else {
								return compound.Parts
									.Select(cl => FileService.GetOrCreateOpenedFile(cl.CompilationUnit.FileName))
									.Distinct();
							}
							
						}
					}
				}
			}
			
			throw new FormsDesignerLoadException("Could not find InitializeComponent method in any part of the open class.");
		}
		
		/// <summary>
		/// Removes the field declaration with the specified name from the source file.
		/// </summary>
		void RemoveField(string fieldName)
		{
			try {
				LoggingService.Info("Remove field declaration: "+fieldName);
				Reparse();
				
				IField field = GetField(completeClass, fieldName);
				if (field != null) {
					string fileName = field.DeclaringType.CompilationUnit.FileName;
					LoggingService.Debug("-> Field is declared in file '" + fileName + "', Region: " + field.Region.ToString());
					OpenedFile file = FileService.GetOpenedFile(fileName);
					if (file == null) throw new InvalidOperationException("The file where the field is declared is not open, although it belongs to the class.");
					ITextDocument doc = this.ViewContent.GetDocumentForFile(file);
					if (doc == null) throw new InvalidOperationException("Could not get document for file '" + file.FileName + "'.");
					
					this.RemoveFieldDeclaration(doc, field);
					file.MakeDirty();
				} else {
					LoggingService.Warn("-> Field '" + fieldName + "' not found in class");
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		protected virtual string GenerateFieldDeclaration(CodeDOMGenerator domGenerator, CodeMemberField field)
		{
			StringWriter writer = new StringWriter();
			domGenerator.ConvertContentDefinition(field, writer);
			return writer.ToString().Trim();
		}
		
		/// <summary>
		/// Contains the tabs in front of the InitializeComponents declaration.
		/// Used to indent the fields and generated statements.
		/// </summary>
		protected string tabs;
		
		/// <summary>
		/// Adds the declaration for the specified field to the source file
		/// or replaces the already present declaration for a field with the same name.
		/// </summary>
		/// <param name="domGenerator">The CodeDOMGenerator used to generate the field declaration.</param>
		/// <param name="newField">The CodeDom field to be added or replaced.</param>
		void AddOrReplaceField(CodeDOMGenerator domGenerator, CodeMemberField newField)
		{
			try {
				Reparse();
				IField oldField = GetField(completeClass, newField.Name);
				if (oldField != null) {
					string fileName = oldField.DeclaringType.CompilationUnit.FileName;
					LoggingService.Debug("-> Old field is declared in file '" + fileName + "', Region: " + oldField.Region.ToString());
					OpenedFile file = FileService.GetOpenedFile(fileName);
					if (file == null) throw new InvalidOperationException("The file where the field is declared is not open, although it belongs to the class.");
					ITextDocument doc = this.ViewContent.GetDocumentForFile(file);
					if (doc == null) throw new InvalidOperationException("Could not get document for file '" + file.FileName + "'.");
					this.ReplaceFieldDeclaration(doc, oldField, GenerateFieldDeclaration(domGenerator, newField));
					file.MakeDirty();
				} else {
					int endOffset = this.ViewContent.DesignerCodeFileDocument.PositionToOffset(new TextLocation(0, initializeComponents.BodyRegion.EndLine));
					this.ViewContent.DesignerCodeFileDocument.Insert(endOffset, tabs + GenerateFieldDeclaration(domGenerator, newField) + Environment.NewLine);
				}
			} catch (Exception ex) {
				MessageService.ShowError(ex);
			}
		}
		
		protected abstract System.CodeDom.Compiler.CodeDomProvider CreateCodeProvider();
		
		protected abstract DomRegion GetReplaceRegion(ICSharpCode.TextEditor.Document.ITextDocument document, IMethod method);
		
		/// <summary>
		/// Removes a field declaration from the source code document.
		/// </summary>
		/// <remarks>
		/// The default implementation assumes that the field region starts at the very beginning
		/// of the line of the field declaration and ends at the end of that line.
		/// Override this method if that is not the case in a specific language.
		/// </remarks>
		protected virtual void RemoveFieldDeclaration(ITextDocument document, IField field)
		{
			int startOffset = document.PositionToOffset(new TextLocation(0, field.Region.BeginLine - 1));
			int endOffset   = document.PositionToOffset(new TextLocation(0, field.Region.EndLine));
			document.Remove(startOffset, endOffset - startOffset);
		}
		
		/// <summary>
		/// Replaces a field declaration in the source code document.
		/// </summary>
		/// <remarks>
		/// The default implementation assumes that the field region starts at the very beginning
		/// of the line of the field declaration and ends at the end of that line.
		/// Override this method if that is not the case in a specific language.
		/// </remarks>
		protected virtual void ReplaceFieldDeclaration(ITextDocument document, IField oldField, string newFieldDeclaration)
		{
			int startOffset = document.PositionToOffset(new TextLocation(0, oldField.Region.BeginLine - 1));
			int endOffset   = document.PositionToOffset(new TextLocation(0, oldField.Region.EndLine));
			document.Replace(startOffset, endOffset - startOffset, tabs + newFieldDeclaration + Environment.NewLine);
		}
		
		protected virtual void FixGeneratedCode(IClass formClass, CodeMemberMethod code)
		{
		}
		
		public virtual void NotifyFormRenamed(string newName)
		{
			Reparse();
			LoggingService.Info("Renaming form to " + newName);
			if (this.formClass == null) {
				LoggingService.Warn("Cannot rename, formClass not found");
			} else {
				ICSharpCode.SharpDevelop.Refactoring.FindReferencesAndRenameHelper.RenameClass(this.formClass, newName);
				Reparse();
			}
		}
		
		public virtual void MergeFormChanges(CodeCompileUnit unit)
		{
			Reparse();
			
			// find InitializeComponent method and the class it is declared in
			CodeTypeDeclaration formClass = null;
			CodeMemberMethod initializeComponent = null;
			foreach (CodeNamespace n in unit.Namespaces) {
				foreach (CodeTypeDeclaration typeDecl in n.Types) {
					foreach (CodeTypeMember m in typeDecl.Members) {
						if (m is CodeMemberMethod && m.Name == "InitializeComponent") {
							formClass = typeDecl;
							initializeComponent = (CodeMemberMethod)m;
							break;
						}
					}
				}
			}
			
			if (formClass == null || initializeComponent == null) {
				throw new InvalidOperationException("InitializeComponent method not found in framework-generated CodeDom.");
			}
			if (this.formClass == null) {
				MessageService.ShowMessage("Cannot save form: InitializeComponent method does not exist anymore. You should not modify the Designer.cs file while editing a form.");
				return;
			}
			
			FixGeneratedCode(this.formClass, initializeComponent);
			
			// generate file and get initialize components string
			StringWriter writer = new StringWriter();
			CodeDOMGenerator domGenerator = new CodeDOMGenerator(this.CodeDomProvider, tabs + '\t');
			domGenerator.ConvertContentDefinition(initializeComponent, writer);
			
			string statements = writer.ToString();
			
			// initializeComponents.BodyRegion.BeginLine + 1
			DomRegion bodyRegion = GetReplaceRegion(this.ViewContent.DesignerCodeFileDocument, initializeComponents);
			if (bodyRegion.BeginColumn <= 0 || bodyRegion.EndColumn <= 0)
				throw new InvalidOperationException("Column must be > 0");
			int startOffset = this.ViewContent.DesignerCodeFileDocument.PositionToOffset(new TextLocation(bodyRegion.BeginColumn - 1, bodyRegion.BeginLine - 1));
			int endOffset   = this.ViewContent.DesignerCodeFileDocument.PositionToOffset(new TextLocation(bodyRegion.EndColumn - 1, bodyRegion.EndLine - 1));
			
			this.ViewContent.DesignerCodeFileDocument.Replace(startOffset, endOffset - startOffset, statements);
			
			// apply changes the designer made to field declarations
			// first loop looks for added and changed fields
			foreach (CodeTypeMember m in formClass.Members) {
				if (m is CodeMemberField) {
					CodeMemberField newField = (CodeMemberField)m;
					IField oldField = GetField(completeClass, newField.Name);
					if (oldField == null || FieldChanged(oldField, newField)) {
						AddOrReplaceField(domGenerator, newField);
					}
				}
			}
			
			// second loop looks for removed fields
			List<string> removedFields = new List<string>();
			foreach (IField field in completeClass.Fields) {
				bool found = false;
				foreach (CodeTypeMember m in formClass.Members) {
					if (m is CodeMemberField && m.Name == field.Name) {
						found = true;
						break;
					}
				}
				if (!found) {
					removedFields.Add(field.Name);
				}
			}
			// removing fields is done in two steps because
			// we must not modify the c.Fields collection while it is enumerated
			removedFields.ForEach(RemoveField);
			
			ParserService.EnqueueForParsing(this.ViewContent.DesignerCodeFile.FileName, this.ViewContent.DesignerCodeFileDocument.TextContent);
		}
		
		/// <summary>
		/// Compares the SharpDevelop.Dom field declaration oldField to
		/// the CodeDom field declaration newField.
		/// </summary>
		/// <returns>true, if the fields are different in type or modifiers, otherwise false.</returns>
		static bool FieldChanged(IField oldField, CodeMemberField newField)
		{
			// compare types
			if (oldField.ReturnType != null && newField.Type != null) {
				if (AreTypesDifferent(oldField.ReturnType, newField.Type)) {
					LoggingService.Debug("FieldChanged (type): "+oldField.Name+", "+oldField.ReturnType.FullyQualifiedName+" -> "+newField.Type.BaseType);
					return true;
				}
			}
			
			// compare modifiers
			ModifierEnum oldModifiers = oldField.Modifiers & ModifierEnum.VisibilityMask;
			MemberAttributes newModifiers = newField.Attributes & MemberAttributes.AccessMask;
			
			// SharpDevelop.Dom always adds Private modifier, even if not specified
			// CodeDom omits Private modifier if not present (although it is the default)
			if (oldModifiers == ModifierEnum.Private) {
				if (newModifiers != 0 && newModifiers != MemberAttributes.Private) {
					return true;
				}
			}
			
			ModifierEnum[] sdModifiers = new ModifierEnum[] {ModifierEnum.Protected, ModifierEnum.ProtectedAndInternal, ModifierEnum.Internal, ModifierEnum.Public};
			MemberAttributes[] cdModifiers = new MemberAttributes[] {MemberAttributes.Family, MemberAttributes.FamilyOrAssembly, MemberAttributes.Assembly, MemberAttributes.Public};
			for (int i = 0; i < sdModifiers.Length; i++) {
				if ((oldModifiers  == sdModifiers[i]) ^ (newModifiers  == cdModifiers[i])) {
					return true;
				}
			}
			
			return false;
		}
		
		static bool AreTypesDifferent(IReturnType oldType, CodeTypeReference newType)
		{
			IClass oldClass = oldType.GetUnderlyingClass();
			if (oldClass == null) {
				// ignore type changes to untyped VB fields
				return false;
			}
			if (newType.BaseType == "System.Void") {
				// field types get replaced with System.Void if the type cannot be resolved
				// (e.g. generic fields in the Boo designer which aren't converted to CodeDom)
				// we'll ignore such type changes (fields should never have the type void)
				return false;
			}
			
			ArrayReturnType oldArray = oldType.IsArrayReturnType ? oldType.CastToArrayReturnType() : null;
			if (oldArray == null ^ newType.ArrayRank < 1)
			{
				return true;
			}
			
			if (oldArray == null) {
				
				if (oldClass.DotNetName != newType.BaseType) {
					return true;
				}
				
			} else {
				
				if (oldArray.ArrayDimensions != newType.ArrayRank) {
					return true;
				}
				if (AreTypesDifferent(oldArray.ArrayElementType, newType.ArrayElementType)) {
					return true;
				}
				
			}
			
			return false;
		}
		
		protected void Reparse()
		{
			Dictionary<OpenedFile, ParseInformation> parsings = new Dictionary<OpenedFile, ParseInformation>();
			ParseInformation info;
			ICompilationUnit cu;
			
			// Reparse all source files for the designed form
			foreach (KeyValuePair<OpenedFile, ITextDocument> entry in this.ViewContent.SourceFiles) {
				parsings.Add(entry.Key, ParserService.ParseFile(entry.Key.FileName, entry.Value.TextContent, false));
			}
			
			// Update currentClassPart from PrimaryFile
			this.currentClassPart = null;
			if (this.ViewContent.PrimaryFile != null && parsings.TryGetValue(this.ViewContent.PrimaryFile, out info)) {
				cu = info.BestCompilationUnit;
				foreach (IClass c in cu.Classes) {
					if (FormsDesignerSecondaryDisplayBinding.BaseClassIsFormOrControl(c)) {
						if (FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(c) != null) {
							this.currentClassPart = c;
							break;
						}
					}
				}
				if (this.currentClassPart == null) {
					LoggingService.Warn("AbstractDesignerGenerator.Reparse: Could not find designed class in primary file '" + this.ViewContent.PrimaryFile.FileName + "'");
				}
			} else {
				LoggingService.Debug("AbstractDesignerGenerator.Reparse: Primary file is unavailable");
			}
			
			// Update initializeComponents, completeClass and formClass
			// from designer code file
			this.completeClass = null;
			this.formClass = null;
			this.initializeComponents = null;
			if (this.ViewContent.DesignerCodeFile == null ||
			    !parsings.TryGetValue(this.ViewContent.DesignerCodeFile, out info)) {
				LoggingService.Warn("AbstractDesignerGenerator.Reparse: Designer source code file is unavailable");
				if (this.currentClassPart != null) {
					this.completeClass = this.currentClassPart.GetCompoundClass();
				}
				return;
			}
			
			cu = info.BestCompilationUnit;
			foreach (IClass c in cu.Classes) {
				if (FormsDesignerSecondaryDisplayBinding.BaseClassIsFormOrControl(c)) {
					this.initializeComponents = FormsDesignerSecondaryDisplayBinding.GetInitializeComponents(c);
					if (this.initializeComponents != null) {
						using (StringReader r = new StringReader(this.ViewContent.DesignerCodeFileContent)) {
							int count = this.initializeComponents.Region.BeginLine;
							for (int i = 1; i < count; i++)
								r.ReadLine();
							string line = r.ReadLine();
							tabs = GetIndentation(line);
						}
						this.completeClass = c.GetCompoundClass();
						this.formClass = this.initializeComponents.DeclaringType;
						break;
					}
				}
			}
			
			if (this.completeClass == null || this.formClass == null) {
				LoggingService.Warn("AbstractDesignerGenerator.Reparse: Could not find InitializeComponents in designer source code file '" + this.ViewContent.DesignerCodeFile.FileName + "'");
			}
		}
		
		protected static string GetIndentation(string line)
		{
			return line.Substring(0, line.Length - line.TrimStart().Length);
		}
		
		protected abstract string CreateEventHandler(Type eventType, string eventMethodName, string body, string indentation);
		
		protected virtual int GetCursorLine(ITextDocument document, IMethod method)
		{
			return method.BodyRegion.BeginLine + 1;
		}
		
		protected virtual int GetCursorLineAfterEventHandlerCreation()
		{
			return 2;
		}
		
		/// <summary>
		/// If found return true and int as position
		/// </summary>
		/// <param name="component"></param>
		/// <param name="edesc"></param>
		/// <returns></returns>
		public virtual bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out string file, out int position)
		{
			Reparse();
			
			LoggingService.Debug("Forms designer: AbstractDesignerGenerator.InsertComponentEvent: eventMethodName=" + eventMethodName);
			
			foreach (IMethod method in completeClass.Methods) {
				if (method.Name == eventMethodName) {
					file = method.DeclaringType.CompilationUnit.FileName;
					OpenedFile openedFile = FileService.GetOpenedFile(file);
					ITextDocument doc;
					if (openedFile != null && (doc = this.ViewContent.GetDocumentForFile(openedFile)) != null) {
						position = GetCursorLine(doc, method);
					} else {
						try {
							position = GetCursorLine(FindReferencesAndRenameHelper.GetDocumentInformation(file).CreateDocument(), method);
						} catch (FileNotFoundException) {
							position = 0;
							return false;
						}
					}
					return true;
				}
			}
			
			viewContent.MergeFormChanges();
			Reparse();
			
			file = currentClassPart.CompilationUnit.FileName;
			int line = GetEventHandlerInsertionLine(currentClassPart);
			LoggingService.Debug("-> Inserting new event handler at line " + line.ToString(System.Globalization.CultureInfo.InvariantCulture));
			
			if (line - 1 == this.viewContent.PrimaryFileDocument.TotalNumberOfLines) {
				// insert a newline at the end of file if necessary (can happen in Boo if there is no newline at the end of the document)
				this.viewContent.PrimaryFileDocument.Insert(this.viewContent.PrimaryFileDocument.TextLength, Environment.NewLine);
			}
			
			int offset = this.viewContent.PrimaryFileDocument.GetLineSegment(line - 1).Offset;
			
			this.viewContent.PrimaryFileDocument.Insert(offset, CreateEventHandler(edesc.EventType, eventMethodName, body, tabs));
			position = line + GetCursorLineAfterEventHandlerCreation();
			this.viewContent.PrimaryFile.MakeDirty();
			
			return true;
		}
		
		/// <summary>
		/// Gets a method implementing the signature specified by the event descriptor
		/// </summary>
		protected static IMethod ConvertEventInvokeMethodToDom(IClass declaringType, Type eventType, string methodName)
		{
			MethodInfo mInfo = eventType.GetMethod("Invoke");
			DefaultMethod m = new DefaultMethod(declaringType, methodName);
			m.ReturnType = ReflectionLayer.ReflectionReturnType.Create(m, mInfo.ReturnType, false);
			foreach (ParameterInfo pInfo in mInfo.GetParameters()) {
				m.Parameters.Add(new ReflectionLayer.ReflectionParameter(pInfo, m));
			}
			return m;
		}
		
		/// <summary>
		/// Gets a method implementing the signature specified by the event descriptor
		/// </summary>
		protected static ICSharpCode.NRefactory.Ast.MethodDeclaration
			ConvertEventInvokeMethodToNRefactory(IClass context, Type eventType, string methodName)
		{
			if (context == null)
				throw new ArgumentNullException("context");
			
			return ICSharpCode.SharpDevelop.Dom.Refactoring.CodeGenerator.ConvertMember(
				ConvertEventInvokeMethodToDom(context, eventType, methodName),
				new ClassFinder(context, context.BodyRegion.BeginLine + 1, 1)
			) as ICSharpCode.NRefactory.Ast.MethodDeclaration;
		}
		
		protected virtual int GetEventHandlerInsertionLine(IClass c)
		{
			return c.Region.EndLine;
		}
		
		public virtual ICollection GetCompatibleMethods(EventDescriptor edesc)
		{
			Reparse();
			ArrayList compatibleMethods = new ArrayList();
			MethodInfo methodInfo = edesc.EventType.GetMethod("Invoke");
			foreach (IMethod method in completeClass.Methods) {
				if (method.Parameters.Count == methodInfo.GetParameters().Length) {
					bool found = true;
					for (int i = 0; i < methodInfo.GetParameters().Length; ++i) {
						ParameterInfo pInfo = methodInfo.GetParameters()[i];
						IParameter p = method.Parameters[i];
						if (p.ReturnType.FullyQualifiedName != pInfo.ParameterType.ToString()) {
							found = false;
							break;
						}
					}
					if (found) {
						compatibleMethods.Add(method.Name);
					}
				}
			}
			
			return compatibleMethods;
		}
		
		protected IField GetField(IClass c, string name)
		{
			foreach (IField field in c.Fields) {
				if (field.Name == name) {
					return field;
				}
			}
			return null;
		}
	}
}
