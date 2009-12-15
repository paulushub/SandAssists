using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ICSharpCode.SharpDevelop.Gui
{
    static class IconManager
    {
        private static ImageList icons = new ImageList();
        private static Dictionary<string, int> iconIndecies = new Dictionary<string, int>();

        static IconManager()
        {
            icons.ColorDepth = ColorDepth.Depth32Bit;
        }

        public static ImageList List
        {
            get
            {
                return icons;
            }
        }

        public static int GetIndexForFile(string file)
        {
            string key;

            // icon files and exe files can have their custom icons
            if (Path.GetExtension(file).Equals(".ico", StringComparison.OrdinalIgnoreCase) ||
               Path.GetExtension(file).Equals(".exe", StringComparison.OrdinalIgnoreCase))
            {
                key = file;
            }
            else
            {
                key = Path.GetExtension(file).ToLower();
            }

            // clear the icon cache
            if (icons.Images.Count > 100)
            {
                icons.Images.Clear();
                iconIndecies.Clear();
            }

            if (iconIndecies.ContainsKey(key))
            {
                return iconIndecies[key];
            }
            else
            {
                icons.Images.Add(DriveObject.GetImageForFile(file));
                int index = icons.Images.Count - 1;
                iconIndecies.Add(key, index);
                return index;
            }
        }
    }
}
