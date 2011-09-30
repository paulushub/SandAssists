using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Construction.VcProjects;

namespace Sandcastle.Construction.ProjectSections
{
    /// <summary>
    /// <para>
    /// This creates a project section for the <c>Visual C++ .NET</c> created
    /// by the <c>VS.NET 2005</c> and <c>VS.NET 2008</c>.
    /// </para>
    /// <para>
    /// Even though, <c>VS.NET 2002</c> and <c>VS.NET 2003</c> are also supported,
    /// these do not support the <c>XML</c> documentations for .NET.
    /// </para>
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <c>Visual C++ .NET</c> created by <c>VS.NET 2005</c> and <c>VS.NET 2008</c>
    /// are in the <c>XML</c> format, but are not based on the <c>MSBuild</c>.
    /// </para>
    /// </remarks>
    public sealed class VcProjectSection : ProjectSection
    {
        #region Private Fields

        private Version _version;
        private VcProjectRootElement _project;

        #endregion

        #region Constructors and Destructor

        public VcProjectSection()
        {
        }

        #endregion

        #region Public Properties

        public Version Version
        {
            get
            {
                return _version;
            }
        }

        public VcProjectRootElement Project
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

            // The platform and configuration information are provided by the
            // user for the Visual C++ projects...
            this.Platform      = context.ActivePlatform;
            this.Configuration = context.ActiveConfiguration;

            // Open the project for parsing...
            _project = VcProjectRootElement.Open(projectFile);
            if (_project == null)
            {
                return false;
            }

            _version = new Version(_project.Version);

            string tempText = _project.TargetFrameworkVersion;
            if (String.IsNullOrEmpty(tempText))
            {
                // Project files for VC++ 2003/2005 do not have the TargetFrameworkVersion
                // attribute, so we use the project version information...
                switch (_version.Major)
                {
                    case 8: // VS.NET 2005 and supports only .NET 2.0
                        this.TargetFrameworkVersion = "v2.0";
                        break;
                    case 7: // VS.NET 2002 or 2003 and support .NET 1.0 or 1.1                       
                        this.TargetFrameworkVersion = (_version.Minor == 1) ?
                            "v1.1" : "v1.0"; // VC++ 2002 is not yet tested
                        break;
                    default:
                        throw new InvalidDataException(String.Format(
                            "The target framework version '{0}' is not known.", tempText));
                }
            }
            else
            {   
                //TODO: Still could not find any reference for these version
                //      numbers, using the known values...
                switch (tempText.Trim())
                {
                    case "196613":
                        this.TargetFrameworkVersion = "v3.5";
                        break;
                    case "196608":
                        this.TargetFrameworkVersion = "v3.0";
                        break;
                    case "131072":
                        this.TargetFrameworkVersion = "v2.0";
                        break;
                    default:
                        throw new InvalidDataException(String.Format(
                            "The target framework version '{0}' is not known.", tempText));
                }
            }   

            string targetIdentifer = _project["Keyword"];
            if (String.Equals(targetIdentifer, "ManagedCProj", 
                StringComparison.OrdinalIgnoreCase))
            {
                this.TargetFrameworkIdentifier = ".NETFramework";
            }
            else
            {
                this.TargetFrameworkIdentifier = targetIdentifer;
            }
            this.ProjectName = _project.Name;
            this.ProjectGuid = _project.ProjectGUID;

            bool isSuccessful = this.ValidatePlatforms();
            if (!isSuccessful)
            {
                throw new InvalidOperationException(
                    "The specified platform is not available.");
            }

            isSuccessful = this.ParseConfigurations();

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

        #region Private Methods

        #region ValidatePlatform Method

