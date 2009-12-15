using System;
using System.IO;
using System.Drawing;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Gui
{
    sealed class DriveObject
    {
        private DriveInfo driveInfo;
        private string text = null;

        public string Drive
        {
            get
            {
                return driveInfo.Name;
            }
        }

        public static Image GetImageForFile(string fileName)
        {
            return IconService.GetBitmap(IconService.GetImageForFile(fileName));
        }

        public DriveObject(DriveInfo driveInfo)
        {
            this.driveInfo = driveInfo;

            text = this.Drive.Substring(0, 2);

            switch (driveInfo.DriveType)
            {
                case DriveType.Removable:
                    text += " (${res:MainWindow.Windows.FileScout.DriveType.Removeable})";
                    break;
                case DriveType.Fixed:
                    text += " (${res:MainWindow.Windows.FileScout.DriveType.Fixed})";
                    break;
                case DriveType.CDRom:
                    text += " (${res:MainWindow.Windows.FileScout.DriveType.CD})";
                    break;
                case DriveType.Network:
                    text += " (${res:MainWindow.Windows.FileScout.DriveType.Remote})";
                    break;
            }
            text = StringParser.Parse(text);
        }

        public override string ToString()
        {
            return text;
        }
    }
}
