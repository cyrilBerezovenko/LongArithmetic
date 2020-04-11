using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LongArithmetic {

	class Lint {

		private List<uint> d;

		public int Length { get => d.Count; }

		public uint this[int i] {
			get => d[i];
			private set => d[i] = value;
		}

		public Lint(int n) {
			if (n < 0)
				throw new ArgumentException("Lint cannot be negative");
			FillList((uint)n);
		}

		public Lint(uint n) =>
			FillList(n);

		public Lint(List<uint> list) =>
			d = new List<uint>(list);

		public Lint(List<int> list) =>
			d = list.Select(@int => (uint)@int).ToList();

		public Lint(string s) {
			d = new List<uint>();
			s = s.Trim();

			if(!IsNumber(s))
				throw new ArgumentException("Provided string does not represent a number");

			for(int i = s.Length - 1; i >= 0; i--) {
				uint digit = (uint)(s[i] - '0');
				d.Add(digit);
			}
		}

		private bool IsNumber(string s) =>
			new Regex("^(0|[1-9][0-9]*)$").IsMatch(s);
		

		private void FillList(uint n) {
			d = new List<uint>();
			if(n == 0) {
				d.Add(0);
				return;
			}
			for (; n > 0; d.Add(n % 10), n /= 10);
		}

		public override string ToString() =>
			d.Select(u => u.ToString())
			 .Reverse()
			 .Aggregate((a, b) => a + b);

		public static implicit operator Lint(int @int) =>
			new Lint(@int);

		public static implicit operator Lint(uint @uint) =>
			new Lint(@uint);

		public static explicit operator int(Lint lint) {
			int res = 0;
			int ten = 1;
			for(int i = 0; i < lint.Length; i++) {
				checked {
					res += ten * (int)lint[i];
					ten *= 10;
				}
			}
			return res;
		}

		public static explicit operator uint(Lint lint) {
			uint res = 0;
			uint ten = 1;
			for (int i = 0; i < lint.Length; i++) {
				checked {
					res += ten * lint[i];
					ten *= 10;
				}
			}
			return res;
		}

		public static Lint operator ++ (Lint n) =>
			n += 1;

		public static Lint operator -- (Lint n) =>
			n -= 1;

		public static bool operator == (Lint first, Lint second) =>
			(first is null && second is null) || 
			( !(first is null) && !(second is null) && Enumerable.SequenceEqual(first.d, second.d) );

		public static bool operator != (Lint first, Lint second) =>
			!(first == second);

		public static bool operator > (Lint first, Lint second) {
			if (first.Length != second.Length)
				return first.Length > second.Length;

			int i;
			for(i = first.Length - 1; i >= 0 && first[i] == second[i]; i--);

			return i >= 0 && first[i] > second[i];
		}

		public static bool operator < (Lint first, Lint second) {
			if (first.Length != second.Length)
				return first.Length < second.Length;

			int i;
			for(i = first.Length - 1; i >= 0 && first[i] == second[i]; i--);

			return i >= 0 && first[i] < second[i];
		}

		public static bool operator >= (Lint first, Lint second) {
			if (first.Length != second.Length)
				return first.Length > second.Length;

			int i;
			for(i = first.Length - 1; i >= 0 && first[i] == second[i]; i--);

			return i == -1 ? true : first[i] > second[i];
		}

		public static bool operator <= (Lint first, Lint second) {
			if (first.Length != second.Length)
				return first.Length < second.Length;

			int i;
			for(i = first.Length - 1; i >= 0 && first[i] == second[i]; i--);

			return i == -1 ? true : first[i] < second[i];
		}

		public static Lint operator + (Lint first, Lint second) {
			Lint a = first > second ? first : second;
			Lint b = first <= second ? first : second;

			List<uint> result = new List<uint>();
			uint carry = 0;

			for (int i = 0; i < a.Length; i++) {
				uint sum = a[i] + (i < b.Length ? b[i] : 0) + carry;

				carry = sum / 10;
				result.Add(sum % 10);
			}

			if (carry != 0)
				result.Add(carry);

			return new Lint(result);
		}

		public static Lint operator - (Lint a, Lint b) {
			if (b > a)
				throw new ArgumentException("Result cannot be negative");

			List<uint> result = new List<uint>();
			uint carry = 0;

			for (int i = 0; i < a.Length; i++) {
				int diff = (int)a[i] - (int)(i < b.Length ? b[i] : 0) - (int)carry;
				carry = 0;

				if (diff < 0) {
					diff += 10;
					carry++;
				}
				result.Add((uint)diff);
			}

			if (result[0] == 0)
				return 0;

			result.Reverse();
			return new Lint(result.SkipWhile(n => n == 0).Reverse().ToList());
		}

		public static Lint operator * (Lint a, Lint b) {
			uint[] result = new uint[a.Length + b.Length];

			int ai = 0;
			int bi = 0;

			for (int i = 0; i < a.Length; i++, ai++) {
				uint carry = 0;
				bi = 0;

				for (int j = 0; j < b.Length; j++, bi++) {
					uint sum = a[i] * b[j] + result[ai + bi] + carry;

					carry = sum / 10;
					result[ai + bi] = sum % 10;
				}

				if (carry > 0)
					result[ai + bi] += carry;
			}

			int k = result.Length - 1;
			for(; k >= 0 && result[k] == 0; k--);
				
			if (k == -1)
				return 0;

			return new Lint(result.Take(k+1).ToList());
		}

		public static Lint operator / (Lint a, int b) =>
			b >= 0 ? a / (uint)b : throw new ArgumentException("Result cannot be negative");

		public static Lint operator / (Lint a, uint b) {
			List<uint> result = new List<uint>();

			int idx = a.Length - 1;
			uint temp = a[idx];

			while (temp < b && idx > 0) 
				temp = temp * 10 + a[--idx];
			--idx;

			while (idx >= 0) {
				result.Add(temp / b);

				temp = (temp % b) * 10 + a[idx];
				idx--;
			}
			result.Add(temp / b);

			result.Reverse();
			return new Lint(result);
		}

		public static int operator % (Lint a, int b) =>
			b >= 0 ? (int)(a - (a / b) * b) : throw new ArgumentException("Result cannot be negative");

		public static uint operator % (Lint a, uint b) =>
			(uint)(a - (a / b) * b);

		public Lint SquareRoot() {
			Lint lo = 0;
			Lint hi = this;
			Lint mid = 0;
			Lint prevMid = null;

			for (;;) {
				mid = (lo + hi) / 2;

				if (prevMid == mid)
					return mid;

				Lint midSq = mid * mid;
				if (midSq == this)
					return mid;
				else if (midSq > this)
					hi = mid;
				else
					lo = mid;

				prevMid = mid;
			}
		}
	}
}
