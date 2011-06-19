using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceXPathConfiguration : ReferenceConfiguration
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this configuration.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this configuration.
        /// </value>
        public const string ConfigurationName =
            "Sandcastle.References.ReferenceXPathConfiguration";

        #endregion

        #region Private Fields

        private List<ReferenceXPathItem> _xpathExpressions;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceXPathConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceXPathConfiguration"/> class
        /// to the default values.
        /// </summary>
        public ReferenceXPathConfiguration()
            : this(ConfigurationName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceXPathConfiguration"/> class
        /// with the specified options or category name.
        /// </summary>
        /// <param name="optionsName">
        /// A <see cref="System.String"/> specifying the name of this category of options.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="optionsName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="optionsName"/> is empty.
        /// </exception>
        private ReferenceXPathConfiguration(string optionsName)
            : base(optionsName)
        {
            _xpathExpressions = new List<ReferenceXPathItem>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceXPathConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceXPathConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceXPathConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceXPathConfiguration(ReferenceXPathConfiguration source)
            : base(source)
        {
            _xpathExpressions = source._xpathExpressions;
        }

        #endregion

        #region Public Properties

        public IList<ReferenceXPathItem> Expressions
        {
            get
            {
                return _xpathExpressions;
            }
        }

        /// <inheritdoc/>
        public override string Category
        {
            get
            {
                return "ReferenceVisitor";
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the visitor implementation for this configuration.
        /// </summary>
        /// <returns>
        /// A instance of the reference visitor, <see cref="ReferenceVisitor"/>,
        /// which is used to process this configuration settings during build.
        /// </returns>
        public override ReferenceVisitor CreateVisitor()
        {
            return new ReferenceXPathVisitor(this);
        }

        #endregion

        #region ICloneable Members

        /// <summary>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new build object that is a deep copy of this instance.
        /// </returns>
        public override BuildConfiguration Clone()
        {
            ReferenceXPathConfiguration options = new ReferenceXPathConfiguration(this);
            if (_xpathExpressions != null)
            {
                options._xpathExpressions = new List<ReferenceXPathItem>();
                if (_xpathExpressions.Count != 0)
                {
                    for (int i = 0; i < _xpathExpressions.Count; i++)
                    {
                        options._xpathExpressions.Add(_xpathExpressions[i].Clone());
                    }
                }
            }

            return options;
        }

        #endregion
    }
}
