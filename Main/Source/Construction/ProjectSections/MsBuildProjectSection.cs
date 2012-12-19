using System;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Sandcastle.Construction.Utils; 
using Sandcastle.Construction.Evaluation;

namespace Sandcastle.Construction.ProjectSections
{
    /// <summary>
    /// This is the <see cref="abstract"/> base class for all project sections
    /// of projects in the <c>MSBuild</c> file format.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <c>Visual C++</c> projects created by the <c>VS.NET 2005</c> and
    /// <c>VS.NET 2008</c> are not in the <c>MSBuild</c> file format.
    /// </para>
    /// </remarks>
    public abstract class MsBuildProjectSection : ProjectSection
    {
        #region Private Static Fields

        /// <summary>
        /// A regular expression for extracting property referenced name.
        /// </summary>
        private static readonly Regex _propertyRegex = new Regex(
            "^\\$\\(([^\\$\\(\\)]*)\\)$", RegexOptions.Compiled);

        #endregion

        #region Private Fields

        private ProjectRootElement _project;

        #endregion

        #region Constructors and Destructor

        protected MsBuildProjectSection()
        {
        }

        #endregion

        #region Public Properties

        public ProjectRootElement Project
        {
            get
            {
                return _project;
            }
        }

        #endregion

        #region Public Methods

        public override bool Parse(ProjectSectionContext context, string projectFile)
        {
            if (!base.Parse(context, projectFile))
            {
                return false;
            }

            // Open the project for parsing...
            _project = ProjectRootElement.Open(this.ProjectFile, new ProjectCollection());

            return (_project != null);
        }

        #region IsConditionMatched Method

        /// <summary>
        /// This is a simple condition matching designed for only the configuration
        /// and platform conditions.
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="platform"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static bool IsConditionMatched(string configuration,
            string platform, string condition)
        {
            // Format: '$(Configuration)|$(Platform)' == 'Debug|x86'
            //         '$(Configuration)|$(Platform)' == 'Release|AnyCPU'
            bool isMatched = false;

            if (String.IsNullOrEmpty(condition) && String.IsNullOrEmpty(configuration))
            {
                return isMatched;
            }

            StreamTokenizer tokenizer = new StreamTokenizer(condition);
            List<Token> tokens = new List<Token>();
            if (!tokenizer.Tokenize(tokens))
            {
                return isMatched;
            }

            String symbols = String.Empty;
            List<string> quotedValues = new List<string>();
            foreach (Token token in tokens)
            {
                switch (token.Type)
                {
                    case TokenType.Char:
                        if (Char.IsSymbol(token.StringValue, 0))
                        {
                            symbols += token.StringValue;
                        }
                        break;
                    case TokenType.Quote:
                        // Format: '$(Configuration)', we remove the single quotes...
                        quotedValues.Add(token.StringValue.Trim('\''));
                        break;
                }
            }

            symbols = symbols.Trim();
            if (!symbols.Equals("==", StringComparison.Ordinal) ||
                quotedValues.Count != 2)
            {
                return isMatched;
            }

            IDictionary<string, string> expressionMap = ExtractConditionMap(
                quotedValues[0], quotedValues[1]);

            if (expressionMap == null || expressionMap.Count == 0)
            {
                return isMatched;
            }
            string configurationValue;
            if (!expressionMap.TryGetValue("Configuration", out configurationValue) ||
                String.IsNullOrEmpty(configurationValue))
            {
                return isMatched;
            }
            if (!configuration.Equals(configurationValue,
                StringComparison.OrdinalIgnoreCase))
            {
                return isMatched;
            }
            if (!String.IsNullOrEmpty(platform) && expressionMap.Count > 1)
            {
                string platformValue;
                if (!expressionMap.TryGetValue("Platform", out platformValue) ||
                    String.IsNullOrEmpty(platformValue))
                {
                    isMatched = false;
                }
                else if (platform.Equals("AnyCPU", StringComparison.OrdinalIgnoreCase) ||
                    platform.Equals("Any CPU", StringComparison.OrdinalIgnoreCase))
                {
                    platform = platform.Replace(" ", "");
                    isMatched = platform.Equals(platformValue.Replace(" ", ""),
                        StringComparison.OrdinalIgnoreCase);
                }
                else
                {
                    isMatched = platform.Equals(platformValue, StringComparison.OrdinalIgnoreCase);
                }
            }

            return isMatched;
        }

        #endregion

        #endregion

        #region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rootElement"></param>
        /// <returns></returns>
        protected abstract bool ParseProperties();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyGroup"></param>
        /// <returns></returns>
        protected abstract bool ParseProperyGroup(ProjectPropertyGroupElement propertyGroup);

