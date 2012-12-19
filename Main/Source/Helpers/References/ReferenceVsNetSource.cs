using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Contents;
using Sandcastle.Utilities;  
using Sandcastle.Construction.ProjectSections;

namespace Sandcastle.References
{
    /// <summary>
    /// A content source which creates a reference contents from VS.NET project
    /// and/or solution files.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The VS.NET project and solution files are specified using the reference
    /// item, <see cref="ReferenceVsNetItem"/>.
    /// </para>
    /// <para>
    /// All the reference assemblies, comments and dependencies information 
    /// from the defined <see cref="ReferenceVsNetItem"/> items are combined
    /// into a single content, <see cref="ReferenceContent"/>, which is used
    /// by a single build group, <see cref="ReferenceGroup"/>.
    /// </para>
    /// <para>
    /// All the items must target the same framework, such as the standard
    /// <c>.NET Framework</c>, <c>Silverlight</c> or <c>.NET Portable</c>.
    /// Multiple framework targets are not allowed. However, various versions
    /// of the same target framework can be used, in which case the latest
    /// version will be applied.
    /// </para>
    /// </remarks>
    /// <example>
    /// <para>
    /// The following illustrates how to create and use this content source.
    /// </para>
    /// <code lang="c#">
    /// using System;
    /// 
    /// using Sandcastle;
    /// using Sandcastle.Contents;
    /// using Sandcastle.References;
    /// 
    /// namespace ConsoleSample
    /// {
    ///     class Program
    ///     {
    ///         static void Main(string[] args)
    ///         {
    ///             // 1. The solution source for the content
    ///             string sourceFile = @"A:\aDirectory\aSolution.sln";
    ///             // 2. Create the content source
    ///             ReferenceVsNetSource source = new ReferenceVsNetSource();
    ///             ReferenceVsNetItem vsItem = new ReferenceVsNetItem(
    ///                 new BuildFilePath(sourceFile));
    ///             vsItem.XamlSyntax = false;
    ///             // 3. Include a single project from the solution...
    ///             vsItem.AddInclude("{41A48F1C-3E52-4995-B181-363EDBC02CA0}");
    ///             source.Add(vsItem);
    /// 
    ///             // 4. Create sample project and namespace summaries...
    ///             CommentContent comments = source.Comments;
    ///             CommentItem projItem = new CommentItem("R:Project",
    ///                 CommentItemType.Project);
    ///             projItem.Value.Add(new CommentPart("Summary of the project",
    ///                 CommentPartType.Summary));
    ///             comments.Add(projItem);
    ///             CommentItem nsItem = new CommentItem("N:TestLibraryCLR",
    ///                 CommentItemType.Namespace);
    ///             nsItem.Value.Add(new CommentPart("Summary of the namespace",
    ///                 CommentPartType.Summary));
    ///             comments.Add(nsItem);
    /// 
    ///             // 5. Create the reference group from the source...
    ///             ReferenceGroup group = new ReferenceGroup(
    ///                 "The name of the group", Guid.NewGuid().ToString(), source);
    ///             group.RunningHeaderText = "The running header text";
    ///             group.VersionType = ReferenceVersionType.AssemblyAndFile;
    ///             group.RootNamespaceTitle = "Testing C++/CLR Library";
    /// 
    ///             // 6. Use and build the group...
    ///             // ----
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="ReferenceGroup.Source"/>
    /// <seealso cref="ReferenceVsNetItem"/>
    [Serializable]
    public sealed class ReferenceVsNetSource : ReferenceSource
    {
        #region Public Static Fields

        /// <summary>
        /// Gets the unique name of this content source.
        /// </summary>
        /// <value>
        /// The value is <c>Sandcastle.References.ReferenceVsNetSource</c>.
        /// </value>
        public const string SourceName =
            "Sandcastle.References.ReferenceVsNetSource";

        #endregion

        #region Private Fields

