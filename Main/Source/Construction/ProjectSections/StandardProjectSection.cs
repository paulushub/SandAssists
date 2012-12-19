using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace Sandcastle.Construction.ProjectSections
{
    public sealed class StandardProjectSection : MsBuildProjectSection
    {
        #region Private Fields

        private StandardProjectType _standardType;

        #endregion

        #region Constructors and Destructor

        public StandardProjectSection()
        {   
            _standardType = StandardProjectType.None;
        }

        #endregion

        #region Public Properties

        public StandardProjectType StandardType
        {
            get
            {
                return _standardType;
            }
        }

        #endregion

        #region Public Methods

        public override bool Parse(ProjectSectionContext context, string projectFile)
        {
            _standardType = StandardProjectType.None;

            if (!base.Parse(context, projectFile))
            {
                return false;
            }

            _standardType = ProjectSectionFactory.GetStandardType(projectFile);

            bool isSuccessful = this.ParseProperties();
            if (!isSuccessful)
            {
                // If not successful, we try parsing "Choose" elements, since the ff.
                // is equally valid as conditioned property group....

                //<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
                //    <PropertyGroup>
                //        <Configuration Condition="'$(Configuration)' == ''">Debug</Configuration>
                //        <OutputType>Exe</OutputType>
                //        <RootNamespace>Application1</RootNamespace>
                //        <AssemblyName>Application1</AssemblyName>
                //        <WarningLevel>4</WarningLevel>
                //    </PropertyGroup>
                //    <Choose>
                //        <When Condition=" '$(Configuration)'=='debug' ">
                //            <PropertyGroup>
                //                <OutputPath>.\bin\Debug\</OutputPath>
                //            </PropertyGroup>
                //        </When>
                //        <When Condition=" '$(Configuration)'=='retail' ">
                //            <PropertyGroup>
                //                <OutputPath>.\bin\Release\</OutputPath>
                //            </PropertyGroup>
                //        </When>
                //        <Otherwise>
                //            <PropertyGroup>
                //                <OutputPath>.\bin\$(Configuration)\</OutputPath>
                //            </PropertyGroup>
                //        </Otherwise>
                //    </Choose>
                //</Project>

                isSuccessful = this.ParseChoose();
            }

            if (!isSuccessful)
            {
                return isSuccessful;
            }

            List<ProjectInfo> referencedProjects = new List<ProjectInfo>();
            isSuccessful = this.ParseReferenceItems(referencedProjects);

            this.CreateChildren(referencedProjects);

            return isSuccessful;
        }

        #endregion

        #region Protected Methods

        #region ParseProperties Method

        protected override bool ParseProperties()
        {
            // We will split the property groups into two...
            // a. global property groups, which are guided with a condition... 
            ICollection<ProjectPropertyGroupElement> propertyGroups =
                this.Project.PropertyGroups;
            // b. the conditioned property groups, which are normally guided by the
            //    configuration and the platform conditions...
            List<ProjectPropertyGroupElement> conditionedGroups =
                new List<ProjectPropertyGroupElement>();

            foreach (ProjectPropertyGroupElement propertyGroup in propertyGroups)
            {
                if (String.IsNullOrEmpty(propertyGroup.Condition))
                {
                    // For the global properties, extra the needed properties...
                    ICollection<ProjectPropertyElement> properties = propertyGroup.Properties;
                    foreach (ProjectPropertyElement property in properties)
                    {
                        switch (property.Name)
                        {
                            case "Platform":
                                this.Platform = property.Value;
                                break;
                            case "ProjectGuid":
                                this.ProjectGuid = property.Value;
                                break;
                            case "ProjectTypeGuids":
                                // Silverlight projects created by VS2008
                                // do not have the TargetFrameworkIdentifier
                                // property, so we look at the project type Guids
                                // Also, Portable Class Libraries do not have...
                                string tempText = property.Value;
                                if (!String.IsNullOrEmpty(tempText) && tempText.IndexOf(';') > 0)
                                {
                                    if (tempText.IndexOf("{A1591282-1198-4647-A2B1-27E5FF5F6F3B}",
                                        StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        // C#/VB.NET Silverlight projects...
                                        this.TargetFrameworkIdentifier = "Silverlight";
                                    }
                                    else if (tempText.IndexOf("{786C830F-07A1-408B-BD7F-6EE04809D6DB}",
                                        StringComparison.OrdinalIgnoreCase) >= 0)
                                    {                  
                                        // Portable C# projects...
                                        this.TargetFrameworkIdentifier = ".NETPortable";
                                    }
                                    else if (tempText.IndexOf("{14182A97-F7F0-4C62-8B27-98AA8AE2109A}",
                                        StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        // Portable VB.NET projects...
                                        this.TargetFrameworkIdentifier = ".NETPortable";
                                    }
                                    else if (tempText.IndexOf("{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}",
                                        StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        // All C# projects are .NET Framework...
                                        this.TargetFrameworkIdentifier = ".NETFramework";
                                    }
                                    else if (tempText.IndexOf("{F184B08F-C81C-45F6-A57F-5ABD9991F28F}",
                                        StringComparison.OrdinalIgnoreCase) >= 0)
                                    {
                                        // All VB.NET projects are .NET Framework...
                                        this.TargetFrameworkIdentifier = ".NETFramework";
                                    }
                                }
                                break;
                            case "OutputType":
                                this.OutputType = property.Value;
                                break;
                            case "AssemblyName":
                                this.AssemblyName = property.Value;
                                break;
                            case "Configuration":
                                this.Configuration = property.Value;
                                break;
                            case "TargetFrameworkVersion":
                                this.TargetFrameworkVersion = property.Value;
                                break;
                            case "TargetFrameworkIdentifier":
                                this.TargetFrameworkIdentifier = property.Value;
                                break;
                            case "PlatformFamilyName":
                                this.PlatformFamilyName = property.Value;
                                break;
                            case "ReferencePath":
                                string pathValue = property.Value;
                                if (!String.IsNullOrEmpty(pathValue))
                                {
                                    IList<string> referencePaths = this.ReferencedPaths;

                                    string[] pathValues = pathValue.Split(';');
                                    for (int k = 0; k < pathValues.Length; k++)
                                    {
                                        string referencePath = pathValues[k];
                                        if (!String.IsNullOrEmpty(referencePath))
                                        {
                                            referencePath = this.EvaluateMacros(referencePath);

                                            // Make sure it ends with backslash...
                                            if (!referencePath.EndsWith("\\"))
                                            {
                                                referencePath += "\\";
                                            }

                                            referencePaths.Add(referencePath);
                                        }
                                    }
                                }
                                break;
                        }
                    }
                }
                else if (IsConditionMatched(this.Configuration, this.Platform,
                    propertyGroup.Condition))
                {
                    conditionedGroups.Add(propertyGroup);
                }
            }

            // For the Compact Framework...
            string platformFamily = this.PlatformFamilyName;
            if (!String.IsNullOrEmpty(platformFamily))
            {
                if (platformFamily.Equals("PocketPC",
                    StringComparison.OrdinalIgnoreCase) ||
                    platformFamily.Equals("Smartphone",
                    StringComparison.OrdinalIgnoreCase) ||
                    platformFamily.Equals("WindowsCE",
                    StringComparison.OrdinalIgnoreCase))
                {
                    this.TargetFrameworkIdentifier = "Compact";
                }
            }

            if (String.IsNullOrEmpty(this.TargetFrameworkIdentifier))
            {
                bool isScriptSharp = false;

                // Script# is one poor project format, it leaves nothing
                // to reveal its target framework. We try looking for it...
                ICollection<ProjectImportElement> listImports = this.Project.Imports;
                if (listImports != null && listImports.Count != 0)
                {
                    foreach (ProjectImportElement import in listImports)
                    {
                        string project = import.Project;
                        if (!String.IsNullOrEmpty(project) &&
                            project.EndsWith("ScriptSharp.targets", StringComparison.OrdinalIgnoreCase))
                        {
                            isScriptSharp = true;
                            break;
                        }
                    }
                }

                if (isScriptSharp)
                {
                    this.TargetFrameworkIdentifier = "ScriptSharp";
                }
                else
                {
                    this.TargetFrameworkIdentifier = ".NETFramework";
                }
            }

            Debug.Assert(!String.IsNullOrEmpty(this.AssemblyName));

            if (String.IsNullOrEmpty(this.AssemblyName))
            {
                return false;
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

            return isSuccessful;
        }

        #endregion

        #region ParseProperyGroup Method

        protected override bool ParseProperyGroup(ProjectPropertyGroupElement propertyGroup)
        {
            if (propertyGroup == null || propertyGroup.Count == 0)
            {
                return false;
            }
            Debug.Assert(!String.IsNullOrEmpty(this.AssemblyName));

            if (String.IsNullOrEmpty(this.AssemblyName))
            {
                return false;
            }

            ProjectPropertyElement outputPathProperty        = null;
            ProjectPropertyElement constantsProperty         = null;
            ProjectPropertyElement documentationFileProperty = null;
            // Get the OutputPath property to determine the full path of the
            // the build output...
            ICollection<ProjectPropertyElement> properties = propertyGroup.Properties;
            foreach (ProjectPropertyElement property in properties)
            {
                if (String.Equals(property.Name, "OutputPath",
                    StringComparison.OrdinalIgnoreCase))
                {
                    outputPathProperty = property;
                }
                else if (String.Equals(property.Name, "DocumentationFile",
                    StringComparison.OrdinalIgnoreCase))
                {
                    documentationFileProperty = property;
                }
                else if (String.Equals(property.Name, "DefineConstants",
                    StringComparison.OrdinalIgnoreCase))
                {
                    constantsProperty = property;
                }
            }

            string tempValue = null;
            if (constantsProperty != null)
            {
                tempValue = constantsProperty.Value;
                if (!String.IsNullOrEmpty(tempValue))
                {   
                    // Script# only defines constants for C# projects...
                    if (tempValue.IndexOf("SCRIPTSHARP", 
                        StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        this.TargetFrameworkIdentifier = "ScriptSharp";
                    }
                    else if (tempValue.IndexOf("SILVERLIGHT", 
                        StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        this.TargetFrameworkIdentifier = "Silverlight";
                    }
                }
            }
            
            if (documentationFileProperty != null)
            {
                tempValue = documentationFileProperty.Value;
                if (!String.IsNullOrEmpty(tempValue))
                {
                    tempValue = this.EvaluateMacros(tempValue);

                    if (Path.IsPathRooted(tempValue))
                    {
                        this.CommentFile = Path.GetFullPath(tempValue);
                    }
                    else
                    {
                        this.CommentFile = Path.GetFullPath(Path.Combine(
                            this.ProjectDir, tempValue));
                    }
                }
            }

            if (outputPathProperty != null)
            {
                tempValue = outputPathProperty.Value;
                if (!String.IsNullOrEmpty(tempValue))
                {
                    tempValue = this.EvaluateMacros(tempValue);

                    if (Path.IsPathRooted(tempValue))
                    {
                        this.OutputPath = Path.GetFullPath(tempValue);
                    }
                    else
                    {
                        this.OutputPath = Path.GetFullPath(Path.Combine(
                            this.ProjectDir, tempValue));
                    }

                    switch (this.OutputType.ToLower())
                    {
                        case "library":
                        case "dynamiclibrary":
                            this.OutputFile = Path.GetFullPath(Path.Combine(
                                this.OutputPath, this.AssemblyName + ".dll"));
                            break;
                        case "exe":
                        case "application":
                            this.OutputFile = Path.GetFullPath(Path.Combine(
                                this.OutputPath, this.AssemblyName + ".exe"));
                            break;
                    }
                }

                // If the DocumentationFile property is not found for some reason,
                // we try looking for the comment in the same directory...
                if (documentationFileProperty == null ||
                    (String.IsNullOrEmpty(this.CommentFile) || !File.Exists(this.CommentFile)))
                {
                    if (File.Exists(this.OutputFile))
                    {
                        tempValue = Path.ChangeExtension(this.OutputFile, ".xml");
                        if (File.Exists(tempValue))
                        {
                            this.CommentFile = tempValue;
                        }
                    }
                }
            }

            //if (this.IsTarget)
            //{
            //    // It is an referenced project and is normally used as dependency.
            //    // For this, only the assembly file is required...
            //    return (!String.IsNullOrEmpty(this.OutputFile) && File.Exists(this.OutputFile));
            //}
            //else
            //{
            //    // It is an outer project, and must have both assembly and comment
            //    // to be valid...
            //    return ((!String.IsNullOrEmpty(this.OutputFile) && File.Exists(this.OutputFile))
            //        && (!String.IsNullOrEmpty(this.CommentFile) && File.Exists(this.CommentFile)));
            //}

            return (!String.IsNullOrEmpty(this.OutputFile) && File.Exists(this.OutputFile));
        }

        #endregion

        #endregion
    }
}
