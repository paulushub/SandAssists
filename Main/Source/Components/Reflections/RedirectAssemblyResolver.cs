using System;
using System.Compiler;
using System.Xml.XPath;
using System.Globalization;
using System.Collections.Generic;

using Microsoft.Ddue.Tools.Reflection;
using Microsoft.Ddue.Tools.CommandLine;
            
namespace Sandcastle.Reflections
{
    /// <summary>
    /// An implementation of the <see cref="AssemblyResolver"/> for the
    /// <c>MRefBuilder</c> reflection tool to support redirection of assembly
    /// versions.
    /// </summary>
    /// <remarks>
    /// A sample configuration of this assembly resolver is shown below:
    /// <code lang="xml">
    /// <![CDATA[
    /// <resolver type="Sandcastle.Reflections.RedirectAssemblyResolver" assembly="Sandcastle.Components.dll" use-gac="false">
    ///   <bindingRedirects>
    ///     <bindingRedirect 
    ///       from="Sample.Tests, Version=1.1.2.4, Culture=neutral, PublicKeyToken=6544464cdeaab546" 
    ///       to="Sample.Tests, Version=1.1.2.8, Culture=neutral, PublicKeyToken=6544464cdeaab546" />
    ///   </bindingRedirects>
    /// </resolver>    
    /// ]]>
    /// </code>
    /// <list type="bullet">
    /// <item>
    /// <term>from</term>
    /// <description>This specifies the referenced assembly strong name.</description>
    /// </item>
    /// <item>
    /// <term>to</term>
    /// <description>This specifies the available assembly strong name.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public sealed class RedirectAssemblyResolver : AssemblyResolver
    {
        #region Private Fields

        private Dictionary<string, string>       _targets;
        private Dictionary<string, AssemblyNode> _redirects;

        #endregion

        #region Constructors and Destructor

        /// <summary>
        /// Initializes a new instance of the <see cref="RedirectAssemblyResolver"/>
        /// with the specified <see cref="XPathNavigator"/>, which represents the
        /// configuration data for this instance.
        /// </summary>
        /// <param name="configuration">
        /// An instance of the <see cref="XPathNavigator"/> defining the configuration
        /// data for this instance.
        /// </param>
        public RedirectAssemblyResolver(XPathNavigator configuration) 
            : base(configuration) 
        {
            _targets = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            _redirects = new Dictionary<string, AssemblyNode>(
                StringComparer.OrdinalIgnoreCase);

            XPathNodeIterator iterator = configuration.Select(
                "bindingRedirects/bindingRedirect");
            if (iterator != null && iterator.Count != 0)
            {
                foreach (XPathNavigator navigator in iterator)
                {
                    string redirectFrom = navigator.GetAttribute(
                        "from", String.Empty);
                    string redirectTo = navigator.GetAttribute(
                        "to", String.Empty);

                    if (!String.IsNullOrEmpty(redirectFrom) &&
                        !String.IsNullOrEmpty(redirectTo))
                    {
                        _targets[redirectTo] = redirectFrom;
                    }
                }
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        public override void Add(AssemblyNode assembly)
        {
            base.Add(assembly);

            string strongName = assembly.StrongName;

            if (_targets != null && _targets.Count != 0 &&
                _targets.ContainsKey(strongName))
            {
                string redirectName = _targets[strongName];

                if (!String.IsNullOrEmpty(redirectName))
                {
                    _redirects[redirectName] = assembly;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="module"></param>
        /// <returns></returns>
        public override AssemblyNode ResolveReference(
            AssemblyReference reference, Module module)
        {   
            AssemblyNode assembly = base.ResolveReference(reference, module);

            if (assembly == null)
            {
                string strongName = reference.StrongName;

                if (_redirects != null && _redirects.Count != 0 &&
                    _redirects.ContainsKey(strongName))
                {   
                    assembly = _redirects[strongName];

                    ConsoleApplication.WriteMessage(LogLevel.Info,
                        "Redirecting, '{0}' version, from '{1}' to '{2}'", 
                        reference.Name, reference.Version, assembly.Version);
                }
            }

            return assembly;
        }

        #endregion
    }
}
