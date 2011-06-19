using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Company.TestLibrary
{
    public sealed class TestClassEventArgs : EventArgs
    {
    }

    public delegate void TestClassHandler(TestClassEventArgs args);

}
