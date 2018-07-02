using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Mathematics;
using CG2.Rendering;
using CG2.Shading;

namespace CG2.Modeling
{
	public class AABB : Model
	{
		#region Properties

		public Vector4 Min;
		public Vector4 Max;

		#endregion

		
		#region Init

		public AABB()
		{
		}

        public AABB(Shader shader, Vector4 min, Vector4 max)
		{
            Shader = shader;
			Min = min;
			Max = max;
		}

		#endregion

		
		#region Raytracing

		public override void Collide(Ray ray)
		{
			Collide(ray, this);
		}

		public static void Collide(Ray ray, AABB box)
		{
			Vector4 dMin = box.Min - ray.Origin;
			Vector4 dMax = box.Max - ray.Origin;
			Vector4 tMin = new Vector4(dMin.X / ray.Direction.X, dMin.Y / ray.Direction.Y, dMin.Z / ray.Direction.Z);
			Vector4 tMax = new Vector4(dMax.X / ray.Direction.X, dMax.Y / ray.Direction.Y, dMax.Z / ray.Direction.Z);

			Vector4 n = new Vector4(-1, -1, -1);

			if (tMax.X < tMin.X) { MathEx.Swap(ref tMin.X, ref tMax.X); n.X = +1; }
			if (tMax.Y < tMin.Y) { MathEx.Swap(ref tMin.Y, ref tMax.Y); n.Y = +1; }
			if (tMax.Z < tMin.Z) { MathEx.Swap(ref tMin.Z, ref tMax.Z); n.Z = +1; }

			Double t0 = MathEx.Max3(tMin.X, tMin.Y, tMin.Z);
			Double t1 = MathEx.Min3(tMax.X, tMax.Y, tMax.Z);

			if (t0 > t1) return; // no hit
            ray.HitModelCount++;
            if (t0 >= ray.HitParam || t1 < ray.Bias) return;

			if (t0 < ray.Bias) // we are inside box -> exiting (hit at t1)
			{
				t0 = t1;
				tMin = tMax;
				n = -n;
			}

			if (t0 == tMin.X) n = new Vector4(n.X, 0, 0);
			else if (t0 == tMin.Y) n = new Vector4(0, n.Y, 0);
			else if (t0 == tMin.Z) n = new Vector4(0, 0, n.Z);

			ray.HitModel = box;
			ray.HitParam = t0;
            ray.HitNormal = n;
            
		}

		#endregion
	}
}
