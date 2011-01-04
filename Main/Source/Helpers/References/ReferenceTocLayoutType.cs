using System;

namespace Sandcastle.References
{
    /// <summary>
    /// This specifies the layout type for the reference table of contents.
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
    /// <description><see cref="ReferenceTocLayoutType.Null"/></description>
    /// <description>-1</description>
    /// <description>
    /// This indicates an uninitialized reference table of contents layout type.
    /// </description>
    /// </item>
    /// <item>
    /// <description><see cref="ReferenceTocLayoutType.None"/></description>
    /// <description>0</description>
    /// <description>
    /// This indicates an unknown or default reference table of contents 
    /// layout type.
    /// </description>
    /// </item>
    /// <item>
    /// <description><see cref="ReferenceTocLayoutType.Flat"/></description>
    /// <description>1</description>
    /// <description>
    /// This indicates the flat namespaces layout for the reference table
    /// of contents.
    /// </description>
    /// </item>
    /// <item>
    /// <description><see cref="ReferenceTocLayoutType.Hierarchical"/></description>
    /// <description>2</description>
    /// <description>
    /// This indicates the hierarchical namespaces layout for the reference 
    /// table of contents.
    /// </description>
    /// </item>
    /// <item>
    /// <description><see cref="ReferenceTocLayoutType.Custom"/></description>
    /// <description>10</description>
    /// <description>
    /// This indicates a custom or user-defined namespaces layout for the 
    /// references table of contents.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    [Serializable]
    public struct ReferenceTocLayoutType : IEquatable<ReferenceTocLayoutType>,
        IComparable<ReferenceTocLayoutType>
    {
        #region Public Enumerations

        /// <summary>
        /// <para>
        /// This indicates an uninitialized reference table of contents layout type.
        /// </para>
        /// <para>The enumeration value is <c>-1</c>.</para>
        /// </summary>
        public readonly static ReferenceTocLayoutType Null = new ReferenceTocLayoutType(-1);
        /// <summary>
        /// <para>
        /// This indicates an unknown or default reference table of contents 
        /// layout type.
        /// </para>
        /// <para>The enumeration value is <c>0</c>.</para>
        /// </summary>
        public readonly static ReferenceTocLayoutType None = new ReferenceTocLayoutType(0);
        /// <summary>
        /// <para>
        /// This indicates the flat namespaces layout for the reference table
        /// of contents.
        /// </para>
        /// <para>The enumeration value is <c>1</c>.</para>
        /// </summary>
        public readonly static ReferenceTocLayoutType Flat = new ReferenceTocLayoutType(1);
        /// <summary>
        /// <para>
        /// This indicates the hierarchical namespaces layout for the reference 
        /// table of contents.
        /// </para>
        /// <para>The enumeration value is <c>2</c>.</para>
        /// </summary>
        public readonly static ReferenceTocLayoutType Hierarchical = new ReferenceTocLayoutType(2);
        /// <summary>
        /// <para>
        /// This indicates a custom or user-defined namespaces layout for the 
        /// references table of contents.
        /// </para>
        /// <para>The enumeration value is <c>10</c>.</para>
        /// <para>All the custom types must have values above this value.</para>
        /// </summary>
        public readonly static ReferenceTocLayoutType Custom = new ReferenceTocLayoutType(10);

        #endregion

        #region Private Fields

        /// <summary>
        /// The enumeration value of this structure.
        /// </summary>
        private int _typeValue;

        /// <summary>
        /// Defines the constant prefix to all custom items.
        /// </summary>
        private const string CustomPrefix = "CustomLayoutToc";

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceTocLayoutType"/> structure
        /// with the specified integer value.
        /// </summary>
        /// <param name="value">
        /// The numerical value of this enumeration structure.
        /// </param>
        public ReferenceTocLayoutType(int value)
        {
            _typeValue = value;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the <see cref="ReferenceTocLayoutType.Value"/> 
        /// is <see langword="null"/>.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if <see cref="ReferenceTocLayoutType.Value"/> 
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
        /// Gets the value of this <see cref="ReferenceTocLayoutType"/> structure.
        /// </summary>
        /// <value>
        /// An integer representing the value of this <see cref="ReferenceTocLayoutType"/> structure. 
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
        /// The user-defined or custom types have <see cref="ReferenceTocLayoutType.Value"/>
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
        /// Converts the specified <see cref="ReferenceTocLayoutType"/> structure to an 
        /// integer, which is its value.
        /// </summary>
        /// <param name="type">A <see cref="ReferenceTocLayoutType"/> to be converted.</param>
        /// <returns>The integer value of the specified <see cref="ReferenceTocLayoutType"/>.</returns>
        public static explicit operator int(ReferenceTocLayoutType type)
        {
            return type._typeValue;
        }

        /// <summary>
        /// Converts the specified integer to the equivalent 
        /// <see cref="ReferenceTocLayoutType"/> structure.
        /// </summary>
        /// <param name="value">An integer to be converted.</param>
        /// <returns>
        /// The <see cref="ReferenceTocLayoutType"/> whose value is specified in the integer
        /// parameter.
        /// </returns>
        public static explicit operator ReferenceTocLayoutType(int value)
        {
            if (value < 0)
            {
                return ReferenceTocLayoutType.Null;
            }

            switch (value)
            {
                case 0:
                    return ReferenceTocLayoutType.None;
                case 1:
                    return ReferenceTocLayoutType.Flat;
                case 2:
                    return ReferenceTocLayoutType.Hierarchical;
            }

            return new ReferenceTocLayoutType(value);
        }

        /// <summary>
        /// Performs a logical comparison of the two <see cref="ReferenceTocLayoutType"/> 
        /// parameters to determine whether they are equal.
        /// </summary>
        /// <param name="type1">An instance of <see cref="ReferenceTocLayoutType"/>.</param>
        /// <param name="type2">An instance of <see cref="ReferenceTocLayoutType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameters,
        /// <paramref name="type1"/> and <paramref name="type2"/> represent the 
        /// same value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(ReferenceTocLayoutType type1, ReferenceTocLayoutType type2)
        {
            return type1._typeValue == type2._typeValue;
        }

        /// <summary>
        /// Performs a logical comparison of the two <see cref="ReferenceTocLayoutType"/> 
        /// parameters to determine whether they are not equal. 
        /// </summary>
        /// <param name="type1">An instance of <see cref="ReferenceTocLayoutType"/>.</param>
        /// <param name="type2">An instance of <see cref="ReferenceTocLayoutType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameters,
        /// <paramref name="type1"/> and <paramref name="type2"/> do not represent 
        /// the same value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(ReferenceTocLayoutType type1, ReferenceTocLayoutType type2)
        {
            return type1._typeValue != type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="ReferenceTocLayoutType"/> parameters to determine 
        /// whether the first is greater than the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="ReferenceTocLayoutType"/>.</param>
        /// <param name="type2">An instance of <see cref="ReferenceTocLayoutType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is greater 
        /// than the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >(ReferenceTocLayoutType type1, ReferenceTocLayoutType type2)
        {
            return type1._typeValue > type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="ReferenceTocLayoutType"/> parameters to determine 
        /// whether the first is greater than or equal to the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="ReferenceTocLayoutType"/>.</param>
        /// <param name="type2">An instance of <see cref="ReferenceTocLayoutType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is greater 
        /// than or equal to the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >=(ReferenceTocLayoutType type1, ReferenceTocLayoutType type2)
        {
            return type1._typeValue >= type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="ReferenceTocLayoutType"/> parameters to determine 
        /// whether the first is less than the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="ReferenceTocLayoutType"/>.</param>
        /// <param name="type2">An instance of <see cref="ReferenceTocLayoutType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is less 
        /// than the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <(ReferenceTocLayoutType type1, ReferenceTocLayoutType type2)
        {
            return type1._typeValue < type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="ReferenceTocLayoutType"/> parameters to determine 
        /// whether the first is less than or equal to the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="ReferenceTocLayoutType"/>.</param>
        /// <param name="type2">An instance of <see cref="ReferenceTocLayoutType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is less 
        /// than or equal to the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <=(ReferenceTocLayoutType type1, ReferenceTocLayoutType type2)
        {
            return type1._typeValue <= type2._typeValue;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts the string representation of the enumeration to 
        /// its <see cref="ReferenceTocLayoutType"/> equivalent.
        /// </summary>
        /// <param name="text">A string containing a enumeration to convert.</param>
        /// <returns>
        /// <para>
        /// A <see cref="ReferenceTocLayoutType"/> equivalent to the enumeration contained 
        /// in the specified parameter, <paramref name="text"/>.
        /// </para>
        /// <para>
        /// This will return <see cref="ReferenceTocLayoutType.Null"/> if the string is empty
        /// or <see langword="null"/>.
        /// </para>
        /// </returns>
        /// <exception cref="FormatException">
        /// If the parameter, <paramref name="text"/>, does not contain a valid 
        /// string representation of a <see cref="ReferenceTocLayoutType"/> structure.
        /// </exception>
        public static ReferenceTocLayoutType Parse(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return ReferenceTocLayoutType.Null;
            }

            switch (text.ToLower())
            {
                case "(null)":
                    return ReferenceTocLayoutType.Null;
                case "none":
                    return ReferenceTocLayoutType.None;
                case "flat":
                    return ReferenceTocLayoutType.Flat;
                case "hierarchical":
                    return ReferenceTocLayoutType.Hierarchical;
            }

            if (text.Length > CustomPrefix.Length &&
                text.StartsWith(CustomPrefix, StringComparison.OrdinalIgnoreCase))
            {
                string customText = text.Substring(CustomPrefix.Length);
                int customIndex = Int32.Parse(customText);

                if (customIndex >= Custom._typeValue)
                {
                    return new ReferenceTocLayoutType(customIndex);
                }
            }

            throw new FormatException(
                "The specified parameter, text, is not a valid representation of this structure.");
        }

        /// <summary>
        /// Converts the specified string representation of an enumeration 
        /// to its <see cref="ReferenceTocLayoutType"/> equivalent and returns a value 
        /// that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="text">A string containing a enumeration to convert.</param>
        /// <param name="value">
        /// When this method returns, contains the <see cref="ReferenceTocLayoutType"/> value 
        /// equivalent to the enumeration contained in <paramref name="text"/>, 
        /// if the conversion succeeded, or <see cref="ReferenceTocLayoutType.Null"/> 
        /// if the conversion failed. 
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameter, 
        /// <paramref name="text"/>, is converted successfully; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public static bool TryParse(string text, out ReferenceTocLayoutType value)
        {
            value = ReferenceTocLayoutType.Null;

            if (String.IsNullOrEmpty(text))
            {
                return true;
            }

            switch (text.ToLower())
            {
                case "(null)":
                    value = ReferenceTocLayoutType.Null;
                    return true;
                case "none":
                    value = ReferenceTocLayoutType.None;
                    return true;
                case "flat":
                    value = ReferenceTocLayoutType.Flat;
                    return true;
                case "hierarchical":
                    value = ReferenceTocLayoutType.Hierarchical;
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
                    value = new ReferenceTocLayoutType(customIndex);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Converts this <see cref="ReferenceTocLayoutType"/> structure to its string
        /// representation, which is the enumeration name of this structure.
        /// </summary>
        /// <returns>
        /// <para>
        /// A string object equal to the value of this <see cref="ReferenceTocLayoutType"/>.
        /// </para>
        /// <para>
        /// The <see cref="ReferenceTocLayoutType.Null"/> instance will return <c>(Null)</c>.
        /// </para>
        /// <para>
        /// The <see cref="ReferenceTocLayoutType.Custom"/> instances will return a string
        /// starting with <c>CustomLayoutToc</c>.
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
                    return "Flat";
                case 2:
                    return "Hierarchical";
            }

            if (_typeValue >= 10)
            {
                return CustomPrefix + _typeValue.ToString();
            }

            return base.ToString();
        }

        #endregion

        #region IEquatable<ReferenceTocLayoutType> Members

        /// <overloads>
        /// Performs a logical comparison of two structures to determine whether 
        /// they are equal.
        /// </overloads>
        /// <summary>
        /// Performs a logical comparison of this and the specified 
        /// <see cref="ReferenceTocLayoutType"/> parameter to determine whether 
        /// they are equal. 
        /// </summary>
        /// <param name="other">The <see cref="ReferenceTocLayoutType"/> to be compared.</param>
        /// <returns>
        /// This returns <see langword="true"/> if this and the specified 
        /// <see cref="ReferenceTocLayoutType"/> are equal; otherwise <see langword="false"/>.
        /// </returns>
        public bool Equals(ReferenceTocLayoutType other)
        {
            return (_typeValue == other._typeValue);
        }

        /// <summary>
        /// Compares the specified object parameter to the <see cref="ReferenceTocLayoutType.Value"/> 
        /// property of the <see cref="ReferenceTocLayoutType"/> object.
        /// </summary>
        /// <param name="obj">The object to be compared.</param>
        /// <returns>
        /// This returns <see langword="true"/> if object is an instance of 
        /// <see cref="ReferenceTocLayoutType"/> and the two are equal; otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is ReferenceTocLayoutType)
            {
                return this.Equals((ReferenceTocLayoutType)obj);
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

        #region IComparable<ReferenceTocLayoutType> Members

        /// <summary>
        /// Compares the current <see cref="ReferenceTocLayoutType"/> with the specified
        /// <see cref="ReferenceTocLayoutType"/> structure.
        /// </summary>
        /// <param name="other">
        /// The <see cref="ReferenceTocLayoutType"/> structure to compare with this structure.
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
        public int CompareTo(ReferenceTocLayoutType other)
        {
            return this._typeValue.CompareTo(other._typeValue);
        }

        #endregion
    }
}
