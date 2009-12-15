// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3671 $</version>
// </file>

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace ICSharpCode.Core
{
	public sealed class AddIn
    {
        #region Private Fields

		private Properties     properties;
        private IList<Runtime> runtimes;
        private IList<string> bitmapResources;
        private IList<string> stringResources;

        private bool enabled;
        private bool dependenciesLoaded;
        private string customErrorMessage;

        private string addInFileName;

        private AddInAction action;
        private AddInManifest manifest;
        private Dictionary<string, ExtensionPath> paths;

        private static bool hasShownErrorMessage;

        #endregion

        #region Construtors and Destructor

        internal AddIn()
        {
            properties      = new Properties();
            runtimes        = new List<Runtime>();
            bitmapResources = new List<string>();
            stringResources = new List<string>();

            manifest = new AddInManifest();
            paths = new Dictionary<string, ExtensionPath>(
                StringComparer.OrdinalIgnoreCase);
            action   = AddInAction.Disable;
        }

        #endregion

        #region Public Properties

        /// <summary>
		/// Gets the message of a custom load error. Used only when AddInAction is set to CustomError.
		/// Settings this property to a non-null value causes Enabled to be set to false and
		/// Action to be set to AddInAction.CustomError.
		/// </summary>
		public string CustomErrorMessage {
			get {
				return customErrorMessage;
			}
			internal set {
				if (value != null) {
					Enabled = false;
					Action = AddInAction.CustomError;
				}
				customErrorMessage = value;
			}
		}
		
		/// <summary>
		/// Action to execute when the application is restarted.
		/// </summary>
		public AddInAction Action {
			get {
				return action;
			}
			set {
				action = value;
			}
		}
		
		public IList<Runtime> Runtimes {
			get {
				return runtimes;
			}
		}
		
		public Version Version {
			get {
				return manifest.PrimaryVersion;
			}
		}
		
		public string FileName {
			get {
				return addInFileName;
			}
            internal set
            {
                addInFileName = value;
            }
		}
		
		public string Name {
			get {
				return properties["name"];
			}
		}
		
		public AddInManifest Manifest {
			get {
				return manifest;
			}
		}
		
		public IDictionary<string, ExtensionPath> Paths {
			get {
				return paths;
			}
		}
		
		public Properties Properties {
			get {
				return properties;
			}
            internal set
            {
                if (value != null)
                {
                    properties = value;
                }
            }
		}
		
		public IList<string> BitmapResources {
			get {
				return bitmapResources;
			}
			set {
				bitmapResources = value;
			}
		}
		
		public IList<string> StringResources {
			get {
				return stringResources;
			}
			set {
				stringResources = value;
			}
		}
		
		public bool Enabled {
			get {
				return enabled;
			}
			set {
				enabled = value;
				this.Action = value ? AddInAction.Enable : AddInAction.Disable;
			}
        }

        #endregion

        #region Public Methods

        public object CreateObject(string className)
		{
			LoadDependencies();
			foreach (Runtime runtime in runtimes) {
				object o = runtime.CreateInstance(className);
				if (o != null) {
					return o;
				}
			}
			if (hasShownErrorMessage) {
				LoggingService.Error("Cannot create object: " + className);
			} else {
				hasShownErrorMessage = true;
				MessageService.ShowError("Cannot create object: " + className + "\nFuture missing objects will not cause an error message.");
			}
			return null;
		}
		
		public void LoadRuntimeAssemblies()
		{
			LoadDependencies();
			foreach (Runtime runtime in runtimes) {
				runtime.Load();
			}
        }

        public override string ToString()
        {
            return "[AddIn: " + Name + "]";
        }

        public ExtensionPath GetExtensionPath(string pathName)
        {
            if (!paths.ContainsKey(pathName))
            {
                ExtensionPath extPath = new ExtensionPath(pathName, this);
                paths[pathName] = extPath;

                return extPath;
            }
            return paths[pathName];
        }

        #endregion

        #region Private Methods

        private void LoadDependencies()
		{
			if (!dependenciesLoaded) {
				dependenciesLoaded = true;
				foreach (AddInReference r in manifest.Dependencies) {
					if (r.RequirePreload) {
						bool found = false;
						foreach (AddIn addIn in AddInTree.AddIns) {
							if (addIn.Manifest.Identities.ContainsKey(r.Name)) {
								found = true;
								addIn.LoadRuntimeAssemblies();
							}
						}
						if (!found) {
							throw new AddInLoadException("Cannot load run-time dependency for " + r.ToString());
						}
					}
				}
			}
        }

        #endregion
	}
}
