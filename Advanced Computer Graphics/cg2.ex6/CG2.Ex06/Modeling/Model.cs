using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CG2.Rendering;
using CG2.Mathematics;
using CG2.Shading;

namespace CG2.Modeling
{
	public class Model
	{
		#region Properties

        public Shader Shader;

		#endregion


		#region Ray Tracing

        /// <summary>
        /// Collide ray with object and return intersection and intersected object.
        /// </summary>
		public virtual void Collide(Ray ray)
		{
		}

		#endregion
	}
}
