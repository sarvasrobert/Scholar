using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Mathematics;
using CG2.Modeling;

namespace CG2.Rendering
{
	public class Ray
	{
		#region Properties
		public Vector4 Origin;
		public Vector4 Direction;

        public Double Bias = 0.01;
        public Double HitParam;
        public Vector4 HitNormal;
		public Model HitModel = null;

        // TODO: declare InModel being the model ray is currently passing through (apply for transparent objects only)
        public Model InModel = null;

        // TODO: declare ReflectionLevel and RefractionLevel being the interaction level of reflection and refraction
        public Int32 ReflectionLevel = 0;
        public Int32 RefractionLevel = 0;

		#endregion


		#region Init

		public Ray()
		{
		}

        public Ray(Vector4 origin, Vector4 direction, Model inModel, Int32 reflectionLevel, Int32 refractionLevel, Double zNear = 0.01, Double zFar = Double.MaxValue)
		{
            Set(origin, direction, inModel, reflectionLevel, refractionLevel, zNear, zFar);
		}

        public void Set(Vector4 origin, Vector4 direction, Model inModel, Int32 reflectionLevel, Int32 refractionLevel, Double zNear = 0.01, Double zFar = Double.MaxValue)
        {
            Origin = origin;
            Direction = direction;
            HitParam = zFar;
            Bias = zNear;

            // TODO: set given reflectionLevel and refractionLevel
            InModel = inModel;
            ReflectionLevel = reflectionLevel;
            RefractionLevel = refractionLevel;
        }

        public Vector4 GetHitPoint()
        {
            return Origin + HitParam * Direction;
        }

		#endregion
	}
}
