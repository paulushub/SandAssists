﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3359 $</version>
// </file>

using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ResourceEditor
{
	public class ResourceEditorDisplayBinding : IDisplayBinding
	{
		// IDisplayBinding interface
		public bool CanCreateContentForFile(string fileName)
		{
			return Path.GetExtension(fileName).Equals(".RESOURCES", StringComparison.OrdinalIgnoreCase) ||
				Path.GetExtension(fileName).Equals(".RESX", StringComparison.OrdinalIgnoreCase);
		}
		
		public IViewContent CreateContentForFile(OpenedFile file)
		{
			return new ResourceEditWrapper(file);
		}
	}
	
	/// <summary>
	/// This class describes the main functionality of a language codon
	/// </summary>
	public class ResourceEditWrapper : AbstractViewContentHandlingLoadErrors, IClipboardHandler
	{
		ResourceEditorControl resourceEditor = new ResourceEditorControl();
		
		public ResourceEditorControl ResourceEditor {
			get { return resourceEditor; }
		}
		
		public override bool IsReadOnly {
			get {
				return false;
			}
		}

		void SetDirty(object sender, EventArgs e)
		{
			this.PrimaryFile.MakeDirty();
		}
		
		public ResourceEditWrapper(OpenedFile file)
		{
			this.TabPageText = "Resource editor";
			base.UserControl = resourceEditor;
			resourceEditor.ResourceList.Changed += new EventHandler(SetDirty);
			this.Files.Add(file);
		}
		
		public override void RedrawContent()
		{
		}

        protected override void Dispose(bool disposing)
		{
            if (!this.IsDisposed)
            {
                resourceEditor.Dispose();
            }

            base.Dispose(disposing);
		}
		
		protected override void LoadInternal(OpenedFile file, Stream stream)
		{
			resourceEditor.ResourceList.LoadFile(file.FileName, stream);
		}
		
		protected override void SaveInternal(OpenedFile file, Stream stream)
		{
			resourceEditor.ResourceList.SaveFile(file.FileName, stream);
		}
		
		
		public bool EnableCut
		{
			get {
				if (resourceEditor.ResourceList.IsEditing || !resourceEditor.ResourceList.Focused) {
					return false;
				}
				return resourceEditor.ResourceList.SelectedItems.Count > 0;
			}
		}
		
		public bool EnableCopy
		{
			get {
				if (resourceEditor.ResourceList.IsEditing || !resourceEditor.ResourceList.Focused) {
					return false;
				}
				return resourceEditor.ResourceList.SelectedItems.Count > 0;
			}
		}
		
		public bool EnablePaste
		{
			get {
				if (resourceEditor.ResourceList.IsEditing || !resourceEditor.ResourceList.Focused) {
					return false;
				}
				return true;
			}
		}
		
		public bool EnableDelete
		{
			get {
				if (resourceEditor.ResourceList.IsEditing || !resourceEditor.ResourceList.Focused) {
					return false;
				}
				return resourceEditor.ResourceList.SelectedItems.Count > 0;
			}
		}
		
		public bool EnableSelectAll
		{
			get {
				if (resourceEditor.ResourceList.IsEditing || !resourceEditor.ResourceList.Focused) {
					return false;
				}
				return true;
			}
		}
		
		public void Cut()
		{
			if (resourceEditor.ResourceList.WriteProtected || resourceEditor.ResourceList.SelectedItems.Count < 1)
				return;
			
			Hashtable tmphash = new Hashtable();
			foreach (ListViewItem item in resourceEditor.ResourceList.SelectedItems) {
				tmphash.Add(item.Text, resourceEditor.ResourceList.Resources[item.Text].ResourceValue);
				resourceEditor.ResourceList.Resources.Remove(item.Text);
				resourceEditor.ResourceList.Items.Remove(item);
			}
			resourceEditor.ResourceList.OnChanged();
			ClipboardWrapper.SetDataObject(tmphash);
		}
		
		public void Copy()
		{
			if (resourceEditor.ResourceList.SelectedItems.Count < 1) {
				return;
			}
			
			Hashtable tmphash = new Hashtable();
			foreach (ListViewItem item in resourceEditor.ResourceList.SelectedItems) {
				object resourceValue = GetClonedResource(resourceEditor.ResourceList.Resources[item.Text].ResourceValue);
				tmphash.Add(item.Text, resourceValue); // copy a clone to clipboard
			}
			ClipboardWrapper.SetDataObject(tmphash);
		}
		
		public void Paste()
		{
			if (resourceEditor.ResourceList.WriteProtected) {
				return;
			}
			
			IDataObject dob = ClipboardWrapper.GetDataObject();
			if (dob == null)
				return;
			
			if (dob.GetDataPresent(typeof(Hashtable).FullName)) {
				Hashtable tmphash = (Hashtable)dob.GetData(typeof(Hashtable));
				foreach (DictionaryEntry entry in tmphash) {
					
					object resourceValue = GetClonedResource(entry.Value);
					ResourceItem item;
					
					if (!resourceEditor.ResourceList.Resources.ContainsKey((string)entry.Key)) {
						item  = new ResourceItem(entry.Key.ToString(), resourceValue);
					} else {
						int count = 1;
						string newNameBase = entry.Key.ToString() + " ";
						string newName = newNameBase + count.ToString();
						
						while(resourceEditor.ResourceList.Resources.ContainsKey(newName)) {
							count++;
							newName = newNameBase + count.ToString();
						}
						item = new ResourceItem(newName, resourceValue);
					}
					resourceEditor.ResourceList.Resources.Add(item.Name, item);
					resourceEditor.ResourceList.OnChanged();
				}
				resourceEditor.ResourceList.InitializeListView();
			}
		}

		/// <summary>
		/// Clones a resource if the <paramref name="resource"/>
		/// is cloneable.
		/// </summary>
		/// <param name="resource">A resource to clone.</param>
		/// <returns>A cloned resource if the object implements
		/// the ICloneable interface, otherwise the
		/// <paramref name="resource"/> object.</returns>
		object GetClonedResource(object resource)
		{
			object clonedResource = null;
			
			ICloneable cloneableResource = resource as ICloneable;
			if (cloneableResource != null) {
				clonedResource = cloneableResource.Clone();
			} else {
				clonedResource = resource;
			}
			
			return clonedResource;
		}
		
		public void Delete()
		{
			if (resourceEditor.ResourceList.WriteProtected) {
				return;
			}
			
			if (resourceEditor.ResourceList.SelectedItems.Count==0) return; // nothing to do
			DialogResult rc;
			
			try {
				
				rc=MessageBox.Show(ResourceService.GetString("ResourceEditor.DeleteEntry.Confirm"),ResourceService.GetString("ResourceEditor.DeleteEntry.Title"),MessageBoxButtons.OKCancel);
			}
			catch {
				// when something happens - like resource is missing - try to use default message
				rc = MessageBox.Show("Do you really want to delete?","Delete-Warning!",MessageBoxButtons.OKCancel);
			}
			
			if (rc != DialogResult.OK) {
				return;
			}
			
			foreach (ListViewItem item in resourceEditor.ResourceList.SelectedItems) {
				//// not clear why this check is present here - seems to be extra
				////if (item.Text != null) {
				resourceEditor.ResourceList.Resources.Remove(item.Text);
				resourceEditor.ResourceList.Items.Remove(item);
				// and set dirty flag
				resourceEditor.ResourceList.OnChanged();
			}
		}
		
		public void SelectAll()
		{
			foreach (ListViewItem i in resourceEditor.ResourceList.Items) {
				i.Selected=true;
			}
		}
	}
}
