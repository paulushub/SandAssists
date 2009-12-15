// <file>
//     <copyright license="BSD-new" see="prj:///COPYING"/>
//     <owner name="David Srbeck�" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 3648 $</version>
// </file>

namespace Debugger.AddIn.TreeModel
{
	public interface ISetText
	{
		bool CanSetText { get; }
		
		bool SetText(string text);
	}
}
