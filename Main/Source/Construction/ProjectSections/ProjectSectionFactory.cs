using System;
using System.IO;
using System.Xml;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

using Sandcastle.Construction;
using Sandcastle.Construction.Utils;
using Sandcastle.Construction.Evaluation;

namespace Sandcastle.Construction.ProjectSections
{
    public static class ProjectSectionFactory
    {
        #region Private Static Fields

        /// <summary>
        /// A list of well-known GAC installed .NET Framework assemblies, 
        /// cached for quick access.
        /// </summary>
        private static readonly HashSet<string> _knownAssemblyNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            { "Accessibility.dll" },
            { "CustomMarshalers.dll" },
            { "IEExecRemote.dll" },
            { "IEHost.dll" },
            { "IIEHost.dll" },
            { "ISymWrapper.dll" },
            { "Microsoft.Build.dll" },
            { "Microsoft.Build.Conversion.dll" },
            { "Microsoft.Build.Conversion.v3.5.dll" },
            { "Microsoft.Build.Conversion.v4.0.dll" },
            { "Microsoft.Build.CPPTasks.Common.dll" },
            { "Microsoft.Build.CPPTasks.Itanium.dll" },
            { "Microsoft.Build.CPPTasks.Win32.dll" },
            { "Microsoft.Build.CPPTasks.x64.dll" },
            { "Microsoft.Build.Engine.dll" },
            { "Microsoft.Build.Framework.dll" },
            { "Microsoft.Build.Tasks.dll" },
            { "Microsoft.Build.Tasks.v3.5.dll" },
            { "Microsoft.Build.Tasks.v4.0.dll" },
            { "Microsoft.Build.Utilities.dll" },
            { "Microsoft.Build.Utilities.v3.5.dll" },
            { "Microsoft.Build.Utilities.v4.0.dll" },
            { "Microsoft.Build.VisualJSharp.dll" },
            { "Microsoft.JScript.dll" },
            { "Microsoft.VisualBasic.dll" },
            { "Microsoft.VisualC.dll" },
            { "Microsoft.VisualC.STLCLR.dll" },
            { "Mscorlib.dll" },
            { "PresentationBuildTasks.dll" },
            { "PresentationCFFRasterizer.dll" },
            { "PresentationCore.dll" },
            { "PresentationFramework.dll" },
            { "PresentationFramework.Aero.dll" },
            { "PresentationFramework.Classic.dll" },
            { "PresentationFramework.Luna.dll" },
            { "PresentationFramework.Royale.dll" },
            { "PresentationUI.dll" },
            { "ReachFramework.dll" },
            { "System.dll" },
            { "System.AddIn.dll" },
            { "System.AddIn.Contract.dll" },
            { "System.ComponentModel.Composition.dll" },
            { "System.ComponentModel.DataAnnotations.dll" },
            { "System.Configuration.dll" },
            { "System.Configuration.Install.dll" },
            { "System.Core.dll" },
            { "System.Data.dll" },
            { "System.Data.DataSetExtensions.dll" },
            { "System.Data.Entity.dll" },
            { "System.Data.Entity.Design.dll" },
            { "System.Data.Linq.dll" },
            { "System.Data.OracleClient.dll" },
            { "System.Data.Services.dll" },
            { "System.Data.Services.Client.dll" },
            { "System.Data.Services.Design.dll" },
            { "System.Data.SqlXml.dll" },
            { "System.Deployment.dll" },
            { "System.Design.dll" },
            { "System.DirectoryServices.dll" },
            { "System.DirectoryServices.AccountManagement.dll" },
            { "System.DirectoryServices.Protocols.dll" },
            { "System.Drawing.dll" },
            { "System.Drawing.Design.dll" },
            { "System.EnterpriseServices.dll" },
            { "System.IdentityModel.dll" },
            { "System.IdentityModel.Selectors.dll" },
            { "System.IO.Log.dll" },
            { "System.Management.Instrumentation.dll" },
            { "System.Net.dll" },
            { "System.Printing.dll" },
            { "System.Runtime.dll" },
            { "System.Runtime.Remoting.dll" },
            { "System.Runtime.Serialization.dll" },
            { "System.Runtime.Serialization.Formatters.Soap.dll" },
            { "System.Security.dll" },
            { "System.ServiceModel.dll" },
            { "System.ServiceModel.Channels.dll" },
            { "System.ServiceModel.Web.dll" },
            { "System.ServiceProcess.dll" },
            { "System.Speech.dll" },
            { "System.Transactions.dll" },
            { "System.Web.dll" },
            { "System.Web.Abstractions.dll" },
            { "System.Web.DynamicData.dll" },
            { "System.Web.DynamicData.Design.dll" },
            { "System.Web.Entity.dll" },
            { "System.Web.Entity.Design.dll" },
            { "System.Web.Extensions.dll" },
            { "System.Web.Extensions.Design.dll" },
            { "System.Web.Mobile.dll" },
            { "System.Web.RegularExpressions.dll" },
            { "System.Web.Routing.dll" },
            { "System.Web.Services.dll" },
            { "System.Windows.dll" },
            { "System.Windows.Browser.dll" },
            { "System.Windows.Forms.dll" },
            { "System.Windows.Presentation.dll" },
            { "System.Workflow.Activities.dll" },
            { "System.Workflow.ComponentModel.dll" },
            { "System.Workflow.Runtime.dll" },
            { "System.WorkflowServices.dll" },
            { "System.Xaml.dll" },
            { "System.Xaml.Hosting.dll" },
            { "System.Xml.dll" },
            { "System.Xml.Linq.dll" },
            { "UIAutomationClient.dll" },
            { "UIAutomationClientsideProviders.dll" },
            { "UIAutomationProvider.dll" },
            { "UIAutomationTypes.dll" },
            { "WindowsBase.dll" },
            { "WindowsFormsIntegration.dll" },
        };

