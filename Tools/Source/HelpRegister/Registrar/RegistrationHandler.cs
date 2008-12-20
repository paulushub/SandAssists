//
// Help 2.0 Registration Utility
// Copyright (c) 2005 Mathias Simmack. All rights reserved.
//
using System;
using System.IO;
using System.Xml;
using System.Diagnostics;
using System.Globalization;

namespace Sandcastle.HelpRegister
{
	public sealed class RegistrationHandler : MarshalByRefObject
	{
		#region Private Fields

        private RegistrationOptions _regOptions;
        private IRegistrationHelper _regHelper;

        #endregion

        #region Constructors and Destructor

        public RegistrationHandler()
        {
        }

        #endregion

        #region Public Methods

        public int Run(RegistrationOptions options, IRegistrationHelper helper)
		{
            if (options == null || helper == null)
            {
                return -1;
            }

            _regOptions = options;
            _regHelper  = helper;

            RegistrationHelper.Instance.Add(_regHelper);

            if (options.IsViewer)
            {
                this.DoViewerStuff();

                return 0;
            }

            bool isErrorCodes = _regOptions.ErrorCodes;

            if (_regOptions.ShowLogo)
            {
                _regHelper.DisplayLogo();
            }
            if (_regOptions.ShowHelp || _regOptions.Count == 0)
            {
                _regHelper.DisplayHelp();

                return isErrorCodes ? 1 : -1;
            }

			if (!ApplicationHelpers.IsClassRegistered(
                "{31411198-A502-11D2-BBCA-00C04F8EC294}"))
			{
                if (!_regOptions.IsQuiet)
                    _regHelper.WriteLine(String.Format(ResourcesHelper.GetString(
                        "ErrorNoHelp2Environment")));

                return isErrorCodes ? 2 : -1;
			}

			if (!ApplicationHelpers.IsThisUserPrivileged())
			{
                if (!_regOptions.IsQuiet)
                    _regHelper.WriteLine(String.Format(ResourcesHelper.GetString(
                        "ErrorInvalidPrivileges")));

                return isErrorCodes ? 3 : -1;
			}

            string actionParam = _regOptions.ActionParam;

            if (actionParam != "/r" && actionParam != "/u" && actionParam != "+r" &&
                actionParam != "-r" && actionParam != "+p" && actionParam != "-p" &&
                actionParam != "/v" && actionParam != "-v")
			{
				if (!_regOptions.IsQuiet)
                    _regHelper.WriteLine(String.Format(ResourcesHelper.GetString(
                        "ErrorInvalidCommandLine"), actionParam));

                return isErrorCodes ? 4 : -1;
			}

			if (String.IsNullOrEmpty(_regOptions.FileName) || 
                !File.Exists(_regOptions.FileName))
			{
				if (!_regOptions.IsQuiet)
                    _regHelper.WriteLine(String.Format(ResourcesHelper.GetString(
                        "ErrorInvalidXmlFile"), _regOptions.FileName));

                return isErrorCodes ? 5 : -1;
			}

            XmlValidator schemaValidator = new XmlValidator(helper);

            if (!schemaValidator.SchemaExists)
			{
				if (!_regOptions.IsQuiet)
                    _regHelper.WriteLine(String.Format(ResourcesHelper.GetString(
                        "ErrorCannotValidateXmlFile"), _regOptions.FileName));

                return isErrorCodes ? 6 : -1;
			}

            if (!schemaValidator.Validate(_regOptions.FileName, _regOptions.IsQuiet))
			{
				// get a message from the validator class
                return isErrorCodes ? 6 : -1;
			}

            if (actionParam != "/v" && actionParam != "-v")
            {
                _regHelper.CloseViewers();
            }

			if (actionParam == "/r" || actionParam == "+r") 
                this.DoHelpStuff(true);
			if (actionParam == "/r" || actionParam == "+p") 
                this.DoPluginStuff(true);
			if (actionParam == "/u" || actionParam == "-p") 
                this.DoPluginStuff(false);
			if (actionParam == "/u" || actionParam == "-r") 
                this.DoHelpStuff(false);
            if (actionParam == "/v" || actionParam == "-v" ||
                _regOptions.ViewHelp)
                this.DoViewerStuff();

			return 0;
        }

        #endregion

        #region Private Methods

        #region Help Action Methods

        private void DoHelpStuff(bool register)
		{
			NamespaceItems namespaces = new NamespaceItems(_regOptions.FileName, 
                _regOptions.XPathSequence);

			if (!_regOptions.IsQuiet)
			{
				namespaces.RegisterOrRemoveNamespace += this.RegisterOrRemoveNamespace;
				namespaces.RegisterOrRemoveHelpDocument += this.RegisterOrRemoveHelpDocument;
				namespaces.RegisterOrRemoveFilter += this.RegisterOrRemoveFilter;
				namespaces.RegisterOrRemovePlugin += this.RegisterOrRemovePlugin;
				namespaces.NamespaceMerge += this.NamespaceMerge;
			}

			if (register)
			{
				namespaces.Register();
			}
			else
			{
				namespaces.Unregister();
			}
		}

