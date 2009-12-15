// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Represents an association between an xml schema and a file extension.
	/// </summary>
	public class XmlSchemaAssociation //: IXmlConvertable
	{
		string namespaceUri = String.Empty;
		string extension = String.Empty;
		string namespacePrefix = String.Empty;
		
		public XmlSchemaAssociation(string extension)
			: this(extension, String.Empty, String.Empty)
		{
		}
		
		public XmlSchemaAssociation(string extension, string namespaceUri)
			: this(extension, namespaceUri, String.Empty)
		{
		}
		
		public XmlSchemaAssociation(string extension, string namespaceUri, string namespacePrefix)
		{
			this.extension = extension;
			this.namespaceUri = namespaceUri;
			this.namespacePrefix = namespacePrefix;
		}
		
		public string NamespaceUri {
			get {
				return namespaceUri;
			}
			
			set {
				namespaceUri = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the file extension (e.g. '.xml').
		/// </summary>
		public string Extension {
			get {
				return extension;
			}
			
			set {
				extension = value;
			}
		}
		
		/// <summary>
		/// Gets or sets the default namespace prefix that will be added
		/// to the xml elements.
		/// </summary>
		public string NamespacePrefix {
			get {
				return namespacePrefix;
			}
			
			set {
				namespacePrefix = value;
			}
		}		
		
		/// <summary>
		/// Gets the default schema association for the file extension. 
		/// </summary>
		/// <remarks>
		/// These defaults are hard coded.
		/// </remarks>
		public static XmlSchemaAssociation GetDefaultAssociation(string extension)
		{
			XmlSchemaAssociation association = null;
			
			switch (extension.ToLowerInvariant()) {
				case ".wxs":
					association = new XmlSchemaAssociation(extension, @"http://schemas.microsoft.com/wix/2003/01/wi");
					break;
				case ".config":
					association = new XmlSchemaAssociation(extension, @"urn:app-config");
					break;
				case ".build":
					association = new XmlSchemaAssociation(extension, @"http://nant.sf.net/release/0.85/nant.xsd");
					break;
				case ".addin":
					association = new XmlSchemaAssociation(extension, @"http://www.icsharpcode.net/2005/addin");
					break;
				case ".xsl":
				case ".xslt":
					association = new XmlSchemaAssociation(extension, @"http://www.w3.org/1999/XSL/Transform", "xsl");
					break;
				case ".xsd":
					association = new XmlSchemaAssociation(extension, @"http://www.w3.org/2001/XMLSchema", "xs");
					break;
				case ".manifest":
					association = new XmlSchemaAssociation(extension, @"urn:schemas-microsoft-com:asm.v1");
					break;					
				case ".xaml":
					association = new XmlSchemaAssociation(extension, @"http://schemas.microsoft.com/winfx/avalon/2005");
					break;
				default:
					association = new XmlSchemaAssociation(extension);
					break;
			}
			return association;
		}
		
		/// <summary>
		/// Two schema associations are considered equal if their file extension,
		/// prefix and namespaceUri are the same.
		/// </summary>
		public override bool Equals(object obj)
		{
			bool equals = false;
			
			XmlSchemaAssociation rhs = obj as XmlSchemaAssociation;
			if (rhs != null) {
				if ((this.namespacePrefix == rhs.namespacePrefix) &&
				    (this.extension == rhs.extension) &&
				    (this.namespaceUri == rhs.namespaceUri)) {
					equals = true;
				}
			}
						
			return equals;
		}
		
		public override int GetHashCode()
		{
			return (namespaceUri != null ? namespaceUri.GetHashCode() : 0) ^ (extension != null ? extension.GetHashCode() : 0) ^ (namespacePrefix != null ? namespacePrefix.GetHashCode() : 0);
		}
		
		/// <summary>
		/// Creates an XmlSchemaAssociation from the saved xml.
		/// </summary>
		public static XmlSchemaAssociation ConvertFromString(string text)
		{
			string[] parts = text.Split(new char[] {'|'}, 3);
			return new XmlSchemaAssociation(parts[0], parts[1], parts[2]);
		}
		
		public string ConvertToString()
		{	
			return extension + "|" + namespaceUri + "|" + namespacePrefix;
		}
	}
}