        #region ParseChoose Method

        protected virtual bool ParseChoose()
        {
            ICollection<ProjectChooseElement> chooseElements =
                _project.ChooseElements;

            if (chooseElements == null || chooseElements.Count == 0)
            {
                return false;
            }

            List<ProjectPropertyGroupElement> conditionedGroups =
                new List<ProjectPropertyGroupElement>();

            List<ProjectPropertyGroupElement> otherwiseGroups =
                new List<ProjectPropertyGroupElement>();

            foreach (ProjectChooseElement chooseElement in chooseElements)
            {
                // Get the matching property groups in the When elements...
                foreach (ProjectWhenElement whenElement in chooseElement.WhenElements)
                {
                    if (IsConditionMatched(this.Configuration, this.Platform,
                        whenElement.Condition))
                    {
                        ICollection<ProjectPropertyGroupElement> propertyGroups =
                            whenElement.PropertyGroups;
                        if (propertyGroups != null && propertyGroups.Count != 0)
                        {
                            conditionedGroups.AddRange(propertyGroups);
                        }
                    }
                }

                // Get the property groups in the Otherwise element, if available...
                ProjectOtherwiseElement otherwiseElement = chooseElement.OtherwiseElement;
                if (otherwiseElement != null)
                {
                    ICollection<ProjectPropertyGroupElement> propertyGroups =
                        otherwiseElement.PropertyGroups;
                    if (propertyGroups != null && propertyGroups.Count != 0)
                    {
                        otherwiseGroups.AddRange(propertyGroups);
                    }
                }
            }

            bool isSuccessful = false;

            // We parse the actual properties in the "second pass" since the MSBuild
            // project is unstructured format...
            if (conditionedGroups != null && conditionedGroups.Count != 0)
            {
                foreach (ProjectPropertyGroupElement propertyGroup in conditionedGroups)
                {
                    if (this.ParseProperyGroup(propertyGroup))
                    {
                        // There is no need to parse multiple project groups with the
                        // same condition...
                        isSuccessful = true;
                        break;
                    }
                }
            }
            // If not successful, try searching the Otherwise property groups...
            if (!isSuccessful && (otherwiseGroups != null && otherwiseGroups.Count != 0))
            {
                foreach (ProjectPropertyGroupElement propertyGroup in otherwiseGroups)
                {
                    if (this.ParseProperyGroup(propertyGroup))
                    {
                        // There is no need to parse multiple project groups with the
                        // same condition...
                        isSuccessful = true;
                        break;
                    }
                }
            }

            return isSuccessful;
        }

        #endregion

        #region ParseReferenceItems Method

