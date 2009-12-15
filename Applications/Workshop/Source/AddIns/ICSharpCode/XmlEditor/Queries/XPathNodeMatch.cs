﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1662 $</version>
// </file>

using System;
using System.Xml;
using System.Xml.XPath;

namespace ICSharpCode.XmlEditor
{
	/// <summary>
	/// Stores an XmlNode and its associated line number and position after an 
	/// XPath query has been evaluated.
	/// </summary>
	public class XPathNodeMatch : IXmlLineInfo
	{
		int? lineNumber;
		int linePosition;
		string value;
		string displayValue;
		XPathNodeType nodeType;
		
		/// <summary>
		/// Creates an XPathNodeMatch from the navigator which should be position on the
		/// node.
		/// </summary>
		/// <remarks>
		/// We deliberately use the OuterXml when we find a Namespace since the
		/// navigator location returned starts from the xmlns attribute.
		/// </remarks>
		public XPathNodeMatch(XPathNavigator currentNavigator)
		{
			SetLineNumbers(currentNavigator as IXmlLineInfo);
			nodeType = currentNavigator.NodeType;
			switch (nodeType) {
				case XPathNodeType.Text:
					SetTextValue(currentNavigator);
					break;
				case XPathNodeType.Comment:
					SetCommentValue(currentNavigator);
					break;
				case XPathNodeType.Namespace:
					SetNamespaceValue(currentNavigator);
					break;
				case XPathNodeType.Element:
					SetElementValue(currentNavigator);
					break;
				case XPathNodeType.ProcessingInstruction:
					SetProcessingInstructionValue(currentNavigator);
					break;
				case XPathNodeType.Attribute:
					SetAttributeValue(currentNavigator);
					break;
				default:
					value = currentNavigator.LocalName;
					displayValue = value;
					break;
			}
		}
		
		/// <summary>
		/// Line numbers are zero based.
		/// </summary>
		public int LineNumber {
			get {
				return lineNumber.GetValueOrDefault(0);
			}
		}
		
		/// <summary>
		/// Line positions are zero based.
		/// </summary>
		public int LinePosition {
			get {
				return linePosition;
			}
		}
		
		public bool HasLineInfo()
		{
			return lineNumber.HasValue;
		}
		
		/// <summary>
		/// Gets the text value of the node.
		/// </summary>
		public string Value {
			get {
				return value;
			}
		}
		
		/// <summary>
		/// Gets the node display value. This includes the angle brackets if it is
		/// an element, for example.
		/// </summary>
		public string DisplayValue {
			get {
				return displayValue;
			}
		}
		
		public XPathNodeType NodeType {
			get {
				return nodeType;
			}
		}
		
		void SetElementValue(XPathNavigator navigator)
		{
			value = navigator.Name;
			if (navigator.IsEmptyElement) {
				displayValue = String.Concat("<", value, "/>");
			} else {
				displayValue = String.Concat("<", value, ">");
			}
		}
		
		void SetTextValue(XPathNavigator navigator)
		{
			value = navigator.Value;
			displayValue = value;
		}
		
		void SetCommentValue(XPathNavigator navigator)
		{
			value = navigator.Value;
			displayValue = navigator.OuterXml;
		}
		
		void SetNamespaceValue(XPathNavigator navigator)
		{
			value = navigator.OuterXml;
			displayValue = value;
		}
		
		void SetProcessingInstructionValue(XPathNavigator navigator)
		{
			value = navigator.Name;
			displayValue = navigator.OuterXml;
		}
		
		void SetAttributeValue(XPathNavigator navigator)
		{
			value = navigator.Name;
			displayValue = String.Concat("@", value);
		}
		
		/// <summary>
		/// Takes one of the xml line number so the numbers are now zero
		/// based instead of one based.
		/// </summary>
		/// <remarks>A namespace query (e.g. //namespace::*) will return
		/// a line info of -1, -1 for the xml namespace. Which looks like
		/// a bug in the XPathDocument class.</remarks>
		void SetLineNumbers(IXmlLineInfo lineInfo)
		{
			if (lineInfo.HasLineInfo() && lineInfo.LineNumber > 0) {
				lineNumber = lineInfo.LineNumber - 1;
				linePosition = lineInfo.LinePosition - 1;
			}
		}
	}
}
