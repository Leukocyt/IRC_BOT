using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using ChatSharp;
using System.Windows.Forms;

namespace IRCBot
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "test")
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Form1());
            }
            else {

                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new IRCBot()
                };
                ServiceBase.Run(ServicesToRun);
            }
            
        }
    }
}
