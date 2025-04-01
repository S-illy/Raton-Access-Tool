using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.ComponentModel;
using System.Threading.Tasks;

namespace CustomControls
{
    public class CustomSwitch : CheckBox
    {
        private Color onBackColor = ColorTranslator.FromHtml("#69f58e");
        private Color onToggleColor = Color.WhiteSmoke;
        private Color offBackColor = Color.Gray;
        private Color offToggleColor = Color.Gainsboro;
        private bool solidStyle = true;
        private float togglePosition;
        private bool isChecked;

        [Category("Custom")]
        public Color OnBackColor { get => onBackColor; set { onBackColor = value; Invalidate(); } }

        [Category("Custom")]
        public Color OnToggleColor { get => onToggleColor; set { onToggleColor = value; Invalidate(); } }

        [Category("Custom")]
        public Color OffBackColor { get => offBackColor; set { offBackColor = value; Invalidate(); } }

        [Category("Custom")]
        public Color OffToggleColor { get => offToggleColor; set { offToggleColor = value; Invalidate(); } }

        [Browsable(false)]
        public override string Text { get => base.Text; set { } }

        [Category("Custom")]
        [DefaultValue(true)]
        public bool SolidStyle { get => solidStyle; set { solidStyle = value; Invalidate(); } }
        public event EventHandler CheckedChanged;

        [Category("Custom")]
        [DefaultValue(false)]
        public bool Checked
        {
            get => isChecked;
            set
            {
                if (isChecked != value)
                {
                    isChecked = value;
                    AnimateToggle();
                    Invalidate();
                    CheckedChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public CustomSwitch()
        {
            MinimumSize = new Size(45, 22);
            togglePosition = 2;
            isChecked = false;
        }

        private GraphicsPath GetFigurePath()
        {
            int arcSize = Height - 1;
            Rectangle leftArc = new Rectangle(0, 0, arcSize, arcSize);
            Rectangle rightArc = new Rectangle(Width - arcSize - 2, 0, arcSize, arcSize);
            GraphicsPath path = new GraphicsPath();
            path.StartFigure();
            path.AddArc(leftArc, 90, 180);
            path.AddArc(rightArc, 270, 180);
            path.CloseFigure();
            return path;
        }

        protected override void OnPaint(PaintEventArgs pevent)
        {
            int toggleSize = Height - 5;
            pevent.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            pevent.Graphics.Clear(Parent.BackColor);

            if (isChecked)
            {
                if (solidStyle)
                    pevent.Graphics.FillPath(new SolidBrush(onBackColor), GetFigurePath());
                else
                    pevent.Graphics.DrawPath(new Pen(onBackColor, 2), GetFigurePath());

                pevent.Graphics.FillEllipse(new SolidBrush(onToggleColor), new Rectangle((int)togglePosition, 2, toggleSize, toggleSize));
            }
            else
            {
                if (solidStyle)
                    pevent.Graphics.FillPath(new SolidBrush(offBackColor), GetFigurePath());
                else
                    pevent.Graphics.DrawPath(new Pen(offBackColor, 2), GetFigurePath());

                pevent.Graphics.FillEllipse(new SolidBrush(offToggleColor), new Rectangle((int)togglePosition, 2, toggleSize, toggleSize));
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Checked = !Checked;
        }

        private async void AnimateToggle()
        {
            int start = Checked ? 2 : Width - Height + 1;
            int end = Checked ? Width - Height + 1 : 2;
            int step = (end > start) ? 2 : -2;

            for (int i = start; (step > 0 ? i <= end : i >= end); i += step)
            {
                togglePosition = i;
                Invalidate();
                await Task.Delay(5);
            }
        }
    }
}
