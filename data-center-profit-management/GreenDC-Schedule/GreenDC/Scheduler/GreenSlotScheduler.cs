using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.Scheduler
{
    public class GreenSlotScheduler: BestFitScheduler
    {
        public GreenSlotScheduler(Config config, ProblemSetting ps)
            : base(config, ps)
        {
            this.SchedulerType = SchedulerType.GreenSlot;
        }

        //protected override double ComputeJobCost(Job job, List<int> assignTimeSlotsList, out int usedGreenEnergy)
        //{
        //    var cost = base.ComputeJobCost(job, assignTimeSlotsList, out usedGreenEnergy);
           
        //    cost += 0.05 * (job.SlackTime-this.CurrentTime);

        //    return cost;
        //}


        /// <summary>
        /// greenslot add penalty of jobs that are about to miss their deadline(20%*RequiredTimeSlots before its deadline)
        /// </summary>
        /// <param name="job"></param>
        /// <param name="startTime"></param>
        /// <returns></returns>
        protected override double ComputeJobCost(Job job, int startTime)
        {
            double cost =  base.ComputeJobCost(job, startTime);

            //extend the running time by 20% to avoid missing the deadline
            var fakeSlackTime = (int)(job.SlackTime - 0.5 * job.RequiredTimeSlots);

            //add penalty
            if (startTime > fakeSlackTime)
            {
                cost += Config.GreenSlotPenaltyFact * job.RequiredNodes * (startTime - fakeSlackTime);
            }

            return cost;
        }

    }
}
