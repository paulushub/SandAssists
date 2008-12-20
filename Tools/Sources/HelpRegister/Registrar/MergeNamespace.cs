//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
using System;
using System.Globalization;
using System.Runtime.InteropServices;

using MSHelpServices;

namespace Sandcastle.HelpRegister
{
    /// <summary>
    /// 
    /// </summary>
	public static class MergeNamespace
	{
        /// <summary>
        /// 
        /// </summary>
        /// <param name="namespaceName"></param>
		public static void CallMerge(string namespaceName)
		{
            if (String.IsNullOrEmpty(namespaceName))
            {
                return;
            }

			try
			{
				HxSessionClass session = new HxSessionClass();
				session.Initialize(String.Format(CultureInfo.InvariantCulture, 
                    "ms-help://{0}", namespaceName), 0);

				// Next lesson about the Help 2.0 API: You have to wait until
				// "MergeIndex" is ready. This is a console tool, so ... But
				// if you want to do it with a GUI tool, you have to use a
				// thread or something. I used a thread in my Delphi version.

				IHxCollection collection = session.Collection;
                if (collection != null)
                {
                    session.IndexMergeStatus += 
                        new IHxSessionEvents_IndexMergeStatusEventHandler(
                            OnIndexMergeStatus);
                    session.MergeIndexFileName += 
                        new IHxSessionEvents_MergeIndexFileNameEventHandler(
                            OnMergeIndexFileName);
                    session.PrintMergeStatus += 
                        new IHxSessionEvents_PrintMergeStatusEventHandler(
                            OnPrintMergeStatus);

                    collection.MergeIndex();
                }
			}
			catch (COMException)
			{
			}
		}

        private static void OnPrintMergeStatus(object pSession, object pCancel, 
            int status)
        {
        }

        private static void OnMergeIndexFileName(object pDisp, string bstrFile)
        {
        }

        private static void OnIndexMergeStatus(object pSession, object pCancel, 
            int status)
        {
        }
	}
}
