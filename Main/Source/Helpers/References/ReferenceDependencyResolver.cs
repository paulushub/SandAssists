using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;

using Mono.Cecil;
using Sandcastle.Contents;
using Sandcastle.Utilities;

namespace Sandcastle.References
{
    [Serializable]
    public sealed class ReferenceDependencyResolver : ReferenceGroupVisitor
    {
        #region Public Fields

        /// <summary>
        /// Gets the unique name of this group visitor.
        /// </summary>
        /// <value>
        /// A string specifying the unique name of this group visitor.
        /// </value>
        public const string VisitorName =
            "Sandcastle.References.ReferenceDependencyResolver";

        #endregion

        #region Private Fields

        private string _sourceId;
        private ReferenceContent _content;

        private List<ReferenceItem> _listReference;

        private Dictionary<string, bool> _dictionary;
        private Dictionary<string, bool> _dictDependency;
        private Dictionary<string, bool> _dictReference;

        private DependencyContent _resolveContent;
        private DependencyContent _dependencyContent;

        private List<string> _linkCommentFiles;
        private List<DependencyItem> _bindingRedirects;        

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceDependencyResolver"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceDependencyResolver"/> class
        /// to the default values.
        /// </summary>
        public ReferenceDependencyResolver()
            : this(VisitorName)
        {
        }

        public ReferenceDependencyResolver(string sourceId, 
            ReferenceContent content) : this(VisitorName)
        {
            _sourceId = sourceId;
            _content  = content;
        }         

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceDependencyResolver"/> class
        /// with the specified group visitor name.
        /// </summary>
        /// <param name="visitorName">
        /// A <see cref="System.String"/> specifying the name of this group visitor.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="visitorName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If the <paramref name="visitorName"/> is empty.
        /// </exception>
        private ReferenceDependencyResolver(string visitorName)
            : base(visitorName)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceDependencyResolver"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceDependencyResolver"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceDependencyResolver"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceDependencyResolver(ReferenceDependencyResolver source)
            : base(source)
        {
        }

        #endregion

        #region Protected Methods

