// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2991 $</version>
// </file>

using System;
using System.Collections;
using System.IO;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;

namespace ICSharpCode.SharpDevelop.Gui
{
	/// <summary>
	/// Summary description for Form2.
	/// </summary>
    public partial class SelectReferenceDialog : System.Windows.Forms.Form, ISelectReferenceDialog
	{
		IProject configureProject;
		
		public IProject ConfigureProject {
			get { return configureProject; }
		}
		
		public ArrayList ReferenceInformations {
			get {
				ArrayList referenceInformations = new ArrayList();
				foreach (ListViewItem item in referencesListView.Items) {
					System.Diagnostics.Debug.Assert(item.Tag != null);
					referenceInformations.Add(item.Tag);
				}
				return referenceInformations;
			}
		}
		
		public SelectReferenceDialog()
		{
			InitializeComponent();
		}

        public SelectReferenceDialog(IProject configureProject)
        {
            this.configureProject = configureProject;

            InitializeComponent();

            Translate(this);
            gacTabPage.Controls.Add(new GacReferencePanel(this));
            projectTabPage.Controls.Add(new ProjectReferencePanel(this));
            browserTabPage.Controls.Add(new AssemblyReferencePanel(this));
            comTabPage.Controls.Add(new COMReferencePanel(this));
        }
		
		void Translate(Control ctl)
		{
			ctl.Text = StringParser.Parse(ctl.Text);
			foreach (Control c in ctl.Controls)
				Translate(c);
			if (ctl is ListView) {
				foreach (ColumnHeader h in ((ListView)ctl).Columns) {
					h.Text = StringParser.Parse(h.Text);
				}
			}
		}
		
		public void AddReference(string referenceName, string referenceType, string referenceLocation, ReferenceProjectItem projectItem)
		{
			if (projectItem == null)
				throw new ArgumentNullException("projectItem");
			
			foreach (ListViewItem item in referencesListView.Items) {
				if (referenceLocation == item.SubItems[2].Text && referenceName == item.Text ) {
					return;
				}
			}
			
			ListViewItem newItem = new ListViewItem(new string[] {referenceName, referenceType, referenceLocation});
			newItem.Tag = projectItem;
			referencesListView.Items.Add(newItem);
		}
		
		void SelectReference(object sender, EventArgs e)
		{
			IReferencePanel refPanel = (IReferencePanel)referenceTabControl.SelectedTab.Controls[0];
			refPanel.AddReference();
		}
		
		void OkButtonClick(object sender, EventArgs e)
		{
			if (referencesListView.Items.Count == 0) {
				SelectReference(sender, e);
			}
		}
		
		void RemoveReference(object sender, EventArgs e)
		{
			ArrayList itemsToDelete = new ArrayList();
			
			foreach (ListViewItem item in referencesListView.SelectedItems) {
				itemsToDelete.Add(item);
			}
			
			foreach (ListViewItem item in itemsToDelete) {
				referencesListView.Items.Remove(item);
			}
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}
	}
	
	public interface IReferencePanel
	{
		void AddReference();
	}
	
	public interface ISelectReferenceDialog
	{
		/// <summary>
		/// Project to create references for.
		/// </summary>
		IProject ConfigureProject { get; }
		
		void AddReference(string referenceName, string referenceType, string referenceLocation, ReferenceProjectItem projectItem);
	}
	
	public enum ReferenceType {
		Assembly,
		Typelib,
		Gac,
		
		Project
	}
}
