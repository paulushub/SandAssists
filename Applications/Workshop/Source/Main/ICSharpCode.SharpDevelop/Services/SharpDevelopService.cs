using System;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop
{
    public static class SharpDevelopService
    {
        public static Version Version
        {
            get
            {
                return new Version(RevisionClass.FullVersion);
            }
        }

        public static string LicenseSentence
        {
            get
            {
                return StringParser.Parse("${res:Dialog.About.License}",
                                          new string[,] { { "License", "GNU Lesser General Public License" } });
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public static string GetVersionInformationString()
        {
            string str = "";
            Version v = typeof(SharpDevelopService).Assembly.GetName().Version;
            str += "Workshop Version     : " + v.ToString() + Environment.NewLine;
            str += ".NET Version         : " + Environment.Version.ToString() + Environment.NewLine;
            str += "OS Version           : " + Environment.OSVersion.ToString() + Environment.NewLine;
            string cultureName = null;
            try
            {
                cultureName = CultureInfo.CurrentCulture.Name;
                str += "Current culture      : " + CultureInfo.CurrentCulture.EnglishName + " (" + cultureName + ")" + Environment.NewLine;
            }
            catch
            {
            }
            try
            {
                if (cultureName == null || !cultureName.StartsWith(ResourceService.Language))
                {
                    str += "Current UI language  : " + ResourceService.Language + Environment.NewLine;
                }
            }
            catch
            {
            }
            try
            {
                if (IntPtr.Size != 4)
                {
                    str += "Running as " + (IntPtr.Size * 8) + " bit process" + Environment.NewLine;
                }
                string PROCESSOR_ARCHITEW6432 = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITEW6432");
                if (!string.IsNullOrEmpty(PROCESSOR_ARCHITEW6432))
                {
                    str += "Running under WOW6432, processor architecture: " + PROCESSOR_ARCHITEW6432 + Environment.NewLine;
                }
            }
            catch
            {
            }
            try
            {
                if (SystemInformation.TerminalServerSession)
                {
                    str += "Terminal Server Session" + Environment.NewLine;
                }
                if (SystemInformation.BootMode != BootMode.Normal)
                {
                    str += "Boot Mode            : " + SystemInformation.BootMode + Environment.NewLine;
                }
            }
            catch
            {
            }
            str += "Working Set Memory   : " + (Environment.WorkingSet / 1024) + "kb" + Environment.NewLine;
            str += "GC Heap Memory       : " + (GC.GetTotalMemory(false) / 1024) + "kb" + Environment.NewLine;
            return str;
        }
    }
}
