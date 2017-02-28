using System;
using IRCBot.Scheduling;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using Quartz;
using Quartz.Impl;

namespace IRCBot.Scheduling
{
    


    public class IRCMessageScheduler
    {
        IRCListener irc_bot;
        ISchedulerFactory schedFact;
        IScheduler sched;
        List<TriggerJobViewModel> trigger_job_list = null;
        public IRCMessageScheduler(ref IRCListener bot)
        {
            //Assing Irc bot.
            irc_bot = bot;
            // construct a scheduler factory
            schedFact = new StdSchedulerFactory();
            // get a scheduler, start the schedular before triggers or anything else
            sched = schedFact.GetScheduler();
            sched.Start();
            trigger_job_list = new List<TriggerJobViewModel>();
            //IRC BOTTI tänne.
            sched.Context.Put("ircbot", irc_bot);
            //Luodaan jobit antaen identiteetiksi skedulointirivin rowid. Tämän perusteella homma jo toimii.
        }
    }

    public class JobExecutor : IJob
    {
        void IJob.Execute(IJobExecutionContext context)
        {
            //throw new NotImplementedException();
            Console.WriteLine("Hello, JOb executed");
        }
        private void sendMessagesForTrigger(Int64 timing_ID, ref IRCListener irc_bot)
        {
            using (var db = new internetEntities())
            {
                foreach (var m in db.timed_messages)
                {
                    try
                    {
                        irc_bot.sendMessage(m.channel, m.message);
                    }
                    catch (Exception e)
                    {
                        helperClass.writeLog(e.ToString(), 3);
                    }
                }
            }
        }
    }
}
