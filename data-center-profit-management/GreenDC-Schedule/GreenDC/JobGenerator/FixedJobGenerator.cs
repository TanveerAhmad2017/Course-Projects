using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenSlot.Util;

namespace GreenSlot.JobGenerator
{
    
    class FixedJobGenerator:IJobGenerator
    {
        public List<Job> GenerateJob(Config config)
        {
            List<Job> jobs = JobUtil.ReadJobsFromFile(Config.FixedJobFile);
            return jobs;           
        }
    }
}
