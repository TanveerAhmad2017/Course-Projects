using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

namespace LibGreenDC
{
    public static class JobExtension
    {

        public static List<Job> GetAvaiableJobs(this List<Job> self, int t)
        {
            var jb = new  List<Job>();
            foreach (var p in self)
            {
                if (p.ArrivalTime <= t)  jb.Add(p);
            }
            return jb;
        }
        
        public static double GetUsedGreenEnergyByTimeSlot(this List<Job> self, int t)
        {
            return self.Sum(j => j.UsedGreenEnergyByTime[t]);
        }       
    }
}
