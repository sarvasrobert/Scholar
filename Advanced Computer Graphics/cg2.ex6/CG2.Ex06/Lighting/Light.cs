using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Mathematics;
using CG2.Rendering;

namespace CG2.Lighting
{
    public class Light
    {
        // ToDo: declare light Intensity
        public Double Intensity;

        // ToDo: declare Diffuse
        public Vector4 DiffuseColor = new Vector4(1, 1, 1);

        public virtual Double GetIntensityAt(Vector4 point)
        {
            return Intensity;
        }

        public virtual void SetLightRayAt(Vector4 point, Ray ray)
        {
        }

    }
}
