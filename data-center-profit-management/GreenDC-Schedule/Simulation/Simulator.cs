using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibGreenDC;

namespace Simulation
{
    public class Simulator
    {
        #region properties
        public IScheduler Scheduler { get; private set; }
        public ProblemSetting PS { get; private set; }
        public List<Job> Jobs { get; private set; }

        #endregion


        public Simulator(IScheduler scheduler, ProblemSetting ps, List<Job> jobs)
        {
            this.Scheduler = scheduler;
            this.PS = ps;
            this.Jobs = jobs;
        }

        /// <summary>
        /// simulate scheduling and return scheduled jobs
        /// </summary>
        /// <returns></returns>
        public SimulateResult Simulate()
        {

            //firstfitscheduler.sc
            this.PS = PS.Clone();
            this.Jobs  = Jobs.DeepClone();

            for (var t = 1; t <= PS.TimeSlots; t++)
            {
                // create some jobs
                var currentJob = Jobs.Where(j => j.ArrivalTime == t).ToList();
                Scheduler.CurrentTime = t;
                if (currentJob.Count > 0)
                {
                    Scheduler.AddJobs(currentJob);
                    Scheduler.Schedule();
                }
            }

            var sechuledJobs = Scheduler.GetScheduledJobs();

            return this.GenerateSimulationResult(sechuledJobs);
        }


        private SimulateResult GenerateSimulationResult(List<Job> jobs) {
            var sr = new SimulateResult
            {
                SchedulerType = this.Scheduler.SchedulerType,
                ScheduledProfit = jobs.Sum(j => j.Profit),
                UsedGreenEnergy = jobs.Sum(j => j.UsedGreenEnergyAmount),
                UsedBrownEnergyAmount = jobs.Sum(j => j.UsedBrownEnergyAmount),
                UsedBrownEnergyCost = jobs.Sum(j => j.UsedBrownEnergyCost),
                ScheduledJobs = jobs
            };
            return sr;
        }
    }


    public class SimulateResult {
        public SchedulerType SchedulerType { get; set; }
        public double UsedGreenEnergy { get;  set; }
        public double ScheduledProfit { get; set; }
        public double UsedBrownEnergyAmount { get; set; }
        public double UsedBrownEnergyCost { get; set; }
        public List<Job> ScheduledJobs { get; set; }

        public override string ToString()
        {
            return string.Format("{0} SProf = {1} UsedGreen = {2} UsedBrown = {3} UsedBrownCost = {4} ScheJobNum = {5}", SchedulerType, ScheduledProfit, UsedGreenEnergy, UsedBrownEnergyAmount, UsedBrownEnergyCost, ScheduledJobs.Count);
        }
    }


}
