#pragma once

#include "Base.h"

using namespace System;

namespace TestLibraryCLR 
{  
    /// <summary>
    /// Testing derived class.
    /// </summary>
    public ref class Derived : Base
    {
        /// <summary>
        /// Performs application-defined tasks associated with freeing, 
        /// releasing, or resetting unmanaged resources.
        /// </summary>
        ~Derived() // implements/overrides the IDisposable::Dispose method
        {
            // free managed and unmanaged resources
        }

        /// <summary>
        /// Allows an object to try to free resources and perform other 
        /// cleanup operations before it is reclaimed by garbage collection.
        /// </summary>
        !Derived() // implements/overrides the Object::Finalize method
        {
            // free unmanaged resources only
        }
    };
}
