using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.JobGenerator
{
    
    /// <summary>
    /// generate jobs that arrive in staggered pattern
    /// </summary>
    public class StaggeredJobGenerator:IJobGenerator
    {
        public List<Job> GenerateJob(Config config)
        {
            List<Job> jobs = new List<Job>();

            jobs.Clear();

            int dayNum = config.TimeSlots / config.SlotPerDay;

            int jobPerDay = config.JobNum / dayNum;


            for (int i = 0; i < dayNum; i++)
            {
                int generateJobNum = jobPerDay;
                // if (i == jobNum - 1) generateJobNum = jobPerDay / 2;
                for (int j = 0; j < generateJobNum; j++)
                {

                    var arrivalTime = RandomGenerator.GetRandomInteger((i * 24 + config.DayBegin) * (config.SlotPerDay / 24),(i * 24 + config.DayBegin+12) * (config.SlotPerDay / 24));
                    //day job end by day
                    var deadline = (int)(arrivalTime + config.StaggeredJobSpan * config.SlotPerDay);
                    if (deadline > config.TimeSlots) deadline = config.TimeSlots;

                    //night job end by day
                    if (j < generateJobNum * config.NightPortion)
                    {
                        arrivalTime = RandomGenerator.GetRandomInteger(Math.Max((i * 24 - 3), 0) * (config.SlotPerDay / 24), (i * 24 - 3+ 12) * (config.SlotPerDay / 24));
                        //arrivalTime = (i == 0) ? 0 : (i * 24 - 3) * (config.SlotPerDay / 24);
                        deadline = (int)(arrivalTime + config.StaggeredJobSpan * config.SlotPerDay);

                        


                        if (deadline > config.TimeSlots) deadline = config.TimeSlots;

                    }

                    var nodeNum = RandomGenerator.GetRandomInteger(1, (int)(2 * config.AvgNodeNum));
                    var timeCost = RandomGenerator.GetRandomInteger(1, 2 * config.AvgProcessTime);
                    var energyCost = timeCost * nodeNum * config.RevenueRate;
                    var weight = energyCost;

                    //TODO: need further investigate 
                    if (config.isDeadlineBounded)
                    {
                        deadline = arrivalTime + (int)(timeCost/ config.L);
                    }

                    Job jb = new Job(arrivalTime, deadline, weight, energyCost, timeCost, nodeNum);

                    jobs.Add(jb);
                }


            }
            return jobs;
        }
    }
}
