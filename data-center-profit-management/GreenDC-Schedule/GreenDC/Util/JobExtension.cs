using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;

namespace GreenSlot.Util
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

        public static Job DeepClone(this Job obj)
        {
            using (var ms = new MemoryStream())
            {
                XmlSerializer xs = new XmlSerializer(typeof(Job));
                xs.Serialize(ms, obj);
                ms.Flush();
                ms.Position = 0;
                var cloned = (Job)xs.Deserialize(ms);

                cloned.Color = obj.Color;

                return cloned;
            }
        }

        
        public static double GetUsedGreenEnergyByTimeSlot(this List<Job> self, int t)
        {
            return self.Sum(j => j.UsedGreenEnergyByTime[t]);
        }


        /// <summary>
        /// Rescale requiredNodeNumber of job
        /// </summary>
        /// <param name="jobs"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static List<Job> GenerateJobByUtilization(this List<Job> jobs, Config config)
        {
            var sw = new Stopwatch();
            sw.Start();

            List<int> list = new List<int>();

            List<int> parentList = new List<int>();

            for (int i = 0; i < jobs.Count; i++)
            {
                parentList.Add(i);
            }

            //get job number equal to config.JobNum
            list = parentList.OrderBy(x => RandomGenerator.GetRandomInteger(0, int.MaxValue)).ToList();
            List<Job> rntJob = new List<Job>();

            var ultilization = 0.0;
            var criticalUtilization = config.JobUtilization * config.ClusterNodeNum * config.TimeSlots;

            for (int i = 0; i < list.Count; i++)
            {
                var job = jobs[list[i]];

                rntJob.Add(job);

                ultilization += (double)job.RequiredNodes * job.RequiredTimeSlots;

                if (ultilization >= criticalUtilization)
                {
                    break;
                }
            }

            sw.Stop();
            Console.WriteLine("Rescale cost = {0}ms, Generate Num = {1}, Utilization = {2}", sw.ElapsedMilliseconds, rntJob.Count, ultilization);


            return rntJob;

        }
    }
}
