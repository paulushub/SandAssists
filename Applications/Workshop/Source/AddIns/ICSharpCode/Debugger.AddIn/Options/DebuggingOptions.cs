﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="David Srbecký" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 4074 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using Debugger;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Services
{
	public class DebuggingOptions : Options
	{
		public static DebuggingOptions Instance {
			get {
				return PropertyService.Get("DebuggingOptions", new DebuggingOptions());
			}
		}
		
		public bool ICorDebugVisualizerEnabled;
		public bool ShowValuesInHexadecimal;
		public bool ShowArgumentNames;
		public bool ShowArgumentValues;
		public bool ShowExternalMethods;
		
		// Properties for the DebuggerExceptionForm
		public FormWindowState DebuggerEventWindowState = FormWindowState.Normal;
		//public Size DebuggerEventWindowSize = new Size(646, 235);
		
		// Properties for the DebuggeeExceptionForm
		public FormWindowState DebuggeeExceptionWindowState = FormWindowState.Normal;
        //public Size DebuggeeExceptionWindowSize = new Size(646,431);
        //public int DebugeeExceptionSplitterDistance = 163;
        //public bool ShowExceptionDetails;
	}
}
