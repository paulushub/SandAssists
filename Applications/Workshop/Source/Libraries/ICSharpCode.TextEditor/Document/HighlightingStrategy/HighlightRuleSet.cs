// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 3036 $</version>
// </file>

using System;
using System.Xml;
using System.Collections.Generic;

using ICSharpCode.TextEditor.Util;

namespace ICSharpCode.TextEditor.Document
{
	public sealed class HighlightRuleSet
	{
		private LookupTable keyWords;
        private List<Span> spans;
        private LookupTable prevMarkers;
        private LookupTable nextMarkers;
        private char escapeCharacter;

        private bool ignoreCase;
        private string name;

        private bool[] delimiters;

        private string reference;

        public HighlightRuleSet()
        {
            spans       = new List<Span>();
            delimiters  = new bool[256];

            keyWords    = new LookupTable(false);
            prevMarkers = new LookupTable(false);
            nextMarkers = new LookupTable(false);
        }
		
		public HighlightRuleSet(XmlElement el)
		{
            spans      = new List<Span>();
            delimiters = new bool[256];

			XmlNodeList nodes;
			
			if (el.Attributes["name"] != null) {
				Name = el.Attributes["name"].InnerText;
			}
			
			if (el.HasAttribute("escapecharacter")) {
				escapeCharacter = el.GetAttribute("escapecharacter")[0];
			}
			
			if (el.Attributes["reference"] != null) {
				reference = el.Attributes["reference"].InnerText;
			}
			
			if (el.Attributes["ignorecase"] != null) {
				ignoreCase  = Boolean.Parse(el.Attributes["ignorecase"].InnerText);
			}

            for (int i = 0; i < delimiters.Length; ++i)
            {
				delimiters[i] = false;
			}
			
			if (el["Delimiters"] != null) {
				string delimiterString = el["Delimiters"].InnerText;
				foreach (char ch in delimiterString) {
					delimiters[(int)ch] = true;
				}
			}
			
//			Spans       = new LookupTable(!IgnoreCase);

			keyWords    = new LookupTable(!IgnoreCase);
			prevMarkers = new LookupTable(!IgnoreCase);
			nextMarkers = new LookupTable(!IgnoreCase);
			
			nodes = el.GetElementsByTagName("KeyWords");
			foreach (XmlElement el2 in nodes) {
				HighlightColor color = new HighlightColor(el2);
				
				XmlNodeList keys = el2.GetElementsByTagName("Key");
				foreach (XmlElement node in keys) {
					keyWords[node.Attributes["word"].InnerText] = color;
				}
			}
			
			nodes = el.GetElementsByTagName("Span");
			foreach (XmlElement el2 in nodes) {
				Spans.Add(new Span(el2));
				/*
				Span span = new Span(el2);
				Spans[span.Begin] = span;*/
			}
			
			nodes = el.GetElementsByTagName("MarkPrevious");
			foreach (XmlElement el2 in nodes) {
				PrevMarker prev = new PrevMarker(el2);
				prevMarkers[prev.What] = prev;
			}
			
			nodes = el.GetElementsByTagName("MarkFollowing");
			foreach (XmlElement el2 in nodes) {
				NextMarker next = new NextMarker(el2);
				nextMarkers[next.What] = next;
			}
		}

        public IList<Span> Spans
        {
            get
            {
                return spans;
            }
        }
		
		internal IHighlightingStrategyUsingRuleSets Highlighter;
		
		public LookupTable KeyWords {
			get {
				return keyWords;
			}
		}
		
		public LookupTable PrevMarkers {
			get {
				return prevMarkers;
			}
		}
		
		public LookupTable NextMarkers {
			get {
				return nextMarkers;
			}
		}
		
		public bool[] Delimiters {
			get {
				return delimiters;
			}
		}
		
		public char EscapeCharacter {
			get {
				return escapeCharacter;
			}
		}
		
		public bool IgnoreCase {
			get {
				return ignoreCase;
			}
		}
		
		public string Name {
			get {
				return name;
			}
			set {
				name = value;
			}
		}
		
		public string Reference {
			get {
				return reference;
			}
		}
		
		/// <summary>
		/// Merges spans etc. from the other rule set into this rule set.
		/// </summary>
		public void MergeFrom(HighlightRuleSet ruleSet)
		{
            int itemCount = delimiters.Length;
			for (int i = 0; i < itemCount; i++) 
            {
				delimiters[i] |= ruleSet.delimiters[i];
			}
			// insert merged spans in front of old spans
			List<Span> oldSpans = spans;
            spans = new List<Span>(ruleSet.spans);
			spans.AddRange(oldSpans);
			//keyWords.MergeFrom(ruleSet.keyWords);
			//prevMarkers.MergeFrom(ruleSet.prevMarkers);
			//nextMarkers.MergeFrom(ruleSet.nextMarkers);
		}
	}
}
