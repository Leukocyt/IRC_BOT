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

namespace IRCBot
{
    public partial class Form1 : Form
    {
        Thread botThread = null;
        IRCListener ircBot;
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            IRCBot b = new IRCBot();
            b.onServiceStart(false);                     
        }




        public void runBot()
        {
            ircBot = new IRCListener("hemuli", "hemppe", "irc.inet.fi");
            ircBot.addChannel("#G6");
            ircBot.Connect();
            while (true)
            {
                ;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ircBot.disconnect();
            Thread.Sleep(10000);
            botThread.Abort();
            Thread.Sleep(500);
            System.Console.WriteLine(" Botin yhteys on katkaistu.");
        }

        private void button3_Click(object sender, EventArgs e)
        {
            IRCBot i = new IRCBot();
            i.onServiceStart();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            IRCBot b = new IRCBot();
            b.onServiceStart();
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            // construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            // get a scheduler, start the schedular before triggers or anything else
            IScheduler sched = schedFact.GetScheduler();
            sched.Start();
            // create job          
            IJobDetail job = JobBuilder.Create<Testing>()
                    .WithIdentity("job1", "group1")
                    .Build();

            // create trigger
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(45).RepeatForever())
                .Build();

            ITrigger t = TriggerBuilder.Create()
                .WithIdentity("myTrigger")
                //.ForJob("myJob")
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(21, 50)) // execute job daily at 9:30
                //.ModifiedByCalendar("myHolidays") // but not on holidays
                .Build();

            // Schedule the job using the job and trigger 
            sched.ScheduleJob(job, t);
            sched.ScheduleJob(job, trigger);
        }
    }
}