        protected virtual bool ParseReferenceItems(IList<ProjectInfo> referencedProjects)
        {
            ICollection<ProjectItemElement> allItems   = _project.Items;
            List<ProjectItemElement> references        = new List<ProjectItemElement>();
            List<ProjectItemElement> comReferences     = new List<ProjectItemElement>();
            List<ProjectItemElement> projectReferences = new List<ProjectItemElement>();

            foreach (ProjectItemElement item in allItems)
            {
                switch (item.ItemType)
                {
                    case "Reference":
                        references.Add(item);
                        break;
                    case "COMReference":
                        comReferences.Add(item);
                        break;
                    case "ProjectReference":
                        projectReferences.Add(item);
                        break;
                }
            }

            IList<string> referencedAssemblies      = this.ReferencedAssemblies;
            IList<string> referencedKnownAssemblies = this.ReferencedKnownAssemblies;
            foreach (ProjectItemElement item in references)
            {
                string referenceInclude = item.Include;
                if (referenceInclude.IndexOf(',') > 0)
                {
                    AssemblyName asmName = new AssemblyName(referenceInclude);

                    referenceInclude = asmName.Name;
                }

                string referencedAssembly = referenceInclude + ".dll";
                if (ProjectSectionFactory.IsKnownAssemblyName(referencedAssembly))
                {
                    referencedKnownAssemblies.Add(referencedAssembly);
                    continue;
                }

                if (item.HasMetadata)
                {
                    ProjectMetadataElement hintMetadata = null;

                    foreach (ProjectMetadataElement metadata in item.Metadata)
                    {
                        if (String.Equals(metadata.Name, "HintPath",
                            StringComparison.OrdinalIgnoreCase))
                        {
                            hintMetadata = metadata;
                            break;
                        }
                    }

                    if (hintMetadata != null)
                    {
                        string fullPath = Path.GetFullPath(
                            Path.Combine(this.ProjectDir, hintMetadata.Value));
                        if (File.Exists(fullPath))
                        {
                            referencedAssemblies.Add(fullPath);
                        }
                    }
                }
                else if (!String.IsNullOrEmpty(this.OutputType))
                {
                    // Trying some possibilities...
                    string extension = null;
                    switch (this.OutputType.ToLower())
                    {
                        case "dll":
                        case "library":
                        case "dynamiclibrary":
                            extension = ".dll";
                            break;
                        case "exe":
                        case "application":
                            extension = ".exe";
                            break;
                    }
                    if (!String.IsNullOrEmpty(extension))
                    {
                        // A referenced file in the project directory...
                        string fullPath = Path.GetFullPath(Path.Combine(this.ProjectDir,
                            referenceInclude + extension));
                        if (!File.Exists(fullPath) && (!String.IsNullOrEmpty(this.OutputPath)
                            && Directory.Exists(this.OutputPath))) 
                        {
                            // A referenced file in the output directory...
                            fullPath = Path.GetFullPath(Path.Combine(
                                this.OutputPath, referenceInclude + extension));
                        }
                        if (!String.IsNullOrEmpty(fullPath) && File.Exists(fullPath))
                        {
                            referencedAssemblies.Add(fullPath);
                        }
                    }
                }
            }

            foreach (ProjectItemElement item in comReferences)
            {
                //string referenceInclude = item.Include;
                //if (referenceInclude.IndexOf(',') > 0)
                //{
                //    AssemblyName asmName = new AssemblyName(referenceInclude);

                //    referenceInclude = asmName.Name;
                //}

                //if (item.HasMetadata)
                //{
                //    foreach (ProjectMetadataElement metadata in item.Metadata)
                //    {
                //    }
                //}
            }

            foreach (ProjectItemElement item in projectReferences)
            {
                string referenceInclude = item.Include;

                if (String.IsNullOrEmpty(referenceInclude) || !item.HasMetadata)
                {
                    continue;
                }

                string projectName = String.Empty;
                string projectGuid = String.Empty;
                foreach (ProjectMetadataElement metadata in item.Metadata)
                {
                    if (String.Equals(metadata.Name, "Project",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        projectGuid = metadata.Value;
                    }
                    else if (String.Equals(metadata.Name, "Name",
                        StringComparison.OrdinalIgnoreCase))
                    {
                        projectName = metadata.Value;
                    }
                }

                string projectPath = String.Empty;
                if (Path.IsPathRooted(referenceInclude))
                {
                    projectPath = Path.GetFullPath(referenceInclude);
                }
                else
                {
                    projectPath = Path.GetFullPath(
                       Path.Combine(this.ProjectDir, referenceInclude));
                }

                if (File.Exists(projectPath))
                {
                    if (String.IsNullOrEmpty(projectName))
                    {
                        projectName = Path.GetFileNameWithoutExtension(projectPath);
                    }

                    ProjectInfo projectInfo = new ProjectInfo(projectPath,
                        projectGuid, projectName);
                    if (projectInfo.IsValid)
                    {
                        referencedProjects.Add(projectInfo);
                    }
                }
                else
                {
                    if (!String.IsNullOrEmpty(projectGuid))
                    {
                        // If the path does not exist, we try looking through
                        // any available list of references...
                        ProjectInfo projectInfo = this.Context.GetProjectInfo(
                            projectGuid);
                        if (projectInfo != null && projectInfo.IsValid)
                        {
                            referencedProjects.Add(projectInfo);
                        }
                    }
                }
            }

            return true;
        }

        #endregion

        #region ExtractConditionMap Method

        /// <summary>
        /// Extracts a mapping of the condition names to their values.
        /// </summary>
        /// <param name="leftValue"></param>
        /// <param name="rightValue"></param>
        /// <returns></returns>
        protected static IDictionary<string, string> ExtractConditionMap(
            string leftValue, string rightValue)
        {
            Dictionary<string, string> expressionMap = new Dictionary<string, string>
                (StringComparer.OrdinalIgnoreCase);

            string[] leftValues = leftValue.Split('|');

            for (int i = 0; i < leftValues.Length; i++)
            {
                Match match = _propertyRegex.Match(leftValues[i]);
                if (match.Success)
                {
                    int num = rightValue.IndexOf('|');
                    string item;
                    if (num == -1 || i == leftValues.Length - 1)
                    {
                        item = rightValue;
                        i = leftValues.Length;
                    }
                    else
                    {
                        item = rightValue.Substring(0, num);
                        rightValue = rightValue.Substring(num + 1);
                    }
                    string key = match.Groups[1].ToString();

                    expressionMap[key] = item;
                }
            }

            return expressionMap;
        }

        #endregion

        #endregion
    }
}
