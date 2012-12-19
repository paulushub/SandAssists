using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Company.TestLibrary
{
    /// <summary>
    /// A simple structure test.
    /// </summary>
    public struct SimpleStruct
    {
        /// <summary>
        /// Gets or sets the position, a field.
        /// </summary>
        /// <value>Value of the position.</value>
        public int Position;
        /// <summary>
        /// Gets or sets the value indicating whether it exists, a field.
        /// </summary>
        /// <value>true if it exists.</value>
        public bool Exists;
        /// <summary>
        /// Gets or sets the last value, a field.
        /// </summary>
        /// <value>Last value.</value>
        public double LastValue;
    }      

    /// <summary>
    /// This is a test interface.
    /// </summary>
    /// <remarks>
    /// This is a test link to conceptual topic: <see topic="2aca5da4-6f94-43a0-9817-5f413d16f101"/>.
    /// </remarks>
    /// <seealso topic="2aca5da4-6f94-43a0-9817-5f413d16f101">Another Quick Test</seealso>
    public interface IMyInterface
    {
        /// <summary>
        /// A sample test method.
        /// </summary>
        void MethodB();
    }

    /// <summary>
    /// A test class implementing the <see cref="IMyInterface"/>.
    /// </summary>
    public class A : IMyInterface
    {
        /// <summary>
        /// A sample test method.
        /// </summary>
        public void MethodB() { Console.WriteLine("A.MethodB()"); }
    }

    /// <summary>
    /// A test class implementing the <see cref="IMyInterface"/>.
    /// </summary>
    public class B : IMyInterface
    {
        /// <summary>
        /// A sample test method.
        /// </summary>
        public void MethodB() { Console.WriteLine("B.MethodB()"); }
        /// <summary>
        /// Another test method.
        /// </summary>
        /// <param name="i">A test parameter.</param>
        public void MethodA(int i) { Console.WriteLine("B.MethodA(int i)"); }
    }

    /// <summary>
    /// A test class implementing the <see cref="IMyInterface"/>.
    /// </summary>
    public class C : IMyInterface
    {
        /// <summary>
        /// A sample test method.
        /// </summary>
        public void MethodB() { Console.WriteLine("C.MethodB()"); }
        /// <summary>
        /// Another test method.
        /// </summary>
        /// <param name="obj">A test parameter.</param>
        public void MethodA(object obj) { Console.WriteLine("C.MethodA(object obj)"); }
    }

    /// <summary>
    /// Define extension methods for any type that implements IMyInterface.
    /// </summary>
    public static class ExtensionMethods
    {
        /// <summary>
        /// Testing extension method A.
        /// </summary>
        /// <param name="myInterface">The extension target.</param>
        /// <param name="i">Test parameter.</param>
        public static void MethodA(this IMyInterface myInterface, int i)
        {
            Console.WriteLine("Extension.MethodA(this IMyInterface myInterface, int i)");
        }

        /// <summary>
        /// Testing extension method A.
        /// </summary>
        /// <param name="myInterface">The extension target.</param>
        /// <param name="s">Test parameter.</param>
        public static void MethodA(this IMyInterface myInterface, string s)
        {
            Console.WriteLine("Extension.MethodA(this IMyInterface myInterface, string s)");
        }

        /// <summary>
        /// Testing the extension method B.
        /// </summary>
        /// <param name="myInterface">The extension target.</param>
        /// <remarks>
        /// This method is never called, because the three classes implement MethodB.
        /// </remarks>
        public static void MethodB(this IMyInterface myInterface)
        {
            Console.WriteLine("Extension.MethodB(this IMyInterface myInterface)");
        }

        /// <summary>
        /// Testing the extension method C.
        /// </summary>
        /// <param name="obj">The extension target.</param>
        public static void MethodC(this object obj)
        {
        }

        /// <summary>
        /// Testing the extension method D.
        /// </summary>
        /// <param name="obj">The extension target.</param>
        public static void MethodD(this ContentA obj)
        {
        }
    }
}
