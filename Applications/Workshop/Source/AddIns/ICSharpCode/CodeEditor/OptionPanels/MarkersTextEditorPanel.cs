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
    public partial class MarkersTextEditorPanel : DialogPanel
    {
        public MarkersTextEditorPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.MarkersTextEditorPanel.xfrm"));

            CodeEditorProperties properties = CodeEditorProperties.Instance;
            showLineNumberCheckBox.Checked = properties.ShowLineNumbers;
            showInvalidLinesCheckBox.Checked = properties.ShowInvalidLines;
            showBracketHighlighterCheckBox.Checked = properties.ShowMatchingBracket;
            showErrorsCheckBox.Checked = properties.UnderlineErrors;
            showHRulerCheckBox.Checked = properties.ShowHorizontalRuler;
            showEOLMarkersCheckBox.Checked = properties.ShowEOLMarker;
            showVRulerCheckBox.Checked = properties.ShowVerticalRuler;
            showTabCharsCheckBox.Checked = properties.ShowTabs;
            showSpaceCharsCheckBox.Checked = properties.ShowSpaces;

            vRulerRowTextBox.Text = properties.VerticalRulerRow.ToString();

            showLineHighlighterCheckBox.Checked = 
                (properties.LineViewerStyle == LineViewerStyle.FullRow);

            BracketMatchingStyle braceStyle = properties.BracketMatchingStyle;
            if (braceStyle == BracketMatchingStyle.Before)
            {
                highlightBeforeCaret.Checked = true;
            }
            else
            {
                highlightAfterCaret.Checked = true;
            }
        }

        public override bool StorePanelContents()
        {
            CodeEditorProperties properties = CodeEditorProperties.Instance;
            properties.ShowInvalidLines = showInvalidLinesCheckBox.Checked;
            properties.ShowLineNumbers = showLineNumberCheckBox.Checked;
            properties.ShowMatchingBracket = showBracketHighlighterCheckBox.Checked;
            properties.UnderlineErrors = showErrorsCheckBox.Checked;
            properties.ShowHorizontalRuler = showHRulerCheckBox.Checked;
            properties.ShowEOLMarker = showEOLMarkersCheckBox.Checked;
            properties.ShowVerticalRuler = showVRulerCheckBox.Checked;
            properties.ShowTabs = showTabCharsCheckBox.Checked;
            properties.ShowSpaces = showSpaceCharsCheckBox.Checked;

            try
            {
                properties.VerticalRulerRow = Int32.Parse(vRulerRowTextBox.Text);
            }
            catch (Exception)
            {
            }

            properties.LineViewerStyle = showLineHighlighterCheckBox.Checked ?
                LineViewerStyle.FullRow : LineViewerStyle.None;   
            BracketMatchingStyle braceStyle = BracketMatchingStyle.Before;
            if (highlightAfterCaret.Checked)
            {
                braceStyle = BracketMatchingStyle.After;
            }
            properties.BracketMatchingStyle = braceStyle;

            IViewContent activeViewContent = WorkbenchSingleton.Workbench.ActiveViewContent;

            if (activeViewContent is ITextEditorControlProvider)
            {
                TextEditorControl textarea = ((ITextEditorControlProvider)activeViewContent).TextEditorControl;
                textarea.OptionsChanged();
            }

            return true;
        }
    }
}
