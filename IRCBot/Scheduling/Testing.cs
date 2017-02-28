using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;
using Quartz.Impl;

namespace IRCBot.Scheduling
{
    public class Testing : IJob
    {
        void IJob.Execute(IJobExecutionContext context)
        {
            //throw new NotImplementedException();
            Console.WriteLine("Hello, JOb executed");
        }

    }
}
