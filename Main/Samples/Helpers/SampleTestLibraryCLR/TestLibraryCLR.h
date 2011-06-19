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

        /// <summary>
        ///
        /// </summary>
        virtual void TestMethod()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="testValue"></param>
        virtual void TestMethod(Object^ testValue)
        {
        }

        // visible inside assembly
    internal: //private public:

        // visible to derived types outside and all code inside assembly
    protected public:
    };
}
