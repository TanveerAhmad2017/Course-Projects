using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.Scheduler
{
    public class WeightedSmallWindowScheduler : CombineSchedulerBase, IScheduler
    {
        //used to keep the original problem setting, since it might be changed during the scheduling
        //and when rescheduling used FirstFit or BestFit, we need the original setting
        public ProblemSetting originalProblemSetting;

        public ProblemSetting lastWindowSetting;
        public int lastWindowTime;

        //weight for First-Fit, similarly, weight for Best-Fit would be (1-weight)
        private double ffweight;
        //the window size of estimating the workload
        protected int WindowSize { get; set; }

        //intialize ffweight
        private double initFFWeight = 0.5;

        private const double AdjustScale = 2;

        //initialize two scheduler to schedule job 
        protected FirstFitScheduler FirstFitExpert {get; set;}
        protected BestFitScheduler BestFitExpert { get; set; }
       

        public WeightedSmallWindowScheduler(Config config, ProblemSetting ps)
            : base(config, ps)
        {
            this.SchedulerType = SchedulerType.WeightedSmallWindow;
            this.ffweight = this.initFFWeight;
            //record the orginal setting
            this.originalProblemSetting = ps.Clone();
            this.lastWindowTime = 0;
            this.WindowSize = 6;

            this.FirstFitExpert = new FirstFitScheduler(config, ps);
            this.BestFitExpert = new BestFitScheduler(config, ps);
        }

        /// <summary>
        /// get or set firstfit
        /// </summary>
        private double FirstFitWeight
        {
            get
            {
                return ffweight;
            }

            set
            {
                ffweight = Math.Max(Math.Min(1, value), 0);
            }
        }

        /// <summary>
        /// processJobList and update FF-Weight
        /// Question: make sure with Xi
        /// </summary>
        public override void ProcessJobList()
        {

            base.ProcessJobList();
            if (this.CurrentTime % WindowSize == 0)
            {
                updateWeight();
            }
        }

        /// <summary>
        /// decide whether a job is schedulable
        /// </summary>
        /// <param name="job"></param>
        public override void ScheduleJob(Job job)
        {
            //pre-scheduled using FirstFir or BestFit
            if (RandomGenerator.GetRandomDouble() < FirstFitWeight)
            {
                this.firstFitScheduler.ScheduleJob(job);
            }
            else
            {
                this.bestFitScheduler.ScheduleJob(job);
            }


        }

        /// <summary>
        /// estimated workload of last window
        /// Note: not in use right now
        /// </summary>
        /// <returns></returns>
        protected double EstimateWorkLoad()
        {
            var jobsInWindow = this.ProblemSetting.Jobs.Where(j => j.ArrivalTime >= this.CurrentTime - WindowSize && j.ArrivalTime < this.CurrentTime).ToList();

            //var jobsInWindow = this.ProblemSetting.Jobs.Where(j => j.ArrivalTime >= this.CurrentTime - WindowSize && j.Deadline <= this.CurrentTime).ToList();

            var workload = (double)jobsInWindow.Sum(j => j.RequiredTimeSlots * j.RequiredNodes) / (WindowSize * this.Cluster.NodeNum);

            return workload;
        }

        //update weight 
        public void updateWeight()
        {

            //get jobs from begining to current time
            var oldJobs = this.originalProblemSetting.Jobs.Where(j => j.ArrivalTime <= this.CurrentTime && j.ArrivalTime >= this.lastWindowTime).ToList();

            //clone the jobs to be scheduled
            var FFJobs = Job.createJob(oldJobs);
            var BFJobs = Job.createJob(oldJobs);

          

            var firstFitProfit = this.FirstFitExpert.ScheduleJobList(FFJobs, this.lastWindowTime, this.CurrentTime);
            var bestFitProfit = this.BestFitExpert.ScheduleJobList(BFJobs, this.lastWindowTime, this.CurrentTime);

            //adjust the weight of FF and BF according to the scheduled profits
            if (firstFitProfit > bestFitProfit)
            {
                //need to use first fit
                FirstFitWeight *= AdjustScale;
                //weight no more than 1
                //FirstFitWeight = Math.Min(1, FirstFitWeight);
            }
            else if (firstFitProfit < bestFitProfit)
            {
                //need to use best fit
                FirstFitWeight /= AdjustScale;
            }

            //update window
            this.lastWindowTime = this.CurrentTime;

            this.FirstFitExpert.Cluster = this.Cluster.Clone();
            this.BestFitExpert.Cluster = this.Cluster.Clone();
        }

    }
}