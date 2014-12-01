using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GreenSlot.Model;

namespace GreenSlot.Simulations
{
    class Sim_RealWorkload_Analysis
    {

        public static void AnalyzeRealTrace(Config config)
        {
            config.IsFilterLargeJob = false;
         
            var jobs = JobTrace.LoadAllJobs(Config.dirWorkload,config);

            using (var file = new StreamWriter(Config.RealJobRecord))
            {
                jobs.ForEach(j =>
                {
                    file.WriteLine("{0},{1},{2}", j.Key, j.Value, j.Key * j.Value);
                });

            }


        }
    }
}
