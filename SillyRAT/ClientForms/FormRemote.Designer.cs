
namespace RatonRAT.ClientForms
{
    partial class FormRemote
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormRemote));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mouseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.keyboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.button3 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.checkBox1 = new CustomControls.CustomSwitch();
            this.checkBox2 = new CustomControls.CustomSwitch();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(55)))));
            this.pictureBox1.ContextMenuStrip = this.contextMenuStrip1;
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(999, 414);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseDown);
            this.pictureBox1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseMove);
            this.pictureBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox1_MouseUp);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mouseToolStripMenuItem,
            this.keyboardToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(125, 48);
            // 
            // mouseToolStripMenuItem
            // 
            this.mouseToolStripMenuItem.Image = global::RatonRAT.Properties.Resources.Mousee;
            this.mouseToolStripMenuItem.Name = "mouseToolStripMenuItem";
            this.mouseToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.mouseToolStripMenuItem.Text = "Mouse";
            this.mouseToolStripMenuItem.Click += new System.EventHandler(this.mouseToolStripMenuItem_Click);
            // 
            // keyboardToolStripMenuItem
            // 
            this.keyboardToolStripMenuItem.Image = global::RatonRAT.Properties.Resources.keyboardd;
            this.keyboardToolStripMenuItem.Name = "keyboardToolStripMenuItem";
            this.keyboardToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
            this.keyboardToolStripMenuItem.Text = "Keyboard";
            this.keyboardToolStripMenuItem.Click += new System.EventHandler(this.keyboardToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(953, 429);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "FPS: ??";
            // 
            // trackBar1
            // 
            this.trackBar1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))));
            this.trackBar1.Location = new System.Drawing.Point(470, 463);
            this.trackBar1.Maximum = 100;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(306, 45);
            this.trackBar1.TabIndex = 2;
            this.trackBar1.Value = 20;
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(84)))), ((int)(((byte)(255)))), ((int)(((byte)(189)))));
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.Color.OliveDrab;
            this.button1.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button1.ImageIndex = 0;
            this.button1.Location = new System.Drawing.Point(12, 439);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(155, 45);
            this.button1.TabIndex = 6;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(40)))), ((int)(((byte)(80)))));
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.FlatAppearance.BorderSize = 0;
            this.button2.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button2.Font = new System.Drawing.Font("Arial Rounded MT Bold", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.ForeColor = System.Drawing.SystemColors.Control;
            this.button2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.button2.ImageIndex = 0;
            this.button2.Location = new System.Drawing.Point(173, 439);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(155, 46);
            this.button2.TabIndex = 7;
            this.button2.Text = "Stop";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // timer2
            // 
            this.timer2.Interval = 1000;
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // comboBox1
            // 
            this.comboBox1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(782, 463);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(229, 26);
            this.comboBox1.TabIndex = 9;
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(27)))), ((int)(((byte)(34)))));
            this.button3.FlatAppearance.BorderSize = 0;
            this.button3.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button3.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.ForeColor = System.Drawing.SystemColors.Control;
            this.button3.Location = new System.Drawing.Point(12, 500);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(999, 51);
            this.button3.TabIndex = 14;
            this.button3.Text = "Okay, I\'ve seen too much.";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(378, 441);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 18);
            this.label2.TabIndex = 15;
            this.label2.Text = "Mouse";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.ForeColor = System.Drawing.SystemColors.Control;
            this.label3.Location = new System.Drawing.Point(378, 467);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(86, 18);
            this.label3.TabIndex = 16;
            this.label3.Text = "Keyboard";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.SystemColors.Control;
            this.label4.Location = new System.Drawing.Point(779, 444);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(122, 17);
            this.label4.TabIndex = 17;
            this.label4.Text = "Select a screen";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial Rounded MT Bold", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.SystemColors.Control;
            this.label5.Location = new System.Drawing.Point(477, 444);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 17);
            this.label5.TabIndex = 18;
            this.label5.Text = "Quality";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(334, 466);
            this.checkBox1.MinimumSize = new System.Drawing.Size(45, 22);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.OffBackColor = System.Drawing.Color.Gray;
            this.checkBox1.OffToggleColor = System.Drawing.Color.Gainsboro;
            this.checkBox1.OnBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(245)))), ((int)(((byte)(142)))));
            this.checkBox1.OnToggleColor = System.Drawing.Color.WhiteSmoke;
            this.checkBox1.Size = new System.Drawing.Size(45, 22);
            this.checkBox1.TabIndex = 13;
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(334, 439);
            this.checkBox2.MinimumSize = new System.Drawing.Size(45, 22);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.OffBackColor = System.Drawing.Color.Gray;
            this.checkBox2.OffToggleColor = System.Drawing.Color.Gainsboro;
            this.checkBox2.OnBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(105)))), ((int)(((byte)(245)))), ((int)(((byte)(142)))));
            this.checkBox2.OnToggleColor = System.Drawing.Color.WhiteSmoke;
            this.checkBox2.Size = new System.Drawing.Size(45, 22);
            this.checkBox2.TabIndex = 12;
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // FormRemote
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(1029, 563);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.Name = "FormRemote";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FormRemote";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormRemote_FormClosing);
            this.Load += new System.EventHandler(this.FormRemote_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormRemote_KeyDown);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.FormRemote_KeyUp);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.PictureBox pictureBox1;
        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.TrackBar trackBar1;
        public System.Windows.Forms.Button button1;
        public System.Windows.Forms.Button button2;
        public System.Windows.Forms.Timer timer1;
        public System.Windows.Forms.Timer timer2;
        public System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem mouseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem keyboardToolStripMenuItem;
        private CustomControls.CustomSwitch checkBox2;
        private CustomControls.CustomSwitch checkBox1;
        private System.Windows.Forms.Button button3;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label5;
    }
}