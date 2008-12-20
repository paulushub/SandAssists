//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using MSHelpServices;

namespace Sandcastle.HelpRegister
{
	public static class PluginSearch
	{
        [DllImport("shlwapi.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool PathMatchSpec(string pwszFile, string pwszSpec);

		public static bool PluginDoesExist(string namespaceName, string pluginName)
		{
			if (String.IsNullOrEmpty(namespaceName) || String.IsNullOrEmpty(pluginName))
			{
				return false;
			}

			try
			{
				HxRegistryWalkerClass registryWalker = new HxRegistryWalkerClass();
				IHxRegNamespaceList help2Namespaces  = 
                    registryWalker.get_RegisteredNamespaceList("");

				foreach (IHxRegNamespace currentNamespace in help2Namespaces)
				{
					if (String.Equals(currentNamespace.Name, namespaceName))
					{
						IHxRegPlugInList p =
							(IHxRegPlugInList)currentNamespace.GetProperty(
                            HxRegNamespacePropId.HxRegNamespacePlugInList);
						foreach (IHxRegPlugIn plugin in p)
						{
							string currentName = (string)plugin.GetProperty(
                                HxRegPlugInPropId.HxRegPlugInName);
							if (String.Equals(currentName, pluginName))
							{
								return true;
							}
						}
					}
				}
			}
			catch (COMException)
			{
			}
			return false;
		}

		public static IList<string> FindPlugin(string pluginName)
		{
            List<string> namespaces = new List<string>();
            if (String.IsNullOrEmpty(pluginName))
            {
                return namespaces;
            }

			try
			{
				HxRegistryWalkerClass registryWalker = new HxRegistryWalkerClass();
				IHxRegNamespaceList help2Namespaces  = 
                    registryWalker.get_RegisteredNamespaceList("");
				
				foreach (IHxRegNamespace currentNamespace in help2Namespaces)
				{
					IHxRegPlugInList p =
						(IHxRegPlugInList)currentNamespace.GetProperty(
                        HxRegNamespacePropId.HxRegNamespacePlugInList);
					foreach (IHxRegPlugIn plugin in p)
					{
						string currentName = (string)plugin.GetProperty(
                            HxRegPlugInPropId.HxRegPlugInName);
						if (String.Compare(currentName, pluginName) == 0)
						{
							namespaces.Add(currentNamespace.Name);
							break;
						}
					}
				}
			}
			catch (COMException)
			{
			}

			return namespaces;
		}

        public static IList<string> FindPluginAsGenericList(string pluginName)
		{
			List<string> namespaces = new List<string>();
            if (String.IsNullOrEmpty(pluginName))
            {
                return namespaces;
            }

            try
			{
				HxRegistryWalkerClass registryWalker = new HxRegistryWalkerClass();
				IHxRegNamespaceList help2Namespaces  = 
                    registryWalker.get_RegisteredNamespaceList("");
				
				foreach (IHxRegNamespace currentNamespace in help2Namespaces)
				{
					IHxRegPlugInList p = 
                        (IHxRegPlugInList)currentNamespace.GetProperty(
                        HxRegNamespacePropId.HxRegNamespacePlugInList);
					foreach (IHxRegPlugIn plugin in p)
					{
						string currentName = (string)plugin.GetProperty(
                            HxRegPlugInPropId.HxRegPlugInName);
						if (String.Compare(currentName, pluginName) == 0)
						{
							namespaces.Add(currentNamespace.Name);
							break;
						}
					}
				}
			}
			catch (COMException)
			{
			}

            return namespaces.AsReadOnly();
		}

		public static string GetFirstMatchingNamespaceName(string matchingName)
		{
			HxRegistryWalkerClass registryWalker;
			IHxRegNamespaceList help2Namespaces;
			try
			{
				registryWalker = new HxRegistryWalkerClass();
				help2Namespaces = registryWalker.get_RegisteredNamespaceList("");
			}
			catch (COMException)
			{
				help2Namespaces = null;
				registryWalker = null;
			}

			if (registryWalker == null || help2Namespaces == null || 
                help2Namespaces.Count == 0 || String.IsNullOrEmpty(matchingName))
			{
				return String.Empty;
			}
			foreach (IHxRegNamespace currentNamespace in help2Namespaces)
			{
				if (PathMatchSpec(currentNamespace.Name, matchingName))
				{
					return currentNamespace.Name;
				}
			}

			return help2Namespaces.ItemAt(1).Name;
		}
	}
}
