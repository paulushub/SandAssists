// Copyright (C) Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

using Sandcastle.Components.MediaLinks;

namespace Sandcastle.Components
{
    /// <summary>
    /// The is the <see langword="abstract"/> base class for build components
    /// supporting images and multimedia objects.
    /// </summary>
    /// <remarks>
    /// The derived classes will allow the user to include images and multimedia
    /// objects, defined in a media configuration file, in the documentations.
    /// </remarks>
    /// <seealso cref="ConceptualMediaComponent"/>
    /// <seealso cref="ReferenceMediaComponent"/>
    public abstract class MediaComponent : BuildComponentEx
    {
        #region Private Fields

        private XPathExpression _artIdExpression;
        private XPathExpression _artFileExpression;
        private XPathExpression _artTextExpression;

        private Dictionary<string, MediaTarget> _artTargets;

        #endregion

        #region Constructors and Destructor

        protected MediaComponent(BuildAssembler assembler, XPathNavigator configuration)
            : base(assembler, configuration)
        {
            _artIdExpression   = XPathExpression.Compile("string(@id)");
            _artFileExpression = XPathExpression.Compile("string(image/@file)");
            _artTextExpression = XPathExpression.Compile("string(image/altText)");

            _artTargets = new Dictionary<string, MediaTarget>(
                StringComparer.OrdinalIgnoreCase);

            XPathNodeIterator targetsNodes = configuration.Select("targets");

            foreach (XPathNavigator targetsNode in targetsNodes)
            {
                string input = targetsNode.GetAttribute("input", String.Empty);
                if (String.IsNullOrEmpty(input))
                    this.WriteMessage(MessageLevel.Error,
                        "Each targets element must have an input attribute specifying a directory containing art files.");

                input = Environment.ExpandEnvironmentVariables(input);

                if (!Directory.Exists(input))
                {
                    this.WriteMessage(MessageLevel.Error, String.Format(
                        "The art input directory '{0}' does not exist.", input));
                }

                string baseOutputPath = targetsNode.GetAttribute(
                    "baseOutput", String.Empty);
                if (!String.IsNullOrEmpty(baseOutputPath))
                {
                    baseOutputPath = Path.GetFullPath(
                        Environment.ExpandEnvironmentVariables(baseOutputPath));
                }

                string outputPathValue = targetsNode.GetAttribute(
                    "outputPath", String.Empty);
                if (string.IsNullOrEmpty(outputPathValue))
                {
                    this.WriteMessage(MessageLevel.Error,
                        "Each targets element must have an output attribute specifying a directory in which to place referenced art files.");
                }
                XPathExpression outputXPath = XPathExpression.Compile(
                    outputPathValue);

                string linkValue = targetsNode.GetAttribute("link", String.Empty);
                if (String.IsNullOrEmpty(linkValue)) linkValue = "../art";
                //linkValue = Environment.ExpandEnvironmentVariables(linkValue);

                string map = targetsNode.GetAttribute("map", String.Empty);
                if (String.IsNullOrEmpty(map))
                {
                    this.WriteMessage(MessageLevel.Error,
                        "Each targets element must have a map attribute specifying a file that maps art ids to files in the input directory.");
                }
                map = Environment.ExpandEnvironmentVariables(map);
                if (!File.Exists(map))
                {
                    WriteMessage(MessageLevel.Error, String.Format(
                        "The art map file '{0}' does not exist.", map));
                }

                string format = targetsNode.GetAttribute("format", String.Empty);
                XPathExpression formatXPath = String.IsNullOrEmpty(format) ?
                    null : XPathExpression.Compile(format);

                string relativeTo = targetsNode.GetAttribute(
                    "relative-to", String.Empty);
                XPathExpression relativeToXPath = String.IsNullOrEmpty(
                    relativeTo) ? null : XPathExpression.Compile(relativeTo);

                this.AddTargets(map, input, baseOutputPath, outputXPath, linkValue,
                    formatXPath, relativeToXPath);
            }

            this.WriteMessage(MessageLevel.Info, String.Format(
                "Indexed {0} art targets.", _artTargets.Count));
        }

        #endregion

        #region Public Properties

        protected MediaTarget this[string id]
        {
            get
            {
                if (String.IsNullOrEmpty(id) || !_artTargets.ContainsKey(id))
                {
                    return null;
                }

                return _artTargets[id];
            }
        }

        #endregion

        #region Public Methods

        #endregion

        #region Private Methods

        private void AddTargets(string map, string input, string baseOutputPath,
            XPathExpression outputXPath, string link, XPathExpression formatXPath,
            XPathExpression relativeToXPath)
        {  
            XPathDocument document = new XPathDocument(map);

            XPathNodeIterator items = document.CreateNavigator().Select("//item");
            if (items == null || items.Count == 0)
            {
                return;
            }
            // Check the required condition, which works for all cases...
            if (!input.EndsWith("\\"))
            {
                input += "\\";
            }

            Type mediaType = typeof(MediaType);
            Type mediaUnit = typeof(MediaSizeUnit);

            foreach (XPathNavigator item in items)
            {
                string id   = (string)item.Evaluate(_artIdExpression);
                string file = (string)item.Evaluate(_artFileExpression);
                string text = (string)item.Evaluate(_artTextExpression);

                if (String.IsNullOrEmpty(id))
                {
                    continue;
                }
                if (text == null)
                {
                    text = String.Empty;
                }

                id = id.ToLower();
                string name = Path.GetFileName(file);

                MediaTarget target    = new MediaTarget();
                target.Id             = id;
                target.InputPath      = Path.GetFullPath(Path.Combine(input, file));
                target.baseOutputPath = baseOutputPath;
                target.OutputXPath    = outputXPath;

                string mediaText = item.GetAttribute("type", String.Empty);
                if (!String.IsNullOrEmpty(mediaText))
                {
                    target.Media = (MediaType)Enum.Parse(mediaType, mediaText, true);
                }
                mediaText = item.GetAttribute("unit", String.Empty);
                if (!String.IsNullOrEmpty(mediaText))
                {
                    target.Unit = (MediaSizeUnit)Enum.Parse(mediaUnit, mediaText, true);
                }
                mediaText = item.GetAttribute("width", String.Empty);
                if (!String.IsNullOrEmpty(mediaText))
                {
                    target.Width = Convert.ToInt32(mediaText);
                }
                mediaText = item.GetAttribute("height", String.Empty);
                if (!String.IsNullOrEmpty(mediaText))
                {
                    target.Height = Convert.ToInt32(mediaText);
                }

                if (string.IsNullOrEmpty(name))
                {
                    target.LinkPath = link;
                }
                else
                {
                    target.LinkPath = String.Format("{0}/{1}", link, name);
                }

                target.Text            = text;
                target.Name            = name;
                target.FormatXPath     = formatXPath;
                target.RelativeToXPath = relativeToXPath;

                XPathNavigator imageNode = item.SelectSingleNode("image");
                if (imageNode != null)
                {
                    string useMap = imageNode.GetAttribute("usemap", String.Empty);
                    if (!String.IsNullOrEmpty(useMap) && useMap[0] == '#')
                    {
                        XPathNavigator imageMap = item.SelectSingleNode("map");
                        if (imageMap != null && useMap.EndsWith(
                            imageMap.GetAttribute("name", String.Empty)))
                        {
                            target.UseMap = useMap;
                            target.Map    = imageMap.OuterXml;
                        }
                    }
                }

                _artTargets[id] = target;
            }
        }

        #endregion
    }
}
