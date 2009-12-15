using System;
using System.IO;

using ICSharpCode.XmlEditor;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace Sandcastle.Workshop.Bindings
{
	/// <summary>
	/// Display binding for the MAML editor.
	/// </summary>
	public sealed class MamlDisplayBinding : IDisplayBinding
	{
		public IViewContent CreateContentForFile(OpenedFile file)
		{
            if (file == null)
            {
                return null;
            }
            if (String.Equals(Path.GetExtension(file.FileName), 
                ".cmp", StringComparison.OrdinalIgnoreCase))
            {
                return new MetadataDesignViewContent(file);
            }

            return new MamlTextViewContent(file, false);
		}
		
		/// <summary>
		/// Can only create content for file with extensions that are
		/// known to be xml files as specified in the SyntaxModes.xml file.
		/// </summary>
		public bool CanCreateContentForFile(string fileName)
		{
            if (String.IsNullOrEmpty(fileName))
            {
                return false;
            }
            string fileExt = Path.GetExtension(fileName);
            if (fileExt.Equals(".aml", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (fileExt.Equals(".maml", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
            if (fileExt.Equals(".cmp", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
		}
	}
}
