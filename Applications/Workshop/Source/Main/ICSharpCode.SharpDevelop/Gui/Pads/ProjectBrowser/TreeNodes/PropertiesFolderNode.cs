// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <version>$Revision: 0001 $</version>
// </file>

using System;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Project
{
    public class PropertiesFolderNode : DirectoryNode
    {
        private IProject _project;

        public PropertiesFolderNode(IProject project)
            : this(project, project.Directory)
        {
        }

        public PropertiesFolderNode(IProject project, string directory)
            : base(directory)
        {
            this._project = project;

            this.sortOrder = 0;

            this.SpecialFolder            = SpecialFolder.AppDesigner;
            this.ToolbarAddinTreePath     = "/SharpDevelop/Pads/ProjectBrowser/ToolBar/File";
            this.ContextmenuAddinTreePath = "/SharpDevelop/Pads/ProjectBrowser/ContextMenu/PropertiesFolderNode";
        }

        #region Public Properties

        public override IProject Project
        {
            get
            {
                return _project;
            }
        }

        public override bool EnableDelete
        {
            get
            {
                return false;
            }
        }

        public override bool EnableCut
        {
            get
            {
                return false;
            }
        }

        public override bool EnablePaste
        {
            get
            {
                return false;
            }
        }

        #endregion

        protected override void Initialize()
        {
            base.Initialize();
        }

        public override void ActivateItem()
        {
            Commands.ViewProjectOptions.ShowProjectOptions(_project);
        }

        public override object AcceptVisitor(
            ProjectBrowserTreeNodeVisitor visitor, object data)
        {
            return visitor.Visit(this, data);
        }
    }
}
