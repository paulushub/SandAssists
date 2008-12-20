using System;
using System.Diagnostics;
using System.Collections.Generic;

using Sandcastle.Viewers.HtmlHelp.Decoding;

namespace Sandcastle.Viewers.HtmlHelp
{
	/// <summary>
	/// The class <c>HtmlHelpToc</c> holds the TOC of the htmlhelp system class.
	/// </summary>
	public class HtmlHelpToc
	{
        private List<HtmlHelpTocItem> _toc = new List<HtmlHelpTocItem>();

		/// <summary>
		/// Standard constructor
		/// </summary>
		public HtmlHelpToc()
		{
		}

		/// <summary>
		/// Constructor of the class
		/// </summary>
		/// <param name="toc"></param>
        public HtmlHelpToc(List<HtmlHelpTocItem> toc)
		{
			_toc = toc;
		}

		/// <summary>
		/// Gets the internal stored table of contents
		/// </summary>
        public IList<HtmlHelpTocItem> TOC
		{
			get { return _toc; }
		}

		/// <summary>
		/// Clears the current toc
		/// </summary>
		public void Clear()
		{
			if(_toc!=null)
				_toc.Clear();
		}

		/// <summary>
		/// Gets the number of topics in the toc
		/// </summary>
		/// <returns>Returns the number of topics in the toc</returns>
		public int Count()
		{
			if(_toc!=null)
				return _toc.Count;
			else
				return 0;
		}

		/// <summary>
		/// Merges the <c>arrToC</c> list to the one in this instance
		/// </summary>
		/// <param name="arrToC">the toc list which should be merged with the current one</param>
		internal void MergeToC(IList<HtmlHelpTocItem> arrToC )
		{
			if(_toc==null)
                _toc = new List<HtmlHelpTocItem>();

			Debug.WriteLine("HtmlHelpToc.MergeToC() ");
			Debug.Indent();
			Debug.WriteLine("Start: " + DateTime.Now.ToString("HH:mm:ss.ffffff"));
			MergeToC(_toc, arrToC, null);
			Debug.WriteLine("End: " + DateTime.Now.ToString("HH:mm:ss.ffffff"));
			Debug.Unindent();
		}

		/// <summary>
		/// Merges the <c>arrToC</c> list to the one in this instance (called if merged files
		/// were found in a CHM)
		/// </summary>
		/// <param name="arrToC">the toc list which should be merged with the current one</param>
		/// <param name="openFiles">An arraylist of CHMFile instances.</param>
        internal void MergeToC(IList<HtmlHelpTocItem> arrToC, IList<CHMFile> openFiles)
		{
			if(_toc==null)
                _toc = new List<HtmlHelpTocItem>();

			Debug.WriteLine("HtmlHelpToc.MergeToC() ");
			Debug.Indent();
			Debug.WriteLine("Start: " + DateTime.Now.ToString("HH:mm:ss.ffffff"));
			MergeToC(_toc, arrToC, openFiles);
			Debug.WriteLine("End: " + DateTime.Now.ToString("HH:mm:ss.ffffff"));
			Debug.Unindent();
		}

		/// <summary>
		/// Internal method for recursive toc merging
		/// </summary>
		/// <param name="globalLevel">level of global toc</param>
		/// <param name="localLevel">level of local toc</param>
		/// <param name="openFiles">An arraylist of CHMFile instances.</param>
        private void MergeToC(IList<HtmlHelpTocItem> globalLevel,
            IList<HtmlHelpTocItem> localLevel, IList<CHMFile> openFiles)
		{
			foreach( HtmlHelpTocItem curItem in localLevel)
			{
				// if it is a part of the merged-links, we have to do nothing, 
				// because the method HtmlHelpSystem.RecalculateMergeLinks() has already
				// placed this item at its correct position.
				if(!IsMergedItem(curItem.Name, curItem.Local, openFiles))
				{
					HtmlHelpTocItem globalItem = ContainsToC(globalLevel, curItem.Name);
					if(globalItem == null)
					{
						// the global toc doesn't have a topic with this name
						// so we need to add the complete toc node to the global toc

						globalLevel.Add( curItem );
					} 
					else 
					{
						// the global toc contains the current topic
						// advance to the next level

						if( (globalItem.Local.Length <= 0) && (curItem.Local.Length > 0) )
						{
							// set the associated url
							globalItem.Local = curItem.Local;
							globalItem.ChmFile = curItem.ChmFile;
						}

                        //TODO--PAUL - was the original correct?
						MergeToC(globalItem.Children, curItem.Children, null);
					}
				}
			}
		}

		/// <summary>
		/// Checks if the item is part of the merged-links
		/// </summary>
		/// <param name="name">name of the topic</param>
		/// <param name="local">local of the topic</param>
		/// <param name="openFiles">An arraylist of CHMFile instances.</param>
		/// <returns>Returns true if this item is part of the merged-links</returns>
        private bool IsMergedItem(string name, string local,
            IList<CHMFile> openFiles)
		{
			if(openFiles==null)
				return false;

            foreach (CHMFile curFile in openFiles)
            {
                foreach (HtmlHelpTocItem curItem in curFile.MergLinks)
                    if ((curItem.Name == name) && (curItem.Local == local))
                        return true;
            }
			return false;
		}

		/// <summary>
		/// Checks if a topicname exists in a SINGLE toc level 
		/// </summary>
		/// <param name="arrToC">toc list</param>
		/// <param name="Topic">topic to search</param>
		/// <returns>Returns the topic item if found, otherwise null</returns>
		private HtmlHelpTocItem ContainsToC(IList<HtmlHelpTocItem> arrToC, string Topic)
		{
			foreach(HtmlHelpTocItem curItem in arrToC)
			{
				if(curItem.Name == Topic)
					return curItem;
			}

			return null;
		}
		
		/// <summary>
		/// Searches the table of contents for a special topic
		/// </summary>
		/// <param name="topic">topic to search</param>
		/// <returns>Returns an instance of HtmlHelpTocItem if found, otherwise null</returns>
		public HtmlHelpTocItem SearchTopic(string topic)
		{
			return SearchTopic(topic, _toc);
		}

		/// <summary>
		/// Internal recursive tree search
		/// </summary>
		/// <param name="topic">topic to search</param>
		/// <param name="searchIn">tree level list to look in</param>
		/// <returns>Returns an instance of HtmlHelpTocItem if found, otherwise null</returns>
		private HtmlHelpTocItem SearchTopic(string topic, IList<HtmlHelpTocItem> searchIn)
		{
			foreach(HtmlHelpTocItem curItem in searchIn)
			{
				if(curItem.Name.ToLower() == topic.ToLower() )
					return curItem;

				if(curItem.Children.Count>0)
				{
					HtmlHelpTocItem nf = SearchTopic(topic, curItem.Children);
					if(nf != null)
						return nf;
				}
			}

			return null;
		}
	}
}
