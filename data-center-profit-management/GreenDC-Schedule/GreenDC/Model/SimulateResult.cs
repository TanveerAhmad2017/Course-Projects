using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.Model
{
    public class SimulateResult
    {
        public double Workload{get; set;}
        public double ScheduledJobNum{get; set;}
        public double ScheduledRevenue{get; set;}
        public double UsedGreenEnergy{get; set;}
        public double UsedBrownEnergy { get; set; }
        public double UsedBrownEnergyCost { get; set; }

        public double RunTime { get; set; }


        public SimulateResult(double workload, double scheduledJobNum, double scheduledRevenue, double usedGreenEnergy, double usedBrownEnergy, double UsedBrownEnergyCost, double RunTime)
        {
            this.Workload = workload;
            this.ScheduledJobNum = scheduledJobNum;
            this.ScheduledRevenue = scheduledRevenue;
            this.UsedGreenEnergy = usedGreenEnergy;
            this.UsedBrownEnergy = usedBrownEnergy;
            this.UsedBrownEnergyCost = UsedBrownEnergyCost;
            this.RunTime = RunTime;
        }

     }
}
