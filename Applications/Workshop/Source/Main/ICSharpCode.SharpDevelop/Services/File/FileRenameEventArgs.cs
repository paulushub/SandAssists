// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop
{
    public class FileRenameEventArgs : EventArgs
    {
        bool isDirectory;
        string sourceFile;
        string targetFile;

        public FileRenameEventArgs(string sourceFile, string targetFile,
            bool isDirectory)
        {
            this.sourceFile  = sourceFile;
            this.targetFile  = targetFile;
            this.isDirectory = isDirectory;
        }

        public string SourceFile
        {
            get
            {
                return sourceFile;
            }
        }

        public string TargetFile
        {
            get
            {
                return targetFile;
            }
        }

        public bool IsDirectory
        {
            get
            {
                return isDirectory;
            }
        }
    }

	public class FileRenamingEventArgs : FileRenameEventArgs
	{
        private bool cancel;
        private bool operationAlreadyDone;

        public FileRenamingEventArgs(string sourceFile, string targetFile, 
            bool isDirectory) : base(sourceFile, targetFile, isDirectory)
        {
        }

		public bool Cancel 
        {
			get 
            {
				return cancel;
			}
			set 
            {
				cancel = value;
			}
		}

		public bool OperationAlreadyDone 
        {
			get 
            {
				return operationAlreadyDone;
			}
			set 
            {
				operationAlreadyDone = value;
			}
		}
	}
}
