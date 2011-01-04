using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    /// <summary>
    /// This is an <see langword="abstract"/> base class for most objects in 
    /// this build library. This is used as the base object to create components 
    /// object hierarchy.
    /// </summary>
    public abstract class BuildObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BuildObject"/> class.
        /// </summary>
        protected BuildObject()
        {   
        }
    }
}
