using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Mathematics;
using System.Drawing;
using CG2.Modeling;
using CG2.Shading;
using CG2.Lighting;

namespace CG2.Rendering
{
	public class Camera
	{

        public struct HitPoint
        {
            public Vector4 Position;
            public Vector4 Color;
            public Vector4 Normal;
            public Double ShadowCount;
        }

		#region Properties

		public Vector4 Position;
		public Vector4 Target;
		public Vector4 Up = new Vector4(0, 0, 1);
		public Double FovY = 45;
        public Double zFar;
        public Double zNear;
		public Vector4 U, V, W; // camera to world space
		public Vector4 BgColor = new Vector4(0, 0, 0);

        //Result
        public Bitmap Bitmap;
        public Int32 BitmapWidth;
        public Int32 BitmapHeight;
        public Vector4[] Pixels;
        public Int32 Width;
        public Int32 Height;
        public World World;

        public Boolean UseShadows = true;

        public Model Vacuum = new Model() { Shader = new Phong() };
        public Double MinFactor = 0.01;
        public Int32 ReflectionCount = 0;
        public Int32 RefractionCount = 0;

        //AA
        // SubSampleCount (=1) being the number of sub sampling rays per pixel
        public int SubSampleCount = 1;

        //Blur
        public Boolean UseBlur = false;
        public Int32 BlurRadius = 1;

		#endregion


		#region Init

        public Camera(Int32 width, Int32 height)
		{

            Width = width * SubSampleCount;
            Height = height * SubSampleCount; 
            BitmapWidth = width;
            BitmapHeight = height; 
            Bitmap = new Bitmap(BitmapWidth, BitmapHeight);
            Pixels = new Vector4[Width * Height];
		}

		#endregion


		#region Buffer Acess

		public Vector4 GetPixel(Int32 i, Int32 j)
		{
			return Pixels[i + j * Width];
		}

		public void SetPixel(Int32 i, Int32 j, Vector4 color)
		{
			Pixels[i + j * Width] = color;
		}


        public Vector4 GetPixel(Vector4[] buffer, Int32 i, Int32 j)
        {
            return buffer[i + j * Width];
        }

        public void SetPixel(Vector4[] buffer, Int32 i, Int32 j, Vector4 color)
        {
            buffer[i + j * Width] = color;
        }
        #endregion


        #region Rendering

        public void Render()
		{
            InitRender();
			RayTrace();
            // post proccesing
            AA();
            //Blur();
            Blur1();
			PresentFrame();
		}

        public void InitRender()
        {
            Width = BitmapWidth * SubSampleCount;
            Height = BitmapHeight * SubSampleCount;
            Bitmap = new Bitmap(BitmapWidth, BitmapHeight);
            Pixels = new Vector4[Width * Height];
        }

        public void BlurVertical(Vector4[] source, Vector4[] dest)
        {
           for (int y= 0; y < BitmapHeight; y++)
            {
                for (int x = 0; x < BitmapWidth; x++)
                {
                    Vector4 c = Vector4.Zero;
                    for (int p = (y-BlurRadius); p <= (y+BlurRadius) ; p++)
                    { 
                        if (p >= 0 && p < BitmapHeight)
                        {
                            c += GetPixel(source, x, p);  
                        }
                    }
                    double d = (2*BlurRadius) + 1.0;
                    c *= (float)(1.0 / d);
                    SetPixel(dest, x, y, c);
                }
            }
           
        }

        public void BlurHorizontal(Vector4[] source, Vector4[] dest)
        {
            for (int y = 0; y < BitmapHeight; y++)
            {
                for (int x = 0; x < BitmapWidth; x++)
                {
                    Vector4 c = Vector4.Zero;
                    for (int p = x - BlurRadius; p <= x + BlurRadius; p++)
                    {
                        if (p >= 0 && p < BitmapWidth)
                        {
                            c += GetPixel(source, p, y);
                        }
                    }
                    double d = (2 * BlurRadius) + 1.0;
                    c *= (float)(1.0 / d);
                    SetPixel(dest, x, y, c);
                }
            }
        }

        public void Blur1()
        {
            if (UseBlur)
            {
                Vector4[] buffer = new Vector4[Width * Height];
                BlurVertical(Pixels, buffer);
                Pixels = buffer;
                BlurHorizontal(Pixels, buffer);
                Pixels = buffer;
            }
        }

        public void Blur()
        {
            if (UseBlur)
            {
                Vector4[] buffer = new Vector4[Width * Height];

                for (int x = 0; x < BitmapWidth; x++)
                {
                    for (int y = 0; y < BitmapHeight; y++)
                    {
                        Vector4 color = Vector4.Zero;

                        for (int sx = x - BlurRadius; sx <= x + BlurRadius; sx++)
                        {
                            for (int sy = y - BlurRadius; sy <= y + BlurRadius; sy++)
                            {
                                if (sx >= 0 && sx < BitmapWidth && sy >= 0 && sy < BitmapHeight)
                                {
                                    color += GetPixel(sx, sy);
                                }

                            }

                        }
                        double d =  2*BlurRadius + 1.0;
                        color *= (float)(1.0 / (double)(d * d));
                        SetPixel(buffer, x, y, color);

                    }
                }
                Pixels = buffer;
            }
        }

