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
using System.Globalization;
using Common.Helpers;

namespace Common.ExpressionEvaluation {
	public struct Expression {
		#region Public members

		public static Expression operator +(Expression a, Expression b) {
			if (a.Is<string>() && b.Is<string>()) {
				return new Expression(a.As<string>() + b.As<string>());
			} else if (a.IsNumber && b.IsNumber) {
				return DoMath(a, b, (i1, i2) => i1 + i2, (d1, d2) => d1 + d2);
			}

			throw new EvaluationException(StringExtensions.Fi("Can't add {0} and {1}", a, b));
		}

		public static Expression operator /(Expression a, Expression b) {
			if (a.IsNumber && b.IsNumber) {
				return DoMath(a, b, (i1, i2) => i1/i2, (d1, d2) => d1/d2);
			}

			throw new EvaluationException(StringExtensions.Fi("Can't divide {0} and {1}", a, b));
		}

		public static Expression operator ==(Expression a, Expression b) {
			return new Expression(a.Equals(b));
		}

		public static bool operator false(Expression a) {
			return !a.As<bool>();
		}

		public static Expression operator >(Expression a, Expression b) {
			return DoRelop(a, b, (d1, d2) => d1 > d2);
		}

		public static Expression operator >=(Expression a, Expression b) {
			return DoRelop(a, b, (d1, d2) => d1 >= d2);
		}

		public static Expression operator !=(Expression a, Expression b) {
			return !(a == b);
		}

		public static Expression operator <(Expression a, Expression b) {
			return DoRelop(a, b, (d1, d2) => d1 < d2);
		}

		public static Expression operator <=(Expression a, Expression b) {
			return DoRelop(a, b, (d1, d2) => d1 <= d2);
		}

		public static Expression operator !(Expression a) {
			return new Expression(!a.As<bool>());
		}

		public static Expression operator %(Expression a, Expression b) {
			if (a.Is<int>() && b.Is<int>()) {
				return new Expression(a.As<int>()%b.As<int>());
			}

			throw new EvaluationException(StringExtensions.Fi("Can't take modulo of {0} and {1}", a, b));
		}

		public static Expression operator *(Expression a, Expression b) {
			if (a.IsNumber && b.IsNumber) {
				return DoMath(a, b, (i1, i2) => i1* i2, (d1, d2) => d1* d2);
			}

			throw new EvaluationException(StringExtensions.Fi("Can't multiply {0} and {1}", a, b));
		}

		public static Expression operator -(Expression a, Expression b) {
			if (a.IsNumber && b.IsNumber) {
				return DoMath(a, b, (i1, i2) => i1 - i2, (d1, d2) => d1 - d2);
			}

			throw new EvaluationException(StringExtensions.Fi("Can't subtract {0} and {1}", a, b));
		}

		public static bool operator true(Expression a) {
			return a.As<bool>();
		}

		public static Expression operator -(Expression a) {
			if (a.IsNumber) {
				return DoMath(new Expression(0), a, (i1, i2) => i1 - i2, (d1, d2) => d1 - d2);
			}

			throw new EvaluationException(StringExtensions.Fi("Can't take negative of {0}", a));
		}

		public Expression(object value) {
			this.m_value = value;
			this.ExpressionList = new List<Expression>(2);
		}

		public bool IsNull {
			get { return null == this.m_value; }
		}

		public bool IsNumber {
			get { return !this.IsNull && (this.m_value.GetType() == typeof (double) || this.m_value.GetType() == typeof (int)); }
		}

		public string TypeName {
			get { return this.IsNull ? "null" : this.m_value.GetType().Name; }
		}

		public object Value {
			get { return this.m_value; }
		}

		public static Expression DoMath(Expression a, Expression b, Func<int, int, int> integerOp, Func<double, double, double> doubleOp) {
			if (a.Is<double>()) {
				return new Expression(doubleOp(a.As<double>(), b.As<double>()));
			} else if (a.Is<int>()) {
				if (b.Is<int>()) {
					return new Expression(integerOp(a.As<int>(), b.As<int>()));
				} else {
					return new Expression(doubleOp(a.As<double>(), b.As<double>()));
				}
			}

			throw new EvaluationException(StringExtensions.Fi("Can't do math on {0} and {1}", a, b));
		}

		public static Expression DotOperator(Expression a, Expression b) {
			if (a.Is<string>() && b.Is<string>()) {
				return new Expression(a.As<string>() + b.As<string>());
			}

			throw new EvaluationException(StringExtensions.Fi("Can't apply dot operator (.) to {0} and {1}", a, b));
		}

		public T As<T>() {
			if (this.IsNull) {
				return default(T);
			}

			if (typeof (T) == this.m_value.GetType()) {
				return (T) this.m_value;
			} else {
				return (T) Convert.ChangeType(this.m_value, typeof (T), CultureInfo.InvariantCulture);
			}
		}

		public override bool Equals(object obj) {
			if (!(obj is Expression)) {
				return false;
			}

			Expression other = (Expression) obj;
			if (null == this.m_value) {
				return other.m_value == null;
			}

			return this.m_value.Equals(((Expression) obj).m_value);
		}

		public override int GetHashCode() {
			return this.m_value.GetHashCode();
		}

		public bool Is<T>() {
			return !this.IsNull && typeof (T) == this.m_value.GetType();
		}

		public override string ToString() {
			return this.IsNull ? "<null>" : StringExtensions.Fi("<{0}: {1}>", this.m_value.GetType().Name, this.m_value.ToString());
		}

		#endregion

		#region Internal and Private members

		internal List<Expression> ExpressionList;
		private object m_value;

		internal void AddExpressionToList(Expression expression) {
			this.ExpressionList.Add(expression);
		}

		private static Expression DoRelop(Expression a, Expression b, Func<double, double, bool> relop) {
			if (a.IsNumber && b.IsNumber) {
				return new Expression(relop(a.As<double>(), b.As<double>()));
			}

			throw new EvaluationException(StringExtensions.Fi("Can't compare {0} to {1}", a, b));
		}

		#endregion
	}
}