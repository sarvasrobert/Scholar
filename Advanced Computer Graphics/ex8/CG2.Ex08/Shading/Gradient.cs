using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Mathematics;
using CG2.Lighting;

namespace CG2.Shading
{
    public class Gradient : Shader
    {
         // Even material
        public Shader Shader0;
        // Odd material
        public Shader Shader1;
        public Vector4 Q = new Vector4(0, 0, 0);
        public Vector4 v = new Vector4(1, 0, 0);

        public override Vector4 GetColor(Vector4 point, Vector4 normal, Vector4 eyeDir,
                                        Vector4 lightDir, Double lightIntensity, Light light, Boolean InShadow)
        {
            Double beta = ((point * 0.5 - Q) * v) / v.Length;
            //then we can calculate the alfa value in 1D
            Double alfa = (1.0 - Math.Cos(beta * Math.PI)) / 2.0;

            return (1.0 - alfa) * 
                Shader0.GetColor(point, normal, eyeDir, lightDir, lightIntensity, light, InShadow)
                + alfa * 
                Shader1.GetColor(point, normal, eyeDir, lightDir, lightIntensity, light, InShadow);
        }
    }
}
