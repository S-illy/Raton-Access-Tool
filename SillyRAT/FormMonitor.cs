using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace RatonRAT
{
    public partial class FormMonitor : Form
    {
        private Timer updateTimer;
        private PerformanceCounter cpuCounter;
        private PerformanceCounter ramCounter;
        private Process currentProcess;

        public FormMonitor()
        {
            InitializeComponent();
            currentProcess = Process.GetCurrentProcess();

            cpuCounter = new PerformanceCounter("Process", "% Processor Time", currentProcess.ProcessName, true);
            ramCounter = new PerformanceCounter("Memory", "Available Bytes");

            ConfigureChart();

            updateTimer = new Timer { Interval = 500 };
            updateTimer.Tick += UpdateStats;
            updateTimer.Start();
        }

        private void ConfigureChart()
        {
            chart1.ChartAreas.Clear();
            chart1.Series.Clear();
            chart1.BackColor = Color.White;

            ChartArea chartArea = new ChartArea
            {
                BackColor = Color.Transparent,
                AxisX = { Title = " ", LabelStyle = { ForeColor = Color.Black } },
                AxisY = { Title = " ", LabelStyle = { ForeColor = Color.Black } }
            };

            chart1.ChartAreas.Add(chartArea);

            Series cpuSeries = new Series("CPU Usage")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Red,
                BorderWidth = 2
            };

            Series ramSeries = new Series("Memory Usage")
            {
                ChartType = SeriesChartType.Line,
                Color = Color.Blue,
                BorderWidth = 2
            };

            chart1.Series.Add(cpuSeries);
            chart1.Series.Add(ramSeries);
        }

        private void UpdateStats(object sender, EventArgs e)
        {
            try
            {
                float cpuUsage = cpuCounter.NextValue() / Environment.ProcessorCount;

                float totalMemory = new Microsoft.VisualBasic.Devices.ComputerInfo().TotalPhysicalMemory;

                float availableMemory = ramCounter.NextValue();

                float usedMemory = totalMemory - availableMemory;

                float memoryUsagePercentage = (usedMemory / totalMemory) * 100;

                chart1.ChartAreas[0].AxisY.Maximum = 100;
                chart1.ChartAreas[0].AxisY.Minimum = 0;

                chart1.Series["CPU Usage"].Points.AddY(cpuUsage);
                chart1.Series["Memory Usage"].Points.AddY(memoryUsagePercentage);

                if (chart1.Series["CPU Usage"].Points.Count > 50)
                {
                    chart1.Series["CPU Usage"].Points.RemoveAt(0);
                    chart1.Series["Memory Usage"].Points.RemoveAt(0);
                }

                listView1.BeginUpdate();
                listView1.Items[0].SubItems[1].Text = $"{cpuUsage:F2} %";
                listView1.Items[1].SubItems[1].Text = $"{memoryUsagePercentage:F2} %";
                listView1.EndUpdate();
            }
            catch (Exception ex) {
                Console.Write(ex.Message);
            }
        }


        private void FormMonitor_Load(object sender, EventArgs e)
        {
            new Classes.LightMode().Apply(this);
            this.Region = new Region(CreateRoundPath(this.ClientRectangle, 25));
            listView1.Region = new Region(CreateRoundPath(listView1.ClientRectangle, 10));
            chart1.Region = new Region(CreateRoundPath(chart1.ClientRectangle, 10));
            button4.Region = new Region(CreateRoundPath(button4.ClientRectangle, 10));
        }
        private static GraphicsPath CreateRoundPath(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            path.AddArc(rect.X, rect.Y, radius, radius, 180, 90);
            path.AddArc(rect.Width - radius - 1, rect.Y, radius, radius, 270, 90);
            path.AddArc(rect.Width - radius - 1, rect.Height - radius - 1, radius, radius, 0, 90);
            path.AddArc(rect.X, rect.Height - radius - 1, radius, radius, 90, 90);
            path.CloseFigure();
            return path;
        }

        private void FormMonitor_Paint(object sender, PaintEventArgs e)
        {
            if (!this.DesignMode)
            {
                using (Pen pen = new Pen(Color.White, 3))
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.DrawPath(pen, CreateRoundPath(this.ClientRectangle, 25));
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
