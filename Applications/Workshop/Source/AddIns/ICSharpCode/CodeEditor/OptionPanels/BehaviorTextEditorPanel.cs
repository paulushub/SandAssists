// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
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
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.TextEditor.Gui;
using ICSharpCode.TextEditor;
using ICSharpCode.TextEditor.Document;

namespace ICSharpCode.SharpDevelop.TextEditor.Gui.OptionPanels
{
    public partial class BehaviorTextEditorPanel : DialogPanel
    {
        public BehaviorTextEditorPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.BehaviorTextEditorPanel.xfrm"));

            CodeEditorProperties properties = CodeEditorProperties.Instance;

            if (properties.ConvertTabsToSpaces)
            {
                insertSpaces.Checked = true;
            }
            else
            {
                insertTabs.Checked = true;
            }

            tabSizeTextBox.Text = properties.TabIndent.ToString();
            indentSizeTextBox.Text = properties.IndentationSize.ToString();

            //indentStyleComboBox.Items.Add(StringParser.Parse(
            //    "${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.None}"));
            //indentStyleComboBox.Items.Add(StringParser.Parse(
            //    "${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.Automatic}"));
            //indentStyleComboBox.Items.Add(StringParser.Parse(
            //    "${res:Dialog.Options.IDEOptions.TextEditor.Behaviour.IndentStyle.Smart}"));

            IndentStyle curStyle = properties.IndentStyle;
            indentNone.Checked = (curStyle == IndentStyle.None);
            indentAutomatic.Checked = (curStyle == IndentStyle.Auto);
            indentSmart.Checked = (curStyle == IndentStyle.Smart);
            //indentStyleComboBox.SelectedIndex = (int)properties.IndentStyle;
        }

        public override bool StorePanelContents()
        {
            CodeEditorProperties properties = CodeEditorProperties.Instance;

            properties.ConvertTabsToSpaces = insertSpaces.Checked;

            IndentStyle selStyle = IndentStyle.None;
            if (indentAutomatic.Checked)
            {
                selStyle = IndentStyle.Auto;
            }
            else if (indentSmart.Checked)
            {
                selStyle = IndentStyle.Smart;
            }
            properties.IndentStyle = selStyle;
            //properties.IndentStyle = (IndentStyle)indentStyleComboBox.SelectedIndex;

            try
            {
                int tabSize = Int32.Parse(tabSizeTextBox.Text);

                // FIX: don't allow to set tab size to zero as this will cause divide by zero exceptions in the text control.
                // Zero isn't a setting that makes sense, anyway.
                if (tabSize > 0)
                {
                    properties.TabIndent = tabSize;
                }
            }
            catch (Exception)
            {
            }

            try
            {
                properties.IndentationSize = Int32.Parse(indentSizeTextBox.Text);
            }
            catch (Exception)
            {
            }

            IViewContent activeViewContent = WorkbenchSingleton.Workbench.ActiveViewContent;

            if (activeViewContent is ITextEditorControlProvider)
            {
                TextEditorControl textarea = 
                    ((ITextEditorControlProvider)activeViewContent).TextEditorControl;
                textarea.OptionsChanged();
            }

            return true;
        }
    }
}
