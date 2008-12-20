//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
using System;
using System.Xml;
using System.Xml.XPath;

namespace Sandcastle.HelpRegister
{
	public static class XmlHelper
	{
		public static bool SetXmlStringValue(XPathNavigator parentNode, 
            string valueName, string newValue)
		{
			if (parentNode == null || String.IsNullOrEmpty(valueName))
			{
				return false;
			}
			try
			{
				XPathNavigator factory = parentNode.Clone();
				do
				{
					factory.MoveToFirstAttribute();
					
					if (String.Compare(factory.Name, valueName) == 0)
					{
						factory.SetValue(newValue);
						return true;
					}
				}
				while (!factory.MoveToNextAttribute());

				parentNode.CreateAttribute(String.Empty, valueName, 
                    String.Empty, newValue);

				return true;
			}
			catch (ArgumentNullException)
			{
			}
			catch (InvalidOperationException)
			{
			}

			return false;
		}

		public static string GetXmlStringValue(XPathNavigator parentNode, 
            string valueName)
		{
			if (parentNode == null || String.IsNullOrEmpty(valueName))
			{
				return String.Empty;
			}
			try
			{
				return parentNode.GetAttribute(valueName, String.Empty);
			}
			catch (NullReferenceException)
			{
                return String.Empty;
			}
		}

		public static int GetXmlIntValue(XPathNavigator parentNode, 
            string valueName, int defaultValue)
		{
			if (parentNode == null || String.IsNullOrEmpty(valueName))
			{
				return defaultValue;
			}
			try
			{
				string nodeValue = parentNode.GetAttribute(valueName, String.Empty);
                if (String.IsNullOrEmpty(nodeValue))
                {
                    return defaultValue;
                }

				return Convert.ToInt32(nodeValue);
			}
			catch (NullReferenceException)
			{
                return defaultValue;
			}
			catch (FormatException)
			{
                return defaultValue;
			}
		}

		public static bool GetXmlBoolValue(XPathNavigator parentNode, 
            string valueName)
		{
			return GetXmlBoolValue(parentNode, valueName, false);
		}

		public static bool GetXmlBoolValue(XPathNavigator parentNode, 
            string valueName, bool emptyIsTrue)
		{
			if (parentNode == null || String.IsNullOrEmpty(valueName))
			{
				return false;
			}
			bool result = false;

			try
			{
				string nodeValue = parentNode.GetAttribute(valueName, String.Empty);
				if (emptyIsTrue)
				{
                    result = (String.IsNullOrEmpty(nodeValue) || 
                        (nodeValue != null && nodeValue == "yes"));
				}
				else
				{
                    result = (nodeValue != null && nodeValue == "yes");
				}
			}
			catch (NullReferenceException)
			{
			}

			return result;
		}
	}
}
