// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 2534 $</version>
// </file>

using System;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.TextEditor.Gui.OptionPanels
{
    public partial class CodeCompletionPanel : DialogPanel
    {
        public CodeCompletionPanel()
        {
            InitializeComponent();

            // Initialize the language-based resources
            InitializeResources();
        }

        public override void LoadPanelContents()
        {
            //SetupFromXmlStream(this.GetType().Assembly.GetManifestResourceStream(
            //    "ICSharpCode.SharpDevelop.Resources.CodeCompletionOptionPanel.xfrm"));

            EnableCodeCompletionSettingsGroupBox();
            codeCompletionEnabledCheckBox.CheckedChanged += delegate(object sender, EventArgs e)
            {
                EnableCodeCompletionSettingsGroupBox();
            };
            codeCompletionEnabledCheckBox.Checked = CodeCompletionOptions.EnableCodeCompletion;

            useDataUsageCacheCheckBox.CheckedChanged += delegate(object sender, EventArgs e)
            {
                dataUsageCacheLabel1.Enabled = useDataUsageCacheCheckBox.Checked;
                dataUsageCacheLabel2.Enabled = useDataUsageCacheCheckBox.Checked;
                dataUsageCacheItemCountNumericUpDown.Enabled = useDataUsageCacheCheckBox.Checked;
            };
            useDataUsageCacheCheckBox.Checked = CodeCompletionOptions.DataUsageCacheEnabled;

            dataUsageCacheItemCountNumericUpDown.Value = CodeCompletionOptions.DataUsageCacheItemCount;

            clearDataUseCacheButton.Click += delegate(object sender, EventArgs e)
            {
                ICSharpCode.SharpDevelop.TextEditor.Gui.CodeCompletionDataUsageCache.ResetCache();
            };

            useTooltipsCheckBox.CheckedChanged += delegate(object sender, EventArgs e)
            {
                useDebugTooltipsOnlyCheckBox.Enabled = useTooltipsCheckBox.Checked;
            };
            useTooltipsCheckBox.Checked = CodeCompletionOptions.TooltipsEnabled;

            useDebugTooltipsOnlyCheckBox.Checked = CodeCompletionOptions.TooltipsOnlyWhenDebugging;

            completeWhenTypingCheckBox.Checked = CodeCompletionOptions.CompleteWhenTyping;
            useKeywordCompletionCheckBox.Checked = CodeCompletionOptions.KeywordCompletionEnabled;

            useInsightCheckBox.CheckedChanged += delegate(object sender, EventArgs e)
            {
                refreshInsightOnCommaCheckBox.Enabled = useInsightCheckBox.Checked;
            };
            useInsightCheckBox.Checked = CodeCompletionOptions.InsightEnabled;

            refreshInsightOnCommaCheckBox.Checked = CodeCompletionOptions.InsightRefreshOnComma;
        }

        public override bool StorePanelContents()
        {
            CodeCompletionOptions.EnableCodeCompletion  = codeCompletionEnabledCheckBox.Checked;
            CodeCompletionOptions.DataUsageCacheEnabled = useDataUsageCacheCheckBox.Checked;
            CodeCompletionOptions.DataUsageCacheItemCount = (int)dataUsageCacheItemCountNumericUpDown.Value;
            CodeCompletionOptions.TooltipsEnabled = useTooltipsCheckBox.Checked;
            CodeCompletionOptions.TooltipsOnlyWhenDebugging = useDebugTooltipsOnlyCheckBox.Checked;
            CodeCompletionOptions.CompleteWhenTyping = completeWhenTypingCheckBox.Checked;
            CodeCompletionOptions.KeywordCompletionEnabled = useKeywordCompletionCheckBox.Checked;
            CodeCompletionOptions.InsightEnabled = useInsightCheckBox.Checked;
            CodeCompletionOptions.InsightRefreshOnComma = refreshInsightOnCommaCheckBox.Checked;
            
            return base.StorePanelContents();
        }

        private void EnableCodeCompletionSettingsGroupBox()
        {
            groupBox.Enabled = codeCompletionEnabledCheckBox.Checked;
        }
    }
}
