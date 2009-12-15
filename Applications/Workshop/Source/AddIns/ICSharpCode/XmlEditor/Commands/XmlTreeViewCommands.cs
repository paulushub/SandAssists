// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.XmlEditor
{
    /// <summary>
    /// This is the <see langword="abstract"/> base class for all XML tree view
    /// operation commands.
    /// </summary>
    public abstract class XmlTreeViewCommand : AbstractMenuCommand
    {
        protected XmlTreeViewContainerControl _control;

        protected XmlTreeViewCommand()
        {   
        }

        public XmlTreeViewContainerControl Control
        {
            get
            {
                return _control;
            }
        }

        protected override void OnOwnerChanged(EventArgs e)
        {
            base.OnOwnerChanged(e);

            MenuCommand menuItem = this.Owner as MenuCommand;            
            if (menuItem != null)
            {
                _control = menuItem.Caller as XmlTreeViewContainerControl;
                return;
            }

            ToolBarCommand toolItem = this.Owner as ToolBarCommand;
            if (toolItem != null)
            {
                _control = toolItem.Caller as XmlTreeViewContainerControl;
                return;
            }
        }
    }

    /// <summary>
    /// Adds a new attribute to the XML Tree's attribute property grid.
    /// </summary>
    public sealed class AddAttributeCommand : XmlTreeViewCommand
    {
        public override void Run()
        {
            if (_control != null)
            {
                _control.AddAttribute();
            }
        }
    }

    /// <summary>
    /// Adds a new comment to the selected element.
    /// </summary>
    public sealed class AddChildCommentCommand : XmlTreeViewCommand
    {
        public override void Run()
        {
            if (_control != null)
            {
                _control.AppendChildComment();
            }
        }
    }

    /// <summary>
    /// Adds a new child element to the XML Tree.
    /// </summary>
    public sealed class AddChildElementCommand : XmlTreeViewCommand
    {
        public override void Run()
        {
            if (_control != null)
            {
                _control.AddChildElement();
            }
        }
    }

    /// <summary>
    /// Adds a new text node to selected element.
    /// </summary>
    public sealed class AddChildTextNodeCommand : XmlTreeViewCommand
    {
        public override void Run()
        {
            if (_control != null)
            {
                _control.AppendChildTextNode();
            }
        }
    }

    /// <summary>
    /// Inserts a new comment node after the selected node.
    /// </summary>
    public sealed class InsertCommentAfterCommand : XmlTreeViewCommand
    {
        public override void Run()
        {
            if (_control != null)
            {
                _control.InsertCommentAfter();
            }
        }
    }

    /// <summary>
    /// Inserts a new comment node before the selected node.
    /// </summary>
    public sealed class InsertCommentBeforeCommand : XmlTreeViewCommand
    {
        public override void Run()
        {
            if (_control != null)
            {
                _control.InsertCommentBefore();
            }
        }
    }

    /// <summary>
    /// Inserts a child element after the selected element in the XML tree.
    /// </summary>
    public sealed class InsertElementAfterCommand : XmlTreeViewCommand
    {
        public override void Run()
        {
            if (_control != null)
            {
                _control.InsertElementAfter();
            }
        }
    }

    /// <summary>
    /// Inserts a child element before the selected element in the XML tree.
    /// </summary>
    public sealed class InsertElementBeforeCommand : XmlTreeViewCommand
    {
        public override void Run()
        {
            if (_control != null)
            {
                _control.InsertElementBefore();
            }
        }
    }

    /// <summary>
    /// Inserts a new text node to after the selected node.
    /// </summary>
    public sealed class InsertTextNodeAfterCommand : XmlTreeViewCommand
    {
        public override void Run()
        {
            if (_control != null)
            {
                _control.InsertTextNodeAfter();
            }
        }
    }

    /// <summary>
    /// Inserts a new text node to before the selected node.
    /// </summary>
    public sealed class InsertTextNodeBeforeCommand : XmlTreeViewCommand
    {
        public override void Run()
        {
            if (_control != null)
            {
                _control.InsertTextNodeBefore();
            }
        }
    }

    /// <summary>
    /// Removes the selected attribute from the xml document being 
    /// displayed in the XML tree.
    /// </summary>
    public sealed class RemoveAttributeCommand : XmlTreeViewCommand
    {
        public override void Run()
        {
            if (_control != null)
            {
                _control.RemoveAttribute();
            }
        }
    }
}
