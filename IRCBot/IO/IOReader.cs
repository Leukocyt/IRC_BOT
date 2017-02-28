using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace IRCBot.IO
{
    public static class IOReader
    {
        public static List<string> getChannels(string path)
        {
            StreamReader sr = null;
            try {
                sr = new StreamReader(path);
                List<string> channels = new List<string>();
                string curline;
                string[] curLineSplitted = null;
                while ((curline = sr.ReadLine()) != null)
                {
                    curLineSplitted = curline.Split(',');
                    foreach (var c in curLineSplitted)
                    {
                        channels.Add(c.Trim());
                    }
                }
                sr.Close();
                return channels;
            } catch( Exception e)
            {
                try
                {
                    sr.Close();
                } catch( Exception e2) {}
                return null;
            }
        }
    }
}
