using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    [Serializable]
    public abstract class BuildConfiguration : BuildObject<BuildConfiguration>
    {
        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="BuildConfiguration"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildConfiguration"/> class
        /// to the default properties or values.
        /// </summary>
        protected BuildConfiguration()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildConfiguration"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="BuildConfiguration"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="BuildConfiguration"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected BuildConfiguration(BuildConfiguration source)
            : base(source)
        {                
        }

        #endregion

        #region Public Properties

        public abstract BuildConfigurationType ConfigurationType
        {
            get;
        }

        public abstract BuildConfigurationCoverage Coverage
        {
            get;
        }

        public abstract string ComponentName
        {
            get;
        }

        public abstract string ComponentPath
        {
            get;
        }

        public abstract string ComponentPathKey
        {
            get;
        }

        #endregion

        #region Public Methods

        public abstract void Configure(BuildSystem system, 
            BuildContext contex, BuildGroup group);

        #endregion
    }
}
