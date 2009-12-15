﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 4885 $</version>
// </file>

using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

using ICSharpCode.SharpDevelop;

namespace ICSharpCode.FormsDesigner
{
	public interface IDesignerGenerator
	{
		CodeDomProvider CodeDomProvider {
			get;
		}
		void Attach(FormsDesignerViewContent viewContent);
		void Detach();
		FormsDesignerViewContent ViewContent { get; }
		/// <summary>
		/// Gets the collection of OpenedFiles that contain code which belongs
		/// to the designed form, not including resource files.
		/// </summary>
		/// <param name="designerCodeFile">Receives the file which contains the code to be modified by the forms designer.</param>
		/// <returns>A collection of OpenedFiles that contain code which belongs to the designed form.</returns>
		/// <remarks>The returned collection must include the <paramref name="designerCodeFile"/>.</remarks>
		IEnumerable<OpenedFile> GetSourceFiles(out OpenedFile designerCodeFile);
		void MergeFormChanges(CodeCompileUnit unit);
		bool InsertComponentEvent(IComponent component, EventDescriptor edesc, string eventMethodName, string body, out string file, out int position);
		ICollection GetCompatibleMethods(EventDescriptor edesc);
	}
	
	public interface IDesignerGenerator2 : IDesignerGenerator
	{
		void NotifyFormRenamed(string newName);
	}
}
