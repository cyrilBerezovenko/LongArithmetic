using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LongArithmetic {
	class Program {

		static void Main(string[] args) {

			Lint a = new Lint("6131066257800");
			PrimeFactorizer factorizer = new PrimeFactorizer(19, 331, 1009, 4663, 6661);
			factorizer.Factorize(a).ForEach(Console.WriteLine);
		}
	}
}
