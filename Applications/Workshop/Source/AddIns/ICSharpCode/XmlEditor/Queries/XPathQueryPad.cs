// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 2563 $</version>
// </file>

using System;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
	public class XPathQueryPad : AbstractPadContent
	{
		public const string XPathQueryControlProperties = "XPathQueryControl.Options";
		
		XPathQueryControl xPathQueryControl;
		static XPathQueryPad instance;
		
		public XPathQueryPad()
		{
			xPathQueryControl = new XPathQueryControl();
			WorkbenchSingleton.Workbench.ActiveViewContentChanged += ActiveViewContentChanged;
			Properties properties = PropertyService.Get(XPathQueryControlProperties, new Properties());
			xPathQueryControl.SetMemento(properties);
			instance = this;
		}
		
		public static XPathQueryPad Instance {
			get {
				return instance;
			}
		}
		
		/// <summary>
		/// The <see cref="System.Windows.Forms.Control"/> representing the pad.
		/// </summary>
		public override Control Control {
			get {
				return xPathQueryControl;
			}
		}
		
		public void RemoveXPathHighlighting()
		{
			xPathQueryControl.RemoveXPathNodeTextMarkers();
		}
		
		void ActiveViewContentChanged(object source, EventArgs e)
		{
			xPathQueryControl.ActiveWindowChanged();
		}

        protected override void Dispose(bool disposing)
        {
            if (!this.IsDisposed)
            {
                WorkbenchSingleton.Workbench.ActiveViewContentChanged -= ActiveViewContentChanged;
                Properties properties = xPathQueryControl.CreateMemento();
                PropertyService.Set(XPathQueryControlProperties, properties);

                xPathQueryControl.Dispose();
            }

            base.Dispose(disposing);
        }
	}
}
