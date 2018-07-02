using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Mathematics;
using CG2.Shading;
using CG2.Rendering;

namespace CG2.Lighting
{
    public class SunLight : Light
    {
        public Vector4 Direction;

        public override double GetIntensityAt(Vector4 point)
        {
            return Intensity;
        }

        public override void SetLightRayAt(Vector4 point, Ray ray)
        {
            ray.Set(point, -Direction.Normalized);
        }
      
    }
}
