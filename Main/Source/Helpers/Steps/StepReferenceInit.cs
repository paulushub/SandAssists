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

            ReferenceGroupContext groupContext =
                context.GroupContexts[_group.Id] as ReferenceGroupContext;
            if (groupContext == null)
            {
                throw new BuildException(
                    "The group context is not provided, and it is required by the build system.");
            }

            string workingDir = _group.WorkingDirectory;

            // 1. Copy the comments to the expected directory...
            ProcessReferenceItems(groupContext, workingDir);

            // 2. Copy the dependencies to the expected directory...
            ProcessDependencyItems(groupContext, workingDir);
            
            return true;
        }

        #endregion

        #region Private Methods

        #region ProcessReferenceItems Method

        private void ProcessReferenceItems(ReferenceGroupContext groupContext, 
            string workingDir)
        {
            ReferenceContent contents = _group.Content;
            if (contents == null)
            {
                return;
            }

            string commentsDir = groupContext.CommentFolder;
            string assemblyDir = groupContext.AssemblyFolder;
            if (String.IsNullOrEmpty(commentsDir))
            {
                commentsDir = "Comments";
            }
            if (!Path.IsPathRooted(commentsDir))
            {
                commentsDir = Path.Combine(workingDir, commentsDir);
            }
            if (!Directory.Exists(commentsDir))
            {
                Directory.CreateDirectory(commentsDir);
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

            int itemCount = contents.Count;
            List<string> commentFiles = new List<string>(itemCount);

            for (int i = 0; i < itemCount; i++)
            {
                ReferenceItem item = contents[i];
                if (item == null || item.IsEmpty)
                {
                    continue;
                }

                string commentsFile = item.Comments;
                if (!String.IsNullOrEmpty(commentsFile))
                {
                    string fileName = Path.GetFileName(commentsFile);
                    fileName = Path.Combine(commentsDir, fileName);
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
        }

        #endregion

        #region ProcessDependencyItems Method

        private void ProcessDependencyItems(ReferenceGroupContext groupContext, 
            string workingDir)
        {
            ReferenceContent content = _group.Content;
            DependencyContent dependencies = content.Dependencies;

            if (dependencies == null || dependencies.Count == 0)
            {
                return;
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
                    fileName = Path.Combine(dependencyDir, fileName);
                    if (dependencyFile.Length != fileName.Length ||
                        String.Equals(dependencyFile, fileName,
                        StringComparison.OrdinalIgnoreCase) == false)
                    {
                        File.Copy(dependencyFile, fileName, true);
                        File.SetAttributes(fileName, FileAttributes.Normal);
                    }
                }
            }
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
