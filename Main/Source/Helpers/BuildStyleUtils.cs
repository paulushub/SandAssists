using System;
using System.Text;
using System.Collections.Generic;

namespace Sandcastle
{
    public static class BuildStyleUtils
    {
        public static string StyleFolder(BuildStyleType styleType)
        {
            if (styleType == BuildStyleType.ClassicWhite || 
                styleType == BuildStyleType.ClassicBlue)
            {
                return "Vs2005";
            }

            return "Vs2005";
        }
    }
}
