using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.Scheduler
{
    public class DensityFitScheduler : CombineSchedulerBase, IScheduler
    {        
        private double criticalDensity;

        public DensityFitScheduler(Config config, ProblemSetting ps)
            : base(config, ps)
        {
            this.SchedulerType = SchedulerType.DensityFit;
     
            // init the critical density
            this.criticalDensity = (config.RevenueRate - config.NightPrice) / config.RevenueRate / 1.05;
        }

        /// <summary>
        /// decide whether it is feasible to schedule job using firstfit or bestfit
        /// note that job will not be schedulable if profit density is lower than critical profit density
        /// </summary>
        /// <param name="job"></param>
        public override void ScheduleJob(Job job)
        {
            // use first fit to schedule the job first
            this.firstFitScheduler.ScheduleJob(job);
            
            // if not schedualbe, which means best fit cannot schedule the job either.
            // just return
            if (!job.Schedulable) return;            

            // compute the first fit density
            var firstFitDensity = (job.Revenue - this.ComputeJobCost(job, job.StartTime)) / job.Revenue;

            //firstfit is feasible
            if (firstFitDensity > criticalDensity)
            {
                // use first fit

                //const double weight = 0.2;

                //// increase the critical density
                //criticalDensity = criticalDensity * (1 - weight) + firstFitDensity * weight;

                return;
            }

            // compute the best fit density
            this.bestFitScheduler.ScheduleJob(job);

            var bestFitDensity = (job.Revenue - this.ComputeJobCost(job, job.StartTime)) / job.Revenue;

            if (bestFitDensity > criticalDensity)
            {
                // use best fit

                return;
            }
            else
            {
                // density too low, drop the job
                job.Schedulable = false;

                const double weight = 0.8;

                // decrease the critical density
                criticalDensity = criticalDensity * (1 - weight) + bestFitDensity * weight;
            }
        }        
    }
}
