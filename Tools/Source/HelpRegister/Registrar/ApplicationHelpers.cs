//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.ComponentModel;

using System.Security;
using System.Security.Principal;
using System.Security.Permissions;

using Microsoft.Win32;

namespace Sandcastle.HelpRegister
{
    /// <summary>
    /// 
    /// </summary>
	public static class ApplicationHelpers
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="classId"></param>
        /// <returns></returns>
		public static bool IsClassRegistered(string classId)
		{
            if (String.IsNullOrEmpty(classId))
            {
                return false;
            }

			try
			{
				RegistryKey regKey = Registry.ClassesRoot.OpenSubKey
					(String.Format(CultureInfo.InvariantCulture, 
                    @"CLSID\{0}\InprocServer32", classId), false);
                if (regKey == null)
                {
                    return false;
                }
				string help2Library = (string)regKey.GetValue("", String.Empty);
				regKey.Close();

				return (!String.IsNullOrEmpty(help2Library) && 
                    File.Exists(help2Library));
			}
			catch (ArgumentException)
			{
			}
			catch (SecurityException)
			{
			}
			return false;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invalidProcesses"></param>
		[PermissionSet(SecurityAction.LinkDemand, Name="Execution")]
		public static void KillProcesses(string[] invalidProcesses)
		{
			if (invalidProcesses == null || invalidProcesses.Length == 0)
			{
				return;
			}

			Process[] runningProcesses = Process.GetProcesses();
			foreach (Process currentProcess in runningProcesses)
			{
				try
				{
					string fileName = Path.GetFileName(
                        currentProcess.MainModule.FileName);
					foreach (string invalidProcess in invalidProcesses)
					{
						if (String.Compare(invalidProcess.ToLower(
                            CultureInfo.InvariantCulture), 
                            fileName.ToLower(CultureInfo.InvariantCulture)) == 0)
						{
							currentProcess.Kill();
							currentProcess.WaitForExit();
						}
					}
				}
				catch (Win32Exception)
				{
				}
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
		public static bool IsThisUserPrivileged()
		{
			switch(Environment.OSVersion.Platform)
			{
				case PlatformID.Win32NT:
					AppDomain.CurrentDomain.SetPrincipalPolicy(
                        PrincipalPolicy.WindowsPrincipal);
					WindowsPrincipal wp = (Thread.CurrentPrincipal as WindowsPrincipal);
					return (wp.IsInRole(WindowsBuiltInRole.Administrator) || 
                        wp.IsInRole(WindowsBuiltInRole.PowerUser));

				case PlatformID.Win32Windows:
					return true;

				default:
					return false;
			}
		}
	}
}
