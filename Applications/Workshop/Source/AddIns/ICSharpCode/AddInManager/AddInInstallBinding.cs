// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2313 $</version>
// </file>

using System;

using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.AddInManager
{
    public class AddInInstallBinding : IDisplayBinding
	{
		public bool CanCreateContentForFile(string fileName)
		{
			return true;
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
            ManagerForm dlg = new ManagerForm();
            dlg.BeginInstall(new string[] { file.FileName });
            dlg.ShowDialog();

			//ManagerForm.Instance.ShowInstallableAddIns(new string[] { file.FileName });

			return null;
		}
	}
}
