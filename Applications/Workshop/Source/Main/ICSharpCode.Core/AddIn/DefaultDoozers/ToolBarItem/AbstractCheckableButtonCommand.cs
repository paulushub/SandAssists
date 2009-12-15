// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="" email=""/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace ICSharpCode.Core
{
    public abstract class AbstractCheckableButtonCommand : AbstractButtonCommand, ICheckableMenuCommand
    {
        bool isChecked;

        protected AbstractCheckableButtonCommand()
        {   
        }

        public virtual bool IsChecked
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
            }
        }

        public override void Run()
        {
            IsChecked = !IsChecked;
        }
    }
}
