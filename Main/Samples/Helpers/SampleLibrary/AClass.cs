using System;
using System.ComponentModel;

namespace ANamespace
{
    /// <summary>
    /// <isnew/> This is a sample class summary, AClass.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An implementation of the <see cref="BClass"/> class.
    /// </para>
    /// <para>
    /// Here we have decided to embed an image 
    /// <mediaLinkInline>
    /// <image href="2aca5da4-6f94-43a0-9817-5f413d16f803"/>
    /// </mediaLinkInline>
    /// to make the experience cool. Another way is to embed the image like
    /// this <mediaLinkInline href="2aca5da4-6f94-43a0-9817-5f413d16f805"/>.
    /// </para>
    /// <para>
    /// The following are the illustrations of the displayed media:
    /// </para>
    /// <para>Image with the default caption:</para>
    /// <mediaLink>
    ///    <caption>A caption of the image.</caption>
    ///    <image href="2aca5da4-6f94-43a0-9817-5f413d16f801"/>
    /// </mediaLink>
    /// <para>Image with a lead caption, will be displayed at the top.</para>
    /// <mediaLink>
    ///    <caption lead="Figure 1">A caption of the image.</caption>
    ///    <image href="2aca5da4-6f94-43a0-9817-5f413d16f801"/>
    /// </mediaLink>
    /// <para>
    /// Image with a lead caption placed at the bottom.
    /// </para>
    /// <mediaLink>
    ///    <caption placement="after" lead="Figure 1">A caption of the image.</caption>
    ///    <image href="2aca5da4-6f94-43a0-9817-5f413d16f801"/>
    /// </mediaLink>
    /// <para>With the image centered, and the caption at the bottom.</para>
    /// <mediaLink placement="center">
    ///    <caption location="bottom" lead="Figure 1">A caption of the image.</caption>
    ///    <image href="2aca5da4-6f94-43a0-9817-5f413d16f801"/>
    /// </mediaLink>
    /// <para>
    /// You can use the shorter syntax for an image without a caption.
    /// </para>
    /// <mediaLink placement="right" href="2aca5da4-6f94-43a0-9817-5f413d16f801"/>
    /// <para>
    /// Finally, we use image with image up.
    /// </para>
    /// <mediaLink placement="center" href="2aca5da4-6f94-43a0-9817-5f413d16f813"/>
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