// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3794 $</version>
// </file>

using System;
using System.Drawing;
using System.Windows.Forms;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.OptionPanels
{
    public partial class SelectCulturePanel : DialogPanel
    {
        #region Private Fields

        private const string uiLanguageProperty = "CoreProperties.UILanguage";

        #endregion

        #region Constructors and Destructor

        public SelectCulturePanel()
        {
            InitializeComponent();

            Initialize();
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                    components = null;
                }
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Properties

        private string SelectedCulture
        {
			get {
				if (listView.SelectedItems.Count > 0) {
					return listView.SelectedItems[0].SubItems[1].Text;
				}
				return null;
			}
		}

        private string SelectedCountry
        {
			get {
				if (listView.SelectedItems.Count > 0) {
					return listView.SelectedItems[0].Text;
				}
				return null;
			}
        }

        #endregion

        #region Public Methods

        public override bool ReceiveDialogMessage(DialogMessage message)
		{
			if (message == DialogMessage.OK) {
				if (SelectedCulture != null) {
                    PropertyService.Set(uiLanguageProperty, SelectedCulture);
				}
			}
			return true;
        }

        #endregion

        #region Private Methods

        private void Initialize()
        {
            this.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.LargeImageList = LanguageService.LanguageImageList;

            foreach (Language language in LanguageService.Languages)
            {
                listView.Items.Add(new ListViewItem(
                    new string[] { language.Name, language.Code }, 
                    language.ImageIndex));
            }

            this.newCulture.Text = String.Empty;
            this.culture.Text = ResourceService.GetString("Dialog.Options.IDEOptions.SelectCulture.CurrentUILanguageLabel") + " " + GetCulture(PropertyService.Get(uiLanguageProperty, "en"));
            this.groupBox.Text = ResourceService.GetString("Dialog.Options.IDEOptions.SelectCulture.DescriptionText");
        }

        private void ChangeCulture(object sender, EventArgs e)
		{
			newCulture.Text = ResourceService.GetString(
                "Dialog.Options.IDEOptions.SelectCulture.UILanguageSetToLabel") + " " + SelectedCountry;
		}

        private string GetCulture(string languageCode)
		{
			foreach (Language language in LanguageService.Languages) {
				if (languageCode.StartsWith(language.Code)) {
					return language.Name;
				}
			}
			return "English";
		}

        #endregion
    }
}
