using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Mathematics;
using CG2.Rendering;
using CG2.Shading;

namespace CG2.Modeling
{
	public class Sphere : Model
	{
		#region Properties

		public Vector4 Origin;
		public Double Radius;

		#endregion


		#region Init

		public Sphere()
		{
		}

		public Sphere(Shader shader, Vector4 origin, Double radius)
		{
            Shader = shader;
			Origin = origin;
			Radius = radius;
		}

		#endregion


		#region Raytracing

		public override void Collide(Ray ray)
		{
			Collide(ray, this);
		}

		public static void Collide(Ray ray, Sphere sphere)
		{
			Vector4 oo = ray.Origin - sphere.Origin; // (o_2 - o_1)

			Double A = ray.Direction * ray.Direction; // A = v^2
            Double B = -2.0 * oo * ray.Direction; // -B = v^T * (o_2 - o_1)
			Double C = oo * oo - sphere.Radius * sphere.Radius; // C = (o_2 - o_1)^2 - r^2
			Double D = B * B - 4.0f * A * C; // discriminant

			if (D < 0) return; // no collision

			Double sD = Math.Sqrt(D);
			Double t1 = 0.5 * (B + sD) / A; if (t1 < ray.Bias) t1 = Double.MaxValue;
			Double t2 = 0.5 * (B - sD) / A; if (t2 < ray.Bias) t2 = Double.MaxValue;
			Double t = (t1 < t2) ? t1 : t2;
            ray.HitModelCount++;
			if (t >= ray.HitParam) return; // collisions beyond current ray param are ignored

			ray.HitModel = sphere;
			ray.HitParam = t;

            // Yes, normal vector is also important
            ray.HitNormal = (ray.GetHitPoint() - sphere.Origin).Normalized;
            //ray.HitModelCount++;
		}

	
		#endregion
	}
}
