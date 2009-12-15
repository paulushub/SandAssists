// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Mike Krüger" email="mike@icsharpcode.net"/>
//     <version>$Revision: 1965 $</version>
// </file>

using System;
using System.Collections.Generic;

namespace ICSharpCode.Core
{
	public interface IStringTagProvider : IEquatable<IStringTagProvider> 
	{
        string ID
        {
            get;
        }

        IEnumerable<string> Tags
        {
			get;
		}

        string this[string tag]
        {
            get;
            set;
        }

        bool Contains(string tag);
		string Convert(string tag);
	}
}
