using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Mathematics;
using CG2.Lighting;

namespace CG2.Shading
{
	public class Shader
	{
        public Double ReflectionFactor = 0;
        public Double RefractionFactor = 0;
        public Double RefractionIndex = 1.0;

		#region Shading

        public virtual Vector4 GetColor(Vector4 point, Vector4 normal, Vector4 eyeDir, 
                                        Vector4 lightDir, Double lightIntensity, Light light, Boolean InShadow)
		{
            return Vector4.Zero;
		}

		#endregion
	}
}
