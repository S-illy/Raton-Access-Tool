using System;
using System.IO;
using System.Windows.Forms;
using NAudio.Wave;

namespace Jumpscare
{
    public partial class ScreamerForm : Form
    {
        private IWavePlayer wavePlayer;
        private AudioFileReader audioFileReader;

        public ScreamerForm()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
        }

        private void Screamer_Load(object sender, EventArgs e)
        {
            timer1.Start();
            this.TopMost = true;
            this.Activate();
            this.BringToFront();
            byte[] audioBytes = Properties.Resources.video;

            string tempFilePath = Path.Combine(Path.GetTempPath(), "screamer.mp3");
            File.WriteAllBytes(tempFilePath, audioBytes);

            wavePlayer = new WaveOutEvent();
            audioFileReader = new AudioFileReader(tempFilePath);
            wavePlayer.Init(audioFileReader);
            wavePlayer.Play();
            Resiz();
        }

        private void Resiz()
        {
            pictureBox1.Size = this.ClientSize;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
