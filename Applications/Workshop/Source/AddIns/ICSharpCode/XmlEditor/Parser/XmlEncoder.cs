﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Text;
using System.Xml;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Utility class that will encode special XML characters.
	/// </summary>
	public static class XmlEncoder
	{
		/// <summary>
		/// Encodes any special characters in the xml string.
		/// </summary>
		public static string Encode(string xml, char quoteCharacter)
		{
			XmlEncoderTextWriter encoderWriter = new XmlEncoderTextWriter();
			using (XmlTextWriter writer = new XmlTextWriter(encoderWriter)) {
				writer.WriteStartElement("root");
				writer.WriteStartAttribute("attribute");
				writer.QuoteChar = quoteCharacter;
				
				encoderWriter.BeginMarkup();
				writer.WriteString(xml);
				return encoderWriter.Markup;
			}
		}
		
		/// <summary>
		/// Special XmlWriter that will return the last item written to
		/// it from a certain point. This is used by the XmlEncoder to
		/// get the encoded attribute string so the XmlEncoder does not
		/// have to do the special character encoding itself, but can
		/// use the .NET framework to do the work.
		/// </summary>
		private sealed class XmlEncoderTextWriter : EncodedStringWriter
		{
			StringBuilder markup = new StringBuilder();
			
			public XmlEncoderTextWriter() : base(Encoding.UTF8)
			{
			}
			
			/// <summary>
			/// Sets the point from which we are interested in 
			/// saving the string written to the text writer.
			/// </summary>
			public void BeginMarkup()
			{
				markup = new StringBuilder();
			}
			
			public void EndMarkup()
			{
				BeginMarkup();
			}
			
			/// <summary>
			/// Returns the string written to this text writer after the
			/// BeginMarkup method was called.
			/// </summary>
			public string Markup {
				get {
					return markup.ToString();
				}
			}
			
			public override void Write(string text)
			{
				base.Write(text);
				markup.Append(text);
			}
			
			public override void Write(char value)
			{
				base.Write(value);
				markup.Append(value);
			}
		}
	}
}