        private static readonly Regex _projectLineRx = new Regex(
           "^Project\\(\"(?<ProjectTypeGuid>.*)\"\\)\\s*=\\s*\"(?<ProjectName>.*)\"\\s*,\\s*\"(?<ProjectPath>.*)\"\\s*,\\s*\"(?<ProjectGuid>.*)\"$", RegexOptions.Compiled);

        #endregion

        #region Public Methods

        #region CreateSection Methods

        public static ProjectSection CreateSection(string projectFile, 
            string solutionFile, string platform, string configuration)
        {
            ProjectSectionContext context = new ProjectSectionContext();
            context.SolutionFile  = solutionFile;
            context.Platform      = platform;
            context.Configuration = configuration;

            if (String.IsNullOrEmpty(projectFile))
            {
                return null;
            }
            projectFile = Path.GetFullPath(
                Environment.ExpandEnvironmentVariables(projectFile));

            string fileExt = Path.GetExtension(projectFile);
            if (String.IsNullOrEmpty(fileExt))
            {
                return null;
            }

            return CreateSection(context, projectFile);
        }

        public static ProjectSection CreateSection(ProjectSectionContext context, 
            ProjectInfo projectInfo)
        {
            if (context != null)
            {
                ProjectSection projectSection = context.GetProjectSection(
                    projectInfo.ProjectGuid);

                if (projectSection != null)
                {
                    return projectSection;
                }
            }

            return CreateSection(context, projectInfo.ProjectPath);
        }

        public static ProjectSection CreateSection(ProjectSectionContext context, 
            string projectFile)
        {
            if (String.IsNullOrEmpty(projectFile))
            {
                return null;
            }
            projectFile = Path.GetFullPath(
                Environment.ExpandEnvironmentVariables(projectFile));

            string fileExt = Path.GetExtension(projectFile);
            if (String.IsNullOrEmpty(fileExt))
            {
                return null;
            }

            ProjectSection projectSection = null;

            switch (fileExt.ToLower())
            {
                case ".csproj":
                case ".vbproj":
                case ".fsproj":
                case ".pyproj":
                case ".rbproj":
                case ".vjsproj":
                    projectSection = new StandardProjectSection();
                    projectSection.Parse(context, projectFile);
                    break;
                case ".vcproj":
                    // For the Visual C++ project, these are required...
                    RequiresAll(context.SolutionFile, context.Platform,
                        context.Configuration);

                    projectSection = new VcProjectSection();
                    projectSection.Parse(context, projectFile);
                    break;
                case ".vcxproj":
                    // For the Visual C++ project, these are required...
                    RequiresAll(context.SolutionFile, context.Platform,
                        context.Configuration);

                    projectSection = new VcxProjectSection();
                    projectSection.Parse(context, projectFile);
                    break;
            }

            return projectSection;
        }

        #endregion

        #region CreateSections Methods

        public static IList<ProjectSection> CreateSections(string sourceFile)
        {
            return CreateSections(sourceFile, String.Empty, String.Empty);
        }

        public static IList<ProjectSection> CreateSections(string sourceFile,
            string platform, string configuration)
        {
            return CreateSections(sourceFile, platform, configuration, null);
        }

