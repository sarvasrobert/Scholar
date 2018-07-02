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

        /// <summary>
        /// Calculates reflection vector
        /// </summary>
        /// <param name="normal">Unit surface normal</param>
        /// <param name="dir">Direction vector</param>
        /// <param name="result">Reflected direction vector</param>
        public static void Reflect(Vector4 normal, Vector4 dir, out Vector4 result)
        {
            result = (2 * (normal * dir)) * normal - dir;
        }

        /// <summary>
        /// Calculates refraction vector (n - unit normal, v - view vector, k = n1/n2 - n1,n2 refraction indices)
        /// </summary>
        /// <param name="normal">Unit surface normal</param>
        /// <param name="dir">Direction vector</param>
        /// <param name="refIndexRatio">Ratio of refraction indices of incident materials (k = n1/n2)</param>
        /// <param name="result">Refracted direction vector</param>
        /// <returns></returns>
        public static Boolean Refract(Vector4 normal, Vector4 dir, Double refIndexRatio, out Vector4 result)
        {
            /*     d    
             *   \--->|a 
             *    \   |      ^
             *   u \  | c    | n
             *      \a|      |
             *  n1   vv      |
             *  -------------- 
             *  n2    |\
             *        |b\
             *      e |  \ v
             *        |   \
             *        |    \
             *        v---->v
             *           f
             * 
             *  |n| = |u| = |v| = 1          // assumption
             *  
             *  k := n2/n1 = sin(b)/sin(a)   // Snell Law
             *  nu := (n*u) = cos(a)
             *  knu := k*nu = k*(n*u) 
             *  
             *  c = nu*n;
             *  d = u - c = u - nu*n;
             *  
             *  |c| = nu*|n| = nu
             *  |d| = sqrt(|u|^2 - |c|^2) = sqrt(1 - nu^2)
             *  
             *  |f| = k*|d| = k*sqrt(1 - nu^2)
             *  |e| = sqrt(|v|^2 - |f|^2) = sqrt(1 - k^2 * (1 - nu^2)) = sqrt(1 - k^2 + k^2 * nu^2) = sqrt(1 - k^2 + knu^2)
             *  
             *  e = |e|*(-n) = -sqrt(1 - k^2 + knu^2)*n
             *  f = |f| * (d / |d|) = |f|/|d| * d = k*d = k*(u - c) = k*(u - nu*n) = k*u - knu*n
             *  
             *  v = e + f = f + e = k*u - knu*n - sqrt(1 - k^2 + knu^2)*n
             *    = k*u - (knu + sqrt(1 - k^2 + knu^2))*n
             */

            Double nu = (normal * dir);
            Double knu = refIndexRatio * nu;
            Double ee = 1 - refIndexRatio * refIndexRatio + knu * knu; // = |e|^2

            if (ee > 0) // refraction is valid only when |e|^2 > 0
            {
                result = refIndexRatio * dir - (knu + Math.Sqrt(ee)) * normal;
                return true;
            }
            else // total internal reflection
            {
                result = dir - (2 * nu) * normal;
                return false;
            }
        }

		#endregion

	}
}
