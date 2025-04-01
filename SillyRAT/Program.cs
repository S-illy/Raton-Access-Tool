using RatonRAT;
using System;
using System.IO;
using System.Windows.Forms;

namespace SillyRAT
{
    static class Program
    {
        public static string ClientsFolder = Path.Combine(Environment.CurrentDirectory, "Clients");
        public static string DataFolder = Path.Combine(Environment.CurrentDirectory, "Data");
        public static Form2 form2 { get; private set; }

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!Directory.Exists(ClientsFolder))
            {
                Directory.CreateDirectory(ClientsFolder);
            }

            if (CertificateExists())
            {
                form2 = new Form2();
                form2.Show();
                Form1 form1 = new Form1();
                Application.Run(form1);
            }
            else
            {
                FormSecurity formSecurity = new FormSecurity();
                Application.Run(formSecurity);
            }
        }

        private static bool CertificateExists()
        {
            try
            {
                string certificatePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SSLRaton.pfx");
                return File.Exists(certificatePath);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
