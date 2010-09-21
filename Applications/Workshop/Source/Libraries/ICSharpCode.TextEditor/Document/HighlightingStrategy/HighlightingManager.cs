// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3209 $</version>
// </file>

using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;

namespace ICSharpCode.TextEditor.Document
{
	public sealed class HighlightingManager
	{
        private static HighlightingManager highlightingManager;

		private List<ISyntaxModeFileProvider> syntaxModeFileProviders;
		
		// hash table from extension name to highlighting definition,
		// OR from extension name to Pair SyntaxMode,ISyntaxModeFileProvider
        private Dictionary<string, object> highlightingDefs;

        private Dictionary<string, string> extensionsToName;
		
		public HighlightingManager()
		{
            highlightingDefs = new Dictionary<string, object>(
                StringComparer.OrdinalIgnoreCase);
            extensionsToName = new Dictionary<string, string>(
                StringComparer.OrdinalIgnoreCase);

            syntaxModeFileProviders = new List<ISyntaxModeFileProvider>();

			CreateDefaultHighlightingStrategy();
		}

        static HighlightingManager()
        {
            highlightingManager = new HighlightingManager();
            highlightingManager.AddSyntaxModeFileProvider(
                new ResourceSyntaxModeProvider());
        }

        public IDictionary<string, object> HighlightingDefinitions
        {
            get
            {
                return highlightingDefs;
            }
        }

        public static HighlightingManager Manager
        {
            get
            {
                return highlightingManager;
            }
        }
		
		public void AddSyntaxModeFileProvider(ISyntaxModeFileProvider syntaxModeFileProvider)
		{
			foreach (SyntaxMode syntaxMode in syntaxModeFileProvider.SyntaxModes) {
                syntaxMode.Provider = syntaxModeFileProvider;
                highlightingDefs[syntaxMode.Name] = syntaxMode;
				foreach (string extension in syntaxMode.Extensions) {
					extensionsToName[extension] = syntaxMode.Name;
				}
			}
			if (!syntaxModeFileProviders.Contains(syntaxModeFileProvider)) {
				syntaxModeFileProviders.Add(syntaxModeFileProvider);
			}
		}

		public void AddHighlightingStrategy(IHighlightingStrategy highlightingStrategy)
		{
			highlightingDefs[highlightingStrategy.Name] = highlightingStrategy;
			foreach (string extension in highlightingStrategy.Extensions)
			{
				extensionsToName[extension] = highlightingStrategy.Name;
			}
		}
		
		public void ReloadSyntaxModes()
		{
			highlightingDefs.Clear();
			extensionsToName.Clear();
			CreateDefaultHighlightingStrategy();
            int itemCount = syntaxModeFileProviders.Count;
            for (int i = 0; i < itemCount; i++)
            {
                ISyntaxModeFileProvider provider = syntaxModeFileProviders[i];
                if (provider != null)
                {
                    provider.UpdateSyntaxModeList();
                    AddSyntaxModeFileProvider(provider);
                }
            }
			OnReloadSyntaxHighlighting(EventArgs.Empty);
		}
		
		void CreateDefaultHighlightingStrategy()
		{
			HighlightingStrategy defaultHighlightingStrategy = new HighlightingStrategy();
			defaultHighlightingStrategy.Extensions = new string[] {};
			defaultHighlightingStrategy.Rules.Add(new HighlightRuleSet());
			highlightingDefs["Default"] = defaultHighlightingStrategy;
		}

        IHighlightingStrategy LoadDefinition(SyntaxMode syntaxMode)
		{
            ISyntaxModeFileProvider syntaxModeFileProvider = syntaxMode.Provider;

			HighlightingStrategy highlightingStrategy = null;
			try {
				var reader = syntaxModeFileProvider.GetSyntaxModeFile(syntaxMode, null);
				if (reader == null)
					throw new HighlightingDefinitionInvalidException("Could not get syntax mode file for " + syntaxMode.Name);
				highlightingStrategy = HighlightingDefinitionParser.Parse(syntaxMode, reader);
				if (highlightingStrategy.Name != syntaxMode.Name) {
					throw new HighlightingDefinitionInvalidException(
                        "The name specified in the .xshd '" + highlightingStrategy.Name + 
                        "' must be equal the syntax mode name '" + syntaxMode.Name + "'");
				}
			} finally {
				if (highlightingStrategy == null) {
					highlightingStrategy = DefaultHighlighting;
				}
				highlightingDefs[syntaxMode.Name] = highlightingStrategy;
				highlightingStrategy.ResolveReferences();
			}
			return highlightingStrategy;
		}
		
		public HighlightingStrategy DefaultHighlighting 
        {
			get 
            {
                if (!highlightingDefs.ContainsKey("Default"))
                {
                    this.CreateDefaultHighlightingStrategy();
                }

                return (HighlightingStrategy)highlightingDefs["Default"];
            }
		}
		
		internal KeyValuePair<SyntaxMode, ISyntaxModeFileProvider> FindHighlighterEntry(string name)
		{
			foreach (ISyntaxModeFileProvider provider in syntaxModeFileProviders) {
				foreach (SyntaxMode mode in provider.SyntaxModes) {
					if (mode.Name == name) {
						return new KeyValuePair<SyntaxMode, ISyntaxModeFileProvider>(mode, provider);
					}
				}
			}
			return new KeyValuePair<SyntaxMode, ISyntaxModeFileProvider>(null, null);
		}
		
		public IHighlightingStrategy FindHighlighter(string name)
		{
            if (String.IsNullOrEmpty(name))
            {
                return this.DefaultHighlighting;
            }

            if (highlightingDefs.ContainsKey(name))
            {
                object def = highlightingDefs[name];
                SyntaxMode syntaxMode = def as SyntaxMode;
                if (syntaxMode != null)
                {
                    return this.LoadDefinition(syntaxMode);
                }

                return (IHighlightingStrategy)def;
            }

			return this.DefaultHighlighting;
		}
		
		public IHighlightingStrategy FindHighlighterForFile(string fileName)
		{
            string fileExt = Path.GetExtension(fileName);
            if (String.IsNullOrEmpty(fileExt) || !extensionsToName.ContainsKey(fileExt))
            {
                return this.DefaultHighlighting;
            }

            string highlighterName = extensionsToName[fileExt];
			if (highlighterName != null) 
            {
                if (highlightingDefs.ContainsKey(highlighterName))
                {
                    object def = highlightingDefs[highlighterName];
                    SyntaxMode syntaxMode = def as SyntaxMode;
                    if (syntaxMode != null)
                    {
                        return this.LoadDefinition(syntaxMode);
                    }

                    return (IHighlightingStrategy)def;
                }

                return this.DefaultHighlighting;
            } 
            else 
            {
                return this.DefaultHighlighting;
			}
		}
		
		private void OnReloadSyntaxHighlighting(EventArgs e)
		{
			if (ReloadSyntaxHighlighting != null) {
				ReloadSyntaxHighlighting(this, e);
			}
		}
		
		public event EventHandler ReloadSyntaxHighlighting;
	}
}
