using System;

using XsdDocumentation.Model;

namespace XsdDocumentation.Markup
{
    public sealed class ListItem
    {
        public ArtItem ArtItem { get; set; }
        public Topic Topic { get; set; }
        public string SummaryMarkup { get; set; }
    }
}