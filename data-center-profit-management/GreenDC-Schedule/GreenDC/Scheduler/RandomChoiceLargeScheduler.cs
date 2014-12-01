using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.Scheduler
{
    class RandomChoiceLargeScheduler: RandomChoiceMediumScheduler, IScheduler
    {
        public RandomChoiceLargeScheduler(Config config, ProblemSetting ps) : base(config, ps) {

            this.SchedulerType = SchedulerType.RandomChoiceLarge;
            this.semiRatio = 0.9;
        }
    }
}
