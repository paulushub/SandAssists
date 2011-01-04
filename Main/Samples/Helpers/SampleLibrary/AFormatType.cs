using System;

namespace TestLibrary
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
    /// <description><see cref="AFormatType.Null"/></description>
    /// <description>-1</description>
    /// <description>Null</description>
    /// </item>
    /// <item>
    /// <description><see cref="AFormatType.None"/></description>
    /// <description>0</description>
    /// <description>None</description>
    /// </item>
    /// <item>
    /// <description><see cref="AFormatType.ItemA"/></description>
    /// <description>1</description>
    /// <description>A</description>
    /// </item>
    /// <item>
    /// <description><see cref="AFormatType.ItemB"/></description>
    /// <description>2</description>
    /// <description>B</description>
    /// </item>
    /// <item>
    /// <description><see cref="AFormatType.ItemC"/></description>
    /// <description>3</description>
    /// <description>C</description>
    /// </item>
    /// <item>
    /// <description><see cref="AFormatType.ItemD"/></description>
    /// <description>4</description>
    /// <description>D</description>
    /// </item>
    /// <item>
    /// <description><see cref="AFormatType.Custom"/></description>
    /// <description>10</description>
    /// <description>Custom</description>
    /// </item>
    /// </list>
    /// </remarks>
    [Serializable]
    public struct AFormatType : IEquatable<AFormatType>, 
        IComparable<AFormatType>
    {
        #region Public Enumerations

        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>-1</c>.</para>
        /// </summary>
        public readonly static AFormatType Null   = new AFormatType(-1);
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>0</c>.</para>
        /// </summary>
        public readonly static AFormatType None   = new AFormatType(0);
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>1</c>.</para>
        /// </summary>
        public readonly static AFormatType ItemA  = new AFormatType(1);
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>2</c>.</para>
        /// </summary>
        public readonly static AFormatType ItemB  = new AFormatType(2);
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>3</c>.</para>
        /// </summary>
        public readonly static AFormatType ItemC  = new AFormatType(3);
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>4</c>.</para>
        /// </summary>
        public readonly static AFormatType ItemD  = new AFormatType(4);
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>10</c>.</para>
        /// <para>All the custom types must have values above this value.</para>
        /// </summary>
        public readonly static AFormatType Custom = new AFormatType(10);

        #endregion

        #region Private Fields

        /// <summary>
        /// The enumeration value of this structure.
        /// </summary>
        private int _typeValue;

        /// <summary>
        /// Defines the constant prefix to all custom items.
        /// </summary>
        private const string CustomPrefix = "CustomFormat";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="AFormatType"/> structure
        /// with the specified integer value.
        /// </summary>
        /// <param name="value">
        /// The numerical value of this enumeration structure.
        /// </param>
        public AFormatType(int value)
        {
            _typeValue = value;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the <see cref="AFormatType.Value"/> 
        /// is <see langword="null"/>.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if <see cref="AFormatType.Value"/> 
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
        /// Gets the value of this <see cref="AFormatType"/> structure.
        /// </summary>
        /// <value>
        /// An integer representing the value of this <see cref="AFormatType"/> structure. 
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
        /// The user-defined or custom types have <see cref="AFormatType.Value"/>
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
        /// Converts the specified <see cref="AFormatType"/> structure to an 
        /// integer, which is its value.
        /// </summary>
        /// <param name="type">A <see cref="AFormatType"/> to be converted.</param>
        /// <returns>The integer value of the specified <see cref="AFormatType"/>.</returns>
        public static explicit operator int(AFormatType type)
        {
            return type._typeValue;
        }

        /// <summary>
        /// Converts the specified integer to the equivalent 
        /// <see cref="AFormatType"/> structure.
        /// </summary>
        /// <param name="value">An integer to be converted.</param>
        /// <returns>
        /// The <see cref="AFormatType"/> whose value is specified in the integer
        /// parameter.
        /// </returns>
        public static explicit operator AFormatType(int value)
        {
            if (value < 0)
            {
                return AFormatType.Null;
            }

            switch (value)
            {
                case 0:
                    return AFormatType.None;
                case 1:
                    return AFormatType.ItemA;
                case 2:
                    return AFormatType.ItemB;
                case 3:
                    return AFormatType.ItemC;
                case 4:
                    return AFormatType.ItemD;
            }

            return new AFormatType(value);
        }

        /// <summary>
        /// Performs a logical comparison of the two <see cref="AFormatType"/> 
        /// parameters to determine whether they are equal.
        /// </summary>
        /// <param name="type1">An instance of <see cref="AFormatType"/>.</param>
        /// <param name="type2">An instance of <see cref="AFormatType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameters,
        /// <paramref name="type1"/> and <paramref name="type2"/> represent the 
        /// same value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator==(AFormatType type1, AFormatType type2)
        {
            return type1._typeValue == type2._typeValue;
        }

        /// <summary>
        /// Performs a logical comparison of the two <see cref="AFormatType"/> 
        /// parameters to determine whether they are not equal. 
        /// </summary>
        /// <param name="type1">An instance of <see cref="AFormatType"/>.</param>
        /// <param name="type2">An instance of <see cref="AFormatType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameters,
        /// <paramref name="type1"/> and <paramref name="type2"/> do not represent 
        /// the same value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(AFormatType type1, AFormatType type2)
        {
            return type1._typeValue != type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="AFormatType"/> parameters to determine 
        /// whether the first is greater than the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="AFormatType"/>.</param>
        /// <param name="type2">An instance of <see cref="AFormatType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is greater 
        /// than the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >(AFormatType type1, AFormatType type2)
        {
            return type1._typeValue > type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="AFormatType"/> parameters to determine 
        /// whether the first is greater than or equal to the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="AFormatType"/>.</param>
        /// <param name="type2">An instance of <see cref="AFormatType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is greater 
        /// than or equal to the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >=(AFormatType type1, AFormatType type2)
        {
            return type1._typeValue >= type2._typeValue;
        }   

        /// <summary>
        /// Compares the two <see cref="AFormatType"/> parameters to determine 
        /// whether the first is less than the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="AFormatType"/>.</param>
        /// <param name="type2">An instance of <see cref="AFormatType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is less 
        /// than the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <(AFormatType type1, AFormatType type2)
        {
            return type1._typeValue < type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="AFormatType"/> parameters to determine 
        /// whether the first is less than or equal to the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="AFormatType"/>.</param>
        /// <param name="type2">An instance of <see cref="AFormatType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is less 
        /// than or equal to the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <=(AFormatType type1, AFormatType type2)
        {
            return type1._typeValue <= type2._typeValue;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts the string representation of the enumeration to 
        /// its <see cref="AFormatType"/> equivalent.
        /// </summary>
        /// <param name="text">A string containing a enumeration to convert.</param>
        /// <returns>
        /// <para>
        /// A <see cref="AFormatType"/> equivalent to the enumeration contained 
        /// in the specified parameter, <paramref name="text"/>.
        /// </para>
        /// <para>
        /// This will return <see cref="AFormatType.Null"/> if the string is empty
        /// or <see langword="null"/>.
        /// </para>
        /// </returns>
        /// <exception cref="FormatException">
        /// If the parameter, <paramref name="text"/>, does not contain a valid 
        /// string representation of a <see cref="AFormatType"/> structure.
        /// </exception>
        public static AFormatType Parse(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return AFormatType.Null;
            }

            switch (text.ToLower())
            {
                case "(null)":
                    return AFormatType.Null;
                case "none":
                    return AFormatType.None;
                case "itema":
                    return AFormatType.ItemA;
                case "itemb":
                    return AFormatType.ItemB;
                case "itemc":
                    return AFormatType.ItemC;
                case "itemd":
                    return AFormatType.ItemD;
            }

            if (text.Length > CustomPrefix.Length &&
                text.StartsWith(CustomPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string customText = text.Substring(CustomPrefix.Length);
                int customIndex   = Int32.Parse(customText);

                if (customIndex >= Custom._typeValue)
                {
                    return new AFormatType(customIndex);
                }
            }

            throw new FormatException(
                "The specified parameter, text, is not a valid representation of this structure.");
        }

        /// <summary>
        /// Converts the specified string representation of an enumeration 
        /// to its <see cref="AFormatType"/> equivalent and returns a value 
        /// that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="text">A string containing a enumeration to convert.</param>
        /// <param name="value">
        /// When this method returns, contains the <see cref="AFormatType"/> value 
        /// equivalent to the enumeration contained in <paramref name="text"/>, 
        /// if the conversion succeeded, or <see cref="AFormatType.Null"/> 
        /// if the conversion failed. 
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameter, 
        /// <paramref name="text"/>, is converted successfully; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public static bool TryParse(string text, out AFormatType value)
        {
            value = AFormatType.Null;

            if (String.IsNullOrEmpty(text))
            {
                return true;
            }

            switch (text.ToLower())
            {
                case "(null)":
                    value = AFormatType.Null;
                    return true;
                case "none":
                    value = AFormatType.None;
                    return true;
                case "itema":
                    value = AFormatType.ItemA;
                    return true;
                case "itemb":
                    value = AFormatType.ItemB;
                    return true;
                case "itemc":
                    value = AFormatType.ItemC;
                    return true;
                case "itemd":
                    value = AFormatType.ItemD;
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
                    value = new AFormatType(customIndex);
                    return true;
               }
            }

            return false;
        }

        /// <summary>
        /// Converts this <see cref="AFormatType"/> structure to its string
        /// representation, which is the enumeration name of this structure.
        /// </summary>
        /// <returns>
        /// <para>
        /// A string object equal to the value of this <see cref="AFormatType"/>.
        /// </para>
        /// <para>
        /// The <see cref="AFormatType.Null"/> instance will return <c>(Null)</c>.
        /// </para>
        /// <para>
        /// The <see cref="AFormatType.Custom"/> instances will return a string
        /// starting with <c>CustomFormat</c>.
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
                    return "ItemA";
                case 2:
                    return "ItemB";
                case 3:
                    return "ItemC";
                case 4:
                    return "ItemD";
            }

            if (_typeValue >= 10)
            {
                return CustomPrefix + _typeValue.ToString();
            }

            return base.ToString();
        }

        #endregion

        #region IEquatable<AFormatType> Members

        /// <overloads>
        /// Performs a logical comparison of two structures to determine whether 
        /// they are equal.
        /// </overloads>
        /// <summary>
        /// Performs a logical comparison of this and the specified 
        /// <see cref="AFormatType"/> parameter to determine whether 
        /// they are equal. 
        /// </summary>
        /// <param name="other">The <see cref="AFormatType"/> to be compared.</param>
        /// <returns>
        /// This returns <see langword="true"/> if this and the specified 
        /// <see cref="AFormatType"/> are equal; otherwise <see langword="false"/>.
        /// </returns>
        public bool Equals(AFormatType other)
        {
            return (_typeValue == other._typeValue);
        }

        /// <summary>
        /// Compares the specified object parameter to the <see cref="AFormatType.Value"/> 
        /// property of the <see cref="AFormatType"/> object.
        /// </summary>
        /// <param name="obj">The object to be compared.</param>
        /// <returns>
        /// This returns <see langword="true"/> if object is an instance of 
        /// <see cref="AFormatType"/> and the two are equal; otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is AFormatType)
            {
                return this.Equals((AFormatType)obj);
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

        #region IComparable<AFormatType> Members

        /// <summary>
        /// Compares the current <see cref="AFormatType"/> with the specified
        /// <see cref="AFormatType"/> structure.
        /// </summary>
        /// <param name="other">
        /// The <see cref="AFormatType"/> structure to compare with this structure.
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
        public int CompareTo(AFormatType other)
        {
            return this._typeValue.CompareTo(other._typeValue);
        }

        #endregion
    }
}
