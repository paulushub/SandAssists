//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
using System;
using System.IO;
using System.Xml;
using System.Globalization;
using System.Runtime.InteropServices;

using MSHelpServices;

namespace Sandcastle.HelpRegister
{
	public class HelpRegistrar : MarshalByRefObject, IDisposable
    {
        #region Private Fields

        private IHxPlugIn              _hxPlugins;
        private IHxFilters             _hxFilters;
        private IHxRegister            _hxRegister;
        private HxRegisterSessionClass _hxRegisterSession;

        #endregion

        #region Constructors and Destructor

        public HelpRegistrar()
		{
			try
			{
				_hxRegisterSession = new HxRegisterSessionClass();
				_hxRegisterSession.CreateTransaction("");

				_hxRegister = (IHxRegister)_hxRegisterSession.GetRegistrationObject(
                    HxRegisterSession_InterfaceType.HxRegisterSession_IHxRegister);
				_hxFilters  = (IHxFilters)_hxRegisterSession.GetRegistrationObject(
                    HxRegisterSession_InterfaceType.HxRegisterSession_IHxFilters);
				_hxPlugins  = (IHxPlugIn)_hxRegisterSession.GetRegistrationObject(
                    HxRegisterSession_InterfaceType.HxRegisterSession_IHxPlugIn);
			}
			catch (COMException)
			{
			}
		}

		~HelpRegistrar()
		{
			Dispose(false);
		}

		#endregion

        #region Public Methods

        #region Register/Unregister Methods

        public bool RegisterNamespace(string namespaceName, string collectionFile)
		{
			return this.RegisterNamespace(namespaceName, collectionFile, 
                String.Empty, true);
		}

		public bool RegisterNamespace(string namespaceName, string collectionFile, 
            string description)
		{
			return RegisterNamespace(namespaceName, collectionFile, 
                description, true);
		}
		
		public bool RegisterNamespace(string namespaceName, 
            string collectionFile, string description, bool overwrite)
		{
			if (_hxRegister == null || String.IsNullOrEmpty(namespaceName) 
                || String.IsNullOrEmpty(collectionFile))
			{
				return false;
			}
			try
			{
				// The default setting is to remove the namespace. But if you
				// just want to add some new help documents or filters, you
				// shouldn't remove it.
				if(overwrite && _hxRegister.IsNamespace(namespaceName))
				{
					_hxRegister.RemoveNamespace(namespaceName);
				}

				// If the namespace doesn't exist, create it
				if(!_hxRegister.IsNamespace(namespaceName))
				{
					_hxRegister.RegisterNamespace(namespaceName, 
                        collectionFile, description);
				}
				return true;
			}
			catch (COMException)
			{
                return false;
			}
		}

		public bool RemoveNamespace(string namespaceName)
		{
			if (_hxRegister == null || String.IsNullOrEmpty(namespaceName))
			{
				return false;
			}
			try
			{
				if(_hxRegister.IsNamespace(namespaceName))
				{
					_hxRegister.RemoveNamespace(namespaceName);
				}

				return true;
			}
			catch (COMException)
			{
                return false;
			}
		}

		public bool RegisterHelpFile(string namespaceName, string helpFileId, 
            int languageId, string hxsFile, string hxiFile, string hxqFile, 
            string hxrFile, int hxsMediaId,	int hxqMediaId, int hxrMediaId, 
            int sampleMediaId)
		{
			if (_hxRegister == null                 ||
			    String.IsNullOrEmpty(namespaceName) ||
			    String.IsNullOrEmpty(helpFileId)    ||
			    String.IsNullOrEmpty(hxsFile))
			{
				return false;
			}
			try
			{
				if(_hxRegister.IsNamespace(namespaceName))
				{
					_hxRegister.RegisterHelpFileSet(
                        namespaceName,  // Help 2.0 Collection Namespace
					    helpFileId,		// internal Help document ID
					    languageId,		// Language ID
					    hxsFile,		// Help document
					    hxiFile,		// external Help index
					    hxqFile,		// merged query file
					    hxrFile,		// combined attributes file
					    hxsMediaId,
					    hxqMediaId,
					    hxrMediaId,
					    sampleMediaId);

					// If you want to know something about those file types, I suggest 
                    // you take a look at Microsoft's VSHIK documentation.
				}

				return true;
			}
			catch (COMException)
			{
                return false;
			}
		}

		public bool RemoveHelpFile(string namespaceName, string helpFileId, 
            int languageId)
		{
			if (_hxRegister == null ||
			    String.IsNullOrEmpty(namespaceName) ||
			    String.IsNullOrEmpty(helpFileId))
			{
				return false;
			}
			try
			{
				if(_hxRegister.IsNamespace(namespaceName))
				{
					_hxRegister.RemoveHelpFile(namespaceName, helpFileId, languageId);
				}

				return true;
			}
			catch (COMException)
			{
			}
			return false;
		}

