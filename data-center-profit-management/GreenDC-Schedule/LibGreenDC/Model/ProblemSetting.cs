using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using LibGreenDC.Model;

namespace LibGreenDC
{
    [Serializable]
    public class ProblemSetting
    {


        public int ClusterNodeNum { get; set; }
        public int TimeSlots { get; set; }
        public double RevenueRate { get; set; }

        public List<Job> Jobs { get; set; }
        
        public List<double> BrownPriceList { get; set; }
        public List<int> SolarEnergyList { get; set; }

        public ProblemSetting Clone()
        {
            var ps = new ProblemSetting
            {
                
                Jobs = new List<Job>(),
                TimeSlots = this.TimeSlots,
                BrownPriceList = this.BrownPriceList !=null ? new List<double>(this.BrownPriceList) : null,
                SolarEnergyList = this.SolarEnergyList !=null ? new List<int>(this.SolarEnergyList): null
            };

            Jobs.ForEach(job =>
                {
                    ps.Jobs.Add(job.DeepClone());
                });

            return ps;

        }

        public void Save(string path)
        {
            using (var sw = new StreamWriter(path))
            {
                var xs = new XmlSerializer(typeof(ProblemSetting));
                xs.Serialize(sw, this);
            }
        }

       
        //public static ProblemSetting InitProblemSetting(Config config)
        //{
           
        //    //TODO, reconstruct
        //    //rescale config
        //    if (config.Scale > 1)
        //    {
        //        config.RescaleConfig();
        //    }

        //    Console.WriteLine("generateJobs...");
        //    List<Job> jobs;

        //    //Generate jobs
        //    jobs = JobGenerator.JobGeneratorFactory.GetJobGenerator(config.JobArrivalType).GenerateJob(config);

        //    config.JobNum = jobs.Count;

        //    if (config.JobLengthType == JobLengthType.Equal)
        //    {
        //        for (int i = 0; i < jobs.Count; i++)
        //        {
        //            jobs[i].RequiredNodes = (int)config.AvgNodeNum;
        //            jobs[i].ProcessingTime = config.AvgProcessTime;

        //            if (config.isDeadlineBounded) {
        //                var deadline = jobs[i].ArrivalTime + (int)(jobs[i].ProcessingTime / config.L);
        //                deadline = Math.Min(config.TimeSlots, deadline);
        //                jobs[i].Deadline = deadline;
        //            }

        //        }
                

                
        //    }          

        //    Console.WriteLine("JobNum= {0}", jobs.Count);

        //    //generate brown energy price list
        //    Console.WriteLine("Generate brownPriceList...");
        //    List<double> brownPrice;
        //    brownPrice = BrownPriceUtil.generateBrownPrice(config.TimeSlots, config.SlotPerDay, config.DayPrice, config.NightPrice, config.DayBegin);
        //    Console.WriteLine("BrownPriceList count={0}", brownPrice.Count);

        //    //generate solar energy list
        //    Console.WriteLine("Generate solarEnergyList...");
        //    List<int> solarEnergy;
        //    if (!config.IsComOpt)
        //    {
        //        solarEnergy = SolarUtil.generateSolarEnergy(config.TimeSlots, config.Scale, config.ClusterNodeNum, config.SolarLoadFactor);
        //        if (config.OutputToLingo)
        //        {
        //            SolarUtil.writeSolarToFile(Config.LingoSolarFile, solarEnergy);
        //            JobUtil.writeJobToFile(Config.LingoJobFile, jobs);
        //        }
                
        //    }
        //    else
        //        solarEnergy = SolarUtil.GenerateFixedSolarTrace(Config.FixedSolarFile);
            
        //    Console.WriteLine("SolarEnergyList count ={0}", solarEnergy.Count);
        //    Console.WriteLine("SolarEnergyList avg ={0}", solarEnergy.Average());

            

        //    var ps = new ProblemSetting
        //    {
        //        Jobs = new List<Job>(jobs),
        //        TimeSlots = config.TimeSlots,
        //        BrownPriceList = brownPrice,
        //        SolarEnergyList = solarEnergy
        //    };

        //    //set job id
        //    //TODO, explain why set job ID
        //    //ps.Jobs.Sort((j1, j2) => { return j1.ArrivalTime.CompareTo(j2.ArrivalTime); });
        //    ps.Jobs.Sort(new JobArrialTimeASCComparer());

        //    var jobId = 1;

        //    ps.Jobs.ForEach(j => { j.JobId = jobId++; });
           
        //    return ps;
            
        //}

     

    }
}
