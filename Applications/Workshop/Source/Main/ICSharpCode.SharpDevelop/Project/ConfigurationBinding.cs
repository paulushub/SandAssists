// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 2455 $</version>
// </file>

using System;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Project
{
    public abstract class ConfigurationBinding
    {
        private ConfigurationHelper helper;
        private string property;
        private string propertyValue;
        private bool treatPropertyValueAsLiteral = true;

        private PropertyStorageLocations defaultLocation = PropertyStorageLocations.Base;
        private PropertyStorageLocations location = PropertyStorageLocations.Unknown;
        private ChooseStorageLocationButton storageLocationButton;

        private bool isFirstGet = true;

        public MSBuildBasedProject Project
        {
            get
            {
                return helper.Project;
            }
        }

        public ConfigurationHelper Helper
        {
            get
            {
                return helper;
            }
            internal set
            {
                helper = value;
            }
        }

        public string Property
        {
            get
            {
                return property;
            }
            internal set
            {
                property = value;
            }
        }

        public virtual string Value
        {
            get
            {
                return propertyValue;
            }
            set
            {
                propertyValue = value;
            }
        }

        /// <summary>
        /// Gets if the value should be evaluated when loading the property and escaped
        /// when saving. The default value is true.
        /// </summary>
        public bool TreatPropertyValueAsLiteral
        {
            get
            {
                return treatPropertyValueAsLiteral;
            }
            set
            {
                treatPropertyValueAsLiteral = value;
            }
        }


        public PropertyStorageLocations DefaultLocation
        {
            get
            {
                return defaultLocation;
            }
            set
            {
                defaultLocation = value;
            }
        }

        public PropertyStorageLocations Location
        {
            get
            {
                return location;
            }
            set
            {
                if (location != value)
                {
                    location = value;
                    if (storageLocationButton != null)
                    {
                        storageLocationButton.StorageLocation = value;
                    }
                    helper.IsDirty = true;
                }
            }
        }

        public ChooseStorageLocationButton CreateLocationButton()
        {
            ChooseStorageLocationButton btn = new ChooseStorageLocationButton();
            //if (location == PropertyStorageLocations.Unknown)
            //{
            //    btn.StorageLocation = defaultLocation;
            //}
            //else
            //{
            //    btn.StorageLocation = location;
            //}
            //RegisterLocationButton(btn);
            return btn;
        }

        /// <summary>
        /// Makes this configuration binding being controlled by the specified button.
        /// Use this method if you want to use one ChooseStorageLocationButton to control
        /// multiple properties.
        /// </summary>
        public void RegisterLocationButton(ChooseStorageLocationButton btn)
        {
            this.storageLocationButton = btn;
            btn.StorageLocationChanged += delegate(object sender, EventArgs e)
            {
                this.Location = ((ChooseStorageLocationButton)sender).StorageLocation;
            };
        }

        public ChooseStorageLocationButton CreateLocationButtonInPanel(Control panel)
        {
            ChooseStorageLocationButton btn = CreateLocationButton();
            //foreach (Control ctl in panel.Controls)
            //{
            //    if ((ctl.Anchor & AnchorStyles.Left) == AnchorStyles.Left)
            //    {
            //        ctl.Left += btn.Width + 8;
            //        if ((ctl.Anchor & AnchorStyles.Right) == AnchorStyles.Right)
            //        {
            //            ctl.Width -= btn.Width + 8;
            //        }
            //    }
            //}
            //btn.Location = new Point(4, (panel.ClientSize.Height - btn.Height) / 2);
            //panel.Controls.Add(btn);
            //panel.Controls.SetChildIndex(btn, 0);
            return btn;
        }

        /// <summary>
        /// Moves the <paramref name="replacedControl"/> a bit to the right and inserts a
        /// <see cref="ChooseStorageLocationButton"/>.
        /// </summary>
        public ChooseStorageLocationButton CreateLocationButton(Control replacedControl)
        {
            ChooseStorageLocationButton btn = CreateLocationButton();
            //btn.Location = new Point(replacedControl.Left, replacedControl.Top + (replacedControl.Height - btn.Height) / 2);
            //replacedControl.Left += btn.Width + 4;
            //replacedControl.Width -= btn.Width + 4;
            //replacedControl.Parent.Controls.Add(btn);
            //replacedControl.Parent.Controls.SetChildIndex(btn, replacedControl.Parent.Controls.IndexOf(replacedControl));
            return btn;
        }

        public T Get<T>(T defaultValue)
        {
            if (isFirstGet)
            {
                isFirstGet = false;
                return helper.GetProperty(property, defaultValue,
                                          treatPropertyValueAsLiteral, out location);
            }
            else
            {
                return helper.GetProperty(property, defaultValue, treatPropertyValueAsLiteral);
            }
        }

        public void Set<T>(T value)
        {
            if (location == PropertyStorageLocations.Unknown)
            {
                location = defaultLocation;
            }
            helper.SetProperty(property, value, treatPropertyValueAsLiteral, location);
        }

        public abstract void Load();
        public abstract bool Save();
    }
}
