using System;
using System.Xml;
using System.Text;
using System.Collections.Generic;

using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Internal.Templates
{
    public static class ClipService
    {
        private static List<ClipTemplate> _textTemplates = new List<ClipTemplate>();

        static ClipService()
        {
            List<string> files = FileUtility.SearchDirectory(FileUtility.Combine(
                PropertyService.DataDirectory, "options", "textlib"), "*.xml");
            foreach (string file in files)
            {
                LoadTextTemplate(file);
            }
        }

        public static ICollection<ClipTemplate> TextTemplates
        {
            get
            {
                return _textTemplates;
            }
        }

        private static void LoadTextTemplate(string filename)
        {
            ClipTemplate template = new ClipTemplate();
            template.Load(filename);

            _textTemplates.Add(template);
        }
    }
}
