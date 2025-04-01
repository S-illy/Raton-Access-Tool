using Server.Connection;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace RatonRAT.ClientForms
{
    public partial class Notes : Form
    {
        private bool isMouseDown = false;

        private Point mouseOffset;

        private void Form_MouseDown(object sender, MouseEventArgs e)
        {
            isMouseDown = true;
            mouseOffset = new Point(e.X, e.Y);
        }

        private void Form_MouseMove(object sender, MouseEventArgs e)
        {
            if (isMouseDown)
            {
                this.Location = new Point(this.Left + e.X - mouseOffset.X, this.Top + e.Y - mouseOffset.Y);
            }
        }

        private void Form_MouseUp(object sender, MouseEventArgs e)
        {
            isMouseDown = false;
        }

        public SillyClient SillyClient { get; set; }

        public string clientID { get; set; }

        public Notes()
        {
            InitializeComponent();
            timer1.Start();
            this.MouseDown += Form_MouseDown;
            this.MouseMove += Form_MouseMove;
            this.MouseUp += Form_MouseUp;
        }

        private void Notes_Load(object sender, EventArgs e)
        {
            LoadNotes();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveNotes(textBox1.Text);
            MessageBox.Show("All saved bro :)", "Notes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            this.Close();
        }

        private void SaveNotes(string newNotes)
        {
            string folderPath = "Notes";
            string filePath = System.IO.Path.Combine(folderPath, $"{clientID}_notes.txt");

            if (!System.IO.Directory.Exists(folderPath))
            {
                System.IO.Directory.CreateDirectory(folderPath);
            }

            string existingNotes = string.Empty;

            if (System.IO.File.Exists(filePath))
            {
                existingNotes = System.IO.File.ReadAllText(filePath);
            }

            string combinedNotes = existingNotes + Environment.NewLine + newNotes;

            System.IO.File.WriteAllText(filePath, combinedNotes);
        }

        private void LoadNotes()
        {
            string folderPath = "Notes";
            string filePath = System.IO.Path.Combine(folderPath, $"{clientID}_notes.txt");

            if (System.IO.File.Exists(filePath))
            {
                MessageBox.Show("Client notes loaded", "Load", MessageBoxButtons.OK, MessageBoxIcon.Information);
                string notes = System.IO.File.ReadAllText(filePath);
                textBox1.Text = notes;
            }
            else
            {
                textBox1.Text = string.Empty;
            }
        }


        private void timer1_Tick(object sender, EventArgs e)
        {
            if (SillyClient == null || !SillyClient.isConnected())
            {
                this.Close();
            }
        }
    }
}
