﻿using System;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;

namespace Sandcastle.Components
{
    /// <summary>
    /// This component provides a means to create document processing branches in
    /// the build process, with each branch processing a cloned version of the document.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This components is the same as a similarly named build component in the official
    /// Sandcastle package. It is re-implemented here to add support for "default"
    /// branch, which will process the original document and prevent waste.
    /// </para>
    /// </remarks>
    public sealed class CloneComponent : BuildComponentEx
    {
        #region Private Fields

        private IList<BuildComponent> _defaultBranch;
        private List<IList<BuildComponent>> _listBranches;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes new instance of the <see cref="CloneComponent"/> class with
        /// the specified parameters.
        /// </summary>
        /// <param name="assembler"></param>
        /// <param name="configuration"></param>
        public CloneComponent(BuildAssembler assembler, XPathNavigator configuration)
            : base(assembler, configuration)
        {   
            try
            {
                _listBranches = new List<IList<BuildComponent>>();

                // Select and process all the branches...
                XPathNodeIterator branchNodes = configuration.Select("branch");
                if (branchNodes != null && branchNodes.Count != 0)
                {
                    foreach (XPathNavigator branchNode in branchNodes)
                    {
                        IList<BuildComponent> components = assembler.LoadComponents(branchNode);
                        if (components != null && components.Count != 0)
                        {
                            _listBranches.Add(components);
                        }
                    }
                }

                // Select and process the "default" node, only one is expected...
                XPathNavigator defaultNode = configuration.SelectSingleNode("default");
                if (defaultNode != null)
                {
                    _defaultBranch = assembler.LoadComponents(defaultNode);
                }

                // For the special case where neither "branch" nor "default" is
                // specified, we treat anything there as default...
                if ((branchNodes == null || branchNodes.Count == 0) &&
                    defaultNode == null)
                {
                    _defaultBranch = assembler.LoadComponents(configuration);
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
                // Process the various branches, if available, with the cloned document...
                int itemCount = _listBranches == null ? 0 : _listBranches.Count; 
                for (int i = 0; i < itemCount; i++)
                {
                    IList<BuildComponent> components = _listBranches[i];
                    XmlDocument clonedDocument  = (XmlDocument)document.Clone();
                    for (int j = 0; j < components.Count; j++)
                    {
                        BuildComponent component = components[j];
                        component.Apply(clonedDocument, key);
                    }
                }

                // Process the default branch, with the original document...
                itemCount = _defaultBranch == null ? 0 : _defaultBranch.Count;
                for (int k = 0; k < itemCount; k++)
                {
                    BuildComponent component = _defaultBranch[k];
                    component.Apply(document, key);
                }
            }
            catch (Exception ex)
            {
                this.WriteMessage(MessageLevel.Error, ex);
            }
        }

        #endregion
    }
}
