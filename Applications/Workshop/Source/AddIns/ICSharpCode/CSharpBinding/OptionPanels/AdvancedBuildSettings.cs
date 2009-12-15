// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2739 $</version>
// </file>

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Globalization;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using MSBuild = Microsoft.Build.BuildEngine;
using StringPair = System.Collections.Generic.KeyValuePair<System.String, System.String>;

namespace CSharpBinding.OptionPanels
{
    public partial class AdvancedBuildSettings : AdvancedBuildSettingsBase
    {
        private bool cancelFormClosing;
        private DebugSymbolType selectedDebugInfo;

        private ConfigurationBinding checkOverflow;
        private ConfigurationBinding noCorlib;
        private ConfigurationBinding dllBaseAddress;
        private ConfigurationBinding baseOutputPath;
        private ConfigurationBinding outputPath;
        private ConfigurationBinding debugInfoBinding;
        private ConfigurationBinding fileAlignment;

        private ConfigurationHelper helper;

        public AdvancedBuildSettings()
        {
            InitializeComponent();

            foreach (Control control in Controls.GetRecursive())
            {
                control.Text = StringParser.Parse(control.Text);
            }
            this.Text = StringParser.Parse(this.Text);
        }

        public AdvancedBuildSettings(ConfigurationHelper helper)
            : this()
        {
            this.helper = helper;
        }

        public DebugSymbolType SelectedDebugInfo
        {
            get
            {     
                return selectedDebugInfo;
            }
        }

        private void OnFormLoad(object sender, EventArgs e)
        {
            checkOverflow = helper.BindBoolean(checkForOverflowCheckBox,
                "CheckForOverflowUnderflow", false);
            checkOverflow.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
            //checkOverflow.CreateLocationButton(checkForOverflowCheckBox);

            noCorlib = helper.BindBoolean(noCorlibCheckBox, "NoStdLib", false);
            //noCorlib.CreateLocationButton(noCorlibCheckBox);

            debugInfoBinding = helper.BindEnum<DebugSymbolType>(debugInfoComboBox,
                "DebugType");
            //debugInfoBinding.CreateLocationButton(debugInfoLabel);
            //DebugSymbolsLoaded(null, null);
            //helper.Loaded += DebugSymbolsLoaded;

            fileAlignment = helper.BindStringEnum(fileAlignmentComboBox, 
                                      "FileAlignment", "4096",
                                      new StringPair("512", "512"),
                                      new StringPair("1024", "1024"),
                                      new StringPair("2048", "2048"),
                                      new StringPair("4096", "4096"),
                                      new StringPair("8192", "8192"));
            fileAlignment.DefaultLocation = PropertyStorageLocations.PlatformSpecific;
            //fileAlignment.RegisterLocationButton(advancedLocationButton);


            dllBaseAddress = helper.BindHexadecimal(dllBaseAddressTextBox, "BaseAddress", 0x400000);
            dllBaseAddress.DefaultLocation = PropertyStorageLocations.PlatformSpecific;
            //dllBaseAddress.RegisterLocationButton(advancedLocationButton);

            baseOutputPath = helper.BindString(baseIntermediateOutputPathTextBox,
                              "BaseIntermediateOutputPath",
                              TextBoxEditMode.EditRawProperty,
                              delegate
                              {
                                  return @"obj\";
                              }
                             );
            //baseOutputPath.CreateLocationButton(baseIntermediateOutputPathTextBox);
            ConnectBrowseFolder(baseIntermediateOutputPathBrowseButton,
                baseIntermediateOutputPathTextBox,
                "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}",
                                TextBoxEditMode.EditRawProperty);

            outputPath = helper.BindString(intermediateOutputPathTextBox,
                "IntermediateOutputPath", TextBoxEditMode.EditRawProperty,
                delegate
                {
                    return Path.Combine(helper.GetProperty("BaseIntermediateOutputPath", @"obj\", true),
                                        helper.Configuration);
                }
            );
            outputPath.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
            //outputPath.CreateLocationButton(intermediateOutputPathTextBox);
            ConnectBrowseFolder(intermediateOutputPathBrowseButton,
                                intermediateOutputPathTextBox,
                                "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}",
                                TextBoxEditMode.EditRawProperty);

        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = cancelFormClosing;
        }

        private void OnAcceptSettings(object sender, EventArgs e)
        {
            cancelFormClosing = false;

            if (dllBaseAddress != null)
            {
                string textValue = dllBaseAddressTextBox.Text.Trim();
                NumberStyles style = NumberStyles.Integer;
                if (textValue.StartsWith("0x"))
                {
                    textValue = textValue.Substring(2);
                    style = NumberStyles.HexNumber;
                }

                int intValue;
                if (!Int32.TryParse(textValue, style,
                    NumberFormatInfo.InvariantInfo, out intValue))
                {
                    MessageService.ShowMessage(
                        "${res:Dialog.ProjectOptions.PleaseEnterValidNumber}");

                    cancelFormClosing = true;

                    return;
                }
                dllBaseAddress.Value = dllBaseAddressTextBox.Text;
            }

            if (checkOverflow != null)
            {
                checkOverflow.Value = checkForOverflowCheckBox.Checked ? "true" : "false";
            }
            if (noCorlib != null)
            {
                noCorlib.Value = noCorlibCheckBox.Checked ? "true" : "false";
            }
            if (baseOutputPath != null)
            {
                baseOutputPath.Value = baseIntermediateOutputPathTextBox.Text;
            }
            if (outputPath != null)
            {
                outputPath.Value = intermediateOutputPathTextBox.Text;
            }
            if (debugInfoBinding != null)
            {
                debugInfoBinding.Value = debugInfoComboBox.SelectedIndex.ToString();
            }
            if (fileAlignment != null)
            {
                fileAlignment.Value = fileAlignmentComboBox.SelectedIndex.ToString();
            }

            selectedDebugInfo = (DebugSymbolType)debugInfoComboBox.SelectedIndex;
        }

        private void OnRejectSettings(object sender, EventArgs e)
        {
            cancelFormClosing = false;
        }
    }
}
