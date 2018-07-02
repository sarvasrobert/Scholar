using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Mathematics;
using CG2.Lighting;

namespace CG2.Shading
{
    class CookTorrance : Phong
    {
        // set important material values
        Double roughnessValue = 0.3; // 0 : smooth, 1: rough
        Double F0 = 0.7; // Fresnel reflectance at normal incidence

        #region Init

        public CookTorrance()
        {
        }

        public CookTorrance(Phong shader)
        {
            AmbientColor = shader.AmbientColor;
            DiffuseColor = shader.DiffuseColor;
            SpecularColor = shader.SpecularColor;
            Shininess = shader.Shininess;
        }

        #endregion

        #region Shading

        public override Vector4 GetColor(Vector4 point, Vector4 normal, Vector4 eyeDir, Vector4 lightDir, Double lightIntensity, Light light, Boolean InShadow)
        {
            //ToDo: calculate diffuse factor do not forget to clamp negative values
            Double NdotL = 0;
            //Cook-Torrance shader calculates new value for specular factor
            Double specular = 0.0;
            //calculate only if diffuse factor is greater than zero
            if (NdotL > 0.0)
            {
                //ToDo: calculate intermediary values of dot products used later don't forget to clamp negative values
                //ToDo: calculate half vector

                Vector4 half = (eyeDir + lightDir).Normalized;
                //ToDo: calculate normal dot half
                double k = half * normal;
                //ToDo: calculate geometric attenuation

                // Hint: more info in slides
                double GA = 1.0;
                double GB = (2.0 * ((normal * half) * (normal * eyeDir))) / (eyeDir * half);
                double GC = (2.0 * ((normal * half) * (normal * lightDir))) / (lightDir * half);

                double G = Math.Min(Math.Min(GA, GB),GC);
                //ToDo: implement roughness (or: microfacet distribution function)
                // Hint: you can use Beckmann distribution function from slides

                // ToDo: implement Fresnel effect with Schlick approximation
                // Hint: more info in slides

                //resulting specular factor is equal to (fresnel * geoAtt * roughness) / (NdotV * NdotL * Math.PI)

            }

            Vector4 fColor = AmbientColor;
            fColor += lightIntensity * NdotL * (specular * SpecularColor + DiffuseColor) ^ light.DiffuseColor;
            return fColor;
        }

        #endregion
    }
}
