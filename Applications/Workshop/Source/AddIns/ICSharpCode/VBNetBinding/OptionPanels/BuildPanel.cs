// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 4210 $</version>
// </file>

using System;
using System.IO;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Project;
using ICSharpCode.SharpDevelop.Gui.OptionPanels;
using StringPair = System.Collections.Generic.KeyValuePair<System.String, System.String>;

namespace VBNetBinding.OptionPanels
{
    public partial class BuildPanel : ProjectDialogPanel
    {
        private DebugSymbolType selectedDebugInfo;
        private ConfigurationBinding debugInfoBinding;
        //protected ChooseStorageLocationButton advancedLocationButton;

        public BuildPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlResource("VBNetBinding.Resources.BuildOptions.xfrm");
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "VBNetBinding.Resources.BuildOptions.xfrm"));

            InitializeHelper();

            ConfigurationBinding b;

            b = helper.BindString(conditionalSymbolsTextBox, 
                "DefineConstants", TextBoxEditMode.EditRawProperty);
            b.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
            b.CreateLocationButton(conditionalSymbolsTextBox);

            b = helper.BindBoolean(optimizeCodeCheckBox, "Optimize", false);
            b.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
            b.CreateLocationButton(optimizeCodeCheckBox);

            //b = helper.BindBoolean(removeOverflowCheckBox, "RemoveIntegerChecks", false);
            //b.CreateLocationButton(removeOverflowCheckBox);

            ChooseStorageLocationButton locationButton;
            b = helper.BindStringEnum(optionExplicitComboBox, "OptionExplicit", "On",
                                      new StringPair("Off", "Explicit Off"),
                                      new StringPair("On", "Explicit On"));
            locationButton = b.CreateLocationButton(optionExplicitComboBox);
            b = helper.BindStringEnum(optionStrictComboBox, "OptionStrict", "Off",
                                      new StringPair("Off", "Strict Off"),
                                      new StringPair("On", "Strict On"));
            b.RegisterLocationButton(locationButton);
            b = helper.BindStringEnum(optionCompareComboBox, "OptionCompare", "Binary",
                                      new StringPair("Binary", "Compare Binary"),
                                      new StringPair("Text", "Compare Text"));
            b.RegisterLocationButton(locationButton);

            b = helper.BindStringEnum(optionInferComboBox, "OptionInfer", "Off",
                                      new StringPair("Off", "Infer Off"),
                                      new StringPair("On", "Infer On"));

            b.RegisterLocationButton(locationButton);

            //InitBaseIntermediateOutputPath();
            //InitIntermediateOutputPath();
            InitOutputPath();
            InitXmlDoc();
            //InitTargetFramework();

            InitDebugInfo();
            InitAdvanced();
            InitWarnings();

            helper.AddConfigurationSelector(this);
        }

        //protected void InitBaseIntermediateOutputPath()
        //{
        //    ConfigurationBinding binding = helper.BindString(
        //        baseIntermediateOutputPathTextBox,
        //                      "BaseIntermediateOutputPath",
        //                      TextBoxEditMode.EditRawProperty,
        //                      delegate
        //                      {
        //                          return @"obj\";
        //                      }
        //                     );
        //    binding.CreateLocationButton(baseIntermediateOutputPathTextBox);
        //    ConnectBrowseFolder(baseIntermediateOutputPathBrowseButton,
        //        baseIntermediateOutputPathTextBox,
        //        "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}",
        //                        TextBoxEditMode.EditRawProperty);
        //}

        //protected void InitIntermediateOutputPath()
        //{
        //    ConfigurationBinding binding = helper.BindString(intermediateOutputPathTextBox,
        //        "IntermediateOutputPath", TextBoxEditMode.EditRawProperty,
        //        delegate
        //        {
        //            return Path.Combine(helper.GetProperty("BaseIntermediateOutputPath", @"obj\", true),
        //                                helper.Configuration);
        //        }
        //    );
        //    binding.DefaultLocation = PropertyStorageLocations.ConfigurationSpecific;
        //    binding.CreateLocationButton(intermediateOutputPathTextBox);
        //    ConnectBrowseFolder(intermediateOutputPathBrowseButton,
        //                        intermediateOutputPathTextBox,
        //                        "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}",
        //                        TextBoxEditMode.EditRawProperty);
        //}

        protected void InitOutputPath()
        {
            ConfigurationBinding binding = helper.BindString(
                outputPathTextBox, "OutputPath", TextBoxEditMode.EditRawProperty);
            binding.CreateLocationButton(outputPathTextBox);
            ConnectBrowseFolder(outputPathBrowseButton, outputPathTextBox,
                "${res:Dialog.Options.PrjOptions.Configuration.FolderBrowserDescription}",
                TextBoxEditMode.EditRawProperty);
        }

        protected void InitXmlDoc()
        {
            ConfigurationBinding b;
            b = helper.BindString(xmlDocumentationTextBox, "DocumentationFile", TextBoxEditMode.EditRawProperty);
            b.CreateLocationButton(xmlDocumentationCheckBox);
            helper.Loaded += XmlDocHelperLoaded;
            XmlDocHelperLoaded(null, null);
        }

        private void XmlDocHelperLoaded(object sender, EventArgs e)
        {
            xmlDocumentationCheckBox.CheckedChanged -= UpdateXmlEnabled;
            xmlDocumentationCheckBox.Checked = xmlDocumentationTextBox.Text.Length > 0;
            xmlDocumentationCheckBox.CheckedChanged += UpdateXmlEnabled;
            xmlDocumentationTextBox.Enabled = xmlDocumentationCheckBox.Checked;
        }

        private void UpdateXmlEnabled(object sender, EventArgs e)
        {
            xmlDocumentationTextBox.Enabled = xmlDocumentationCheckBox.Checked;
            if (xmlDocumentationCheckBox.Checked)
            {
                if (xmlDocumentationTextBox.Text.Length == 0)
                {
                    xmlDocumentationTextBox.Text = MSBuildInternals.Escape(
                        Path.ChangeExtension(FileUtility.GetRelativePath(baseDirectory, 
                        project.OutputAssemblyFullPath), ".xml"));
                }
            }
            else
            {
                xmlDocumentationTextBox.Text = "";
            }
        }

        protected void InitWarnings()
        {
            ConfigurationBinding b;
            b = helper.BindStringEnum(warningLevelComboBox, "WarningLevel",
                                      "4",
                                      new StringPair("0", "0"),
                                      new StringPair("1", "1"),
                                      new StringPair("2", "2"),
                                      new StringPair("3", "3"),
                                      new StringPair("4", "4"));
            ChooseStorageLocationButton locationButton =
                b.CreateLocationButtonInPanel(warningsGroupBox);
            b = helper.BindString(suppressWarningsTextBox,
                "NoWarn", TextBoxEditMode.EditEvaluatedProperty);
            b.RegisterLocationButton(locationButton);

            b = new WarningsAsErrorsBinding(this);
            helper.AddBinding("TreatWarningsAsErrors", b);
            locationButton = b.CreateLocationButtonInPanel(
                treatWarningsAsErrorsGroupBox);
            b = helper.BindString(specificWarningsTextBox, "WarningsAsErrors", TextBoxEditMode.EditEvaluatedProperty); // must be saved AFTER TreatWarningsAsErrors
            b.RegisterLocationButton(locationButton);

            EventHandler setDirty = delegate
            {
                helper.IsDirty = true;
            };
            noneRadioButton.CheckedChanged += setDirty;
            specificWarningsRadioButton.CheckedChanged += setDirty;
            allRadioButton.CheckedChanged += setDirty;

            specificWarningsRadioButton.CheckedChanged += new EventHandler(UpdateWarningChecked);

            UpdateWarningChecked(this, EventArgs.Empty);
        }

        private void UpdateWarningChecked(object sender, EventArgs e)
        {
            specificWarningsTextBox.Enabled = specificWarningsRadioButton.Checked;
        }

        private sealed class WarningsAsErrorsBinding : ConfigurationBinding
        {
            RadioButton none, specific, all;
            Control specificWarningsTextBox;

            public WarningsAsErrorsBinding(BuildPanel panel)
            {
                this.none = panel.noneRadioButton;
                this.specific = panel.specificWarningsRadioButton;
                this.all = panel.allRadioButton;
                this.specificWarningsTextBox = panel.specificWarningsTextBox;
            }

            public override void Load()
            {
                if (bool.Parse(Get("false")))
                {
                    all.Checked = true;
                }
                else
                {
                    if (this.Helper.GetProperty("WarningsAsErrors", "", true).Length > 0)
                    {
                        specific.Checked = true;
                    }
                    else
                    {
                        none.Checked = true;
                    }
                }
            }

            public override bool Save()
            {
                if (none.Checked)
                {
                    specificWarningsTextBox.Text = "";
                }
                if (all.Checked)
                {
                    Set("true");
                }
                else
                {
                    Set("false");
                }
                return true;
            }
        }

        protected void InitDebugInfo()
        {
            //debugInfoBinding = helper.BindEnum<DebugSymbolType>(debugInfoComboBox,
            //    "DebugType");
            //debugInfoBinding.CreateLocationButton(debugInfoLabel);
            debugInfoBinding = helper.BindEnum<DebugSymbolType>(null,
                "DebugType");
            DebugSymbolsLoaded(null, null);
            helper.Loaded += DebugSymbolsLoaded;
            helper.Saved += DebugSymbolsSave;
        }

        protected void InitAdvanced()
        {
            ConfigurationBinding b;
            b = helper.BindBoolean(registerCOMInteropCheckBox, "RegisterForComInterop", false);
            b.DefaultLocation = PropertyStorageLocations.PlatformSpecific;
            //advancedLocationButton = b.CreateLocationButtonInPanel(platformSpecificOptionsPanel);

            b = helper.BindStringEnum(generateSerializationAssemblyComboBox, "GenerateSerializationAssemblies",
                                      "Auto",
                                      new StringPair("Off", "${res:Dialog.ProjectOptions.Build.Off}"),
                                      new StringPair("On", "${res:Dialog.ProjectOptions.Build.On}"),
                                      new StringPair("Auto", "${res:Dialog.ProjectOptions.Build.Auto}"));
            b.DefaultLocation = PropertyStorageLocations.PlatformSpecific;
            //b.RegisterLocationButton(advancedLocationButton);

            //b = helper.BindHexadecimal(dllBaseAddressTextBox, "BaseAddress", 0x400000);
            //b.DefaultLocation = PropertyStorageLocations.PlatformSpecific;
            //b.RegisterLocationButton(advancedLocationButton);

            b = CreatePlatformTarget();
            //b = helper.BindStringEnum(targetCpuComboBox, "PlatformTarget",
            //                          "AnyCPU",
            //                          new StringPair("AnyCPU", "${res:Dialog.ProjectOptions.Build.TargetCPU.Any}"),
            //                          new StringPair("x86", "${res:Dialog.ProjectOptions.Build.TargetCPU.x86}"),
            //                          new StringPair("x64", "${res:Dialog.ProjectOptions.Build.TargetCPU.x64}"),
            //                          new StringPair("Itanium", "${res:Dialog.ProjectOptions.Build.TargetCPU.Itanium}"));
            //b.DefaultLocation = PropertyStorageLocations.PlatformSpecific;
            //b.RegisterLocationButton(advancedLocationButton);
        }

        protected ConfigurationBinding CreatePlatformTarget()
        {
            ConfigurationBinding b;
            b = helper.BindStringEnum(targetCpuComboBox, "PlatformTarget",
                                      "AnyCPU",
                                      new StringPair("AnyCPU", "${res:Dialog.ProjectOptions.Build.TargetCPU.Any}"),
                                      new StringPair("x86", "${res:Dialog.ProjectOptions.Build.TargetCPU.x86}"),
                                      new StringPair("x64", "${res:Dialog.ProjectOptions.Build.TargetCPU.x64}"),
                                      new StringPair("Itanium", "${res:Dialog.ProjectOptions.Build.TargetCPU.Itanium}"));
            b.DefaultLocation = PropertyStorageLocations.PlatformSpecific;
            return b;
        }

        private void DebugSymbolsLoaded(object sender, EventArgs e)
        {
            PropertyStorageLocations location;
            helper.GetProperty("DebugType", "", true, out location);
            if (location == PropertyStorageLocations.Unknown)
            {
                bool debug = helper.GetProperty("DebugSymbols", false, true, out location);
                if (location != PropertyStorageLocations.Unknown)
                {
                    debugInfoBinding.Location = location;
                    helper.SetProperty("DebugType", debug ? DebugSymbolType.Full : DebugSymbolType.None, true, location);
                    debugInfoBinding.Load();
                }
            }
        }

        private void DebugSymbolsSave(object sender, EventArgs e)
        {
            if (selectedDebugInfo == DebugSymbolType.Full)
            {
                helper.SetProperty("DebugSymbols", "true", true, debugInfoBinding.Location);
            }
            else
            {
                helper.SetProperty("DebugSymbols", "false", true, debugInfoBinding.Location);
            }
        }

        private void OnAdvancedSettings(object sender, EventArgs e)
        {
            AdvancedBuildSettings dlg = new AdvancedBuildSettings(this.helper);
            dlg.BaseDirectory = this.baseDirectory;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                selectedDebugInfo = dlg.SelectedDebugInfo;
            }
        }
    }
}
