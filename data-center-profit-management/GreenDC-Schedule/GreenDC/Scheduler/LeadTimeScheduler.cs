using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenSlot.Scheduler;
using GreenSlot.Model;

namespace GreenSlot.Scheduler
{
    class LeadTimeScheduler: SchedulerBase,IScheduler
    {

        public LeadTimeScheduler(Config config, ProblemSetting ps)
            : base(config, ps)
        {
            this.SchedulerType = SchedulerType.LeadTimeFit;
        }
        public override void AssignJob(Job job)
        {
            if(job.JobId == 331)
            {
                job.JobId = 331;
            }

          //  if (this.CurrentTime - job.ArrivalTime < Config.LeadTime) return false;
            for (int t = this.CurrentTime; t <= job.SlackTime; t++)
            {
                var isLeadTimeFull = t - job.ArrivalTime >= Config.LeadTime;
                var solarSum = -1;
                var tryToSechudle = isLeadTimeFull;

                if(!tryToSechudle)
                {
                    solarSum = this.GetAvailableSolarEngery(t, job.RequiredTimeSlots);
                    tryToSechudle = solarSum > 0;
                }

                if(!tryToSechudle)
                {
                    tryToSechudle = RandomGenerator.GetRandomDouble() >= ComputeScheduleProb(job);
                }

                if (!tryToSechudle) continue;

                // try sechudle a job
                
                var nodeList = this.Cluster.GetFitNodeList(job.RequiredNodes, t, job.RequiredTimeSlots);
                if (nodeList.Count < job.RequiredNodes) continue;

                job.StartTime = t;
                job.NodeIdList = nodeList;
                job.Schedulable = true;
                

                return;                           
            }

            return;
        }


        public double ComputeScheduleProb(Job job)
        {
            const double min_weight = 1.0;
            const double scale = 1.0;

            double p = 1.0 / (1.0 + Math.Pow(Math.E, -(job.Weight - min_weight) * scale));

            return p;
        }

        /// <summary>
        /// clean up expire jobs
        /// </summary>
        protected override void CleanUpJobs()
        {
            base.CleanUpJobs();

            this.CurrentJobList.RemoveAll(j => this.CurrentTime - j.ArrivalTime > Config.LeadTime);
        }

        public override bool AssignJobWithPushOut(Job job)
        {
            throw new NotImplementedException();
        }

        public override bool AssignPreemptiveJob(Job job)
        {
            throw new NotImplementedException();
        }

        public override bool AssignSemiPreemptiveJob(Job job)
        {
            throw new NotImplementedException();
        }



        public override void ScheduleJob(Job job)
        {

            if (!job.Schedulable) return;

            job.Scheduled = true;

            UpdateResouce(job);

            this.Cluster.FillBitMap(job);
        }
    }
}
