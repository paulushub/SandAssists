// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3863 $</version>
// </file>

using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;

namespace ICSharpCode.Core
{
	/// <summary>
	/// This class parses internal ${xyz} tags of #Develop.
	/// All environment variables are available under the name env.[NAME]
    /// where [NAME] represents the string under which it is available in
	/// the environment.
	/// </summary>
	public static class StringParser
	{
		readonly static Dictionary<string, string>             properties;
		//readonly static Dictionary<string, IStringTagProvider> stringTagProviders;
		readonly static Dictionary<string, object>             propertyObjects;
        readonly static List<IStringTagProvider> stringTagProviders;
        readonly static StringPropertiesHelper propertiesHelper;
		
		public static IDictionary<string, string> Properties {
			get {
                return propertiesHelper;
			}
		}
		
		public static IDictionary<string, object> PropertyObjects {
			get {
				return propertyObjects;
			}
		}
		
		static StringParser()
		{
			properties         = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			//stringTagProviders = new Dictionary<string, IStringTagProvider>(StringComparer.OrdinalIgnoreCase);
            stringTagProviders = new List<IStringTagProvider>();
			propertyObjects    = new Dictionary<string, object>();

            propertiesHelper   = new StringPropertiesHelper(properties, stringTagProviders);
			
			// entryAssembly == null might happen in unit test mode
			Assembly entryAssembly = Assembly.GetEntryAssembly();
			if (entryAssembly != null) {
				string exeName = entryAssembly.Location;
				propertyObjects["exe"] = FileVersionInfo.GetVersionInfo(exeName);
			}
			properties["USER"]    = Environment.UserName;
			properties["Version"] = RevisionClass.FullVersion;
            properties["AppBinPath"] = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string sandPath = Environment.ExpandEnvironmentVariables("%DXROOT%");
            if (!String.IsNullOrEmpty(sandPath))
            {
                properties["SandcastlePath"] = FileUtility.StripEndBackSlash(sandPath);
            }
			
			// Maybe test for Mono?
			if (IntPtr.Size == 4) {
				properties["Platform"] = "Win32";
			} else if (IntPtr.Size == 8) {
				properties["Platform"] = "Win64";
			} else {
				properties["Platform"] = "unknown";
			}
		}
		
		/// <summary>
		/// Expands ${xyz} style property values.
		/// </summary>
		public static string Parse(string input)
		{
			return Parse(input, null);
		}
		
		/// <summary>
		/// Parses an array and replaces the elements in the existing array.
		/// </summary>
		public static void Parse(string[] inputs)
		{
			for (int i = 0; i < inputs.Length; ++i) {
				inputs[i] = Parse(inputs[i], null);
			}
		}
		
		public static void RegisterStringTagProvider(IStringTagProvider tagProvider)
		{
            if (tagProvider == null || stringTagProviders.Contains(tagProvider))
            {
                return;
            }
            stringTagProviders.Add(tagProvider);
            //foreach (string str in tagProvider.Tags) 
            //{
            //    if (!String.IsNullOrEmpty(str))
            //    {
            //        stringTagProviders[str] = tagProvider;
            //    }
            //}
		}
		
		public static void UnRegisterStringTagProvider(IStringTagProvider tagProvider)
		{
            if (tagProvider == null)
            {
                return;
            }
            stringTagProviders.Remove(tagProvider);

            //foreach (string str in tagProvider.Tags) 
            //{
            //    if (!String.IsNullOrEmpty(str))
            //    {
            //        stringTagProviders.Remove(str);
            //    }
            //}
		}
		
		//readonly static Regex pattern = new Regex(@"\$\{([^\}]*)\}", RegexOptions.Compiled | RegexOptions.CultureInvariant);
		
		/// <summary>
		/// Expands ${xyz} style property values.
		/// </summary>
		public static string Parse(string input, string[,] customTags)
		{
			if (input == null)
				return null;
			int pos = 0;
			StringBuilder output = null; // don't use StringBuilder if input is a single property
			do {
				int oldPos = pos;
				pos = input.IndexOf("${", pos, StringComparison.Ordinal);
				if (pos < 0) {
					if (output == null) {
						return input;
					} else {
						if (oldPos < input.Length) {
							// normal text after last property
							output.Append(input, oldPos, input.Length - oldPos);
						}
						return output.ToString();
					}
				}
				if (output == null) {
					if (pos == 0)
						output = new StringBuilder();
					else
						output = new StringBuilder(input, 0, pos, pos + 16);
				} else {
					if (pos > oldPos) {
						// normal text between two properties
						output.Append(input, oldPos, pos - oldPos);
					}
				}
				int end = input.IndexOf('}', pos + 1);
				if (end < 0) {
					output.Append("${");
					pos += 2;
				} else {
					string property = input.Substring(pos + 2, end - pos - 2);
					string val = GetValue(property, customTags);
					if (val == null) {
						output.Append("${");
						output.Append(property);
						output.Append('}');
					} else {
						output.Append(val);
					}
					pos = end + 1;
				}
			} while (pos < input.Length);

			return output.ToString();
		}
		
