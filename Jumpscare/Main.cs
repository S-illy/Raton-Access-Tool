using Jumpscare;
using System.Windows.Forms;

namespace Screamer
{
    public class Main
    {
        public Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new ScreamerForm());
        }
    }
}
