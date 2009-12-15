using System;
using System.Text;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.XmlEditor
{
    public sealed class XPathQueryPadCommand : AbstractCheckableMenuCommand
    {
        public override bool IsChecked
        {
            get
            {
                XPathQueryPad pad = XPathQueryPad.Instance;
                if (pad != null)
                {
                    return pad.IsVisible;
                }

                return false;
            }
        }

        public override void Run()
        {
            XPathQueryPad pad = XPathQueryPad.Instance;
            if (pad != null)
            {
                pad.BringToFront();
            }
            else
            {
                foreach (PadDescriptor padContent in WorkbenchSingleton.Workbench.PadContentCollection)
                {
                    if (String.Equals(padContent.Category, "Tools", 
                        StringComparison.OrdinalIgnoreCase) &&
                        String.Equals(padContent.Class, "ICSharpCode.XmlEditor.XPathQueryPad", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        padContent.BringPadToFront();
                        break;
                    }
                }
            }

            base.Run();
        }
    }
}
