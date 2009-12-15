// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
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

namespace ICSharpCode.FormsDesigner.Gui.OptionPanels
{
    public partial class GeneralOptionsPanel : DialogPanel
    {
        public GeneralOptionsPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public static bool UseSmartTags
        {
            get
            {
                return PropertyService.Get(
                    "FormsDesigner.DesignerOptions.UseSmartTags", true);
            }
            set
            {
                PropertyService.Set(
                    "FormsDesigner.DesignerOptions.UseSmartTags", value);
            }
        }

        public static bool SmartTagAutoShow
        {
            get
            {
                return PropertyService.Get(
                    "FormsDesigner.DesignerOptions.ObjectBoundSmartTagAutoShow", true);
            }
            set
            {
                PropertyService.Set(
                    "FormsDesigner.DesignerOptions.ObjectBoundSmartTagAutoShow", value);
            }
        }

        public static bool InsertTodoComment
        {
            get
            {
                return PropertyService.Get(
                    "FormsDesigner.DesignerOptions.InsertTodoComment", false);
            }
            set
            {
                PropertyService.Set(
                    "FormsDesigner.DesignerOptions.InsertTodoComment", value);
            }
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.FormsDesigner.Resources.WindowsFormsGeneralOptions.xfrm"));

            sortAlphabeticalCheckBox.Checked = PropertyService.Get(
                "FormsDesigner.DesignerOptions.PropertyGridSortAlphabetical", false);
            optimizedCodeGenerationCheckBox.Checked = PropertyService.Get(
                "FormsDesigner.DesignerOptions.UseOptimizedCodeGeneration", true);
            smartTagAutoShowCheckBox.Checked = SmartTagAutoShow;
            inPlaceEditCheckBox.Checked = PropertyService.Get(
                "FormsDesigner.DesignerOptions.EnableInSituEditing", true);
            useSmartTagsCheckBox.Checked = UseSmartTags;
            insertTodoCommentCheckBox.Checked = InsertTodoComment;

            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.FormsDesigner.Resources.WindowsFormsGridOptions.xfrm"));

            bool snapToGridOn = PropertyService.Get(
                "FormsDesigner.DesignerOptions.SnapToGridMode", false);

            snapToGridRadioButton.Checked = snapToGridOn;
            snapLinesRadioButton.Checked = !snapToGridOn;
            widthTextBox.Text = PropertyService.Get(
                "FormsDesigner.DesignerOptions.GridSizeWidth", 8).ToString();
            heightTextBox.Text = PropertyService.Get(
                "FormsDesigner.DesignerOptions.GridSizeHeight", 8).ToString();
            showGridCheckBox.Checked = PropertyService.Get(
                "FormsDesigner.DesignerOptions.ShowGrid", true);
            snapToGridCheckBox.Checked = PropertyService.Get(
                "FormsDesigner.DesignerOptions.SnapToGrid", true);

            snapToGridRadioButton.CheckedChanged += SnapToGridChanged;
            EnableGridOptions(snapToGridOn);
        }

        public override bool StorePanelContents()
        {
            PropertyService.Set(
                "FormsDesigner.DesignerOptions.PropertyGridSortAlphabetical", 
                sortAlphabeticalCheckBox.Checked);
            PropertyService.Set(
                "FormsDesigner.DesignerOptions.UseOptimizedCodeGeneration", 
                optimizedCodeGenerationCheckBox.Checked);
            SmartTagAutoShow = smartTagAutoShowCheckBox.Checked;
            PropertyService.Set("FormsDesigner.DesignerOptions.EnableInSituEditing", 
                inPlaceEditCheckBox.Checked);
            UseSmartTags = useSmartTagsCheckBox.Checked;
            InsertTodoComment = insertTodoCommentCheckBox.Checked;

            int width = 0;
            try
            {
                width = Int32.Parse(widthTextBox.Text);
            }
            catch
            {
                MessageService.ShowError("Forms Designer grid width is invalid");
                return false;
            }

            int height = 0;
            try
            {
                height = Int32.Parse(heightTextBox.Text);
            }
            catch
            {
                MessageService.ShowError("Forms Designer grid height is invalid");
                return false;
            }

            PropertyService.Set("FormsDesigner.DesignerOptions.SnapToGridMode", 
                snapToGridRadioButton.Checked);
            PropertyService.Set("FormsDesigner.DesignerOptions.GridSizeWidth", width);
            PropertyService.Set("FormsDesigner.DesignerOptions.GridSizeHeight", height);
            PropertyService.Set("FormsDesigner.DesignerOptions.UseSnapLines", 
                snapLinesRadioButton.Checked);
            PropertyService.Set("FormsDesigner.DesignerOptions.ShowGrid", 
                showGridCheckBox.Checked);
            PropertyService.Set("FormsDesigner.DesignerOptions.SnapToGrid", 
                snapToGridCheckBox.Checked);

            return true;
        }

        private void EnableGridOptions(bool snapToGridOn)
        {
            widthTextBox.Enabled = snapToGridOn;
            heightTextBox.Enabled = snapToGridOn;
            showGridCheckBox.Enabled = snapToGridOn;
            snapToGridCheckBox.Enabled = snapToGridOn;
        }

        private void SnapToGridChanged(object source, EventArgs e)
        {
            EnableGridOptions(snapToGridRadioButton.Checked);
        }
    }
}
