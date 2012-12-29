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

using System.Collections.Generic;
using Common.Helpers;

namespace Iris.Highlighting.VimBasedScanning {
	internal class SyntaxContext {
		#region Public members

		public void AddTopCluster(Cluster topCluster) {
			ArgumentValidator.ThrowIfNull(topCluster, "topCluster");

			// It's possible for the same cluster to be added to a context as a TopCluster. For example, gsp.vim runtimes html.vim, which in turn
			// includes javaScript.vim into @htmlJavaScript. But then gsp.vim later includes java.vim, which in turn
			// re-includes html.vim. Got that? :)  Well, when html.vim is re-included, it tries to include javascript.vim
			// _again_, and into the same cluster, @htmlJavaScript. In those cases, we just return.
			if (this.TopClusters.Contains(topCluster)) {
				return;
			}

			for (int i = 0; i < this.TopItems.Items.Length; i++) {
				topCluster.AddDirectItem(this.TopItems.Items[i]);
			}

			this.TopClusters.Add(topCluster);
		}

		public override string ToString() {
			return StringExtensions.Fi("{0}: {1} items", this.Name, this.AllItems.Count);
		}

		#endregion

		#region Internal and Private members

		internal readonly List<SyntaxItem> AllItems;
		internal readonly string Name;
		internal readonly SyntaxDefinition SyntaxDefinition;
		internal SetOfSyntaxItems NonTopItems;
		internal List<Cluster> TopClusters;
		internal SetOfSyntaxItems TopItems;

		internal SyntaxContext(SyntaxDefinition syntaxDefinition, string scopeName) {
			this.SyntaxDefinition = syntaxDefinition;
			this.AllItems = new List<SyntaxItem>(16);
			this.NonTopItems = new SetOfSyntaxItems();
			this.TopItems = new SetOfSyntaxItems();
			this.Name = scopeName;
			this.TopClusters = new List<Cluster>(2);
		}

		internal void PrepareForRuntime() {
			this.TopItems = SetOfSyntaxItems.PrepareForRuntime(this.TopItems, this.SyntaxDefinition);
			this.AllItems.ForEach(item => item.AddYourselfToGroupsYouAreContainedIn());
		}

		#endregion
	}
}