        public static IList<ProjectSection> CreateSections(string sourceFile,
            string platform, string configuration, HashSet<string> targetProjectGuids)
        {
            sourceFile = Path.GetFullPath(
                Environment.ExpandEnvironmentVariables(sourceFile));

            string fileExt = Path.GetExtension(sourceFile);
            if (String.IsNullOrEmpty(fileExt))
            {
                return null;
            }

            ProjectSectionContext context = new ProjectSectionContext();
            context.Platform      = platform;
            context.Configuration = configuration;

            if (targetProjectGuids != null && targetProjectGuids.Count != 0)
            {
                context.RegisterTargets(targetProjectGuids);
            }

            List<ProjectSection> projectSections = new List<ProjectSection>();

            if (fileExt.Equals(".sln", StringComparison.OrdinalIgnoreCase))
            {
                context.SolutionFile = sourceFile;

                IList<ProjectInfo> listProjectInfo = GetProjectInfo(sourceFile);

                if (listProjectInfo != null && listProjectInfo.Count != 0)
                {
                    // Register projects for reference resolution...
                    context.RegisterInfo(listProjectInfo);

                    // If we are documenting all, then add the targets...
                    if (targetProjectGuids == null || targetProjectGuids.Count == 0)
                    {   
                        for (int i = 0; i < listProjectInfo.Count; i++)
                        {
                            ProjectInfo projectInfo = listProjectInfo[i];

                            if (projectInfo != null && projectInfo.IsValid)
                            {
                                context.RegisterTarget(projectInfo.ProjectGuid);
                            }
                        }    
                    }

                    for (int i = 0; i < listProjectInfo.Count; i++)
                    {
                        ProjectInfo projectInfo = listProjectInfo[i];

                        if (projectInfo != null && projectInfo.IsValid &&
                            context.IsTarget(projectInfo.ProjectGuid))
                        {
                            ProjectSection projectSection = context.GetProjectSection(
                                projectInfo.ProjectGuid);
                            if (projectSection == null)
                            {
                                projectSection = CreateSection(context, projectInfo);
                            }

                            if (projectSection != null)
                            {
                                projectSections.Add(projectSection);
                                context.RegisterSection(projectSection);
                            }
                       }
                    }
                }
            }
            else
            {
                ProjectSection projectSection = CreateSection(context, sourceFile);

                if (projectSection != null)
                {
                    projectSections.Add(projectSection);
                    context.RegisterSection(projectSection);
                }
            }

            return projectSections;
        }

        #endregion

        #region Other Methods

        public static bool IsKnownAssemblyName(string assemblyName)
        {
            if (String.IsNullOrEmpty(assemblyName))
            {
                return false;
            }
            if (!assemblyName.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
            {
                assemblyName += ".dll";
            }

            return _knownAssemblyNames.Contains(assemblyName);
        }

        public static StandardProjectType GetStandardType(string projectFile)
        {
            StandardProjectType projectType = StandardProjectType.None;

            if (String.IsNullOrEmpty(projectFile))
            {
                return projectType;
            }

            string fileExt = Path.GetExtension(projectFile);
            if (String.IsNullOrEmpty(fileExt))
            {
                return projectType;
            }

            switch (fileExt.ToLower())
            {
                case ".csproj":
                    projectType = StandardProjectType.CsProj;
                    break;
                case ".vbproj":
                    projectType = StandardProjectType.VbProj;
                    break;
                case ".fsproj":
                    projectType = StandardProjectType.FsProj;
                    break;
                case ".pyproj":
                    projectType = StandardProjectType.PyProj;
                    break;
                case ".rbproj":
                    projectType = StandardProjectType.RbProj;
                    break;
                case ".vjsproj":
                    projectType = StandardProjectType.VjsProj;
                    break;
            }

            return projectType;
        }

        public static IList<ProjectInfo> GetProjectInfo(string solutionPath)
        {
            List<ProjectInfo> listProjectInfo = new List<ProjectInfo>();

            if (String.IsNullOrEmpty(solutionPath) || !File.Exists(solutionPath))
            {
                return listProjectInfo;
            }

            string solutionDir = Path.GetDirectoryName(solutionPath);

            if (!solutionDir.EndsWith("\\", StringComparison.Ordinal))
            {
                solutionDir += "\\";
            }

            using (StreamReader reader = File.OpenText(solutionPath))
            {
                while (!reader.EndOfStream)
                {
                    string lineText = reader.ReadLine();

                    if (!String.IsNullOrEmpty(lineText))
                    {
                        Match match = _projectLineRx.Match(lineText.Trim());
                        if (match != null && match.Success)
                        {
                            string projectName = match.Groups["ProjectName"].Value;
                            string projectPath = match.Groups["ProjectPath"].Value;
                            string projectGuid = match.Groups["ProjectGuid"].Value;

                            if (!String.IsNullOrEmpty(projectName) &&
                                !String.IsNullOrEmpty(projectPath) &&
                                !String.IsNullOrEmpty(projectGuid))
                            {
                                if (Path.IsPathRooted(projectPath))
                                {
                                    projectPath = Path.GetFullPath(projectPath);
                                }
                                else
                                {
                                    projectPath = Path.GetFullPath(
                                        Path.Combine(solutionDir, projectPath));
                                }

                                ProjectInfo info = new ProjectInfo(projectPath,
                                    projectGuid, projectName);

                                if (info.IsValid)
                                {
                                    listProjectInfo.Add(info);
                                }
                            }
                        }
                    }
                }
            }

            return listProjectInfo;
        }

        #endregion

        #endregion

        #region Private Methods

        private static void RequiresAll(string solutionFile, string platform, 
            string configuration)
        {
            ProjectExceptions.PathMustExist(solutionFile,    "solutionFile");

            string storageFile = Path.ChangeExtension(solutionFile, ".suo");
            if (File.Exists(storageFile))
            {
                // If the storage file exits, we could still extract the
                // platform and configuration information...
                return;
            }

            ProjectExceptions.NotNullNotEmpty(platform,      "platform");
            ProjectExceptions.NotNullNotEmpty(configuration, "configuration");
        }   

        #endregion
    }
}
