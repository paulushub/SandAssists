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
    /// <description><see cref="BuildFrameworkType.Null"/></description>
    /// <description>-1</description>
    /// <description>Null</description>
    /// </item>
    /// <item>
    /// <description><see cref="BuildFrameworkType.None"/></description>
    /// <description>0</description>
    /// <description>None</description>
    /// </item>
    /// <item>
    /// <description><see cref="BuildFrameworkType.Framework10"/></description>
    /// <description>1</description>
    /// <description>A</description>
    /// </item>
    /// <item>
    /// <description><see cref="BuildFrameworkType.Framework11"/></description>
    /// <description>2</description>
    /// <description>B</description>
    /// </item>
    /// <item>
    /// <description><see cref="BuildFrameworkType.Framework20"/></description>
    /// <description>3</description>
    /// <description>C</description>
    /// </item>
    /// <item>
    /// <description><see cref="BuildFrameworkType.Framework30"/></description>
    /// <description>4</description>
    /// <description>D</description>
    /// </item>
    /// <item>
    /// <description><see cref="BuildFrameworkType.Custom"/></description>
    /// <description>10</description>
    /// <description>Custom</description>
    /// </item>
    /// </list>
    /// </remarks>
    [Serializable]
    public struct BuildFrameworkType : IEquatable<BuildFrameworkType>,
        IComparable<BuildFrameworkType>
    {
        #region Public Enumerations

        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>-1</c>.</para>
        /// </summary>
        public readonly static BuildFrameworkType Null = 
            new BuildFrameworkType(-1, "Unknown");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>0</c>.</para>
        /// </summary>
        public readonly static BuildFrameworkType None = 
            new BuildFrameworkType(0, "Unspecified");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>10</c>.</para>
        /// </summary>
        public readonly static BuildFrameworkType Framework10 =
            new BuildFrameworkType(10, "Microsoft .NET Framework 1.0");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>11</c>.</para>
        /// </summary>
        public readonly static BuildFrameworkType Framework11 =
            new BuildFrameworkType(11, "Microsoft .NET Framework 1.1");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>20</c>.</para>
        /// </summary>
        public readonly static BuildFrameworkType Framework20 =
            new BuildFrameworkType(20, "Microsoft .NET Framework 2.0");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>30</c>.</para>
        /// </summary>
        public readonly static BuildFrameworkType Framework30 =
            new BuildFrameworkType(30, "Microsoft .NET Framework 3.0");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>35</c>.</para>
        /// </summary>
        public readonly static BuildFrameworkType Framework35 =
            new BuildFrameworkType(35, "Microsoft .NET Framework 3.5");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>40</c>.</para>
        /// </summary>
        public readonly static BuildFrameworkType Framework40 =
            new BuildFrameworkType(40, "Microsoft .NET Framework 4.0");

        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>110</c>.</para>
        /// </summary>
        public readonly static BuildFrameworkType Silverlight10 =
            new BuildFrameworkType(110, "Microsoft Silverlight 1.0");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>120</c>.</para>
        /// </summary>
        public readonly static BuildFrameworkType Silverlight20 =
            new BuildFrameworkType(120, "Microsoft Silverlight 2.0");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>130</c>.</para>
        /// </summary>
        public readonly static BuildFrameworkType Silverlight30 =
            new BuildFrameworkType(130, "Microsoft Silverlight 3.0");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>140</c>.</para>
        /// </summary>
        public readonly static BuildFrameworkType Silverlight40 =
            new BuildFrameworkType(140, "Microsoft Silverlight 4.0");


        #endregion

        #region Private Fields

        /// <summary>
        /// The enumeration value of this structure.
        /// </summary>
        private int _typeValue;
        /// <summary>
        /// 
        /// </summary>
        private string _typeLabel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildFrameworkType"/> structure
        /// with the specified integer value.
        /// </summary>
        /// <param name="value">
        /// The numerical value of this enumeration structure.
        /// </param>
        public BuildFrameworkType(int value, string label)
        {
            _typeValue = value;
            if (label == null)
            {
                _typeLabel = String.Empty;
            }
            else
            {
                _typeLabel = label.Trim();
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value indicating whether the <see cref="BuildFrameworkType.Value"/> 
        /// is <see langword="null"/>.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if <see cref="BuildFrameworkType.Value"/> 
        /// is <see langword="null"/>; otherwise, <see langword="false"/>.
        /// </value>
        public bool IsNull
        {
            get
            {
                return (_typeValue < 0);
            }
        }

        public bool IsSilverlight
        {
            get
            {
                // includes the version 5...
                return (_typeValue >= 110 && _typeValue <= 150);  
            }
        }

        /// <summary>
        /// Gets the value of this <see cref="BuildFrameworkType"/> structure.
        /// </summary>
        /// <value>
        /// An integer representing the value of this <see cref="BuildFrameworkType"/> structure. 
        /// </value>
        public int Value
        {
            get
            {
                return _typeValue;
            }
        }

        public string Label
        {
            get
            {
                return _typeLabel;
            }
        }

        #endregion

        #region Public Operators

        /// <summary>
        /// Converts the specified <see cref="BuildFrameworkType"/> structure to an 
        /// integer, which is its value.
        /// </summary>
        /// <param name="type">A <see cref="BuildFrameworkType"/> to be converted.</param>
        /// <returns>The integer value of the specified <see cref="BuildFrameworkType"/>.</returns>
        public static explicit operator int(BuildFrameworkType type)
        {
            return type._typeValue;
        }

        /// <summary>
        /// Converts the specified integer to the equivalent 
        /// <see cref="BuildFrameworkType"/> structure.
        /// </summary>
        /// <param name="value">An integer to be converted.</param>
        /// <returns>
        /// The <see cref="BuildFrameworkType"/> whose value is specified in the integer
        /// parameter.
        /// </returns>
        public static explicit operator BuildFrameworkType(int value)
        {
            if (value < 0)
            {
                return BuildFrameworkType.Null;
            }

            switch (value)
            {
                case 0:
                    return BuildFrameworkType.None;
                case 10:
                    return BuildFrameworkType.Framework10;
                case 11:
                    return BuildFrameworkType.Framework11;
                case 20:
                    return BuildFrameworkType.Framework20;
                case 30:
                    return BuildFrameworkType.Framework30;
                case 35:
                    return BuildFrameworkType.Framework35;
                case 40:
                    return BuildFrameworkType.Framework40;

                case 110:
                    return BuildFrameworkType.Silverlight10;
                case 120:
                    return BuildFrameworkType.Silverlight20;
                case 130:
                    return BuildFrameworkType.Silverlight30;
                case 140:
                    return BuildFrameworkType.Silverlight40;
            }

            return BuildFrameworkType.Null;
        }

        /// <summary>
        /// Performs a logical comparison of the two <see cref="BuildFrameworkType"/> 
        /// parameters to determine whether they are equal.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildFrameworkType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildFrameworkType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameters,
        /// <paramref name="type1"/> and <paramref name="type2"/> represent the 
        /// same value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(BuildFrameworkType type1, BuildFrameworkType type2)
        {
            return type1._typeValue == type2._typeValue;
        }

        /// <summary>
        /// Performs a logical comparison of the two <see cref="BuildFrameworkType"/> 
        /// parameters to determine whether they are not equal. 
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildFrameworkType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildFrameworkType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameters,
        /// <paramref name="type1"/> and <paramref name="type2"/> do not represent 
        /// the same value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(BuildFrameworkType type1, BuildFrameworkType type2)
        {
            return type1._typeValue != type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="BuildFrameworkType"/> parameters to determine 
        /// whether the first is greater than the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildFrameworkType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildFrameworkType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is greater 
        /// than the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >(BuildFrameworkType type1, BuildFrameworkType type2)
        {
            return type1._typeValue > type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="BuildFrameworkType"/> parameters to determine 
        /// whether the first is greater than or equal to the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildFrameworkType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildFrameworkType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is greater 
        /// than or equal to the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >=(BuildFrameworkType type1, BuildFrameworkType type2)
        {
            return type1._typeValue >= type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="BuildFrameworkType"/> parameters to determine 
        /// whether the first is less than the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildFrameworkType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildFrameworkType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is less 
        /// than the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <(BuildFrameworkType type1, BuildFrameworkType type2)
        {
            return type1._typeValue < type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="BuildFrameworkType"/> parameters to determine 
        /// whether the first is less than or equal to the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildFrameworkType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildFrameworkType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is less 
        /// than or equal to the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <=(BuildFrameworkType type1, BuildFrameworkType type2)
        {
            return type1._typeValue <= type2._typeValue;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts the string representation of the enumeration to 
        /// its <see cref="BuildFrameworkType"/> equivalent.
        /// </summary>
        /// <param name="text">A string containing a enumeration to convert.</param>
        /// <returns>
        /// <para>
        /// A <see cref="BuildFrameworkType"/> equivalent to the enumeration contained 
        /// in the specified parameter, <paramref name="text"/>.
        /// </para>
        /// <para>
        /// This will return <see cref="BuildFrameworkType.Null"/> if the string is empty
        /// or <see langword="null"/>.
        /// </para>
        /// </returns>
        /// <exception cref="FormatException">
        /// If the parameter, <paramref name="text"/>, does not contain a valid 
        /// string representation of a <see cref="BuildFrameworkType"/> structure.
        /// </exception>
        public static BuildFrameworkType Parse(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return BuildFrameworkType.Null;
            }

            BuildFrameworkType frameworkType = BuildFrameworkType.Null;
            if (TryParse(text, out frameworkType))
            {
                return frameworkType;
            }

            throw new FormatException(
                "The specified parameter, text, is not a valid representation of this structure.");
        }

        /// <summary>
        /// Converts the specified string representation of an enumeration 
        /// to its <see cref="BuildFrameworkType"/> equivalent and returns a value 
        /// that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="text">A string containing a enumeration to convert.</param>
        /// <param name="value">
        /// When this method returns, contains the <see cref="BuildFrameworkType"/> value 
        /// equivalent to the enumeration contained in <paramref name="text"/>, 
        /// if the conversion succeeded, or <see cref="BuildFrameworkType.Null"/> 
        /// if the conversion failed. 
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameter, 
        /// <paramref name="text"/>, is converted successfully; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public static bool TryParse(string text, out BuildFrameworkType value)
        {
            value = BuildFrameworkType.Null;

            if (String.IsNullOrEmpty(text))
            {
                return true;
            }

            switch (text.ToLower())
            {
                case "(null)":
                    value = BuildFrameworkType.Null;
                    return true;
                case "none":
                    value = BuildFrameworkType.None;
                    return true;
                case "framework10":
                    value = BuildFrameworkType.Framework10;
                    return true;
                case "framework11":
                    value = BuildFrameworkType.Framework11;
                    return true;
                case "framework20":
                    value = BuildFrameworkType.Framework20;
                    return true;
                case "framework30":
                    value = BuildFrameworkType.Framework30;
                    return true;
                case "framework35":
                    value = BuildFrameworkType.Framework35;
                    return true;
                case "framework40":
                    value = BuildFrameworkType.Framework40;
                    return true;

                case "silverlight10":
                    value = BuildFrameworkType.Silverlight10;
                    return true;
                case "silverlight20":
                    value = BuildFrameworkType.Silverlight20;
                    return true;
                case "silverlight30":
                    value = BuildFrameworkType.Silverlight30;
                    return true;
                case "silverlight40":
                    value = BuildFrameworkType.Silverlight40;
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Converts this <see cref="BuildFrameworkType"/> structure to its string
        /// representation, which is the enumeration name of this structure.
        /// </summary>
        /// <returns>
        /// <para>
        /// A string object equal to the value of this <see cref="BuildFrameworkType"/>.
        /// </para>
        /// <para>
        /// The <see cref="BuildFrameworkType.Null"/> instance will return <c>(Null)</c>.
        /// </para>
        /// <para>
        /// The <see cref="BuildFrameworkType.Custom"/> instances will return a string
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
                case 10:
                    return "Framework10";
                case 11:
                    return "Framework11";
                case 20:
                    return "Framework20";
                case 30:
                    return "Framework30";
                case 35:
                    return "Framework35";
                case 40:
                    return "Framework40";
                case 110:
                    return "Silverlight10";
                case 120:
                    return "Silverlight20";
                case 130:
                    return "Silverlight30";
                case 140:
                    return "Silverlight40";
            }

            return base.ToString();
        }

        #endregion

        #region IEquatable<BuildFrameworkType> Members

        /// <overloads>
        /// Performs a logical comparison of two structures to determine whether 
        /// they are equal.
        /// </overloads>
        /// <summary>
        /// Performs a logical comparison of this and the specified 
        /// <see cref="BuildFrameworkType"/> parameter to determine whether 
        /// they are equal. 
        /// </summary>
        /// <param name="other">The <see cref="BuildFrameworkType"/> to be compared.</param>
        /// <returns>
        /// This returns <see langword="true"/> if this and the specified 
        /// <see cref="BuildFrameworkType"/> are equal; otherwise <see langword="false"/>.
        /// </returns>
        public bool Equals(BuildFrameworkType other)
        {
            return (_typeValue == other._typeValue);
        }

        /// <summary>
        /// Compares the specified object parameter to the <see cref="BuildFrameworkType.Value"/> 
        /// property of the <see cref="BuildFrameworkType"/> object.
        /// </summary>
        /// <param name="obj">The object to be compared.</param>
        /// <returns>
        /// This returns <see langword="true"/> if object is an instance of 
        /// <see cref="BuildFrameworkType"/> and the two are equal; otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is BuildFrameworkType)
            {
                return this.Equals((BuildFrameworkType)obj);
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

        #region IComparable<BuildFrameworkType> Members

        /// <summary>
        /// Compares the current <see cref="BuildFrameworkType"/> with the specified
        /// <see cref="BuildFrameworkType"/> structure.
        /// </summary>
        /// <param name="other">
        /// The <see cref="BuildFrameworkType"/> structure to compare with this structure.
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
        public int CompareTo(BuildFrameworkType other)
        {
            return this._typeValue.CompareTo(other._typeValue);
        }

        #endregion
    }   
}
