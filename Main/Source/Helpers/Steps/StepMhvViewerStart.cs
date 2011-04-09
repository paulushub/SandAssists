using System;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Globalization;
using System.Collections.Generic;

using Microsoft.Win32;

using Sandcastle.Utilities;

namespace Sandcastle.Steps
{
    /// <summary>
    /// This step will validate the Microsoft Help Viewer 1.0 installations,
    /// uninstall the current help book, if any, install the help book and start
    /// a browser to view it.
    /// </summary>
    public sealed class StepMhvViewerStart : BuildStep
    {
        #region Private Fields

        private const string HelpLibAgent   = "HelpLibAgent.exe";
        private const string HelpLibManager = "HelpLibManager.exe";

        private const string InstalledRegistryKey = @"SOFTWARE\Microsoft\Help\v1.0";

        private string _catalogLocale;
        private string _catalogVersion;
        private string _catalogProductId;

        private string _helpPath;
        private string _helpSetup;

        private string _helpRoot;
        private string _helpLocalStore;

        private string _helpLibAgentPath;
        private string _helpLibManagerPath;

        private IList<CatalogInfo> _registryCatalogs;
        private IList<CatalogInfo> _manifestCatalogs;

        #endregion

        #region Constructors and Destructor

        public StepMhvViewerStart()
        {
            this.ConstructorDefaults();
        }

        public StepMhvViewerStart(string workingDir, string helpPath,
            string helpSetup) : base(workingDir)
        {
            this.ConstructorDefaults();

            _helpPath  = helpPath;
            _helpSetup = helpSetup;
        }

