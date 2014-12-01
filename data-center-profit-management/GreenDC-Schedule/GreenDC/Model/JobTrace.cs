using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GreenSlot.Util;

namespace GreenSlot.Model
{
    public class JobTrace
    {

        private static Dictionary<string, List<Job>> _cache;

        static JobTrace()
        {
            _cache = new Dictionary<string, List<Job>>();
        }

        /// <summary>
        /// get jobs from real traces, jobs not satisfied with constraints will be filter out
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="config"></param>
        /// <param name="month"></param>
        /// <param name="Startday"></param>
        /// <returns></returns>
        public static List<Job> ReadEntries(string fileName, Config config, string month, int Startday)
        {
            List<Job> rtn = new List<Job>();
           
            int TimeInterval = 15 * 60;

            string StartTimeString = Startday + " " + month + " 2005 00:00:00 GMT";
            if (Startday <= 9)
            {
                StartTimeString = "0"+Startday + " " + month + " 2005 00:00:00 GMT";
            }
            var GlobalStartTime = Convert.ToDateTime(Startday + " "+month+" 2005 00:00:00 GMT");

            var day = config.TimeSlots / config.SlotPerDay;


            using (var file = new StreamReader(fileName))
            {
                while (!file.EndOfStream)
                {
                    var line = file.ReadLine();
                    if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line)) continue;

                    var items = line.Split('\t');

                    var rawTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(long.Parse(items[1]));

                    var arrivalTime = (int)Math.Ceiling(rawTime.Subtract(GlobalStartTime).TotalSeconds / TimeInterval);

                    var timeCost = (int)Math.Ceiling(double.Parse(items[8]) / TimeInterval);

                    var nodeNum = int.Parse(items[7]);

                    Job job = new Job(arrivalTime, 0, 0, 0, timeCost, nodeNum);

                   
                    if (job.ArrivalTime < 0) continue;
                    if (job.ArrivalTime + job.RequiredTimeSlots >= config.TimeSlots - 1)
                    {
                        continue;
                    }

                    //if (job.RequiredTimeSlots > config.SlotPerDay /4 && config.IsFilterLargeJob)
                    //{
                    //    DiscardedLargeJobNum += 1;
                    //    continue;
                    //}

                    if (job.ArrivalTime + config.AvgProcessTime >= config.TimeSlots - 1) continue;                  
                    if ((job.ArrivalTime) > config.TimeSlots) break;
                    if (job.RequiredTimeSlots <= 0) continue;

                    rtn.Add(job);
                }
              
                Console.WriteLine("Raw Job Num ={0}", rtn.Count);
                
            }

            rtn.Sort(new JobArrialTimeASCComparer());

            //JobUtil.writeJobToFile("RawJobs.txt", rtn);  
            return rtn;
        }

        /// <summary>
        /// get all jobs from real traces
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static List<KeyValuePair<double,double>> LoadAllJobs(String fileName,Config config)
        {
            var rtn = new List<KeyValuePair<double, double>>();
           

          
            using (var file = new StreamReader(fileName))
            {
                while (!file.EndOfStream)
                {
                    var line = file.ReadLine();
                    if (line.StartsWith("#") || string.IsNullOrWhiteSpace(line)) continue;

                    var items = line.Split('\t');

                    

                    var timeCost = (int)Math.Ceiling(double.Parse(items[8]));
                   // Console.WriteLine("timeCost: {0}", timeCost);
                    var nodeNum = int.Parse(items[7]);

                    //Job job = new Job(1, 0, 0, 0, timeCost, nodeNum);

                    var job = new KeyValuePair<double, double>(timeCost, nodeNum);


                    //if (job.ArrivalTime < 0) continue;
                    //if (job.ArrivalTime + job.RequiredTimeSlots >= config.TimeSlots - 1)
                    //{
                    //    //DiscardedLargeJobNum += 1;
                    //    continue;
                    //}
                  
                    //if (job.ArrivalTime + config.AvgProcessTime >= config.TimeSlots - 1) continue;
                    //if (job.ArrivalTime + config.LeadTime + job.RequiredTimeSlots >= config.TimeSlots - 1) continue;
                    //if ((job.ArrivalTime) > config.TimeSlots) break;
                    if (timeCost <= 0) continue;
                    if (nodeNum <= 0) continue;

                    // Console.WriteLine(arrivalTime);
                    // if (jobentry.ArrivalTime < JobEntry.GlobalStartTime) JobEntry.GlobalStartTime = jobentry.ArrivalTime;

                    rtn.Add(job);
                }

              


                Console.WriteLine("Raw Job Num ={0}", rtn.Count);

            }

           // rtn.Sort(new JobArrialTimeASCComparer());

           // JobUtil.writeJobToFile("AllJobsAnalyze.txt", rtn);
            // rtn.ForEach(j => {Console.WriteLine(j.ArrivalTime);});


            return rtn;
        }


       
        
        /// <summary>
        /// call ReadEntries to get jobs from real traces
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public static List<Job> LoadJobTrace(Config config, String month, int day)
        {

            var key = month + "_" + day;

            if (!_cache.ContainsKey(key))
            {

                //change start day from 20 to 22
                var list = ReadEntries(Config.dirWorkload, config, month, day);

                list = RescaleJobs(list, config);

                _cache[key] = list;
            }

            // var rst = list.GroupBy(e => e.ArrivalTime).ToList();

            return new List<Job>(_cache[key]);
        }

        private static List<Job> RescaleJobs(List<Job> jobs, Config config)
        {
            var rntJob = new List<Job>();

            jobs.ForEach(job =>
            {
                var j = job.DeepClone();

                j.RequiredNodes = j.RequiredNodes * 2;
                j.RequiredTimeSlots = 5 * j.RequiredTimeSlots;
                j.RequiredTimeSlots = Math.Min(j.RequiredTimeSlots, config.SlotPerDay / 2);
                j.RequiredNodes = (int)Math.Min((double)j.RequiredNodes, config.ClusterNodeNum / 2);

                rntJob.Add(j);
            });

            rntJob.RemoveAll(jb => { return (jb.ArrivalTime + jb.RequiredTimeSlots >= config.TimeSlots); });
            //filter out large jobs
            rntJob.RemoveAll(jb => { return (jb.RequiredTimeSlots >= config.TimeSlots / 4); });

            for (int j = 0; j < rntJob.Count; j++)
            {
                Job jb = rntJob[j];

                var deadline = RandomGenerator.GetRandomInteger(jb.ArrivalTime + jb.RequiredTimeSlots, config.TimeSlots);

                if (config.isDeadlineBounded)
                {
                    deadline = jb.ArrivalTime + (int)(jb.RequiredTimeSlots / config.L);
                }


                // hard limt deadline to config.TimeSlots - 1
                jb.Deadline = Math.Min(deadline, config.TimeSlots - 1);

                var energyCost = jb.RequiredTimeSlots * jb.RequiredNodes * config.RevenueRate;
                jb.Weight = energyCost;
            }

            return rntJob;
        }

    }

}