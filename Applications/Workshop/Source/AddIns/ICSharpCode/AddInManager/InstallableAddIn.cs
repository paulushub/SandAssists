// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.IO;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpZipLib.Zip;
using AddInManagerEx = ICSharpCode.Core.AddInManager;

namespace ICSharpCode.AddInManager
{
	public class InstallableAddIn
	{
		string fileName;
		bool isPackage;
		AddIn addIn;
		
		public AddIn AddIn {
			get {
				return addIn;
			}
		}
		
		public InstallableAddIn(string fileName, bool isPackage)
		{
			this.fileName = fileName;
			this.isPackage = isPackage;
			if (isPackage) {
				ZipFile file = new ZipFile(fileName);
				try {
					LoadAddInFromZip(file);
				} finally {
					file.Close();
				}
			} else {
                addIn = AddInLoader.Load(fileName);
			}
			if (addIn.Manifest.PrimaryIdentity == null)
				throw new AddInLoadException(ResourceService.GetString("AddInManager.AddInMustHaveIdentity"));
		}
		
		void LoadAddInFromZip(ZipFile file)
		{
			ZipEntry addInEntry = null;
			foreach (ZipEntry entry in file) {
				if (entry.Name.EndsWith(".addin")) {
					if (addInEntry != null)
						throw new AddInLoadException("The package may only contain one .addin file.");
					addInEntry = entry;
				}
			}
			if (addInEntry == null)
				throw new AddInLoadException("The package must contain one .addin file.");
			using (Stream s = file.GetInputStream(addInEntry)) {
				using (StreamReader r = new StreamReader(s)) {
                    addIn = AddInLoader.Load(r);
				}
			}
		}
		
		public void Install(bool isUpdate)
		{
			foreach (string identity in addIn.Manifest.Identities.Keys) {
				AddInManagerEx.AbortRemoveUserAddInOnNextStart(identity);
			}
			if (isPackage) {
				string targetDir = Path.Combine(AddInManagerEx.AddInInstallTemp,
				                                addIn.Manifest.PrimaryIdentity);
				if (Directory.Exists(targetDir))
					Directory.Delete(targetDir, true);
				Directory.CreateDirectory(targetDir);
				FastZip fastZip = new FastZip();
				fastZip.CreateEmptyDirectories = true;
				fastZip.ExtractZip(fileName, targetDir, null);
				
				addIn.Action = AddInAction.Install;
				if (!isUpdate) {
					AddInTree.InsertAddIn(addIn);
				}
			} else {
				AddInManagerEx.AddExternalAddIns(new AddIn[] { addIn });
			}
		}
		
		public static void CancelUpdate(IList<AddIn> addIns)
		{
			foreach (AddIn addIn in addIns) {
				foreach (string identity in addIn.Manifest.Identities.Keys) {
					// delete from installation temp (if installation or update is pending)
					string targetDir = Path.Combine(AddInManagerEx.AddInInstallTemp,
					                                identity);
					if (Directory.Exists(targetDir))
						Directory.Delete(targetDir, true);
				}
			}
		}
		
		public static void Uninstall(IList<AddIn> addIns)
		{
			CancelUpdate(addIns);
			foreach (AddIn addIn in addIns) {
				foreach (string identity in addIn.Manifest.Identities.Keys) {
					// remove the user AddIn
					string targetDir = Path.Combine(AddInManagerEx.UserAddInPath, identity);
					if (Directory.Exists(targetDir)) {
						if (!addIn.Enabled) {
							try {
								Directory.Delete(targetDir, true);
								continue;
							} catch {
							}
						}
						AddInManagerEx.RemoveUserAddInOnNextStart(identity);
					}
				}
			}
		}
	}
}
