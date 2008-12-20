//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

using System.Xml;
using System.Xml.XPath;

namespace Sandcastle.HelpRegister
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
	public class NamespaceItem
    {
        #region Private Fields

        private bool _update;
        private bool _merge;
        private bool _noremove;
        private string _name;
		private string _description;
		private string _collection;
        private IList<string> _connections;
        private List<FilterItem> _filters;
		private List<DocumentItem> _documents;
		private List<PluginChildItem> _plugins;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// 
        /// </summary>
        protected NamespaceItem()
        {
            _merge     = true;
            _noremove  = true;
            _documents = new List<DocumentItem>();
            _filters   = new List<FilterItem>();
            _plugins   = new List<PluginChildItem>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootNode"></param>
        public NamespaceItem(XPathNavigator rootNode)
            : this()
		{
			if (rootNode == null)
			{
				throw new ArgumentNullException("rootNode");
			}
			
			_name        = XmlHelper.GetXmlStringValue(rootNode, "name");
			_description = XmlHelper.GetXmlStringValue(rootNode, "description");
			_collection  = XmlHelper.GetXmlStringValue(rootNode, "file");
			_update      = XmlHelper.GetXmlBoolValue(rootNode, "update");
			_merge       = XmlHelper.GetXmlBoolValue(rootNode, "merge", true);
			_noremove    = XmlHelper.GetXmlBoolValue(rootNode, "noremove");

			_connections = PluginSearch.FindPluginAsGenericList(_name);

			Initialize(rootNode);
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string Name
		{
			get { return _name; }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string Description
		{
			get { return _description; }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public string CollectionLevelFile
		{
			get { return _collection; }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool ForceCreation
		{
			get { return !_update; }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool Merge
		{
			get { return _merge; }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public bool Remove
		{
			get { return !_noremove; }
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public IList<DocumentItem> Documents
		{
			get
			{
                return _documents.AsReadOnly();
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public IList<FilterItem> Filters
		{
			get
			{
                return _filters.AsReadOnly();
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public IList<PluginChildItem> Plugins
		{
			get
			{
                return _plugins.AsReadOnly();
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// 
        /// </value>
        public IList<string> ConnectedNamespaces
		{
			get { return _connections; }
		}

		#endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
		{
			return String.Format(CultureInfo.InvariantCulture, 
                "[Help 2.0 Namespace; {0}, {1}]", this._name, this._description);
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootNode"></param>
        private void Initialize(XPathNavigator rootNode)
		{
			// get all related documents
			XPathNodeIterator files = rootNode.SelectChildren("file", String.Empty);
			while (files.MoveNext())
			{
				_documents.Add(new DocumentItem(files.Current));
			}

			// get all related filters
			XPathNodeIterator filters = rootNode.SelectChildren("filter", String.Empty);
			while (filters.MoveNext())
			{
				_filters.Add(new FilterItem(filters.Current));
			}

			// get all related plugins
			XPathNodeIterator p = rootNode.SelectChildren("plugin", String.Empty);
			while (p.MoveNext())
			{
				XPathNodeIterator child = p.Current.SelectChildren("child", String.Empty);
				while (child.MoveNext())
				{
					_plugins.Add(new PluginChildItem(child.Current));
				}
			}
        }

        #endregion
    }
}
