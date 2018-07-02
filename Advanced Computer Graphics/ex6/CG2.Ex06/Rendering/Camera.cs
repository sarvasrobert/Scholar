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
        public Int32 Width;
        public Int32 Height;
        public Vector4[] Pixels;
        public World World;

        public Boolean UseShadows = true;

        // TODO: declare Vacuum model being model for empty space
        public Model Vacuum = new Model() { Shader = new Phong() };

        // TODO: declare MinFactor (=0.01) being the minimal color factor we trace rays. Rays with factor less than MinFactor are not traced
        public Double MinFactor = 0.01;

        // TODO: declare ReflectionCount and RefractionCount being the maximal number of reflection and refraction iterations.
        public Int32 ReflectionCount = 0;
        public Int32 RefractionCount = 0;

		#endregion


		#region Init

		public Camera(Int32 width, Int32 height)
		{
			Width = width;
			Height = height;
            Bitmap = new Bitmap(Width, Height);
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

		#endregion


		#region Rendering

		public void Render()
		{
            InitRender();
			RayTrace();
			PresentFrame();
		}

        public void InitRender()
        {
            Bitmap = new Bitmap(Width, Height);
            Pixels = new Vector4[Width * Height];
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
                    if (r == 105 && c == 213)
                    { int a = 0; }
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

            // ToDo: Calculate current reflectionFactor ( and refractionFactor) being ReflectionFactor (and RefractionFactor) of hitModels shader modulated by factor
            Double reflectionFactor = factor * ray.HitModel.Shader.ReflectionFactor;
            Double refractionFactor = factor * ray.HitModel.Shader.RefractionFactor;
            // ToDo: set n1 (and n2) to refraction index of inModels (and outModels) shader
            Double n1 = ray.HitModel.Shader.RefractionFactor;
            Double n2 = outModel.Shader.RefractionFactor;
            Boolean trueRefraction = true;

            // Refraction
            // ToDo: If RefractionLevel of ray is within RefractionCount and refractionFactor is at least MinFactor trace recursively refraction ray
            //       You need to construct the refraction ray - Vector4 - Refract function
            //       Also choose if the model is outModel or inModel
            //       After the ray is created - we can raytrace this ray and add the result to 'color'

            if (ray.RefractionLevel < RefractionCount && refractionFactor > MinFactor)
            {
                
                double ratio = 1.0;
                if (n2 != 0.0) { ratio = n1 / n2; }
                Vector4 refractedDir;
                if (Vector4.Refract(hitPoint.Normal, -ray.Direction, ratio, out refractedDir))
                {
                    Ray refractedRay = new Ray();
                    refractedRay.Set(hitPoint.Position, refractedDir, outModel, ray.ReflectionLevel, ray.RefractionLevel + 1);
                    hitPoint.Color += RayTrace(refractedRay, refractionFactor);
                }
                else
                {
                    trueRefraction = false;
                    Vector4 reflectedDir;
                    Vector4.Reflect(hitPoint.Normal, -ray.Direction, out reflectedDir);
                    Ray reflectedRay = new Ray();
                    reflectedRay.Set(hitPoint.Position, reflectedDir, ray.InModel, ray.ReflectionLevel + 1, ray.RefractionLevel + 1);
                    hitPoint.Color += RayTrace(reflectedRay, reflectionFactor);
                }          
            }

            // Reflection
            // ToDo: If ReflectionLevel of ray is within ReflectionCount and reflectionFactor is at least MinFactor trace recursively reflection ray.
            //       Import - if total internal reflection happened during refraction, explicit reflection is not needed
            //       You need to construct the reflection ray - Vector4 - Reflect function
            //       After the ray is created - we can raytrace this ray and add the result to 'color'

            if ( (trueRefraction== true) && (ray.ReflectionLevel < ReflectionCount) && (reflectionFactor > MinFactor) )
            {
                Vector4 reflectedDir;
                Vector4.Reflect(hitPoint.Normal, -ray.Direction, out reflectedDir);
                Ray reflectedRay = new Ray();
                reflectedRay.Set(hitPoint.Position, reflectedDir, ray.InModel, ray.ReflectionLevel + 1, ray.RefractionLevel);
                hitPoint.Color += RayTrace(reflectedRay, reflectionFactor);
            }
            return factor * hitPoint.Color * (1.2 - hitPoint.ShadowCount * (1 / lights));
        }

        /// <summary>
        /// Copy all the pixels from pixel buffer to the Bitmap.
        /// Color is clamped in post process.
        /// </summary>
		public void PresentFrame()
		{
			for (Int32 y = 0; y < Height; y++)
			{
				for (Int32 x = 0; x < Width; x++)
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
