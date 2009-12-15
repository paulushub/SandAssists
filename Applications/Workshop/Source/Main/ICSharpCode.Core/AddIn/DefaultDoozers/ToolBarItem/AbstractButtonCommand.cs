// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="" email=""/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace ICSharpCode.Core
{
    public abstract class AbstractButtonCommand : AbstractCommand, IMenuCommand
    {         
        private bool isEnabled;

        protected AbstractButtonCommand()
        {
            isEnabled = true;
        }

        protected AbstractButtonCommand(bool enabled)
        {
            isEnabled = enabled;
        }

        public virtual bool IsEnabled
        {
            get
            {
                return isEnabled;
            }
            set
            {
                isEnabled = value;
            }
        }
    }
}
