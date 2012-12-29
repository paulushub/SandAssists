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
using System.Text;
using Common.Helpers;

namespace Iris.Highlighting.VimBasedScanning {
	internal class HumanReadableSyntaxDumper {
		#region Internal and Private members

		internal static void Dump(SyntaxDefinition syntaxDefiniton, TextWriter writer) {
			ArgumentValidator.ThrowIfNull(syntaxDefiniton, "syntaxDefiniton");
			ArgumentValidator.ThrowIfNull(writer, "writer");

			foreach (SyntaxContext context in syntaxDefiniton.m_syntaxContexts) {
				DoContext(context);
			}

			foreach (KeyValuePair<string, Cluster> pair in syntaxDefiniton.ClustersByName) {
				Console.WriteLine("Cluster  {0,-10} {1,-5} items Scope: {2}", pair.Key, pair.Value.CachedMemberItems.Items.Length, pair.Value.SyntaxContext.Name);
			}
		}

		private static void AppendCommonOptions(SyntaxItem item, StringBuilder firstLine) {
			if (item.IsContained) {
				firstLine.Append(" contained");
			}

			if (item.IsTransparent) {
				firstLine.Append(" transparent");
			}

			if (item is ContainerItem && (item as ContainerItem).Extend) {
				firstLine.Append(" extend");
			}

			if (item is Region && (item as Region).IsOneLine) {
				firstLine.Append(" oneline");
			}

			if (item is Region && (item as Region).KeepEnd) {
				firstLine.Append(" keepend");
			}

			if (0 < item.ContainedIn.ContainedGroupsAndClusters.Count) {
				firstLine.AppendFormat(" containedin={0}", item.ContainedIn);
			}

			if (item is ContainerItem && 0 < (item as ContainerItem).Contains.ContainedGroupsAndClusters.Count) {
				firstLine.AppendFormat(" contains={0}", (item as ContainerItem).Contains);
			}

			if (0 < item.NextGroupCluster.ContainedGroupsAndClusters.Count) {
				firstLine.AppendFormat(" nextgroup={0}", item.NextGroupCluster);

				if (item.SkipEmptyLine) {
					firstLine.Append(" skipempty");
				}

				if (item.SkipNewLine) {
					firstLine.Append(" skipnl");
				}

				if (item.SkipWhite) {
					firstLine.Append(" skipwhite");
				}
			}
		}

		private static void DoContext(SyntaxContext context) {
			Console.WriteLine("\nContext:  {0}", context.Name);

			foreach (SyntaxItem item in context.AllItems) {
				StringBuilder firstLine = new StringBuilder(32);
				string moreLines = null;

				if (item is Keyword) {
					firstLine.AppendFormat("{0,3} keyword  {1,-10} {2,-10} {3}", item.LineNumberInSyntaxFile, (item as Keyword).Name, item.GroupName, item.HighlightMode);
				} else if (item is VimMatch) {
					firstLine.AppendFormat("{0,3} vimMatch {1,-10} {2}", item.LineNumberInSyntaxFile, item.GroupName, item.HighlightMode);
					moreLines = DumpPattern((item as VimMatch).Pattern, string.Empty);
				} else if (item is Region) {
					firstLine.AppendFormat("{0,3} region	{1,-10} {2}", item.LineNumberInSyntaxFile, item.GroupName, item.HighlightMode);
					moreLines = DumpRegionPatterns((Region) item);
				}

				AppendCommonOptions(item, firstLine);

				Console.WriteLine(firstLine.ToString());
				if (moreLines != null) {
					Console.Write(moreLines);
				}
			}
		}

		private static string DumpPattern(Pattern pattern, string description) {
			StringBuilder sb = new StringBuilder();
			sb.AppendFormat("	{0}{1}   ", description, pattern);

			if (0 < pattern.LeadingContext) {
				sb.AppendFormat("LeadingContext={0} ", pattern.LeadingContext);
			}

			foreach (PatternOffset offset in pattern.Offsets) {
				sb.AppendFormat("{0} ", offset);
			}

			if (pattern.HasHighlightMode) {
				sb.AppendFormat("{0} ", pattern.HighlightMode);
			}

			if (0 < pattern.CntExternalGroups) {
				sb.AppendFormat("CntExternalGroups={0} ", pattern.CntExternalGroups);
			}

			if (pattern.EatNewLine) {
				sb.Append("EatNewLine ");
			}

			if (0 < pattern.LastExternalMatch) {
				sb.AppendFormat("LastExternalMatch={0} ", pattern.LastExternalMatch);
			}

			sb.AppendLine();

			return sb.ToString();
		}

		private static string DumpRegionPatterns(Region region) {
			StringBuilder lines = new StringBuilder();

			foreach (Pattern pattern in region.StartPatterns) {
				lines.Append(DumpPattern(pattern, "start "));
			}

			foreach (Pattern pattern in region.SkipPatterns) {
				lines.Append(DumpPattern(pattern, "skip  "));
			}

			foreach (Pattern pattern in region.EndPatterns) {
				lines.Append(DumpPattern(pattern, "end   "));
			}

			return lines.ToString();
		}

		#endregion
	}
}