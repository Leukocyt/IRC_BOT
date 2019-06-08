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
using IRCBot.DB;
using Quartz;
using Quartz.Impl;

namespace IRCBot.Scheduling
{
    


    public class IRCMessageScheduler
    {
        IRCListener irc_bot;
        StdSchedulerFactory schedFact;
        IScheduler sched;
        List<TriggerJobViewModel> trigger_job_list = null;
        //Testit
        ITrigger trigger = null;
        IJobDetail job = null;

        public IRCMessageScheduler(IRCListener bot)
        {
            //Assing Irc bot.
            irc_bot = bot;
            // construct a scheduler factory
            schedFact = new StdSchedulerFactory();
            // get a scheduler, start the schedular before triggers or anything else
            sched = schedFact.GetScheduler().Result;
            sched.Start();
            trigger_job_list = new List<TriggerJobViewModel>();
            //IRC BOTTI tänne.
            sched.Context.Put("ircbot", irc_bot);
            sched.Context.Put("me", this);
            //Luodaan jobit antaen identiteetiksi skedulointirivin rowid. Tämän perusteella homma jo toimii.
            
            //Testitriggeri
            trigger = TriggerBuilder.Create()
                .WithIdentity("trigger1", "group1")
                .WithSimpleSchedule(x => x.WithIntervalInMinutes(1).RepeatForever())
                .Build();
            //Testijobi
            job = JobBuilder.Create<MaintExecutor>()
                    .WithIdentity("job1", "group1")
                    .Build();
            sched.ScheduleJob(job, trigger);
            //fillTriggersAndJobs();
        }

        #region Julkiset metodit
        public void start_scheduling()
        {
            /*
            if (sched != null)
            {
                sched.ScheduleJob(job, trigger);
            }
            */
            /*
            foreach(var s in trigger_job_list)
            {
                try {
                    sched.ScheduleJob(s.job, s.trigger);
                } catch(Exception e) { }
            }
            */
            //Maintenance trigger
        }
        #endregion


        #region yksityiset metodit

        //Ajetaan alussa.
        private void fillTriggersAndJobs()
        {
            try {
                using (var db = new internetEntities())
                {
                    foreach (var s in db.timing_table)
                    {
                        try
                        {
                            //Jobi kullekin.
                            IJobDetail job = createJob(s);
                            //Samaten triggeri.
                            ITrigger t = createTrigger(s);
                            //Lisätään triggeri ja jobi listalle.
                            trigger_job_list.Add(new TriggerJobViewModel { job = job, trigger = t, rowid = s.rowid, trigger_time = s.trigger_time });
                        } catch (Exception e) { }
                    }
                }
            } catch(Exception e) { }
        }

        private IJobDetail createJob(timing_table s)
        {
            try {
                IJobDetail job = JobBuilder.Create<JobExecutor>()
                .WithIdentity(s.rowid.ToString(), "group1")
                .UsingJobData("rowid", s.rowid)
                .Build();
                return job;
            } catch(Exception e)
            {
                return null;
            }
        }

        private ITrigger createTrigger(timing_table s)
        {
            //“0 24 23 * * ? *”
            string cronString = "20 " + s.trigger_time.Value.Minutes.ToString() + " " + s.trigger_time.Value.Hours.ToString() + " * * ? *";
            try
            {
                ITrigger t = TriggerBuilder.Create()
                    .WithIdentity(s.rowid.ToString())
                    //.ForJob("myJob")
                    //.WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(s.trigger_time.Value.Hours, s.trigger_time.Value.Minutes))
                    .WithCronSchedule(cronString)
                    .Build();
                return t;
            } catch(Exception e)
            {
                return null;
            }
        }

        private TriggerJobViewModel createJobAndTrigger(timing_table s)
        {
            try
            {
                IJobDetail j = createJob(s);
                ITrigger t = createTrigger(s);
                TriggerJobViewModel m = new TriggerJobViewModel { job = j, trigger = t, rowid = s.rowid, trigger_time = s.trigger_time };
                return m;
            }
            catch (Exception e)
            {
                return null;
            }
        }
        public void updateJobs()
        {
            using( var db = new internetEntities())
            {
                //Kannan vertaus tallessa olevaan paskaan.
                foreach(var s in db.timing_table)
                {
                    try {
                        //Löytyykö jo listalta?
                        if (trigger_job_list.Where(x => x.rowid == s.rowid).Any())
                        {
                            //Tarkistetaan päivityksen tarve ja jos on tarpeen, päivitetään.
                            TriggerJobViewModel j = trigger_job_list.Where(x => x.rowid == s.rowid).First();
                            if (j.trigger_time != s.trigger_time)
                            {
                                sched.DeleteJob(j.job.Key);
                                trigger_job_list.Remove(j);
                                TriggerJobViewModel m = createJobAndTrigger(s);
                                trigger_job_list.Add(m);
                                sched.ScheduleJob(m.job, m.trigger);
                            }
                        } else
                        {
                            //Lisätään tämä
                            TriggerJobViewModel m = createJobAndTrigger(s);
                            trigger_job_list.Add(m);
                            sched.ScheduleJob(m.job, m.trigger);
                        }
                    } catch(Exception e)
                    {
                        helperClass.writeLog(e.ToString(), 4);
                    }
                }
                //Tallessa olevan paskan vertaus kantaan.
                List<TriggerJobViewModel> tmpList = trigger_job_list.ToList();
                foreach ( var s in tmpList)
                {
                    try
                    {
                        if ( !db.timing_table.Where(x => x.rowid == s.rowid).Any()) //Jos ei löydy, niin helvettiin koko paska.
                        {
                            sched.DeleteJob(s.job.Key);
                            trigger_job_list.Remove(s);
                        }
                    } catch(Exception e)
                    {
                        helperClass.writeLog(e.ToString(), 4);
                    }
                }
            }
                   
        }


        #endregion
    }

    #region ajosäikeet
    public class JobExecutor : IJob
    { 
        Task IJob.Execute(IJobExecutionContext context)
        {
            try
            {

                Console.WriteLine("Start Executing");
                long row_id = context.JobDetail.JobDataMap.GetLong("rowid");                
                //throw new NotImplementedException();
                var schedulerContext = context.Scheduler.Context;
                //Botti haetaan tässä.
                IRCListener bot = (IRCListener)schedulerContext.Get("ircbot");
                //Haetaan aikataulun yksilöivä row_ID
                sendMessagesForTrigger(row_id, ref bot);
                //bot.sendMessage("#sarppiDev", "moi!");
                Console.WriteLine("Hello, JOb executed");
                return null;
                 
            } catch(Exception e)
            {
                return null;
            }
        }

        private void sendMessagesForTrigger(Int64 timing_ID, ref IRCListener irc_bot)
        {
            using (var db = new internetEntities())
            {
                foreach (var m in db.timed_messages.Where(x => x.timing_ID == timing_ID))
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

    public class MaintExecutor : IJob
    {
        Task IJob.Execute(IJobExecutionContext context)
        {
            try
            {
                var schedulerContext = context.Scheduler.Context;
                IRCMessageScheduler m = (IRCMessageScheduler)schedulerContext.Get("me");
                m.updateJobs();
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

    }
    #endregion
}
