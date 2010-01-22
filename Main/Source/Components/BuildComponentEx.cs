using System;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    public abstract class BuildComponentEx : BuildComponent
    {
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

        protected void WriteMessage(MessageLevel level, Exception ex)
        {
            string message = ex.Message;
            if (message == null)
            {
                message = ex.ToString();
            }
            this.WriteMessage(level,  message);
            //this.WriteMessage(level, String.Format("Exception({0}) - {1}",
            //    ex.GetType().FullName, message));
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
