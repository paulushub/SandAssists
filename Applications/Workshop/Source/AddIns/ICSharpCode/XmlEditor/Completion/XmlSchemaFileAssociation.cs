// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 5215 $</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.XmlEditor
{
	public sealed class XmlSchemaFileAssociation : IEquatable<XmlSchemaFileAssociation>
	{
		private string namespaceUri;
        private string fileExtension;
        private string namespacePrefix;
		
		public XmlSchemaFileAssociation(string fileExtension, string namespaceUri)
			: this(fileExtension, namespaceUri, String.Empty)
		{
		}
		
		public XmlSchemaFileAssociation(string fileExtension, string namespaceUri, 
            string namespacePrefix)
		{
			this.fileExtension   = fileExtension.ToLowerInvariant();
			this.namespaceUri    = namespaceUri;
			this.namespacePrefix = namespacePrefix;
		}
		
		public string NamespaceUri {
			get { return namespaceUri; }
		}
		
		public string FileExtension {
			get { return fileExtension; }
		}
		
		/// <summary>
		/// Gets or sets the default namespace prefix that will be added
		/// to the xml elements.
		/// </summary>
		public string NamespacePrefix {
			get { return namespacePrefix; }
		}

		public bool IsEmpty {
			get { 
				return String.IsNullOrEmpty(fileExtension) &&
					String.IsNullOrEmpty(namespaceUri) &&
					String.IsNullOrEmpty(namespacePrefix);
			}
		}

        #region IEquatable<XmlSchemaFileAssociation> Members

        public bool Equals(XmlSchemaFileAssociation other)
        {
            if (other == null)
            {
                return false;
            }

            return (this.namespacePrefix == other.NamespacePrefix) &&
                (this.fileExtension == other.fileExtension) &&
                (this.namespaceUri == other.namespaceUri);
        }
				
		/// <summary>
		/// Two schema associations are considered equal if their file extension,
		/// prefix and namespaceUri are the same.
		/// </summary>
		public override bool Equals(object obj)
		{
			XmlSchemaFileAssociation rhs = obj as XmlSchemaFileAssociation;
			if (rhs != null) {
				return (this.namespacePrefix == rhs.NamespacePrefix) && 
				    (this.fileExtension == rhs.fileExtension) &&
					(this.namespaceUri == rhs.namespaceUri);
			}
			return false;
		}
		
		public override int GetHashCode()
		{
			return (namespaceUri != null ? namespaceUri.GetHashCode() : 0) ^ 
				(fileExtension != null ? fileExtension.GetHashCode() : 0) ^ 
				(namespacePrefix != null ? namespacePrefix.GetHashCode() : 0);
        }

        #endregion

        /// <summary>
		/// Converts from a string such as "file-extension|schema-namespace|schema-xml-prefix" to an 
		/// XmlSchemaAssociation.
		/// </summary>
		public static XmlSchemaFileAssociation ConvertFromString(string text)
		{
			const int totalParts = 3;
			string[] parts = text.Split(new char[] {'|'}, totalParts);
			if(parts.Length == totalParts) {
				return new XmlSchemaFileAssociation(parts[0], parts[1], parts[2]);
			}
			return new XmlSchemaFileAssociation(String.Empty, String.Empty);
		}
		
		public override string ToString()
		{
			return fileExtension + "|" + namespaceUri + "|" + namespacePrefix;
		}
    }
}
