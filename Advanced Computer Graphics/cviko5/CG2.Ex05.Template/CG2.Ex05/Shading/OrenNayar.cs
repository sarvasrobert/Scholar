using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Mathematics;
using CG2.Lighting;

namespace CG2.Shading
{
    class OrenNayar : Phong
    {
        // set value depending on material; let the user set this from the outside instead...
        Double roughness = 1.0;

        #region Init

        public OrenNayar()
        {
        }

        public OrenNayar(Phong shader)
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
            //ToDo: calculate intermediary values
            //ToDo: calculate normal dot light_direction
            //ToDo: calculate normal dot view_direction

            //ToDo: calculate angle between normal and view_direction
            //ToDo: calculate angle between normal and light_direction

            double ndotL = lightDir * normal;
            //ToDo: calculate normal dot view_direction
            double Ndotv = eyeDir * normal;
            //ToDo: calculate view_direction dot half
            double NangV = Ndotv / (normal.Length * eyeDir.Length);
            double NangL = Ndotv / (normal.Length * lightDir.Length);
                //ToDo: square roughness of the material
            Double alpha = Math.Max(NangV, NangL);
            Double beta = Math.Min(NangL, NangV);

            double gama = (eyeDir - normal * Ndotv) * (lightDir - normal * ndotL);

            double roughSqr = roughness * roughness;

            double A = 1 - 0.5 * (roughSqr / (roughSqr + 0.57));

            double B = 0.45 * (roughSqr / (roughSqr + 0.09));

            double C = Math.Sin(alpha) * Math.Tan(beta);

            double L1 = Math.Max(0, ndotL) * (A + B * Math.Max(0, gama) * C);
            //ToDo: set alpha to max of the calculated angles
            //ToDo: set beta as min of the calculated angles
            //ToDo: calculate gamma value
            // Hint: more info in slides

            //ToDo: square roughness of the material

            //ToDo: calculate A
            // Hint: more info in slides
            //ToDo: calculate B
            // Hint: more info in slides
            //ToDo: calculate C
            // Hint: more info in slides

            //ToDo: put it all together
            // Hint: more info in slides

            // get the final color by summing both components
            Vector4 finalValue = AmbientColor;
            finalValue += DiffuseColor ^ light.DiffuseColor * (L1);

            return finalValue;
        }

        #endregion
    }
}
