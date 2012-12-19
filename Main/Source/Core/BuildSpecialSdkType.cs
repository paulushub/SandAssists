using System;

namespace Sandcastle
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// </remarks>
    [Serializable]
    public struct BuildSpecialSdkType : IEquatable<BuildSpecialSdkType>,
        IComparable<BuildSpecialSdkType>
    {
        #region Public Enumerations

        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>-1</c>.</para>
        /// </summary>
        public readonly static BuildSpecialSdkType Null =
            new BuildSpecialSdkType(-1, "Unknown");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>0</c>.</para>
        /// </summary>
        public readonly static BuildSpecialSdkType None =
            new BuildSpecialSdkType(0, "Unspecified");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>1</c>.</para>
        /// </summary>
        public readonly static BuildSpecialSdkType Blend01 = 
            new BuildSpecialSdkType(1, "Microsoft Expression Blend 1.0 SDK - Not Released");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>2</c>.</para>
        /// </summary>
        public readonly static BuildSpecialSdkType Blend02 =
            new BuildSpecialSdkType(2, "Microsoft Expression Blend 2.0 SDK - Not Released");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>3</c>.</para>
        /// </summary>
        public readonly static BuildSpecialSdkType Blend03 =
            new BuildSpecialSdkType(3, "Microsoft Expression Blend 3.0 SDK");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>4</c>.</para>
        /// </summary>
        public readonly static BuildSpecialSdkType Blend04 =
            new BuildSpecialSdkType(4, "Microsoft Expression Blend 4.0 SDK");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>5</c>.</para>
        /// </summary>
        public readonly static BuildSpecialSdkType Blend05 =
            new BuildSpecialSdkType(5, "Microsoft Expression Blend 5.0 SDK");

        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>10</c>.</para>
        /// </summary>
        public readonly static BuildSpecialSdkType WebMvc01 =
            new BuildSpecialSdkType(10, "ASP.NET MVC 1.0");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>20</c>.</para>
        /// </summary>
        public readonly static BuildSpecialSdkType WebMvc02 =
            new BuildSpecialSdkType(20, "ASP.NET MVC 2.0");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>30</c>.</para>
        /// </summary>
        public readonly static BuildSpecialSdkType WebMvc03 =
            new BuildSpecialSdkType(30, "ASP.NET MVC 3.0");
        /// <summary>
        /// <para></para>
        /// <para>The enumeration value is <c>40</c>.</para>
        /// </summary>
        public readonly static BuildSpecialSdkType WebMvc04 =
            new BuildSpecialSdkType(40, "ASP.NET MVC 4.0");

        #endregion

        #region Private Fields

        /// <summary>
        /// The enumeration value of this structure.
        /// </summary>
        private int _typeValue;
        /// <summary>
        /// A descriptive text for the enumeration.
        /// </summary>
        private string _typeLabel;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildSpecialSdkType"/> structure
        /// with the specified integer value.
        /// </summary>
        /// <param name="value">
        /// The numerical value of this enumeration structure.
        /// </param>
        /// <param name="label">
        /// A description of this enumeration structure.
        /// </param>
        public BuildSpecialSdkType(int value, string label)
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

        public BuildSpecialSdkKind Kind
        {
            get
            {
                if (_typeValue >= 1 && _typeValue <= 5)
                {
                    return BuildSpecialSdkKind.Blend;
                }
                if (_typeValue >= 10 && _typeValue <= 40)
                {
                    return BuildSpecialSdkKind.WebMvc;
                }

                return BuildSpecialSdkKind.None;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="BuildSpecialSdkType.Value"/> 
        /// is <see langword="null"/>.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if <see cref="BuildSpecialSdkType.Value"/> 
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
        /// Gets the value of this <see cref="BuildSpecialSdkType"/> structure.
        /// </summary>
        /// <value>
        /// An integer representing the value of this <see cref="BuildSpecialSdkType"/> structure. 
        /// </value>
        public int Value
        {
            get
            {
                return _typeValue;
            }
        }

        /// <summary>
        /// Gets the descriptive text of this <see cref="BuildFrameworkType"/>
        /// structure.
        /// </summary>
        /// <value>
        /// A string representing a description of this structure.
        /// </value>
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
        /// Converts the specified <see cref="BuildSpecialSdkType"/> structure to an 
        /// integer, which is its value.
        /// </summary>
        /// <param name="type">A <see cref="BuildSpecialSdkType"/> to be converted.</param>
        /// <returns>The integer value of the specified <see cref="BuildSpecialSdkType"/>.</returns>
        public static explicit operator int(BuildSpecialSdkType type)
        {
            return type._typeValue;
        }

        /// <summary>
        /// Converts the specified integer to the equivalent 
        /// <see cref="BuildSpecialSdkType"/> structure.
        /// </summary>
        /// <param name="value">An integer to be converted.</param>
        /// <returns>
        /// The <see cref="BuildSpecialSdkType"/> whose value is specified in the integer
        /// parameter.
        /// </returns>
        public static explicit operator BuildSpecialSdkType(int value)
        {
            if (value < 0)
            {
                return BuildSpecialSdkType.Null;
            }

            switch (value)
            {
                case 0:
                    return BuildSpecialSdkType.None;

                case 1:
                    return BuildSpecialSdkType.Blend01;
                case 2:
                    return BuildSpecialSdkType.Blend02;
                case 3:
                    return BuildSpecialSdkType.Blend03;
                case 4:
                    return BuildSpecialSdkType.Blend04;
                case 5:
                    return BuildSpecialSdkType.Blend05;

                case 10:
                    return BuildSpecialSdkType.WebMvc01;
                case 20:
                    return BuildSpecialSdkType.WebMvc02;
                case 30:
                    return BuildSpecialSdkType.WebMvc03;
                case 40:
                    return BuildSpecialSdkType.WebMvc04;
            }

            return BuildSpecialSdkType.Null;
        }

        /// <summary>
        /// Performs a logical comparison of the two <see cref="BuildSpecialSdkType"/> 
        /// parameters to determine whether they are equal.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildSpecialSdkType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildSpecialSdkType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameters,
        /// <paramref name="type1"/> and <paramref name="type2"/> represent the 
        /// same value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator ==(BuildSpecialSdkType type1, BuildSpecialSdkType type2)
        {
            return type1._typeValue == type2._typeValue;
        }

        /// <summary>
        /// Performs a logical comparison of the two <see cref="BuildSpecialSdkType"/> 
        /// parameters to determine whether they are not equal. 
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildSpecialSdkType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildSpecialSdkType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameters,
        /// <paramref name="type1"/> and <paramref name="type2"/> do not represent 
        /// the same value; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator !=(BuildSpecialSdkType type1, BuildSpecialSdkType type2)
        {
            return type1._typeValue != type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="BuildSpecialSdkType"/> parameters to determine 
        /// whether the first is greater than the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildSpecialSdkType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildSpecialSdkType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is greater 
        /// than the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >(BuildSpecialSdkType type1, BuildSpecialSdkType type2)
        {
            return type1._typeValue > type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="BuildSpecialSdkType"/> parameters to determine 
        /// whether the first is greater than or equal to the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildSpecialSdkType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildSpecialSdkType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is greater 
        /// than or equal to the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator >=(BuildSpecialSdkType type1, BuildSpecialSdkType type2)
        {
            return type1._typeValue >= type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="BuildSpecialSdkType"/> parameters to determine 
        /// whether the first is less than the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildSpecialSdkType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildSpecialSdkType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is less 
        /// than the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <(BuildSpecialSdkType type1, BuildSpecialSdkType type2)
        {
            return type1._typeValue < type2._typeValue;
        }

        /// <summary>
        /// Compares the two <see cref="BuildSpecialSdkType"/> parameters to determine 
        /// whether the first is less than or equal to the second.
        /// </summary>
        /// <param name="type1">An instance of <see cref="BuildSpecialSdkType"/>.</param>
        /// <param name="type2">An instance of <see cref="BuildSpecialSdkType"/>.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the first instance is less 
        /// than or equal to the second instance; otherwise, <see langword="false"/>.
        /// </returns>
        public static bool operator <=(BuildSpecialSdkType type1, BuildSpecialSdkType type2)
        {
            return type1._typeValue <= type2._typeValue;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Converts the string representation of the enumeration to 
        /// its <see cref="BuildSpecialSdkType"/> equivalent.
        /// </summary>
        /// <param name="text">A string containing a enumeration to convert.</param>
        /// <returns>
        /// <para>
        /// A <see cref="BuildSpecialSdkType"/> equivalent to the enumeration contained 
        /// in the specified parameter, <paramref name="text"/>.
        /// </para>
        /// <para>
        /// This will return <see cref="BuildSpecialSdkType.Null"/> if the string is empty
        /// or <see langword="null"/>.
        /// </para>
        /// </returns>
        /// <exception cref="FormatException">
        /// If the parameter, <paramref name="text"/>, does not contain a valid 
        /// string representation of a <see cref="BuildSpecialSdkType"/> structure.
        /// </exception>
        public static BuildSpecialSdkType Parse(string text)
        {
            if (String.IsNullOrEmpty(text))
            {
                return BuildSpecialSdkType.Null;
            }

            switch (text.ToLower())
            {
                case "(null)":
                    return BuildSpecialSdkType.Null;
                case "none":
                    return BuildSpecialSdkType.None;

                case "blend01":
                    return BuildSpecialSdkType.Blend01;
                case "blend02":
                    return BuildSpecialSdkType.Blend02;
                case "blend03":
                    return BuildSpecialSdkType.Blend03;
                case "blend04":
                    return BuildSpecialSdkType.Blend04;
                case "blend05":
                    return BuildSpecialSdkType.Blend05;

                case "webmvc01":
                    return BuildSpecialSdkType.WebMvc01;
                case "webmvc02":
                    return BuildSpecialSdkType.WebMvc02;
                case "webmvc03":
                    return BuildSpecialSdkType.WebMvc03;
                case "webmvc04":
                    return BuildSpecialSdkType.WebMvc04;
            }

            throw new FormatException(
                "The specified parameter, text, is not a valid representation of this structure.");
        }

        /// <summary>
        /// Converts the specified string representation of an enumeration 
        /// to its <see cref="BuildSpecialSdkType"/> equivalent and returns a value 
        /// that indicates whether the conversion succeeded.
        /// </summary>
        /// <param name="text">A string containing a enumeration to convert.</param>
        /// <param name="value">
        /// When this method returns, contains the <see cref="BuildSpecialSdkType"/> value 
        /// equivalent to the enumeration contained in <paramref name="text"/>, 
        /// if the conversion succeeded, or <see cref="BuildSpecialSdkType.Null"/> 
        /// if the conversion failed. 
        /// </param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified parameter, 
        /// <paramref name="text"/>, is converted successfully; otherwise, 
        /// <see langword="false"/>.
        /// </returns>
        public static bool TryParse(string text, out BuildSpecialSdkType value)
        {
            value = BuildSpecialSdkType.Null;

            if (String.IsNullOrEmpty(text))
            {
                return true;
            }

            switch (text.ToLower())
            {
                case "(null)":
                    value = BuildSpecialSdkType.Null;
                    return true;
                case "none":
                    value = BuildSpecialSdkType.None;
                    return true;

                case "blend01":
                    value = BuildSpecialSdkType.Blend01;
                    return true;
                case "blend02":
                    value = BuildSpecialSdkType.Blend02;
                    return true;
                case "blend03":
                    value = BuildSpecialSdkType.Blend03;
                    return true;
                case "blend04":
                    value = BuildSpecialSdkType.Blend04;
                    return true;
                case "blend05":
                    value = BuildSpecialSdkType.Blend05;
                    return true;

                case "webmvc01":
                    value = BuildSpecialSdkType.WebMvc01;
                    return true;
                case "webmvc02":
                    value = BuildSpecialSdkType.WebMvc02;
                    return true;
                case "webmvc03":
                    value = BuildSpecialSdkType.WebMvc03;
                    return true;
                case "webmvc04":
                    value = BuildSpecialSdkType.WebMvc04;
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Converts this <see cref="BuildSpecialSdkType"/> structure to its string
        /// representation, which is the enumeration name of this structure.
        /// </summary>
        /// <returns>
        /// <para>
        /// A string object equal to the value of this <see cref="BuildSpecialSdkType"/>.
        /// </para>
        /// <para>
        /// The <see cref="BuildSpecialSdkType.Null"/> instance will return <c>(Null)</c>.
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
                    return "Blend01";
                case 2:
                    return "Blend02";
                case 3:
                    return "Blend03";
                case 4:
                    return "Blend04";
                case 5:
                    return "Blend05";

                case 10:
                    return "WebMvc01";
                case 20:
                    return "WebMvc02";
                case 30:
                    return "WebMvc03";
                case 40:
                    return "WebMvc04";
            }

            return base.ToString();
        }

        #endregion

        #region IEquatable<BuildSpecialSdkType> Members

        /// <overloads>
        /// Performs a logical comparison of two structures to determine whether 
        /// they are equal.
        /// </overloads>
        /// <summary>
        /// Performs a logical comparison of this and the specified 
        /// <see cref="BuildSpecialSdkType"/> parameter to determine whether 
        /// they are equal. 
        /// </summary>
        /// <param name="other">The <see cref="BuildSpecialSdkType"/> to be compared.</param>
        /// <returns>
        /// This returns <see langword="true"/> if this and the specified 
        /// <see cref="BuildSpecialSdkType"/> are equal; otherwise <see langword="false"/>.
        /// </returns>
        public bool Equals(BuildSpecialSdkType other)
        {
            return (_typeValue == other._typeValue);
        }

        /// <summary>
        /// Compares the specified object parameter to the <see cref="BuildSpecialSdkType.Value"/> 
        /// property of the <see cref="BuildSpecialSdkType"/> object.
        /// </summary>
        /// <param name="obj">The object to be compared.</param>
        /// <returns>
        /// This returns <see langword="true"/> if object is an instance of 
        /// <see cref="BuildSpecialSdkType"/> and the two are equal; otherwise <see langword="false"/>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is BuildSpecialSdkType)
            {
                return this.Equals((BuildSpecialSdkType)obj);
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

        #region IComparable<BuildSpecialSdkType> Members

        /// <summary>
        /// Compares the current <see cref="BuildSpecialSdkType"/> with the specified
        /// <see cref="BuildSpecialSdkType"/> structure.
        /// </summary>
        /// <param name="other">
        /// The <see cref="BuildSpecialSdkType"/> structure to compare with this structure.
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
        public int CompareTo(BuildSpecialSdkType other)
        {
            return this._typeValue.CompareTo(other._typeValue);
        }

        #endregion
    }
}