        private bool ValidatePlatforms()
        {
            bool isSuccessful = false;
            string platformName = this.Platform;
            if (String.IsNullOrEmpty(platformName))
            {
                return isSuccessful;
            }
            // Mixed Platforms is not written in the project file...
            if (platformName.Equals("MixedPlatforms", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            foreach (VcProjectPlatformsElement platforms in _project.Platforms)
            {
                foreach (VcProjectPlatformElement platform in platforms.Platforms)
                {
                    if (String.Equals(platformName, platform.Name, 
                        StringComparison.OrdinalIgnoreCase))
                    {
                        isSuccessful = true;
                        break;
                    }
                }
            }

            return isSuccessful;
        }

        #endregion

        #region ParseConfigurations Methods

        private bool ParseConfigurations()
        {
            // This is the matching target configuration, must match the
            // specified configuration and platform names...
            VcProjectConfigurationElement targetConfig = null;
            List<VcProjectConfigurationElement> partConfigs = 
                new List<VcProjectConfigurationElement>();

            string configName   = this.Configuration;
            string platformName = this.Platform;

            foreach (VcProjectConfigurationsElement configurations in _project.Configurations)
            {
                foreach (VcProjectConfigurationElement configuration in configurations.Configurations)
                {
                    string name = configuration.Name;
                    if (!String.IsNullOrEmpty(name))
                    {
                        string[] values = name.Split('|');

                        if (values != null && values.Length == 2)
                        {
                            string configValue   = values[0].Trim();
                            string platformValue = values[1].Trim();

                            if (String.Equals(platformName, platformValue,
                                StringComparison.OrdinalIgnoreCase) &&
                                String.Equals(configName, configValue,
                                StringComparison.OrdinalIgnoreCase))
                            {
                                targetConfig = configuration;
                                break;
                            }  
                            else if (String.Equals(configName, configValue,
                                StringComparison.OrdinalIgnoreCase))
                            {
                                // If only the configuration is matched, we
                                // will still keep it...
                                partConfigs.Add(configuration);
                            }
                        }
                    }
                }
            }

            // We give this provision to match only the configuration and not
            // necessarily the platform...
            if (targetConfig == null && partConfigs.Count > 0)
            {
                targetConfig = partConfigs[0];
            }

            Debug.Assert(targetConfig != null);
            if (targetConfig == null)
            {
                return false;
            }

            string outputFile = null;
            // The configuration type determines the output type
            int outputType = Int32.Parse(targetConfig.ConfigurationType);
            switch (outputType)
            {
                case 1:
                    this.OutputType = "Application";
                    // set the default...
                    outputFile = @"$(OutDir)\$(ProjectName).exe";
                    break;
                case 2:
                    this.OutputType = "Library";
                    // set the default...
                    outputFile = @"$(OutDir)\$(ProjectName).dll";
                    break;
            }
            // The matching configuration is found, we parse it...
            string outputDir = targetConfig.OutputDirectory;
            if (String.IsNullOrEmpty(outputDir))
            {
                // If not found, we simply use the default...
                outputDir = "$(SolutionDir)$(ConfigurationName)";
            }
            outputDir = this.EvaluateMacros(outputDir);
            if (!String.IsNullOrEmpty(outputDir))
            {
                if (Path.IsPathRooted(outputDir))
                {
                    outputDir = Path.GetFullPath(outputDir);
                }
                else
                {
                    outputDir = Path.GetFullPath(Path.Combine(
                        this.ProjectDir, outputDir));
                }
            }
            if (String.IsNullOrEmpty(outputDir) || !Directory.Exists(outputDir))
            {
                return false;
            }
            this.OutputPath = outputDir;

            VcProjectToolElement linkerTool     = null;
            VcProjectToolElement xdcMakeTool    = null;
            VcProjectToolElement clCompilerTool = null;

            foreach (VcProjectToolElement tool in targetConfig.Tools)
            {
                // We are mainly interested in the compiler and documentation
                // tools, as these generate the required output...
                switch (tool.Name)
                {
                    case "VCCLCompilerTool":
                        clCompilerTool = tool;
                        break;
                    case "VCXDCMakeTool":
                        xdcMakeTool = tool;
                        break;
                    case "VCLinkerTool":
                        linkerTool = tool;
                        break;
                }
            }

            Debug.Assert(linkerTool != null);
            Debug.Assert(xdcMakeTool != null);
            Debug.Assert(clCompilerTool != null);    
            // Both tools should normally be present, even if the XDC tool is not
            // configured or has default output...
            if (clCompilerTool == null || xdcMakeTool == null || linkerTool == null)
            {
                return false;
            }

            // Process the output file...
            string tempText = linkerTool["OutputFile"];
            if (!String.IsNullOrEmpty(tempText))
            {
                outputFile = tempText;
            }
            outputFile = this.EvaluateMacros(outputFile);
            if (String.IsNullOrEmpty(outputFile))
            {
                return false;
            }
            if (Path.IsPathRooted(outputFile))
            {
                outputFile = Path.GetFullPath(outputFile);
            }
            else
            {
                outputFile = Path.GetFullPath(Path.Combine(
                    this.ProjectDir, outputFile));
            }
            this.OutputFile   = outputFile;
            this.AssemblyName = Path.GetFileNameWithoutExtension(outputFile);

            // We need a confirmation that documentation is generated...
            tempText = clCompilerTool["GenerateXMLDocumentationFiles"];
            if (!String.IsNullOrEmpty(tempText) && Convert.ToBoolean(tempText))
            {
                string documentFile = @"$(TargetDir)$(TargetName).xml";
                tempText = xdcMakeTool["OutputDocumentFile"];
                if (!String.IsNullOrEmpty(tempText))
                {
                    documentFile = tempText;
                }

                documentFile = this.EvaluateMacros(documentFile);
                if (!String.IsNullOrEmpty(documentFile))
                {
                    if (Path.IsPathRooted(documentFile))
                    {
                        documentFile = Path.GetFullPath(documentFile);
                    }
                    else
                    {
                        outputFile = Path.GetFullPath(Path.Combine(
                            this.ProjectDir, documentFile));
                    }
                }
                if (File.Exists(documentFile))
                {
                    this.CommentFile = documentFile;
                }

                // If the DocumentationFile property is not found for some reason,
                // we try looking for the comment in the same directory...
                if ((String.IsNullOrEmpty(this.CommentFile) || !File.Exists(this.CommentFile)))
                {
                    if (File.Exists(this.OutputFile))
                    {
                        tempText = Path.ChangeExtension(this.OutputFile, ".xml");
                        if (File.Exists(tempText))
                        {
                            this.CommentFile = tempText;
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

        #region ParseReferenceItems Method

        private bool ParseReferenceItems(IList<ProjectInfo> referencedProjects)
        {
            bool isSuccessful = true;

            string projectDir = this.ProjectDir;

            IList<string> referencedAssemblies = this.ReferencedAssemblies;
            IList<string> referencedKnownAssemblies = this.ReferencedKnownAssemblies;

            foreach (VcProjectReferencesElement references in _project.References)
            {
                // For the referenced .NET assemblies...
                foreach (VcProjectAssemblyReferenceElement reference 
                    in references.AssemblyReferences)
                {
                    string assemblyPath = this.EvaluateMacros(reference.RelativePath);
                    if (String.IsNullOrEmpty(assemblyPath))
                    {
                        // TODO: This property is actually required and will 
                        //       not be empty or null...
                        continue;
                    }
                    // For a known assembly (.NET Framework assembly)...
                    string assemblyName = Path.GetFileName(assemblyPath);
                    if (ProjectSectionFactory.IsKnownAssemblyName(assemblyName))
                    {
                        referencedKnownAssemblies.Add(assemblyName);
                        continue;
                    }

                    if (Path.IsPathRooted(assemblyPath))
                    {
                        assemblyPath = Path.GetFullPath(assemblyPath);
                    }
                    else
                    {
                        assemblyPath = Path.GetFullPath(Path.Combine(
                            projectDir, assemblyPath));
                    }

                    if (File.Exists(assemblyPath))
                    {
                        referencedAssemblies.Add(assemblyPath);
                    }
                }

                // For the referenced projects...
                foreach (VcProjectProjectReferenceElement reference 
                    in references.ProjectReferences)
                {
                    string projectGuid = reference.ReferencedProjectIdentifier;
                    if (String.IsNullOrEmpty(projectGuid))
                    {
                        throw new InvalidDataException(
                            "The project is not well-formed, it has project Guid.");
                    }

                    string projectPath = this.EvaluateMacros(reference.RelativePathToProject);
                    if (!String.IsNullOrEmpty(projectPath))
                    {
                        if (Path.IsPathRooted(projectPath))
                        {
                            projectPath = Path.GetFullPath(projectPath);
                        }
                        else
                        {
                            projectPath = Path.GetFullPath(Path.Combine(
                                projectDir, projectPath));
                        }
                    }

                    if (File.Exists(projectPath))
                    {
                        string projectName = reference.Name;
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

                foreach (VcProjectActiveXReferenceElement reference 
                    in references.ActiveXReferences)
                {
                }
            }

            return isSuccessful;
        }

        #endregion

        #endregion
    }
}
