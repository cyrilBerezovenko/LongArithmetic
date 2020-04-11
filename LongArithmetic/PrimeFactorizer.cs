using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LongArithmetic {

	class PrimeFactorizer {

		private int[] p;
		private bool[][] mods;

		public PrimeFactorizer() {
			p = new int[] { 7, 11, 67 };
			InitMods();
		}

		public PrimeFactorizer(params int[] p) {
			this.p = p;
			InitMods();
		}

		private void InitMods() {
			mods = new bool[p.Length][];
			for (int i = 0; i < p.Length; i++) {
				mods[i] = new bool[p[i]];
				for (int j = 0; j < p[i]; j++) {
					mods[i][(j * j) % p[i]] = true;
				}
			}
		}

		private bool CanBeSquare(Lint n) {
			for (int i = 0; i < p.Length; i++) {
				var yy = n % p[i];
				if (!mods[i][yy])
					return false;
			}
			return true;
		}

		public List<Lint> Factorize(Lint n) {
			List<Lint> list = new List<Lint>();
			for (; n % 2 == 0; list.Add(2), n /= 2) ;

			for (Lint i = 0; ; i++) {

				Lint k = n + i * i;
				if (!CanBeSquare(k))
					continue;

				Lint root = k.SquareRoot();
				if (root * root != k)
					continue;

				Lint n1 = root - i;
				Lint n2 = root + i;
				if (n1 == 1 || n2 == 1)
					list.Add(n);
				else {
					list.AddRange(Factorize(n1));
					list.AddRange(Factorize(n2));
				}

				return list;
			}
		}
	}
}
