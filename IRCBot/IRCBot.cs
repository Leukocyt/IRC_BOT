using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Threading;


namespace IRCBot
{
    public partial class IRCBot : ServiceBase
    {
        Thread botThread = null;
        IRCListener ircBot;
        string nick = "";
        string server;
        public IRCBot()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            onServiceStart();
        }

        protected override void OnStop()
        {
            try
            {
                helperClass.writeLog("Palvelu pysäytetään...", 0);
                ircBot.disconnect();
            } catch( Exception e)
            {

            }

        }
        public void onServiceStart(bool check_channels = true)
        {
            helperClass.writeLog("Palvelu käynnistyy", 0);
            if (check_channels)
            {
                ircBot = new IRCListener(Properties.Settings.Default.nick, Properties.Settings.Default.host, Properties.Settings.Default.server);
            } else
            {
                ircBot = new IRCListener("sarppiTestaajakka", "sarppiTestaus", "irc.inet.fi");
            }
            List<string> channels = null;
            //Tarkistetaan kanavat, jos check_channels on tosi. TÄmä on epätosi testeissä.
            if (check_channels)
            {
                try
                {
                    channels = IO.IOReader.getChannels(Properties.Settings.Default.pathToChannelList);
                }
                catch (Exception e) { }
            } else
            {
                channels = new List<string>();
                channels.Add("#sarppiDev");
            }


            if( channels != null)
            {
                foreach ( var c in channels)
                {
                    ircBot.addChannel(c);
                }
            } else
            {
                ircBot.addChannel("#tesmi");
            }
            botThread = new Thread(runBot);
            botThread.IsBackground = true;
            botThread.Start();
            helperClass.writeLog("Botti käynnistetty", 0);
        }


        private void getChannelList(string filePath)
        {
            List<string> channels = new List<string>();

        }


        public void runBot()
        {
            if (ircBot == null) {
                ircBot = new IRCListener("irc.inet.fi", "hemuliSeta", "hemppuLiini");
                ircBot.addChannel("#G6");
            }
            helperClass.writeLog("Botti yhdistää", 0);
            ircBot.Connect();
            while (true)
            {
                ;
            }
        }

    }
}
