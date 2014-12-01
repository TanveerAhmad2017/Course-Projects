using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.Scheduler
{
    class RandomChoiceSmallScheduler: RandomChoiceMediumScheduler, IScheduler
    {
        public RandomChoiceSmallScheduler(Config config, ProblemSetting ps)
        : base(config, ps){

            this.SchedulerType = SchedulerType.RandomChoiceSmall;
            this.semiRatio = 0.4;
        }
    }
}
