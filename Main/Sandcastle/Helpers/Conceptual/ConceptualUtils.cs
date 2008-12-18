using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Sandcastle.Conceptual
{
    public static class ConceptualUtils
    {
        #region Private Static Fields

        private static Regex validId = new Regex(
            "^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$", 
            RegexOptions.Compiled);

        #endregion

        #region Public Static Fields

        public const string DdueXmlDir        = "DdueXml";
        public const string XmlCompDir        = "XmlComp";
        public const string ExtractedFilesDir = "ExtractedFiles";

        //public const string TocFile           = "ConceptualToc.xml";
        //public const string MetadataFile      = "ContentMetadata.xml";
        //public const string ManifestFile      = "ConceptualManifest.xml";

        #endregion

        #region Private Static Methods

        public static bool IsValidId(string testId)
        {
            if (String.IsNullOrEmpty(testId))
            {
                return false;
            }
            if (testId.Length != 36)
            {
                return false;
            }

            return validId.IsMatch(testId);
        }

        #endregion
    }
}
