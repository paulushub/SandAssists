// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3786 $</version>
// </file>

using System;
using System.IO;
using System.Text;
using System.Globalization;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace ICSharpCode.Core
{
    /// <summary>
    /// This interface flags an object beeing "mementocapable". This means that the
    /// state of the object could be saved to an <see cref="Properties"/> object
    /// and set from a object from the same class.
    /// This is used to save and restore the state of GUI objects.
    /// </summary>
    public interface IMementoCapable
    {
        /// <summary>
        /// Creates a new memento from the state.
        /// </summary>
        Properties CreateMemento();

        /// <summary>
        /// Sets the state to the given memento.
        /// </summary>
        void SetMemento(Properties memento);
    }

    /// <summary>
    /// Description of PropertyGroup.
    /// </summary>
    public class Properties
    {
        #region Private Fields

        private Dictionary<string, object> properties;

        #endregion

        #region Constructors and Destructor

        public Properties()
        {
            properties = new Dictionary<string, object>(
                StringComparer.OrdinalIgnoreCase);
        }

        #endregion

        #region SerializedValue Class

        /// <summary> Needed for support of late deserialization </summary>
        private sealed class SerializedValue
        {
            string content;

            public string Content
            {
                get
                {
                    return content;
                }
            }

            public T Deserialize<T>()
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return (T)serializer.Deserialize(new StringReader(content));
            }

            public SerializedValue(string content)
            {
                this.content = content;
            }
        }

        #endregion

        #region Public Events

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region Public Properties

        public int Count
        {
            get
            {
                lock (properties)
                {
                    return properties.Count;
                }
            }
        }

        public string this[string property]
        {
            get
            {
                return Get(property, String.Empty);
            }
            set
            {
                if (value == null)
                {
                    value = String.Empty;
                }
                Set(property, value);
            }
        }

        public string[] Keys
        {
            get
            {
                lock (properties)
                {
                    ICollection<string> collKeys = properties.Keys;
                    List<string> ret = new List<string>(collKeys);
                    //foreach (KeyValuePair<string, object> property in properties)
                    //    ret.Add(property.Key);

                    return ret.ToArray();
                }
            }
        }

        #endregion

        #region Public Methods

        public object Get(string property)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            lock (properties)
            {
                object val;
                properties.TryGetValue(property, out val);
                return val;
            }
        }

        public string Get(string property, string defaultValue)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            lock (properties)
            {
                object itemValue;
                if (properties.TryGetValue(property, out itemValue))
                {
                    if (itemValue != null)
                    {
                        return itemValue.ToString();
                    }
                }

                return defaultValue;
            }
        }

        public T Get<T>(string property, T defaultValue)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            lock (properties)
            {
                object itemValue;
                if (!properties.TryGetValue(property, out itemValue))
                {
                    properties.Add(property, defaultValue);
                    return defaultValue;
                }

                if (itemValue is string && typeof(T) != typeof(string))
                {
                    TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
                    try
                    {
                        itemValue = c.ConvertFromInvariantString(itemValue.ToString());
                    }
                    catch (Exception ex)
                    {
                        MessageService.ShowWarning("Error loading property '" + property + "': " + ex.Message);
                        itemValue = defaultValue;
                    }
                    properties[property] = itemValue; // store for future look up
                }
                else if (itemValue is StringList && typeof(T).IsArray)
                {
                    properties[property] = itemValue; // store for future look up
                }
                else if (itemValue is ArrayList && typeof(T).IsArray)
                {
                    ArrayList list = (ArrayList)itemValue;
                    Type elementType = typeof(T).GetElementType();
                    Array arr = System.Array.CreateInstance(elementType, list.Count);
                    TypeConverter c = TypeDescriptor.GetConverter(elementType);
                    try
                    {
                        for (int i = 0; i < arr.Length; ++i)
                        {
                            if (list[i] != null)
                            {
                                arr.SetValue(c.ConvertFromInvariantString(list[i].ToString()), i);
                            }
                        }
                        itemValue = arr;
                    }
                    catch (Exception ex)
                    {
                        MessageService.ShowWarning("Error loading property '" + property + "': " + ex.Message);
                        itemValue = defaultValue;
                    }
                    properties[property] = itemValue; // store for future look up
                }
                else if (!(itemValue is string) && typeof(T) == typeof(string))
                {
                    TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
                    if (c.CanConvertTo(typeof(string)))
                    {
                        itemValue = c.ConvertToInvariantString(itemValue);
                    }
                    else
                    {
                        itemValue = itemValue.ToString();
                    }
                }
                else if (itemValue is SerializedValue)
                {
                    try
                    {
                        itemValue = ((SerializedValue)itemValue).Deserialize<T>();
                    }
                    catch (Exception ex)
                    {
                        MessageService.ShowWarning("Error loading property '" + property + "': " + ex.Message);
                        itemValue = defaultValue;
                    }
                    properties[property] = itemValue; // store for future look up
                }

                try
                {
                    return (T)itemValue;
                }
                catch (NullReferenceException)
                {
                    // can happen when configuration is invalid -> o is null and a value type is expected
                    return defaultValue;
                }
                catch (Exception ex)
                {
                    MessageService.ShowError(ex);
                    // can happen when configuration is invalid -> o is null and a value type is expected
                    return defaultValue;
                }
            }
        }

        public void Set(string property, string value)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            if (value == null)
            {
                value = String.Empty;
            }

            string oldValue = String.Empty;
            lock (properties)
            {
                if (!properties.ContainsKey(property))
                {
                    properties.Add(property, value);
                }
                else
                {
                    oldValue = Get<string>(property, value);
                    properties[property] = value;
                }
            }
            OnPropertyChanged(new PropertyChangedEventArgs(this, property, oldValue, value));
        }

        public void Set<T>(string property, T value)
        {
            if (property == null)
                throw new ArgumentNullException("property");
            if (value == null)
                throw new ArgumentNullException("value");

            T oldValue = default(T);
            lock (properties)
            {
                if (!properties.ContainsKey(property))
                {
                    properties.Add(property, value);
                }
                else
                {
                    oldValue = Get<T>(property, value);
                    properties[property] = value;
                }
            }
            OnPropertyChanged(new PropertyChangedEventArgs(this, 
                property, oldValue, value));
        }

        public bool Contains(string property)
        {
            lock (properties)
            {
                return properties.ContainsKey(property);
            }
        }

        public bool Remove(string property)
        {
            lock (properties)
            {
                return properties.Remove(property);
            }
        }

        public override string ToString()
        {
            lock (properties)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("[Properties:{");
                foreach (KeyValuePair<string, object> entry in properties)
                {
                    sb.Append(entry.Key);
                    sb.Append("=");
                    sb.Append(entry.Value);
                    sb.Append(",");
                }
                sb.Append("}]");
                return sb.ToString();
            }
        }

        public static Properties ReadFromAttributes(XmlReader reader)
        {
            Properties properties = new Properties();
            if (reader.HasAttributes)
            {
                for (int i = 0; i < reader.AttributeCount; i++)
                {
                    reader.MoveToAttribute(i);
                    properties[reader.Name] = reader.Value;
                }

                reader.MoveToElement(); //Moves the reader back to the element node.
            }
            return properties;
        }

        public void WriteXml(XmlWriter writer)
        {
            lock (properties)
            {
                List<KeyValuePair<string, object>> sortedProperties = new List<KeyValuePair<string, object>>(properties);
                sortedProperties.Sort((a, b) => StringComparer.OrdinalIgnoreCase.Compare(a.Key, b.Key));
                foreach (KeyValuePair<string, object> entry in sortedProperties)
                {
                    object val = entry.Value;
                    if (val is Properties)
                    {
                        writer.WriteStartElement("Properties");
                        writer.WriteAttributeString("name", entry.Key);
                        ((Properties)val).WriteXml(writer);
                        writer.WriteEndElement();
                    }
                    else if (val is StringList)
                    {
                        writer.WriteStartElement("StringList");
                        writer.WriteAttributeString("name", entry.Key);
                        foreach (string o in (StringList)val)
                        {
                            writer.WriteStartElement("String");
                            WriteValue(writer, o);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                    else if (val is Array || val is ArrayList)
                    {
                        writer.WriteStartElement("Array");
                        writer.WriteAttributeString("name", entry.Key);
                        foreach (object o in (IEnumerable)val)
                        {
                            writer.WriteStartElement("Element");
                            WriteValue(writer, o);
                            writer.WriteEndElement();
                        }
                        writer.WriteEndElement();
                    }
                    else if (TypeDescriptor.GetConverter(val).CanConvertFrom(typeof(string)))
                    {
                        writer.WriteStartElement(entry.Key);
                        WriteValue(writer, val);
                        writer.WriteEndElement();
                    }
                    else if (val is SerializedValue)
                    {
                        writer.WriteStartElement("SerializedValue");
                        writer.WriteAttributeString("name", entry.Key);
                        writer.WriteRaw(((SerializedValue)val).Content);
                        writer.WriteEndElement();
                    }
                    else
                    {
                        writer.WriteStartElement("SerializedValue");
                        writer.WriteAttributeString("name", entry.Key);
                        XmlSerializer serializer = new XmlSerializer(val.GetType());
                        serializer.Serialize(writer, val, null);
                        writer.WriteEndElement();
                    }
                }
            }
        }

        public void Save(string fileName)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                return;
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            using (XmlWriter writer = XmlWriter.Create(fileName, settings))
            {
                //writer.Formatting = Formatting.Indented;
                writer.WriteStartElement("Properties");
                this.WriteXml(writer);
                writer.WriteEndElement();
            }
        }

        public void Load(string fileName)
        {
            if (String.IsNullOrEmpty(fileName) || !File.Exists(fileName))
            {
                return;
            }

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            settings.IgnoreProcessingInstructions = true;

            using (XmlReader reader = XmlReader.Create(fileName, settings))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element &&
                        reader.LocalName == "Properties")
                    {
                        this.ReadXml(reader);
                        break;
                    }
                }
            }
        }

        public void ReadXml(XmlReader reader)
        {
            this.ReadXml(reader, "Properties");
        }   

        public void ReadXml(XmlReader reader, string endElement)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (reader.LocalName == endElement)
                        {
                            return;
                        }
                        break;
                    case XmlNodeType.Element:
                        string propertyName = reader.LocalName;
                        if (propertyName == "Properties")
                        {
                            propertyName = reader.GetAttribute(0);
                            Properties p = new Properties();
                            p.ReadXml(reader);
                            properties[propertyName] = p;
                        }
                        else if (propertyName == "Array")
                        {
                            propertyName = reader.GetAttribute(0);
                            properties[propertyName] = ReadArray(reader);
                        }
                        else if (propertyName == "StringList")
                        {
                            propertyName = reader.GetAttribute(0);
                            properties[propertyName] = ReadStringList(reader);
                        }
                        else if (propertyName == "SerializedValue")
                        {
                            propertyName = reader.GetAttribute(0);
                            properties[propertyName] = new SerializedValue(reader.ReadInnerXml());
                        }
                        else
                        {
                            properties[propertyName] = reader.HasAttributes ? reader.GetAttribute(0) : null;
                        }
                        break;
                }
            }
        }

        #endregion

        #region Private Methods

        private ArrayList ReadArray(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return new ArrayList(0);
            ArrayList l = new ArrayList();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (reader.LocalName == "Array")
                        {
                            return l;
                        }
                        break;
                    case XmlNodeType.Element:
                        l.Add(reader.HasAttributes ? reader.GetAttribute(0) : null);
                        break;
                }
            }
            return l;
        }

        private StringList ReadStringList(XmlReader reader)
        {
            if (reader.IsEmptyElement)
                return new StringList();
            StringList list = new StringList();
            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        list.Add(reader.HasAttributes ? reader.GetAttribute(0) : String.Empty);
                        break;
                    case XmlNodeType.EndElement:
                        if (reader.LocalName == "StringList")
                        {
                            return list;
                        }
                        break;
                }
            }
            return list;
        }

        private void WriteValue(XmlWriter writer, string val)
        {
            if (val == null)
            {
                val = String.Empty;
            }
            writer.WriteAttributeString("value", val);
        }

        private void WriteValue(XmlWriter writer, object val)
        {
            if (val != null)
            {
                if (val is string)
                {
                    writer.WriteAttributeString("value", val.ToString());
                }
                else
                {
                    TypeConverter c = TypeDescriptor.GetConverter(val.GetType());
                    writer.WriteAttributeString("value", c.ConvertToInvariantString(val));
                }
            }
        }

        #endregion

        #region Protected Methods

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, e);
            }
        }

        #endregion
    }
}
