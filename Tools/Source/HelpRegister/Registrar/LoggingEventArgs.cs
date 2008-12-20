//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
using System;

namespace Sandcastle.HelpRegister
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class LoggingEventArgs : EventArgs
    {
        private readonly string _message;

        public LoggingEventArgs(string message)
        {
            _message = message;
        }

        public string Message
        {
            get
            {
                return _message;
            }
        }
    }
}
