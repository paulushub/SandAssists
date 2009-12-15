// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 4457 $</version>
// </file>

using System;
using System.Collections.Generic;

using ICSharpCode.Core;
using ICSharpCode.SharpDevelop;
using ICSharpCode.SharpDevelop.Gui;

namespace ICSharpCode.TextEditor.Searching
{
	public static class SearchInFilesManager
	{
		private static Search find = new Search();

        private static string currentFileName = String.Empty;
		
		static SearchInFilesManager()
		{
			find.TextIteratorBuilder = new ForwardTextIteratorBuilder();
		}

        public static void ShowSearchResults(string pattern, List<SearchResultMatch> results)
        {
            SearchResultPanel.Instance.ShowSearchResults(new SearchResult(pattern, results));
        }

        public static void FindAll(IProgressMonitor monitor)
        {
            if (!InitializeSearchInFiles(monitor))
            {
                return;
            }

            List<SearchResultMatch> results = new List<SearchResultMatch>();
            while (true)
            {
                SearchResultMatch result = find.FindNext(monitor);
                if (result == null)
                {
                    break;
                }
                results.Add(result);
            }
            FinishSearchInFiles(results);
        }

        public static void FindAll(int offset, int length, IProgressMonitor monitor)
        {
            if (!InitializeSearchInFiles(monitor))
            {
                return;
            }

            List<SearchResultMatch> results = new List<SearchResultMatch>();
            while (true)
            {
                SearchResultMatch result = find.FindNext(offset, length);
                if (result == null)
                {
                    break;
                }
                results.Add(result);
            }
            FinishSearchInFiles(results);
        }

        private static void SetSearchOptions(IProgressMonitor monitor)
		{
			find.SearchStrategy   = SearchReplaceUtilities.CreateSearchStrategy(
                SearchOptions.SearchStrategyType);
            find.DocumentIterator = SearchReplaceUtilities.CreateDocumentIterator(
                SearchOptions.DocumentIteratorType, monitor);
        }

        private static bool InitializeSearchInFiles(IProgressMonitor monitor)
		{
			SetSearchOptions(monitor);
			
			find.Reset();
			if (!find.SearchStrategy.CompilePattern(monitor))
				return false;
			
			currentFileName = String.Empty;
			return true;
		}

        private static void FinishSearchInFiles(List<SearchResultMatch> results)
		{
			ShowSearchResults(SearchOptions.FindPattern, results);
		}
	}
}
