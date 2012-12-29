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
using System.Text.RegularExpressions;
using Common.Helpers;

namespace Iris.VimRegexConversion {
	internal static class ScanHelper {
		public static string ConvertQuantifier(string vimQuantifier) {
			ArgumentValidator.ThrowIfNullOrEmpty(vimQuantifier, "vimQuantifier");

			string quantifier;
			
			Match m = m_quantifierRegex.Match(vimQuantifier);
			if (!m.Success) {
				string msg = "Cannot parse vim quantifier: " + vimQuantifier;
				throw (new AssertionViolationException(msg));
			}
			
			if (!m.Groups["comma"].Success) {
				if (m.Groups["firstNumber"].Success) {
					quantifier = "{" + m.Groups["firstNumber"].Value + "}";
				} else {
					quantifier = "*";
				}
			} else {
				string firstNumber = m.Groups["firstNumber"].Success ? m.Groups["firstNumber"].Value : "0";
				quantifier = "{" + firstNumber + "," + m.Groups["secondNumber"].Value + "}";	
			}
			
			if (m.Groups["nonGreedy"].Success) {
				quantifier = quantifier + "?";
			}

			return quantifier;
		}

		public static Regex m_quantifierRegex = new Regex(@"{  (?<nonGreedy>-)?  (?<firstNumber>\d+)?  (?<comma>,)?  (?<secondNumber>\d+)? \\? }",
				RegexOptions.Compiled |
				RegexOptions.IgnorePatternWhitespace);
	}
}
