using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.Scheduler
{
    public abstract class CombineSchedulerBase : SchedulerBase, IScheduler
    {
        protected FirstFitScheduler firstFitScheduler;
        protected BestFitScheduler bestFitScheduler;

        public CombineSchedulerBase(Config config, ProblemSetting ps)
            : base(config, ps)
        {
            this.SchedulerType = SchedulerType.Abstract;

            this.firstFitScheduler = new FirstFitScheduler(config, ps);
            this.bestFitScheduler = new BestFitScheduler(config, ps);

            //share resource
            this.firstFitScheduler.Cluster = this.bestFitScheduler.Cluster = this.Cluster;
            this.firstFitScheduler.ProblemSetting = this.bestFitScheduler.ProblemSetting = this.ProblemSetting;            
        }

        /// <summary>
        /// let firstfit and bestfit share "CurrentJobList" and "CurrentTime"
        /// </summary>
        public override void ProcessJobList()
        {
            base.ProcessJobList();
            this.firstFitScheduler.CurrentJobList = this.bestFitScheduler.CurrentJobList = this.CurrentJobList;
            this.firstFitScheduler.CurrentTime = this.bestFitScheduler.CurrentTime = this.CurrentTime;
        }


        /// <summary>
        /// assing job if job is schedulable
        /// </summary>
        /// <param name="job"></param>
        public override void AssignJob(Job job)
        {
            if (!job.Schedulable) return;

            job.Scheduled = true;

            this.UpdateResouce(job);

            this.Cluster.FillBitMap(job);
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
            throw new NotImplementedException();
        }
    }
}
