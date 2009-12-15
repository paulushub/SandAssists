// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 3791 $</version>
// </file>

using System;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.SharpDevelop.Project
{
    /// <summary>
    /// Class that helps connecting configuration GUI controls to MsBuild properties.
    /// </summary>
    public class ConfigurationHelper : ICanBeDirty
    {
        private MSBuildBasedProject project;
        private List<ConfigurationBinding> bindings;
        private Dictionary<string, int> bindingMaps;

        private ConfigurationHelper()
        {
            bindings    = new List<ConfigurationBinding>();
            bindingMaps = new Dictionary<string, int>(
                StringComparer.CurrentCultureIgnoreCase);
        }

        public ConfigurationHelper(MSBuildBasedProject project)
            : this()
        {
            if (project == null)
            {
                throw new ArgumentNullException("project",
                    "The parameter project cannot be null (or Nothing).");
            }

            this.project       = project;
            this.platform      = project.ActivePlatform;
            this.configuration = project.ActiveConfiguration;
        }

        public MSBuildBasedProject Project
        {
            get
            {
                return project;
            }
        }

        #region Get/Set Properties Methods

        public T GetProperty<T>(string propertyName, T defaultValue,
                                bool treatPropertyValueAsLiteral)
        {
            string v;
            if (treatPropertyValueAsLiteral)
                v = project.GetProperty(configuration, platform, propertyName);
            else
                v = project.GetUnevalatedProperty(configuration, platform, propertyName);
            return GenericConverter.FromString(v, defaultValue);
        }

        public T GetProperty<T>(string propertyName, T defaultValue,
                                bool treatPropertyValueAsLiteral,
                                out PropertyStorageLocations location)
        {
            string v;
            if (treatPropertyValueAsLiteral)
                v = project.GetProperty(configuration, platform, propertyName, out location);
            else
                v = project.GetUnevalatedProperty(configuration, platform, propertyName, out location);
            return GenericConverter.FromString(v, defaultValue);
        }

        public void SetProperty<T>(string propertyName, T value,
                                   bool treatPropertyValueAsLiteral,
                                   PropertyStorageLocations location)
        {
            project.SetProperty(configuration, platform, propertyName,
                                GenericConverter.ToString(value), location, treatPropertyValueAsLiteral);
        }
        
        #endregion

        #region Manage bindings

        /// <summary>
        /// Initializes the Property and Project properties on the binding and calls the load method on it.
        /// Registers the binding so that Save is called on it when Save is called on the ConfigurationHelper.
        /// </summary>
        public void AddBinding(string property, ConfigurationBinding binding)
        {
            binding.Property = property;
            binding.Helper = this;
            binding.Load();
            bindings.Add(binding);

            bindingMaps.Add(property, bindings.Count - 1);
        }

        public void Load()
        {
            if (Loading != null)
            {
                Loading(this, EventArgs.Empty);
            }
            foreach (ConfigurationBinding binding in bindings)
            {
                binding.Load();
            }
            if (this.Loaded != null)
            {
                this.Loaded(this, EventArgs.Empty);
            }
            this.IsDirty = false;
        }

        public bool Save()
        {
            foreach (ConfigurationBinding binding in bindings)
            {
                if (!binding.Save())
                {
                    return false;
                }
            }
            if (this.Saved != null)
            {
                this.Saved(this, EventArgs.Empty);
            }
            this.IsDirty = false;

            return true;
        }

        /// <summary>
        /// This event is raised when another configuration is beginning to load.
        /// </summary>
        public event EventHandler Loading;

        /// <summary>
        /// This event is raised when another configuration has been loaded.
        /// </summary>
        public event EventHandler Loaded;

        /// <summary>
        /// This event is raised after the configuration has been saved.
        /// </summary>
        public event EventHandler Saved;

        private void ControlValueChanged(object sender, EventArgs e)
        {
            this.IsDirty = true;
        }

        private bool dirty;

        public bool IsDirty
        {
            get
            {
                return dirty;
            }
            set
            {
                if (dirty != value)
                {
                    dirty = value;
                    if (this.DirtyChanged != null)
                    {
                        this.DirtyChanged(this, EventArgs.Empty);
                    }
                }
            }
        }

        public event EventHandler DirtyChanged;

        private string configuration;

        public string Configuration
        {
            get
            {
                return configuration;
            }
            set
            {
                configuration = value;
            }
        }

        private string platform;

        public string Platform
        {
            get
            {
                return platform;
            }
            set
            {
                platform = value;
            }
        }

        #endregion

        #region Binding Methods and Classes

        #region Bind bool to CheckBox

        public ConfigurationBinding BindBoolean(Control control, string property, bool defaultValue)
        {
            CheckBox checkBox = control as CheckBox;
            if (checkBox != null)
            {
                return BindBoolean(checkBox, property, defaultValue);
            }
            else
            {
                throw new ApplicationException("Cannot bind " + control.GetType().Name + " to bool property.");
            }
        }

        public ConfigurationBinding BindBoolean(CheckBox checkBox, string property, bool defaultValue)
        {
            CheckBoxBinding binding = null;
            int curIndex = -1;
            if (bindingMaps.TryGetValue(property, out curIndex))
            {
                binding = bindings[curIndex] as CheckBoxBinding;
                if (binding != null)
                {
                    binding.control = checkBox;
                    binding.defaultValue = defaultValue;
                    bool isChecked = false;
                    if (Boolean.TryParse(binding.Value, out isChecked))
                    {
                        checkBox.Checked = isChecked;
                    }
                    else
                    {
                        checkBox.Checked = defaultValue;
                    }
                }
            }

            if (binding == null)
            {
                binding = new CheckBoxBinding(checkBox, defaultValue);
                AddBinding(property, binding);
            }
            checkBox.CheckedChanged += ControlValueChanged;

            return binding;
        }

        private class CheckBoxBinding : ConfigurationBinding
        {
            internal CheckBox control;
            internal bool defaultValue;

            public CheckBoxBinding(CheckBox control, bool defaultValue)
            {
                this.control = control;
                this.defaultValue = defaultValue;
            }

            public override string Value
            {
                get
                {
                    return base.Value;
                }
                set
                {
                    bool checkedValue = false;
                    if (Boolean.TryParse(value, out checkedValue))
                    {
                        base.Value = value;
                        Helper.IsDirty = true;
                    }
                }
            }

            public override void Load()
            {
                bool checkedValue = Get(defaultValue);
                if (control != null && control.IsDisposed == false)
                {
                    control.Checked = Get(defaultValue);
                }
                base.Value = checkedValue.ToString().ToLowerInvariant();
            }

            public override bool Save()
            {
                string oldValue = Get("True");
                if (oldValue == "true" || oldValue == "false")
                {
                    if (control != null && control.IsDisposed == false)
                    {
                        // keep value in lower case
                        Set(control.Checked.ToString().ToLowerInvariant());
                    }
                    else
                    {
                        Set(base.Value);
                    }
                }
                else
                {
                    if (control != null && control.IsDisposed == false)
                    {
                        Set(control.Checked.ToString());
                    }
                    else
                    {
                        Set(base.Value);
                    }
                }
                return true;
            }
        }

        #endregion

        #region Bind string to TextBox or ComboBox

        static Func<string> GetEmptyString = delegate
        {
            return "";
        };

        public ConfigurationBinding BindString(Control control, string property, TextBoxEditMode textBoxEditMode)
        {
            return BindString(control, property, textBoxEditMode, GetEmptyString);
        }

        public ConfigurationBinding BindString(Control control, string property, TextBoxEditMode textBoxEditMode, Func<string> defaultValueProvider)
        {
            if (control is TextBoxBase || control is ComboBox)
            {
                SimpleTextBinding binding = null;
                int curIndex = -1;
                if (bindingMaps.TryGetValue(property, out curIndex))
                {
                    binding = bindings[curIndex] as SimpleTextBinding;
                    if (binding != null)
                    {
                        binding.control = control;
                        if (binding.Value != null)
                        {
                            control.Text = binding.Value;
                        }
                        else
                        {
                            if (defaultValueProvider != null)
                            {
                                control.Text = defaultValueProvider();
                            }
                        }
                    }
                }

                if (binding == null)
                {
                    binding = new SimpleTextBinding(control, defaultValueProvider);
                    if (textBoxEditMode == TextBoxEditMode.EditEvaluatedProperty)
                    {
                        binding.TreatPropertyValueAsLiteral = true;
                    }
                    else
                    {
                        binding.TreatPropertyValueAsLiteral = false;
                    }
                    AddBinding(property, binding);
                }
                control.TextChanged += ControlValueChanged;
                if (control is ComboBox)
                {
                    control.KeyDown += ComboBoxKeyDown;
                }
                return binding;
            }
            else
            {
                throw new ApplicationException("Cannot bind " + control.GetType().Name + " to string property.");
            }
        }

        private void ComboBoxKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.Control | Keys.S))
            {
                e.Handled = true;
                new ICSharpCode.SharpDevelop.Commands.SaveFile().Run();
            }
        }

        private class SimpleTextBinding : ConfigurationBinding
        {
            internal Control control;
            Func<string> defaultValueProvider;

            public SimpleTextBinding(Control control, Func<string> defaultValueProvider)
            {
                this.defaultValueProvider = defaultValueProvider;
                this.control = control;
            }

            public override string Value
            {
                get
                {
                    return base.Value;
                }
                set
                {
                    base.Value = value;
                    Helper.IsDirty = true;
                }
            }

            public override void Load()
            {
                string textValue = defaultValueProvider();
                if (control != null && control.IsDisposed == false)
                {
                    textValue = Get(textValue);
                    control.Text = textValue;
                }
                base.Value = textValue;
            }

            public override bool Save()
            {
                if (control != null && control.IsDisposed == false)
                {
                    if (control.Text == defaultValueProvider())
                        Set("");
                    else
                        Set(control.Text);
                }
                else
                {
                    if (base.Value == defaultValueProvider())
                        Set("");
                    else
                        Set(base.Value);
                }

                return true;
            }
        }

        #endregion

        #region Bind int to NumericUpDown

        public ConfigurationBinding BindInt(Control control, string property, int defaultValue)
        {
            NumericUpDown updownControl = control as NumericUpDown;
            if (updownControl != null)
            {
                return this.BindInt(updownControl, property, defaultValue);
            }
            else
            {
                throw new ApplicationException("Cannot bind " + control.GetType().Name + " to int property.");
            }
        }

        public ConfigurationBinding BindInt(NumericUpDown control, string property, int defaultValue)
        {
            SimpleIntBinding binding = null;
            int curIndex = -1;
            if (bindingMaps.TryGetValue(property, out curIndex))
            {
                binding = bindings[curIndex] as SimpleIntBinding;
                if (binding != null)
                {
                    binding.control = control;
                    binding.defaultValue = defaultValue;
                    int curValue;
                    if (!Int32.TryParse(binding.Value, NumberStyles.Integer,
                        NumberFormatInfo.InvariantInfo, out curValue))
                    {
                        curValue = defaultValue;
                    }
                    control.Text = defaultValue.ToString();
                }
            }

            if (binding == null)
            {
                binding = new SimpleIntBinding(control, defaultValue);
                AddBinding(property, binding);
            }
            control.TextChanged += ControlValueChanged;
            return binding;
        }

        private class SimpleIntBinding : ConfigurationBinding
        {
            internal NumericUpDown control;
            internal int defaultValue;

            public SimpleIntBinding(NumericUpDown control, int defaultValue)
            {
                this.control = control;
                this.defaultValue = defaultValue;
            }

            public override string Value
            {
                get
                {
                    return base.Value;
                }
                set
                {
                    base.Value = value;
                    Helper.IsDirty = true;
                }
            }

            public override void Load()
            {
                int val;
                if (!Int32.TryParse(Get(defaultValue.ToString(
                    NumberFormatInfo.InvariantInfo)), NumberStyles.Integer, 
                    NumberFormatInfo.InvariantInfo, out val))
                {
                    val = defaultValue;
                }                 
                if (control != null && control.IsDisposed == false)
                {
                    control.Text = val.ToString();
                }
                base.Value = val.ToString();
            }

            public override bool Save()
            {
                string txt = this.Value.Trim();
                if (control != null && control.IsDisposed == false)
                {
                    txt = control.Text.Trim();
                }
                NumberStyles style = NumberStyles.Integer;
                int val;
                val = Int32.Parse(txt, style, NumberFormatInfo.InvariantInfo);
                Set(val.ToString(NumberFormatInfo.InvariantInfo));
                return true;
            }
        }     

        #endregion

        #region Bind hex number to TextBox

        public ConfigurationBinding BindHexadecimal(TextBoxBase textBox, string property, int defaultValue)
        {
            HexadecimalBinding binding = null;
            int curIndex = -1;
            if (bindingMaps.TryGetValue(property, out curIndex))
            {
                binding = bindings[curIndex] as HexadecimalBinding;
                if (binding != null)
                {
                    binding.textBox = textBox;
                    binding.defaultValue = defaultValue;
                    if (binding.Value != null)
                    {
                        textBox.Text = binding.Value;
                    }
                    else
                    {
                        string textValue = "0x" + defaultValue.ToString(
                            "x", NumberFormatInfo.InvariantInfo);
                        textBox.Text = textValue;
                    }
                }
            }

            if (binding == null)
            {
                binding = new HexadecimalBinding(textBox, defaultValue);
                AddBinding(property, binding);
            }
            textBox.TextChanged += ControlValueChanged;

            return binding;
        }

        private class HexadecimalBinding : ConfigurationBinding
        {
            internal TextBoxBase textBox;
            internal int defaultValue;

            public HexadecimalBinding(TextBoxBase textBox, int defaultValue)
            {
                this.textBox = textBox;
                this.defaultValue = defaultValue;
            }

            public override string Value
            {
                get
                {
                    return base.Value;
                }
                set
                {
                    base.Value = value;
                    Helper.IsDirty = true;
                }
            }

            public override void Load()
            {
                int val;
                if (!Int32.TryParse(Get(defaultValue.ToString(
                    NumberFormatInfo.InvariantInfo)), NumberStyles.Integer, 
                    NumberFormatInfo.InvariantInfo, out val))
                {
                    val = defaultValue;
                }
                string textValue = "0x" + val.ToString("x", NumberFormatInfo.InvariantInfo);
                if (textBox != null && textBox.IsDisposed == false)
                {
                    textBox.Text = textValue;
                }
                base.Value = textValue;
            }

            public override bool Save()
            {
                string txt = base.Value.Trim();
                if (textBox != null && textBox.IsDisposed == false)
                {
                    txt = textBox.Text.Trim();
                }
                NumberStyles style = NumberStyles.Integer;
                if (txt.StartsWith("0x"))
                {
                    txt = txt.Substring(2);
                    style = NumberStyles.HexNumber;
                }
                int val;
                if (!Int32.TryParse(txt, style, NumberFormatInfo.InvariantInfo, 
                    out val))
                {
                    if (textBox != null && textBox.IsDisposed == false)
                    {
                        textBox.Focus();
                    }
                    MessageService.ShowMessage(
                        "${res:Dialog.ProjectOptions.PleaseEnterValidNumber}");
                    return false;
                }
                Set(val.ToString(NumberFormatInfo.InvariantInfo));
                return true;
            }
        }

        #endregion

        #region Bind enum to ComboBox

        /// <summary>
        /// Bind enum to ComboBox. Assumes the first enum member is the default.
        /// </summary>
        public ConfigurationBinding BindEnum<T>(Control control, string property, params T[] values) where T : struct
        {
            Type type = typeof(T);
            if (values == null || values.Length == 0)
            {
                values = (T[])Enum.GetValues(type);
            }
            ComboBox comboBox = control as ComboBox;
            if (comboBox != null)
            {
                return BindEnum<T>(comboBox, property, values);
            }

            throw new ApplicationException("Cannot bind " + control.GetType().Name + " to enum property.");
        }

        /// <summary>
        /// Bind enum to ComboBox. Assumes the first enum member is the default.
        /// </summary>
        public ConfigurationBinding BindEnum<T>(ComboBox comboBox, string property, params T[] values) where T : struct
        {
            Type type = typeof(T);
            if (values == null || values.Length == 0)
            {
                values = (T[])Enum.GetValues(type);
            }

            foreach (T element in values)
            {
                object[] attr = type.GetField(Enum.GetName(type, element)).GetCustomAttributes(typeof(DescriptionAttribute), false);
                string description;
                if (attr.Length > 0)
                {
                    description = StringParser.Parse((attr[0] as DescriptionAttribute).Description);
                }
                else
                {
                    description = Enum.GetName(type, element);
                }
                if (comboBox != null)
                {
                    comboBox.Items.Add(description);
                }
            }
            string[] valueNames = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
                valueNames[i] = values[i].ToString();

            ComboBoxBinding binding = null;
            int curIndex = -1;
            if (bindingMaps.TryGetValue(property, out curIndex))
            {
                binding = bindings[curIndex] as ComboBoxBinding;
                if (binding != null)
                {
                    binding.control = comboBox;
                    binding.values = valueNames;
                    binding.defaultValue = valueNames[0];
                    int selIndex;
                    if (!Int32.TryParse(binding.Value, out selIndex))
                    {
                        selIndex = 0;
                    }
                    if (comboBox != null)
                    {
                        comboBox.SelectedIndex = selIndex;
                    }
                }
            }

            if (binding == null)
            {
                binding = new ComboBoxBinding(comboBox,
                    valueNames, valueNames[0]);
                AddBinding(property, binding);
            }
            if (comboBox != null)
            {
                comboBox.SelectedIndexChanged += ControlValueChanged;
                comboBox.KeyDown += ComboBoxKeyDown;
            }

            return binding;
        }

        /// <summary>
        /// Bind list of strings to ComboBox.
        /// entries: value -> Description
        /// </summary>
        public ConfigurationBinding BindStringEnum(Control control, string property, string defaultValue, params KeyValuePair<string, string>[] entries)
        {
            ComboBox comboBox = control as ComboBox;
            if (comboBox != null)
            {
                return BindStringEnum(comboBox, property, defaultValue, entries);
            }

            throw new ApplicationException("Cannot bind " + control.GetType().Name + " to enum property.");
       }

        /// <summary>
        /// Bind list of strings to ComboBox.
        /// entries: value -> Description
        /// </summary>
        public ConfigurationBinding BindStringEnum(ComboBox comboBox, string property, string defaultValue, params KeyValuePair<string, string>[] entries)
        {
            string[] valueNames = new string[entries.Length];
            for (int i = 0; i < entries.Length; i++)
            {
                valueNames[i] = entries[i].Key;
                if (comboBox != null)
                {
                    comboBox.Items.Add(StringParser.Parse(entries[i].Value));
                }
            }

            ComboBoxBinding binding = null;
            int curIndex = -1;
            if (bindingMaps.TryGetValue(property, out curIndex))
            {
                binding = bindings[curIndex] as ComboBoxBinding;
                if (binding != null)
                {
                    binding.control = comboBox;
                    binding.values = valueNames;
                    binding.defaultValue = defaultValue;
                    int selIndex = -1;
                    if (!Int32.TryParse(binding.Value, out selIndex))
                    {
                        int indexOf = Array.IndexOf(valueNames, defaultValue);
                        selIndex = indexOf >= 0 ? indexOf : 0;
                    }
                    if (comboBox != null)
                    {
                        comboBox.SelectedIndex = selIndex;
                    }
                }
            }

            if (binding == null)
            {
                binding = new ComboBoxBinding(comboBox, valueNames, defaultValue);
                AddBinding(property, binding);
            }
            if (comboBox != null)
            {
                comboBox.SelectedIndexChanged += ControlValueChanged;
                comboBox.KeyDown += ComboBoxKeyDown;
            }

            return binding;
        }

        private class ComboBoxBinding : ConfigurationBinding
        {
            internal ComboBox control;
            internal string[] values;
            internal string defaultValue;

            public ComboBoxBinding(ComboBox control, string[] values, string defaultValue)
            {
                this.control = control;
                this.values = values;
                this.defaultValue = defaultValue;
            }

            public override string Value
            {
                get
                {
                    return base.Value;
                }
                set
                {
                    base.Value = value;
                    Helper.IsDirty = true;
                }
            }

            public override void Load()
            {
                string val = Get(defaultValue);
                int i;
                for (i = 0; i < values.Length; i++)
                {
                    if (val.Equals(values[i], StringComparison.OrdinalIgnoreCase))
                        break;
                }
                if (i == values.Length) 
                    i = 0;

                if (control != null && control.IsDisposed == false)
                {
                    control.SelectedIndex = i;
                }
                base.Value = i.ToString();
            }

            public override bool Save()
            {
                if (control != null && control.IsDisposed == false)
                {
                    Set(values[control.SelectedIndex]);
                }
                else
                {
                    Set(values[Convert.ToInt32(base.Value)]);
                }

                return true;
            }
        }
        
        #endregion

        #region Bind enum to RadioButtons

        /// <summary>
        /// Bind enum to RadioButtons
        /// </summary>
        public ConfigurationBinding BindRadioEnum<T>(string property, params KeyValuePair<T, RadioButton>[] values) where T : struct
        {
            RadioEnumBinding<T> binding = new RadioEnumBinding<T>(values);
            AddBinding(property, binding);
            foreach (KeyValuePair<T, RadioButton> pair in values)
            {
                pair.Value.CheckedChanged += ControlValueChanged;
            }
            return binding;
        }

        private class RadioEnumBinding<T> : ConfigurationBinding where T : struct
        {
            internal KeyValuePair<T, RadioButton>[] values;

            public RadioEnumBinding(KeyValuePair<T, RadioButton>[] values)
            {
                this.values = values;
            }

            public override string Value
            {
                get
                {
                    return base.Value;
                }
                set
                {
                    base.Value = value;
                    Helper.IsDirty = true;
                }
            }

            public override void Load()
            {
                T val = Get(values[0].Key);
                int i;
                for (i = 0; i < values.Length; i++)
                {
                    if (val.Equals(values[i].Key))
                        break;
                }
                if (i == values.Length) i = 0;

                values[i].Value.Checked = true;
                base.Value = i.ToString();
            }

            public override bool Save()
            {
                if (this.IsDisposed())
                {
                    if (values == null || values.Length == 0)
                    {
                        return false;
                    }

                    Set(values[Convert.ToInt32(base.Value)].Key);
                }
                else
                {
                    foreach (KeyValuePair<T, RadioButton> pair in values)
                    {
                        if (pair.Value.Checked)
                        {
                            Set(pair.Key);
                            break;
                        }
                    }
                }
                return true;
            }

            private bool IsDisposed()
            {
                if (values == null || values.Length == 0)
                {
                    return true;
                }

                bool isDisposed = false;
                foreach (KeyValuePair<T, RadioButton> pair in values)
                {
                    Control control = pair.Value;

                    if (control == null || control.IsDisposed)
                    {
                        isDisposed = true;
                        break;
                    }
                }

                return isDisposed;
            }
        }

        #endregion

        #endregion

        #region ConfigurationSelector
        /// <summary>
        /// Gets the height of the configuration selector in pixel.
        /// </summary>
        public const int ConfigurationSelectorHeight = 30;

        public Control CreateConfigurationSelector()
        {
            return new ConfigurationSelector(this);
        }

        public void AddConfigurationSelector(Control parent)
        {
            foreach (Control ctl in parent.Controls)
            {
                ctl.Top += ConfigurationSelectorHeight;
            }
            Control sel = CreateConfigurationSelector();
            sel.Width = parent.ClientSize.Width;
            parent.Controls.Add(sel);
            parent.Controls.SetChildIndex(sel, 0);
            sel.Anchor |= AnchorStyles.Right;
        }

        sealed class ConfigurationSelector : Panel
        {
            ConfigurationHelper helper;
            Label configurationLabel = new Label();
            ComboBox configurationComboBox = new ComboBox();
            Label platformLabel = new Label();
            ComboBox platformComboBox = new ComboBox();
            Label line = new Label();

            public ConfigurationSelector(ConfigurationHelper helper)
            {
                const int marginTop = 4;
                const int marginLeft = 4;
                this.helper = helper;
                this.Height = ConfigurationSelectorHeight;
                configurationLabel.Text = StringParser.Parse("${res:Dialog.ProjectOptions.Configuration}:");
                configurationLabel.TextAlign = ContentAlignment.MiddleRight;
                configurationLabel.Location = new Point(marginLeft, marginTop);
                configurationLabel.Width = 80;
                configurationComboBox.Location = new Point(4 + configurationLabel.Right, marginTop);
                configurationComboBox.Width = 120;
                configurationComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                platformLabel.Text = StringParser.Parse("${res:Dialog.ProjectOptions.Platform}:");
                platformLabel.TextAlign = ContentAlignment.MiddleRight;
                platformLabel.Location = new Point(4 + configurationComboBox.Right, marginTop);
                platformLabel.Width = 68;
                platformComboBox.Location = new Point(4 + platformLabel.Right, marginTop);
                platformComboBox.Width = 120;
                platformComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
                line.Bounds = new Rectangle(marginLeft, ConfigurationSelectorHeight - 2, Width - marginLeft * 2, ConfigurationSelectorHeight - 2);
                line.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
                //line.BackColor = SystemColors.ControlDark;
                this.Controls.AddRange(new Control[] { configurationLabel, configurationComboBox, platformLabel, platformComboBox, line });
                line.Anchor |= AnchorStyles.Right;
                FillBoxes();
                configurationComboBox.SelectedIndexChanged += ConfigurationChanged;
                platformComboBox.SelectedIndexChanged += ConfigurationChanged;
            }

            void FillBoxes()
            {
                List<string> items;
                configurationComboBox.Items.Clear();
                items = helper.Project.ConfigurationNames.ToList();
                items.Sort();
                configurationComboBox.Items.AddRange(items.ToArray());
                platformComboBox.Items.Clear();
                items = helper.Project.PlatformNames.ToList();
                items.Sort();
                platformComboBox.Items.AddRange(items.ToArray());
                ResetIndex();
            }

            bool resettingIndex;

            void ResetIndex()
            {
                resettingIndex = true;
                configurationComboBox.SelectedIndex = configurationComboBox.Items.IndexOf(helper.Configuration);
                platformComboBox.SelectedIndex = platformComboBox.Items.IndexOf(helper.Platform);
                resettingIndex = false;
            }

            void ConfigurationChanged(object sender, EventArgs e)
            {
                if (resettingIndex) return;
                if (helper.IsDirty)
                {
                    if (!MessageService.AskQuestion("${res:Dialog.ProjectOptions.ContinueSwitchConfiguration}"))
                    {
                        ResetIndex();
                        return;
                    }
                    if (!helper.Save())
                    {
                        ResetIndex();
                        return;
                    }
                    helper.Project.Save();
                }
                helper.Configuration = (string)configurationComboBox.SelectedItem;
                helper.Platform = (string)platformComboBox.SelectedItem;
                helper.Load();
            }
        }
        #endregion
    }
}
