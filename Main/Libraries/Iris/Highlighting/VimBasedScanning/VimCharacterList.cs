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
using Common.Helpers;

namespace Iris.Highlighting.VimBasedScanning {
	internal class VimCharacterList {
		#region Public members

		public const string StandardKeywordCharacters = "@,48-57,_,128-167,224-235";

		public static void AddCharacters(bool[] characters, string characterList) {
			ArgumentValidator.ThrowIfNull(characters, "characters");
			ArgumentValidator.ThrowIfNull(characterList, "characterList");
			if (characters.Length != 256) {
				throw new ArgumentException(StringExtensions.Translate("charecters must be a bool array with 256 elements - one for each ASCII character"), "characters");
			}

			string[] pieces = characterList.Split(',');

			for (int i = 0; i < pieces.Length; i++) {
				// unescape chars (eg, \" becomes "). I don't think the escaping is necessary, but radiance.vim does it
				if (2 == pieces.Length && '\\' == pieces[i][0]) {
					pieces[i] = new string(pieces[i][1], 1);
				}

				// First we handle the tricky case for ,,, which is how you specify a comma. It leads to two empty strings in the pieces
				if (0 == pieces[i].Length) {
					if (i < pieces.Length - 1 && 0 == pieces[i + 1].Length) {
						characters[','] = true;
						i++;
					}

					continue;
				}

				if (pieces[i][0] == '^') {
					if (1 == pieces[i].Length) {
						if (i == pieces.Length - 1) {
							characters['^'] = true;
						} else if (0 == pieces[i + 1].Length) {
							// this handles the case of ^,, which is used to exclude a comma. It leads to pieces "^" and string.Empty
							characters[','] = false;
							i++;
						}

						continue;
					}

					ApplyRangeTo(characters, false, pieces[i].Substring(1));
				} else {
					ApplyRangeTo(characters, true, pieces[i]);
				}
			}
		}

		#endregion

		#region Internal and Private members

		private static void ApplyRangeTo(bool[] characters, bool result, string range) {
			int idxStart;
			int idxEnd;

			int asciiCode;

			if ("@-@" == range) {
				idxStart = idxEnd = '@';
			} else if ("@" == range) {
				// this one is tricky. We have to apply the result to all letters and accented letters
				for (int i = 'A'; i <= 'Z'; i++) {
					characters[i] = result;
					characters[i | 32] = result;
				}

				// 'Ç' to 'ù'
				for (int i = 128; i <= 151; i++) {
					characters[i] = result;
				}

				// 'á' to 'Ñ'
				for (int i = 160; i <= 165; i++) {
					characters[i] = result;
				}

				return;
			} else if (int.TryParse(range, out asciiCode)) {
				idxStart = idxEnd = asciiCode;
			} else if (1 == range.Length) {
				idxStart = idxEnd = range[0];
			} else {
				string[] startAndEnd = range.Split('-');
				if (startAndEnd.Length != 2) {
					string msg = StringExtensions.Fi("Invalid character range '{0}': multiple characters but dash (-) not found", range);
					throw new ArgumentException(msg, "range");
				}

				ParseRangeBoundary(startAndEnd[0], out idxStart);
				ParseRangeBoundary(startAndEnd[1], out idxEnd);
			}

			for (int i = idxStart; i <= idxEnd; i++) {
				characters[i] = result;
			}
		}

		private static void ParseRangeBoundary(string boundary, out int asciiCode) {
			if (!int.TryParse(boundary, out asciiCode)) {
				if (boundary.Length != 1) {
					string msg = StringExtensions.Fi("Invalid range boundary '{0}': it has multiple characters but it's not a valid number", boundary);
					throw new ArgumentException(msg, "boundary");
				}

				asciiCode = boundary[0];
			}

			if (asciiCode < 0 || 255 < asciiCode) {
				string msg = StringExtensions.Fi("Invalid ascii code {0} in boundary '{1}': ascii code must be between 0 and 255", asciiCode, boundary);
				throw new ArgumentException(msg, "boundary");
			}
		}

		#endregion
	}
}