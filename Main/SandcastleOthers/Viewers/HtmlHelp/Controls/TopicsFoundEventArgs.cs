using System;
using System.Collections.Generic;

namespace Sandcastle.Viewers.HtmlHelp.Controls
{
	/// <summary>
	/// The class <c>TopicsFoundEventArgs</c> implements event arguments for the TopicsFound event of the index 
	/// user control. 
	/// </summary>
	/// <remarks>If you don't add an event handler for this event, the user control will show its own dialog 
	/// with the found topics.</remarks>
    public class TopicsFoundEventArgs : EventArgs
	{
        private IList<HtmlHelpIndexTopic> _topics;

		/// <summary>
		/// Constructor of the class
		/// </summary>
		/// <param name="topics">topics found</param>
        public TopicsFoundEventArgs(IList<HtmlHelpIndexTopic> topics)
		{
			_topics = topics;
		}

		/// <summary>
		/// Gets an arraylist containing all found topics.
		/// </summary>
		/// <remarks>Each item is of type IndexTopic.</remarks>
        public IList<HtmlHelpIndexTopic> Topics
		{
			get { return _topics; }
		}
	}
}
