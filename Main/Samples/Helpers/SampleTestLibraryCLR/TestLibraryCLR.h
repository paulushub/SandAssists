// TestLibraryCLR.h

#pragma once

using namespace System;

namespace TestLibraryCLR 
{      
    /// <summary>
    /// This is a summary for <c>PointBase</c>.
    /// </summary>    
    /// <seealso cref="O:TestLibraryCLR.PointBase.TestMethod"/>
	public ref class PointBase
	{
    private:
        String^ _value;

		// TODO: Add your methods for this class here.

        // visible inside and outside assembly
    public: 

        /// <summary>
        /// Gets and sets the name.
        /// </summary>
        /// <value>
        /// A string containing the name.
        /// </value>
        property String^ Name 
        {
            String^ get()
            {
                return _value;
            }
            void set(String^ value)
            {
                _value = value;
            }
        }

        /// <overloads>
        /// Overloads summary of the <c>TestMethod</c>
        /// </overloads>
        /// <summary>
        /// Summary of the <c>TestMethod</c> without parameter.
        /// </summary>
        virtual void TestMethod()
        {
        }

        /// <summary>
        /// Summary of the <c>TestMethod</c> with parameter.
        /// </summary>
        /// <param name="testValue">The test value.</param>
        virtual void TestMethod(Object^ testValue)
        {
        }

        // visible inside assembly
    internal: //private public:

        // visible to derived types outside and all code inside assembly
    protected public:
    };
}
