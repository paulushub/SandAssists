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
using System.Text;

namespace Iris.VimRegexConversion {
	public class ConversionOptions {
		static ConversionOptions() {
			DefaultMultiLine = new ConversionOptions();
			DefaultMultiLine.ForceMultilineMode = true;

			CaseInsensitiveMultiline = new ConversionOptions();
			CaseInsensitiveMultiline.IgnoreCase = true;
			CaseInsensitiveMultiline.ForceMultilineMode = true;
		}
		
		public string IdentChars;
		public string KeywordChars;
		public string printableChars;
		public string FileNameChars;

		public Magicness Magicness;
		public bool IgnoreCase;

		public bool ForceMultilineMode;

		public string NewLine;
		
		public ConversionOptions() {
			this.IdentChars = @"_A-Za-z\x80-\xA7\xE0-\xED";
			this.KeywordChars = @"_A-Za-z\x80-\xA7\xE0-\xEB";
			this.printableChars = @"\x20-\x2F\x3A-\x7E\x80-\xFF";
			this.FileNameChars = @"A-Za-z\x80-\xA7\xE0-\xED\d/\\.\-_+,\#$%{}[\]:@!~=";
			
			this.Magicness = Magicness.Magic;
			this.IgnoreCase = false;

			this.NewLine = @"\n";
		}

		public static readonly ConversionOptions DefaultMultiLine;
		public static readonly ConversionOptions CaseInsensitiveMultiline;
	}
}