        private void ConstructorDefaults()
        {
            _catalogLocale       = "en-us";
            _catalogVersion      = "100";
            _catalogProductId    = "VS";

            this.LogTitle        = "Opening Help 3.x (Help Viewer) page for viewing.";
            this.ContinueOnError = true;
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        protected override bool OnExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            // 1. Perform the initial sanity checks...
            if (String.IsNullOrEmpty(_helpPath) || String.IsNullOrEmpty(_helpSetup) 
                || !File.Exists(_helpPath) || !File.Exists(_helpSetup))
            {
                if (logger != null)
                {
                    logger.WriteLine("The Microsoft Help Viewer help or setup file path is not provided or does not exist.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            // 2. Validate the Microsoft Help Viewer installation...
            this.ValidViewerInstallation(context);

            _helpLibAgentPath = GetAgentLocation();
            if (!File.Exists(_helpLibAgentPath))
            {   
                if (logger != null)
                {
                    logger.WriteLine("The installed location of the Microsoft Help Viewer cannot be found.", 
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            string viewerDir    = Path.GetDirectoryName(_helpLibAgentPath);
            _helpLibManagerPath = Path.Combine(viewerDir, "HelpLibManager.exe");
            if (!File.Exists(_helpLibManagerPath))
            {   
                if (logger != null)
                {
                    logger.WriteLine("The installed location of the Microsoft Help Viewer cannot be found.", 
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            if (ProcessRunner.IsProcessOpen("HelpLibManager"))
            {                
                if (logger != null)
                {
                    logger.WriteLine("The Microsoft Help Library Manager tool is currently running. The help cannot be installed.", 
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            // Start the Microsoft Help Library Agent process.
            // This will ensure that it is ready and listening by the time we
            // opening the help page...
            StartHelpAgent(context);
                         
            if (!this.UninstallHelpFile(context))
            {
                return false;
            }
            if (!this.InstallHelpFile(context))
            {
                return false;
            }

            return OpenHelpFile(context);
        }

        #endregion

        #region Private Methods

        #region ValidViewerInstallation Method

        /// <summary>
        /// This will query the Windows registry for information on the installed
        /// Microsoft Help Viewer, logging success and failure.
        /// </summary>
        /// <param name="context">The context of the current build.</param>
        /// <returns>
        /// This returns <see langword="true"/>, an installation is found and it
        /// is valid; otherwise, it returns <see langword="false"/>.
        /// </returns>
        private bool ValidViewerInstallation(BuildContext context)
        {
            _helpRoot       = null;
            _helpLocalStore = null;
            _registryCatalogs   = null;

            bool isValid = false;
            BuildLogger logger = context.Logger;
            if (logger != null)
            {
                logger.WriteLine("Validating the Microsoft Help Viewer 1.0 installation.",
                    BuildLoggerLevel.Info);
            }

            // We are going to walk the registry and search for the installation
            // information required to validate...
            try
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(
                    InstalledRegistryKey, false))
                {
                    if (key != null)
                    {
                        string appRoot = (string)key.GetValue("AppRoot", String.Empty);
                        if (!String.IsNullOrEmpty(appRoot))
                        {
                            isValid = true;

                            if (logger != null)
                            {
                                logger.WriteLine("Installation found at: " + appRoot,
                                    BuildLoggerLevel.Info);
                            }

                            _helpRoot = appRoot;
                        }
                        string localStore = (string)key.GetValue("LocalStore", String.Empty);
                        if (!String.IsNullOrEmpty(localStore))
                        {
                            if (logger != null)
                            {
                                logger.WriteLine("Local Store found at: " + localStore,
                                    BuildLoggerLevel.Info);
                            }

                            if (!Directory.Exists(localStore))
                            {
                                if (logger != null)
                                {
                                    logger.WriteLine("The catalog is not yet initialized. Please run the Microsoft Help Library Manager tool.",
                                        BuildLoggerLevel.Error);
                                }

                                isValid = false;
                            }

                            _helpLocalStore = localStore;
                        }
                        else 
                        {
                            isValid = false;
                        }

                        // If we are valid at this point then we go further to 
                        // look for the default catalog information.
                        if (isValid)
                        {
                            isValid = this.FindCatalogInfo(key, context);
                        }
                    }
                    else
                    {
                        if (logger != null)
                        {
                            isValid = false;
                            logger.WriteLine("The Microsoft Help Viewer 1.0 is not installed.",
                                BuildLoggerLevel.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                isValid = false;

                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }
            }

            if (logger != null)
            {
                if (!isValid)
                {
                    logger.WriteLine("The validation failed.",
                        BuildLoggerLevel.Error);
                }
                else
                {
                    logger.WriteLine("The validation is successful.",
                        BuildLoggerLevel.Info);
                }
            }

            return isValid;
        }

        /// <summary>
        /// This searches and creates a list of the installed catalogs.
        /// </summary>
        /// <param name="key">The entry or main help registry key.</param>
        /// <param name="context">The context of the build system.</param>
        /// <returns>
        /// This returns <see langword="true"/>, if at least a valid catalog is
        /// found; otherwise, it returns <see langword="false"/>.
        /// </returns>
        private bool FindCatalogInfo(RegistryKey key, BuildContext context)
        {
            bool isValid = false;

            List<CatalogInfo> registryCatalog = new List<CatalogInfo>();

            BuildLogger logger = context.Logger;

            using (RegistryKey catalogInfoKey = key.OpenSubKey("CatalogInfo", false))
            {
                if (catalogInfoKey != null)
                {
                    isValid = true;

                    string[] valueNames = catalogInfoKey.GetValueNames();
                    if (valueNames != null && valueNames.Length != 0)
                    {
                        for (int i = 0; i < valueNames.Length; i++)
                        {
                            string itemName = valueNames[i];
                            string keyValue = (string)catalogInfoKey.GetValue(
                                itemName, String.Empty);
                            if (String.IsNullOrEmpty(keyValue))
                            {
                                continue;
                            }

                            // The format is vs_100_en-us (for English store).
                            string[] itemValue = itemName.Split('_');
                            if (itemValue != null && itemValue.Length == 3)
                            {
                                string location = (string)catalogInfoKey.GetValue(
                                    itemName, String.Empty);

                                if (!String.IsNullOrEmpty(location) && 
                                    File.Exists(location))
                                {
                                    CatalogInfo catalogInfo = new CatalogInfo(
                                        itemValue[0], itemValue[1], itemValue[2],
                                        location);
                                    
                                    if (catalogInfo.IsValid)
                                    {
                                        registryCatalog.Add(catalogInfo);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (logger != null)
                    {
                        logger.WriteLine("No installed catalog information is found. Please run the Microsoft Help Library Manager tool.",
                            BuildLoggerLevel.Error);
                    }

                    isValid = false;
                }
            }

            if (isValid)
            {
                isValid = (registryCatalog != null && registryCatalog.Count != 0);
            }

            _registryCatalogs = registryCatalog;

            return isValid;
        }

        #endregion

        #region SelectInstalledLocale Method

        /// <summary>
        /// This selects a matching locale, if available, or a default locale
        /// for the installation of the help file.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool SelectInstalledLocale(BuildContext context)
        {
            // This should not happen after the validation, we still check...
            if (_registryCatalogs == null || _registryCatalogs.Count == 0)
            {
                return false;
            }
            BuildSettings settings = context.Settings;
            CultureInfo culture    = settings.CultureInfo;
            string helpLocale      = culture.Name.ToLower();
            string defaultLocale   = String.Empty;

            if (_registryCatalogs.Count == 1)  // this is the usual case...
            {
                CatalogInfo catalog = _registryCatalogs[0];

                defaultLocale = catalog.Locale;

                if (String.Equals(catalog.Locale, helpLocale, 
                    StringComparison.OrdinalIgnoreCase))
                {
                    _catalogLocale = catalog.Locale;

                    return true;
                }
            }
            else 
            {
                // We use the first as the default, installation.
                // Currently, only one locale is listed in the registry...
                defaultLocale = _registryCatalogs[0].Locale;

                // normally we should not have more than one registry catalog entry...
                for (int i = 0; i < _registryCatalogs.Count; i++)
                {
                    CatalogInfo catalog = _registryCatalogs[i];

                    if (String.Equals(catalog.Locale, helpLocale,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _catalogLocale = catalog.Locale;

                        return true;
                    }
                }
            }

            // Check for multiple locale help stores, only the default is 
            // currently listed in the registry...
            if (LoadInstalledCatalogs(context) &&
                (_manifestCatalogs != null && _manifestCatalogs.Count != 0))
            {
                for (int i = 0; i < _manifestCatalogs.Count; i++)
                {
                    CatalogInfo catalog = _manifestCatalogs[i];

                    if (String.Equals(catalog.Locale, helpLocale,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        _catalogLocale = catalog.Locale;

                        return true;
                    }
                }
            }

            if (!String.IsNullOrEmpty(defaultLocale))
            {
                _catalogLocale = defaultLocale;

                return true;
            }

            return false;
        }

        private bool LoadInstalledCatalogs(BuildContext context)
        {
            if (String.IsNullOrEmpty(_helpLocalStore) || 
                !Directory.Exists(_helpLocalStore))
            {
                return false;
            }

            List<CatalogInfo> manifestCatalogs = new List<CatalogInfo>();

            string manifestPath = Path.Combine(_helpLocalStore, "manifest");
            if (!Directory.Exists(manifestPath))
            {
                return false;
            }
            string[] manifestFiles = Directory.GetFiles(manifestPath,
                "queryManifest.*.xml", SearchOption.TopDirectoryOnly);
            if (manifestFiles == null || manifestFiles.Length == 0)
            {
                return false;
            }

            for (int i = 0; i < manifestFiles.Length; i++)
            {
                XPathDocument document = new XPathDocument(manifestFiles[i]);
                XPathNavigator xpathNavigator = document.CreateNavigator();

                XPathNodeIterator iterator = xpathNavigator.Select(
                    "/queryManifest/catalogs/catalog");

                if (iterator != null && iterator.Count != 0)
                {
                    foreach (XPathNavigator navigator in iterator)
                    {
                        XPathNavigator pathNode = navigator.SelectSingleNode("./catalogPath");

                        if (pathNode != null && Directory.Exists(pathNode.Value))
                        {
                            CatalogInfo catalogInfo = new CatalogInfo(
                                navigator.GetAttribute("productId", String.Empty),
                                navigator.GetAttribute("productVersion", String.Empty),
                                navigator.GetAttribute("productLocale", String.Empty),
                                pathNode.Value);

                            if (catalogInfo.IsValid)
                            {
                                manifestCatalogs.Add(catalogInfo);
                            }
                        }
                    }
                }
            }

            if (manifestCatalogs.Count != 0)
            {
                _manifestCatalogs = manifestCatalogs;

                return true;
            }

            return false;
        }

        #endregion

        #region UninstallHelpFile Method

        private bool UninstallHelpFile(BuildContext context)
        {
            BuildLogger logger     = context.Logger;
            BuildSettings settings = context.Settings;
            BuildFeedback feedback = settings.Feedback;

            string uninstallFormat =
                "/product {0} /version {1} /locale {2} /uninstall /silent /vendor \"{3}\" /mediaBookList \"{4}\" /productName \"{5}\"";

            try
            {
                string companyName = feedback.Company;
                string productName = feedback.Product;
                string bookName    = settings.HelpTitle;

                if (logger != null)
                {
                    logger.WriteLine(String.Format(
                        "Uninstalling: Company={0}, Product={1}, Book={2}",
                        companyName, productName, bookName), 
                        BuildLoggerLevel.Info);
                }

                string uninstallArguments = String.Format(uninstallFormat,
                    _catalogProductId, _catalogVersion, _catalogLocale, companyName, bookName, productName);

                Process proc = new Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName  = _helpLibManagerPath;
                proc.StartInfo.Arguments = uninstallArguments;
                if (Environment.OSVersion.Version.Major >= 6) // Vista/Window7
                {
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.Verb            = "runas";
                }
                proc.Start();
                proc.WaitForExit();

                int exitCode = proc.ExitCode;

                proc.Close();

                if (exitCode == 0 || exitCode == 200)
                {
                    if (logger != null)
                    {
                        if (exitCode == 0)
                        {
                            logger.WriteLine("The uninstallation is successful.",
                                BuildLoggerLevel.Info);
                        }
                        else if (exitCode == 200)
                        {
                            logger.WriteLine("The specified book is not currently installed.",
                                BuildLoggerLevel.Info);
                        }
                    }

                    return true;
                }

                ReportUninstallAgentError(exitCode);

                return false;
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }

                return false;
            }
        }

        #endregion

        #region InstallHelpFile Method

        private bool InstallHelpFile(BuildContext context)
        {
            BuildLogger logger     = context.Logger;
            BuildSettings settings = context.Settings;
            BuildFeedback feedback = settings.Feedback;
            // The installation command line options...
            string installFormat =
                "/product {0} /version {1} /locale {2} /sourceMedia \"{3}\"";

            try
            {
                string companyName = feedback.Company;
                string productName = feedback.Product;
                string bookName    = settings.HelpTitle;

                if (logger != null)
                {
                    logger.WriteLine(String.Format(
                        "Installing: Company={0}, Product={1}, Book={2}",
                        companyName, productName, bookName),
                        BuildLoggerLevel.Info);
                }

                string installArguments = String.Format(installFormat,
                    _catalogProductId, _catalogVersion, _catalogLocale, 
                    _helpSetup);

                Process proc = new Process();
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName  = _helpLibManagerPath;
                proc.StartInfo.Arguments = installArguments;
                if (Environment.OSVersion.Version.Major >= 6) // Vista/Window7
                {
                    proc.StartInfo.UseShellExecute = true;
                    proc.StartInfo.Verb            = "runas";
                }
                proc.Start();
                proc.WaitForExit();

                int exitCode = proc.ExitCode;

                proc.Close();

                if (exitCode != 0)
                {
                    ReportInstallAgentError(exitCode);

                    return false;
                }
                else
                {
                    // Verify the successful installation.
                    // Even with an indication of success, the Help Library 
                    // Manager process might have being cancelled...
                    if (this.IsHelpLocaleBookInstalled(context))
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("The installation was successful.",
                                BuildLoggerLevel.Info);
                        }

                        return true;
                    }
                    else
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("The installation failed. The installation is probably cancelled.",
                                BuildLoggerLevel.Error);
                        }

                        return false;
                    }  
                }
            }
            catch (Exception ex)
            {
                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }

                return false;
            }
        }

        #endregion

        #region IsHelpLocaleBookInstalled Methods

        private bool IsHelpLocaleBookInstalled()
        {
            if (String.IsNullOrEmpty(_helpSetup) || 
                !File.Exists(_helpSetup))
            {
                return false;
            }

            string companyName = String.Empty;
            string productName = String.Empty;
            string bookName    = String.Empty;
            string locale      = String.Empty;
            string fileName    = String.Empty;  // only one package is supported...
            string filePrefix  = String.Empty;  // only one package is supported...

            XmlReaderSettings xmlSettings = new XmlReaderSettings();
            xmlSettings.IgnoreComments               = true;
            xmlSettings.IgnoreProcessingInstructions = true;
            xmlSettings.XmlResolver                  = null;

            using (XmlReader reader = XmlReader.Create(_helpSetup, xmlSettings))
            {
                bool foundIt = false;
                XmlNodeType nodeType = XmlNodeType.None;

                while (reader.Read() && !foundIt)
                {
                    nodeType = reader.NodeType;

                    if (nodeType == XmlNodeType.Element &&
                        String.Equals(reader.Name, "div",
                        StringComparison.OrdinalIgnoreCase) &&
                        String.Equals(reader.GetAttribute("class"), "details",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        foundIt = true;

                        while (reader.Read())
                        {
                            nodeType = reader.NodeType;

                            if (nodeType == XmlNodeType.Element &&
                                String.Equals(reader.Name, "span",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                switch (reader.GetAttribute("class").ToLower())
                                {
                                    case "vendor":
                                        companyName = reader.ReadString();
                                        break;
                                    case "locale":
                                        locale = reader.ReadString();
                                        break;
                                    case "product":
                                        productName = reader.ReadString();
                                        break;
                                    case "name":
                                        bookName = reader.ReadString();
                                        break;
                                }
                            }
                            else if (nodeType == XmlNodeType.EndElement &&
                                String.Equals(reader.Name, "div",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                break;
                            }
                        }
                    }
                    else if (nodeType == XmlNodeType.Element &&
                            String.Equals(reader.Name, "div",
                            StringComparison.OrdinalIgnoreCase) &&
                            String.Equals(reader.GetAttribute("class"), "package",
                            StringComparison.OrdinalIgnoreCase))
                    {
                        while (reader.Read())
                        {
                            nodeType = reader.NodeType;

                            if (nodeType == XmlNodeType.Element &&
                                String.Equals(reader.Name, "span",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                filePrefix = reader.ReadString();
                            }
                            else if (nodeType == XmlNodeType.Element &&
                                String.Equals(reader.Name, "a",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                fileName = reader.ReadString();
                            }
                            else if (nodeType == XmlNodeType.EndElement &&
                                String.Equals(reader.Name, "div",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                break;
                            }
                        }
                    }

                }
            }

            return this.IsHelpLocaleBookInstalled(companyName, productName,
                bookName, locale, fileName);
        }

        private bool IsHelpLocaleBookInstalled(BuildContext context)
        {
            BuildSettings settings = context.Settings;
            BuildFeedback feedback = settings.Feedback;

            string companyName = feedback.Company;
            string productName = feedback.Product;
            string bookName    = settings.HelpTitle;
            string locale      = settings.CultureInfo.Name.ToLower();
            string fileName    = Path.GetFileName(_helpPath);

            return this.IsHelpLocaleBookInstalled(companyName, productName,
                bookName, locale, fileName);
        }

        private bool IsHelpLocaleBookInstalled(string company, string product, 
            string book, string locale, string fileName)
        {                                    
            if (String.IsNullOrEmpty(company) || String.IsNullOrEmpty(product) ||
                String.IsNullOrEmpty(book)    || String.IsNullOrEmpty(locale))
            {
                return false;
            }
            if (String.IsNullOrEmpty(_helpLocalStore) || 
                !Directory.Exists(_helpLocalStore))
            {
                return false;
            }

            // 1. Examine the cache settings for the vendor...
            string cldSettings = Path.Combine(_helpLocalStore, "cld-settings.xml");
            if (!File.Exists(cldSettings))
            {
                return false;
            }
            bool isVendorCached = false;
            using (XmlReader reader = XmlReader.Create(cldSettings))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element &&
                        String.Equals(reader.Name, "vendor", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        if (String.Equals(reader.GetAttribute("name"), company,
                            StringComparison.OrdinalIgnoreCase))
                        {
                            isVendorCached = true;
                            break;
                        }
                    }
                }
            }
            if (!isVendorCached)
            {
                return false;
            }

            // 2. Examine the vendor directory...
            string vendorDir = Path.Combine(_helpLocalStore, @"content\" + company);
            if (!Directory.Exists(vendorDir))
            {
                return false;
            }

            // 3. Query the manifest file for the specified book...
            string manifestPath = Path.Combine(_helpLocalStore, "manifest");
            if (!Directory.Exists(manifestPath))
            {
                return false;
            }
            string[] manifestFiles = Directory.GetFiles(manifestPath,
                "queryManifest.*.xml", SearchOption.TopDirectoryOnly);
            if (manifestFiles == null || manifestFiles.Length == 0)
            {
                return false;
            }

            bool isInstalled = false;

            for (int i = 0; i < manifestFiles.Length && !isInstalled; i++)
            {
                XPathDocument document = new XPathDocument(manifestFiles[i]);
                XPathNavigator xpathNavigator = document.CreateNavigator();

                XPathNodeIterator iterator = xpathNavigator.Select(
                    "/queryManifest/catalogs/catalog");

                if (iterator != null && iterator.Count != 0 && !isInstalled)
                {
                    foreach (XPathNavigator navigator in iterator)
                    {
                        // Apply the locale filter...
                        if (!String.Equals(navigator.GetAttribute("productLocale", String.Empty),
                            locale, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        string sourceFile = "./catalogSources/catalogSource/sourceFiles/sourceFile[@vendorPath='" + company + @"\store" + "']";
                        XPathNavigator sourceNode = navigator.SelectSingleNode(sourceFile);

                        if (sourceNode != null)
                        {
                            string indexFileName = sourceNode.GetAttribute(
                                "fileName", String.Empty);

                            XPathNavigator contentNode = sourceNode.SelectSingleNode(
                                "./contentFiles/contentFile[@vendorPath='" + company + @"\store" + "']");

                            if (contentNode != null)
                            {
                                string contentFileName = contentNode.GetAttribute(
                                    "fileName", String.Empty);

                                if (String.Equals(contentFileName, fileName,
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    isInstalled = true;
                                    break;
                                }
                            }
                        }
                    }
                }
            }


            return isInstalled;
        }

        #endregion

        #region OpenHelpFile Method

        private bool OpenHelpFile(BuildContext context)
        {
            // 1. Start the Microsoft Help Library Agent process...
            StartHelpAgent(context);

            // 2. Wait for the help library service to be available...
            int waitCount = 0;
            while (!ProcessRunner.IsProcessOpen("HelpLibAgent"))
            {
                // We wait for a max of 5 times, should be enough on even slow
                // systems...
                if (waitCount == 5) 
                {
                    break;
                }

                waitCount++;
                Thread.Sleep(100);
            }

            BuildLogger logger = context.Logger;
            if (logger != null)
            {
                logger.WriteLine("Opening: " + _helpPath,
                    BuildLoggerLevel.Info);
            }

            string helpUrlFormat =
                "ms-xhelp:///?method=page&id={0}&product={1}&productversion={2}&locale={3}";

            string helpUrl     = null;
            // 3. The startup help ID will normally be saved in the context, get it...
            string helpStartId = context["$HelpTocRoot"];
            string tempText    = context["$HelpHierarchicalToc"];

            if (!String.IsNullOrEmpty(tempText) && String.Equals(tempText,
                Boolean.TrueString, StringComparison.OrdinalIgnoreCase))
            {
                helpStartId = context["$HelpHierarchicalTocRoot"];
            }

            if (String.IsNullOrEmpty(helpStartId))
            {
                helpUrl = String.Format(
                    "ms-xhelp:///?method=page&id=-1&format=html&product={0}&productVersion={1}",
                    _catalogProductId, _catalogVersion);
            }
            else
            {
                helpUrl = String.Format(helpUrlFormat,
                    helpStartId, _catalogProductId, _catalogVersion, _catalogLocale);                
            }

            try
            {
                // 4. Request the Microsoft Help Library Agent to open the page...
                Process startHelp = Process.Start(helpUrl);
                // The return could be null, if no process resource is started 
                // (for example, if an existing process is reused as in browsers).
                if (startHelp != null) 
                {
                    startHelp.Close();
                }

                return true;
            }
            catch (Exception ex)
            {   
                if (logger != null)
                {
                    logger.WriteLine(ex, BuildLoggerLevel.Error);
                }

                return false;
            }
        }

        #endregion

        #region Help Library Agent Methods

        private string GetAgentLocation()
        {
            string agentPath = String.Empty;

            // 1. Retrieve it from the validation information...
            if (!String.IsNullOrEmpty(_helpRoot) && Directory.Exists(_helpRoot))
            {
                agentPath = Path.Combine(_helpRoot, HelpLibAgent);
                if (File.Exists(agentPath))
                {
                    return agentPath;
                }

                agentPath = String.Empty;
            }

            // 2. Retrieve it from the protocol...
            using (RegistryKey key = Registry.ClassesRoot.OpenSubKey(
                @"MS-XHelp\shell\open\command", false))
            {
                // Trim off quotes
                if (key != null)
                {   
                    agentPath = key.GetValue(null).ToString().ToLower().Replace("\"", "");
                    if (!String.IsNullOrEmpty(agentPath) && !agentPath.EndsWith("exe"))
                    {
                        //get rid of everything after the ".exe"
                        agentPath = agentPath.Substring(0, 
                            agentPath.LastIndexOf(".exe") + 4);
                    }
                }
            }

            // 3. Retrieve it from the usual install directory...
            if (String.IsNullOrEmpty(agentPath) || !File.Exists(agentPath))
            {
                string directPath =
                    @"%ProgramFiles%\Microsoft Help Viewer\v1.0\HelpLibAgent.exe";
                directPath = Environment.ExpandEnvironmentVariables(directPath);

                agentPath = Path.GetFullPath(directPath);
            }

            return agentPath;
        }

        private void StartHelpAgent(BuildContext context)
        {
            if (ProcessRunner.IsProcessOpen("HelpLibAgent"))
            {
                return;
            }

            BuildLogger logger = context.Logger;
            if (logger != null)
            {
                logger.WriteLine("Starting: Microsoft Help Library Agent",
                    BuildLoggerLevel.Info);
            }

            using (Process proc = new Process())
            {
                proc.EnableRaisingEvents = false;
                proc.StartInfo.FileName = GetAgentLocation();
                proc.Start();
            }
        }

        #endregion

        #region ReportAgentError Method

        private void ReportInstallAgentError(int exitCode)
        {
            BuildContext context = this.Context;
            if (context == null)
            {
                return;
            }
            BuildLogger logger = context.Logger;
            if (logger == null)
            {
                return;
            }

            switch (exitCode)
            {
                case 0:
                    logger.WriteLine("The operation was successful.",
                        BuildLoggerLevel.Info);
                    break;
                case 100:
                    logger.WriteLine("One or more command line arguments was missing or invalid.",
                        BuildLoggerLevel.Error);
                    break;
                case 110:
                    logger.WriteLine("The application configuration file for HLM was missing or invalid",
                            BuildLoggerLevel.Error);
                    break;
                case 120:
                    logger.WriteLine("The help content store could not be locked for update. This error typically occurs when the content is locked for update by another process.",
                            BuildLoggerLevel.Error);
                    break;
                case 130:
                    logger.WriteLine("Files required to install content for a new product were not found.",
                            BuildLoggerLevel.Error);
                    break;
                case 131:
                    logger.WriteLine("Files required to install content for a new product were invalid.",
                            BuildLoggerLevel.Error);
                    break;
                case 140:
                    logger.WriteLine("The path specified for the /content switch is invalid.",
                            BuildLoggerLevel.Error);
                    break;
                case 150:
                    logger.WriteLine("The local content store is in an invalid state. This error occurs when the directory permissions do not allow writing, or a required file is missing from the directory.",
                        BuildLoggerLevel.Error);
                    break;
                case 200:
                    logger.WriteLine("The arguments passed to HLM did not result in content being installed. This can occur when the content is already installed.",
                        BuildLoggerLevel.Error);
                    break;
                case 400:
                    logger.WriteLine("The removal of content failed. Detailed information can be found in the event log and in the installation log.",
                        BuildLoggerLevel.Error);
                    break;
                case 401:
                    logger.WriteLine("The installation of content failed. Detailed information can be found in the event log and in the installation log.",
                        BuildLoggerLevel.Error);
                    break;
                default:
                    logger.WriteLine("Unknown error.",
                        BuildLoggerLevel.Error);
                    break;
            }
        }

        private void ReportUninstallAgentError(int exitCode)
        {
            BuildContext context = this.Context;
            if (context == null)
            {
                return;
            }   
            BuildLogger logger = context.Logger;
            if (logger == null)
            {
                return;
            }

            switch (exitCode)
            {
                case 0:
                    logger.WriteLine("The operation was successful.",
                        BuildLoggerLevel.Info);
                    break;
                case 100:
                    logger.WriteLine("One or more command line arguments was missing or invalid.",
                        BuildLoggerLevel.Error);
                    break;
                case 110:
                    logger.WriteLine("The application configuration file for HLM was missing or invalid",
                            BuildLoggerLevel.Error);
                    break;
                case 120:
                    logger.WriteLine("The help content store could not be locked for update. This error typically occurs when the content is locked for update by another process.",
                            BuildLoggerLevel.Error);
                    break;
                case 130:
                    logger.WriteLine("Files required to install content for a new product were not found.",
                            BuildLoggerLevel.Error);
                    break;
                case 131:
                    logger.WriteLine("Files required to install content for a new product were invalid.",
                            BuildLoggerLevel.Error);
                    break;
                case 140:
                    logger.WriteLine("The path specified for the /content switch is invalid.",
                            BuildLoggerLevel.Error);
                    break;
                case 150:
                    logger.WriteLine("The local content store is in an invalid state. This error occurs when the directory permissions do not allow writing, or a required file is missing from the directory.",
                        BuildLoggerLevel.Error);
                    break;
                case 200:
                    logger.WriteLine("The arguments passed to HLM did not result in content being installed. This can occur when the content is already installed.",
                        BuildLoggerLevel.Error);
                    break;
                case 400:
                    logger.WriteLine("The removal of content failed. Detailed information can be found in the event log and in the installation log.",
                        BuildLoggerLevel.Error);
                    break;
                case 401:
                    logger.WriteLine("The installation of content failed. Detailed information can be found in the event log and in the installation log.",
                        BuildLoggerLevel.Error);
                    break;
                default:
                    logger.WriteLine("Unknown error.",
                        BuildLoggerLevel.Error);
                    break;
            }
        }

        #endregion

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion

        #region CatalogInfo Class

        /// <summary>
        /// The contains the installed catalog information retrieved from the
        /// registry or the manifest file.
        /// </summary>
        private sealed class CatalogInfo
        {
            #region Private Fields

            private string _product;
            private string _version;
            private string _locale;
            private string _location;

            #endregion

            #region Constructors and Destructor

            public CatalogInfo(string product, string version, 
                string locale, string location)
            {
                if (product == null)
                {
                    product = String.Empty;
                }
                if (locale == null)
                {
                    locale = String.Empty;
                }

                _product  = product.ToUpper();
                _version  = version;
                _locale   = locale.ToLower();
                _location = location;
            }

            #endregion

            #region Public Properties

            public bool IsValid
            {
                get
                {
                    if (String.IsNullOrEmpty(_product) ||
                        String.IsNullOrEmpty(_version) ||
                        String.IsNullOrEmpty(_locale)  ||
                        String.IsNullOrEmpty(_location))
                    {
                        return false;
                    }
                    if (!String.Equals(_product, "VS", 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                    int verNumber;
                    if (!Int32.TryParse(_version, out verNumber) ||
                        verNumber != 100)
                    {
                        return false;
                    }
                    try
                    {
                        CultureInfo culture = CultureInfo.GetCultureInfo(_locale);
                        if (culture == null)
                        {
                            return false;
                        }
                    }
                    catch
                    {
                        return false;
                    }
                    if (!File.Exists(_location)) // mostly a cab file
                    {                              
                        // may also point to a directory...
                        if (!Directory.Exists(_location))
                        {
                            return false;
                        }
                    }                

                    return true;
                }
            }

            public string Product
            {
                get
                {
                    return _product;
                }
            }

            public string Version
            {
                get
                {
                    return _version;
                }
            }

            public string Locale
            {
                get
                {
                    return _locale;
                }
            }

            #endregion
        }

        #endregion
    }
}
