// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;

namespace ICSharpCode.SharpDevelop
{
	public class FileEventArgs : EventArgs
	{
        private bool isDirectory;
        private string fileName;

        public FileEventArgs(string fileName, bool isDirectory)
        {
            this.fileName = fileName;
            this.isDirectory = isDirectory;
        }
		
		public string FileName 
        {
			get 
            {
				return fileName;
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
	
	public class FileCancelEventArgs : FileEventArgs
	{
        private bool cancel;
        private bool operationAlreadyDone;

        public FileCancelEventArgs(string fileName, bool isDirectory)
            : base(fileName, isDirectory)
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