        private string _targetIdentifier;
        private BuildList<ReferenceVsNetItem> _listItems;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="ReferenceVsNetSource"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceVsNetSource"/> class
        /// with the default parameters. 
        /// </summary>
        public ReferenceVsNetSource()
        {
            _listItems        = new BuildList<ReferenceVsNetItem>();
            _targetIdentifier = String.Empty;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReferenceVsNetSource"/> class
        /// with parameters copied from the specified instance of the 
        /// <see cref="ReferenceVsNetSource"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="ReferenceVsNetSource"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        public ReferenceVsNetSource(ReferenceVsNetSource source)
            : base(source)
        {
            _listItems        = source._listItems;
            _targetIdentifier = source._targetIdentifier;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the unique name of this reference content source.
        /// </summary>
        /// <value>
        /// It has the same value as the <see cref="ReferenceVsNetSource.SourceName"/>.
        /// </value>
        public override string Name
        {
            get
            {
                return ReferenceVsNetSource.SourceName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this reference content source is
        /// valid and contains contents.
        /// </summary>
        /// <value>
        /// This property is <see langword="true"/> if the content source is
        /// not empty; otherwise, it is <see langword="false"/>. This also
        /// verifies that at least an item is not empty.
        /// </value>
        public override bool IsValid
        {
            get
            {
                if (_listItems == null || _listItems.Count == 0)
                {
                    return false;
                }
                for (int i = 0; i < _listItems.Count; i++)
                {
                    if (!_listItems[i].IsEmpty)
                    {
                        return true;
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the number of items in this reference content source.
        /// </summary>
        /// <value>
        /// This property returns the number of <see cref="ReferenceVsNetItem"/>
        /// items defining the content source.
        /// </value>
        public int Count
        {
            get
            {
                return _listItems.Count;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ReferenceVsNetItem"/> item at the
        /// specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index of the item to get or set.
        /// </param>
        /// <value>The item at the specified index.</value>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the <paramref name="index"/> is less than <c>0</c>.
        /// <para>-or-</para>
        /// The <paramref name="index"/> is equal to or greater than the <see cref="ReferenceVsNetSource.Count"/>.
        /// </exception>
        public ReferenceVsNetItem this[int index]
        {
            get
            {
                return _listItems[index];
            }
            set
            {
                if (value != null)
                {
                    _listItems[index] = value;
                }
            }
        }

        /// <summary>
        /// Gets the collection of the items defining this content source.
        /// </summary>
        /// <value>
        /// A list of <see cref="ReferenceVsNetItem"/>, which defines the
        /// content source.
        /// </value>
        public ICollection<ReferenceVsNetItem> Items
        {
            get
            {
                return _listItems;
            }
        }

        /// <summary>
        /// Gets or sets the identifier for the framework-family of the 
        /// target framework.
        /// </summary>
        /// <value>
        /// The possible values of the framework-families are <c>.NETFramework</c>, 
        /// <c>Silverlight</c>, <c>.NETPortable</c> etc. The default is empty string.
        /// </value>
        /// <remarks>
        /// This is used to filter the reference items to be included in the 
        /// generated reference content.
        /// </remarks>
        public string TargetFrameworkIdentifier
        {
            get
            {
                return _targetIdentifier;
            }
            set
            {
                if (value != null)
                {
                    value = value.Trim();
                }
                if (String.IsNullOrEmpty(value))
                {
                    _targetIdentifier = String.Empty;
                }
                else
                {
                    _targetIdentifier = value;
                }
            }
        }

        #endregion

        #region Public Methods

        public override ReferenceContent Create(BuildGroupContext groupContext)
        {
            BuildExceptions.NotNull(groupContext, "groupContext");

            BuildContext context = groupContext.Context;
            BuildLogger logger = null;
            if (context != null)
            {
                logger = context.Logger;
            }

            if (!this.IsInitialized)
            {
                throw new BuildException(String.Format(
                    "The content source '{0}' is not yet initialized.", this.Title));
            }
            if (!this.IsValid)
            {
                if (logger != null)
                {
                    logger.WriteLine(String.Format(
                        "The content group source '{0}' is invalid.", this.Title),
                        BuildLoggerLevel.Warn);
                }

                return null;
            }

            return this.OnCreateContent(groupContext);
        }

        /// <summary>
        /// Adds an item to the end of the list.
        /// </summary>
        /// <param name="item">The item to be added to the list.</param>
        /// <exception cref="item">
        /// If the <paramref name="ArgumentNullException"/> is <see langword="null"/>.
        /// </exception>
        public void Add(ReferenceVsNetItem item)
        {                               
            BuildExceptions.NotNull(item, "item");

            _listItems.Add(item);
        }

        /// <summary>
        /// Inserts the given item at the specified index.
        /// </summary>
        /// <param name="index">
        /// The zero-based index at which the <paramref name="item"/> should
        /// be inserted.
        /// </param>
        /// <param name="item">The item to insert.</param>
        /// <exception cref="item">
        /// If the <paramref name="ArgumentNullException"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If the <paramref name="index"/> is less than <c>0</c>.
        /// <para>-or-</para>
        /// The <paramref name="index"/> is greater than the <see cref="ReferenceVsNetSource.Count"/>
        /// </exception>
        public void Insert(int index, ReferenceVsNetItem item)
        {
            BuildExceptions.NotNull(item, "item");

            _listItems.Add(item);
        }

        /// <summary>
        /// Determines whether the specified item is in the list of items.
        /// </summary>
        /// <param name="item">The item to locate in the list.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the specified item is 
        /// found in the list; otherwise, it is <see langword="false"/>.
        /// </returns>
        /// <exception cref="item">
        /// If the <paramref name="ArgumentNullException"/> is <see langword="null"/>.
        /// </exception>
        public bool Contains(ReferenceVsNetItem item)
        {
            BuildExceptions.NotNull(item, "item");

            return _listItems.Contains(item);
        }

        /// <summary>
        /// Searches for the specified item and returns the zero-based index 
        /// of the first occurrence within the list.
        /// </summary>
        /// <param name="item">The item to locate.</param>
        /// <returns>
        /// This returns the zero-based index of the first occurrence of the
        /// <paramref name="item"/> within the list if found; otherwise, it
        /// returns <c>-1</c>.
        /// </returns>
        /// <exception cref="item">
        /// If the <paramref name="ArgumentNullException"/> is <see langword="null"/>.
        /// </exception>
        public int IndexOf(ReferenceVsNetItem item)
        {
            BuildExceptions.NotNull(item, "item");

            return _listItems.IndexOf(item);
        }

        /// <overloads>
        /// Removes an item from the list of items in this source.
        /// </overloads>
        /// <summary>
        /// Removes the item of the specified index from the list of items.
        /// </summary>
        /// <param name="index">The index of the item to be removed.</param>
        /// <returns>
        /// This returns <see langword="true"/> if list is not empty and the
        /// index is within range; otherwise, it returns <see langword="false"/>.
        /// </returns>
        public bool Remove(int index)
        {
            if (index < 0 || index >= _listItems.Count)
            {
                return false;
            }

            _listItems.RemoveAt(index);

            return true;
        }

        /// <summary>
        /// Removes the specified item from the list of items.
        /// </summary>
        /// <param name="item">The item to be removed from the list.</param>
        /// <returns>
        /// This returns <see langword="true"/> if the item is included in the
        /// list and is successfully removed; otherwise, this returns <see langword="false"/>.
        /// </returns>
        /// <exception cref="item">
        /// If the <paramref name="ArgumentNullException"/> is <see langword="null"/>.
        /// </exception>
        public bool Remove(ReferenceVsNetItem item)
        {
            BuildExceptions.NotNull(item, "item");

            return _listItems.Remove(item);
        }

        /// <summary>
        /// Removes all the <see cref="ReferenceVsNetItem"/> items in this
        /// reference source.
        /// </summary>
        public void Clear()
        {
            _listItems.Clear();
        }

        #endregion

        #region Private Methods

        #region ReadPropertyGroup Method

        private void ReadPropertyGroup(XmlReader reader)
        {
            string startElement = reader.Name;
            Debug.Assert(String.Equals(startElement, "propertyGroup"));
            Debug.Assert(String.Equals(reader.GetAttribute("name"), "General"));

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "property", StringComparison.OrdinalIgnoreCase))
                    {
                        switch (reader.GetAttribute("name").ToLower())
                        {
                            case "title":
                                this.Title = reader.ReadString();
                                break;
                            case "targetframeworkidentifier":
                                _targetIdentifier = reader.ReadString();
                                break;
                            default:
                                // Should normally not reach here...
                                throw new NotImplementedException(reader.GetAttribute("name"));
                        }
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement, StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region ReadItems Method

        private void ReadItems(XmlReader reader)
        {
            if (reader.IsEmptyElement)
            {
                return;
            }

            if (_listItems == null || _listItems.Count != 0)
            {
                _listItems = new BuildList<ReferenceVsNetItem>();
            }

            string startElement = reader.Name;

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, ReferenceVsNetItem.TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        ReferenceVsNetItem item = new ReferenceVsNetItem();
                        item.ReadXml(reader);

                        this.Add(item);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, startElement,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        #endregion

        #region OnCreateContent Method

        /// <summary>
        /// This creates the reference content from the available VS.NET items.
        /// </summary>
        /// <param name="groupContext">
        /// The context of the group which owns this source.
        /// </param>
        /// <returns>
        /// An instance of the <see cref="ReferenceContent"/> if successful;
        /// otherwise, this is <see langword="null"/>.
        /// </returns>
        private ReferenceContent OnCreateContent(BuildGroupContext groupContext)
        {
            Dictionary<string, ProjectSection> projects = new Dictionary<string, ProjectSection>(
                StringComparer.OrdinalIgnoreCase);

            BuildContext context = groupContext.Context;
            string platform      = context.TargetPlatform;
            string configuration = context.TargetConfiguration;

            BuildLogger logger   = context.Logger;

            // For each item, create the project sections...
            for (int i = 0; i < _listItems.Count; i++)
            {
                ReferenceVsNetItem vsNetItem = _listItems[i];

                if (vsNetItem != null && !vsNetItem.IsEmpty)
                {
                    HashSet<string> includeSet = new HashSet<string>(
                        vsNetItem.Includes);

                    IList<ProjectSection> sections =
                        ProjectSectionFactory.CreateSections(
                        vsNetItem.SourcePath.Path, platform, configuration,
                        includeSet);

                    if (sections != null && sections.Count != 0)
                    {
                        string useXamlSyntax = vsNetItem.XamlSyntax.ToString();

                        for (int j = 0; j < sections.Count; j++)
                        {
                            ProjectSection section = sections[j];
                            if (!String.IsNullOrEmpty(section.TargetFrameworkVersion) ||
                                !String.IsNullOrEmpty(section.TargetFrameworkIdentifier))
                            {
                                // Since the mapping from the VS.NET items
                                // to the project section is lost after this,
                                // we save this information...
                                section["UseXamlSyntax"] = useXamlSyntax;

                                projects[section.ProjectGuid] = section;
                            }
                        }
                    }
                }
            }

            // This is not expected, no managed project is found...
            if (projects.Count == 0)
            {
                if (logger != null)
                {
                    logger.WriteLine(String.Format(
                        "The '{0}' reference content source does not contain any valid project.",
                        this.Title), BuildLoggerLevel.Warn);
                }

                return null;
            }

            IList<ProjectSection> projectSetions = new List<ProjectSection>(
                projects.Values);

            if (String.IsNullOrEmpty(_targetIdentifier))
            {   
                // We are not filtering a particular target framework...
                if (projectSetions.Count > 1)
                {
                    BuildMultiMap<string, ProjectSection> multiSections =
                        new BuildMultiMap<string, ProjectSection>(
                            StringComparer.OrdinalIgnoreCase);

                    for (int i = 0; i < projectSetions.Count; i++)
                    {
                        ProjectSection section = projectSetions[i];
                        string targetIdentifier = section.TargetFrameworkIdentifier;
                        if (!String.IsNullOrEmpty(targetIdentifier))
                        {
                            multiSections.Add(targetIdentifier, section);
                        }
                    }

                    List<string> targetIdentifiers = new List<string>(multiSections.Keys);
                    if (targetIdentifiers.Count > 1)
                    {        
                        // Error, there are more one target framework identifier. 
                        if (logger != null)
                        {
                            StringBuilder builder = new StringBuilder();
                            for (int i = 0; i < targetIdentifiers.Count; i++)
                            {
                                builder.Append(targetIdentifiers[i]);
                                if (i < (targetIdentifiers.Count - 1))
                                {
                                    builder.Append(";");
                                }
                            }

                            logger.WriteLine(String.Format(
                                "The project items of '{0}' contain more than one target framework identifier '{1}'.", 
                                this.Title, builder.ToString()), BuildLoggerLevel.Error);
                        }

                        return null;
                    }
                }
            }
            else
            {
                IList<ProjectSection> filteredSetions = new List<ProjectSection>(projectSetions.Count);
                for (int i = 0; i < projectSetions.Count; i++ )
                {
                    ProjectSection section = projectSetions[i];
                    if (String.Equals(section.TargetFrameworkIdentifier,
                        _targetIdentifier, StringComparison.OrdinalIgnoreCase))
                    {
                        filteredSetions.Add(section);
                    }
                }

                // We will use the filtered sections
                projectSetions = filteredSetions;
            }

            // This is not expected, no managed project is found...
            if (projectSetions == null || projectSetions.Count == 0)
            {
                if (logger != null)
                {
                    logger.WriteLine(String.Format(
                        "The '{0}' reference content source does not contain any valid project.",
                        this.Title), BuildLoggerLevel.Warn);
                }

                return null;
            }

            ReferenceContent content = new ReferenceContent();

            HashSet<string> dependencyDirs = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);
            HashSet<string> referencedAssemblies = new HashSet<string>(
                StringComparer.OrdinalIgnoreCase);

            Version frameworkVersion = new Version(1, 0, 0, 0);

            for (int i = 0; i < projectSetions.Count; i++)
            {
                ProjectSection section = projectSetions[i];

                string commentFile  = section.CommentFile;
                if (String.IsNullOrEmpty(commentFile) || !File.Exists(commentFile))
                {
                    throw new BuildException(String.Format(
                        "The project '{0}' has no comment file.", 
                        section.ProjectName));
                }
                string assemblyFile = section.OutputFile;
                if (String.IsNullOrEmpty(assemblyFile) || !File.Exists(assemblyFile))
                {
                    throw new BuildException(String.Format(
                        "The project '{0}' has no assembly file.",
                        section.ProjectName));
                }

                ReferenceItem refItem = new ReferenceItem(commentFile, 
                    assemblyFile);

                string tempText = section["UseXamlSyntax"];
                if (!String.IsNullOrEmpty(tempText))
                {
                    refItem.XamlSyntax = Convert.ToBoolean(tempText);
                }

                // This should normally be in the format: v2.0, v3.0 etc
                string versionText = section.TargetFrameworkVersion;
                if (versionText != null && versionText.StartsWith("v", 
                    StringComparison.OrdinalIgnoreCase))
                {
                    versionText = versionText.Substring(1);
                }
                if (!String.IsNullOrEmpty(versionText))
                {
                    Version version = new Version(versionText);
                    if (version > frameworkVersion)
                    {
                        frameworkVersion = version;
                    }
                }

                content.Add(refItem);

                // Recursively extract the dependent assemblies information...
                CreateDependencies(section, dependencyDirs, 
                    referencedAssemblies);
            }

            // Provide the framework information of the content...
            BuildFrameworkKind frameworkKind = BuildFrameworkKind.None;
            string targetIdentifer = projectSetions[0].TargetFrameworkIdentifier;
            if (String.IsNullOrEmpty(targetIdentifer))
            {
                if (logger != null)
                {
                    logger.WriteLine("The target framework identifier is not found. Standard .NET Framework is assumed.", 
                        BuildLoggerLevel.Warn);
                }

                frameworkKind = BuildFrameworkKind.DotNet;
            }
            else
            {
                switch (targetIdentifer.ToLower())
                {
                    case ".netframework":
                        frameworkKind = BuildFrameworkKind.DotNet;
                        break;
                    case "silverlight":
                        frameworkKind = BuildFrameworkKind.Silverlight;
                        break;
                    case ".netportable":
                        frameworkKind = BuildFrameworkKind.Portable;
                        break;
                    case "scriptsharp":
                        // For the Script#, the version starts from 1.0 and
                        // does not match the .NET Framework version
                        frameworkVersion = BuildFrameworks.LatestScriptSharpVersion;
                        frameworkKind    = BuildFrameworkKind.ScriptSharp;
                        break;
                    case "compact":
                        frameworkKind = BuildFrameworkKind.Compact;
                        break;
                    default:
                        frameworkKind = BuildFrameworkKind.DotNet;
                        break;
                }
            }

            // Get the best framework for this content...
            BuildFramework framework = BuildFrameworks.GetFramework(
                frameworkVersion.Major, frameworkVersion.Minor, frameworkKind);

            if (framework == null)
            {
                if (logger != null)
                {
                    logger.WriteLine(String.Format(
                        "The expected version '{0}' for '{1}', cannot be found.",
                        frameworkVersion, this.Title), BuildLoggerLevel.Warn);
                }

                framework = BuildFrameworks.LatestFramework;

                if (framework == null)
                {
                    // If not successful, use the default...
                    framework = BuildFrameworks.DefaultFramework;
                }
            }

            content.FrameworkType = framework.FrameworkType;

            // Provide the dependency information for the content...
            DependencyContent depContents = content.Dependencies;
            IList<BuildDirectoryPath> depPaths = depContents.Paths;
            foreach (string dependencyDir in dependencyDirs)
            {
                if (String.IsNullOrEmpty(dependencyDir) || 
                    !Directory.Exists(dependencyDir))
                {
                    continue;
                }

                depPaths.Add(new BuildDirectoryPath(dependencyDir));
            }
            foreach (string referencedAssembly in referencedAssemblies)
            {
                depContents.AddItem(referencedAssembly);
            }

            // Provide other user-supplied information to the content...
            content.Comments         = this.Comments;
            content.HierarchicalToc  = this.HierarchicalToc;
            content.TypeFilters      = this.TypeFilters;
            content.AttributeFilters = this.AttributeFilters;

            return content;
        }

        /// <summary>
        /// This extracts the dependent assembly and project references from
        /// the project section.
        /// </summary>
        /// <param name="section">
        /// The project section from which the dependent information are extracted.
        /// </param>
        /// <param name="dependencyDirs">
        /// A set of the extracted dependent reference directories.
        /// </param>
        /// <param name="referencedAssemblies">
        /// A set of the reference assemblies.
        /// </param>
        private static void CreateDependencies(ProjectSection section,
            HashSet<string> dependencyDirs, HashSet<string> referencedAssemblies)
        {
            string outputPath = section.OutputPath;
            if (!String.IsNullOrEmpty(outputPath) && !outputPath.EndsWith("\\"))
            {
                outputPath += "\\";
            }
            dependencyDirs.Add(outputPath);

            IList<string> assemblies = section.ReferencedAssemblies;
            if (assemblies != null && assemblies.Count != 0)
            {
                for (int i = 0; i < assemblies.Count; i++)
                {
                    string assembly = assemblies[i];
                    if (!String.IsNullOrEmpty(assembly) &&
                        File.Exists(assembly))
                    {
                        referencedAssemblies.Add(assembly);
                    }
                }
            }

            ICollection<ProjectSection> children = section.Sections;
            if (children == null || children.Count == 0)
            {
                return;
            }

            foreach (ProjectSection child in children)
            {
                CreateDependencies(child, dependencyDirs, referencedAssemblies);
            }
        }

        #endregion

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void ReadXml(XmlReader reader)
        {
            BuildExceptions.NotNull(reader, "reader");

            Debug.Assert(reader.NodeType == XmlNodeType.Element);
            if (reader.NodeType != XmlNodeType.Element)
            {
                return;
            }

            if (!String.Equals(reader.Name, TagName,
                StringComparison.OrdinalIgnoreCase))
            {
                Debug.Assert(false, String.Format(
                    "The element name '{0}' does not match the expected '{1}'.",
                    reader.Name, TagName));
                return;
            }

            if (reader.IsEmptyElement)
            {
                return;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (String.Equals(reader.Name, "propertyGroup",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadPropertyGroup(reader);
                    }
                    else if (String.Equals(reader.Name, "items",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadItems(reader);
                    }
                    else if (String.Equals(reader.Name, "contents",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadContents(reader);
                    }
                    else if (String.Equals(reader.Name, "filters",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        this.ReadFilters(reader);
                    }
                }
                else if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Equals(reader.Name, TagName,
                        StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public override void WriteXml(XmlWriter writer)
        {
            BuildExceptions.NotNull(writer, "writer");

            if (!this.IsValid)
            {
                return;
            }

            writer.WriteStartElement(TagName);  // start - TagName
            writer.WriteAttributeString("name", this.Name);

            writer.WriteStartElement("propertyGroup");  // start - propertyGroup
            writer.WriteAttributeString("name", "General");
            writer.WritePropertyElement("Title", this.Title);
            writer.WritePropertyElement("TargetFrameworkIdentifier", _targetIdentifier);
            writer.WriteEndElement();                   // end - propertyGroup

            writer.WriteStartElement("items");  // start - items
            if (_listItems != null && _listItems.Count != 0)
            {
                for (int i = 0; i < _listItems.Count; i++)
                {
                    _listItems[i].WriteXml(writer);
                }
            }
            writer.WriteEndElement();           // end - items

            // Write the user-defined contents...
            this.WriteContents(writer);

            // Write the filters...
            this.WriteFilters(writer);

            writer.WriteEndElement();           // end - TagName
        }

        #endregion

        #region ICloneable Members

        public override ReferenceSource Clone()
        {
            ReferenceVsNetSource source = new ReferenceVsNetSource(this);

            this.Clone(source);

            if (_targetIdentifier != null)
            {
                source._targetIdentifier = String.Copy(_targetIdentifier);
            }
            if (_listItems != null)
            {
                source._listItems = _listItems.Clone();
            }

            return source;
        }

        #endregion
    }
}
