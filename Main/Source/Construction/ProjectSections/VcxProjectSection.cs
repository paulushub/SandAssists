using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Construction.Evaluation;

namespace Sandcastle.Construction.ProjectSections
{
    /// <summary>
    /// <para>
    /// This creates a project section for the new <c>Visual C++</c> project file
    /// format in the <c>VS.NET 2010</c>. Is is in the <c>MSBuild</c> format.
    /// </para>
    /// <para>
    /// The project file extension is <c>.vcxproj</c>.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The MSBuild format for the <c>Visual C++</c> is different from other
    /// <c>.NET</c> language project files. Both the <c>.NET</c> and standard
    /// <c>C/C++</c> use the same project file format as in previous versions of
    /// the <c>VS.NET</c>.
    /// </para>
    /// <para>
    /// The following is the layout of the format.
    /// </para>
    /// <code lang="xml">
    /// <![CDATA[
    /// <Project DefaultTargets="Build" ToolsVersion="4.0" 
    ///   xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
    ///     <ItemGroup Label="ProjectConfigurations" />
    ///     <PropertyGroup Label="Globals" />
    ///     <Import Project="$(VCTargetsPath)\Microsoft.Cpp.default.props" />
    ///     <PropertyGroup Label="Configuration" />
    ///     <Import Project="$(VCTargetsPath)\Microsoft.Cpp.props" />
    ///     <ImportGroup Label="ExtensionSettings" />
    ///     <ImportGroup Label="PropertySheets" />
    ///     <PropertyGroup Label="UserMacros" />
    ///     <PropertyGroup />
    ///     <ItemDefinitionGroup />
    ///     <ItemGroup />
    ///     <Import Project="$(VCTargetsPath)\Microsoft.Cpp.targets" />
    ///     <ImportGroup Label="ExtensionTargets" />
    /// </Project>    
    /// ]]>
    /// </code>
    /// </remarks>
    public sealed class VcxProjectSection : MsBuildProjectSection
    {
        #region Private Fields

        #endregion

        #region Constructors and Destructor

        public VcxProjectSection()
        {
        }

        #endregion

        #region Public Properties

        #endregion

        #region Public Methods