		static string GetValue(string propertyName, string[,] customTags)
		{
			// most properties start with res: in lowercase,
			// so we can save 2 string allocations here, in addition to all the jumps
			// All other prefixed properties {prefix:Key} should get handled in the switch below.
			if (propertyName.StartsWith("res:", StringComparison.OrdinalIgnoreCase)) {
				try {
					return Parse(ResourceService.GetString(propertyName.Substring(4)), customTags);
				} catch (ResourceNotFoundException) {
					return null;
				}
			}
			if (propertyName.StartsWith("DATE:", StringComparison.OrdinalIgnoreCase))
			{
				try {
					return DateTime.Now.ToString(propertyName.Split(':')[1]);
				} catch (Exception ex) {
					return ex.Message;
				}
			}
            if (propertyName.StartsWith("GUID:", StringComparison.OrdinalIgnoreCase))
			{
				try {
                    string resultGuid = Guid.NewGuid().ToString(
                        propertyName.Split(':')[1]);


                    if (propertyName.Equals("GUID:", StringComparison.Ordinal))
                    {
                        return resultGuid.ToUpperInvariant();
                    }

                    return resultGuid;
                }
                catch (Exception ex)
                {
					return ex.Message;
				}
			}
			if (propertyName.Equals("DATE", StringComparison.OrdinalIgnoreCase))
				return DateTime.Today.ToShortDateString();
			if (propertyName.Equals("TIME", StringComparison.OrdinalIgnoreCase))
				return DateTime.Now.ToShortTimeString();
			if (propertyName.Equals("ProductName", StringComparison.OrdinalIgnoreCase))
				return MessageService.ProductName;
            if (propertyName.Equals("GUID", StringComparison.OrdinalIgnoreCase))
            {
                if (propertyName.Equals("GUID", StringComparison.Ordinal))
                {
                    return Guid.NewGuid().ToString().ToUpperInvariant();
                }

                return Guid.NewGuid().ToString();
            }
			
			if (customTags != null) {
				for (int j = 0; j < customTags.GetLength(0); ++j) {
					if (propertyName.Equals(customTags[j, 0], StringComparison.OrdinalIgnoreCase)) {
						return customTags[j, 1];
					}
				}
			}
			
			if (properties.ContainsKey(propertyName)) {
				return properties[propertyName];
			}

            if (stringTagProviders != null && stringTagProviders.Count != 0)
            {
                int providerCount = stringTagProviders.Count;
                for (int q = providerCount - 1; q >= 0; q--)
                {
                    IStringTagProvider tagProvider = stringTagProviders[q];

                    if (tagProvider.Contains(propertyName))
                    {
                        return tagProvider.Convert(propertyName);
                    }
                }
            }
			
			int k = propertyName.IndexOf(':');
			if (k <= 0)
				return null;
			string prefix = propertyName.Substring(0, k);
			propertyName = propertyName.Substring(k + 1);
			switch (prefix.ToUpperInvariant()) {
				case "SDKTOOLPATH":
					return FileUtility.GetSdkPath(propertyName);
				case "ADDINPATH":
					foreach (AddIn addIn in AddInTree.AddIns) {
						if (addIn.Manifest.Identities.ContainsKey(propertyName)) {
							return System.IO.Path.GetDirectoryName(addIn.FileName);
						}
					}
					return null;
				case "ENV":
					return Environment.GetEnvironmentVariable(propertyName);
				case "RES":
					try {
						return Parse(ResourceService.GetString(propertyName), customTags);
					} catch (ResourceNotFoundException) {
						return null;
					}
				case "PROPERTY":
					return GetProperty(propertyName);
				default:
					if (propertyObjects.ContainsKey(prefix)) {
						return Get(propertyObjects[prefix], propertyName);
					} else {
						return null;
					}
			}
		}
		
