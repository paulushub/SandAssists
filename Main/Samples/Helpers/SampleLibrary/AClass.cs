using System;
using System.ComponentModel;

namespace ANamespace
{
    /// <summary>
    /// <isnew/> This is a sample class summary, AClass.
    /// </summary>
    /// <remarks>
    /// An implementation of the <see cref="BClass"/> class.
    /// </remarks>
    [Serializable]
    public class AClass : BClass, IAClass
    {
        /// <summary>
        /// A test class member.
        /// </summary>
        private string _aText;

        /// <summary>
        /// Another test class member.
        /// </summary>
        internal string _aName;

        /// <overloads>
        /// This is for all sample class constructors.
        /// </overloads>
        /// <summary>
        /// This is a sample class parameterless constructor.
        /// </summary>
        public AClass()
        {   
        }

        /// <summary>
        /// This is a sample class constructor.
        /// </summary>
        /// <param name="text">A text content.</param>
        /// <exception cref="ArgumentNullException">If the <paramref name="text"/> is <see langword="null"/>. </exception>
        public AClass(string text)
        {
            if (text == null)
            {
                throw new ArgumentNullException();
            }

            _aText = text;
        }

        /// <summary>
        /// Gets or sets the text contents, updated.
        /// </summary>
        /// <value>
        /// <inheritdoc/>
        /// <para>
        /// Note that, this is the contents from the <see cref="AClass"/>.
        /// </para>
        /// </value>
        [BrowsableAttribute(true)]
        [DefaultValueAttribute("Sample")]
        [CategoryAttribute("Testing")]
        sealed public override string Text
        {
            get
            {
                return _aText;
            }
            set
            {
                _aText = value;
            }
        }

        /// <summary>
        /// Gets or sets the text contents, updated.
        /// </summary>
        /// <value>
        /// <inheritdoc/>
        /// <para>
        /// Note that, this is the contents from the <see cref="AClass"/>.
        /// </para>
        /// </value>
        [BrowsableAttribute(true)]
        [DefaultValueAttribute("Sample")]
        [CategoryAttribute("Testing")]
        sealed protected override string Description
        {
            get
            {
                return _aText;
            }
            set
            {
                _aText = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of this object.
        /// </summary>
        /// <value>
        /// A string value for the name.
        /// </value>
        internal string Name
        {
            get
            {
                return _aName;
            }
            set
            {
                _aName = value;
            }
        }

        internal void UpdateName(string name)
        {
            _aName = name;
        }

        protected void ResetName()
        {
            _aName = String.Empty;
        }

        protected internal void ResetNameAll()
        {
            _aName = String.Empty;
        }

        private void SetNameNull()
        {
            _aName = null;
        }

        /// <overloads>
        /// Returns the text representation of this object.
        /// </overloads>
        /// <summary>
        /// Returns the text representation of this object, updated.
        /// </summary>
        /// <returns>
        /// <inheritdoc cref="Object.ToString"/>
        /// </returns>
        /// <remarks>
        /// This is just testing, AClass:ToString().
        /// </remarks>
        /// <example>
        /// <code>
        /// a = b + c
        /// </code>
        /// </example>
        public override string ToString()
        {
            return base.ToString();
        }

        /// <summary><isnew/> A AClass to-string.</summary>
        /// <param name="testing">A test parameter.</param>
        /// <inheritdoc cref="ToString()" select="returns|remarks|example"/>
        public string ToString(string testing)
        {
            return base.ToString() + testing;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string NewQuantity()
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public string NewQuantity(double value)
        {
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public string NewQuantity(string expression)
        {
            return null;
        }

        #region IDisposable Members

        sealed protected override void Dispose(bool disposing)
        {
        }

        #endregion
    }
}

/*
/// <summary>
/// Gets or sets the text contents, updated.
/// </summary>
/// <value>
/// <inheritdoc/>
/// <para>
/// Note that, this is the contents from the <see cref="AClass"/>.
/// </para>
/// </value>
*/