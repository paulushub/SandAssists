// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Pretty prints the xml.
	/// </summary>
    public sealed class FormatXmlCommand : AbstractMenuCommand
	{
		public override void Run()
		{
			// Find active XmlView.
			XmlView xmlView = XmlView.ActiveXmlView;
			if (xmlView != null) {
				xmlView.FormatXml();
			}
		}
	}
}
