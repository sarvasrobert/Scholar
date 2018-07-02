using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Mathematics;
using CG2.Lighting;

namespace CG2.Shading
{
	public class Checker : Shader
	{
		#region Properties

        // Even material
		public Shader Shader0;
        // Odd material
		public Shader Shader1;
        // Cube size should be included to calculations
		public Double CubeSize = 1;

		#endregion


		#region Shading

        public override Vector4 GetColor(Vector4 point, Vector4 normal, Vector4 eyeDir, Vector4 lightDir, double lightIntensity, Light light, Boolean InShadow)
		{
            Double bias = 0.00001;
            
            //Floor is number rounding 
            Int64 dx = (Int32)Math.Floor(point.X / CubeSize + bias);
            Int64 dy = (Int32)Math.Floor(point.Y / CubeSize + bias);
            Int64 dz = (Int32)Math.Floor(point.Z / CubeSize + bias);

			return (((dx + dy + dz) % 2) == 0) ?
                Shader0.GetColor(point, normal, eyeDir, lightDir, lightIntensity, light, InShadow) :
                Shader1.GetColor(point, normal, eyeDir, lightDir, lightIntensity, light, InShadow);
		}

		#endregion
	}
}
