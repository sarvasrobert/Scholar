using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Rendering;
using CG2.Lighting;

namespace CG2.Modeling
{
	public class World
	{
		#region Properties

		public List<Model> Models = new List<Model>();
        public List<Light> Lights = new List<Light>();

		#endregion


		#region Ray Tracing

		public void Collide(Ray ray)
		{
			foreach (Model model in Models)
			{
				model.Collide(ray);
			}
		}
		
		#endregion
	}
}
