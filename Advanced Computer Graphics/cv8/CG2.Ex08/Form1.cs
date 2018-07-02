using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using CG2.Rendering;
using CG2.Mathematics;
using CG2.Modeling;
using CG2.Shading;
using CG2.Lighting;


namespace CG2.Ex08
{
    public partial class Form1 : Form
    {
        #region Properties

        World world = new World();
        CultureInfo provider = new CultureInfo("en-US");
        Camera camera;
        private Int32[] reflectionCounts = { 0, 1, 2, 3, 4, 5, 10, 50 };
        private Int32[] refractionCounts = { 0, 1, 2, 3, 4, 5, 10, 50 };

        #endregion

        public Form1()
        {
            InitializeComponent();
            cbUseShadows.Checked = true;
            cbScene.SelectedIndex = 0;
            cbReflections.SelectedIndex = 1;
            cbRefractions.SelectedIndex = 1;
            cbAntiAlias.SelectedIndex = 0;
            BlurBox.SelectedIndex = 0;
            WriteValues();
        }

        public void InitSceneRoom()
        {
            World world = new World();

            #region Shaders

            Shader black = new Phong(new Vector4(0, 0, 0));
            Shader white = new Phong(new Vector4(1, 1, 1));
            Shader red = new Phong(new Vector4(1, 0, 0));
            Shader green = new Phong(new Vector4(0, 1, 0));
            Shader blue = new Phong(new Vector4(0, 0, 1));
            Shader yellow = new Phong(new Vector4(1, 1, 0));
            Shader pink = new Phong(new Vector4(1, 0, 1));
            Shader cyan = new Phong(new Vector4(0, 1, 1));

            Shader checker = new Checker() { Shader0 = white, Shader1 = red, CubeSize = 1 };

            Shader mirror = new Phong(new Vector4(0.0, 0.0, 0.0), new Vector4(0.1, 0.1, 0.1)) { ReflectionFactor = 0.98, RefractionFactor = 0.0, RefractionIndex = 1 };
            Shader glass = new Phong(new Vector4(0.5, 0.1, 0.1), new Vector4(0.1, 0.1, 0.1)) { RefractionFactor = 0.8, RefractionIndex = 1.25 };
            #endregion

            #region Models

            world.Models.Add(new Plane(mirror, new Vector4(0, 0, 0), new Vector4(1, 0, 0)));
            world.Models.Add(new Plane(green, new Vector4(0, 0, 0), new Vector4(0, 1, 0)));
            world.Models.Add(new Plane(checker, new Vector4(0, 0, 0), new Vector4(0, 0, 1)));
            world.Models.Add(new Plane(green, new Vector4(0, 10, 0), new Vector4(0, -1, 0)));
            world.Models.Add(new Plane(blue, new Vector4(0, 0, 8), new Vector4(0, 0, -1)));

            world.Models.Add(new Sphere(glass, new Vector4(6, 7, 1.5), 1));
           
            world.Models.Add(new AABB(pink, new Vector4(5, 1, 1), new Vector4(10, 4, 2)));

            #endregion

            #region Lights

            PointLight point = new PointLight()
            {
                Origin = new Vector4(7, 5, 6),
                Intensity = 1.0,
                Range = 10.0,
                LinearAttenuation = 1,
                QuadraticAttenuation = 1
            };
            world.Lights.Add(point);

            #endregion

            camera = new Camera(500, 400)
            {
                Position = new Vector4(20, 5, 4.5),
                Target = new Vector4(0, 5, 4.5),
                World = world,
                zNear = 0.01,
                zFar = 100.0,
            };
            WriteValues();
        }

