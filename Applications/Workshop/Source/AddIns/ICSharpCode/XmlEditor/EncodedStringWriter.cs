// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 915 $</version>
// </file>

using System;
using System.IO;
using System.Text;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// A string writer that allows you to specify the text encoding to
	/// be used when generating the string.
	/// </summary>
	/// <remarks>
	/// This class is used when generating xml strings using a writer and
	/// the encoding in the xml processing instruction needs to be changed.
	/// The xml encoding string will be the encoding specified in the constructor 
	/// of this class (i.e. UTF-8, UTF-16)</remarks>
	public class EncodedStringWriter : StringWriter
	{
		Encoding encoding = Encoding.UTF8;
		
		/// <summary>
		/// Creates a new string writer that will generate a string with the
		/// specified encoding.  
		/// </summary>
		/// <remarks>The encoding will be used when generating the 
		/// xml encoding header (i.e. UTF-8, UTF-16).</remarks>
		public EncodedStringWriter(Encoding encoding)
		{
			this.encoding = encoding;
		}
		
		/// <summary>
		/// Gets the text encoding that will be used when generating
		/// the string.
		/// </summary>
		public override Encoding Encoding {
			get {
				return encoding;
			}
		}
	}
}
