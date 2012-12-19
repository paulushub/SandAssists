using System;
using System.IO;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    public abstract class PreTransComponent : BuildComponentEx
    {
        #region Constructors and Destructor

        protected PreTransComponent(BuildAssembler assembler, 
            XPathNavigator configuration) : base(assembler, configuration)
        {
            BuildLocalizedContents localizedContents = 
                BuildLocalizedContents.Instance;
            if (localizedContents != null && !localizedContents.IsInitialized)
            {
                XPathNavigator navigator = configuration.SelectSingleNode(
                    "BuildLocalizedContents");
                if (navigator != null)
                {
                    string contentFile = navigator.GetAttribute("file", 
                        String.Empty);
                    if (!String.IsNullOrEmpty(contentFile))
                    {
                        contentFile = Environment.ExpandEnvironmentVariables(contentFile);
                        contentFile = Path.GetFullPath(contentFile);

                        if (File.Exists(contentFile))
                        {
                            localizedContents.Initialize(contentFile, assembler.MessageWriter);
                        }
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        public override void Apply(XmlDocument document, string key)
        {
        }

        #endregion

        #region Protected Methods

        #endregion
    }
}
