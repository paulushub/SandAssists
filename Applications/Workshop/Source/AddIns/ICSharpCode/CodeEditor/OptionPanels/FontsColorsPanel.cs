using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.TextEditor;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.TextEditor.Gui;

namespace ICSharpCode.SharpDevelop.TextEditor.Gui.OptionPanels
{
    public partial class FontsColorsPanel : DialogPanel
    {
        private FontSelectionPanelHelper helper;

        public FontsColorsPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        private Font CurrentFont
        {
            get
            {
                return helper.GetSelectedFont();
            }
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.GeneralTextEditorPanel.xfrm"));

            CodeEditorProperties properties =
                CodeEditorProperties.Instance;

            for (int i = 6; i <= 24; ++i)
            {
                fontSizeComboBox.Items.Add(i);
            }

            fontSizeComboBox.TextChanged += new EventHandler(UpdateFontPreviewLabel);
            fontSizeComboBox.Enabled = false;

            fontListComboBox.Enabled = false;
            fontListComboBox.TextChanged += new EventHandler(UpdateFontPreviewLabel);
            fontListComboBox.SelectedIndexChanged += new EventHandler(UpdateFontPreviewLabel);

            Font currentFont = FontSelectionPanel.ParseFont(properties.FontContainer.DefaultFont.ToString());
            helper = new FontSelectionPanelHelper(fontSizeComboBox, fontListComboBox, currentFont);

            fontListComboBox.MeasureItem += new System.Windows.Forms.MeasureItemEventHandler(helper.MeasureComboBoxItem);
            fontListComboBox.DrawItem += new System.Windows.Forms.DrawItemEventHandler(helper.ComboBoxDrawItem);

            UpdateFontPreviewLabel(null, null);
            helper.StartThread();
        }

        public override bool StorePanelContents()
        {
            CodeEditorProperties properties = CodeEditorProperties.Instance;

            Font currentFont = this.CurrentFont;
            if (currentFont != null)
            {
                properties.Font = currentFont;
            }

            IViewContent activeViewContent = WorkbenchSingleton.Workbench.ActiveViewContent;

            if (activeViewContent is ITextEditorControlProvider)
            {
                TextEditorControl textarea = ((ITextEditorControlProvider)activeViewContent).TextEditorControl;
                textarea.OptionsChanged();
            }

            return true;
        }

        private void UpdateFontPreviewLabel(object sender, EventArgs e)
        {
            helper.UpdateFontPreviewLabel(fontPreviewLabel);
        }
    }
}
