using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Runtime.InteropServices;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    /// <summary>
    /// This dumps the current document being processed to a file for debugging.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The output is file per topic, and filtering is not currently supported.
    /// </para>
    /// <para>
    /// This is similar to the Sandcastle's DisplayComponent component, but this writes
    /// to a file instead of the console window.
    /// </para>
    /// <para>
    /// This is a sample configuration of this component:
    /// </para>
    /// <code lang="xml">
    /// <![CDATA[
    /// <component type="Sandcastle.Components.FileDisplayComponent" assembly="Sandcastle.Components.dll">
    ///     <display stage="Stage 1" xpath="/" directory="..\..\Displays"/>
    /// </component>
    /// ]]>
    /// </code>
    /// </remarks>
    public sealed class FileDisplayComponent : BuildComponentEx
    {
        #region Private Fields

        private string _xpathFormat;
        private string _workingDir;
        private string _writeStage;
        private string _fileExtension;

        #endregion

        #region Constructors and Destructor

        public FileDisplayComponent(BuildAssembler assembler, XPathNavigator configuration)
            : base(assembler, configuration)
        {
            try
            {
                _xpathFormat   = "/";
                _fileExtension = ".xml";
                _workingDir    = Environment.CurrentDirectory;
                _writeStage    = String.Empty;

                string folder  = "FileDisplays";

                // Obtained the XPath for selecting the part of the document to display...
			    XPathNavigator displayNode = configuration.SelectSingleNode("display");
                if (displayNode != null)
                {   
                    string attValue = displayNode.GetAttribute("xpath", String.Empty);
                    if (!String.IsNullOrEmpty(attValue))
                        _xpathFormat = attValue;
                    
                    attValue = displayNode.GetAttribute("directory", String.Empty);
                    if (!String.IsNullOrEmpty(attValue))
                    {
                        _workingDir = Path.GetFullPath(
                            Environment.ExpandEnvironmentVariables(attValue));
                    }

                    attValue = displayNode.GetAttribute("stage", String.Empty);
                    if (!String.IsNullOrEmpty(attValue))
                        _writeStage = attValue;
                }

                if (!Directory.Exists(_workingDir))
                {
                    Directory.CreateDirectory(_workingDir);
                }

                int itemCount = 0;
                string displayLogDir = Path.Combine(_workingDir, folder + itemCount);

                while (Directory.Exists(displayLogDir) && 
                    !PathIsDirectoryEmpty(displayLogDir))
                {
                    itemCount++;
                    displayLogDir = Path.Combine(_workingDir, folder + itemCount);
                }

                _workingDir = displayLogDir;
                Directory.CreateDirectory(_workingDir);

                if (!String.IsNullOrEmpty(_writeStage))
                {
                    // Since the output text is in the XML format...
                    _writeStage = "<!-- " + _writeStage + " -->";
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key) 
        {
            try
            {
                if (String.IsNullOrEmpty(key))
                {
                    return;
                }

                string xpath = String.Format(_xpathFormat, key);

                object result = document.CreateNavigator().Evaluate(xpath);

                if (result == null)
                {
                    return;
                }

                string fileName = key.Replace(':', '.') + _fileExtension;
                fileName = fileName.Replace('#', '.');
                fileName = fileName.Replace('<', '_');
                fileName = fileName.Replace('>', '_');
                fileName = fileName.Replace("..", ".");

                string filePath = Path.Combine(_workingDir, fileName);
                bool fileExits  = File.Exists(filePath);
                using (TextWriter writer = File.AppendText(filePath))
                {
                    if (fileExits)
                    {
                        writer.WriteLine();
                    }
                    writer.WriteLine("<!-- " + _writeStage + " -->");
                    writer.WriteLine();

                    XPathNodeIterator nodes = result as XPathNodeIterator;
                    if (nodes != null)
                    {
                        foreach (XPathNavigator node in nodes)
                        {
                            writer.WriteLine(node.OuterXml);
                        }
                        return;
                    }

                    writer.WriteLine(result.ToString());
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion

        #region Private Methods

        [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
        [return: MarshalAsAttribute(UnmanagedType.Bool)]
        private static extern bool PathIsDirectoryEmpty(
            [MarshalAsAttribute(UnmanagedType.LPWStr), In] string pszPath);

        #endregion
    }
}