        protected override void OnVisit(ReferenceGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            if (!this.IsInitialized)
            {
                throw new BuildException(
                    "ReferenceDependencyResolver: The reference dependency resolver is not initialized.");
            }

            BuildContext context = this.Context;
            BuildLogger logger = context.Logger;

            if (logger != null)
            {
                logger.WriteLine("Begin - Resolving reference dependencies.",
                    BuildLoggerLevel.Info);
            }

            ReferenceGroupContext groupContext = context.GroupContexts[group.Id]
                as ReferenceGroupContext;
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            ReferenceGroupContext sourceContext = groupContext;
            if (!String.IsNullOrEmpty(_sourceId))
            {
                sourceContext = groupContext.Contexts[_sourceId]
                    as ReferenceGroupContext;
            }

            if (sourceContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            _linkCommentFiles = new List<string>();
            _bindingRedirects = new List<DependencyItem>();

            sourceContext.LinkCommentFiles = _linkCommentFiles;
            sourceContext.BindingRedirects = _bindingRedirects;

            string dependencyDir = sourceContext.DependencyDir;

            ReferenceContent content = _content;
            if (content == null)
            {
                content = group.Content;
            }

            DependencyContent dependencies = content.Dependencies;

            _dependencyContent = dependencies;

            this.ResolveDependency(sourceContext, content);
                                        
            if (_dependencyContent != null && _dependencyContent.Count != 0)
            {
                this.CreateDependency(_dependencyContent, dependencyDir);
            }
            if (_resolveContent != null && _resolveContent.Count != 0)
            {
                this.CreateDependency(_resolveContent, dependencyDir);
            }

            if (logger != null)
            {
                logger.WriteLine("Completed - Resolving reference dependencies.",
                    BuildLoggerLevel.Info);
            }
        }

        #endregion

        #region Private Methods

        #region CreateDependency Method

        private void CreateDependency(DependencyContent dependencies,
            string dependencyDir)
        {
            if (dependencies == null || dependencies.Count == 0)
            {
                return;
            }

            int itemCount = dependencies.Count;
            for (int i = 0; i < itemCount; i++)
            {
                DependencyItem dependency = dependencies[i];
                if (dependency == null || dependency.IsEmpty)
                {
                    continue;
                }

                string dependencyFile = dependency.Location;
                if (!String.IsNullOrEmpty(dependencyFile))
                {
                    string fileName = Path.GetFileName(dependencyFile);
                    fileName        = Path.Combine(dependencyDir, fileName);
                    if (!File.Exists(fileName))
                    {
                        File.Copy(dependencyFile, fileName, true);
                        File.SetAttributes(fileName, FileAttributes.Normal);

                        // Set if the comment file exists and link it...
                        dependencyFile = Path.ChangeExtension(dependencyFile, ".xml");
                        if (File.Exists(dependencyFile))
                        {
                            _linkCommentFiles.Add(dependencyFile);
                        }

                        if (dependency.IsRedirected)
                        {
                            _bindingRedirects.Add(dependency);
                        }
                    }
                }
            }
        }

        #endregion        

        #region ResolveDependency Method

        private void ResolveDependency(ReferenceGroupContext sourceContext,
            ReferenceContent referenceContent)
        {
            BuildContext context = this.Context;
            BuildLogger logger   = context.Logger;

            BuildFramework framework = sourceContext.Framework;
            if (framework == null)
            {
                throw new BuildException("No valid framework is specified.");
            }

            DependencyContent dependencies = referenceContent.Dependencies;

            _resolveContent = new DependencyContent();
            _listReference  = new List<ReferenceItem>();

            _dictionary = new Dictionary<string, bool>(
                StringComparer.OrdinalIgnoreCase);
            _dictDependency = new Dictionary<string, bool>(
                StringComparer.OrdinalIgnoreCase);
            _dictReference = new Dictionary<string, bool>(
                StringComparer.OrdinalIgnoreCase);

            if (dependencies != null && dependencies.Count != 0)
            {
                for (int i = 0; i < dependencies.Count; i++)
                {
                    DependencyItem depItem = dependencies[i];
                    if (!depItem.IsEmpty)
                    {
                        _dictDependency[depItem.Location] = true;
                    }
                }
            }

            // Index the reference items to prevent cross referencing...
            if (referenceContent != null && referenceContent.Count != 0)
            {
                for (int i = 0; i < referenceContent.Count; i++)
                {
                    ReferenceItem refItem = referenceContent[i];
                    if (!refItem.IsEmpty && !refItem.IsCommentOnly)
                    {
                        string refAssembly = refItem.Assembly;
                        if (!_dictReference.ContainsKey(refAssembly))
                        {
                            _dictReference[refAssembly] = true;
                            _listReference.Add(refItem);
                        }
                    }
                }
            }

            GeneralAssemblyResolver resolver = new GeneralAssemblyResolver();

            // Add the reference item directories, the most likely place to
            // find the dependencies
            for (int i = 0; i < _listReference.Count; i++)
            {
                resolver.AddSearchDirectory(
                    Path.GetDirectoryName(_listReference[i].Assembly));
            }

            // Add specified dependency directories, if any...
            IList<BuildDirectoryPath> dependencyPaths = dependencies.Paths;
            if (dependencyPaths != null && dependencyPaths.Count != 0)
            {
                for (int i = 0; i < dependencyPaths.Count; i++)
                {
                    BuildDirectoryPath dependencyPath = dependencyPaths[i];
                    if (dependencyPath.Exists)
                    {
                        resolver.AddSearchDirectory(dependencyPath);
                    }
                }
            }

            Version version = framework.Version;

            // Add, for the Silverlight, known installation directories...
            if (framework.FrameworkType.IsSilverlight)
            {
                resolver.UseGac = false;

                string programFiles = PathUtils.ProgramFiles32;
                string searchDir = Path.Combine(programFiles,
                    @"Microsoft Silverlight\" + version.ToString());
                if (Directory.Exists(searchDir))
                {
                    resolver.AddSearchDirectory(searchDir);
                }

                searchDir = Path.Combine(programFiles,
                    @"Reference Assemblies\Microsoft\Framework\Silverlight\v" + version.ToString(2));
                if (Directory.Exists(searchDir))
                {
                    resolver.AddSearchDirectory(searchDir);
                }

                searchDir = Path.Combine(programFiles,
                    @"Microsoft SDKs\Silverlight\v" + version.ToString(2));
                if (Directory.Exists(searchDir))
                {
                    resolver.AddSearchDirectory(searchDir);

                    string tempDir = String.Copy(searchDir);
                    searchDir = Path.Combine(tempDir, @"Libraries\Client");
                    if (Directory.Exists(searchDir))
                    {
                        resolver.AddSearchDirectory(searchDir);
                    }
                    searchDir = Path.Combine(tempDir, @"Libraries\Server");
                    if (Directory.Exists(searchDir))
                    {
                        resolver.AddSearchDirectory(searchDir);
                    }
                }

                if (version.Major == 3)
                {
                    // 3. The Expression 3.0 Blend SDK...
                    string otherDir = Path.Combine(programFiles,
                       @"Microsoft SDKs\Expression\Blend 3\Interactivity\Libraries\Silverlight");
                    if (Directory.Exists(otherDir))
                    {
                        resolver.AddSearchDirectory(otherDir);
                    }
                    otherDir = Path.Combine(programFiles,
                       @"Microsoft SDKs\Expression\Blend 3\Prototyping\Libraries\Silverlight");
                    if (Directory.Exists(otherDir))
                    {
                        resolver.AddSearchDirectory(otherDir);
                    }
                }
                else if (version.Major == 4)
                {
                    // Consider the extension libraries...
                    // 1. The RIA Services...
                    string otherDir = Path.Combine(programFiles,
                       @"Microsoft SDKs\RIA Services\v1.0\Libraries\Silverlight");
                    if (Directory.Exists(otherDir))
                    {
                        resolver.AddSearchDirectory(otherDir);
                    }
                    // 2. For the Silverlight Toolkit...
                    otherDir = Path.Combine(programFiles,
                       @"Microsoft SDKs\Silverlight\v4.0\Toolkit");
                    if (Directory.Exists(otherDir))
                    {
                        // Get the latest installed version...
                        string[] dirs = Directory.GetDirectories(otherDir);
                        if (dirs != null && dirs.Length != 0)
                        {
                            string dir = String.Empty;
                            DateTime latestDt = DateTime.MinValue;
                            for (int j = 0; j < dirs.Length; j++)
                            {
                                string latestDir = Path.GetFileName(dirs[j]);
                                DateTime dt;
                                if (DateTime.TryParse(latestDir, out dt))
                                {
                                    if (dt > latestDt)
                                    {
                                        latestDt = dt;
                                        dir = latestDir;
                                    }
                                }
                            }

                            otherDir = Path.Combine(otherDir, dir + @"\Bin");
                            if (Directory.Exists(otherDir))
                            {
                                resolver.AddSearchDirectory(otherDir);
                            }
                        }
                    }

                    // 3. The Expression 4.0 Blend SDK...
                    otherDir = Path.Combine(programFiles,
                       @"Microsoft SDKs\Expression\Blend\Silverlight\v4.0\Libraries");
                    if (Directory.Exists(otherDir))
                    {
                        resolver.AddSearchDirectory(otherDir);
                    }
                }
            }
            else
            {
                string programFiles = Environment.GetFolderPath(
                    Environment.SpecialFolder.ProgramFiles);
                if (version.Major == 3)
                {
                    // 3. The Expression 3.0 Blend SDK...
                    string otherDir = Path.Combine(programFiles,
                       @"Microsoft SDKs\Expression\Blend 3\Interactivity\Libraries\.NETFramework");
                    if (Directory.Exists(otherDir))
                    {
                        resolver.AddSearchDirectory(otherDir);
                    }
                }
                else if (version.Major == 4)
                {
                    string otherDir = Path.Combine(programFiles,
                       @"Microsoft SDKs\Expression\Blend\.NETFramework\v4.0\Libraries");
                    if (Directory.Exists(otherDir))
                    {
                        resolver.AddSearchDirectory(otherDir);
                    }
                }                
            }

            for (int i = 0; i < _listReference.Count; i++)
            {
                ReferenceItem refItem = _listReference[i];
                AssemblyDefinition asmDef = AssemblyDefinition.ReadAssembly(
                    refItem.Assembly);

                ModuleDefinition modDef = asmDef.MainModule;

                // Try resolving all the dependencies...
                if (modDef.HasAssemblyReferences)
                {
                    IList<AssemblyNameReference> asmRefs = modDef.AssemblyReferences;

                    if (asmRefs != null && asmRefs.Count != 0)
                    {
                        for (int j = 0; j < asmRefs.Count; j++)
                        {
                            this.Resolve(logger, asmRefs[j], resolver);
                        }
                    }
                }

                // Try resolving all the XmlnsDefinitionAttribute attributes...
                if (asmDef.HasCustomAttributes && refItem.XamlSyntax)
                {
                    this.ResolveXmlnsDefinitions(sourceContext, asmDef, modDef.Name);
                }
            }
        }

        private void Resolve(BuildLogger logger, AssemblyNameReference nameRef, 
            IAssemblyResolver resolver)
        {
            if (nameRef.Name.StartsWith("mscorlib",
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }   
            if (_dictionary.ContainsKey(nameRef.FullName))
            {
                return;
            }

            _dictionary.Add(nameRef.FullName, true);

            ModuleDefinition refModDef = null;
            try
            {
                AssemblyDefinition refAsmDef = resolver.Resolve(nameRef);
                if (refAsmDef == null)
                {
                    return;
                }

                refModDef = refAsmDef.MainModule;
                if (refModDef == null)
                {
                    return;
                }

                string qualifiedName = refModDef.FullyQualifiedName;

                // Prevent adding assemblies in the GAC...
                if (String.IsNullOrEmpty(qualifiedName) || qualifiedName.IndexOf(
                    "GAC_MSIL", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return;
                }    
                else if (qualifiedName.IndexOf("GAC_32", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return;
                }    
                else if (qualifiedName.IndexOf("GAC_64", StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return;
                }

                //if (!_dictDependency.ContainsKey(qualifiedName) &&
                //    !_dictReference.ContainsKey(qualifiedName))
                {
                    DependencyItem item = new DependencyItem(Path.GetFileName(
                        qualifiedName), qualifiedName);

                    _resolveContent.Add(item);

                    if (logger != null)
                    {
                        logger.WriteLine("Referenced Assembly: " + item.Name, BuildLoggerLevel.Info);
                    }

                    AssemblyDefinition foundAsmDef = AssemblyDefinition.ReadAssembly(
                       qualifiedName);

                    if (nameRef.Version != foundAsmDef.Name.Version)
                    {
                        if (logger != null)
                        {
                            logger.WriteLine(String.Format(
                                "Creating Redirecting: '{0}' from version '{1}' to '{2}'",
                                item.Name, nameRef.Version, foundAsmDef.Name.Version),
                                BuildLoggerLevel.Info);
                        }

                        item.StrongName      = foundAsmDef.FullName;

                        // For the redirection...
                        item.Redirected      = true;
                        item.RedirectVersion = nameRef.Version;
                        item.SetPublicKeyToken(nameRef.PublicKeyToken);
                        if (!String.IsNullOrEmpty(nameRef.Culture))
                        {
                            item.RedirectCulture = new CultureInfo(nameRef.Culture);
                        }
                    }
                }
            }
            catch (AssemblyResolutionException ex)
            {
                AssemblyNameReference exAsmRef = ex.AssemblyReference;
                if (exAsmRef != null && logger != null)
                {
                    logger.WriteLine("Could not resolve the reference dependency: " 
                        + exAsmRef.FullName, BuildLoggerLevel.Error);
                }

                return;
            }

            IList<AssemblyNameReference> asmRefs = refModDef.AssemblyReferences;

            if (asmRefs != null && asmRefs.Count != 0)
            {
                for (int i = 0; i < asmRefs.Count; i++)
                {
                    AssemblyNameReference nextAsmRef = asmRefs[i];

                    if (!_dictionary.ContainsKey(nextAsmRef.FullName))
                    {
                        this.Resolve(logger, nextAsmRef, resolver);
                    }
                }
            }
        }

        private void ResolveXmlnsDefinitions(ReferenceGroupContext sourceContext,
            AssemblyDefinition asmDef, string assembly)
        {
            ICollection<CustomAttribute> attributes = asmDef.CustomAttributes;
            if (attributes == null || attributes.Count == 0)
            {
                return;
            }

            BuildMultiMap<string, string> xmlnsDefs = new BuildMultiMap<string, string>();

            sourceContext.SetValue(assembly, xmlnsDefs);

            foreach (CustomAttribute attribute in attributes)
            {
                if (String.Equals(attribute.AttributeType.Name,
                    "XmlnsDefinitionAttribute", StringComparison.OrdinalIgnoreCase))
                {
                    ICollection<CustomAttributeArgument> args = attribute.ConstructorArguments;

                    if (args != null && args.Count == 2)
                    {
                        string xmlValue = null;
                        string clrValue = null;

                        int count = 0;
                        foreach (CustomAttributeArgument arg in args)
                        {
                            count++;
                            if (String.Equals(arg.Type.FullName, "System.String", 
                                StringComparison.OrdinalIgnoreCase))
                            {
                                if (count == 1)
                                {
                                    xmlValue = arg.Value.ToString();
                                }
                                else if (count == 2)
                                {
                                    clrValue = arg.Value.ToString();
                                }
                            }

                            if (!String.IsNullOrEmpty(xmlValue) &&
                                !String.IsNullOrEmpty(clrValue))
                            {
                                xmlnsDefs.Add(xmlValue, clrValue);
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #endregion

        #region ICloneable Members

        /// <summary>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new build object that is a deep copy of this instance.
        /// </returns>
        public override ReferenceGroupVisitor Clone()
        {
            ReferenceDependencyResolver visitor = new ReferenceDependencyResolver(this);

            return visitor;
        }

        #endregion
    }
}
