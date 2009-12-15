// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.Text;
using System.Drawing;
using System.Drawing.Text;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Gui
{
    public class FontSelectionPanelHelper
    {
        ComboBox fontSizeComboBox, fontListComboBox;
        Font defaultFont;

        public FontSelectionPanelHelper(ComboBox fontSizeComboBox, ComboBox fontListComboBox, Font defaultFont)
        {
            this.fontSizeComboBox = fontSizeComboBox;
            this.fontListComboBox = fontListComboBox;
            this.defaultFont = defaultFont;
            boldComboBoxFont = new Font(fontListComboBox.Font, FontStyle.Bold);
        }

        public void StartThread()
        {
            Thread thread = new Thread(DetectMonospacedThread);
            thread.IsBackground = true;
            thread.Start();
        }

        void DetectMonospacedThread()
        {
            Thread.Sleep(0); // first allow UI thread to do some work
            DebugTimer.Start();
            InstalledFontCollection installedFontCollection = new InstalledFontCollection();
            Font currentFont = defaultFont;
            List<FontDescriptor> fonts = new List<FontDescriptor>();

            int index = 0;
            foreach (FontFamily fontFamily in installedFontCollection.Families)
            {
                if (fontFamily.IsStyleAvailable(FontStyle.Regular) && fontFamily.IsStyleAvailable(FontStyle.Bold) && fontFamily.IsStyleAvailable(FontStyle.Italic))
                {
                    if (fontFamily.Name == currentFont.Name)
                    {
                        index = fonts.Count;
                    }
                    fonts.Add(new FontDescriptor(fontFamily));
                }
            }
            DebugTimer.Stop("Getting installed fonts");
            WorkbenchSingleton.SafeThreadAsyncCall(
                delegate
                {
                    fontListComboBox.Items.AddRange(fonts.ToArray());
                    fontSizeComboBox.Enabled = true;
                    fontListComboBox.Enabled = true;
                    fontListComboBox.SelectedIndex = index;
                    fontSizeComboBox.Text = currentFont.Size.ToString();
                });
            DebugTimer.Start();
            using (Bitmap newBitmap = new Bitmap(1, 1))
            {
                using (Graphics g = Graphics.FromImage(newBitmap))
                {
                    foreach (FontDescriptor fd in fonts)
                    {
                        fd.DetectMonospaced(g);
                    }
                }
            }
            DebugTimer.Stop("Detect Monospaced");
            fontListComboBox.Invalidate();
        }

        public void MeasureComboBoxItem(object sender, System.Windows.Forms.MeasureItemEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            if (e.Index >= 0)
            {
                FontDescriptor fontDescriptor = (FontDescriptor)comboBox.Items[e.Index];
                SizeF size = e.Graphics.MeasureString(fontDescriptor.Name, comboBox.Font);
                e.ItemWidth = (int)size.Width;
                e.ItemHeight = (int)comboBox.Font.Height;
            }
        }

        static StringFormat drawStringFormat = new StringFormat(StringFormatFlags.NoWrap);
        Font boldComboBoxFont;

        public void ComboBoxDrawItem(object sender, System.Windows.Forms.DrawItemEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            e.DrawBackground();

            Rectangle drawingRect = new Rectangle(e.Bounds.X,
                                                  e.Bounds.Y,
                                                  e.Bounds.Width,
                                                  e.Bounds.Height);

            Brush drawItemBrush = SystemBrushes.WindowText;
            if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
            {
                drawItemBrush = SystemBrushes.HighlightText;
            }

            if (comboBox.Enabled == false)
            {
                e.Graphics.DrawString(ResourceService.GetString("ICSharpCode.SharpDevelop.Gui.Pads.ClassScout.LoadingNode"),
                                      comboBox.Font,
                                      drawItemBrush,
                                      drawingRect,
                                      drawStringFormat);
            }
            else if (e.Index >= 0)
            {
                FontDescriptor fontDescriptor = (FontDescriptor)comboBox.Items[e.Index];
                e.Graphics.DrawString(fontDescriptor.Name,
                                      fontDescriptor.IsMonospaced ? boldComboBoxFont : comboBox.Font,
                                      drawItemBrush,
                                      drawingRect,
                                      drawStringFormat);
            }
            e.DrawFocusRectangle();
        }

        public Font GetSelectedFont()
        {
            if (!fontListComboBox.Enabled)
                return null;
            float fontSize = 10f;
            try
            {
                fontSize = Math.Max(6, Single.Parse(fontSizeComboBox.Text));
            }
            catch (Exception)
            {
            }

            FontDescriptor fontDescriptor = (FontDescriptor)fontListComboBox.Items[fontListComboBox.SelectedIndex];

            return new Font(fontDescriptor.Name,
                            fontSize);
        }

        public void UpdateFontPreviewLabel(Control fontPreviewLabel)
        {
            Font currentFont = GetSelectedFont();
            fontPreviewLabel.Visible = currentFont != null;
            if (currentFont != null)
            {
                fontPreviewLabel.Font = currentFont;
            }
        }

        public class FontDescriptor
        {
            FontFamily fontFamily;
            internal string Name;
            internal bool IsMonospaced;

            public FontDescriptor(FontFamily fontFamily)
            {
                this.fontFamily = fontFamily;
                this.Name = fontFamily.Name;
            }

            internal void DetectMonospaced(Graphics g)
            {
                this.IsMonospaced = DetectMonospaced(g, fontFamily);
            }

            static bool DetectMonospaced(Graphics g, FontFamily fontFamily)
            {
                using (Font f = new Font(fontFamily, 10))
                {
                    // determine if the length of i == m because I see no other way of
                    // getting if a font is monospaced or not.
                    int w1 = TextRenderer.MeasureText("i.", f).Width;
                    int w2 = TextRenderer.MeasureText("mw", f).Width;
                    return w1 == w2;
                }
            }
        }
    }
}
