using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenSlot.Util;

namespace GreenSlot
{
    public class SimOpt
    {
        const int processingTime = 20;
        const int requiredNode = 8;
        List<Job> jobs;
        Config config;
      
       public SimOpt(Config config)
       {
           this.config = config;
           this.jobs = new List<Job>();
       }


       public void GenerateFixedJob()
       {
           bool[,] map = new bool[config.ClusterNodeNum, config.TimeSlots];
           GenerateRawJob(map);
           var solarEnergyTrace = CalculateScheduleResult(map);

           
           for (int i = 0; i < jobs.Count; i++)
           {
               var scheduleTIme = jobs[i].ArrivalTime;
               jobs[i].ArrivalTime = RandomGenerator.GetRandomInteger(0, scheduleTIme);
               jobs[i].Deadline = RandomGenerator.GetRandomInteger(scheduleTIme + processingTime+1, config.TimeSlots);
           }

           JobUtil.writeJobToFile(Config.FixedJobFile, jobs);
           SolarUtil.writeSolarToFile(Config.FixedSolarFile, solarEnergyTrace);
              
       }

        public List<int> CalculateScheduleResult(bool[,] Map)
        {
             var brownPrice = BrownPriceUtil.generateBrownPrice(config.TimeSlots, config.SlotPerDay, config.DayPrice, config.NightPrice, config.DayBegin);
             int usedGreenEnergy = 0;
             int usedBrownEnergy = 0;
             double usedBrownEnergyCost = 0;
             var solarEnergyList = new List<int>();
             double profit;
            for(int i=0; i<config.TimeSlots; i++)
            {            
                int usedCurrentSlot = 0;
                for (int j = 0; j < config.ClusterNodeNum; j++)
                {
                    if (Map[j, i] == true)
                    {
                        usedCurrentSlot += 1;
                    }                 
                }

                if (brownPrice[i] == config.NightPrice)
                {
                    usedBrownEnergy += usedCurrentSlot;
                    usedBrownEnergyCost += usedCurrentSlot * brownPrice[i];
                    solarEnergyList.Add(0);
                }
                else {
                    usedGreenEnergy += usedCurrentSlot;
                    solarEnergyList.Add(usedCurrentSlot);
                }
            }

            profit = processingTime * requiredNode * config.JobNum * config.RevenueRate - usedBrownEnergyCost;

            Console.WriteLine("JobNum = {0}, GESum ={1}", config.JobNum, solarEnergyList.Sum());
            Console.WriteLine("TR ={0}, GE ={1}, BE ={2}，BEC={3}", profit, usedGreenEnergy, usedBrownEnergy, usedBrownEnergyCost);
            return solarEnergyList;
        }


        public void GenerateRawJob(bool[,] map)
        {

            while(jobs.Count < config.JobNum)
            {
                Console.WriteLine("Curr JobNum ={0}", jobs.Count);
                var arrivalTime = RandomGenerator.GetRandomInteger(0, config.TimeSlots -  processingTime - 1);

                var slotPerHour = config.SlotPerDay/24;
                //if (arrivalTime % 24 >= 18 * slotPerHour && arrivalTime % 24 <= 21 * slotPerHour)
                //{
                //    continue;
                //}
                var nodeList = new List<int>();
                var canFit = TestFit(map, arrivalTime, out nodeList);

                if (canFit)
                {
                    map = FillMap(map, arrivalTime, nodeList);
                    var job = new Job
                    {
                        ArrivalTime = arrivalTime,
                        RequiredTimeSlots = processingTime,
                        RequiredNodes = requiredNode
                    };
                    jobs.Add(job);
                }
               
            }
        }

        public bool TestFit(bool[,] map, int startTime, out List<int> nodeList)
        {
            List<int> assignedNodeList = new List<int>();
            for (int node = 0; node < config.ClusterNodeNum; node++)
            {
                bool nodeFit = true;
                for(int t=startTime; t< startTime+processingTime; t++)
                {
                    if (map[node,t] == true)
                    {
                        nodeFit = false;
                        break;
                    }
                }

                if (nodeFit == true)
                {
                    assignedNodeList.Add(node);
                    if (assignedNodeList.Count == requiredNode) break;
                }                             
            }

            if (assignedNodeList.Count == requiredNode)
            {
                nodeList = assignedNodeList; 
                return true;
                
            }
            nodeList = null;
            return false;
        }

        public bool[,] FillMap(bool[,] map, int startTime, List<int> nodeList)
        {
            nodeList.ForEach(node =>
                {
                    for (int t = startTime; t < startTime + processingTime; t++)
                    {
                        map[node, t] = true;
                    }
                });

            return map;
        }

       


      
    }
}
