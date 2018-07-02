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

namespace CG2.Ex05
{
    public partial class Form1 : Form
    {
        #region Properties

        World world = new World();
        CultureInfo provider = new CultureInfo("en-US");
        String LightType;

        SunLight sun = new SunLight()
        {
            Intensity = 0.9,
            Direction = new Vector4(-5, 0, -10)
        };

        //AreaLight area = new AreaLight(8.0, 8.0, new Vector4(0, 0, 12));
        AreaLight area = new AreaLight()
        {
            Origin = new Vector4(0, -3, 12),
            Intensity = 0.02,
            nx = 8,
            ny = 8,
            sx = 4,
            sy = 4
        };

        PointLight point = new PointLight()
        {
            Intensity = 0.9,
            Origin = new Vector4(0, -3, 12),
            Range = 30,
            LinearAttenuation = 0,
            QuadraticAttenuation = 1
        };

        SpotLight spot = new SpotLight()
        {
            Intensity = 0.9,
            Origin = new Vector4(5, 5, 13),
            Direction = new Vector4(-5, -5, -10),
            Range = 30,
            LinearAttenuation = 0,
            QuadraticAttenuation = 1,
            CutoffDeg = 45,
            Exp = 3,
        };

        Camera camera = new Camera(900, 400)
        {
            Position = new Vector4(25, 1, 11),
            Target = new Vector4(0, 1, 4),
            zNear = 0.01,
            zFar = 100.0,
        };

        #endregion

        public Form1()
        {
            InitializeComponent();
            tabControl2.SelectedIndex = 3;
            LightType = "SunLight";
            cbUseShadows.Checked = true;
            WriteValues();
        }

        public void InitSceneLight()
        {
            World world = new World();

            Shader black = new Phong(new Vector4(0, 0, 0));
            Shader white = new Phong(new Vector4(1, 1, 1));
            Shader red = new Phong(new Vector4(1, 0, 0));
            Shader green = new Phong(new Vector4(0, 1, 0));
            Shader blue = new Phong(new Vector4(0, 0, 1));
            Shader yellow = new Phong(new Vector4(1, 1, 0));
            Shader pink = new Phong(new Vector4(1, 0, 1));
            Shader cyan = new Phong(new Vector4(0, 1, 1));
            Shader bluePhong = new Phong(new Vector4(0, 0, 1));
            //ToDo: change carPhong color to match your sample from ExLab
            Shader carPhong = new Phong(new Vector4(0.18, 0.39, 0.56));
            Shader gray = new Phong(new Vector4(0.5, 0.5, 0.5));
            Shader lightBlue = new Phong(new Vector4(91 / 255.0, 142 / 255.0, 210 / 255.0));
            Shader unsaturatedGreen = new Phong(new Vector4(86 / 255.0, 116 / 255.0, 86 / 255.0));
            Shader brown = new Phong(new Vector4(80 / 255.0, 48 / 255.0, 34 / 255.0));
            Shader lightBrown = new Phong(new Vector4(195 / 255.0, 152 / 255.0, 101 / 255.0));

            Shader checker = new Checker()
            {
                Shader0 = unsaturatedGreen,
                Shader1 = white,
                CubeSize = 3
            };

            Shader toon = new Toon(pink as Phong) { SpecularColor = new Vector4(1, 1, 1) };

            Shader cook = new CookTorrance(carPhong as Phong) { SpecularColor = new Vector4(1, 1, 1) };

            Shader oren = new OrenNayar(yellow as Phong) { SpecularColor = new Vector4(1, 1, 1) };

            Shader gradient = new GradientShader() {
                Shader0 = brown,
                Shader1 = lightBrown,
                Q = new Vector4(0, 0, 0),
                v = new Vector4(0, 1, 0)
            };

            world.Models.Add(new Plane(checker, new Vector4(0, 0, 0), new Vector4(0, 0, 1)));
            world.Models.Add(new Sphere(toon, new Vector4(0, -14, 4), 4));
            world.Models.Add(new Sphere(cook, new Vector4(0, -6, 4), 4));
            world.Models.Add(new Sphere(cyan, new Vector4(0, 2, 4), 4));
            world.Models.Add(new Sphere(oren, new Vector4(0, 10, 4), 4));
            world.Models.Add(new Sphere(gradient, new Vector4(0, 18, 4), 4));

            switch (LightType)
            {
                case "SunLight": world.Lights.Add(sun); break;
                case "PointLight": world.Lights.Add(point); break;
                case "SpotLight": world.Lights.Add(spot); break;
                case "AreaLight": area.Set(); world.Lights.Add(area); break;
            }

            camera.World = world;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            this.Width = camera.Width + 240;
            this.Height = camera.Height + 80;
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
            ReadValues();

            InitSceneLight();

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

        private void tabControl2_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (tabControl2.SelectedIndex)
            {
                case 0: LightType = "PointLight"; break;
                case 1: LightType = "SpotLight"; break;
                case 2: LightType = "AreaLight"; break;
                case 3: LightType = "SunLight"; break;
            }
        }

