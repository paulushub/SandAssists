#pragma once

using namespace System;
using namespace System::Collections::Generic;

namespace TestLibraryCLR 
{
    /// <summary>
    /// This is the summary for the <c>TestingGeneric</c> class.
    /// </summary>
    /// <typeparam name="T">Parameter of the delegate is a <c>T</c>.</typeparam>
    generic<typename T>
    public delegate void TestDelegate(T testValue);

    /// <summary>
    /// This is the summary for the <c>TestingGeneric</c> class.
    /// </summary>
    /// <typeparam name="T">The type for this class.</typeparam>
    generic<typename T>
    public ref class TestingGeneric
    {
    private:
        T _field;

    public:
        /// <summary>
        /// Initializes a new instance of the <see cref="TestingGeneric"/> class
        /// </summary>
        TestingGeneric(void);

        /// <summary>
        /// Summary for Keys property.
        /// </summary>
        /// <typeparam name="T">The type for this class.</typeparam>
        property ICollection<T>^ Keys 
        {
            ICollection<T>^ get ()
            {
                return nullptr;
            }
        }

        /// <summary>
        /// Summary for <c>Read</c> method.
        /// </summary>
        /// <param name="equable">Test generic input value.</param>
        void Read(IEquatable<String^>^ equable)
        {   
        }

        /// <summary>
        /// Summary for <c>ReadWith</c> method.
        /// </summary>
        /// <param name="equable">Test generic input value.</param>
        void ReadWith(TestDelegate<String^>^ equable)
        {   
        }

        /// <summary>
        /// Summary for <c>ReadBoth</c> method.
        /// </summary>
        /// <param name="equable1">Test generic input value.</param>
        /// <param name="equable2">Test generic input value.</param>
        void ReadBoth(IEquatable<String^>^ equable1, 
            TestDelegate<IList<String^>^>^ equable2)
        {   
        }

        /// <summary>
        /// Summary for test write method.
        /// </summary>
        /// <param name="input">Test generic input value.</param>
        void Write(List<int>^ input);   
    };
}
