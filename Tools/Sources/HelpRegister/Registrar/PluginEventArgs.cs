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
	public sealed class PluginEventArgs : EventArgs
	{
		private readonly string _parent;
		private readonly string _child;
		private readonly bool _register = true;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="register"></param>
		public PluginEventArgs(string parent, string child, bool register)
		{
			_parent   = parent;
			_child    = child;
			_register = register;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string Parent
		{
			get 
            { 
                return _parent; 
            }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string Child
		{
			get 
            { 
                return _child; 
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
