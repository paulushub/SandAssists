// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Collections.Generic;

namespace ICSharpCode.Core.WinForms
{
	public static class IconService
	{
		private static Dictionary<string, string> extensionHashtable   = 
            new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
        private static Dictionary<string, string> projectFileHashtable =
            new Dictionary<string, string>(StringComparer.CurrentCultureIgnoreCase);
		
		readonly static char[] separators = {Path.DirectorySeparatorChar, Path.VolumeSeparatorChar};
		
		static IconService()
		{
			try 
            {
				InitializeIcons(AddInTree.GetTreeNode("/Workspace/Icons"));
			} 
            catch (TreePathNotFoundException) 
            {				
			}
		}
		
		public static Bitmap GetGhostBitmap(string name)
		{
			return GetGhostBitmap(GetBitmap(name));
		}
		
		public static Bitmap GetGhostBitmap(Bitmap bitmap)
		{
			ColorMatrix clrMatrix = new ColorMatrix(new float[][] {
			                                        	new float[] {1, 0, 0, 0, 0},
			                                        	new float[] {0, 1, 0, 0, 0},
			                                        	new float[] {0, 0, 1, 0, 0},
			                                        	new float[] {0, 0, 0, 0.5f, 0},
			                                        	new float[] {0, 0, 0, 0, 1}
			                                        });
			
			ImageAttributes imgAttributes = new ImageAttributes();
			imgAttributes.SetColorMatrix(clrMatrix,
			                             ColorMatrixFlag.Default,
			                             ColorAdjustType.Bitmap);
			
			Bitmap ghostBitmap = new Bitmap(bitmap.Width, 
                bitmap.Height, PixelFormat.Format32bppArgb);
			
			using (Graphics g = Graphics.FromImage(ghostBitmap)) 
            {
				g.FillRectangle(SystemBrushes.Window, 
                    new Rectangle(0, 0, bitmap.Width, bitmap.Height));
				g.DrawImage(bitmap, new 
                    Rectangle(0, 0, bitmap.Width, bitmap.Height), 
                    0, 0,bitmap.Width, bitmap.Height, GraphicsUnit.Pixel, imgAttributes);
			}
			
			return ghostBitmap;
		}
		
		public static Bitmap GetBitmap(string name)
		{
			Bitmap bmp;
			try 
            {
				bmp = WinFormsResourceService.GetBitmap(name);
			} 
            catch (ResourceNotFoundException ex) 
            {
				LoggingService.Warn(ex);
				bmp = null;
			}
			if (bmp != null) 
            {
				return bmp;
			}
			
			return WinFormsResourceService.GetBitmap("Icons.16x16.MiscFiles");
		}
		
		public static Icon GetIcon(string name)
		{
			Icon icon = WinFormsResourceService.GetIcon(name);
			if (icon != null) 
            {
				return icon;
			}

			return WinFormsResourceService.GetIcon("Icons.16x16.MiscFiles");
		}
		
		
		public static string GetImageForProjectType(string projectType)
		{
			if (projectFileHashtable.ContainsKey(projectType)) 
            {
				return projectFileHashtable[projectType];
			}
			return "Icons.16x16.SolutionIcon";
		}
		
		public static string GetImageForFile(string fileName)
		{
			string extension = Path.GetExtension(fileName);
			if (String.IsNullOrEmpty(extension)) 
                extension = ".txt";
			if (extensionHashtable.ContainsKey(extension)) 
            {
				return extensionHashtable[extension];
			}
			return "Icons.16x16.MiscFiles";
		}
		
		private static void InitializeIcons(AddInTreeNode treeNode)
		{
			extensionHashtable[".prjx"] = "Icons.16x16.SolutionIcon";
			
			extensionHashtable[".cmbx"] = "Icons.16x16.CombineIcon";
			extensionHashtable[".sln"]  = "Icons.16x16.CombineIcon";
			
			List<IconDescriptor> icons = treeNode.BuildChildItems<IconDescriptor>(null);
            int itemCount = icons.Count;
			for (int i = 0; i < itemCount; ++i) 
            {
				IconDescriptor iconCodon = icons[i];
				string imageName = iconCodon.Resource != null ? 
                    iconCodon.Resource : iconCodon.Id;
				
				if (iconCodon.Extensions != null) 
                {
					foreach (string ext in iconCodon.Extensions) 
                    {
						extensionHashtable[ext] = imageName;
					}
				}
				
				if (iconCodon.Language != null) 
                {
					projectFileHashtable[iconCodon.Language] = imageName;
				}
			}
		}
	}
}
