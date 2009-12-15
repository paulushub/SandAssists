// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3702 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.Core.WinForms
{
	public static class ToolbarService
    {
        #region Private Static Fields

        private static List<LanguageChangeWatcher> _langWatchers = 
            new List<LanguageChangeWatcher>();
        private static VisualStudioToolStripRenderer strechedStripRenderer =
            strechedStripRenderer = new VisualStudioToolStripRenderer(false);

        private static List<ToolStrip> _toolStrips = new List<ToolStrip>();

        #endregion

        #region Public Static Properties

        public static IList<ToolStrip> ToolStrips
        {
            get
            {
                return _toolStrips;
            }
        }

        public static ToolStripRenderer StretchedRenderer
        {
            get
            {
                return strechedStripRenderer;
            }
        }

        #endregion

        #region Public Static ToolStrip Management Methods

        public static void RegisterToolStrip(ToolStrip toolStrip)
        {
            if (_toolStrips == null)
            {
                _toolStrips = new List<ToolStrip>();
            }
            if (toolStrip != null && !_toolStrips.Contains(toolStrip))
            {
                _toolStrips.Add(toolStrip);
            }
        }

        public static void RegisterToolStrip(ToolStrip[] toolStrips)
        {
            if (toolStrips == null || toolStrips.Length == 0)
            {
                return;
            }
            if (_toolStrips == null)
            {
                _toolStrips = new List<ToolStrip>();
            }
            int itemCount = toolStrips.Length;
            for (int i = 0; i < itemCount; i++)
            {
                ToolStrip toolStrip = toolStrips[i];
                if (toolStrip != null && !_toolStrips.Contains(toolStrip))
                {
                    _toolStrips.Add(toolStrip);
                }
            }
        }

        public static void UnRegisterToolStrip(string toolStripName)
        {
            int indexOf = IndexOfToolStrip(toolStripName);
            if (indexOf >= 0)
            {
                _toolStrips.RemoveAt(indexOf);
            }
        }

        public static void UnRegisterToolStrip(ToolStrip toolStrip)
        {
            if (toolStrip == null || _toolStrips == null || _toolStrips.Count == 0)
            {
                return;
            }
            int indexOf = _toolStrips.IndexOf(toolStrip);
            if (indexOf >= 0)
            {
                _toolStrips.RemoveAt(indexOf);
            }
        }

        public static int IndexOfToolStrip(string toolStripName)
        {
            if (String.IsNullOrEmpty(toolStripName) ||
               _toolStrips == null || _toolStrips.Count == 0)
            {
                return -1;
            }

            int indexOf   = -1;
            int itemCount = _toolStrips.Count;
            for (int i = 0; i < itemCount; i++)
            {
                if (String.Equals(toolStripName, _toolStrips[i].Name,
                    StringComparison.OrdinalIgnoreCase))
                {
                    indexOf = i;
                    break;
                }
            }

            return indexOf;
        }

        public static int IndexOfToolStrip(ToolStrip toolStrip)
        {
            if (toolStrip == null || _toolStrips == null || _toolStrips.Count == 0)
            {
                return -1;
            }

            return _toolStrips.IndexOf(toolStrip);
        }

        public static bool ContainsToolStrip(string toolStripName)
        {
            if (String.IsNullOrEmpty(toolStripName) ||
               _toolStrips == null || _toolStrips.Count == 0)
            {
                return false;
            }

            bool isContained = false;
            int itemCount = _toolStrips.Count;
            for (int i = 0; i < itemCount; i++)
            {
                if (String.Equals(toolStripName, _toolStrips[i].Name,
                    StringComparison.OrdinalIgnoreCase))
                {
                    isContained = true;
                    break;
                }
            }

            return isContained;
        }

        public static bool ContainsToolStrip(ToolStrip toolStrip)
        {
            if (toolStrip == null || _toolStrips == null || _toolStrips.Count == 0)
            {
                return false;
            }

            return _toolStrips.Contains(toolStrip);
        }

        public static void SetVisibleToolStrip(string toolStripName, 
            bool isVisible, bool isUserRequest)
        {
            int indexOf = IndexOfToolStrip(toolStripName);
            if (indexOf >= 0)
            {
                ToolStrip toolStrip = _toolStrips[indexOf];
                toolStrip.Visible   = isVisible;
            }
        }

        public static void SetVisibleToolStrip(ToolStrip toolStrip, 
            bool isVisible, bool isUserRequest)
        {   
            if (toolStrip == null)
            {
                return;
            }
            int indexOf = IndexOfToolStrip(toolStrip);
            if (indexOf >= 0)
            {
                toolStrip.Visible = isVisible;
            }
        }

        #endregion

        #region Public Static ToolStripItem Creation Methods

        public static ToolStripItem[] CreateToolStripItems(string path, object owner, bool throwOnNotFound)
		{
			return CreateToolStripItems(owner, AddInTree.GetTreeNode(path, throwOnNotFound));
		}
		
		public static ToolStripItem[] CreateToolStripItems(object owner, AddInTreeNode treeNode)
		{
			if (treeNode == null)
				return new ToolStripItem[0];
			List<ToolStripItem> collection = new List<ToolStripItem>();
			foreach (ToolbarItemDescriptor descriptor in 
                treeNode.BuildChildItems<ToolbarItemDescriptor>(owner)) 
            {
				object item = CreateToolbarItemFromDescriptor(descriptor);
                ToolStripItem stripItem = item as ToolStripItem;
                if (stripItem != null)
                {
                    collection.Add(stripItem);
				} 
                else 
                {
                    ISubmenuBuilder submenuBuilder = item as ISubmenuBuilder;
                    if (submenuBuilder != null)
                    {
                        collection.AddRange(submenuBuilder.BuildSubmenu(null, owner));
                    }
                    else
                    {
                        ICommandGroup groupBuilder = item as ICommandGroup;
                        if (groupBuilder != null)
                        {
                            IList<ToolStripItem> groupedItems = groupBuilder.Items;
                            if (groupedItems != null && groupedItems.Count != 0)
                            {
                                collection.AddRange(groupedItems);
                            }
                        } 
                    }
				}
			}
			
			return collection.ToArray();
		}

        public static ToolStripItem InsertHelpItem(ToolStrip toolStrip,
            bool includeText, EventHandler clickHandler)
        {
            ToolStripButton helpItem = new ToolStripButton();
            helpItem.Alignment = ToolStripItemAlignment.Right;
            helpItem.Margin    = new Padding(0, 1, 4, 2);
            helpItem.Name      = "helpItem";
            helpItem.Image     = WinFormsResourceService.GetBitmap("Icons.16x16.ToolHelp");
            helpItem.Text      = StringParser.Parse("${res:Global.HelpButtonText}");
            helpItem.DisplayStyle = includeText ?
                ToolStripItemDisplayStyle.ImageAndText : ToolStripItemDisplayStyle.Image;
            
            if (clickHandler != null)
            {
                helpItem.Click += clickHandler;
            } 
            if (toolStrip != null)
            {
                toolStrip.Items.Add(helpItem);
            }

            return helpItem;
        }

        #endregion

        #region Public Static ToolStrip Creation Methods

        internal static object CreateToolbarItemFromDescriptor(ToolbarItemDescriptor descriptor)
		{
			Codon codon = descriptor.Codon;
			object caller = descriptor.Caller;
			string type = codon.Properties.Contains("type") ? codon.Properties["type"] : "Item";
			
			bool createCommand = codon.Properties["loadclasslazy"] == "false";
			
			switch (type) 
            {
				case "Separator":
					return new ToolBarSeparator(codon, caller);
				case "CheckBox":
					return new ToolBarCheckBox(codon, caller);
				case "Item":
					return new ToolBarCommand(codon, caller, createCommand);
                case "Button":
                    return new ToolBarCommand(codon, caller, createCommand);
				case "ComboBox":
					return new ToolBarComboBox(codon, caller);
				case "TextBox":
					return new ToolBarTextBox(codon, caller);
				case "Label":
					return new ToolBarLabel(codon, caller);
				case "DropDownButton":
					return new ToolBarDropDownButton(codon, caller, MenuService.ConvertSubItems(descriptor.SubItems));
				case "SplitButton":
					return new ToolBarSplitButton(codon, caller, MenuService.ConvertSubItems(descriptor.SubItems));
				case "Builder":
					return codon.AddIn.CreateObject(codon.Properties["class"]);
                case "GroupedButtons":
                    return new ToolBarGroupedButtons(codon, caller, descriptor);
				default:
					throw new NotSupportedException("unsupported menu item type : " + type);
			}
		}

		public static ToolStrip CreateToolStrip(object owner, 
            AddInTreeNode treeNode, bool isMain)
		{
            ToolBarStrip toolStrip = new ToolBarStrip(owner, isMain);

            if (isMain)
            {         
                // This is a little hack to use the toolbar item descriptor as
                // a toolbar label...
                if (treeNode.Codons.Count != 0)
                {
                    Codon codon = treeNode.Codons[0];
                    Properties properties = codon.Properties;

                    String itemType = properties["type"];
                    if (!String.IsNullOrEmpty(itemType) &&
                        itemType.Equals("Description", StringComparison.OrdinalIgnoreCase))
                    {
                        // Now, remove it, otherwise, there will be an exception...
                        treeNode.Codons.RemoveAt(0);

                        toolStrip.Name = properties["id"];
                        toolStrip.Text = properties["label"];

                        toolStrip.Conditions = codon.Conditions;
                    }
                }
            }

            toolStrip.Items.AddRange(CreateToolStripItems(owner, treeNode));

			UpdateToolbar(toolStrip); // setting Visible is only possible after the items have been added
			_langWatchers.Add(new LanguageChangeWatcher(toolStrip));

            if (!isMain)
            {
                toolStrip.Renderer = strechedStripRenderer;
            }

			return toolStrip;
		}

		public static ToolStrip CreateToolStrip(object owner, 
            AddInTreeNode treeNode)
		{
            return CreateToolStrip(owner, treeNode, false);
		}

        public static void IndentItems(ToolStrip toolStrip)
        {
            //TODO - Consider right to left environments...
            if (toolStrip == null || toolStrip.Items.Count == 0)
            {
                return;
            }

            ToolStripItem toolItem = toolStrip.Items[0];
            if (toolItem != null)
            {
                Padding padding = toolItem.Margin;
                padding.Left    = padding.Left + 4;
                toolItem.Margin = padding;
            }
        }
		
		public static ToolStrip CreateToolStrip(object owner, string addInTreePath)
		{
			return CreateToolStrip(owner, AddInTree.GetTreeNode(addInTreePath));
		}
		
		public static ToolStrip[] CreateToolbars(object owner, 
            string addInTreePath, bool isMain)
		{
			AddInTreeNode treeNode;
			try {
				treeNode = AddInTree.GetTreeNode(addInTreePath);
			} catch (TreePathNotFoundException) {
				return null;
				
			}
			List<ToolStrip> toolBars = new List<ToolStrip>();
			foreach (AddInTreeNode childNode in treeNode.ChildNodes.Values) {
                toolBars.Add(CreateToolStrip(owner, childNode, isMain));
			}
			return toolBars.ToArray();
		}

        public static ToolStrip[] CreateToolbars(object owner, string addInTreePath)
        {
            return CreateToolbars(owner, addInTreePath, false);
        }
		
		public static void UpdateToolbar(ToolStrip toolStrip)
		{
            toolStrip.SuspendLayout();
			foreach (ToolStripItem item in toolStrip.Items) {
                IStatusUpdate itemStatus = item as IStatusUpdate;
                if (itemStatus != null)
                {
                    itemStatus.UpdateStatus();
				}
			}

            toolStrip.ResumeLayout();
			//toolStrip.Refresh();
		}
		
		public static void UpdateToolbarVisibility(ToolStrip toolStrip)
		{
            bool isVisible = true;

            ToolBarStrip barStrip = toolStrip as ToolBarStrip;
            if (barStrip != null)
            {
                isVisible = barStrip.IsVisibleStatus;
            }
            if (!isVisible)
            {
                toolStrip.Visible = false;
                return;
            }
            toolStrip.SuspendLayout();
            foreach (ToolStripItem item in toolStrip.Items)
            {
                IStatusUpdate itemStatus = item as IStatusUpdate;
                if (itemStatus != null)
                {
                    itemStatus.UpdateStatus();
                }
            }

            toolStrip.ResumeLayout();

            toolStrip.Visible = true;
		}
		
		public static void UpdateToolbarText(ToolStrip toolStrip)
		{
            toolStrip.SuspendLayout();
			foreach (ToolStripItem item in toolStrip.Items) {
                IStatusUpdate itemStatus = item as IStatusUpdate;
                if (itemStatus != null)
                {
                    itemStatus.UpdateText();
				}
			}

            toolStrip.ResumeLayout();
        }

        #endregion

        #region LanguageChangeWatcher Class

        private class LanguageChangeWatcher
        {
            ToolStrip toolStrip;
            public LanguageChangeWatcher(ToolStrip toolStrip)
            {
                this.toolStrip = toolStrip;
                toolStrip.Disposed += Disposed;
                ResourceService.LanguageChanged += OnLanguageChanged;
            }
            void OnLanguageChanged(object sender, EventArgs e)
            {
                ToolbarService.UpdateToolbarText(toolStrip);
            }
            void Disposed(object sender, EventArgs e)
            {
                ResourceService.LanguageChanged -= OnLanguageChanged;
            }
        }

        #endregion
    }
}
