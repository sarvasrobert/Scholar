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

        // ToDo: Declare Direction of light (pointing to the light target)
        // ToDo: Declare CutoffDeg=45 as the angle in degrees of the visible light cone
        // ToDo: Declare Exp=3 as the exponent of the radial falloff
        // Hint: See > http://goo.gl/z0wtO
        public Vector4 Direction = new Vector4(-5, -5, -10);
		public Double CutoffDeg = 45;
        public Double Exp = 3;

        #endregion

        #region Lighting

        public override double GetIntensityAt(Vector4 point)
        {
            // ToDo: get the intensity of the point light (= base.GetIntensityAt(point))
            // - calculate lightVector being direction from light origin to the given point
            // - calculate lightAngle being angle between lightVector and light direction
            // - if this angle is more than Cutoff return zero intensity
            // - calculate decay being one minus ratio between lightAngle and cutoff angle powered by Exp
            // - return decay * intensity

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
