//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
using System;
using System.Resources;
using System.Reflection;

using Sandcastle.HelpRegister.Properties;

namespace Sandcastle.HelpRegister
{
	public static class ResourcesHelper
	{
        private static ResourceManager _resourceManager;

		public static string GetString(string keyName)
		{
            if (_resourceManager == null)
            {
                _resourceManager = Resources.ResourceManager;
            }

			return _resourceManager.GetString(keyName);
		}
	}
}
