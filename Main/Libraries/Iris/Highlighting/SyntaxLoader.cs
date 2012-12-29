/*
Copyright (c) 2007 Gustavo G. Duarte (http://duartes.org/gustavo)

Permission is hereby granted, free of charge, to any person obtaining a copy of 
this software and associated documentation files (the "Software"), to deal in 
the Software without restriction, including without limitation the rights to 
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do 
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using Common.Helpers;
using Iris.Highlighting.VimBasedScanning;
using Iris.Highlighting.VimScriptParsing;

namespace Iris.Highlighting {
	/// <summary>
	/// Provides methods related to the loading of syntax definition scripts
	/// </summary>
	public static class SyntaxLoader {
		static SyntaxLoader() {
			m_lock = new object();
		}

		#region Public members

		/// <summary>
		/// "IrisCatalog.xml"
		/// </summary>
		public const string CatalogFileName = "IrisCatalog.xml";

		/// <summary>
		/// The IrisCatalog.xml configuration file that contains the syntax and CSS Schemes catalog
		/// </summary>
		public static FileInfo SyntaxCatalogFile {
			get {
				EnsureInitialization();

				return m_syntaxCatalogFile;
			}
		}

		/// <summary>
		/// The directory that contains the syntax definition scripts
		/// </summary>
		public static DirectoryInfo SyntaxDirectory {
			get {
				EnsureInitialization();

				return m_syntaxDirectory;
			}
		}

		/// <summary>
		/// Given a syntax id, returns the file that contains the syntax definition script corresponding to the syntax
		/// </summary>
		/// <param name="syntaxId"></param>
		/// <returns></returns>
		public static FileInfo FileInfoFromSyntaxId(string syntaxId) {
			ArgumentValidator.ThrowIfNullOrEmpty(syntaxId, "syntaxId");

			FileInfo syntaxFile = new FileInfo(Path.Combine(SyntaxDirectory.FullName, syntaxId + ".vim"));
			if (!syntaxFile.Exists) {
				string msg = StringExtensions.Fi("Cannot find syntax file '{0}'", syntaxFile.FullName);
				throw new FileNotFoundException(msg, syntaxFile.FullName);
			}

			return syntaxFile;
		}

		#endregion

		#region Internal and Private members

		private static readonly object m_lock;
		private static FileInfo m_syntaxCatalogFile;
		private static DirectoryInfo m_syntaxDirectory;
		private static bool m_hasInitializedFileSystemLocations;

		internal static SyntaxDefinition BuildSyntaxDefinition(string syntaxId) {
			VimScriptParser parser = new VimScriptParser(FileInfoFromSyntaxId(syntaxId));
			parser.SyntaxDefinition.FinishSyntaxDefinition();
			return parser.SyntaxDefinition;
		}

		internal static void LoadCatalog() {
			LoadVimSyntaxFilesInFolder();

			XmlDocument catalog = new XmlDocument();
			catalog.Load(SyntaxCatalogFile.OpenRead());

			XmlElement iris = catalog["iris"];
			if (null == iris) {
				string msg = StringExtensions.Fi("The syntax catalog file at '{0}' does not contain the XML element <iris>, " +
					"which should be the root element in the file", SyntaxCatalogFile.FullName);
				throw new ConfigurationException(msg);
			}

			bool foundDefault = false;
			foreach (XmlElement cssSchemeElement in iris.SelectNodes("cssSchemes/cssScheme")) {
				foundDefault = foundDefault || LoadCssScheme(cssSchemeElement);
			}

			if (!foundDefault) {
				string msg = StringExtensions.Fi("The syntax catalog file at '{0}' did not specify a default CSS Scheme. At least one <cssScheme> must be " +
					"present containing attribute default='true'", SyntaxCatalogFile.FullName);
				throw new ConfigurationException(msg);
			}

			foreach (XmlElement syntaxElement in iris.SelectNodes("syntaxes/syntax")) {
				ApplySyntaxConfigurations(syntaxElement);
			}
		}

		private static void LoadVimSyntaxFilesInFolder() {
			foreach (FileInfo file in SyntaxDirectory.GetFiles("*.vim")) {
				string id = Path.GetFileNameWithoutExtension(file.Name);
				Syntax newSyntax = new Syntax(id, id);
				SyntaxCatalog.Add(newSyntax);
			}
		}

		private static void ApplySyntaxConfigurations(XmlElement syntaxElement)  {
			// At this point, if the Syntax file really does exist it has already been added by LoadVimSyntaxFilesInFolder,
			// so all we're doing is applying configurations (description, aliases);

			string id = ReadAttributeOrDie(syntaxElement, "id");
			if (!SyntaxCatalog.m_syntaxesByIdOrAlias.ContainsKey(id)) {
				return;
			}

			Syntax syntax = SyntaxCatalog.m_syntaxesByIdOrAlias[id];

			if (syntaxElement.HasAttribute("description")) {
				syntax.Description = ReadAttributeOrDie(syntaxElement, "description");
			}

			if (syntaxElement.HasChildNodes) {
				foreach (XmlElement alias in syntaxElement.GetElementsByTagName("alias")) {
					SyntaxCatalog.AddAlias(alias.InnerText, syntax.Id);
				}
			}
		}

		private static string ReadAttributeOrDie(XmlElement element, string attributeName) {
			if (!element.HasAttribute(attributeName) || string.IsNullOrEmpty(element.Attributes[attributeName].Value)) {
				ThrowError(element, StringExtensions.Fi("Failed to find value for required attribute '{0}' in element '{1}'",
					attributeName, element.LocalName));
			}

			return element.Attributes[attributeName].Value;
		}

		private static void FindSyntaxCatalog(out DirectoryInfo syntaxDirectory, out FileInfo syntaxCatalogFile) {
			List<string> attempts = new List<string>(2);
			List<string> baseDirs = new List<string>(4);

			baseDirs.Add(AppDomain.CurrentDomain.BaseDirectory);
			baseDirs.Add(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

			foreach (string baseDir in Extensions.Distinct(baseDirs)) {
				if (TrySyntaxDirectory(Path.Combine(baseDir, "syntax"), out syntaxDirectory, out syntaxCatalogFile)) {
					return;
				}
				attempts.Add(syntaxDirectory.FullName);

				if (TrySyntaxDirectory(Path.Combine(baseDir, @"bin\syntax"), out syntaxDirectory, out syntaxCatalogFile)) {
					return;
				}
				attempts.Add(syntaxDirectory.FullName);
			}

			string msg = StringExtensions.Fi("Unable to find the directory containing Iris syntax files. We have tried the following directories: '{0}'.",
				string.Join("', '", attempts.ToArray()));
			throw new DirectoryNotFoundException(msg);
		}

		internal static void InitFileSystemLocations(DirectoryInfo syntaxDirectory, FileInfo syntaxCatalogFile) {
			ArgumentValidator.ThrowIfDoesNotExist(syntaxDirectory, "syntaxDirectory");
			ArgumentValidator.ThrowIfDoesNotExist(syntaxCatalogFile, "syntaxCatalogFile");

			if (m_hasInitializedFileSystemLocations) {
				string msg = StringExtensions.Fi("Invalid attempt to initialize file system locations for syntax catalog after "
					+ "they have already been initialized. The syntax directory is '{0}' and the syntax catalog file is '{1}'",
					syntaxDirectory.FullName, syntaxCatalogFile.FullName);

				throw new InvalidOperationException(msg);
			}

			bool gotLock = false;
			try {
				gotLock = Monitor.TryEnter(m_lock, TimeSpan.FromSeconds(3));

				if (m_hasInitializedFileSystemLocations) {
					return;
				}

				m_syntaxDirectory = syntaxDirectory;
				m_syntaxCatalogFile = syntaxCatalogFile;
				m_hasInitializedFileSystemLocations = true;
			} finally {
				if (gotLock) {
					Monitor.Exit(m_lock);
				}
			}
		}

		private static void EnsureInitialization() {
			if (m_hasInitializedFileSystemLocations) {
				return;
			}

			DirectoryInfo syntaxDir;
			FileInfo syntaxFile;

			FindSyntaxCatalog(out syntaxDir, out syntaxFile);
			if (!syntaxFile.Exists) {
				string msg = StringExtensions.Fi("Syntax Catalog file not found at '{0}'", syntaxFile);
				throw new FileNotFoundException(msg);
			}

			InitFileSystemLocations(syntaxDir, syntaxFile);
		}

		private static bool LoadCssScheme(XmlElement cssSchemeElement) {
			bool isDefault = false;
			string cssSchemeName = ReadAttributeOrDie(cssSchemeElement, "name");
			CssScheme cssScheme = new CssScheme(cssSchemeName);

			isDefault = cssSchemeElement.HasAttribute("default") && "true" == cssSchemeElement.Attributes["default"].Value;

			if (isDefault) {
				CssScheme.DefaultCssScheme = cssScheme;
			}

			XmlElement global = cssSchemeElement["global"];
			if (null == global) {
				ThrowError(cssSchemeElement, StringExtensions.Fi("Did not find required child element <global> inside CSS Scheme {0}", cssScheme.Name));
			}

			cssScheme.GlobalStyleSheet = cssSchemeElement["global"].InnerText;

			foreach (XmlElement syntaxCssElement in cssSchemeElement.SelectNodes("syntax")) {
				string syntaxId = ReadAttributeOrDie(syntaxCssElement, "id");

				cssScheme.PerSyntaxStyleSheets[syntaxId] = syntaxCssElement.InnerText;
			}

			CssScheme.CssSchemes.Add(cssScheme);
			return isDefault;
		}

		private static void ThrowError(XmlElement element, string errorDescription) {
			int lineNumber = (element as IXmlLineInfo).LineNumber;
			int position = (element as IXmlLineInfo).LinePosition;

			string msg = StringExtensions.Fi("Iris catalog file '{0}' has a configuration error in line '{1}', position '{2}'. {3}",
				SyntaxCatalogFile.FullName, lineNumber, position, errorDescription);

			throw new ConfigurationException(msg);
		}

		private static bool TrySyntaxDirectory(string fullPath, out DirectoryInfo syntaxDirectory, out FileInfo syntaxCatalog) {
			syntaxDirectory = new DirectoryInfo(fullPath);
			if (syntaxDirectory.Exists) {
				syntaxCatalog = new FileInfo(Path.Combine(syntaxDirectory.FullName, CatalogFileName));
			} else {
				syntaxCatalog = null;
			}

			return syntaxDirectory.Exists;
		}

		#endregion
	}
}