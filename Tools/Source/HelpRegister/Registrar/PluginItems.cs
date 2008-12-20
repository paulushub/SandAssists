//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Globalization;
using System.Collections.Generic;

namespace Sandcastle.HelpRegister
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public sealed class PluginItems : IRegistrationItems
    {
        #region Private Fields

        private string _xmlFilename;
        private string _xmlXpathSequence;
        private XmlDocument _xmldoc;
        private XmlNamespaceManager _xmlns;
        private List<PluginParentItem> _parentPlugins;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// 
        /// </summary>
        private PluginItems()
        {
            _xmlFilename      = String.Empty;
            _xmlXpathSequence = String.Empty;
            _xmldoc           = new XmlDocument();
            _parentPlugins    = new List<PluginParentItem>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlFilename"></param>
        public PluginItems(string xmlFilename) 
            : this(xmlFilename, String.Empty)
		{
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlFilename"></param>
        /// <param name="xmlXpathSequence"></param>
		public PluginItems(string xmlFilename, string xmlXpathSequence)
            : this()
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
        public event EventHandler<PluginEventArgs> RegisterOrRemovePlugin;
        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<MergingEventArgs> NamespaceMerge;

        #endregion

        #region Private Methods

        private void Initialize()
        {
            _xmlns = new XmlNamespaceManager(_xmldoc.NameTable);
            _xmlns.AddNamespace(String.Empty, XmlValidator.Help2NamespaceUri);
            _xmlns.AddNamespace("help2", XmlValidator.Help2NamespaceUri);

            _xmldoc.Load(_xmlFilename);
            XmlNodeList nodes = _xmldoc.SelectNodes(String.Format(
                CultureInfo.InvariantCulture, "/register/plugin{0}", 
                _xmlXpathSequence), _xmlns);

            foreach (XmlNode node in nodes)
            {
                _parentPlugins.Add(new PluginParentItem(node.CreateNavigator()));
            }
        }

        private void PatchParentXmlNode(string realParentName, 
            string matchingParentName)
		{
			if (_xmldoc == null                      ||
			    String.IsNullOrEmpty(realParentName) ||
			    String.IsNullOrEmpty(matchingParentName))
			{
				return;
			}

			XmlNode node = _xmldoc.SelectSingleNode
				(String.Format(CultureInfo.InvariantCulture, 
                "/register/plugin[@parent=\"{0}\"]", realParentName));
			XmlHelper.SetXmlStringValue(node.CreateNavigator(), 
                "parent", matchingParentName);

			_xmldoc.Save(_xmlFilename);
		}

		private void PatchChildXmlNode(string realParentName, 
            string realChildName, string matchingChildName)
		{
			if (_xmldoc == null                      ||
			    String.IsNullOrEmpty(realParentName) ||
			    String.IsNullOrEmpty(realChildName)  ||
			    String.IsNullOrEmpty(matchingChildName))
			{
				return;
			}

			XmlNode node = _xmldoc.SelectSingleNode(String.Format(
                CultureInfo.InvariantCulture, 
                "/register/plugin[@parent=\"{0}\"]", realParentName));

			if (node != null)
			{
				string childName = XmlHelper.GetXmlStringValue(
                    node.CreateNavigator(), "child");
				if (childName.Length > 0 && 
                    String.Compare(childName, realChildName) == 0)
				{
					XmlHelper.SetXmlStringValue(
                        node.CreateNavigator(), "child", matchingChildName);
				}
				else
				{
					XmlNode child = node.SelectSingleNode(String.Format(
                        CultureInfo.InvariantCulture, "child[@name=\"{0}\"]", 
                        realChildName));
					XmlHelper.SetXmlStringValue(
                        child.CreateNavigator(), "name", matchingChildName);
				}

				_xmldoc.Save(_xmlFilename);
			}
        }

        #region Events Handlers

        private void OnRegisterOrRemovePlugin(PluginEventArgs e)
        {
            if (RegisterOrRemovePlugin != null)
            {
                RegisterOrRemovePlugin(null, e);
            }
        }

        private void OnNamespaceMerge(MergingEventArgs e)
        {
            if (NamespaceMerge != null)
            {
                NamespaceMerge(null, e);
            }
        }

        #endregion

        #endregion

        #region IRegistrationItems Members

        /// <summary>
        /// 
        /// </summary>
        public void Register()
        {
            foreach (PluginParentItem plugin in _parentPlugins)
            {
                if (String.IsNullOrEmpty(plugin.MatchingName))
                {
                    continue;
                }

                using (HelpRegistrar register = new HelpRegistrar())
                {
                    foreach (PluginChildItem child in plugin)
                    {
                        if (String.IsNullOrEmpty(child.MatchingName))
                        {
                            continue;
                        }

                        OnRegisterOrRemovePlugin(new PluginEventArgs(
                            plugin.MatchingName, child.MatchingName, true));
                        register.RegisterPlugin(plugin.MatchingName, child.MatchingName);

                        if (String.Compare(plugin.Name, plugin.MatchingName) != 0)
                        {
                            PatchParentXmlNode(plugin.Name, plugin.MatchingName);
                        }
                        if (String.Compare(child.Name, child.MatchingName) != 0)
                        {
                            PatchChildXmlNode(plugin.Name, child.Name, child.MatchingName);
                        }
                    }

                    if (plugin.Merge)
                    {
                        OnNamespaceMerge(new MergingEventArgs(plugin.MatchingName));
                        MergeNamespace.CallMerge(plugin.MatchingName);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Unregister()
        {
            foreach (PluginParentItem plugin in _parentPlugins)
            {
                if (String.IsNullOrEmpty(plugin.MatchingName))
                {
                    continue;
                }

                using (HelpRegistrar register = new HelpRegistrar())
                {
                    foreach (PluginChildItem child in plugin)
                    {
                        if (String.IsNullOrEmpty(child.MatchingName))
                        {
                            continue;
                        }

                        OnRegisterOrRemovePlugin(new PluginEventArgs(
                            plugin.MatchingName, child.MatchingName, false));
                        register.RemovePlugin(plugin.MatchingName, child.MatchingName);
                    }

                    if (plugin.Merge)
                    {
                        OnNamespaceMerge(new MergingEventArgs(plugin.MatchingName));
                        MergeNamespace.CallMerge(plugin.MatchingName);
                    }
                }
            }
        }

        #endregion
    }
}
