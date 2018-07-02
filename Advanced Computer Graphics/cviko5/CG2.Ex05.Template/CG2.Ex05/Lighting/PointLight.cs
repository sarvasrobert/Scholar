using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Mathematics;
using CG2.Rendering;

namespace CG2.Lighting
{
    public class PointLight : Light
    {
        #region Properties

        public Vector4 Origin;
        public Double Range = 1;
        public Double LinearAttenuation = 1.0;
        public Double QuadraticAttenuation = 0.0;

        #endregion

        #region Lighting

        public override Double GetIntensityAt(Vector4 point)
        {
            Double r = (Origin - point).Length;

            return Intensity *
                Range / (Range + LinearAttenuation * r) *
                Range * Range / (Range * Range + QuadraticAttenuation * r * r); 
        }

        public override void SetLightRayAt(Vector4 point, Ray ray)
        {
            ray.Set(point, (Origin - point).Normalized);
        }

        #endregion
    }
}
