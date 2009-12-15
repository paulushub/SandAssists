// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 2632 $</version>
// </file>

using System;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Dom;

namespace ICSharpCode.FormsDesigner.Gui
{
    public partial class AddComponentsDialog : Form
    {
        private ArrayList selectedComponents;

        private Type typeTBItemFilter;
        private Type typeTBItem;
        private Type typeComponents;
        private Type typeDesignVisible;
        private Type typeTBBitmap;

        public AddComponentsDialog()
        {
            InitializeComponent();

            foreach (Control ctl in this.Controls.GetRecursive())
            {
                ctl.Text = StringParser.Parse(ctl.Text);

                ListView listView = ctl as ListView;
                if (listView != null)
                {
                    foreach (ColumnHeader header in listView.Columns)
                    {
                        header.Text = StringParser.Parse(header.Text);
                    }
                }
            }
            this.Text = StringParser.Parse(this.Text);

            typeTBItemFilter = typeof(ToolboxItemFilterAttribute);
            typeTBItem = typeof(ToolboxItemAttribute);
            typeComponents = typeof(System.ComponentModel.IComponent);
            typeDesignVisible = typeof(DesignTimeVisibleAttribute);
            typeTBBitmap = typeof(ToolboxBitmapAttribute);

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.FormsDesigner.Resources.AddSidebarComponentsDialog.xfrm"));

            //componentListView = (ListView)ControlDictionary["componentListView"];

            //Icon = null;
            PrintGACCache();

            browseButton.Click += new System.EventHandler(this.browseButtonClick);
            gacListView.SelectedIndexChanged += new System.EventHandler(this.gacListViewSelectedIndexChanged);
            //			((TextBox)ControlDictionary["fileNameTextBox"]).TextChanged += new System.EventHandler(this.fileNameTextBoxTextChanged);
            okButton.Click += new System.EventHandler(this.buttonClick);
            loadButton.Click += new System.EventHandler(this.loadButtonClick);
        }

        public ArrayList SelectedComponents
        {
            get
            {
                return selectedComponents;
            }
        }

        private void PrintGACCache()
        {
            foreach (DomAssemblyName asm in GacInterop.GetAssemblyList())
            {
                try
                {
                    string assemblyName = asm.FullName;
                    ListViewItem item = new ListViewItem(
                        new string[] { asm.ShortName, asm.Version });
                    item.Tag = assemblyName;
                    gacListView.Items.Add(item);
                    //Assembly assembly = Assembly.Load(assemblyName);

                    //if (this.ContainsComponent(assembly))
                    //{
                    //}
                }
                catch
                {
                	
                }
            }
        }

        private void BeginFillComponentsList()
        {
            componentListView.BeginUpdate();
            componentListView.Items.Clear();
            componentListView.Controls.Clear();
        }

        private void EndFillComponentsList(Assembly lastAssembly)
        {
            if (componentListView.Items.Count == 0)
            {
                if (componentListView.Controls.Count == 0)
                {
                    string name = String.Empty;
                    if (lastAssembly != null)
                    {
                        name = lastAssembly.FullName;
                    }
                    ClearComponentsList(StringParser.Parse(
                        "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.AddSidebarComponents.NoComponentsFound}", new string[,] { { "Name", name } }));
                }
            }
            componentListView.EndUpdate();
        }

