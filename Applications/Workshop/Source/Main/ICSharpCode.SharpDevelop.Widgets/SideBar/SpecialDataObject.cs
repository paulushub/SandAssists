// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3082 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Widgets.SideBar
{
    public class SpecialDataObject : System.Windows.Forms.IDataObject
    {
        List<object> dataObjects = new List<object>();
        public object GetData(string format)
        {
            return GetData(format, true);
        }

        public object GetData(System.Type format)
        {
            foreach (object o in dataObjects)
            {
                if (o.GetType() == format)
                {
                    return o;
                }
            }
            return null;
        }

        public object GetData(string str, bool autoConvert)
        {
            foreach (object o in dataObjects)
            {
                if (o == null)
                {
                    continue;
                }
                Type type = o.GetType();
                string typeStr = type.ToString();
                if (typeStr == str)
                {
                    return o;
                }

                if (typeStr == "ICSharpCode.SharpDevelop.Gui.SharpDevelopSideTabItem" 
                    && str == "TimeSprint.Alexandria.UI.SideBar.AxSideTabItem")
                {
                    return o;
                }

                if (type.BaseType != null)
                {
                    typeStr = type.BaseType.ToString();
                    if (typeStr == str)
                    {
                        return o;
                    }
                }
            }
            return null;
        }

        public bool GetDataPresent(string format)
        {
            return GetDataPresent(format, true);
        }

        public bool GetDataPresent(System.Type format)
        {
            return GetData(format) != null;
        }

        public bool GetDataPresent(string format, bool autoConvert)
        {
            return GetData(format, autoConvert) != null;
        }

        public string[] GetFormats()
        {
            return new string[0];
        }
        public string[] GetFormats(bool autoConvert)
        {
            return new string[0];
        }

        public void SetData(object data)
        {
            dataObjects.Add(data);
        }

        public void SetData(string format, object data)
        {

        }
        public void SetData(System.Type format, object data)
        {

        }
        public void SetData(string format, bool autoConvert, object data)
        {

        }
    }
}
