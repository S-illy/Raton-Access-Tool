using System;
using System.Drawing;
using System.Windows.Forms;

namespace RatonRAT
{
    public partial class Form7 : Form
    {
        private Control[] floatingControls;
        private Timer timer;
        private float angle;
        private float[] phaseOffsets;
        private Random random;

        public Form7()
        {
            InitializeComponent();

            floatingControls = new Control[] {button1, pictureBox1, pictureBox2, pictureBox3, pictureBox4, label2, label3, label4, label5, label6, label7 };
            phaseOffsets = new float[floatingControls.Length];
            random = new Random();

            for (int i = 0; i < floatingControls.Length; i++)
                phaseOffsets[i] = (float)(random.NextDouble() * Math.PI * 2);

            timer = new Timer { Interval = 40 };
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            angle += 0.012f;
            for (int i = 0; i < floatingControls.Length; i++)
            {
                if (floatingControls[i] != null)
                {
                    int yOffset = (int)(Math.Sin(angle + phaseOffsets[i]) * 4);
                    int xOffset = (int)(Math.Cos(angle + phaseOffsets[i]) * 2);
                    floatingControls[i].Location = new Point(floatingControls[i].Location.X + xOffset, floatingControls[i].Location.Y + yOffset);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            timer.Stop();
            timer.Dispose();
            base.OnFormClosing(e); //// veryyyycollll kode!1
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Server.Classes.SFX.click();
            this.Close();
        }

        private void Form7_Load(object sender, EventArgs e)
        {

        }

        private void pictureBox5_Click(object sender, EventArgs e)
        {
                    }
    }
}
