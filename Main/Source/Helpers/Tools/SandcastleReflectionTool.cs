using System;
using System.IO;
using System.Text;
using System.Compiler;
using System.Reflection;
using System.Xml;
using System.Xml.Xsl;
using System.Xml.XPath;
using System.Collections.Generic;

using Microsoft.Ddue.Tools;
using Microsoft.Ddue.Tools.Reflection;

namespace Sandcastle.Tools
{
    /// <summary>
    /// 
    /// </summary>
    public sealed class SandcastleReflectionTool : SandcastleTool
    {
        #region Private Fields

        private bool _documentInternals;  
        private string _configurationFile;
        private string _reflectionFile;
        private IList<string> _dependencyFiles;
        private IList<string> _assemblyFiles;

        private BuildLogger _logger;

        #endregion

        #region Constructors and Destructor

        public SandcastleReflectionTool()
        {
        }

        #endregion

        #region Public Properties

        public bool DocumentInternals
        {
            get
            {
                return _documentInternals;
            }
            set
            {
                _documentInternals = value;
            }
        }

        public string ConfigurationFile
        {
            get
            {
                return _configurationFile;
            }
            set
            {
                _configurationFile = value;
            }
        }

        public string ReflectionFile
        {
            get
            {
                return _reflectionFile;
            }
            set
            {
                _reflectionFile = value;
            }
        }

        public IList<string> DependencyFiles
        {
            get
            {
                return _dependencyFiles;
            }
            set
            {
                _dependencyFiles = value;
            }
        }

        public IList<string> AssemblyFiles
        {
            get
            {
                return _assemblyFiles;
            }
            set
            {
                _assemblyFiles = value;
            }
        }

        #endregion

        #region Protected Methods

