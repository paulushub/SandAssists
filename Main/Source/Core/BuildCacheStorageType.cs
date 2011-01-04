using System;

namespace Sandcastle
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// This is an extensible enumeration type structure with the following members and
    /// values:
    /// <list type="table">
    /// <listheader>
    /// <term>Enumeration</term>
    /// <term>Value</term>
    /// <term>Description</term>
    /// </listheader>
    /// <item>
    /// <description><see cref="BuildCacheStorageType.Null"/></description>
    /// <description>-1</description>
    /// <description>Null</description>
    /// </item>
    /// <item>
    /// <description><see cref="BuildCacheStorageType.None"/></description>
    /// <description>0</description>
    /// <description>None</description>
    /// </item>
    /// <item>
    /// <description><see cref="BuildCacheStorageType.Memory"/></description>
    /// <description>1</description>
    /// <description>A</description>
    /// </item>
    /// <item>
    /// <description><see cref="BuildCacheStorageType.Database"/></description>
    /// <description>2</description>
    /// <description>B</description>
    /// </item>
    /// <item>
    /// <description><see cref="BuildCacheStorageType.Custom"/></description>
    /// <description>10</description>
    /// <description>Custom</description>
    /// </item>
    /// </list>
    /// </remarks>
    [Serializable]
    public struct BuildCacheStorageType : IEquatable<BuildCacheStorageType>,
        IComparable<BuildCacheStorageType>
    {
        #region Public Enumerations

        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>-1</c>.</para>
        /// </summary>
        public readonly static BuildCacheStorageType Null = new BuildCacheStorageType(-1);
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>0</c>.</para>
        /// </summary>
        public readonly static BuildCacheStorageType None = new BuildCacheStorageType(0);
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>1</c>.</para>
        /// </summary>
        public readonly static BuildCacheStorageType Memory = new BuildCacheStorageType(1);
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>2</c>.</para>
        /// </summary>
        public readonly static BuildCacheStorageType Database = new BuildCacheStorageType(2);
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>10</c>.</para>
        /// <para>All the custom types must have values above this value.</para>
        /// </summary>
        public readonly static BuildCacheStorageType Custom = new BuildCacheStorageType(10);

        #endregion

        #region Private Fields

        /// <summary>
        /// The enumeration value of this structure.
        /// </summary>
        private int _typeValue;

        /// <summary>
        /// Defines the constant prefix to all custom items.
        /// </summary>
        private const string CustomPrefix = "CustomCacheStorage";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildCacheStorageType"/> structure
        /// with the specified integer value.
        /// </summary>
        /// <param name="value">
        /// The numerical value of this enumeration structure.
        /// </param>
        public BuildCacheStorageType(int value)
        {
            _typeValue = value;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the <see cref="BuildCacheStorageType.Value"/> 
        /// is <see langword="null"/>.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if <see cref="BuildCacheStorageType.Value"/> 
        /// is <see langword="null"/>; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsNull
        {
            get
            {
                return (_typeValue < 0);
            }
        }

        /// <summary>
        /// Gets the value of this <see cref="BuildCacheStorageType"/> structure.
        /// </summary>
        /// <value>
        /// An integer representing the value of this <see cref="BuildCacheStorageType"/> structure. 
        /// </value>
        public int Value
        {
            get
            {
                return _typeValue;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this structure represents a 
        /// user-defined or custom type.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if this structure is a
        /// user-defined type; otherwise, <see langword="false"/>.
        /// </value>
        /// <remarks>
        /// The user-defined or custom types have <see cref="BuildCacheStorageType.Value"/>
        /// greater than or equal to <c>10</c>.
        /// </remarks>
        public bool IsCustom
        {
            get
            {
                return (_typeValue >= 10);
            }
        }

        #endregion

        #region Public Operators

        /// <summary>
        /// Converts the specified <see cref="BuildCacheStorageType"/> structure to an 
        /// integer, which is its value.
        /// </summary>
        /// <param name="type">A <see cref="BuildCacheStorageType"/> to be converted.</param>
        /// <returns>The integer value of the specified <see cref="BuildCacheStorageType"/>.</returns>
        public static explicit operator int(BuildCacheStorageType type)
        {
            return type._typeValue;
        }

        /// <summary>
        /// Converts the specified integer to the equivalent 
        /// <see cref="BuildCacheStorageType"/> structure.
        /// </summary>
        /// <param name="value">An integer to be converted.</param>
        /// <returns>
        /// The <see cref="BuildCacheStorageType"/> whose value is specified in the integer
        /// parameter.
        /// </returns>
        public static explicit operator BuildCacheStorageType(int value)
        {
            if (value < 0)
            {
                return BuildCacheStorageType.Null;
            }

            switch (value)
            {
                case 0:
                    return BuildCacheStorageType.None;
                case 1:
                    return BuildCacheStorageType.Memory;
                case 2:
                    return BuildCacheStorageType.Database;
            }

            return new BuildCacheStorageType(value);
        }

        /// <summary>
        /// Performs a logical comparison of the two <see cref="BuildCacheStorageType"/> 
        /// parameters to determine whether they are equal.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildCacheStorageType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildCacheStorageType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameters,
        /// <paramref name="type1"/> and <paramref name="type2"/> represent the 
        /// same value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(BuildCacheStorageType type1, BuildCacheStorageType type2)
        {
            return type1._typeValue == type2._typeValue;
        }

        /// <summary>
        /// Performs a logical comparison of the two <see cref="BuildCacheStorageType"/> 
        /// parameters to determine whether they are not equal. 
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildCacheStorageType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildCacheStorageType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameters,
        /// <paramref name="type1"/> and <paramref name="type2"/> do not represent 
        /// the same value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(BuildCacheStorageType type1, BuildCacheStorageType type2)
        {
            return type1._typeValue != type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="BuildCacheStorageType"/> parameters to determine 
        /// whether the first is greater than the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildCacheStorageType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildCacheStorageType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is greater 
        /// than the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >(BuildCacheStorageType type1, BuildCacheStorageType type2)
        {
            return type1._typeValue > type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="BuildCacheStorageType"/> parameters to determine 
        /// whether the first is greater than or equal to the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildCacheStorageType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildCacheStorageType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is greater 
        /// than or equal to the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >=(BuildCacheStorageType type1, BuildCacheStorageType type2)
        {
            return type1._typeValue >= type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="BuildCacheStorageType"/> parameters to determine 
        /// whether the first is less than the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildCacheStorageType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildCacheStorageType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is less 
        /// than the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <(BuildCacheStorageType type1, BuildCacheStorageType type2)
        {
            return type1._typeValue < type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="BuildCacheStorageType"/> parameters to determine 
        /// whether the first is less than or equal to the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildCacheStorageType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildCacheStorageType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is less 
        /// than or equal to the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <=(BuildCacheStorageType type1, BuildCacheStorageType type2)
        {
            return type1._typeValue <= type2._typeValue;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts the string representation of the enumeration to 
        /// its <see cref="BuildCacheStorageType"/> equivalent.
        /// </summary>
        /// <param name="text">A string containing a enumeration to convert.</param>
        /// <returns>
        /// <para>
        /// A <see cref="BuildCacheStorageType"/> equivalent to the enumeration contained 
        /// in the specified parameter, <paramref name="text"/>.
        /// </para>
        /// <para>
        /// This will return <see cref="BuildCacheStorageType.Null"/> if the string is empty
        /// or <see langword="null"/>.
        /// </para>
        /// </returns>
        /// <exception cref="FormatException">
        /// If the parameter, <paramref name="text"/>, does not contain a valid 
        /// string representation of a <see cref="BuildCacheStorageType"/> structure.
        /// </exception>
        public static BuildCacheStorageType Parse(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return BuildCacheStorageType.Null;
            }

            switch (text.ToLower())
            {
                case "(null)":
                    return BuildCacheStorageType.Null;
                case "none":
                    return BuildCacheStorageType.None;
                case "memory":
                    return BuildCacheStorageType.Memory;
                case "database":
                    return BuildCacheStorageType.Database;
            }

            if (text.Length > CustomPrefix.Length &&
                text.StartsWith(CustomPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string customText = text.Substring(CustomPrefix.Length);
                int customIndex = Int32.Parse(customText);

                if (customIndex >= Custom._typeValue)
                {
                    return new BuildCacheStorageType(customIndex);
                }
            }

            throw new FormatException(
                "The specified parameter, text, is not a valid representation of this structure.");
        }

        /// <summary>
        /// Converts the specified string representation of an enumeration 
        /// to its <see cref="BuildCacheStorageType"/> equivalent and returns a value 
        /// that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="text">A string containing a enumeration to convert.</param>
        /// <param name="value">
        /// When this method returns, contains the <see cref="BuildCacheStorageType"/> value 
        /// equivalent to the enumeration contained in <paramref name="text"/>, 
        /// if the conversion succeeded, or <see cref="BuildCacheStorageType.Null"/> 
        /// if the conversion failed. 
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameter, 
        /// <paramref name="text"/>, is converted successfully; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public static bool TryParse(string text, out BuildCacheStorageType value)
        {
            value = BuildCacheStorageType.Null;

            if (String.IsNullOrEmpty(text))
            {
                return true;
            }

            switch (text.ToLower())
            {
                case "(null)":
                    value = BuildCacheStorageType.Null;
                    return true;
                case "none":
                    value = BuildCacheStorageType.None;
                    return true;
                case "memory":
                    value = BuildCacheStorageType.Memory;
                    return true;
                case "database":
                    value = BuildCacheStorageType.Database;
                    return true;
            }

            if (text.Length > CustomPrefix.Length &&
                text.StartsWith(CustomPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string customText = text.Substring(CustomPrefix.Length);
                int customIndex;
                if (Int32.TryParse(customText, out customIndex) &&
                    (customIndex >= Custom._typeValue))
                {
                    value = new BuildCacheStorageType(customIndex);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Converts this <see cref="BuildCacheStorageType"/> structure to its string
        /// representation, which is the enumeration name of this structure.
        /// </summary>
        /// <returns>
        /// <para>
        /// A string object equal to the value of this <see cref="BuildCacheStorageType"/>.
        /// </para>
        /// <para>
        /// The <see cref="BuildCacheStorageType.Null"/> instance will return <c>(Null)</c>.
        /// </para>
        /// <para>
        /// The <see cref="BuildCacheStorageType.Custom"/> instances will return a string
        /// starting with <c>CustomCacheStorage</c>.
        /// </para>
        /// </returns>
        public override string ToString()
        {
            if (_typeValue < 0)
            {
                return "(Null)";
            }

            switch (_typeValue)
            {
                case 0:
                    return "None";
                case 1:
                    return "Memory";
                case 2:
                    return "Database";
            }

            if (_typeValue >= 10)
            {
                return CustomPrefix + _typeValue.ToString();
            }

            return base.ToString();
        }

        #endregion

        #region IEquatable<BuildCacheStorageType> Members

        /// <overloads>
        /// Performs a logical comparison of two structures to determine whether 
        /// they are equal.
        /// </overloads>
        /// <summary>
        /// Performs a logical comparison of this and the specified 
        /// <see cref="BuildCacheStorageType"/> parameter to determine whether 
        /// they are equal. 
        /// </summary>
        /// <param name="other">The <see cref="BuildCacheStorageType"/> to be compared.</param>
        /// <returns>
        /// This returns <see langword="true"/> if this and the specified 
        /// <see cref="BuildCacheStorageType"/> are equal; otherwise <see langword="false"/>.
        /// </returns>
        public bool Equals(BuildCacheStorageType other)
        {
            return (_typeValue == other._typeValue);
        }

        /// <summary>
        /// Compares the specified object parameter to the <see cref="BuildCacheStorageType.Value"/> 
        /// property of the <see cref="BuildCacheStorageType"/> object.
        /// </summary>
        /// <param name="obj">The object to be compared.</param>
        /// <returns>
        /// This returns <see langword="true"/> if object is an instance of 
        /// <see cref="BuildCacheStorageType"/> and the two are equal; otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is BuildCacheStorageType)
            {
                return this.Equals((BuildCacheStorageType)obj);
            }

            return false;
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        public override int GetHashCode()
        {
            return _typeValue.GetHashCode();
        }

        #endregion

        #region IComparable<BuildCacheStorageType> Members

        /// <summary>
        /// Compares the current <see cref="BuildCacheStorageType"/> with the specified
        /// <see cref="BuildCacheStorageType"/> structure.
        /// </summary>
        /// <param name="other">
        /// The <see cref="BuildCacheStorageType"/> structure to compare with this structure.
        /// </param>
        /// <returns>
        /// <para>
        /// </para>
        /// A 32-bit signed integer that indicates the relative order of the 
        /// objects being compared. 
        /// <para>
        /// The return value has the following meanings:
        /// </para>
        /// <list type="table">
        /// <listheader>
        /// <term>Value</term>
        /// <term>Meaning</term>
        /// </listheader>
        /// <item>
        /// <description>Less than zero</description>
        /// <description>This object is less than the <paramref name="other"/> parameter.</description>
        /// </item>
        /// <item>
        /// <description>Zero</description>
        /// <description>This object is equal to <paramref name="other"/> parameter.</description>
        /// </item>
        /// <item>
        /// <description>Greater than zero</description>
        /// <description>This object is greater than <paramref name="other"/> parameter.</description>
        /// </item>
        /// </list>
        /// </returns>
        public int CompareTo(BuildCacheStorageType other)
        {
            return this._typeValue.CompareTo(other._typeValue);
        }

        #endregion
    }
}
