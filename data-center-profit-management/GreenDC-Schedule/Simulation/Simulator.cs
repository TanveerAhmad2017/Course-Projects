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
        public ProblemSetting OriginPS { get; private set; }
        public List<Job> OriginJobs { get; private set; }

        #endregion


        public Simulator(IScheduler scheduler, ProblemSetting ps, List<Job> jobs)
        {
            this.Scheduler = scheduler;
            this.PS = ps.Clone();
            this.Jobs = jobs.DeepClone();
            this.OriginPS = ps.Clone();
            this.OriginJobs = jobs.DeepClone();
        }

        /// <summary>
        /// simulate scheduling and return scheduled scheduledJobs
        /// </summary>
        /// <returns></returns>
        public SimulateResult Simulate()
        {          
            for (var t = 1; t <= PS.TimeSlots; t++)
            {
                // create some scheduledJobs
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


        private SimulateResult GenerateSimulationResult(List<Job> scheduledJobs) {
            var sr = new SimulateResult
            {
                SchedulerType = this.Scheduler.SchedulerType,
                ScheduledProfit = scheduledJobs.Sum(j => j.Profit),
                UsedGreenEnergy = scheduledJobs.Sum(j => j.UsedGreenEnergyAmount),
                UsedBrownEnergyAmount = scheduledJobs.Sum(j => j.UsedBrownEnergyAmount),
                UsedBrownEnergyCost = scheduledJobs.Sum(j => j.UsedBrownEnergyCost),
                ScheduledJobs = scheduledJobs,
                ScheduledWorkloadUtilization = (double)scheduledJobs.Sum(j => j.RequiredNodes*j.ProcessingTime)/this.PS.ClusterNodeNum/this.PS.TimeSlots,
                ArrivedWorkloadUtilization = (double) OriginJobs.Sum(j => j.RequiredNodes*j.ProcessingTime)/this.PS.ClusterNodeNum/this.PS.TimeSlots
                //ArrivedWorkloadUtilization = (double)this.PS.job
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
        public double ScheduledWorkloadUtilization { get; set; }
        public double ArrivedWorkloadUtilization { get; set; }
        public double AvgUnitBrownCost { get { return UsedBrownEnergyAmount == 0 ? 0 : (double)UsedBrownEnergyCost / UsedBrownEnergyAmount; } }

        public override string ToString()
        {
            //return string.Format("{0} SProf = {1} UsedGreen = {2} UsedBrown = {3} UsedBrownCost = {4} ScheJobNum = {5}", SchedulerType, ScheduledProfit, UsedGreenEnergy, UsedBrownEnergyAmount, UsedBrownEnergyCost, ScheduledJobs.Count);

            return string.Format("{0} {1} {2} {3} {4} {5} {6} {7} {8}", SchedulerType, ScheduledProfit, UsedGreenEnergy,
                UsedBrownEnergyAmount, UsedBrownEnergyCost, ScheduledJobs.Count, ScheduledWorkloadUtilization, ArrivedWorkloadUtilization, AvgUnitBrownCost);
        }
    }


}
