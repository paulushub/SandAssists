using System;

namespace Sandcastle
{            
    /// <summary>
    /// This specifies the build output format.
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
    /// <description><see cref="BuildFormatType.Null"/></description>
    /// <description>-1</description>
    /// <description>Indicates uninitialized build output format.</description>
    /// </item>
    /// <item>
    /// <description><see cref="BuildFormatType.None"/></description>
    /// <description>0</description>
    /// <description>Indicates unknown or unspecified build output format.</description>
    /// </item>
    /// <item>
    /// <description><see cref="BuildFormatType.WebHelp"/></description>
    /// <description>1</description>
    /// <description>Indicates the web-help output format.</description>
    /// </item>
    /// <item>
    /// <description><see cref="BuildFormatType.HtmlHelp1"/></description>
    /// <description>2</description>
    /// <description>Indicates the HelpHelp 1.x output format.</description>
    /// </item>
    /// <item>
    /// <description><see cref="BuildFormatType.HtmlHelp2"/></description>
    /// <description>3</description>
    /// <description>Indicates the HelpHelp 2.x output format.</description>
    /// </item>
    /// <item>
    /// <description><see cref="BuildFormatType.HtmlHelp3"/></description>
    /// <description>4</description>
    /// <description>Indicates the HelpHelp 3.x or the Help Viewer 1.0 output format.</description>
    /// </item>
    /// <item>
    /// <description><see cref="BuildFormatType.Custom"/></description>
    /// <description>10</description>
    /// <description>Indicates a user-defined or custom output format.</description>
    /// </item>
    /// </list>
    /// </remarks>
    [Serializable]
    public struct BuildFormatType : IEquatable<BuildFormatType>,
        IComparable<BuildFormatType>
    {
        #region Public Enumerations

        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>-1</c>.</para>
        /// </summary>
        public readonly static BuildFormatType Null     = new BuildFormatType(-1);
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>0</c>.</para>
        /// </summary>
        public readonly static BuildFormatType None     = new BuildFormatType(0);
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>1</c>.</para>
        /// </summary>
        public readonly static BuildFormatType WebHelp   = new BuildFormatType(1);
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>2</c>.</para>
        /// </summary>
        public readonly static BuildFormatType HtmlHelp1 = new BuildFormatType(2);
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>3</c>.</para>
        /// </summary>
        public readonly static BuildFormatType HtmlHelp2 = new BuildFormatType(3);
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>4</c>.</para>
        /// </summary>
        public readonly static BuildFormatType HtmlHelp3 = new BuildFormatType(4);
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>10</c>.</para>
        /// <para>All the custom types must have values above this value.</para>
        /// </summary>
        public readonly static BuildFormatType Custom    = new BuildFormatType(10);

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
        /// Initializes a new instance of the <see cref="BuildFormatType"/> structure
        /// with the specified integer value.
        /// </summary>
        /// <param name="value">
        /// The numerical value of this enumeration structure.
        /// </param>
        public BuildFormatType(int value)
        {
            _typeValue = value;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the <see cref="BuildFormatType.Value"/> 
        /// is <see langword="null"/>.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if <see cref="BuildFormatType.Value"/> 
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
        /// Gets the value of this <see cref="BuildFormatType"/> structure.
        /// </summary>
        /// <value>
        /// An integer representing the value of this <see cref="BuildFormatType"/> structure. 
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
        /// The user-defined or custom types have <see cref="BuildFormatType.Value"/>
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
        /// Converts the specified <see cref="BuildFormatType"/> structure to an 
        /// integer, which is its value.
        /// </summary>
        /// <param name="type">A <see cref="BuildFormatType"/> to be converted.</param>
        /// <returns>The integer value of the specified <see cref="BuildFormatType"/>.</returns>
        public static explicit operator int(BuildFormatType type)
        {
            return type._typeValue;
        }

        /// <summary>
        /// Converts the specified integer to the equivalent 
        /// <see cref="BuildFormatType"/> structure.
        /// </summary>
        /// <param name="value">An integer to be converted.</param>
        /// <returns>
        /// The <see cref="BuildFormatType"/> whose value is specified in the integer
        /// parameter.
        /// </returns>
        public static explicit operator BuildFormatType(int value)
        {
            if (value < 0)
            {
                return BuildFormatType.Null;
            }

            switch (value)
            {
                case 0:
                    return BuildFormatType.None;
                case 1:
                    return BuildFormatType.WebHelp;
                case 2:
                    return BuildFormatType.HtmlHelp1;
                case 3:
                    return BuildFormatType.HtmlHelp2;
                case 4:
                    return BuildFormatType.HtmlHelp3;
            }

            return new BuildFormatType(value);
        }

        /// <summary>
        /// Performs a logical comparison of the two <see cref="BuildFormatType"/> 
        /// parameters to determine whether they are equal.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildFormatType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildFormatType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameters,
        /// <paramref name="type1"/> and <paramref name="type2"/> represent the 
        /// same value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(BuildFormatType type1, BuildFormatType type2)
        {
            return type1._typeValue == type2._typeValue;
        }

        /// <summary>
        /// Performs a logical comparison of the two <see cref="BuildFormatType"/> 
        /// parameters to determine whether they are not equal. 
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildFormatType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildFormatType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameters,
        /// <paramref name="type1"/> and <paramref name="type2"/> do not represent 
        /// the same value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(BuildFormatType type1, BuildFormatType type2)
        {
            return type1._typeValue != type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="BuildFormatType"/> parameters to determine 
        /// whether the first is greater than the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildFormatType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildFormatType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is greater 
        /// than the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >(BuildFormatType type1, BuildFormatType type2)
        {
            return type1._typeValue > type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="BuildFormatType"/> parameters to determine 
        /// whether the first is greater than or equal to the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildFormatType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildFormatType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is greater 
        /// than or equal to the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >=(BuildFormatType type1, BuildFormatType type2)
        {
            return type1._typeValue >= type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="BuildFormatType"/> parameters to determine 
        /// whether the first is less than the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildFormatType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildFormatType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is less 
        /// than the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <(BuildFormatType type1, BuildFormatType type2)
        {
            return type1._typeValue < type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="BuildFormatType"/> parameters to determine 
        /// whether the first is less than or equal to the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildFormatType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildFormatType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is less 
        /// than or equal to the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <=(BuildFormatType type1, BuildFormatType type2)
        {
            return type1._typeValue <= type2._typeValue;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts the string representation of the enumeration to 
        /// its <see cref="BuildFormatType"/> equivalent.
        /// </summary>
        /// <param name="text">A string containing a enumeration to convert.</param>
        /// <returns>
        /// <para>
        /// A <see cref="BuildFormatType"/> equivalent to the enumeration contained 
        /// in the specified parameter, <paramref name="text"/>.
        /// </para>
        /// <para>
        /// This will return <see cref="BuildFormatType.Null"/> if the string is empty
        /// or <see langword="null"/>.
        /// </para>
        /// </returns>
        /// <exception cref="FormatException">
        /// If the parameter, <paramref name="text"/>, does not contain a valid 
        /// string representation of a <see cref="BuildFormatType"/> structure.
        /// </exception>
        public static BuildFormatType Parse(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return BuildFormatType.Null;
            }

            switch (text.ToLower())
            {
                case "(null)":
                    return BuildFormatType.Null;
                case "none":
                    return BuildFormatType.None;
                case "webhelp":
                    return BuildFormatType.WebHelp;
                case "htmlhelp1":
                    return BuildFormatType.HtmlHelp1;
                case "htmlhelp2":
                    return BuildFormatType.HtmlHelp2;
                case "htmlhelp3":
                    return BuildFormatType.HtmlHelp3;
            }

            if (text.Length > CustomPrefix.Length &&
                text.StartsWith(CustomPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string customText = text.Substring(CustomPrefix.Length);
                int customIndex = Int32.Parse(customText);

                if (customIndex >= Custom._typeValue)
                {
                    return new BuildFormatType(customIndex);
                }
            }

            throw new FormatException(
                "The specified parameter, text, is not a valid representation of this structure.");
        }

        /// <summary>
        /// Converts the specified string representation of an enumeration 
        /// to its <see cref="BuildFormatType"/> equivalent and returns a value 
        /// that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="text">A string containing a enumeration to convert.</param>
        /// <param name="value">
        /// When this method returns, contains the <see cref="BuildFormatType"/> value 
        /// equivalent to the enumeration contained in <paramref name="text"/>, 
        /// if the conversion succeeded, or <see cref="BuildFormatType.Null"/> 
        /// if the conversion failed. 
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameter, 
        /// <paramref name="text"/>, is converted successfully; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public static bool TryParse(string text, out BuildFormatType value)
        {
            value = BuildFormatType.Null;

            if (String.IsNullOrEmpty(text))
            {
                return true;
            }

            switch (text.ToLower())
            {
                case "(null)":
                    value = BuildFormatType.Null;
                    return true;
                case "none":
                    value = BuildFormatType.None;
                    return true;
                case "webhelp":
                    value = BuildFormatType.WebHelp;
                    return true;
                case "htmlhelp1":
                    value = BuildFormatType.HtmlHelp1;
                    return true;
                case "htmlhelp2":
                    value = BuildFormatType.HtmlHelp2;
                    return true;
                case "htmlhelp3":
                    value = BuildFormatType.HtmlHelp3;
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
                    value = new BuildFormatType(customIndex);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Converts this <see cref="BuildFormatType"/> structure to its string
        /// representation, which is the enumeration name of this structure.
        /// </summary>
        /// <returns>
        /// <para>
        /// A string object equal to the value of this <see cref="BuildFormatType"/>.
        /// </para>
        /// <para>
        /// The <see cref="BuildFormatType.Null"/> instance will return <c>(Null)</c>.
        /// </para>
        /// <para>
        /// The <see cref="BuildFormatType.Custom"/> instances will return a string
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
                    return "WebHelp";
                case 2:
                    return "HtmlHelp1";
                case 3:
                    return "HtmlHelp2";
                case 4:
                    return "HtmlHelp3";
            }

            if (_typeValue >= 10)
            {
                return CustomPrefix + _typeValue.ToString();
            }

            return base.ToString();
        }

        #endregion

        #region IEquatable<BuildFormatType> Members

        /// <overloads>
        /// Performs a logical comparison of two structures to determine whether 
        /// they are equal.
        /// </overloads>
        /// <summary>
        /// Performs a logical comparison of this and the specified 
        /// <see cref="BuildFormatType"/> parameter to determine whether 
        /// they are equal. 
        /// </summary>
        /// <param name="other">The <see cref="BuildFormatType"/> to be compared.</param>
        /// <returns>
        /// This returns <see langword="true"/> if this and the specified 
        /// <see cref="BuildFormatType"/> are equal; otherwise <see langword="false"/>.
        /// </returns>
        public bool Equals(BuildFormatType other)
        {
            return (_typeValue == other._typeValue);
        }

        /// <summary>
        /// Compares the specified object parameter to the <see cref="BuildFormatType.Value"/> 
        /// property of the <see cref="BuildFormatType"/> object.
        /// </summary>
        /// <param name="obj">The object to be compared.</param>
        /// <returns>
        /// This returns <see langword="true"/> if object is an instance of 
        /// <see cref="BuildFormatType"/> and the two are equal; otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is BuildFormatType)
            {
                return this.Equals((BuildFormatType)obj);
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

        #region IComparable<BuildFormatType> Members

        /// <summary>
        /// Compares the current <see cref="BuildFormatType"/> with the specified
        /// <see cref="BuildFormatType"/> structure.
        /// </summary>
        /// <param name="other">
        /// The <see cref="BuildFormatType"/> structure to compare with this structure.
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
        public int CompareTo(BuildFormatType other)
        {
            return this._typeValue.CompareTo(other._typeValue);
        }

        #endregion
    }
}
