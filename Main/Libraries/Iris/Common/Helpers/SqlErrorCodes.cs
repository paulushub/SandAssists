/*
Copyright (c) 2006 Gustavo G. Duarte (http://duartes.org/gustavo)

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
using System.Data.SqlClient;
using System.Text;

namespace Common.Helpers {
	/// <summary>
	/// Represents SQL Error Codes
	/// </summary>
	public static class SqlErrorCodes {
		/// <summary>
		/// SQL Server Error due to a Unique Index/Constraint being violated.
		/// </summary>
		public const int UniqueIndexViolation = 2601;
		
		public const int PrimaryKeyViolation = 2627;
		
		public static bool IsUniquenessViolation(SqlException ex) {
			ArgumentValidator.ThrowIfNull(ex, "ex");
			
			return (ex.Number == UniqueIndexViolation) || (ex.Number == PrimaryKeyViolation);
		}
	}
}
