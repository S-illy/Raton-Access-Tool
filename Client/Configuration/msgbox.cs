using System;
using System.Windows.Forms;

namespace Client.Configuration
{
    public static class msgbox
    {
        public static void Show(string msg, string icon, string tit)
        {
            if (icon == null) return;
            if (tit == null) return;
            if (msg == null) return;
            if(icon == "Information")
            {
                MessageBox.Show(msg.Replace(Environment.NewLine, "\\n"), tit, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            if (icon == "Error")
            {
                MessageBox.Show(msg.Replace(Environment.NewLine, "\\n"), tit, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            if (icon == "Warning")
            {
                MessageBox.Show(msg.Replace(Environment.NewLine, "\\n"), tit, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

    }
}