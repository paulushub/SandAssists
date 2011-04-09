using System;
using System.Xml;
using System.Xml.XPath;

namespace Sandcastle.Components.MediaLinks
{
    [Serializable]
    public sealed class MediaTarget
    {
        public int Height;

        public int Width;

        public string Id;

        public string UseMap;

        public string Map;

        public string Name;

        public string Text;

        public string InputPath;

        public string baseOutputPath;

        public string LinkPath;

        public MediaType Media;

        public MediaSizeUnit Unit;

        public XPathExpression OutputXPath;

        public XPathExpression FormatXPath;

        public XPathExpression RelativeToXPath;

        public MediaTarget()
        {
            Media = MediaType.Image;
            Unit  = MediaSizeUnit.None;
        }

        public bool HasMap
        {
            get
            {
                return (!String.IsNullOrEmpty(UseMap) &&
                    !String.IsNullOrEmpty(Map));
            }
        }

        public string UnitText
        {
            get
            {
                switch (Unit)
                {
                    case MediaSizeUnit.None:
                        return String.Empty;
                    case MediaSizeUnit.Pixel:
                        return "px";
                    case MediaSizeUnit.Point:
                        return "pt";
                    case MediaSizeUnit.Percent:
                        return "%";
                    case MediaSizeUnit.Em:
                        return "em";
                    case MediaSizeUnit.Pica:
                        return "pc";
                    case MediaSizeUnit.Ex:
                        return "ex";
                }
                return String.Empty;
            }
        }
    }
}
