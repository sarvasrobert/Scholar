using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Mathematics;
using CG2.Rendering;

namespace CG2.Lighting
{
    public class AreaLight : Light
    {
        public List<PointLight> Lights = new List<PointLight>();
        public Vector4 Origin = new Vector4(0, 0, 0);
        public Double nx, ny;
        public Int32 sx, sy;


        //
        public void Set()
        {
            Lights.Clear();
            for (Int32 i = 0; i < nx; i++)
            {
                for (Int32 j = 0; j < ny; j++)
                {
                    Lights.Add(
                        new PointLight() 
                        {
                            Intensity = this.Intensity,
                            Origin = new Vector4((Origin.X - (sx / 2.0)) + i * sx / nx, (Origin.Y - (sy / 2.0)) + j * sy / ny, Origin.Z),
                            Range = 30,
                            LinearAttenuation = 0,
                            QuadraticAttenuation = 1
                        }
                        );
                }
            }
        }
    }
}
