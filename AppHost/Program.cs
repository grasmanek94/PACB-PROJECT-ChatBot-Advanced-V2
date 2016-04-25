using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AppHost
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            apphost_frm frm = new apphost_frm();

            using (ServiceHost host = new ServiceHost(
                frm,
                new Uri[]{
                    new Uri("net.pipe://localhost")
                }))
            {
                host.AddServiceEndpoint(typeof(IBotOperator),
                  new NetNamedPipeBinding(),
                  "PipeAdvancedChatBotController");
                
                host.Open();
                Application.Run(frm);
                host.Close();
            }
        }
    }
}
