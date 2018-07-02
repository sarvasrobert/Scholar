using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Mathematics;
using CG2.Lighting;

namespace CG2.Shading
{
	public class Phong : Shader
	{
		#region Properties

        // Default object color
        public Vector4 DiffuseColor = new Vector4(1, 1, 1); 
		public Vector4 SpecularColor = new Vector4(0.5, 0.5, 0.5);
		public Vector4 AmbientColor = new Vector4(0, 0, 0);

		public Double Shininess = 40;

		#endregion


		#region Init

        //Hint: We can create phong shader with different parameters
        //      Default is phong with just a color
        //      Try to change specular and ambient color to see the behaviour
		public Phong()
		{
		}

		public Phong(Vector4 diffuseColor)
		{
			DiffuseColor = diffuseColor;
		}

		public Phong(Vector4 diffuseColor, Vector4 specularColor)
		{
			DiffuseColor = diffuseColor;
			SpecularColor = specularColor;
		}

        public Phong(Vector4 diffuseColor, Vector4 specularColor, Vector4 ambientColor)
		{
			DiffuseColor = diffuseColor;
			SpecularColor = specularColor;
			AmbientColor = ambientColor;
		}

		#endregion


		#region Shading

        public override Vector4 GetColor(Vector4 point, Vector4 normal, Vector4 eyeDir, Vector4 lightDir, Double lightIntensity, Light light, Boolean InShadow)
        {
            Double diffuseFactor = lightIntensity * (normal * lightDir);
            if (diffuseFactor < 0) diffuseFactor = 0;

            Vector4 half = (lightDir + eyeDir).Normalized;
            Double specularFactor = lightIntensity * Math.Pow(normal * half, Shininess);

            Vector4 color = new Vector4();
            color = diffuseFactor * (DiffuseColor ^ light.DiffuseColor);
            color += specularFactor * (SpecularColor ^ light.DiffuseColor);
            color += AmbientColor;

            if(InShadow)
                return diffuseFactor * (DiffuseColor ^ light.DiffuseColor) + AmbientColor;
            else
                return color;
		}

		#endregion
	}
}
