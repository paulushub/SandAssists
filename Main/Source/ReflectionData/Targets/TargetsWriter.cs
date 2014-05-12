// Copyright ｩ Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.IO;
using System.Text;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace Sandcastle.ReflectionData.Targets
{
    public static class TargetsWriter
    {
        private static XmlWriterSettings _settings;

        public static string WriteXml(Target target)
        {
            if (target == null)
            {
                return String.Empty;
            }

            if (_settings == null)
            {
                _settings = new XmlWriterSettings();
                _settings.Indent             = false;
                _settings.CloseOutput        = true;
                _settings.OmitXmlDeclaration = true;
                _settings.ConformanceLevel   = ConformanceLevel.Fragment;
            }

            StringBuilder builder   = new StringBuilder();
            StringWriter textWriter = new StringWriter(builder); 
            XmlWriter writer = XmlWriter.Create(textWriter, _settings);
            target.WriteXml(writer);
            writer.Close();

            return builder.ToString();
        }
    }
}
