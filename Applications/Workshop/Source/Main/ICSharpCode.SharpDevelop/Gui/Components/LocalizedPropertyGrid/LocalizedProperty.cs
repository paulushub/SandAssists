// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="none" email=""/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Globalization;
using System.ComponentModel;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui
{
	public class LocalizedProperty : PropertyDescriptor
	{
		string category;
		string description;
		string name;
		string type;
		string localizedName;
        private bool _isReadOnly;
		
		TypeConverter typeConverterObject;
		object        defaultValue;
        IStringTagProvider _tagProvider;

        public LocalizedProperty(string name, string type, string category,
            string description)
            : base(name, null)
        {
            this.category = category;
            this.description = description;
            this.name = name;
            this.type = type;
        }
		
		public TypeConverter TypeConverterObject {
			get {
				return typeConverterObject;
			}
			set {
				typeConverterObject = value;
			}
		}

        public IStringTagProvider TagProvider
        {
            get
            {
                return _tagProvider;
            }
            set
            {
                _tagProvider = value;
            }
        }
		
		public object DefaultValue {
			get {
				return defaultValue;
			}
			set {
				defaultValue = value;
			}
		}
		
		public string LocalizedName {
			get {
				if (localizedName == null) {
					return null;
				}
				return StringParser.Parse(localizedName);
			}
			set {
				localizedName = value;
			}
		}
		
		public override bool IsReadOnly {
			get {
                return _isReadOnly;
			}
		}
		
		#region PropertyDescriptor 
		public override string DisplayName {
			get  {
				if (localizedName != null && localizedName.Length > 0) {
					return LocalizedName;
				}
				return Name;
			}
		}
		
		public override string Category {
			get {
				return StringParser.Parse(category);
			}
		}
		
		public override string Description {
			get {
				return StringParser.Parse(description);
			}
		}
		
		public override Type PropertyType {
			get {
				return Type.GetType(this.type); 
			}
		}
		
		public override Type ComponentType {
			get {
				return Type.GetType(this.type); 
			}
		}
		
		public override TypeConverter Converter {
			get {
				if (typeConverterObject != null) {
					return typeConverterObject;
				}
				return base.Converter;
			}
		}
		
		public override object GetValue(object component)
		{
            string propertyName = "Properties." + this.Name;
            string propertyValue = null;
            if (_tagProvider != null && _tagProvider.Contains(propertyName))
            {
                propertyValue = _tagProvider.Convert(propertyName);
            }
            else
            {
                propertyValue = StringParser.Properties[propertyName];
            }
			
			if (typeConverterObject is BooleanTypeConverter) {
				return Boolean.Parse(propertyValue); 
			}
			
			if (typeConverterObject is DateTimeConverter) {
                return DateTime.Parse(propertyValue); 
			}

            if (typeConverterObject is CultureInfoConverter)
            {
                return new CultureInfo(propertyValue); 
			}

			return propertyValue;
		}
		
		public override void SetValue(object component, object val)
		{
            string propertyName = "Properties." + this.Name;
            if (_tagProvider != null && _tagProvider.Contains(propertyName))
            {
                if (typeConverterObject != null)
                {
                    if (val is DateTime)
                    {
                        // The DateTime converter can only convert a DateTime 
                        // object to and from a string.
                        _tagProvider[propertyName] = val.ToString();
                    }
                    else if (val is CultureInfo)
                    {
                        // The CultureInfo converter can only convert a CultureInfo 
                        // object to and from a string.
                        _tagProvider[propertyName] = val.ToString();
                    }
                    else
                    {
                        _tagProvider[propertyName] = typeConverterObject.ConvertFrom(val).ToString();
                    }
                }
                else
                {
                    _tagProvider[propertyName] = val.ToString();
                }
            }
            else
            {
                if (typeConverterObject != null)
                {
                    StringParser.Properties[propertyName] = typeConverterObject.ConvertFrom(val).ToString();
                }
                else
                {
                    StringParser.Properties[propertyName] = val.ToString();
                }
            }
		}
		
		public override bool ShouldSerializeValue(object component)
		{
			return false;
		}
		
		public override bool CanResetValue(object component)
		{
			return defaultValue != null;
		}
		
		public override void ResetValue(object component)
		{
			SetValue(component, defaultValue);
		}

        public void SetReadOnly(bool readOnly)
        {
            _isReadOnly = readOnly;
        }

		#endregion
	}
}
