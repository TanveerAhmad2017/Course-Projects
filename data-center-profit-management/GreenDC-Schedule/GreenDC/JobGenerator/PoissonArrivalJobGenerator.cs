using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.JobGenerator
{
    /**
     * generate jobs that arrive in Poisson pattern
     * 
     */
    class PoissonJobGenerator:IJobGenerator
    {
        /// <summary>
        /// get Poisson arrival time list
        /// </summary>
        /// <param name="config"></param>
        /// <returns></returns>
        public List<Job> GenerateJob(Config config)
        {

            
            List<int> arrivalTimeList = FileUtil.GeneratePoissonArrTimeList(config.ArrivalRate, config.TimeSlots, config.AvgProcessTime);

            List<Job> jobs = new List<Job>();
            jobs.Clear();

            //assign each arrival job a time in the timelist(in order)
            for (int i = 0; i < arrivalTimeList.Count; i++)
            {
                var arrivalTime = arrivalTimeList[i];

                //repeat find a satisfied time cost
                int tolerate = 5;
                var timeCost = RandomGenerator.GetRandomInteger(1, 2 * config.AvgProcessTime);
                while (tolerate > 0 && timeCost + arrivalTime >= config.TimeSlots) {
                    timeCost = RandomGenerator.GetRandomInteger(1, 2 * config.AvgProcessTime);
                }

                if (timeCost + arrivalTime >= config.TimeSlots) continue;


                var deadline = RandomGenerator.GetRandomInteger(arrivalTime + timeCost, config.TimeSlots);

                if (config.isDeadlineBounded) {
                    deadline = arrivalTime + (int)(timeCost / config.L);
                }

                // hard limt deadline to config.TimeSlots - 1
                deadline = Math.Min(deadline, config.TimeSlots - 1);

                // var weight = RandomGenerator.GetRandomInteger(1, weigtRange);
                var nodeNum = RandomGenerator.GetRandomInteger(1, (int)(2 * config.AvgNodeNum));

                nodeNum = Math.Min(nodeNum, config.ClusterNodeNum);

                var energyCost = timeCost * nodeNum * config.RevenueRate;
                var weight = energyCost;

                Job jb = new Job(arrivalTime, deadline, weight, energyCost, timeCost, nodeNum);
                jobs.Add(jb);
            }

            return jobs;

        }
    }
}
