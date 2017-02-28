using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IRCBot
{
    public static class helperClass
    {
        public static void writeLog(string message, int severity)
        {
            try
            {
                using (var DB = new internetEntities())
                {
                    DB.logs.Add(new logs { message = message, severity = severity, timestamp = DateTime.Now });
                    DB.SaveChanges();
                }
            }
            catch (Exception e)
            {
            }
        }
    }
}
