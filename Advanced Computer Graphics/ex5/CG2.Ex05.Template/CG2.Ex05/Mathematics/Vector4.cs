using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CG2.Mathematics
{
	public struct Vector4
	{
		#region Properties

		public Double X;
		public Double Y;
		public Double Z;
		public Double W;

		public Double Length2
		{
			get { return X * X + Y * Y + Z * Z + W * W; }
		}

		public Double Length
		{
			get { return Math.Sqrt(X * X + Y * Y + Z * Z + W * W); }
		}

		public Vector4 Normalized
		{
			get
			{
				Double ilength = 1/ Math.Sqrt(X * X + Y * Y + Z * Z + W * W);
				return new Vector4(ilength * X, ilength * Y, ilength * Z, ilength * W);
			}
		}

		public static Vector4 Zero
		{
			get { return new Vector4(0, 0, 0, 0); }
		}

		#endregion


		#region Init

		public Vector4(Double x, Double y, Double z, Double w = 0)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		#endregion


		#region Object

		public override String ToString()
		{
			return "(" + X.ToString("F2") + "; " + Y.ToString("F2") + "; " + Z.ToString("F2") + "; " + W.ToString("F2") + ")";
		}

		#endregion


		#region Arithmetic Operations

		public static Vector4 operator -(Vector4 a)
		{
			return new Vector4(-a.X, -a.Y, -a.Z, -a.W);
		}

		public static Vector4 operator +(Vector4 a, Vector4 b)
		{
			return new Vector4(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
		}

        public static Boolean operator ==(Vector4 a, Vector4 b)
        {
            if((a.X == b.X) && (a.Y == b.Y) && (a.Z == b.Z))
                return true;

            return false;
        }

        public static Boolean operator !=(Vector4 a, Vector4 b)
        {
            if ((a.X != b.X) && (a.Y != b.Y) && (a.Z != b.Z))
                return true;

            return false;
        }

		public static Vector4 operator -(Vector4 a, Vector4 b)
		{
			return new Vector4(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
		}

		public static Vector4 operator *(Vector4 a, Double b)
		{
			return new Vector4(a.X * b, a.Y * b, a.Z * b, a.W * b);
		}

		public static Vector4 operator *(Double a, Vector4 b)
		{
			return new Vector4(a * b.X, a * b.Y, a * b.Z, a * b.W);
		}

		/// <summary>
		/// Dot Product
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Double operator *(Vector4 a, Vector4 b)
		{
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z + a.W * b.W;
		}

		/// <summary>
		/// 3D Cross Product
		/// </summary>
		/// <param name="a"></param>
		/// <param name="b"></param>
		/// <returns></returns>
		public static Vector4 operator %(Vector4 a, Vector4 b)
		{
			return new Vector4(a.Y * b.Z - a.Z * b.Y, a.Z * b.X - a.X * b.Z, a.X * b.Y - a.Y * b.X, 0);
		}

        /// <summary>
        /// Modulation Product
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Vector4 operator ^(Vector4 a, Vector4 b)
        {
            return new Vector4(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
        }

		public static Vector4 Clamp(Vector4 v, Double min, Double max)
		{
			return new Vector4(
				(v.X < min) ? min : (v.X > max) ? max : v.X,
				(v.Y < min) ? min : (v.Y > max) ? max : v.Y,
				(v.Z < min) ? min : (v.Z > max) ? max : v.Z,
				(v.W < min) ? min : (v.W > max) ? max : v.W
			);
		}

		#endregion

	}
}
