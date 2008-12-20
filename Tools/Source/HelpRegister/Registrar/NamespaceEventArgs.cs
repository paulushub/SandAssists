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
    public sealed class NamespaceEventArgs : EventArgs
    {
        private readonly string _name;
        private readonly bool _register = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="register"></param>
        public NamespaceEventArgs(string name, bool register)
        {
            _name     = name;
            _register = register;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool Register
        {
            get
            {
                return _register;
            }
        }
    }
}