        #region Read & Write

        private void ReadValues()
        {
            camera.FovY = Parse(textBox1.Text);
            camera.zNear = Parse(textBox8.Text);
            camera.zFar = Parse(textBox9.Text);

            camera.Width = (int)Parse(textBox11.Text);
            camera.Height = (int)Parse(textBox10.Text);

            camera.Position.X = Parse(textBox2.Text);
            camera.Position.Y = Parse(textBox3.Text);
            camera.Position.Z = Parse(textBox4.Text);

            camera.Target.X = Parse(textBox5.Text);
            camera.Target.Y = Parse(textBox6.Text);
            camera.Target.Z = Parse(textBox7.Text);

            //sun
            sun.Direction.X = Parse(textBox23.Text);
            sun.Direction.Y = Parse(textBox24.Text);
            sun.Direction.Z = Parse(textBox25.Text);
            sun.Intensity = Parse(textBox33.Text);

            //point
            point.Origin.X = Parse(textBox14.Text);
            point.Origin.Y = Parse(textBox13.Text);
            point.Origin.Z = Parse(textBox12.Text);
            point.Range = Parse(textBox15.Text);
            point.LinearAttenuation = Parse(textBox16.Text);
            point.QuadraticAttenuation = Parse(textBox17.Text);
            point.Intensity = Parse(textBox34.Text);

            //spot
            spot.Origin.X = Parse(textBox39.Text);
            spot.Origin.Y = Parse(textBox38.Text);
            spot.Origin.Z = Parse(textBox37.Text);
            spot.Direction.X = Parse(textBox22.Text);
            spot.Direction.Y = Parse(textBox21.Text);
            spot.Direction.Z = Parse(textBox20.Text);
            spot.CutoffDeg = Parse(textBox19.Text);
            spot.Exp = Parse(textBox18.Text);
            spot.Intensity = Parse(textBox35.Text);

            //area
            area.Origin.X = Parse(textBox30.Text);
            area.Origin.Y = Parse(textBox29.Text);
            area.Origin.Z = Parse(textBox28.Text);
            area.sx = (int)Parse(textBox27.Text);
            area.sy = (int)Parse(textBox26.Text);
            area.nx = Parse(textBox32.Text);
            area.ny = Parse(textBox31.Text);
            area.Intensity = Parse(textBox36.Text);
        }

        private void WriteValues()
        {
            textBox1.Text = camera.FovY.ToString(provider);
            textBox8.Text = camera.zNear.ToString(provider);
            textBox9.Text = camera.zFar.ToString(provider);

            textBox11.Text = camera.Width.ToString(provider);
            textBox10.Text = camera.Height.ToString(provider);

            textBox2.Text = camera.Position.X.ToString(provider);
            textBox3.Text = camera.Position.Y.ToString(provider);
            textBox4.Text = camera.Position.Z.ToString(provider);

            textBox5.Text = camera.Target.X.ToString(provider);
            textBox6.Text = camera.Target.Y.ToString(provider);
            textBox7.Text = camera.Target.Z.ToString(provider);

            //sun
            textBox23.Text = sun.Direction.X.ToString(provider);
            textBox24.Text = sun.Direction.Y.ToString(provider);
            textBox25.Text = sun.Direction.Z.ToString(provider);
            textBox33.Text = sun.Intensity.ToString(provider);

            //point
            textBox14.Text = point.Origin.X.ToString(provider);
            textBox13.Text = point.Origin.Y.ToString(provider);
            textBox12.Text = point.Origin.Z.ToString(provider);
            textBox15.Text = point.Range.ToString(provider);
            textBox16.Text = point.LinearAttenuation.ToString(provider);
            textBox17.Text = point.QuadraticAttenuation.ToString(provider);
            textBox34.Text = point.Intensity.ToString(provider);

            //spot
            textBox39.Text = spot.Origin.X.ToString(provider);
            textBox38.Text = spot.Origin.Y.ToString(provider);
            textBox37.Text = spot.Origin.Z.ToString(provider);
            textBox22.Text = spot.Direction.X.ToString(provider);
            textBox21.Text = spot.Direction.Y.ToString(provider);
            textBox20.Text = spot.Direction.Z.ToString(provider);
            textBox19.Text = spot.CutoffDeg.ToString(provider);
            textBox18.Text = spot.Exp.ToString(provider);
            textBox35.Text = spot.Intensity.ToString(provider);

            //area
            textBox30.Text = area.Origin.X.ToString(provider);
            textBox29.Text = area.Origin.Y.ToString(provider);
            textBox28.Text = area.Origin.Z.ToString(provider);
            textBox27.Text = area.sx.ToString(provider);
            textBox26.Text = area.sy.ToString(provider);
            textBox32.Text = area.nx.ToString(provider);
            textBox31.Text = area.ny.ToString(provider);
            textBox36.Text = area.Intensity.ToString(provider);
        }

        #endregion

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void flowLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
