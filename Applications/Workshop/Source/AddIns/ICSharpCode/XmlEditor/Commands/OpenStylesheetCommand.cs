// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Opens the stylesheet associated with the active XML document.
	/// </summary>
    public sealed class OpenStylesheetCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			XmlView xmlView = XmlView.ActiveXmlView;
			if (xmlView != null) {
				if (xmlView.StylesheetFileName != null) {
					try {
						FileService.OpenFile(xmlView.StylesheetFileName);
					} catch (Exception ex) {
						MessageService.ShowError(ex);
					}
				}
			}
		}
	}
}
