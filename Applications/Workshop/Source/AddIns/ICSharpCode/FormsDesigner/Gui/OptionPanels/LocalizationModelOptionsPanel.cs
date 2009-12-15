// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Christian Hornung" email="chhornung@googlemail.com"/>
//     <version>$Revision$</version>
// </file>

using System;
using System.Windows.Forms;
using System.ComponentModel.Design.Serialization;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.FormsDesigner.Gui.OptionPanels
{
	/// <summary>
	/// Options panel for localization model options.
	/// </summary>
	public partial class LocalizationModelOptionsPanel : DialogPanel
	{
        public const string DefaultLocalizationModelPropertyName = "FormsDesigner.DesignerOptions.DefaultLocalizationModel";
        public const string KeepLocalizationModelPropertyName = "FormsDesigner.DesignerOptions.KeepLocalizationModel";

        const CodeDomLocalizationModel DefaultLocalizationModelDefaultValue = CodeDomLocalizationModel.PropertyReflection;
        const bool KeepLocalizationModelDefaultValue = false;
		
		public LocalizationModelOptionsPanel()
		{
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }
		
		public override void LoadPanelContents()
		{
			base.LoadPanelContents();

            //if (this.Controls.Count == 0) 
            //{
            //    Translate(this);
            //}
			
			this.reflectionRadioButton.Checked = 
                (DefaultLocalizationModel == CodeDomLocalizationModel.PropertyReflection);
			this.assignmentRadioButton.Checked = !this.reflectionRadioButton.Checked;
			this.keepModelCheckBox.Checked = KeepLocalizationModel;
		}
		
		static void Translate(Control container) {
			container.Text = StringParser.Parse(container.Text);
			foreach (Control c in container.Controls) {
				Translate(c);
			}
		}
		
		public override bool StorePanelContents()
		{
			if (this.reflectionRadioButton.Checked) {
				DefaultLocalizationModel = CodeDomLocalizationModel.PropertyReflection;
			} else if (this.assignmentRadioButton.Checked) {
				DefaultLocalizationModel = CodeDomLocalizationModel.PropertyAssignment;
			} else {
				MessageService.ShowError("One localization model must be selected!");
				return false;
			}
			
			KeepLocalizationModel = this.keepModelCheckBox.Checked;
			
			return true;
		}
		
		
		/// <summary>
		/// Gets or sets the default localization model to be used by the Windows Forms designer.
		/// </summary>
		public static CodeDomLocalizationModel DefaultLocalizationModel {
			get { return GetPropertySafe(DefaultLocalizationModelPropertyName, DefaultLocalizationModelDefaultValue); }
			set { PropertyService.Set(DefaultLocalizationModelPropertyName, value); }
		}
		
		/// <summary>
		/// Gets or sets whether the Windows Forms designer should keep the localization model of existing files.
		/// </summary>
		public static bool KeepLocalizationModel {
			get { return GetPropertySafe(KeepLocalizationModelPropertyName, KeepLocalizationModelDefaultValue); }
			set { PropertyService.Set(KeepLocalizationModelPropertyName, value); }
		}
		
		static T GetPropertySafe<T>(string name, T defaultValue)
		{
			if (PropertyService.Initialized) {
				return PropertyService.Get<T>(name, defaultValue);
			} else {
				return defaultValue;
			}
		}
	}
}
