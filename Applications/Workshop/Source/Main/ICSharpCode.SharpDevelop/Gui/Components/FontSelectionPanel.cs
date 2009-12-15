// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.Data;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Gui
{
    public partial class FontSelectionPanel : UserControl
    {
        private FontSelectionPanelHelper helper;

        public FontSelectionPanel()
        {
            InitializeComponent();

            foreach (Control ctl in this.Controls)
            {
                ctl.Text = StringParser.Parse(ctl.Text);
            }

            for (int i = 6; i <= 24; ++i)
            {
                fontSizeComboBox.Items.Add(i);
            }
            fontSizeComboBox.TextChanged += new EventHandler(UpdateFontPreviewLabel);
            fontSizeComboBox.Enabled = false;
            fontListComboBox.Enabled = false;

            fontListComboBox.TextChanged += new EventHandler(UpdateFontPreviewLabel);
            fontListComboBox.SelectedIndexChanged += new EventHandler(UpdateFontPreviewLabel);
        }

        [Browsable(false)]
        public string CurrentFontString
        {
            get
            {
                Font font = CurrentFont;
                if (font != null)
                    return font.ToString();
                else
                    return null;
            }
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                CurrentFont = FontSelectionPanel.ParseFont(value);
            }
        }

        [Browsable(false)]
        public Font CurrentFont
        {
            get
            {
                if (helper == null)
                    return null;
                return helper.GetSelectedFont();
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                if (helper == null)
                {
                    helper = new FontSelectionPanelHelper(fontSizeComboBox,
                        fontListComboBox, value);
                    helper.StartThread();
                    fontListComboBox.MeasureItem += helper.MeasureComboBoxItem;
                    fontListComboBox.DrawItem += helper.ComboBoxDrawItem;
                }
                else
                {
                    int index = 0;
                    for (int i = 0; i < fontListComboBox.Items.Count; ++i)
                    {
                        FontSelectionPanelHelper.FontDescriptor descriptor = 
                            (FontSelectionPanelHelper.FontDescriptor)fontListComboBox.Items[i];
                        if (descriptor.Name == value.Name)
                        {
                            index = i;
                        }
                    }
                    fontListComboBox.SelectedIndex = index;
                }
                fontSizeComboBox.Text = value.Size.ToString();
            }
        }

        public static Font ParseFont(string font)
        {
            if (String.IsNullOrEmpty(font))
            {
                return null;
            }

            try
            {
                string[] descr = font.Split(new char[] { ',', '=' });
                return new Font(descr[1], Single.Parse(descr[3]));
            }
            catch (Exception ex)
            {
                LoggingService.Warn(ex);
                return WinFormsResourceService.DefaultMonospacedFont;
            }
        }

        private void UpdateFontPreviewLabel(object sender, EventArgs e)
        {
            helper.UpdateFontPreviewLabel(fontPreviewLabel);
        }
		
    }
}