        public void InitSceneReflections()
        {
            World world = new World();

            #region Shaders

            Shader red = new Phong(new Vector4(0.5, 0, 0));
            Shader white = new Phong(new Vector4(1, 1, 1));
            Shader blue = new Phong(new Vector4(0, 0, 1));
            Shader lightBlue = new Phong(new Vector4(91 / 255.0, 142 / 255.0, 210 / 255.0));
            Shader unsaturatedGreen = new Phong(new Vector4(86 / 255.0, 116 / 255.0, 86 / 255.0));
            Shader brown = new Phong(new Vector4(80 / 255.0, 48 / 255.0, 34 / 255.0));
            Shader lightBrown = new Phong(new Vector4(195 / 255.0, 152 / 255.0, 101 / 255.0));

            Shader checker = new Checker()
            {
                Shader0 = unsaturatedGreen,
                Shader1 = white,
                CubeSize = 2
            };

            Shader mirror = new Phong(new Vector4(0.0, 0.0, 0.0), new Vector4(0.1, 0.1, 0.1)) { ReflectionFactor = 0.98, RefractionFactor = 0.0, RefractionIndex = 1 };

            #endregion

            #region Models

            world.Models.Add(new Plane(mirror, new Vector4(-20, 0, 0), new Vector4(1, 0, 0)));
            world.Models.Add(new Plane(mirror, new Vector4(20, 0, 0), new Vector4(-1, 0, 0)));
            world.Models.Add(new Plane(mirror, new Vector4(0, -20, 0), new Vector4(0, -1, 0)));
            world.Models.Add(new Plane(mirror, new Vector4(0, 20, 0), new Vector4(0, 1, 0)));
            world.Models.Add(new Plane(checker, new Vector4(0, 0, 0), new Vector4(0, 0, 1)));

            world.Models.Add(new Sphere(lightBlue, new Vector4(0, 0, 2), 1));

            #endregion

            #region Lights

            PointLight point = new PointLight()
            {
                Origin = new Vector4(0, 0, 10),
                Intensity = 0.9,
                Range = 20.0,
                LinearAttenuation = 1,
                QuadraticAttenuation = 0
            };
            world.Lights.Add(point);

            #endregion

            camera = new Camera(700, 500)
            {
                Position = new Vector4(18, 10, 7),
                Target = new Vector4(0, 1, 4),
                World = world,
                zNear = 0.01,
                zFar = 100.0,
            };

            WriteValues();
        }

        public void InitSceneRefractions()
        {
            World world = new World();

            #region Shaders

            Shader black = new Phong(new Vector4(0, 0, 0.5));
            Shader white = new Phong(new Vector4(1, 1, 1));
            Shader unsaturatedGreen = new Phong(new Vector4(86 / 255.0, 116 / 255.0, 86 / 255.0));

            Shader checker = new Checker() { Shader0 = white, Shader1 = unsaturatedGreen, CubeSize = 2 };

            Shader glass0 = new Phong(new Vector4(0.5, 0.1, 0.1), new Vector4(0.1, 0.1, 0.1)) { RefractionFactor = 0.4, RefractionIndex = 1.00 };
            Shader glass1 = new Phong(new Vector4(0.5, 0.1, 0.1), new Vector4(0.1, 0.1, 0.1)) { RefractionFactor = 0.6, RefractionIndex = 1.05 };
            Shader glass2 = new Phong(new Vector4(0.5, 0.1, 0.1), new Vector4(0.1, 0.1, 0.1)) { RefractionFactor = 0.8, RefractionIndex = 1.10 };
            Shader glass3 = new Phong(new Vector4(0.5, 0.1, 0.1), new Vector4(0.1, 0.1, 0.1)) { RefractionFactor = 0.8, RefractionIndex = 1.15 };
            Shader glass4 = new Phong(new Vector4(0.5, 0.1, 0.1), new Vector4(0.1, 0.1, 0.1)) { RefractionFactor = 0.8, RefractionIndex = 1.20 };
            Shader glass5 = new Phong(new Vector4(0.5, 0.1, 0.1), new Vector4(0.1, 0.1, 0.1)) { RefractionFactor = 0.8, RefractionIndex = 1.25 };

            Shader glass6 = new Phong(new Vector4(0.1, 0.5, 0.1), new Vector4(0.1, 0.1, 0.1)) { RefractionFactor = 0.8, RefractionIndex = 1.40 };

            #endregion

            #region Models

            world.Models.Add(new Plane(checker, new Vector4(0, 0, 0), new Vector4(0, 0, 1)));

            world.Models.Add(new Sphere(glass0, new Vector4(0, 0, 2), 1.2));
            world.Models.Add(new Sphere(glass1, new Vector4(0, 3, 2), 1.2));
            world.Models.Add(new Sphere(glass2, new Vector4(0, 6, 2), 1.2));
            world.Models.Add(new Sphere(glass3, new Vector4(0, 9, 2), 1.2));
            world.Models.Add(new Sphere(glass4, new Vector4(0, 12, 2), 1.2));
            world.Models.Add(new Sphere(glass5, new Vector4(0, 15, 2), 1.2));

            world.Models.Add(new AABB(glass6, new Vector4(3, 5, 0), new Vector4(4, 13, 8)));

            #endregion

            #region Lights

            PointLight point = new PointLight()
            {
                Origin = new Vector4(5, 5, 15),
                Intensity = 1.3,
                Range = 80.0,
                LinearAttenuation = 1,
                QuadraticAttenuation = 0
            };
            world.Lights.Add(point);

            #endregion

            camera = new Camera(700, 500)
            {
                Position = new Vector4(18, 15, 10),
                Target = new Vector4(0, 8, 3),
                zNear = 0.01,
                zFar = 100.0,
                World = world,
            };
            WriteValues();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            this.Width = camera.BitmapWidth + 240;
            this.Height = camera.BitmapHeight + 120;
            g.DrawImage(camera.Bitmap, 0, 0);
        }

