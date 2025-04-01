using Stuff;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace RatonRAT
{
    public partial class FormPaint : Form
    {
        private bool isDrawing = false;
        private Bitmap canvas;
        private Graphics graphics;
        private Point lastPoint;
        private Pen currentPen;
        public FormPaint()
        {
            InitializeComponent();
        }

        private void FormPaint_Load(object sender, EventArgs e)
        {
            canvas = new Bitmap(panel1.Width, panel1.Height);
            graphics = Graphics.FromImage(canvas);
            graphics.Clear(Color.White);
            currentPen = new Pen(Color.Black, 3);
            panel1.MouseDown += (s, ev) => { isDrawing = true; lastPoint = ev.Location; };
            panel1.MouseUp += (s, ev) => { isDrawing = false; };
            panel1.MouseMove += (s, ev) => { if (isDrawing) Draw(ev.Location); };
            panel1.Paint += (s, ev) => ev.Graphics.DrawImage(canvas, Point.Empty);
        }

        private void Draw(Point currentPoint)
        {
            graphics.DrawLine(currentPen, lastPoint, currentPoint);
            lastPoint = currentPoint;
            panel1.Invalidate();
        }
        private void saveYourArtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "PNG Image|*.png|JPEG Image|*.jpeg|Bitmap Image|*.bmp";
                saveFileDialog.Title = "Save art";
                saveFileDialog.FileName = "RatonArt_" + Helpers.Random(10);

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;
                    Watermark(graphics);
                    canvas.Save(filePath);
                    MessageBox.Show("Nice drawing vro! All saved :)", "DRAWWEDS", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void Watermark(Graphics g)
        {
            Image watermark = Properties.Resources.ratt;
            Image resize = new Bitmap(watermark, new Size(50, 50));
            PointF position = new PointF(canvas.Width - resize.Width - 10, canvas.Height - resize.Height - 10);
            g.DrawImage(resize, position);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            ColorDialog clrDialog = new ColorDialog();

            if (clrDialog.ShowDialog() == DialogResult.OK)
            {
                currentPen.Color = clrDialog.Color;
            }
        }
    }
}
