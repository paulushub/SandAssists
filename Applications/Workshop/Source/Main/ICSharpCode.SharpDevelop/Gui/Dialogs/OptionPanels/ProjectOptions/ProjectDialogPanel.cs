// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2487 $</version>
// </file>

using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    /// <summary>
    /// Base class for project option panels that are using the <see cref="ConfigurationGuiHelper"/>.
    /// </summary>
    public partial class ProjectDialogPanel : DialogPanel, ICanBeDirty
    {
        protected ConfigurationHelper helper;
        protected MSBuildBasedProject project;

        public ProjectDialogPanel()
        {
            InitializeComponent();

            this.BackColor = Environment.OSVersion.Version.Major >= 6 ?
                Color.FromArgb(233, 236, 250) : SystemColors.ControlLight;
        }

        public bool IsDirty
        {
            get
            {
                if (this.DesignMode)
                {
                    return false;
                }

                return helper.IsDirty;
            }
            set
            {
                if (this.DesignMode)
                {
                    return;
                }

                helper.IsDirty = value;
            }
        }

        protected void InitializeHelper()
        {
            project = (MSBuildBasedProject)CustomizationObject;
            baseDirectory = project.Directory;
            helper = new ConfigurationHelper(project);
        }

        public event EventHandler DirtyChanged
        {
            add
            {
                helper.DirtyChanged += value;
            }
            remove
            {
                helper.DirtyChanged -= value;
            }
        }

        public override bool StorePanelContents()
        {
            return helper.Save();
        }
    }
}
