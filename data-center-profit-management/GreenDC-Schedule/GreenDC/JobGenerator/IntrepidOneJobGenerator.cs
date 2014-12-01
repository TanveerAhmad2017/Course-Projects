using GreenSlot.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.IO;
using GreenSlot.Util;


namespace GreenSlot.JobGenerator
{
    /// <summary>
    /// generate jobs from real traces 2(ANL-Intrepid)
    /// </summary>
    public class IntrepidOneJobGenerator:IJobGenerator
    {
        public String filePath;
        
        /// <summary>
        /// generate jobs to be input of the simulation
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        /// 
        public IntrepidOneJobGenerator() {
            this.filePath = Config.dirIntrepidOne;
        }
        public List<Job> GenerateJob(Config config)
        {
            
            List<Job> jobs = loadANLWorkload(config, filePath);
            return jobs;
        }

        /// <summary>
        /// load real job traces from file
        /// random sample job numbers to meet requested job number
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public List<Job> loadANLWorkload(Config config, String filePath)
        {
            String line;
            StreamReader file = new StreamReader(filePath);
            List<Job> jobs = new List<Job>();
            while ((line = file.ReadLine()) != null) { 
                String[] items = line.Split(' ');
                int arr_time =  Convert.ToInt32( items[0] );
                int proc_time = Convert.ToInt32( items[1] );
                int deadline = Convert.ToInt32( items[2] );
                int node = Convert.ToInt32( items[3] );
                int weight = proc_time * node;
                int energyCost = proc_time * node;

                if (config.isDeadlineBounded)
                {
                    deadline = arr_time + (int)(proc_time / config.L);
                    deadline = Math.Min(config.TimeSlots, deadline);
                }
               

                Job newJB = new Job(arr_time, deadline, weight, energyCost, proc_time, node);
                jobs.Add(newJB);
             }

            // rescale job

            jobs = RescaleJob(jobs, config);

            return jobs;         
        }



        public static List<Job> RescaleJob(List<Job> jobs, Config config)
        {
            var sw = new Stopwatch();
            sw.Start();

            List<int> list = new List<int>();

            List<int> parentList = new List<int>();

            for (int i = 0; i < jobs.Count; i++)
            {
                parentList.Add(i);
            }

            jobs.ForEach(j =>
            {
                
                j.RequiredTimeSlots = Math.Min(j.RequiredTimeSlots, config.SlotPerDay / 2);
                j.RequiredNodes = (int)Math.Min((double)j.RequiredNodes, config.ClusterNodeNum / 2);

                var deadline = RandomGenerator.GetRandomInteger(j.ArrivalTime + j.RequiredTimeSlots, config.TimeSlots);

                if (config.isDeadlineBounded)
                {
                    deadline = j.ArrivalTime + (int)(j.RequiredTimeSlots / config.L);
                }


                // hard limt deadline to config.TimeSlots - 1
                j.Deadline = Math.Min(deadline, config.TimeSlots - 1);

                var energyCost = j.RequiredTimeSlots * j.RequiredNodes * config.RevenueRate;
                j.Weight = energyCost;
            });


            List<Job> rntJob = new List<Job>();
            rntJob = jobs.GenerateJobByUtilization(config);


            sw.Stop();
            Console.WriteLine("Rescale cost = {0}ms", sw.ElapsedMilliseconds);


            return rntJob;

        }

    }


}