		public bool RegisterFilter(string namespaceName, string filterName, 
            string filterQuery)
		{
			if (_hxRegister == null || _hxFilters == null ||
			    String.IsNullOrEmpty(namespaceName) ||
			    String.IsNullOrEmpty(filterName))
			{
				return false;
			}
			try
			{
				_hxFilters.SetNamespace(namespaceName);
				_hxFilters.SetCollectionFiltersFlag(true);
				_hxFilters.RegisterFilter(filterName, filterQuery);
			}
			catch (COMException)
			{
			}
			
			// This function ALWAYS returns true. It's because an empty filter
			// query raises an exception but the filter will be created. A good
			// example is the known "(no filter)" filter.
			// So, don't change it.

			return true;
		}

		public bool RemoveFilter(string namespaceName, string filterName)
		{
			if (_hxRegister == null || _hxFilters == null ||
			    String.IsNullOrEmpty(namespaceName)       ||
			    String.IsNullOrEmpty(filterName))
			{
				return false;
			}
			try
			{
				_hxFilters.SetNamespace(namespaceName);
				_hxFilters.SetCollectionFiltersFlag(true);
				_hxFilters.RemoveFilter(filterName);

				return true;
			}
			catch (COMException)
			{
                return false;
			}
		}

		public bool RegisterPlugin(string parentNamespace, string childNamespace)
		{
			return PluginAction(parentNamespace, childNamespace, true);
		}

		public bool RemovePlugin(string parentNamespace, string childNamespace)
		{
			return PluginAction(parentNamespace, childNamespace, false);
		}

		#endregion

        #endregion

        #region Private Methods

        private static string GetXmlContent(string collectionFile, string xmlNode)
        {
            if (String.IsNullOrEmpty(collectionFile) ||
                String.IsNullOrEmpty(xmlNode))
            {
                return String.Empty;
            }
            try
            {
                XmlDocument xmldoc = new XmlDocument();
                xmldoc.Load(collectionFile);
                XmlNodeList n = xmldoc.SelectNodes
                    (String.Format(CultureInfo.InvariantCulture, "/HelpCollection/{0}/@File", xmlNode));

                if (n.Count > 0)
                {
                    return n.Item(0).InnerText;
                }
            }
            catch (NullReferenceException)
            {
            }

            return String.Empty;
        }

		private bool PluginAction(string parentNamespace, string childNamespace, bool registerIt)
		{
			if (_hxRegister == null || _hxPlugins == null ||
			    String.IsNullOrEmpty(parentNamespace) ||
			    String.IsNullOrEmpty(childNamespace) ||
			    !_hxRegister.IsNamespace(parentNamespace) ||
			    !_hxRegister.IsNamespace(childNamespace))
			{
				return false;
			}

			// if you want to remove a plug-in, at least it should be there
			if(!registerIt && !PluginSearch.PluginDoesExist(parentNamespace, 
                childNamespace))
			{
				return false;
			}

			try
			{
				// unregister plug-in
				if(!registerIt)
				{
					if(PluginSearch.PluginDoesExist(parentNamespace, childNamespace))
					{
						_hxPlugins.RemoveHelpPlugIn(parentNamespace, "", childNamespace, "", "");
						return true;
					}
				}

				// (re)register plug-in
				string path1     = String.Empty;

                string parentToc = String.Empty;
                string childToc  = String.Empty;
                string attr      = String.Empty;

				// The function requires the names of the TOC files. I can take them from
				// the collection level files (*.HxC) of the collections.
                string parentHxC = _hxRegister.GetCollection(parentNamespace);
                if (!String.IsNullOrEmpty(parentHxC) &&
                    String.Equals(Path.GetExtension(parentHxC), ".HxC",
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    parentToc = GetXmlContent(parentHxC, "TOCDef");
                }
                string childHxC = _hxRegister.GetCollection(childNamespace);
                if (!String.IsNullOrEmpty(childHxC) &&
                    String.Equals(Path.GetExtension(childHxC), ".HxC",
                    StringComparison.CurrentCultureIgnoreCase))
                {
                    childToc = GetXmlContent(childHxC, "TOCDef");
                    attr = GetXmlContent(childHxC, "AttributeDef");
                }

				if(!String.IsNullOrEmpty(attr))
				{
					path1 = Path.Combine(Path.GetDirectoryName(_hxRegister.GetCollection(childNamespace)), attr);
				}

                if (registerIt && !String.IsNullOrEmpty(parentToc) && !String.IsNullOrEmpty(childToc))
                {
                    _hxPlugins.RegisterHelpPlugIn(parentNamespace, parentToc, childNamespace, childToc, path1, 0);
                    return true;
                }
                else
                {
                    return false;
                }
			}
			catch (COMException)
			{
			}

			return false;
		}

        #endregion

        #region IDisposable Members

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (_hxRegisterSession != null)
				{
					// PLEASE DO NOT CHANGE OR REMOVE THAT!!!

					_hxRegisterSession.CommitTransaction();

					// It's very important to close the connection to the Help 2.0
					// environment. Trust me. I cannot explain because I don't know
					// anything about the Help 2.0 API. I was experimenting with the
					// Help 2.0 system and I knocked it out so many times ...

                    _hxRegisterSession = null; 
				}
			}
		}

        #endregion
    }
}
