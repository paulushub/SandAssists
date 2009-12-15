// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3671 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ICSharpCode.Core
{
    public static class AddInLoader
    {
        public static AddIn Load(TextReader textReader)
        {
            return Load(textReader, null);
        }

        public static AddIn Load(TextReader textReader, string hintPath)
        {
            AddIn addIn = new AddIn();
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.ConformanceLevel  = ConformanceLevel.Document;
            settings.IgnoreComments    = true;

            using (XmlReader reader = XmlReader.Create(textReader, settings))
            {
                while (reader.Read())
                {
                    if (reader.IsStartElement())
                    {
                        switch (reader.LocalName)
                        {
                            case "AddIn":
                                addIn.Properties = Properties.ReadFromAttributes(reader);
                                SetupAddIn(reader, addIn, hintPath);
                                break;
                            default:
                                throw new AddInLoadException("Unknown add-in file.");
                        }
                    }
                }
            }
            return addIn;
        }

        public static AddIn Load(string fileName)
        {
            try
            {
                using (TextReader textReader = File.OpenText(fileName))
                {
                    AddIn addIn = Load(textReader, Path.GetDirectoryName(fileName));
                    addIn.FileName = fileName;
                    return addIn;
                }
            }
            catch (Exception e)
            {
                throw new AddInLoadException("Can't load " + fileName, e);
            }
        }

        private static void SetupAddIn(XmlReader reader, AddIn addIn, string hintPath)
        {
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.IsStartElement())
                {
                    switch (reader.LocalName)
                    {
                        case "StringResources":
                        case "BitmapResources":
                            if (reader.AttributeCount != 1)
                            {
                                throw new AddInLoadException("BitmapResources requires ONE attribute.");
                            }

                            string filename = StringParser.Parse(reader.GetAttribute("file"));

                            if (reader.LocalName == "BitmapResources")
                            {
                                addIn.BitmapResources.Add(filename);
                            }
                            else
                            {
                                addIn.StringResources.Add(filename);
                            }
                            break;
                        case "Runtime":
                            if (!reader.IsEmptyElement)
                            {
                                Runtime.ReadSection(reader, addIn, hintPath);
                            }
                            break;
                        case "Include":
                            if (reader.AttributeCount != 1)
                            {
                                throw new AddInLoadException("Include requires ONE attribute.");
                            }
                            if (!reader.IsEmptyElement)
                            {
                                throw new AddInLoadException("Include nodes must be empty!");
                            }
                            if (hintPath == null)
                            {
                                throw new AddInLoadException("Cannot use include nodes when hintPath was not specified (e.g. when AddInManager reads a .addin file)!");
                            }
                            string fileName = Path.Combine(hintPath, reader.GetAttribute(0));
                            XmlReaderSettings xrs = new XmlReaderSettings();
                            xrs.ConformanceLevel = ConformanceLevel.Fragment;
                            using (XmlReader includeReader = XmlReader.Create(fileName, xrs))
                            {
                                SetupAddIn(includeReader, addIn, Path.GetDirectoryName(fileName));
                            }
                            break;
                        case "Path":
                            if (reader.AttributeCount != 1)
                            {
                                throw new AddInLoadException("Import node requires ONE attribute.");
                            }
                            string pathName = reader.GetAttribute(0);
                            ExtensionPath extensionPath = addIn.GetExtensionPath(pathName);
                            if (!reader.IsEmptyElement)
                            {
                                ExtensionPath.SetUp(extensionPath, reader, "Path");
                            }
                            break;
                        case "Manifest":
                            addIn.Manifest.ReadManifestSection(reader, hintPath);
                            break;
                        default:
                            throw new AddInLoadException("Unknown root path node:" + reader.LocalName);
                    }
                }
            }
        }
    }
}
