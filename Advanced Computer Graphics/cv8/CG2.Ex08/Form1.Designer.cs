namespace CG2.Ex08
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.cbScene = new System.Windows.Forms.ComboBox();
            this.cbReflections = new System.Windows.Forms.ComboBox();
            this.cbRefractions = new System.Windows.Forms.ComboBox();
            this.cbAntiAlias = new System.Windows.Forms.ComboBox();
            this.cbUseShadows = new System.Windows.Forms.CheckBox();
            this.bRender = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.lRenderTime = new System.Windows.Forms.Label();
            this.BlurBox = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.tabControl1);
            this.flowLayoutPanel1.Controls.Add(this.cbScene);
            this.flowLayoutPanel1.Controls.Add(this.cbUseShadows);
            this.flowLayoutPanel1.Controls.Add(this.bRender);
            this.flowLayoutPanel1.Controls.Add(this.button1);
            this.flowLayoutPanel1.Controls.Add(this.lRenderTime);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(527, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(225, 522);
            this.flowLayoutPanel1.TabIndex = 9;
            // 
            // tabControl1
            // 
            this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.tabControl1.Location = new System.Drawing.Point(3, 3);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(222, 260);
            this.tabControl1.TabIndex = 9;
            // 
            // tabPage1
            // 
            this.tabPage1.BackColor = System.Drawing.Color.Transparent;
            this.tabPage1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tabPage1.Controls.Add(this.BlurBox);
            this.tabPage1.Controls.Add(this.textBox10);
            this.tabPage1.Controls.Add(this.textBox11);
            this.tabPage1.Controls.Add(this.cbReflections);
            this.tabPage1.Controls.Add(this.cbRefractions);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.cbAntiAlias);
            this.tabPage1.Controls.Add(this.textBox9);
            this.tabPage1.Controls.Add(this.textBox8);
            this.tabPage1.Controls.Add(this.textBox7);
            this.tabPage1.Controls.Add(this.textBox6);
            this.tabPage1.Controls.Add(this.textBox5);
            this.tabPage1.Controls.Add(this.textBox4);
            this.tabPage1.Controls.Add(this.textBox3);
            this.tabPage1.Controls.Add(this.textBox2);
            this.tabPage1.Controls.Add(this.label3);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Controls.Add(this.textBox1);
            this.tabPage1.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.tabPage1.Location = new System.Drawing.Point(4, 25);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(0);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Size = new System.Drawing.Size(214, 231);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Camera";
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(120, 87);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(36, 20);
            this.textBox10.TabIndex = 14;
            // 
            // textBox11
            // 
            this.textBox11.Location = new System.Drawing.Point(78, 87);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(36, 20);
            this.textBox11.TabIndex = 13;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(2, 90);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(57, 13);
            this.label4.TabIndex = 12;
            this.label4.Text = "Resolution";
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(162, 7);
            this.textBox9.Name = "textBox9";
            this.textBox9.Size = new System.Drawing.Size(36, 20);
            this.textBox9.TabIndex = 11;
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(120, 7);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(36, 20);
            this.textBox8.TabIndex = 10;
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(162, 61);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(36, 20);
            this.textBox7.TabIndex = 9;
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(120, 61);
            this.textBox6.Name = "textBox6";
            this.textBox6.Size = new System.Drawing.Size(36, 20);
            this.textBox6.TabIndex = 8;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(78, 61);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(36, 20);
            this.textBox5.TabIndex = 7;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(162, 33);
            this.textBox4.Name = "textBox4";
            this.textBox4.Size = new System.Drawing.Size(36, 20);
            this.textBox4.TabIndex = 6;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(120, 33);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(36, 20);
            this.textBox3.TabIndex = 5;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(78, 33);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(36, 20);
            this.textBox2.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(2, 64);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(38, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Target";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(2, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(44, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Position";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(71, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Fov / zN / zF";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(78, 7);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(36, 20);
            this.textBox1.TabIndex = 0;
            // 
            // cbScene
            // 
            this.cbScene.FormattingEnabled = true;
            this.cbScene.Items.AddRange(new object[] {
            "Room",
            "Reflections",
            "Refractions"});
            this.cbScene.Location = new System.Drawing.Point(3, 269);
            this.cbScene.Name = "cbScene";
            this.cbScene.Size = new System.Drawing.Size(121, 21);
            this.cbScene.TabIndex = 16;
            this.cbScene.SelectedIndexChanged += new System.EventHandler(this.cbScene_SelectedIndexChanged);
            // 
            // cbReflections
            // 
            this.cbReflections.FormattingEnabled = true;
            this.cbReflections.Items.AddRange(new object[] {
            "No Reflection",
            "1x Reflection",
            "2x Reflections",
            "3x Reflections",
            "4x Reflections",
            "5x Reflections",
            "10x Reflections",
            "50x Reflections"});
            this.cbReflections.Location = new System.Drawing.Point(5, 113);
            this.cbReflections.Name = "cbReflections";
            this.cbReflections.Size = new System.Drawing.Size(121, 21);
            this.cbReflections.TabIndex = 17;
            // 
            // cbRefractions
            // 
            this.cbRefractions.FormattingEnabled = true;
            this.cbRefractions.Items.AddRange(new object[] {
            "No Refraction",
            "1x Refraction",
            "2x Refractions",
            "3x Refractions",
            "4x Refractions",
            "5x Refractions",
            "10x Refractions",
            "50x Refractions"});
            this.cbRefractions.Location = new System.Drawing.Point(5, 140);
            this.cbRefractions.Name = "cbRefractions";
            this.cbRefractions.Size = new System.Drawing.Size(121, 21);
            this.cbRefractions.TabIndex = 18;
            // 
            // cbAntiAlias
            // 
            this.cbAntiAlias.FormattingEnabled = true;
            this.cbAntiAlias.Items.AddRange(new object[] {
            "No AntiAliasing",
            "2x2 AntiAliasing",
            "4x4 AntiAliasing",
            "8x8 AntiAliasing"});
            this.cbAntiAlias.Location = new System.Drawing.Point(5, 167);
            this.cbAntiAlias.Name = "cbAntiAlias";
            this.cbAntiAlias.Size = new System.Drawing.Size(121, 21);
            this.cbAntiAlias.TabIndex = 19;
            this.cbAntiAlias.SelectedIndexChanged += new System.EventHandler(this.cbAntiAlias_SelectedIndexChanged);
            // 
            // cbUseShadows
            // 
            this.cbUseShadows.AutoSize = true;
            this.cbUseShadows.Location = new System.Drawing.Point(3, 296);
            this.cbUseShadows.Name = "cbUseShadows";
            this.cbUseShadows.Size = new System.Drawing.Size(90, 17);
            this.cbUseShadows.TabIndex = 11;
            this.cbUseShadows.Text = "Use shadows";
            this.cbUseShadows.UseVisualStyleBackColor = true;
            // 
            // bRender
            // 
            this.bRender.Location = new System.Drawing.Point(3, 319);
            this.bRender.Name = "bRender";
            this.bRender.Size = new System.Drawing.Size(105, 23);
            this.bRender.TabIndex = 0;
            this.bRender.Text = "Render";
            this.bRender.UseVisualStyleBackColor = true;
            this.bRender.Click += new System.EventHandler(this.bRender_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(3, 348);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(105, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Save";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // lRenderTime
            // 
            this.lRenderTime.AutoSize = true;
            this.lRenderTime.Location = new System.Drawing.Point(3, 374);
            this.lRenderTime.Name = "lRenderTime";
            this.lRenderTime.Size = new System.Drawing.Size(59, 13);
            this.lRenderTime.TabIndex = 7;
            this.lRenderTime.Text = "Rendering:";
            // 
            // BlurBox
            // 
            this.BlurBox.FormattingEnabled = true;
            this.BlurBox.Items.AddRange(new object[] {
            "No Blur",
            "1 Pass",
            "2 Passes",
            "3 Passes"});
            this.BlurBox.Location = new System.Drawing.Point(6, 194);
            this.BlurBox.Name = "BlurBox";
            this.BlurBox.Size = new System.Drawing.Size(121, 21);
            this.BlurBox.TabIndex = 21;
            this.BlurBox.SelectedIndexChanged += new System.EventHandler(this.BlurBox_SelectedIndexChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 522);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Name = "Form1";
            this.Text = "Ex08 SSAA / FSAA";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.TextBox textBox11;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.ComboBox cbScene;
        private System.Windows.Forms.ComboBox cbReflections;
        private System.Windows.Forms.ComboBox cbRefractions;
        private System.Windows.Forms.CheckBox cbUseShadows;
        private System.Windows.Forms.Button bRender;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lRenderTime;
        private System.Windows.Forms.ComboBox cbAntiAlias;
        private System.Windows.Forms.ComboBox BlurBox;
    }
}

