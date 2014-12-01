using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.JobGenerator
{
    /// <summary>
    /// generate jobs that arrive in uniform pattern
    /// </summary>
    public class UniformJobGenerator:IJobGenerator
    {

        public List<Job> GenerateJob(Config config)
        {
            List<Job> jobs = new List<Job>(config.JobNum);
            jobs.Clear();

            //avoid having arrival time at the end of the simulation
            int margin = 5;

            for (int i = 0; i < config.JobNum; i++)
            {
                var arrivalTime = RandomGenerator.GetRandomInteger(0, config.TimeSlots - margin);

                int tolerate = 5;
                var timeCost = RandomGenerator.GetRandomInteger(1, 2 * config.AvgProcessTime);
                while (tolerate > 0 && timeCost + arrivalTime >= config.TimeSlots)
                {
                    timeCost = RandomGenerator.GetRandomInteger(1, 2 * config.AvgProcessTime);
                }

                if (timeCost + arrivalTime >= config.TimeSlots) continue;
            
                var deadline = RandomGenerator.GetRandomInteger(arrivalTime + timeCost, config.TimeSlots);

                if (config.isDeadlineBounded)
                {
                    deadline = arrivalTime + (int)(timeCost / config.L);
                }

                // hard limt deadline to config.TimeSlots - 1
                deadline = Math.Min(deadline, config.TimeSlots - 1);

                // var weight = RandomGenerator.GetRandomInteger(1, weigtRange);
                var nodeNum = RandomGenerator.GetRandomInteger(1, (int)(2 * config.AvgNodeNum));

                nodeNum = Math.Min(config.ClusterNodeNum, nodeNum);

                var energyCost = timeCost * nodeNum * config.RevenueRate;
                var weight = energyCost;

                Job jb = new Job(arrivalTime, deadline, weight, energyCost, timeCost, nodeNum);
                jobs.Add(jb);
            }

            return jobs;
        }
    }
}
