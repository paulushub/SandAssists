// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Xml.XPath;
using System.Collections.Generic;

namespace Sandcastle.ReflectionData.References
{
    public static class ReferencesWriter
    {
        private static XmlWriterSettings _settings;

        public static string Write(Reference reference)
        {
            if (reference == null)
            {
                return String.Empty;
            }

            if (_settings == null)
            {
                _settings = new XmlWriterSettings();
                _settings.Indent = false;
                _settings.OmitXmlDeclaration = true;
                _settings.ConformanceLevel = ConformanceLevel.Fragment;
            }

            StringBuilder builder = new StringBuilder();
            StringWriter textWriter = new StringWriter(builder);

            using (XmlWriter writer = XmlWriter.Create(textWriter, _settings))
            {
                reference.WriteXml(writer);
            }

            return builder.ToString();
        }
    }
}
