using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Mathematics;
using CG2.Lighting;

namespace CG2.Shading
{
    class GradientShader : Shader
    {
        #region Properties

        // Even material
        public Shader Shader0;
        // Odd material
        public Shader Shader1;
        public Vector4 Q;
        public Vector4 v;

        #endregion


        #region Shading

        public override Vector4 GetColor(Vector4 point, Vector4 normal, Vector4 eyeDir, Vector4 lightDir, Double lightIntensity, Light light, Boolean InShadow)
        {
            //ToDo: create gradient shader
            // Hint: Use additional material > Computer Graphics - David Mount > http://goo.gl/q6Sz0
            //ToDo: gradient is done by alfa blending
            //ToDo: vector from point to gradient center is projected on gradient direction vector v
            //ToDo: then we can calculate the alfa value in 1D

            Vector4 p = point - Q;

            Double x = p * v.Normalized;


            double s = Math.PI / 3;
            double y = Math.Cos(Math.PI * x/s) * 0.5 + 0.5 ;

            Vector4 color1 = Shader0.GetColor(point, normal, eyeDir, lightDir, lightIntensity, light, InShadow);
            Vector4 color0 = Shader1.GetColor(point, normal, eyeDir, lightDir, lightIntensity, light, InShadow);
            return y * color0 + (1-y) * color1;
        }

        #endregion
    }
}
