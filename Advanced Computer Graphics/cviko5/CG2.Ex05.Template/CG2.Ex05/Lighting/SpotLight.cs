using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Mathematics;
using CG2.Rendering;

namespace CG2.Lighting
{
    public class SpotLight : PointLight
    {
        #region Properties

        public Vector4 Direction = new Vector4(-5, -5, -10);
		public Double CutoffDeg = 45;
        public Double Exp = 3;

        #endregion

        #region Lighting

        public override double GetIntensityAt(Vector4 point)
        {
            Double Intensity = base.GetIntensityAt(point);

            Vector4 LightVector = ( point- Origin).Normalized;
            Double devCos = LightVector * Direction.Normalized;
            Double devAngleDeg = MathEx.RadToDeg(Math.Acos(devCos));
            if (devAngleDeg > CutoffDeg)
            {
                return 0;
            }
            else
            {
                Double ratio = devAngleDeg / CutoffDeg;
                Double decay = 1 - Math.Pow(ratio, Exp);
                return  decay * Intensity;
            }
        }

        public override void SetLightRayAt(Vector4 point, Ray ray)
        {
            base.SetLightRayAt(point, ray);
        }

        #endregion
    }

}
