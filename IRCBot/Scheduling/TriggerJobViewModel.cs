using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace IRCBot.Scheduling
{
    public class TriggerJobViewModel
    {
        public IJobDetail job { get; set; }
        public ITrigger trigger { get; set; }        
        public long rowid { get; set; }
        public Nullable<System.TimeSpan> trigger_time { get; set; }
    }
}
