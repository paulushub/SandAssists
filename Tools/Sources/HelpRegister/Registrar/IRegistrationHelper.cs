using System;

namespace Sandcastle.HelpRegister
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRegistrationHelper : IRegistrationLogger
    {
        void CloseViewers();
        void DisplayLogo();
        void DisplayHelp();
    }
}
