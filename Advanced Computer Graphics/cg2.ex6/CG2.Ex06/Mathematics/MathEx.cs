using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CG2.Mathematics
{
	public static class MathEx
	{
		public static Double DegToRad(Double angleDeg)
		{
			return Math.PI / 180 * angleDeg;
		}

		public static Double RadToDeg(Double angleRad)
		{
			return 180 / Math.PI * angleRad;
		}

        public static void Swap<T>(ref T a, ref T b)
        {
            T t = a;
            a = b;
            b = t;
        }

        public static Double Min3(Double a, Double b, Double c)
        {
            return (a < b) ? (a < c ? a : c) : (b < c ? b : c);
        }

        public static Double Max3(Double a, Double b, Double c)
        {
            return (a > b) ? (a > c ? a : c) : (b > c ? b : c);
        }
	}
}
