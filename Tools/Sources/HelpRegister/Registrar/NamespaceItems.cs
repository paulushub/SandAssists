//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
using System;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Collections.Generic;

namespace Sandcastle.HelpRegister
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
	public sealed class NamespaceItems : IRegistrationItems
    {
        #region Private Fields

        private string _xmlFilename = String.Empty;
        private string _xmlXpathSequence = String.Empty;
        private XmlNamespaceManager _xmlns;
        private XmlDocument _xmldoc = new XmlDocument();
        private List<NamespaceItem> _helpNamespaces = new List<NamespaceItem>();

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlFilename"></param>
        public NamespaceItems(string xmlFilename) 
            : this(xmlFilename, String.Empty)
		{
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlFilename"></param>
        /// <param name="xmlXpathSequence"></param>
		public NamespaceItems(string xmlFilename, string xmlXpathSequence)
		{
			_xmlFilename      = xmlFilename;
			_xmlXpathSequence = xmlXpathSequence;
			
            Initialize();
		}

		#endregion

        #region Public Events

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<NamespaceEventArgs> RegisterOrRemoveNamespace;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<NamespaceEventArgs> RegisterOrRemoveHelpDocument;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<NamespaceEventArgs> RegisterOrRemoveFilter;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<PluginEventArgs> RegisterOrRemovePlugin;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<MergingEventArgs> NamespaceMerge;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<LoggingEventArgs> LogProgress;

        #endregion

        #region Private Methods

        private void Initialize()
        {
            _xmlns = new XmlNamespaceManager(_xmldoc.NameTable);
            _xmlns.AddNamespace(String.Empty, XmlValidator.Help2NamespaceUri);
            _xmlns.AddNamespace("help2", XmlValidator.Help2NamespaceUri);

            _xmldoc.Load(_xmlFilename);

            XmlNodeList nodes = _xmldoc.SelectNodes(String.Format(
                CultureInfo.InvariantCulture, "/register/namespace{0}",
                _xmlXpathSequence), _xmlns);

            foreach (XmlNode node in nodes)
            {
                _helpNamespaces.Add(new NamespaceItem(node.CreateNavigator()));
            }
        }

        private void PatchXmlNode(string namespaceName, string pluginName,
            string matchingName)
        {
            if (_xmldoc == null ||
                String.IsNullOrEmpty(namespaceName) ||
                String.IsNullOrEmpty(pluginName)    ||
                String.IsNullOrEmpty(matchingName))
            {
                return;
            }

            XmlNode node = _xmldoc.SelectSingleNode(String.Format(
                CultureInfo.InvariantCulture,
                "/register/namespace[@name=\"{0}\"]/plugin/child[@name=\"{1}\"]",
                namespaceName, pluginName), _xmlns);

            XmlHelper.SetXmlStringValue(node.CreateNavigator(), "name", matchingName);

            _xmldoc.Save(_xmlFilename);
        }

        #region Events Handlers

        private void OnRegisterOrRemoveNamespace(NamespaceEventArgs e)
        {
            if (RegisterOrRemoveNamespace != null)
            {
                RegisterOrRemoveNamespace(this, e);
            }
        }

        private void OnRegisterOrRemoveHelpDocument(NamespaceEventArgs e)
        {
            if (RegisterOrRemoveHelpDocument != null)
            {
                RegisterOrRemoveHelpDocument(this, e);
            }
        }

        private void OnRegisterOrRemoveFilter(NamespaceEventArgs e)
        {
            if (RegisterOrRemoveFilter != null)
            {
                RegisterOrRemoveFilter(this, e);
            }
        }

        private void OnRegisterOrRemovePlugin(PluginEventArgs e)
        {
            if (RegisterOrRemovePlugin != null)
            {
                RegisterOrRemovePlugin(this, e);
            }
        }

        private void OnNamespaceMerge(MergingEventArgs e)
        {
            if (NamespaceMerge != null)
            {
                NamespaceMerge(this, e);
            }
        }

        private void OnLogProgress(LoggingEventArgs e)
        {
            if (LogProgress != null)
            {
                LogProgress(this, e);
            }
        }

        #endregion

        #endregion

        #region IRegistrationItems Members

        #region Register Method

        /// <summary>
        /// 
        /// </summary>
        public void Register()
		{
			foreach (NamespaceItem helpNamespace in _helpNamespaces)
			{
				if (String.IsNullOrEmpty(helpNamespace.Name) ||
				    String.IsNullOrEmpty(helpNamespace.CollectionLevelFile) ||
				    !File.Exists(helpNamespace.CollectionLevelFile))
				{
					continue;
				}

				using (HelpRegistrar register = new HelpRegistrar())
				{
					// force Help 2.0 namespace creation
					if (helpNamespace.ForceCreation)
					{
						OnLogProgress(new LoggingEventArgs(helpNamespace.ToString()));
						OnRegisterOrRemoveNamespace(new NamespaceEventArgs(helpNamespace.Name, true));
						register.RegisterNamespace(helpNamespace.Name,
						                           helpNamespace.CollectionLevelFile,
						                           helpNamespace.Description);
					}

					// register Help 2.0 documents
					foreach (DocumentItem document in helpNamespace.Documents)
					{
						if (String.IsNullOrEmpty(document.Id) ||
						    String.IsNullOrEmpty(document.Hxs) ||
						    !File.Exists(document.Hxs))
						{
							continue;
						}

						OnLogProgress(new LoggingEventArgs(document.ToString()));
						OnRegisterOrRemoveHelpDocument(new NamespaceEventArgs(document.Id, true));
						register.RegisterHelpFile(helpNamespace.Name,
						                          document.Id,
						                          document.LanguageId,
						                          document.Hxs,
						                          document.Hxi,
						                          document.Hxq,
						                          document.Hxr,
						                          document.HxsMediaId,
						                          document.HxqMediaId,
						                          document.HxrMediaId,
						                          document.SampleMediaId);
					}

					// register Help 2.0 filters
					foreach (FilterItem filter in helpNamespace.Filters)
					{
						if (String.IsNullOrEmpty(filter.Name))
						{
							continue;
						}

						OnLogProgress(new LoggingEventArgs(filter.ToString()));
						OnRegisterOrRemoveFilter(new NamespaceEventArgs(filter.Name, true));
						register.RegisterFilter(helpNamespace.Name, filter.Name, filter.Query);
					}

					// register Help 2.0 child plug-ins
					foreach (PluginChildItem plugin in helpNamespace.Plugins)
					{
						if (String.IsNullOrEmpty(plugin.MatchingName))
						{
							continue;
						}

						OnLogProgress(new LoggingEventArgs(plugin.ToString()));
						OnRegisterOrRemovePlugin(new PluginEventArgs(
                            helpNamespace.Name, plugin.MatchingName, true));

						register.RegisterPlugin(helpNamespace.Name, 
                            plugin.MatchingName);

						if (String.Compare(plugin.Name, plugin.MatchingName) != 0)
						{
							PatchXmlNode(helpNamespace.Name, plugin.Name, plugin.MatchingName);
						}
					}

					// merge Help 2.0 namespace
					if (helpNamespace.Merge)
					{
						OnLogProgress(new LoggingEventArgs(String.Format(
                            CultureInfo.InvariantCulture, "[merging {0}]", helpNamespace.Name)));
						OnNamespaceMerge(new MergingEventArgs(helpNamespace.Name));
						MergeNamespace.CallMerge(helpNamespace.Name);

                        IList<string> connectedNamespaces = helpNamespace.ConnectedNamespaces;
                        int itemCount = connectedNamespaces.Count;
                        for (int i = 0; i < itemCount; i++)
                        {
                            string connectedNamespace = connectedNamespaces[i];
                            OnLogProgress(new LoggingEventArgs(String.Format(
                                CultureInfo.InvariantCulture, "[merging {0}]",
                                connectedNamespace)));

                            OnNamespaceMerge(new MergingEventArgs(
                                connectedNamespace));
                            MergeNamespace.CallMerge(connectedNamespace);
                        }
					}
				}
			}
        }

        #endregion

        #region Unregister Method

        /// <summary>
        /// 
        /// </summary>
        public void Unregister()
		{
			foreach (NamespaceItem helpNamespace in _helpNamespaces)
			{
				if (String.IsNullOrEmpty(helpNamespace.Name))
				{
					continue;
				}

				using (HelpRegistrar register = new HelpRegistrar())
				{
					// remove this Help 2.0 namespace, if it is a plug-in
					foreach (string connectedNamespace in helpNamespace.ConnectedNamespaces)
					{
						OnRegisterOrRemovePlugin(new PluginEventArgs(
                            connectedNamespace, helpNamespace.Name, false));
						register.RemovePlugin(connectedNamespace, helpNamespace.Name);

						OnNamespaceMerge(new MergingEventArgs(
                            connectedNamespace));
						MergeNamespace.CallMerge(connectedNamespace);
					}

					// remove this namespace's child plug-ins
					foreach (PluginChildItem plugin in helpNamespace.Plugins)
					{
						OnLogProgress(new LoggingEventArgs(plugin.ToString()));
						OnRegisterOrRemovePlugin(new PluginEventArgs(
                            helpNamespace.Name, plugin.MatchingName, false));
						register.RemovePlugin(helpNamespace.Name, 
                            plugin.MatchingName);
					}

					// remove this namespace's filters
					foreach (FilterItem filter in helpNamespace.Filters)
					{
						OnLogProgress(new LoggingEventArgs(filter.ToString()));
						OnRegisterOrRemoveFilter(new NamespaceEventArgs(
                            filter.Name, false));
						register.RemoveFilter(helpNamespace.Name, filter.Name);
					}

					// remove this namespace's documents
					foreach (DocumentItem document in helpNamespace.Documents)
					{
						OnLogProgress(new LoggingEventArgs(document.ToString()));

						OnRegisterOrRemoveHelpDocument(new NamespaceEventArgs(
                            document.Id, false));

						register.RemoveHelpFile(helpNamespace.Name, 
                            document.Id, document.LanguageId);
					}

					// remove this namespace, ...
					if (helpNamespace.Remove)
					{
						OnRegisterOrRemoveNamespace(new NamespaceEventArgs(
                            helpNamespace.Name, false));
						register.RemoveNamespace(helpNamespace.Name);
					}
					// ... or just (re)merge it
					else
					{
						OnNamespaceMerge(new MergingEventArgs(
                            helpNamespace.Name));
						MergeNamespace.CallMerge(helpNamespace.Name);
					}
				}
			}
        }

        #endregion

        #endregion
    }
}
