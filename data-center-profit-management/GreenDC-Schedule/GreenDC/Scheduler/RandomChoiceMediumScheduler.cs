using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.Scheduler
{
    /// <summary>
    /// SemiBestFitScheduler
    /// Algorithm: schedule the job to the earliest timeslots as long as its profits exceeds a fixed ratio of the profits of Best-Fit
    /// </summary>
    public class RandomChoiceMediumScheduler : CombineSchedulerBase, IScheduler
    {
        //the critical ratio
        protected double semiRatio;

        public RandomChoiceMediumScheduler(Config config, ProblemSetting ps): base(config, ps){
            this.SchedulerType = SchedulerType.RandomChoiceMedium;
            
            //init semiRatio
            this.semiRatio = 0.7;
            
        }

        /// <summary>
        /// schedule the job to the earlist timeslots when its profits gain exceeds a fixed ratio of the profits of Best-Fit
        /// </summary>
        /// <param name="job"></param>
        public override void ScheduleJob(Job job)
        {
            this.firstFitScheduler.ScheduleJob(job);
            var FirstFitProfit = job.SechudledProfit;
            if (!job.Schedulable) return;

            //use the BestFit approach to schedule the job;
            this.bestFitScheduler.ScheduleJob(job);
            //if even Best-Fit cannot schedule the job, then return        
            var BestFitProfit = job.SechudledProfit;

            //if (FirstFitProfit > this.semiRatio * BestFitProfit) {
            //    this.firstFitScheduler.ScheduleJob(job);
            //}


            

            //compute cirtical profits use the scheduledProfit by Best-Fit
            //use random semiRatio
            this.semiRatio = RandomGenerator.GetRandomDouble();

            var criticalProfit = FirstFitProfit + (BestFitProfit - FirstFitProfit) * (double)this.semiRatio;

            criticalProfit = Math.Min(criticalProfit, BestFitProfit);


            for (var t = this.CurrentTime; t <= job.SlackTime; t++)
            {
                var nodeList = this.Cluster.GetFitNodeList(job.RequiredNodes, t, job.RequiredTimeSlots);

                if (nodeList.Count < job.RequiredNodes) continue;

                var cost = this.ComputeJobCost(job, t);
                var tempProfit = job.Revenue - cost;


                if (tempProfit < criticalProfit) continue;

                //found
                //current profits exceeds the critical profits
                job.Schedulable = true;
                job.NodeIdList = nodeList;
                job.StartTime = t;
                //update job cost
                job.SechudledBrownEnergyCost = cost;
                break;
            }

        }
    }
}
