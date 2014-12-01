using GreenSlot.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using GreenSlot.Util;

namespace GreenSlot.JobGenerator
{
    /// <summary>
    /// generate jobs from real traces
    /// </summary>
    public class Grid5kOneJobGenerator:IJobGenerator
    {
       // public static List<Job> cacheJobs;
        public String month;
        public int day;

        public Grid5kOneJobGenerator() {
            this.month = "Jul";
            this.day = 22;
        }

        public List<Job> GenerateJob(Config config)
        {          
                    
            List <Job> tmpJobs = JobTrace.LoadJobTrace(config, month, day);

            var jobs = tmpJobs.GenerateJobByUtilization(config);

            return jobs;
        }

     


        

       

        public void Save(string path, List<Job> jobs)
        {
            var ser = new XmlSerializer(jobs.GetType());
            using (var sw = new StreamWriter(path))
            {
                ser.Serialize(sw, jobs);
            }
        }

        public static List<Job> Load(string path)
        {
            var ser = new XmlSerializer(typeof(List<Job>));
            List<Job> jobs = null;
            using (var sr = new StreamReader(path))
            {
                jobs = (List<Job>)ser.Deserialize(sr);
            }

            return jobs;
        }

    }


}
