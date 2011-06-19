using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace TestLibrary
{
    /// <summary>
    /// For testing generic methods
    /// </summary>
    /// <typeparam name="T">A test type parameter.</typeparam>
    public class GenericMethods<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GenericMethods{T}"/> class.
        /// </summary>
        public GenericMethods()
        {   
        }

        /// <summary>
        /// Testing generic expression.
        /// </summary>
        /// <param name="test">
        /// A test parameter.
        /// </param>
        public void Method1(Expression<Func<T, bool>> test)
        {   
        }

        /// <summary>
        /// Text that is lost in Sandcastle.
        /// </summary>
        /// <param name="param">Text that is lost in Sandcastle</param>
        public void Test(Func<double> param)
        {     
        }

        /// <summary>
        /// Creates a <c>Member</c> using a property, field o non-void expression call.
        /// </summary>
        /// <param name="member">
        /// The member expression to use as a member.
        /// </param>
        /// <returns>A <c>Member</c> for the specified member expression.</returns>
        public static string MemberOf(Expression<Func<object>> member)
        {             
            return member.ToString();
        }

        /// <summary>
        /// A property that may be connected to a carrier object at runtime. 
        /// The property is either connected or disconnected. A disconnected 
        /// property is different than a connected property value of 
        /// <c>null</c>. All members are thread safe.
        /// </summary>
        /// <typeparam name="TValue">The property type.</typeparam>
        public interface IConnectibleProperty<TValue>
        {
            /// <summary>
            /// Gets the value of the property, if it is connected; otherwise, sets the value of the property and returns the new value.
            /// </summary>
            /// <param name="createCallback">
            /// The delegate invoked to create the value of the property, 
            /// if it is disconnected. May not be <c>null</c>. If there is 
            /// a multi-threaded race condition, each thread's delegate may 
            /// be invoked, but all values except one will be discarded.
            /// </param>
            /// <returns>The value of the property.</returns>
            TValue GetOrCreate(Func<TValue> createCallback);
        }
    }
}