        Double Parse(String text)
        {
            NumberStyles styles = NumberStyles.Integer | NumberStyles.AllowDecimalPoint;
            double res = 0;
            float rr = 0;
            double.TryParse(text, styles, provider, out res);
            float.TryParse(text, styles, provider, out rr);
            return res;
        }

        private void bRender_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            DateTime t0 = DateTime.Now;
            camera.UseShadows = cbUseShadows.Checked;
            camera.ReflectionCount = reflectionCounts[cbReflections.SelectedIndex];
            camera.RefractionCount = refractionCounts[cbRefractions.SelectedIndex];
           

            ReadValues();

            camera.Render();

            DateTime t1 = DateTime.Now;
            Cursor = Cursors.Default;
            lRenderTime.Text = "Rendering: " + (t1 - t0).TotalSeconds.ToString("F3") + " s";
            Invalidate();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Images|*.png;*.bmp;*.jpg";
            sfd.InitialDirectory = System.IO.Directory.GetCurrentDirectory() + "\\Output";
            System.Drawing.Imaging.ImageFormat format = System.Drawing.Imaging.ImageFormat.Png;
            if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                camera.Bitmap.Save(sfd.FileName, format);
        }

        #region Read & Write

        private void ReadValues()
        {
            camera.FovY = Parse(textBox1.Text);
            camera.zNear = Parse(textBox8.Text);
            camera.zFar = Parse(textBox9.Text);

            //ToDo: implement when AA is finished
            camera.BitmapWidth = (int)Parse(textBox11.Text);
            camera.BitmapHeight = (int)Parse(textBox10.Text);

            camera.Position.X = Parse(textBox2.Text);
            camera.Position.Y = Parse(textBox3.Text);
            camera.Position.Z = Parse(textBox4.Text);

            camera.Target.X = Parse(textBox5.Text);
            camera.Target.Y = Parse(textBox6.Text);
            camera.Target.Z = Parse(textBox7.Text);
        }

        private void WriteValues()
        {
            textBox1.Text = camera.FovY.ToString(provider);
            textBox8.Text = camera.zNear.ToString(provider);
            textBox9.Text = camera.zFar.ToString(provider);

            //ToDo: implement when AA is finished
            textBox11.Text = camera.BitmapWidth.ToString(provider);
            textBox10.Text = camera.BitmapHeight.ToString(provider);

            textBox2.Text = camera.Position.X.ToString(provider);
            textBox3.Text = camera.Position.Y.ToString(provider);
            textBox4.Text = camera.Position.Z.ToString(provider);

            textBox5.Text = camera.Target.X.ToString(provider);
            textBox6.Text = camera.Target.Y.ToString(provider);
            textBox7.Text = camera.Target.Z.ToString(provider);
        }

        #endregion

        private void cbScene_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((String)cbScene.SelectedItem)
            {
                case "Room": InitSceneRoom(); break;
                case "Reflections": InitSceneReflections(); break;
                case "Refractions": InitSceneRefractions(); break;
            }
        }

        private void cbAntiAlias_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((String)cbAntiAlias.SelectedItem)
            {
                case "No AntiAliasing": camera.SubSampleCount = 1; break;
                case "2x2 AntiAliasing": camera.SubSampleCount = 2; break;
                case "4x4 AntiAliasing": camera.SubSampleCount = 4; break;
                case "8x8 AntiAliasing": camera.SubSampleCount = 8; break;
            }   
        }

        private void BlurBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch ((String)BlurBox.SelectedItem)
            {
                case "No Blur": camera.UseBlur = false; break;
                case "1 Pass": camera.UseBlur = true; camera.BlurRadius = 1; break;
                case "2 Passes": camera.UseBlur = true; camera.BlurRadius = 2; break;
                case "3 Passes": camera.UseBlur = true; camera.BlurRadius = 3; break;
            }  
        }

    }
}
