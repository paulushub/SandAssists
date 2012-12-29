/*
Copyright (c) 2007 Gustavo G. Duarte (http://duartes.org/gustavo)

Permission is hereby granted, free of charge, to any person obtaining a copy of 
this software and associated documentation files (the Software), to deal in 
the Software without restriction, including without limitation the rights to 
use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
of the Software, and to permit persons to whom the Software is furnished to do 
so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all 
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED AS IS, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE 
SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Text;

namespace Iris.Highlighting {
	/// <summary>
	/// Specifies how a fragment of text should be highlighted (eg, Comment, Type, Number, Normal)
	/// </summary>
	/// <remarks>
	/// <para>This class is fundamentally important in Iris. All of the <see href="http://www.vim.org">Vim</see> syntax scripts used by Iris assign
	/// a <see cref="HighlightMode" /> to each and every part of their input text. So all languages, from ABAP to Zsh, boil down to the same
	/// constituents, which are the members of the <see cref="HighlightMode" /> enumeration. Users can then control the appearance of each
	/// of the modes, which works well because the same color scheme can work for several (hundred) languages.</para>
	/// </remarks>
	/// <devdoc>
	/// IMPORTANT: It's assumed throughout the code base that this enum starts at 0 and steps by 1 for each possible value.
	/// </devdoc>
	public enum HighlightMode : byte {
		/// <summary>
		/// No text should ever have this mode. This indicates a bug in Iris
		/// </summary>
		IrisBug = 0,

		/// <summary>
		/// Used rarely when a Vim script doesn't properly choose a mode for a group of syntax items
		/// </summary>
		Unknown,

		/// <summary>
		/// Regular, 'plain' text that is not magic for a given syntax.
		/// </summary>
		Normal,
		
		/// <summary>
		/// Comment
		/// </summary>
		Comment,

		/// <summary>
		/// Booleans, often constants like 'true' and 'false'
		/// </summary>
		Boolean,
		/// <summary>
		/// Usually a one-character literal in a language, such as 'p'
		/// </summary>
		Character,
		/// <summary>
		/// Constant
		/// </summary>
		Constant,
		/// <summary>
		/// Floating point number
		/// </summary>
		Float,
		/// <summary>
		/// Number
		/// </summary>
		Number,
		/// <summary>
		/// String literal
		/// </summary>
		String,

		/// <summary>
		/// In some languages, variable names get this mode
		/// </summary>
		Identifier,
		/// <summary>
		/// Function names and sometimes function definitions
		/// </summary>
		Function,

		/// <summary>
		/// Normally keywords such as 'if', 'else', etc.
		/// </summary>
		Conditional,
		/// <summary>
		/// Exception-related stuff.
		/// </summary>
		Exception,
		/// <summary>
		/// Keyword
		/// </summary>
		Keyword,
		/// <summary>
		/// Labels, for example in goto and switch statements
		/// </summary>
		Label,
		/// <summary>
		/// Operators like + and - that you learned in elementary school
		/// </summary>
		Operator,
		/// <summary>
		/// Loopy statements like while and for.
		/// </summary>
		Repeat,
		/// <summary>
		/// Statement
		/// </summary>
		Statement,

		/// <summary>
		/// Used in various way, for example #DEFINEs in C
		/// </summary>
		Define,
		/// <summary>
		/// File inclusion, as #include in C
		/// </summary>
		Include,
		/// <summary>
		/// Macro definition or usage
		/// </summary>
		Macro,
		/// <summary>
		/// Normally a pre-processor condition
		/// </summary>
		PreCondit,
		/// <summary>
		/// Pre-processor directive
		/// </summary>
		PreProc,

		/// <summary>
		/// Varies with language, but for example 'static' in C-type languages
		/// </summary>
		StorageClass,
		/// <summary>
		/// Example: struct in C
		/// </summary>
		Structure,
		/// <summary>
		/// Type of an identifier
		/// </summary>
		Type,
		/// <summary>
		/// Type definition
		/// </summary>
		Typedef,

		/// <summary>
		/// Debugging-related text
		/// </summary>
		Debug,
		/// <summary>
		/// Delimiters. Sometimes [, ], quotes, and so on.
		/// </summary>
		Delimiter,
		/// <summary>
		/// My mom thinks I'm special
		/// </summary>
		Special,
		/// <summary>
		/// Special character
		/// </summary>
		SpecialChar,
		/// <summary>
		/// Special comments
		/// </summary>
		SpecialComment,
		/// <summary>
		/// Tags
		/// </summary>
		Tag,

		/// <summary>
		/// Stuff that is meant to be underlined
		/// </summary>
		Underlined,

		/// <summary>
		/// This usually flags a syntax error in the input that was detected by the syntax script.
		/// </summary>
		Error,
		
		/// <summary>
		/// Todo notes in languages often get this mode
		/// </summary>
		Todo
	}
}
