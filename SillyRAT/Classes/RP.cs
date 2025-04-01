using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RatonRAT.Classes
{
    public class RP
    {
        public static void ApplyRoundedBorder(Control control, int radius, Color borderColor, int borderWidth = 2)
        {
            control.Paint += (sender, e) =>
            {
                using (Pen pen = new Pen(borderColor, borderWidth))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawPath(pen, CreateRoundPath(control.ClientRectangle, radius));
                }
            };
        }

        public static async void clickButton(Control btn)
        {
            if (btn == null) return;

            var originalSize = btn.Size;

            for (int i = 0; i < 1; i++)
            {
                btn.Size = new Size(originalSize.Width - 4, originalSize.Height - 4);
                await Task.Delay(50);
                btn.Size = originalSize;
                await Task.Delay(50);
            }
        }

        private static GraphicsPath CreateRoundPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}