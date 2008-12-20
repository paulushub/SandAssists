using System;
using System.Runtime.InteropServices;

namespace Sandcastle.HelpRegister
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRegistrationLogger
    {
        void Write(string text);
        void WriteLine();
        void WriteLine(string text);
        void WriteLine(Exception exception);
        void WriteLine(COMException exception);
    }
}
