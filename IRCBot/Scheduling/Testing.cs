﻿using System;
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
        Task IJob.Execute(IJobExecutionContext context)
        {
            //throw new NotImplementedException();
            Console.WriteLine("Hello, JOb executed");
            Console.WriteLine("Moro sano poro, kun se kirnuun kusi.");
            return null;
        }

    }
}