        protected override bool OnRun(BuildLogger logger)
        {
            bool isSuccessful = false;

            _logger = logger;

            if (_assemblyFiles == null || _assemblyFiles.Count == 0)
            {
                _logger.WriteLine(
                    "The assemblies files for reflections are required, but are not specified.", 
                    BuildLoggerLevel.Error);
            }

            // load the configuration file
            XPathDocument config;
            XPathNavigator documentNode;
            string configDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string configFile      = Path.Combine(configDirectory, 
                "Sandcastle.Reflection.config");
            if (!String.IsNullOrEmpty(_configurationFile) && File.Exists(_configurationFile))
            {
                configFile = _configurationFile;
                configDirectory = Path.GetDirectoryName(configFile);
            }
            try
            {
                config = new XPathDocument(configFile);
                documentNode = config.CreateNavigator();
            }
            catch (IOException e)
            {               
                _logger.WriteLine(String.Format(
                    "An error occurred while attempting to read the configuration file '{0}'. The error message is: {1}",
                    configFile, e.Message), BuildLoggerLevel.Error);
                
                return isSuccessful;
            }
            catch (UnauthorizedAccessException e)
            {
                _logger.WriteLine(String.Format(
                    "An error occurred while attempting to read the configuration file '{0}'. The error message is: {1}",
                    configFile, e.Message), BuildLoggerLevel.Error);

                return isSuccessful;
            }
            catch (XmlException e)
            {
                _logger.WriteLine(String.Format(
                    "The configuration file '{0}' is not well-formed. The error message is: {1}",
                    configFile, e.Message), BuildLoggerLevel.Error);

                return isSuccessful;
            }

            // adjust the target platform
            XPathNodeIterator platformNodes = documentNode.Select(
                "/configuration/dduetools/platform");
            if (platformNodes.MoveNext())
            {
                XPathNavigator platformNode = platformNodes.Current;
                string version = platformNode.GetAttribute("version", String.Empty);
                string path = platformNode.GetAttribute("path", String.Empty);
                path = Environment.ExpandEnvironmentVariables(path);
                if (!Directory.Exists(path))
                {
                    _logger.WriteLine(String.Format(
                        "The specified target platform directory '{0}' does not exist.",
                        path), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                if (version == "2.0")
                {
                    TargetPlatform.SetToV2(path);
                }
                else if (version == "1.1")
                {
                    TargetPlatform.SetToV1_1(path);
                }
                else if (version == "1.0")
                {
                    TargetPlatform.SetToV1(path);
                }
                else
                {
                    _logger.WriteLine(String.Format(
                        "Unknown target platform version '{0}'.", version),
                        BuildLoggerLevel.Error);

                    return isSuccessful;
                }
            }

            // create a namer
            ApiNamer namer = new OrcasNamer();
            XPathNavigator namerNode = documentNode.SelectSingleNode(
                "/configuration/dduetools/namer");
            if (namerNode != null)
            {
                string assemblyPath = namerNode.GetAttribute("assembly", String.Empty);
                string typeName = namerNode.GetAttribute("type", String.Empty);

                assemblyPath = Environment.ExpandEnvironmentVariables(assemblyPath);
                if (!Path.IsPathRooted(assemblyPath)) 
                    assemblyPath = Path.Combine(configDirectory, assemblyPath);

                try
                {

                    Assembly assembly = Assembly.LoadFrom(assemblyPath);
                    namer = (ApiNamer)assembly.CreateInstance(typeName);

                    if (namer == null)
                    {
                        _logger.WriteLine(String.Format(
                            "The type '{0}' was not found in the component assembly '{1}'.",
                            typeName, assemblyPath), BuildLoggerLevel.Error);

                        return isSuccessful;
                    }

                }
                catch (IOException e)
                {
                    _logger.WriteLine(String.Format(
                        "A file access error occurred while attempting to load the component assembly '{0}'. The error message is: {1}",
                        assemblyPath, e.Message), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (UnauthorizedAccessException e)
                {
                    _logger.WriteLine(String.Format(
                        "A file access error occurred while attempting to load the component assembly '{0}'. The error message is: {1}",
                        assemblyPath, e.Message), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (BadImageFormatException)
                {
                    _logger.WriteLine(String.Format(
                        "The component assembly '{0}' is not a valid managed assembly.",
                        assemblyPath), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (TypeLoadException)
                {
                    _logger.WriteLine(String.Format(
                        "The type '{0}' was not found in the component assembly '{1}'.",
                        typeName, assemblyPath), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (MissingMethodException)
                {
                    _logger.WriteLine(String.Format(
                        "No appropriate constructor exists for the type'{0}' in the component assembly '{1}'.",
                        typeName, assemblyPath), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (TargetInvocationException e)
                {
                    _logger.WriteLine(String.Format(
                        "An error occurred while initializing the type '{0}' in the component assembly '{1}'. The error message and stack trace follows: {2}",
                        typeName, assemblyPath, e.InnerException.ToString()), 
                        BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (InvalidCastException)
                {
                    _logger.WriteLine(String.Format(
                        "The type '{0}' in the component assembly '{1}' is not a component type.",
                        typeName, assemblyPath), BuildLoggerLevel.Error);

                    return isSuccessful;
                }

            }

            // create a resolver
            AssemblyResolver resolver = new AssemblyResolver();
            XPathNavigator resolverNode = documentNode.SelectSingleNode(
                "/configuration/dduetools/resolver");
            if (resolverNode != null)
            {
                string assemblyPath = resolverNode.GetAttribute("assembly", String.Empty);
                string typeName = resolverNode.GetAttribute("type", String.Empty);

                assemblyPath = Environment.ExpandEnvironmentVariables(assemblyPath);
                if (!Path.IsPathRooted(assemblyPath)) assemblyPath = Path.Combine(configDirectory, assemblyPath);

                try
                {  
                    Assembly assembly = Assembly.LoadFrom(assemblyPath);
                    resolver = (AssemblyResolver)assembly.CreateInstance(typeName, false, 
                        BindingFlags.Public | BindingFlags.Instance, null, 
                        new Object[1] { resolverNode }, null, null);

                    if (resolver == null)
                    {
                        _logger.WriteLine(String.Format(
                            "The type '{0}' was not found in the component assembly '{1}'.",
                            typeName, assemblyPath), BuildLoggerLevel.Error);

                        return isSuccessful;
                    }

                }
                catch (IOException e)
                {
                    _logger.WriteLine(String.Format(
                        "A file access error occurred while attempting to load the component assembly '{0}'. The error message is: {1}",
                        assemblyPath, e.Message), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (UnauthorizedAccessException e)
                {
                    _logger.WriteLine(String.Format(
                        "A file access error occurred while attempting to load the component assembly '{0}'. The error message is: {1}",
                        assemblyPath, e.Message), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (BadImageFormatException)
                {
                    _logger.WriteLine(String.Format(
                        "The component assembly '{0}' is not a valid managed assembly.",
                        assemblyPath), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (TypeLoadException)
                {
                    _logger.WriteLine(String.Format(
                        "The type '{0}' was not found in the component assembly '{1}'.",
                        typeName, assemblyPath), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (MissingMethodException)
                {
                    _logger.WriteLine(String.Format(
                        "No appropriate constructor exists for the type'{0}' in the component assembly '{1}'.",
                        typeName, assemblyPath), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (TargetInvocationException e)
                {
                    _logger.WriteLine(String.Format(
                        "An error occurred while initializing the type '{0}' in the component assembly '{1}'. The error message and stack trace follows: {2}",
                        typeName, assemblyPath, e.InnerException.ToString()), 
                        BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (InvalidCastException)
                {
                    _logger.WriteLine(String.Format(
                        "The type '{0}' in the component assembly '{1}' is not a component type.",
                        typeName, assemblyPath), BuildLoggerLevel.Error);

                    return isSuccessful;
                }

            }
            resolver.UnresolvedAssemblyReference += 
                new EventHandler<AssemblyReferenceEventArgs>(OnUnresolvedAssemblyReferenceHandler);

            // Get a text-writer for output
            TextWriter output = null;
            if (!String.IsNullOrEmpty(_reflectionFile))
            {
                try
                {
                    output = new StreamWriter(_reflectionFile, false, Encoding.UTF8);
                }
                catch (IOException e)
                {
                    _logger.WriteLine(String.Format(
                        "An error occurred while attempting to create an output file. The error message is: {0}",
                        e.Message), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (UnauthorizedAccessException e)
                {
                    _logger.WriteLine(String.Format(
                        "An error occurred while attempting to create an output file. The error message is: {0}",
                        e.Message), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
            }
            else
            {
                output = Console.Out;
            }   

            // dependency directory
            IList<string> dependencies = new List<string>();

            if (_dependencyFiles != null && _dependencyFiles.Count != 0)
            {
                for (int i = 0; i < _dependencyFiles.Count; i++)
                {
                    string dependencyDir = _dependencyFiles[i];
                    if (dependencyDir != null)
                    {
                        dependencyDir = dependencyDir.Trim();
                    }

                    if (!String.IsNullOrEmpty(dependencyDir))
                    {
                        dependencies.Add(dependencyDir);
                    }
                }
            }

            // create a builder
            ManagedReflectionWriter builder = new ManagedReflectionWriter(
                output, namer);

            // specify the resolver for the builder
            builder.Resolver = resolver;

            // builder.ApiFilter = new ExternalDocumentedFilter(documentNode.SelectSingleNode("/configuration/dduetools"));

            // specify the filter for the builder

            if (_documentInternals)
            {
                builder.ApiFilter = new AllDocumentedFilter(
                    documentNode.SelectSingleNode("/configuration/dduetools"));
            }
            else
            {
                builder.ApiFilter = new ExternalDocumentedFilter(
                    documentNode.SelectSingleNode("/configuration/dduetools"));
            }

            // register add-ins to the builder

            XPathNodeIterator addinNodes = documentNode.Select(
                "/configuration/dduetools/addins/addin");
            foreach (XPathNavigator addinNode in addinNodes)
            {
                string assemblyPath = addinNode.GetAttribute("assembly", String.Empty);
                string typeName = addinNode.GetAttribute("type", String.Empty);

                assemblyPath = Environment.ExpandEnvironmentVariables(assemblyPath);
                if (!Path.IsPathRooted(assemblyPath)) 
                    assemblyPath = Path.Combine(configDirectory, assemblyPath);

                try
                {   
                    Assembly assembly = Assembly.LoadFrom(assemblyPath);
                    MRefBuilderAddIn addin = (MRefBuilderAddIn)assembly.CreateInstance(
                        typeName, false, BindingFlags.Public | BindingFlags.Instance, null, 
                        new Object[2] { builder, addinNode }, null, null);

                    if (namer == null)
                    {
                        _logger.WriteLine(String.Format(
                            "The type '{0}' was not found in the addin assembly '{1}'.",
                            typeName, assemblyPath), BuildLoggerLevel.Error);

                        return isSuccessful;
                    }

                }
                catch (IOException e)
                {
                    _logger.WriteLine(String.Format(
                        "A file access error occurred while attempting to load the addin assembly '{0}'. The error message is: {1}",
                        assemblyPath, e.Message), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (BadImageFormatException)
                {
                    _logger.WriteLine(String.Format(
                        "The addin assembly '{0}' is not a valid managed assembly.",
                        assemblyPath), BuildLoggerLevel.Error);
                    return isSuccessful;
                }
                catch (TypeLoadException)
                {
                    _logger.WriteLine(String.Format(
                        "The type '{0}' was not found in the addin assembly '{1}'.",
                        typeName, assemblyPath), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (MissingMethodException)
                {
                    _logger.WriteLine(String.Format(
                        "No appropriate constructor exists for the type '{0}' in the addin assembly '{1}'.",
                        typeName, assemblyPath), BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (TargetInvocationException e)
                {
                    _logger.WriteLine(String.Format(
                        "An error occurred while initializing the type '{0}' in the addin assembly '{1}'. The error message and stack trace follows: {2}",
                        typeName, assemblyPath, e.InnerException.ToString()), 
                        BuildLoggerLevel.Error);

                    return isSuccessful;
                }
                catch (InvalidCastException)
                {
                    _logger.WriteLine(String.Format(
                        "The type '{0}' in the addin assembly '{1}' is not an MRefBuilderAddIn type.",
                        typeName, assemblyPath), BuildLoggerLevel.Error);

                    return isSuccessful;
                }   
            }

            try
            { 
                // add a handler for unresolved assembly references
                //builder.UnresolvedModuleHandler = new System.Compiler.Module.AssemblyReferenceResolver(AssemblyNotFound);

                // load dependent bits
                foreach (string dependency in dependencies)
                {
                    try
                    {
                        builder.LoadAccessoryAssemblies(dependency);
                    }
                    catch (IOException e)
                    {
                        _logger.WriteLine(String.Format(
                            "An error occurred while loading dependency assemblies. The error message is: {0}",
                            e.Message), BuildLoggerLevel.Error);

                        return isSuccessful;
                    }
                }

                // parse the bits
                foreach (string dllPath in _assemblyFiles)
                {
                    try
                    {
                        builder.LoadAssemblies(dllPath);
                    }
                    catch (IOException e)
                    {
                        _logger.WriteLine(String.Format(
                            "An error occurred while loading assemblies for reflection. The error message is: {0}",
                            e.Message), BuildLoggerLevel.Error);

                        return isSuccessful;
                    }
                }

                _logger.WriteLine(String.Format(
                    "Loaded {0} assemblies for reflection and {1} dependency assemblies.",
                    builder.Assemblies.Length, builder.AccessoryAssemblies.Length), 
                    BuildLoggerLevel.Info);

                // register callbacks

                //builder.RegisterStartTagCallback("apis", new MRefBuilderCallback(startTestCallback));

                //MRefBuilderAddIn addin = new XamlAttachedMembersAddIn(builder, null);

                builder.VisitApis();

                _logger.WriteLine(String.Format(
                    "Wrote information on {0} namespaces, {1} types, and {2} members",
                    builder.Namespaces.Length, builder.Types.Length, builder.Members.Length), 
                    BuildLoggerLevel.Info);

                isSuccessful = true;
            }
            finally
            {
                resolver.UnresolvedAssemblyReference -=
                    new EventHandler<AssemblyReferenceEventArgs>(OnUnresolvedAssemblyReferenceHandler);

                builder.Dispose();
            }

            return isSuccessful;
        }

        #endregion

        #region Private Methods

        private AssemblyNode OnAssemblyNotFound(AssemblyReference reference, 
            System.Compiler.Module module)
        {
            _logger.WriteLine(String.Format(
                "Unresolved assembly reference: {0} ({1}) required by {2}",
                reference.Name, reference.StrongName, module.Name), 
                BuildLoggerLevel.Error);

            return null;
        }

        private void OnUnresolvedAssemblyReferenceHandler(Object o, 
            AssemblyReferenceEventArgs e)
        {
            _logger.WriteLine(String.Format(
                "Unresolved assembly reference: {0} ({1}) required by {2}", 
                e.Reference.Name, e.Reference.StrongName, e.Referrer.Name),
                BuildLoggerLevel.Error);
        }

        #endregion
    }
}