		/// <summary>
		/// Allow special syntax to retrieve property values:
		/// ${property:PropertyName}
		/// ${property:PropertyName??DefaultValue}
		/// ${property:ContainerName/PropertyName}
		/// ${property:ContainerName/PropertyName??DefaultValue}
		/// A container is a Properties instance stored in the PropertyService. This is
		/// used by many AddIns to group all their properties into one container.
		/// </summary>
		static string GetProperty(string propertyName)
		{
			string defaultValue = "";
			int pos = propertyName.LastIndexOf("??", StringComparison.Ordinal);
			if (pos >= 0) {
				defaultValue = propertyName.Substring(pos + 2);
				propertyName = propertyName.Substring(0, pos);
			}
			pos = propertyName.IndexOf('/');
			if (pos >= 0) {
				Properties properties = PropertyService.Get(propertyName.Substring(0, pos), new Properties());
				propertyName = propertyName.Substring(pos + 1);
				pos = propertyName.IndexOf('/');
				while (pos >= 0) {
					properties = properties.Get(propertyName.Substring(0, pos), new Properties());
					propertyName = propertyName.Substring(pos + 1);
				}
				return properties.Get(propertyName, defaultValue);
			} else {
				return PropertyService.Get(propertyName, defaultValue);
			}
		}
		
		static string Get(object obj, string name)
		{
			Type type = obj.GetType();
			PropertyInfo prop = type.GetProperty(name);
			if (prop != null) {
				return prop.GetValue(obj, null).ToString();
			}
			FieldInfo field = type.GetField(name);
			if (field != null) {
				return field.GetValue(obj).ToString();
			}
			return null;
        }

        #region StringPropertiesHelper

        private sealed class StringPropertiesHelper : IDictionary<string, string>
        {
            private IDictionary<string, string> _properties;
            readonly IList<IStringTagProvider>  _providers;

            public StringPropertiesHelper(IDictionary<string, string> properties,
                IList<IStringTagProvider> providers)
            {
                _properties = properties;
                _providers  = providers;
            }

            #region IDictionary<string,string> Members

            public void Add(string key, string value)
            {
                _properties.Add(key, value);
            }

            public bool ContainsKey(string key)
            {
                int providerCount = stringTagProviders.Count;
                for (int i = providerCount - 1; i >= 0; i--)
                {
                    IStringTagProvider tagProvider = _providers[i];

                    if (tagProvider.Contains(key))
                    {
                        return true;
                    }
                }

                return _properties.ContainsKey(key);
            }

            public ICollection<string> Keys
            {
                get 
                {
                    return _properties.Keys;
                }
            }

            public bool Remove(string key)
            {
                return _properties.Remove(key);
            }

            public bool TryGetValue(string key, out string value)
            {
                int providerCount = stringTagProviders.Count;
                for (int i = providerCount - 1; i >= 0; i--)
                {
                    IStringTagProvider tagProvider = _providers[i];

                    if (tagProvider.Contains(key))
                    {
                        value = tagProvider[key];

                        return true;
                    }
                }

                return _properties.TryGetValue(key, out value);
            }

            public ICollection<string> Values
            {
                get 
                {
                    return _properties.Values;
                }
            }

            public string this[string key]
            {
                get
                {
                    int providerCount = stringTagProviders.Count;
                    for (int i = providerCount - 1; i >= 0; i--)
                    {
                        IStringTagProvider tagProvider = _providers[i];

                        if (tagProvider.Contains(key))
                        {
                            return tagProvider[key];
                        }
                    }

                    return _properties[key];
                }
                set
                {
                    int providerCount = stringTagProviders.Count;
                    for (int i = 0; i < providerCount; i++)
                    {
                        IStringTagProvider tagProvider = _providers[i];

                        if (tagProvider.Contains(key))
                        {
                            tagProvider[key] = value;
                        }
                    }

                    _properties[key] = value;
                }
            }

            #endregion

            #region ICollection<KeyValuePair<string,string>> Members

            public void Add(KeyValuePair<string, string> item)
            {
                _properties.Add(item);
            }

            public void Clear()
            {
                _properties.Clear();
            }

            public bool Contains(KeyValuePair<string, string> item)
            {
                return _properties.Contains(item);
            }

            public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
            {
                _properties.CopyTo(array, arrayIndex);
            }

            public int Count
            {
                get 
                {
                    return _properties.Count;
                }
            }

            public bool IsReadOnly
            {
                get 
                {
                    return _properties.IsReadOnly;
                }
            }

            public bool Remove(KeyValuePair<string, string> item)
            {
                return _properties.Remove(item);
            }

            #endregion

            #region IEnumerable<KeyValuePair<string,string>> Members

            public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
            {
                return _properties.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _properties.GetEnumerator();
            }

            #endregion
        }

        #endregion
	}
}