		private void DoPluginStuff(bool register)
		{
            PluginItems plugins = new PluginItems(_regOptions.FileName, 
                _regOptions.XPathSequence);

			if (!_regOptions.IsQuiet)
			{
				plugins.RegisterOrRemovePlugin += this.RegisterOrRemovePlugin;
				plugins.NamespaceMerge += this.NamespaceMerge;
			}
			if (register)
			{
				plugins.Register();
			}
			else
			{
				plugins.Unregister();
			}
		}

        private void DoViewerStuff()
        {
            bool foundViewer     = false;
            string viewerPath    = null;
            string specialFolder = Environment.GetFolderPath(
                Environment.SpecialFolder.CommonProgramFiles);
            // VS.NET 2008: C:\Program Files\Common Files\Microsoft Shared\Help 9
            viewerPath = Path.Combine(specialFolder, @"Microsoft Shared\Help 9\dexplore.exe");
            foundViewer = File.Exists(viewerPath);
            if (foundViewer == false)
            {
                // VS.NET 2005: C:\Program Files\Common Files\Microsoft Shared\Help 8
                viewerPath = Path.Combine(specialFolder, @"Microsoft Shared\Help 8\dexplore.exe");
                foundViewer = File.Exists(viewerPath);

                // VS.NET 2003: C:\Program Files\Common Files\Microsoft Shared\Help
                if (foundViewer == false)
                {
                    viewerPath = Path.Combine(specialFolder, @"Microsoft Shared\Help\dexplore.exe");
                    foundViewer = File.Exists(viewerPath);
                }
            }
            if (foundViewer == false)
            {
                return;
            }

            int lcid             = 1033;
            string helpNamespace = String.Empty;
            string helpTitleId   = String.Empty;

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;

            bool readMore = true;

            using (XmlReader reader = XmlReader.Create(_regOptions.FileName, settings))
            {
                while (reader.Read() && readMore)
                {
                    if ((reader.NodeType == XmlNodeType.Element) && 
                        String.Equals(reader.Name, "namespace"))
                    {
                        helpNamespace = reader.GetAttribute("name");
                        while (reader.Read())
                        {
                            if ((reader.NodeType == XmlNodeType.Element) &&
                                String.Equals(reader.Name, "file"))
                            {
                                helpTitleId = reader.GetAttribute("Id");
                                string temp = reader.GetAttribute("LangId");
                                if (!String.IsNullOrEmpty(temp))
                                {
                                    lcid = Convert.ToInt32(temp);
                                }
                                break;
                            }
                        }

                        readMore = false;
                    }
                }
            }

            if (String.IsNullOrEmpty(helpNamespace))
            {
                return;
            }

            // dexplore.exe /helpcol ms-help://<namespace>/<title ID> /LCID <locale ID> /LaunchNamedUrlTopic DefaultPage
            string runOptions = String.Format(
                " /helpcol \"ms-help://{0}/{1}\" /LCID {2} /LaunchNamedUrlTopic DefaultPage",
                helpNamespace, helpTitleId, lcid);

            Process process = Process.Start(viewerPath, runOptions);
        }

		#endregion

		#region Event Handlers

		private void RegisterOrRemoveNamespace(object sender, NamespaceEventArgs e)
		{
			if (e.Register)
			{
                _regHelper.WriteLine(String.Format(ResourcesHelper.GetString(
                    "RegisterHelpNamespace"), e.Name));
			}
			else
			{
                _regHelper.WriteLine(String.Format(ResourcesHelper.GetString(
                    "RemoveHelpNamespace"), e.Name));
			}
		}

		private void RegisterOrRemoveHelpDocument(object sender, NamespaceEventArgs e)
		{
			if (e.Register)
			{
                _regHelper.WriteLine(String.Format(ResourcesHelper.GetString(
                    "RegisterHelpDocument"), e.Name));
			}
			else
			{
                _regHelper.WriteLine(String.Format(ResourcesHelper.GetString(
                    "RemoveHelpDocument"), e.Name));
			}
		}

		private void RegisterOrRemoveFilter(object sender, NamespaceEventArgs e)
		{
			if (e.Register)
			{
                _regHelper.WriteLine(String.Format(ResourcesHelper.GetString(
                    "RegisterHelpFilter"), e.Name));
			}
			else
			{
                _regHelper.WriteLine(String.Format(ResourcesHelper.GetString(
                    "RemoveHelpFilter"), e.Name));
			}
		}

		private void RegisterOrRemovePlugin(object sender, PluginEventArgs e)
		{
			if (e.Register)
			{
                _regHelper.WriteLine(String.Format(ResourcesHelper.GetString(
                    "RegisterHelpPlugin"), e.Child, e.Parent));
			}
			else
			{
                _regHelper.WriteLine(String.Format(ResourcesHelper.GetString(
                    "RemoveHelpPlugin"), e.Child, e.Parent));
			}
		}

		private void NamespaceMerge(object sender, MergingEventArgs e)
		{
            _regHelper.WriteLine(String.Format(ResourcesHelper.GetString(
                "MergeHelpNamespace"), e.Name));
		}

		#endregion

        #endregion
    }
}
