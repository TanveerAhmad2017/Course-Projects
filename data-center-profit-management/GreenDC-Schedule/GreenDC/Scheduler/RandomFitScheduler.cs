using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenSlot.Util;

namespace GreenSlot.Scheduler
{
    public class RandomFitScheduler : CombineSchedulerBase, IScheduler
    {
        private double dayToNightRandomProbability;
        private double nightToDayRandomProbability;

        public RandomFitScheduler(Config config, ProblemSetting ps)
            : base(config, ps)
        {
            this.SchedulerType = SchedulerType.RandomFit;

            //update probability
            var v1v2 = (this.Config.RevenueRate - this.Config.DayPrice) / (this.Config.RevenueRate - this.Config.NightPrice);

            this.dayToNightRandomProbability = (v1v2) / (1 + v1v2 - Math.Pow(v1v2, 2));
            Console.WriteLine("DayToNightRandomProbability = {0}", this.dayToNightRandomProbability);
            
            //update probability
            var v2v3 = (this.Config.RevenueRate - this.Config.NightPrice) / this.Config.RevenueRate;

            this.nightToDayRandomProbability = (v2v3) / (1 + v2v3 - Math.Pow(v2v3, 2));
            Console.WriteLine("NightToDayRandomProbability = {0}", this.nightToDayRandomProbability);        
        }

        /// <summary>
        /// Current ScheduleProbability according to time
        /// </summary>
        private double ScheduleProbability
        {
            get {
                return IsCurrentTimeDay(this.CurrentTime) ?  this.dayToNightRandomProbability : this.nightToDayRandomProbability;
            }
        }
       
        /// <summary>
        /// decide whether time t is a day time
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public bool IsCurrentTimeDay(int t)
        {
            int modResult = t % Config.SlotPerDay;
            if(6<=modResult && modResult<=9) return true;
            else return false;
        }

        /// <summary>
        /// schedule job with either FirstFit or BestFit based on the dayToNight, nightToDay probability
        /// </summary>
        /// <param name="job"></param>
        public override void ScheduleJob(Job job)
        {                       
            if (RandomGenerator.GetRandomDouble() < this.ScheduleProbability)
            {
                this.firstFitScheduler.ScheduleJob(job);
            }
            else
            {
                this.bestFitScheduler.ScheduleJob(job);
            }
        }       


        /// <summary>
        /// unused currenlty
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public override bool AssignPreemptiveJob(Job job)
        {

            var scheduleProbability = this.nightToDayRandomProbability;

            if (IsCurrentTimeDay(this.CurrentTime) == true)
            {
                scheduleProbability = this.dayToNightRandomProbability;
            }


            if (RandomGenerator.GetRandomDouble() < scheduleProbability)
            {
                return this.firstFitScheduler.AssignPreemptiveJob(job);
            }
            else
            {
                return this.bestFitScheduler.AssignPreemptiveJob(job);
            }
        }

        /// <summary>
        /// unused currently
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public override bool AssignSemiPreemptiveJob(Job job)
        {
            var schedulProbability = this.nightToDayRandomProbability;

            if (IsCurrentTimeDay(this.CurrentTime) == true)
            {
                schedulProbability = this.dayToNightRandomProbability;
            }

            if (RandomGenerator.GetRandomDouble() < schedulProbability)
            {
                return this.firstFitScheduler.AssignSemiPreemptiveJob(job);
            }
            else
            {
                return this.bestFitScheduler.AssignSemiPreemptiveJob(job);
            }
        }
       
    }

}