        private void AddComponentsToList(Assembly assembly, string loadPath)
        {
            if (assembly == null)
            {
                return;
            }
            Dictionary<string, int> images = new Dictionary<string, int>();
            ImageList il = new ImageList();
            // try to load res icon
            string[] imgNames = assembly.GetManifestResourceNames();

            foreach (string im in imgNames)
            {
                try
                {
                    Bitmap b = new Bitmap(Image.FromStream(assembly.GetManifestResourceStream(im)));
                    b.MakeTransparent();
                    images[im] = il.Images.Count;
                    il.Images.Add(b);
                }
                catch
                {
                }
            }

            try
            {
                string fullName = null;
                componentListView.SmallImageList = il;
                foreach (Type t in assembly.GetExportedTypes())
                {
                    if (!t.IsAbstract)
                    {
                        if (t.IsDefined(typeTBItemFilter, true) || 
                            t.IsDefined(typeTBItem, true) || 
                            typeComponents.IsAssignableFrom(t))
                        {
                            object[] attributes = t.GetCustomAttributes(false);
                            object[] filterAttrs = t.GetCustomAttributes(typeDesignVisible, true);
                            foreach (DesignTimeVisibleAttribute visibleAttr in filterAttrs)
                            {
                                if (!visibleAttr.Visible)
                                {
                                    goto skip;
                                }
                            }

                            fullName = t.FullName + ".bmp";
                            if (!images.ContainsKey(fullName))
                            {
                                if (t.IsDefined(typeTBBitmap, false))
                                {
                                    foreach (object attr in attributes)
                                    {
                                        if (attr is ToolboxBitmapAttribute)
                                        {
                                            ToolboxBitmapAttribute toolboxBitmapAttribute = (ToolboxBitmapAttribute)attr;
                                            images[fullName] = il.Images.Count;
                                            Bitmap b = new Bitmap(toolboxBitmapAttribute.GetImage(t));
                                            b.MakeTransparent();
                                            il.Images.Add(b);
                                            break;
                                        }
                                    }
                                }
                            }

                            ListViewItem newItem = new ListViewItem(t.Name);
                            newItem.SubItems.Add(t.Namespace);
                            newItem.SubItems.Add(assembly.ToString());
                            newItem.SubItems.Add(assembly.Location);
                            newItem.SubItems.Add(t.Namespace);
                            if (images.ContainsKey(fullName))
                            {
                                newItem.ImageIndex = images[fullName];
                            }
                            newItem.Checked = true;
                            ToolComponent toolComponent = new ToolComponent(t.FullName, 
                                new ComponentAssembly(assembly.FullName, loadPath), true);
                            newItem.Tag = toolComponent;
                            componentListView.Items.Add(newItem);
                            //ToolboxItem item = new ToolboxItem(t);
                        skip: ;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                ClearComponentsList(e.Message);
            }
        }

        private bool ContainsComponent(Assembly assembly)
        {
            if (assembly == null)
            {
                return false;
            }
            int componentCount = 0;
            try
            {
                foreach (Type t in assembly.GetExportedTypes())
                {
                    if (!t.IsAbstract)
                    {
                        if (t.IsDefined(typeTBItemFilter, true) || 
                            t.IsDefined(typeTBItem, true) || 
                            typeComponents.IsAssignableFrom(t))
                        {
                            object[] attributes = t.GetCustomAttributes(false);
                            object[] filterAttrs = t.GetCustomAttributes(typeDesignVisible, true);
                            foreach (DesignTimeVisibleAttribute visibleAttr in filterAttrs)
                            {
                                if (!visibleAttr.Visible)
                                {
                                    continue;
                                }
                            }

                            componentCount++;
                        }
                    }
                }

                return (componentCount > 0);
            }
            catch
            {
                return false;                
            }
        }

        private void gacListViewSelectedIndexChanged(object sender, EventArgs e)
        {
            if (gacListView.SelectedItems != null && 
                gacListView.SelectedItems.Count == 1)
            {
                string assemblyName = gacListView.SelectedItems[0].Tag.ToString();
                try
                {
                    Assembly asm = Assembly.Load(assemblyName);
                    BeginFillComponentsList();
                    AddComponentsToList(asm, null);
                    EndFillComponentsList(asm);
                }
                catch (Exception ex)
                {
                    EndFillComponentsList(null);
                    ClearComponentsList(ex.Message);
                }
            }
            else
            {
                ClearComponentsList(null);
            }
        }

        private void ClearComponentsList(string message)
        {
            componentListView.Items.Clear();
            componentListView.Controls.Clear();
            if (message != null)
            {
                Label lbl = new Label();
                lbl.BackColor = SystemColors.Window;
                lbl.Text = StringParser.Parse(message);
                lbl.Dock = DockStyle.Fill;
                componentListView.Controls.Add(lbl);
            }
        }

        private void loadButtonClick(object sender, EventArgs e)
        {
            BeginFillComponentsList();
            try
            {
                string assemblyFileNames = fileNameTextBox.Text;
                Assembly lastAssembly = null;
                foreach (string assemblyFileName in assemblyFileNames.Split(';'))
                {
                    if (!System.IO.File.Exists(assemblyFileName))
                    {
                        EndFillComponentsList(null);
                        ClearComponentsList(assemblyFileName + " was not found.");
                        MessageService.ShowWarning(
                            "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.AddSidebarComponents.EnterValidFilename}");
                        return;
                    }

                    Assembly asm = Assembly.LoadFrom(assemblyFileName);
                    lastAssembly = asm;
                    AddComponentsToList(asm, Path.GetDirectoryName(assemblyFileName));
                }
                EndFillComponentsList(lastAssembly);
            }
            catch
            {
                EndFillComponentsList(null);
                MessageService.ShowWarning(
                    "${res:ICSharpCode.SharpDevelop.FormDesigner.Gui.AddSidebarComponents.FileIsNotAssembly}");
                ClearComponentsList(null);
            }
        }

        private void buttonClick(object sender, EventArgs e)
        {
            selectedComponents = new ArrayList();
            foreach (ListViewItem item in componentListView.Items)
            {
                if (item.Checked)
                {
                    selectedComponents.Add((ToolComponent)item.Tag);
                }
            }
        }

        private void browseButtonClick(object sender, EventArgs e)
        {
            using (OpenFileDialog fdiag = new OpenFileDialog())
            {
                fdiag.AddExtension = true;

                fdiag.Filter = StringParser.Parse(
                    "${res:SharpDevelop.FileFilter.AssemblyFiles}|*.dll;*.exe|${res:SharpDevelop.FileFilter.AllFiles}|*.*");
                fdiag.Multiselect = true;
                fdiag.CheckFileExists = true;

                if (fdiag.ShowDialog(
                    ICSharpCode.SharpDevelop.Gui.WorkbenchSingleton.MainForm) == DialogResult.OK)
                {
                    fileNameTextBox.Text = string.Join(";", fdiag.FileNames);
                }
            }
        }
    }
}
