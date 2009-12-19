// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 5324 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace ICSharpCode.XmlEditor
{
	public sealed class XmlNamespaceCollection : Collection<XmlNamespace>
	{
		public XmlNamespaceCollection()
		{
		}
		
		public string GetNamespaceForPrefix(string prefix)
		{
			foreach (XmlNamespace ns in this) {
				if (ns.Prefix == prefix) {
					return ns.Name;
				}
			}
			return String.Empty;
		}
		
		public string GetPrefix(string namespaceToMatch)
		{
			foreach (XmlNamespace ns in this) {
				if (ns.Name == namespaceToMatch)  {
					return ns.Prefix;
				}
			}
			return String.Empty;
		}
	}
}
