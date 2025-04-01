using System;
using System.IO;
using System.Windows.Forms;
using NAudio.Wave;

namespace Client
{
    public partial class Form4 : Form
    {
        private WaveOutEvent waveOut;
        private MemoryStream memoryStream;
        private Mp3FileReader mp3Reader;

        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            PlayMp3FromResources();
        }

        private void PlayMp3FromResources()
        {
            timer1.Start();
            byte[] soundData = Properties.Resources.video;
            memoryStream = new MemoryStream(soundData);
            mp3Reader = new Mp3FileReader(memoryStream);
            waveOut = new WaveOutEvent();
            waveOut.Init(mp3Reader);
            waveOut.Play();
        }

        private void Form4_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
