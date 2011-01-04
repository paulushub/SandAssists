using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    /// <summary>
    /// This is the <see langword="abstract"/> base class for all Sandcastle Assist
    /// custom build components.
    /// </summary>                        
    public abstract class BuildComponentEx : BuildComponent
    {
        #region InteropServices Methods

        [DllImport("shell32.dll")]
        private static extern long FindExecutable(
          string lpFile, string lpDirectory, StringBuilder lpResult);

        #endregion

        #region Constructors and Destructor

        protected BuildComponentEx(BuildAssembler assembler,
            XPathNavigator configuration)
            : base(assembler, configuration)
        {
        }

        ~BuildComponentEx()
        {
            Dispose(false);
        }

        #endregion

        #region Public Methods

        #endregion

        #region Protected Methods

        protected static string SearchExecutable(string filename)
        {
            StringBuilder objResultBuffer = new StringBuilder(512);

            if (FindExecutable(filename, string.Empty, objResultBuffer) >= 32)
            {
                return objResultBuffer.ToString();
            }

            return null;
        }

        protected void WriteMessage(MessageLevel level, Exception ex)
        {
            string message = ex.Message;
            if (String.IsNullOrEmpty(message))
            {
                this.WriteMessage(level, ex.ToString());
            }
            else
            {
                this.WriteMessage(level, String.Format("Exception({0}) - {1}",
                    ex.GetType().Name, message));
            }
        }

        protected bool SetXmlStringValue(XPathNavigator navigator,
            string valueName, string newValue)
        {
            if (navigator == null || String.IsNullOrEmpty(valueName))
            {
                return false;
            }
            try
            {
                XPathNavigator factory = navigator.Clone();
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

                navigator.CreateAttribute(String.Empty, valueName,
                    String.Empty, newValue);

                return true;
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);

                return false;
            }
        }

        protected string GetXmlStringValue(XPathNavigator navigator,
            string valueName, string defaultValue)
        {
            string resultValue = defaultValue;
            if (navigator == null || String.IsNullOrEmpty(valueName))
            {
                return resultValue;
            }
            try
            {
                resultValue = navigator.GetAttribute(valueName, String.Empty);
            }
            catch (Exception ex)  // not likely!
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }

            return resultValue;
        }

        protected bool GetXmlBoolValue(XPathNavigator navigator,
            string valueName, bool defaultValue)
        {
            bool resultValue = defaultValue;
            if (navigator == null || String.IsNullOrEmpty(valueName))
            {
                return resultValue;
            }

            try
            {
                string nodeValue = navigator.GetAttribute(valueName, String.Empty);
                if (!String.IsNullOrEmpty(nodeValue))
                {
                    resultValue = Convert.ToBoolean(nodeValue);
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }

            return resultValue;
        }

        protected int GetXmlIntValue(XPathNavigator navigator,
            string valueName, int defaultValue)
        {
            int resultValue = defaultValue;
            if (navigator == null || String.IsNullOrEmpty(valueName))
            {
                return resultValue;
            }
            try
            {
                string nodeValue = navigator.GetAttribute(valueName, String.Empty);
                if (!String.IsNullOrEmpty(nodeValue))
                {
                    resultValue = Convert.ToInt32(nodeValue);
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }

            return resultValue;
        }

        protected float GetXmlSingleValue(XPathNavigator navigator,
            string valueName, float defaultValue)
        {
            float resultValue = defaultValue;
            if (navigator == null || String.IsNullOrEmpty(valueName))
            {
                return resultValue;
            }
            try
            {
                string nodeValue = navigator.GetAttribute(valueName, String.Empty);
                if (!String.IsNullOrEmpty(nodeValue))
                {
                    resultValue = Convert.ToSingle(nodeValue);
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }

            return resultValue;
        }

        protected double GetXmlDoubleValue(XPathNavigator navigator,
            string valueName, double defaultValue)
        {
            double resultValue = defaultValue;
            if (navigator == null || String.IsNullOrEmpty(valueName))
            {
                return resultValue;
            }
            try
            {
                string nodeValue = navigator.GetAttribute(valueName, String.Empty);
                if (!String.IsNullOrEmpty(nodeValue))
                {
                    resultValue = Convert.ToDouble(nodeValue);
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }

            return resultValue;
        }

        protected void WriteIncludeAttribute(XmlWriter writer,
            string name, string item, string parameter)
        {
            if (String.IsNullOrEmpty(name))
            {
                this.WriteMessage(MessageLevel.Warn, 
                    "The name of the include attribute cannot be null or empty.");
            }
            if (String.IsNullOrEmpty(item))
            {
                this.WriteMessage(MessageLevel.Warn,
                    "The item of the include attribute cannot be null or empty.");
            }

            writer.WriteStartElement("includeAttribute"); //start - includeAttribute
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("item", item);
            if (!String.IsNullOrEmpty(parameter))
            {   
                writer.WriteStartElement("parameter");    //start - parameter
                writer.WriteString(parameter);
                writer.WriteEndElement();                 // end - parameter
            }
            writer.WriteEndElement();                     // end - includeAttribute
        }

        protected void WriteIncludeAttributes(XmlWriter writer,
            string[] names, string[] items, string[] parameters)
        {
            if (names == null || items == null || parameters == null)
            {
                return;
            }
            int itemCount = names.Length;
            if (itemCount == 0 || items.Length == itemCount || 
                parameters.Length == itemCount)
            {
                return;
            }

            for (int i = 0; i < itemCount; i++)
            {
                this.WriteIncludeAttribute(writer, 
                    names[i], items[i], parameters[i]);
            }
        }

        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);

            base.Dispose();
        }

        protected virtual void Dispose(bool disposing)
        {   
        }

        #endregion
    }
}
