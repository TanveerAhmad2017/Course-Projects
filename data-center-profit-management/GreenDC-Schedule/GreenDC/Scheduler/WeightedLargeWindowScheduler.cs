using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.Scheduler
{
    public class WeightedLargeWindowScheduler : WeightedSmallWindowScheduler, IScheduler
    {        

        public WeightedLargeWindowScheduler(Config config, ProblemSetting ps)
            : base(config, ps)
        {
            this.SchedulerType = SchedulerType.WeightedLargeWindow;         
            this.WindowSize = 12;
        }        
    }
}