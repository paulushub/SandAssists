using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Company.TestLibrary
{
    /// <summary>
    /// This is a test interface.
    /// </summary>
    /// <remarks>
    /// This is a test link to conceptual topic: <see topic="2aca5da4-6f94-43a0-9817-5f413d16f101"/>.
    /// </remarks>
    /// <seealso topic="2aca5da4-6f94-43a0-9817-5f413d16f101">Another Quick Test</seealso>
    public interface IMyInterface
    {
        void MethodB();
    }

    public class A : IMyInterface
    {
        public void MethodB() { Console.WriteLine("A.MethodB()"); }
    }

    public class B : IMyInterface
    {
        public void MethodB() { Console.WriteLine("B.MethodB()"); }
        public void MethodA(int i) { Console.WriteLine("B.MethodA(int i)"); }
    }

    public class C : IMyInterface
    {
        public void MethodB() { Console.WriteLine("C.MethodB()"); }
        public void MethodA(object obj) { Console.WriteLine("C.MethodA(object obj)"); }
    }

    // Define extension methods for any type that implements IMyInterface.
    public static class ExtensionMethods
    {
        public static void MethodA(this IMyInterface myInterface, int i)
        {
            Console.WriteLine("Extension.MethodA(this IMyInterface myInterface, int i)");
        }

        public static void MethodA(this IMyInterface myInterface, string s)
        {
            Console.WriteLine("Extension.MethodA(this IMyInterface myInterface, string s)");
        }

        // This method is never called, because the three classes implement MethodB.
        public static void MethodB(this IMyInterface myInterface)
        {
            Console.WriteLine("Extension.MethodB(this IMyInterface myInterface)");
        }
    }
}
