// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2488 $</version>
// </file>

using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.TextEditor.Gui;

namespace ICSharpCode.SharpDevelop.TextEditor.Gui.OptionPanels
{
    /// <summary>
    /// General text editor options panel.
    /// </summary>
    public partial class GeneralTextEditorPanel : DialogPanel
    {
        public GeneralTextEditorPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        private bool IsClearTypeEnabled
        {
            get
            {
                return SystemInformation.IsFontSmoothingEnabled && 
                    SystemInformation.FontSmoothingType >= 2;
            }
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.GeneralTextEditorPanel.xfrm"));

            CodeEditorProperties properties = 
                CodeEditorProperties.Instance;

            enableFoldingCheckBox.Checked = properties.EnableFolding;
            showQuickClassBrowserCheckBox.Checked = properties.ShowQuickClassBrowserPanel;

            if (IsClearTypeEnabled)
            {
                // Somehow, SingleBitPerPixelGridFit still renders as Cleartype if cleartype is enabled
                // and we're using the TextRenderer for rendering.
                // So we cannot support not using antialiasing if system-wide font smoothening is enabled.
                enableAAFontRenderingCheckBox.Checked = true;
                enableAAFontRenderingCheckBox.Enabled = false;
            }
            else
            {
                enableAAFontRenderingCheckBox.Checked = 
                    (properties.TextRenderingHint == TextRenderingHint.AntiAliasGridFit 
                    || properties.TextRenderingHint == TextRenderingHint.ClearTypeGridFit);
            }

            mouseWheelZoomCheckBox.Checked = properties.MouseWheelTextZoom;

            foreach (String name in CharacterEncodings.Names)
            {
                textEncodingComboBox.Items.Add(name);
            }
            int encodingIndex = 0;
            try
            {
                encodingIndex = CharacterEncodings.GetEncodingIndex(
                    properties.EncodingCodePage);
            }
            catch
            {
                encodingIndex = CharacterEncodings.GetEncodingIndex(
                    Encoding.UTF8.CodePage);
            }
            textEncodingComboBox.SelectedIndex = encodingIndex;

            autoinsertCurlyBraceCheckBox.Checked = properties.AutoInsertCurlyBracket;
            hideMouseCursorCheckBox.Checked = properties.HideMouseCursor;
            caretBehindEOLCheckBox.Checked = properties.AllowCaretBeyondEOL;
            auotInsertTemplatesCheckBox.Checked = properties.AutoInsertTemplates;
            cutCopyWholeLine.Checked = properties.CutCopyWholeLine;

            if (properties.MouseWheelScrollDown)
            {
                wheelScrollNormal.Checked = true;
            }
            else
            {
                wheelScrollReverse.Checked = true;
            }
        }

        public override bool StorePanelContents()
        {
            CodeEditorProperties properties = CodeEditorProperties.Instance;

            if (enableAAFontRenderingCheckBox.Enabled)
            {
                properties.TextRenderingHint = enableAAFontRenderingCheckBox.Checked
                    ? TextRenderingHint.ClearTypeGridFit : TextRenderingHint.SystemDefault;
            }
            else
            {
                properties.TextRenderingHint = TextRenderingHint.SystemDefault;
            }
            properties.MouseWheelTextZoom = mouseWheelZoomCheckBox.Checked;
            properties.EnableFolding = enableFoldingCheckBox.Checked;
            properties.EncodingCodePage = CharacterEncodings.GetCodePageByIndex(textEncodingComboBox.SelectedIndex);
            properties.ShowQuickClassBrowserPanel = showQuickClassBrowserCheckBox.Checked;

            IViewContent activeViewContent = WorkbenchSingleton.Workbench.ActiveViewContent;

            if (activeViewContent is ITextEditorControlProvider)
            {
                TextEditorControl textarea = ((ITextEditorControlProvider)activeViewContent).TextEditorControl;
                textarea.OptionsChanged();
            }

            properties.AutoInsertCurlyBracket = autoinsertCurlyBraceCheckBox.Checked;
            properties.HideMouseCursor = hideMouseCursorCheckBox.Checked;
            properties.AllowCaretBeyondEOL = caretBehindEOLCheckBox.Checked;
            properties.AutoInsertTemplates = auotInsertTemplatesCheckBox.Checked;
            properties.CutCopyWholeLine = cutCopyWholeLine.Checked;

            properties.MouseWheelScrollDown = wheelScrollNormal.Checked;

            return true;
        }
    }
}
