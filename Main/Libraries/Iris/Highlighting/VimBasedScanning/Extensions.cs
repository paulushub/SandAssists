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
	/// <summary>
	/// Sorry replacement for some C# 3.0-style methods.
	/// </summary>
	public static class Extensions {
		#region Public members

		/// <summary>
		/// Adds a new element to sorted list preserving the sort order.
		/// </summary>
		public static List<T> AddSorted<T>(List<T> sortedList, T newElement) {
			int index = sortedList.BinarySearch(newElement);
			if (index < 0) {
				sortedList.Insert(~index, newElement);
			} else {
				sortedList.Insert(index, newElement);
			}

			return sortedList;
		}

		/// <summary>
		/// Determines whether the given sorted list contains the given item
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="sortedList">The sorted list.</param>
		/// <param name="item">The item.</param>
		/// <returns>
		/// 	<c>true</c> if the specified sorted list contains sorted; otherwise, <c>false</c>.
		/// </returns>
		public static bool ContainsSorted<T>(List<T> sortedList, T item) {
			return sortedList.BinarySearch(item) > -1;
		}

		/// <summary>
		/// Returns the elements in a collection after removing duplicates, like DISTINCT in SQL.
		/// </summary>
		public static IEnumerable<T> Distinct<T>(IEnumerable<T> all) {
			List<T> final = new List<T>();

			foreach (T t in all) {
				if (!final.Contains(t)) {
					final.Add(t);
				}
			}

			return final;
		}

		/// <summary>
		/// Returns the set difference between two collections
		/// </summary>
		public static IEnumerable<T> Except<T>(IEnumerable<T> all, IEnumerable<T> toRemove) {
			List<T> final = new List<T>(all);

			foreach (T t in toRemove) {
				final.Remove(t);
			}

			return final;
		}

		/// <summary>
		/// Performs the specified action on each element in a collection
		/// </summary>
		public static IEnumerable<T> ForEach<T>(IEnumerable<T> colletion, VoidFunc<T> action) {
			foreach (T t in colletion) {
				action(t);
			}

			return colletion;
		}

		#endregion
	}
}