        public override bool Parse(ProjectSectionContext context, string projectFile)
        {
            if (!base.Parse(context, projectFile))
            {
                return false;
            }

            // The platform and configuration information are provided by the
            // user for the Visual C++ projects...
            this.Platform      = context.ActivePlatform;
            this.Configuration = context.ActiveConfiguration;

            bool isSuccessful = this.ValidateConfigurations();
            if (!isSuccessful)
            {
                return false;
            }

            isSuccessful = this.ParseProperties();                
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
                isSuccessful = this.ParseDefaults();  
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
                            case "OutputType":
                            case "ConfigurationType":
                                this.OutputType = property.Value;
                                break;
                            case "AssemblyName":
                            case "TargetName":
                                this.AssemblyName = this.EvaluateMacros(property.Value);
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
                            case "Keyword":
                                if (String.IsNullOrEmpty(this.TargetFrameworkIdentifier))
                                {
                                    string targetIdentifer = property.Value;
                                    if (String.Equals(targetIdentifer, "ManagedCProj",
                                        StringComparison.OrdinalIgnoreCase))
                                    {
                                        this.TargetFrameworkIdentifier = ".NETFramework";
                                    }
                                    else
                                    {
                                        this.TargetFrameworkIdentifier = targetIdentifer;
                                    }
                                }
                                break;
                        }
                    }
                }
                else if (IsConditionMatched(this.Configuration, this.Platform,
                    propertyGroup.Condition))
                {                       
                    ICollection<ProjectPropertyElement> properties = propertyGroup.Properties;
                    // We parse this to look for TargetName in particular...
                    foreach (ProjectPropertyElement property in properties)
                    {
                        switch (property.Name)
                        {
                            case "Platform":
                                this.Platform = property.Value;
                                break;
                            case "OutputType":
                            case "ConfigurationType":
                                this.OutputType = property.Value;
                                break;
                            case "AssemblyName":
                            case "TargetName":
                                this.AssemblyName = this.EvaluateMacros(property.Value);
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
                        }
                    }          

                    conditionedGroups.Add(propertyGroup);
                }
            }

            if (String.IsNullOrEmpty(this.AssemblyName))
            {
                // The default is the project name...
                this.AssemblyName = Path.GetFileNameWithoutExtension(this.ProjectFile);
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
                foreach (ProjectPropertyGroupElement group in conditionedGroups)
                {
                    if (this.ParseProperyGroup(group))
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

            ProjectPropertyElement outputPathProperty = null;
            // Get the OutputPath property to determine the full path of the
            // the build output...
            ICollection<ProjectPropertyElement> properties = propertyGroup.Properties;
            foreach (ProjectPropertyElement property in properties)
            {
                if (String.Equals(property.Name, "OutDir",
                    StringComparison.OrdinalIgnoreCase))
                {
                    outputPathProperty = property;
                    break;
                }
            }

            if (outputPathProperty != null)
            {
                string tempValue = this.EvaluateMacros(outputPathProperty.Value);
                if (!String.IsNullOrEmpty(tempValue))
                {
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
                if (String.IsNullOrEmpty(this.CommentFile) || !File.Exists(this.CommentFile))
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

        #region Private Methods

        #region ValidateConfigurations Method

        private bool ValidateConfigurations()
        {
            // The expected format:
            //<ItemGroup Label="ProjectConfigurations">
            //  <ProjectConfiguration Include="Debug|Win32">
            //    <Configuration>Debug</Configuration>
            //    <Platform>Win32</Platform>
            //  </ProjectConfiguration>
            //  <ProjectConfiguration Include="Release|Win32">
            //    <Configuration>Release</Configuration>
            //    <Platform>Win32</Platform>
            //  </ProjectConfiguration>
            //</ItemGroup>

            ProjectItemGroupElement itemGroup = null;
            foreach (ProjectElement element in this.Project.Children)
            {
                if (element.ElementType == ProjectElementType.ItemGroup &&
                    String.Equals(element.Label, "ProjectConfigurations", 
                    StringComparison.OrdinalIgnoreCase))
                {
                    itemGroup = (ProjectItemGroupElement)element;
                    break;
                }
            }
            if (itemGroup != null)
            {
                ProjectItemElement itemElement = null;
                foreach (ProjectItemElement item in itemGroup.Items)
                {
                    string include = item.Include;
                    if (!String.IsNullOrEmpty(include) &&
                        include.IndexOf(this.Configuration, StringComparison.OrdinalIgnoreCase) >= 0 &&
                        include.IndexOf(this.Platform, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        itemElement = item;
                        break;
                    }
                }

                if (itemElement != null && itemElement.HasMetadata)
                {
                    bool configMatched   = false;
                    bool platformMatched = String.Equals(this.Platform, 
                        "MixedPlatforms", StringComparison.OrdinalIgnoreCase);

                    foreach (ProjectMetadataElement metadata in itemElement.Metadata)
                    {   
                        switch (metadata.Name)
                        {
                            case "Configuration":
                                if (String.Equals(metadata.Value, this.Configuration, 
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    configMatched = true;
                                }
                                break;
                            case "Platform":
                                if (String.Equals(metadata.Value, this.Platform, 
                                    StringComparison.OrdinalIgnoreCase))
                                {
                                    platformMatched = true;
                                }
                                break;
                        }
                    }

                    return (configMatched && platformMatched);
                }
            }

            return false;
        }

        #endregion

        #region ParseDefaults Method

        private bool ParseDefaults()
        {
            if (!String.IsNullOrEmpty(this.OutputFile) &&
                File.Exists(this.OutputFile))
            {
                // Possibility: The documentation file is explicitly defined

                //<ItemDefinitionGroup Condition="'$(Configuration)|$(Platform)'=='Debug|Win32'">
                //  <ClCompile/>
                //  </ClCompile>
                //  <Link/>
                //  <Xdcmake>
                //    <OutputFile>Debug\TestingCPP3.xml</OutputFile>
                //  </Xdcmake>
                //</ItemDefinitionGroup>   

                ICollection<ProjectItemDefinitionGroupElement> itemDefGroups =
                    this.Project.ItemDefinitionGroups;

                foreach (ProjectItemDefinitionGroupElement itemDefGroup
                    in itemDefGroups)
                {
                    if (IsConditionMatched(this.Configuration, this.Platform,
                        itemDefGroup.Condition))
                    {
                        bool isFound = false;

                        foreach (ProjectItemDefinitionElement itemDef
                            in itemDefGroup.ItemDefinitions)
                        {
                            if (String.Equals(itemDef.ItemType, "Xdcmake",
                                StringComparison.OrdinalIgnoreCase))
                            {
                                foreach (ProjectMetadataElement metadata
                                    in itemDef.Metadata)
                                {
                                    if (!String.Equals(metadata.Name, "OutputFile",
                                       StringComparison.OrdinalIgnoreCase))
                                    {
                                        continue;
                                    }

                                    string tempValue = metadata.Value;
                                    if (String.IsNullOrEmpty(tempValue))
                                    {
                                        continue;
                                    }
                                    tempValue = this.EvaluateMacros(tempValue);

                                    if (Path.IsPathRooted(tempValue))
                                    {
                                        this.CommentFile = Path.GetFullPath(tempValue);
                                    }
                                    else
                                    {
                                        this.CommentFile = Path.GetFullPath(
                                            Path.Combine(this.ProjectDir, tempValue));
                                    }

                                    isFound = true;
                                    break;
                                }
                            }

                            if (isFound)
                            {
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                // Possibility: Default output directories are used...
                // $(SolutionDir)$(Configuration)\$(ProjectName) 

                ProjectSectionContext context = this.Context;
                string workingDir = Path.Combine(context.SolutionDir, 
                    this.Configuration);

                if (Directory.Exists(workingDir))
                {
                    if (String.IsNullOrEmpty(this.OutputPath))
                    {
                        this.OutputPath = workingDir;
                    }

                    switch (this.OutputType.ToLower())
                    {
                        case "library":
                        case "dynamiclibrary":
                            this.OutputFile = Path.GetFullPath(Path.Combine(
                                workingDir, this.AssemblyName + ".dll"));
                            break;
                        case "exe":
                        case "application":
                            this.OutputFile = Path.GetFullPath(Path.Combine(
                                workingDir, this.AssemblyName + ".exe"));
                            break;
                    }

                    if (String.IsNullOrEmpty(this.CommentFile) || !File.Exists(this.CommentFile))
                    { 
                        this.CommentFile = Path.GetFullPath(Path.Combine(
                            workingDir, this.AssemblyName + ".xml"));
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