        public void AA()
        {
            
            for (int x = 0; x < BitmapWidth; x++)
            {
                for (int y = 0; y < BitmapHeight; y++)
                {
                    Vector4 color = Vector4.Zero;

                    for (int sx = 0; sx < SubSampleCount; sx++)
                    {
                        for (int sy = 0; sy < SubSampleCount; sy++)
                        {
                            color += GetPixel(x * SubSampleCount + sx, y * SubSampleCount + sy);

                        }
                    }

                    color *= (float)(1.0/ (double)(SubSampleCount*SubSampleCount));
                    SetPixel(x, y, color);

                }
            }
        }
        /// <summary>Derived from Computer Graphics - David Mount. /n
        /// Implementations can differ - make your own from scratch. 
        /// See http://goo.gl/q6Sz0 and http://goo.gl/rB8J6
        /// </summary>
		public void RayTrace()
		{
            W = -1 * (Target - Position).Normalized;
            U = (Up % W).Normalized;
            V = (W % U);

            Double AspectRatio = (Double)Width / (Double)Height;
            Double h = 2 * Math.Tan(MathEx.DegToRad(FovY / 2.0));
            Double w = h * AspectRatio;

            for (int r = 0; r < Height; r++)
            {
                for (int c = 0; c < Width; c++)
                {
                    Double ur = h * ((Double)r / (Double)Height - 0.5);
                    Double uc = w * ((Double)c / (Double)Width - 0.5);
                    Vector4 rayDir = Position + (uc * U + ur * V - W);
                    rayDir = (rayDir - Position).Normalized;
                    Ray ray = new Ray(Position, rayDir, Vacuum, 0, 0, zNear, zFar);
                    Vector4 color = RayTrace(ray, 1.0);
                    SetPixel(c, Height - 1 - r, color);
                }
            }
		}

        /// <summary>
        /// Ray trace the generated ray, compute the lighting, shadows, reflections and refractions
        /// </summary>
        /// <param name="ray">Traced ray</param>
        /// <param name="factor">Parameter of color intensity after all the iterations</param>
        public Vector4 RayTrace(Ray ray, Double factor)
        {
            Vector4 color = Vector4.Zero, TempColor = Vector4.Zero;
            World.Collide(ray);
            if (ray.HitModel == null) return factor * BgColor; 
            
            // ToDo: Create hitpoint to store Position, Color, ShadowCount and Normal
            HitPoint hitPoint;
            hitPoint.Position = ray.GetHitPoint();
            hitPoint.Color = Vector4.Zero;
            hitPoint.Normal = ray.HitNormal;
            hitPoint.ShadowCount = 0;

            Ray lightRay = new Ray();
            Vector4 viewDir = -ray.Direction;
            Double lights = 0;

            //Info: Lighting: only for hard shadows
            foreach (Light light in World.Lights)
            {
                Double nl;
                lights++;
                lightRay.HitModel = null;
                light.SetLightRayAt(hitPoint.Position, lightRay);
                nl = hitPoint.Normal * lightRay.Direction;

                if (UseShadows && nl > 0.0f)
                    World.Collide(lightRay);

                if (lightRay.HitModel != null)
                {
                    hitPoint.Color += ray.HitModel.Shader.GetColor(hitPoint.Position, hitPoint.Normal, viewDir, lightRay.Direction, light.GetIntensityAt(hitPoint.Position), light, true);
                    hitPoint.Color *= lightRay.HitModel.Shader.RefractionFactor;
                    hitPoint.ShadowCount += 1;
                }
                else
                    hitPoint.Color += ray.HitModel.Shader.GetColor(hitPoint.Position, hitPoint.Normal, viewDir, lightRay.Direction, light.GetIntensityAt(hitPoint.Position), light, false);
            }

            //Info: Reflection and refraction properties
            Model outModel = ray.HitModel;

            if (ray.HitModel == ray.InModel) // exiting transparent model
            {
                hitPoint.Normal = -hitPoint.Normal;
                outModel = Vacuum; // we exit back into vacuum
            }

            Double reflectionFactor = ray.HitModel.Shader.ReflectionFactor * factor;
            Double refractionFactor = ray.HitModel.Shader.RefractionFactor * factor;
            Double n1 = ray.InModel.Shader.RefractionIndex;
            Double n2 = outModel.Shader.RefractionIndex;
            Boolean trueRefraction = true;

            //Info: Refraction
            if (ray.RefractionLevel < RefractionCount && refractionFactor > MinFactor)
            {
                Vector4 refractedVector;
                trueRefraction = Vector4.Refract(hitPoint.Normal, ray.Direction, n1 / n2, out refractedVector);
                Model refractedModel = (trueRefraction) ? outModel : ray.InModel;
                Ray refractedRay = new Ray(hitPoint.Position, refractedVector, refractedModel, ray.ReflectionLevel, ray.RefractionLevel + 1);
                hitPoint.Color += RayTrace(refractedRay, refractionFactor);
            }

            //Info: Reflection
            if (ray.ReflectionLevel < ReflectionCount && reflectionFactor > MinFactor && trueRefraction)
            {
                Vector4 reflectedVetor;
                Vector4.Reflect(hitPoint.Normal, -ray.Direction, out reflectedVetor);
                Ray reflectedRay = new Ray(hitPoint.Position, reflectedVetor, ray.InModel, ray.ReflectionLevel + 1, ray.RefractionLevel);
                hitPoint.Color += RayTrace(reflectedRay, reflectionFactor);
            }

            return factor * hitPoint.Color * (1.4 - hitPoint.ShadowCount * (1 / lights));
        }

        /// <summary>
        /// Copy all the pixels from pixel buffer to the Bitmap.
        /// Color is clamped in post process.
        /// </summary>
		public void PresentFrame()
		{
            for (Int32 y = 0; y < BitmapHeight; y++)
            {
                for (Int32 x = 0; x < BitmapWidth; x++)
                {
                    Vector4 c = GetPixel(x, y);
                    c = Vector4.Clamp(c, 0, 1);
                    Bitmap.SetPixel(x, y, Color.FromArgb(
                        (Int32)(255 * c.X),
                        (Int32)(255 * c.Y),
                        (Int32)(255 * c.Z))
                    );
                }
            }
		}

		#endregion
	}
}
