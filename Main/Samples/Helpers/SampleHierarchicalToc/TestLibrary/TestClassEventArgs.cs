using System;

namespace Company.TestLibrary
{
    /// <summary>
    /// A test event argument.
    /// </summary>
    public sealed class TestClassEventArgs : EventArgs
    {
    }

    /// <summary>
    /// A test delegate.
    /// </summary>
    /// <param name="args">A delegate parameter.</param>
    public delegate void TestClassHandler(TestClassEventArgs args);
}
