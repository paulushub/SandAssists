using System;
using System.IO;
using System.Collections.Generic;

using Sandcastle.Contents;
using Sandcastle.References;

namespace Sandcastle.Steps
{
    public sealed class StepReferenceInit : BuildStep
    {
        #region Private Fields

        private ReferenceGroup _group;
        private BuildKeyedList<ReferenceVersions> _listVersions;

        #endregion

        #region Constructors and Destructor

        public StepReferenceInit()
        {
        }

        public StepReferenceInit(ReferenceGroup group)
        {
            BuildExceptions.NotNull(group, "group");

            _group = group;
        }

        #endregion

        #region Public Properties

        public ReferenceGroup Group
        {
            get
            {
                return _group;
            }
            set
            {
                _group = value;
            }
        }

        #endregion

        #region Protected Methods

        protected override bool OnExecute(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            if (_group == null)
            {
                if (logger != null)
                {
                    logger.WriteLine("StepReferenceInit: No reference group is attached to this step.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }
            if (!_group.IsInitialized)
            {
                if (logger != null)
                {
                    logger.WriteLine("StepReferenceInit: The attached reference group is not initialized.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            bool buildResult = this.OnExecuteSingle(context);
            if (!buildResult)
            {
                return buildResult;
            }

            if (_group.IsSingleVersion)
            {
                return buildResult;
            }

            buildResult = this.OnBeginMultiple(context);
            if (!buildResult)
            {
                return buildResult;
            }

            buildResult = this.OnExecuteMultiple(context);
            if (!buildResult)
            {
                return buildResult;
            }

            buildResult = this.OnEndMultiple(context);

            return buildResult;
        }

        #endregion

        #region Private Methods

        #region OnExecuteSingle Method

        private bool OnExecuteSingle(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            ReferenceGroupContext groupContext =
                context.GroupContexts[_group.Id] as ReferenceGroupContext;
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            ReferenceContent content = _group.Content;
            if (content == null)
            {
                if (logger != null)
                {
                    logger.WriteLine("StepReferenceInit: There is no content associated with the reference group.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            BuildFrameworkType frameworkType = content.FrameworkType;
            if (frameworkType == BuildFrameworkType.Null ||
                frameworkType == BuildFrameworkType.None)
            {
                if (logger != null)
                {
                    logger.WriteLine("StepReferenceInit: There is no valid framework type specified for this reference group.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            BuildFramework framework = BuildFrameworks.GetFramework(frameworkType);
            if (framework == null)
            {
                if (logger != null)
                {
                    logger.WriteLine("StepReferenceInit: The specified framework type for this reference group is not installed.",
                        BuildLoggerLevel.Error);
                }

                return false;
            }

            string workingDir = context.WorkingDirectory;

            groupContext.Framework = framework;

            string commentDir  = groupContext.CommentFolder;
            string assemblyDir = groupContext.AssemblyFolder;
            if (String.IsNullOrEmpty(commentDir))
            {
                commentDir = "Comments";
            }
            if (!Path.IsPathRooted(commentDir))
            {
                commentDir = Path.Combine(workingDir, commentDir);
            }
            if (!Directory.Exists(commentDir))
            {
                Directory.CreateDirectory(commentDir);
            }

            if (String.IsNullOrEmpty(assemblyDir))
            {
                assemblyDir = "Assemblies";
            }
            if (!Path.IsPathRooted(assemblyDir))
            {
                assemblyDir = Path.Combine(workingDir, assemblyDir);
            }         
            if (!Directory.Exists(assemblyDir))
            {
                Directory.CreateDirectory(assemblyDir);
            }

            string dependencyDir = groupContext.DependencyFolder;
            if (String.IsNullOrEmpty(dependencyDir))
            {
                dependencyDir = "Dependencies";
            }
            if (!Path.IsPathRooted(dependencyDir))
            {
                dependencyDir = Path.Combine(workingDir, dependencyDir);
            }
            if (!Directory.Exists(dependencyDir))
            {
                Directory.CreateDirectory(dependencyDir);
            }

            groupContext.CommentDir    = commentDir;
            groupContext.AssemblyDir   = assemblyDir;
            groupContext.DependencyDir = dependencyDir;

            // Copy the comments to the expected directory...
            int itemCount = content.Count;
            List<string> commentFiles = new List<string>(itemCount);

            CommentContent commentContent = content.Comments;
            if (commentContent != null && !commentContent.IsEmpty)
            {
                string commentFile = Path.Combine(commentDir,
                    groupContext["$CommentsFile"]);

                // If there is a valid file or there is an attached file...
                BuildFilePath filePath = commentContent.ContentFile;
                if (filePath != null && filePath.Exists)
                {   
                    if (commentContent.IsLoaded)
                    {
                        commentContent.Save();
                    }

                    File.Copy(filePath.Path, commentFile);
                }
                else
                {
                    commentContent.SaveCopyAs(commentFile);
                }
                File.SetAttributes(commentFile, FileAttributes.Normal);

                commentFiles.Add(commentFile);
            }

            for (int i = 0; i < itemCount; i++)
            {
                ReferenceItem item = content[i];
                if (item == null || item.IsEmpty)
                {
                    continue;
                }

                string commentsFile = item.Comments;
                if (!String.IsNullOrEmpty(commentsFile))
                {
                    string fileName = Path.GetFileName(commentsFile);
                    fileName = Path.Combine(commentDir, fileName);
                    if (commentsFile.Length != fileName.Length ||
                        String.Equals(commentsFile, fileName,
                        StringComparison.OrdinalIgnoreCase) == false)
                    {
                        File.Copy(commentsFile, fileName, true);
                        File.SetAttributes(fileName, FileAttributes.Normal);

                        commentFiles.Add(fileName);
                    }
                }

                string assemblyFile = item.Assembly;
                if (!String.IsNullOrEmpty(assemblyFile))
                {
                    string fileName = Path.GetFileName(assemblyFile);
                    fileName = Path.Combine(assemblyDir, fileName);
                    if (assemblyFile.Length != fileName.Length ||
                        String.Equals(assemblyFile, fileName,
                        StringComparison.OrdinalIgnoreCase) == false)
                    {
                        File.Copy(assemblyFile, fileName, true);
                        File.SetAttributes(fileName, FileAttributes.Normal);
                    }
                }
            }

            // Finally, store the list of extracted comment file to its context...
            groupContext.CommentFiles = commentFiles;

            // 1. Copy the dependencies to the expected directory...
            ReferenceProjectVisitor dependencyResolver =
                new ReferenceProjectVisitor();
            dependencyResolver.Initialize(context);
            dependencyResolver.Visit(_group);
            dependencyResolver.Uninitialize();
            
            return true;
        }

        #endregion

        #region OnBeginMultiple Method

        private bool OnBeginMultiple(BuildContext context)
        {
            BuildLogger logger = context.Logger;

            ReferenceGroupContext groupContext =
                context.GroupContexts[_group.Id] as ReferenceGroupContext;
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            string apiVersionsDir = Path.Combine(context.WorkingDirectory, 
                groupContext["$ApiVersionsFolder"]);

            if (!Directory.Exists(apiVersionsDir))
            {
                Directory.CreateDirectory(apiVersionsDir);
            }

            _listVersions = new BuildKeyedList<ReferenceVersions>();

            ReferenceVersionInfo versionInfo = _group.VersionInfo;

            // For the main version information...
            ReferenceVersions versions = new ReferenceVersions(versionInfo.PlatformId,
                versionInfo.PlatformTitle);
            ReferenceVersionSource mainSource = new ReferenceVersionSource(
                versionInfo.VersionId);
            mainSource.Content      = _group.Content;
            mainSource.VersionLabel = versionInfo.VersionLabel;

            versions.Add(mainSource);
            for (int i = 0; i < versionInfo.Count; i++)
            {
                ReferenceVersionSource source = versionInfo[i];
                if (source != null && source.IsValid)
                {
                    versions.Add(source);
                }
            }

            _listVersions.Add(versions);

            // For the related versions information...
            IList<ReferenceVersionRelated> listRelated = versionInfo.RelatedVersions;
            if (listRelated != null && listRelated.Count != 0)
            {
                for (int j = 0; j < listRelated.Count; j++)
                {
                    ReferenceVersionRelated related = listRelated[j];

                    if (related == null || related.IsEmpty)
                    {
                        continue;
                    }

                    versions = new ReferenceVersions(related.PlatformId, related.PlatformTitle);
                    for (int i = 0; i < related.Count; i++)
                    {
                        ReferenceVersionSource source = related[i];
                        if (source != null && source.IsValid)
                        {
                            versions.Add(source);
                        }
                    }

                    _listVersions.Add(versions);
                }
            }

            // Now, we prepare the various platforms and contexts...
            for (int i = 0; i < _listVersions.Count; i++)
            {
                versions = _listVersions[i];
                string indexText = String.Empty;
                if (_listVersions.Count > 1)
                {
                    indexText = (i + 1).ToString();
                }

                string versionsDir = Path.Combine(apiVersionsDir,
                    "Platform" + indexText);

                if (!Directory.Exists(versionsDir))
                {
                    Directory.CreateDirectory(versionsDir);
                }

                versions.PlatformDir = versionsDir;

                int itemCount = versions.Count;
                for (int j = 0; j < itemCount; j++)
                {
                    ReferenceVersionSource source = versions[j]; 

                    ReferenceGroupContext versionsContext =
                        new ReferenceGroupContext(_group, source.SourceId);

                    indexText = String.Empty;
                    if (itemCount > 1)
                    {
                        indexText = (j + 1).ToString();
                    }

                    string workingDir = Path.Combine(versionsDir, "Version" + indexText);
                    if (!Directory.Exists(workingDir))
                    {
                        Directory.CreateDirectory(workingDir);
                    }
                    versions.VersionDirs.Add(workingDir);

                    versionsContext["$GroupIndex"]    = groupContext["$GroupIndex"];
                    versionsContext["$VersionsIndex"] = indexText;
                    versionsContext["$VersionsDir"]   = versionsDir;
                    versionsContext["$WorkingDir"]    = workingDir;

                    versionsContext.CreateProperties(String.Empty); 

                    groupContext.Add(versionsContext);
                }
            }

            groupContext.Versions = _listVersions;

            return true;
        }

        #endregion

        #region OnExecuteMultiple Method

        private bool OnExecuteMultiple(BuildContext context)
        {
            ReferenceGroupContext groupContext =
                context.GroupContexts[_group.Id] as ReferenceGroupContext;
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            BuildLogger logger = context.Logger;

            for (int v = 0; v < _listVersions.Count; v++)
            {
                ReferenceVersions versions = _listVersions[v];

                for (int j = 0; j < versions.Count; j++)
                {
                    ReferenceVersionSource source = versions[j];

                    ReferenceGroupContext versionsContext =
                        groupContext.Contexts[source.SourceId];

                    string workingDir = versionsContext["$WorkingDir"];

                    ReferenceContent content = source.Content;
                    if (content == null)
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("StepReferenceInit: There is no content associated with the reference group.",
                                BuildLoggerLevel.Error);
                        }

                        return false;
                    }

                    BuildFrameworkType frameworkType = content.FrameworkType;
                    if (frameworkType == BuildFrameworkType.Null ||
                        frameworkType == BuildFrameworkType.None)
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("StepReferenceInit: There is no valid framework type specified for this reference group.",
                                BuildLoggerLevel.Error);
                        }

                        return false;
                    }

                    BuildFramework framework = BuildFrameworks.GetFramework(frameworkType);
                    if (framework == null)
                    {
                        if (logger != null)
                        {
                            logger.WriteLine("StepReferenceInit: The specified framework type for this reference group is not installed.",
                                BuildLoggerLevel.Error);
                        }

                        return false;
                    }

                    versionsContext.Framework = framework;

                    string commentDir  = versionsContext.CommentFolder;
                    string assemblyDir = versionsContext.AssemblyFolder;
                    if (String.IsNullOrEmpty(commentDir))
                    {
                        commentDir = "Comments";
                    }
                    if (!Path.IsPathRooted(commentDir))
                    {
                        commentDir = Path.Combine(workingDir, commentDir);
                    }
                    if (!Directory.Exists(commentDir))
                    {
                        Directory.CreateDirectory(commentDir);
                    }

                    if (String.IsNullOrEmpty(assemblyDir))
                    {
                        assemblyDir = "Assemblies";
                    }
                    if (!Path.IsPathRooted(assemblyDir))
                    {
                        assemblyDir = Path.Combine(workingDir, assemblyDir);
                    }         
                    if (!Directory.Exists(assemblyDir))
                    {
                        Directory.CreateDirectory(assemblyDir);
                    }

                    string dependencyDir = versionsContext.DependencyFolder;
                    if (String.IsNullOrEmpty(dependencyDir))
                    {
                        dependencyDir = "Dependencies";
                    }
                    if (!Path.IsPathRooted(dependencyDir))
                    {
                        dependencyDir = Path.Combine(workingDir, dependencyDir);
                    }
                    if (!Directory.Exists(dependencyDir))
                    {
                        Directory.CreateDirectory(dependencyDir);
                    }

                    versionsContext.CommentDir    = commentDir;
                    versionsContext.AssemblyDir   = assemblyDir;
                    versionsContext.DependencyDir = dependencyDir;

                    // Copy the comments to the expected directory...
                    int itemCount = content.Count;
                    List<string> commentFiles = new List<string>(itemCount);

                    for (int i = 0; i < itemCount; i++)
                    {
                        ReferenceItem item = content[i];
                        if (item == null || item.IsEmpty)
                        {
                            continue;
                        }

                        string commentsFile = item.Comments;
                        if (!String.IsNullOrEmpty(commentsFile))
                        {
                            string fileName = Path.GetFileName(commentsFile);
                            fileName = Path.Combine(commentDir, fileName);
                            if (commentsFile.Length != fileName.Length ||
                                String.Equals(commentsFile, fileName,
                                StringComparison.OrdinalIgnoreCase) == false)
                            {
                                File.Copy(commentsFile, fileName, true);
                                File.SetAttributes(fileName, FileAttributes.Normal);

                                commentFiles.Add(fileName);
                            }
                        }

                        string assemblyFile = item.Assembly;
                        if (!String.IsNullOrEmpty(assemblyFile))
                        {
                            string fileName = Path.GetFileName(assemblyFile);
                            fileName = Path.Combine(assemblyDir, fileName);
                            if (assemblyFile.Length != fileName.Length ||
                                String.Equals(assemblyFile, fileName,
                                StringComparison.OrdinalIgnoreCase) == false)
                            {
                                File.Copy(assemblyFile, fileName, true);
                                File.SetAttributes(fileName, FileAttributes.Normal);
                            }
                        }
                    }

                    //TODO--PAUL: Should the project/namespace summary be included?

                    // Finally, store the list of extracted comment file to its context...
                    versionsContext.CommentFiles = commentFiles;

                    // 1. Copy the dependencies to the expected directory...
                    ReferenceProjectVisitor dependencyResolver =
                        new ReferenceProjectVisitor(source.SourceId, content);
                    dependencyResolver.Initialize(context);
                    dependencyResolver.Visit(_group);
                    dependencyResolver.Uninitialize();
                }
            }  
            
            return true;
        }

        #endregion

        #region OnEndMultiple Method

        private bool OnEndMultiple(BuildContext context)
        {
            return true;
        }

        #endregion

        #endregion

        #region IDisposable Members

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        #endregion
    }
}
