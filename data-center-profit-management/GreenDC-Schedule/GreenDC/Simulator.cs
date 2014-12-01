using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenSlot.Scheduler;
using System.Drawing;
using System.Drawing.Drawing2D;
using GreenSlot.Model;
using GreenSlot.Util;

namespace GreenSlot
{
    public class Simulator
    {
        public long RunId { get; set; }

        Config Config { get;  set; }
        ProblemSetting PS { get;  set; }
        List<IScheduler> Schedulers { get;  set; }

        //use to store simulation result
        //for each scheduler, under each job setting, for each iteration
        Dictionary<SchedulerType, Dictionary<string, List<SimulateResult>>> Results;

        string WorkLoadId { get; set; }


        public Simulator(Config config)
        {
            this.Config = config;
            Job.Config = this.Config;

            //got each scheduler, for each type of workloads
            Results = new Dictionary<SchedulerType, Dictionary<string, List<SimulateResult>>>();

            //CHANGE when add new algorithms!
            var sts = new List<SchedulerType> { SchedulerType.FirstFit, SchedulerType.BestFit, SchedulerType.RandomFit,SchedulerType.GreenSlot, SchedulerType.WeightedSmallWindow, SchedulerType.RandomChoiceSmall, SchedulerType.RandomChoiceMedium, SchedulerType.RandomChoiceLarge};
            //var sts = new List<SchedulerType> { SchedulerType.FirstFit, SchedulerType.BestFit, SchedulerType.RandomFit, SchedulerType.GreenSlot, SchedulerType.RandomChoiceSmall, SchedulerType.RandomChoiceMedium, SchedulerType.RandomChoiceLarge };

            sts.ForEach(st => { Results[st] = new Dictionary<string, List<SimulateResult>>(); }); 
           
        }


       
        /// <summary>
        /// simulation to compare multiple algorithms in their achieved profits
        /// </summary>
        public void Simulate()
        {            
            //set seed
            RandomGenerator.SetSeed(Config.Seed);

            //create job traces
            if (Config.JobArrivalType == JobArrivalType.Poisson)
            {
                SimulatePoissonArrival(this.Config, Config.StartWorkLoad, Config.EndWorkLoad);
            }
            else if (Config.IsComOpt == true)
            {
                SimulateOpt(this.Config);
            }
            else if (Config.JobArrivalType == JobArrivalType.Fixed)
            {
                SimulateFixedJobs(this.Config);
            }
            else { 
                 SimulateUniformArrival(this.Config, Config.StartWorkLoad, Config.EndWorkLoad);
            }
        }


        public Config GetConfig()
        {
            return this.Config;
        }



        /// <summary>
        /// create workload traces 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public void SimulateUniformArrival( Config config, int start, int end)
        {
            int tenPercentNum = 0;
            if (config.JobLengthType == JobLengthType.Equal)
            {
                tenPercentNum = (int)(Config.TimeSlots * Config.ClusterNodeNum / ((Config.AvgNodeNum) * (Config.AvgProcessTime)) * 0.1);
            }
            else { 
                tenPercentNum = (int)(Config.TimeSlots * Config.ClusterNodeNum / ((Config.AvgNodeNum) * (Config.AvgProcessTime)) * 0.1);
            }

            if (config.JobArrivalType == JobArrivalType.Grid5kOne)
            {
                //INFOCOM: set parameter as $100$
                tenPercentNum = 50;
            }

            if (config.JobArrivalType == JobArrivalType.Grid5kTwo)
            {
                //INFOCOM: set parameter as $100$
                tenPercentNum = 60;
            }

            if (config.JobArrivalType == JobArrivalType.Grid5kThree)
            {
                //INFOCOM: set parameter as $100$
                tenPercentNum = 60;
            }

            if (config.JobArrivalType == JobArrivalType.IntrepidOne)
            {
                tenPercentNum = 25;
            }

            if (config.JobArrivalType == JobArrivalType.IntrepidTwo)
            {
                tenPercentNum = 30;
            }

            if (config.JobArrivalType == JobArrivalType.IntrepidThree)
            {
                tenPercentNum = 20;
            }

           

            //for each type of workloads,
            //    for each repetition
            //       do simulation

            //var workloadStart = tenPercentNum*1.5;

       

            for (int j = start; j <= end; j ++)
            {
                Config.loadingFactor = (double)j;
                Console.Title = j * 10 + "%" +"_type"+Config.JobArrivalType;

                Config.JobNum = j * tenPercentNum;
                //TODO: add comments
                WorkLoadId = Config.JobNum.ToString();
             
                Config.JobUtilization = (double)j/10;



                for (var r = 0; r < Config.Repetition; r++)
                {
                    //for each repetition, use a different seed
                    var seed = RandomGenerator.GetRandomInteger(0, int.MaxValue);
                    RandomGenerator.SetSeed(seed);

                    //TODO: add comments
                    RunId = DateTime.Now.ToFileTime() % 100000;

                    //create job traces
                    var jobs = InitSchedule();



                    if (config.outputWorkloadTrace && j == 8  && r == 0)
                    {
                        var jobsToPlot = new List<List<Job>>();
                        jobsToPlot.Add(jobs);
                        FileUtil.PlotJobTraces(jobsToPlot, config);
                    }

                   // RecordJobs.Add(jobs);

                    DoSchedule();

                   
                }

                


                //process repetition result
                ProcessRepetitionResult();
                ProcessRepetitionResultConfInt();
 
            }
 
        }


        public void SimulatePoissonArrival(Config config, int start, int end)
        {

            double TenTharrivalRate = 0.0;
            if (config.JobLengthType == JobLengthType.Equal)
            {
                TenTharrivalRate = (double)(Config.TimeSlots * Config.ClusterNodeNum / ((Config.AvgNodeNum) * (Config.AvgProcessTime)) / config.TimeSlots * 0.1);
            }
            else
            {
                TenTharrivalRate = (double)(Config.TimeSlots * Config.ClusterNodeNum / ((Config.AvgNodeNum + 0.5) * (Config.AvgProcessTime + 0.5)) / config.TimeSlots * 0.1);
            }

            for (int i = start; i <= end; i++)
            {
                Config.loadingFactor = i;
                double j = TenTharrivalRate * i;
                Console.Title = i * 10 + "%";
                //used as key when store the schedule results at this workload
                WorkLoadId = i.ToString();

                Config.ArrivalRate = j;

                List<List<Job>> RecordJobs = new List<List<Job>>();

                for (var r = 0; r < Config.Repetition; r++)
                {
                    RunId = DateTime.Now.ToFileTime() % 100000;
                    Console.WriteLine("Repetition = {0}...", r);

                    var jobs = InitSchedule();
                    RecordJobs.Add(jobs);

                    //plot workload traces
                    if (config.outputWorkloadTrace && i == 8 && r == 0)
                    {
                        var jobsToPlot = new List<List<Job>>();
                        jobsToPlot.Add(jobs);
                        FileUtil.PlotJobTraces(jobsToPlot, config);
                    }

                    DoSchedule();
                }
            }


            ProcessRepetitionResult();
            ProcessRepetitionResultConfInt();
        }


        /// <summary>
        /// simulation using fixed jobs
        /// </summary>
        /// <param name="config"></param>
        public void SimulateFixedJobs(Config config)
        {

            var seed = RandomGenerator.GetRandomInteger(0, int.MaxValue);

            RandomGenerator.SetSeed(seed);
            Console.WriteLine("Set seed as {0}", seed);

            var jobs = InitSchedule();

            DoSchedule();

            ProcessRepetitionResult();
            ProcessRepetitionResultConfInt();

        }

        /// <summary>
        /// comparison with optimal
        /// TODO: add commens and explain
        /// </summary>
        /// <param name="config"></param>
        public void SimulateOpt(Config config)
        {
         
                List<List<Job>> RecordJobs = new List<List<Job>>();

                for (var r = 0; r < Config.Repetition; r++)
                {
                    var seed = RandomGenerator.GetRandomInteger(0, int.MaxValue);
                    RandomGenerator.SetSeed(seed);
                    Console.WriteLine("Set seed as {0}", seed);

                    RunId = DateTime.Now.ToFileTime() % 100000;
                    Console.WriteLine("Repetition = {0}...", r);

                    var jobs = InitSchedule();
                    RecordJobs.Add(jobs);

                    DoSchedule();
                }

                if (config.outputWorkloadTrace)
                    FileUtil.PlotJobTraces(RecordJobs, config);
 
                ProcessRepetitionResult();
                ProcessRepetitionResultConfInt();

        }        


        public void ProcessRepetitionResult()
        {
            this.Schedulers.ForEach(sch =>
                {

                    var result = Results[sch.SchedulerType];
                    var fileName = Config.OUTPUTDIR;
                    if (Config.IsSemiPreemptive == true)
                    {
                        fileName += "Prempt_";
                    }
                     fileName += sch.GetType().Name + "_" + Config.JobArrivalType + "_" + Config.JobLengthType;
                    
                    using (var file = new System.IO.StreamWriter(fileName + ".txt"))
                    {
                        file.WriteLine("%ScheduledJobNum workload ScheduledRevenue UsedGreenEnergy UsedBrownEnergy UsedBrownEnergyCost RunningTime");
                        foreach (string key in result.Keys)
                        {
                            var list = result[key];
                            {

                                file.Write(list.Average(s => s.ScheduledJobNum) + " ");


                                file.Write(list.Average(s => s.Workload) + " ");


                                file.Write(list.Average(s => s.ScheduledRevenue) + " ");


                                file.Write(list.Average(s => s.UsedGreenEnergy) + " ");


                                file.Write(list.Average(s => s.UsedBrownEnergy) + " ");


                                file.Write(list.Average(s => s.UsedBrownEnergyCost) + " ");

                                file.Write(list.Average(s => s.RunTime) + " ");

                                file.WriteLine();
                            }

                        }
                    }

                   
                });

        }

        public void ProcessRepetitionResultConfInt()
        {
            this.Schedulers.ForEach(sch =>
            {

                var result = Results[sch.SchedulerType];
                var fileName = Config.OUTPUTDIR;
                if (Config.IsSemiPreemptive == true)
                {
                    fileName += "Prempt_";
                }
                fileName += sch.GetType().Name + "_" + Config.JobArrivalType + "_" + Config.JobLengthType;
                using (var file = new System.IO.StreamWriter(fileName + "_ConfInt.txt"))
                {
                    file.WriteLine("%ScheduledJobNum workload ScheduledRevenue UsedGreenEnergy UsedBrownEnergy UsedBrownEnergyCost RunningTime");
                    foreach (string key in result.Keys)
                    {
                        var list = result[key];
                        {
                            var JobNumList = new List<double>();
                            list.ForEach(s => { JobNumList.Add(s.ScheduledJobNum); });
                            file.Write(Config.Tinv * JobNumList.StandardVariance() / Math.Sqrt(Config.Repetition) + " ");


                            var WorkloadList = new List<double>();
                            list.ForEach(s => { WorkloadList.Add(s.Workload); });
                            file.Write(Config.Tinv * WorkloadList.StandardVariance() / Math.Sqrt(Config.Repetition) + " ");


                            var ScheduledRevenueList = new List<double>();
                            list.ForEach(s => { ScheduledRevenueList.Add(s.ScheduledRevenue); });
                            file.Write(Config.Tinv * ScheduledRevenueList.StandardVariance() / Math.Sqrt(Config.Repetition) + " ");

                            var UsedGreenEnergyList = new List<double>();
                            list.ForEach(s => { UsedGreenEnergyList.Add(s.UsedGreenEnergy); });
                            file.Write(Config.Tinv * UsedGreenEnergyList.StandardVariance() / Math.Sqrt(Config.Repetition) + " ");

                            var UsedBrownEnergyList = new List<double>();
                            list.ForEach(s => { UsedBrownEnergyList.Add(s.UsedBrownEnergy); });
                            file.Write(Config.Tinv * UsedBrownEnergyList.StandardVariance() / Math.Sqrt(Config.Repetition) + " ");

                            var UsedBrownEnergyCostList = new List<double>();
                            list.ForEach(s => { UsedBrownEnergyCostList.Add(s.UsedBrownEnergyCost); });
                            file.Write(Config.Tinv * UsedBrownEnergyCostList.StandardVariance() / Math.Sqrt(Config.Repetition) + " ");

                            var RunningTimeList = new List<double>();
                            list.ForEach(s => { RunningTimeList.Add(s.RunTime); });
                            file.Write(Config.Tinv * RunningTimeList.StandardVariance() / Math.Sqrt(Config.Repetition) + " ");

                            file.WriteLine();
                        }
                    }
                }
            });

        }


 
       /// <summary>
       /// initialize schedule
       /// </summary>
       /// <returns></returns>
        private List<Job> InitSchedule()
        {
            Console.WriteLine("Initial Schedule...");

            //initialize workload traces and solar traces
            this.PS = ProblemSetting.InitProblemSetting(this.Config);

            Console.WriteLine("JobNum= {0}, GreenEnergySum= {1}", PS.Jobs.Count, PS.SolarEnergyList.Sum());

            //CHANGE when add new algorithms!
            //initialize scheduler
            Schedulers = new List<IScheduler>{
                new FirstFitScheduler(Config, this.PS.Clone()),
                new BestFitScheduler(Config, this.PS.Clone()),               
                new RandomFitScheduler(Config, this.PS.Clone()),
                new GreenSlotScheduler(Config, this.PS.Clone()),
                new WeightedSmallWindowScheduler(Config, this.PS.Clone()),
                //new WeightedLargeWindowScheduler(Config, this.PS.Clone()),  
                 new RandomChoiceSmallScheduler(Config, this.PS.Clone()), 
                 new RandomChoiceMediumScheduler(Config, this.PS.Clone()),
                new RandomChoiceLargeScheduler(Config, this.PS.Clone())
            };

            return this.PS.Jobs;
        }

        /// <summary>
        /// do schedule
        /// </summary>
        private void DoSchedule()
        {
            for (int t = 0; t < Config.TimeSlots; t++)
            {             
                this.Schedulers.ForEach(s =>
                {
                    var newJobs = s.ProblemSetting.Jobs.Where(j => { return j.ArrivalTime == t; }).ToList();
                    s.CurrentTime = t;
                    s.AddJobs(newJobs);
                    s.Schedule();
                });
            }

            OutPutEnergyUsage();

            var maxRev = double.MinValue;

            var results = new Dictionary<IScheduler, SimulateResult>();

            this.Schedulers.ForEach(s =>
                {
                    //printJobs(s.ProblemSetting.Jobs);
                    var simulateresult = ProcessScheduleResultOneRepetition(s);

                    maxRev = Math.Max(maxRev, simulateresult.ScheduledRevenue);

                    results[s] = simulateresult;

                    //for each repetition, add simulateresult to dictionary
                    string key = "Num_" + WorkLoadId;
                    if (Results[s.SchedulerType].ContainsKey(key))
                    {
                        Results[s.SchedulerType][key].Add(simulateresult);
                    }
                    else
                    {
                        var list = new List<SimulateResult> { simulateresult };
                        Results[s.SchedulerType][key] = list;
                    }
                });

            foreach (var kv in results)
            {
                Console.WriteLine("SchedulerType = {0}, ratio = {1}", kv.Key.SchedulerType.ToString(), kv.Value.ScheduledRevenue / maxRev);
            };
        }

        /// <summary>
        /// output energy usage
        /// </summary>
        public void OutPutEnergyUsage()
        {
            this.Schedulers.ForEach(s =>
                {
                    FileUtil.writeIntArrayToFile(Config.OUTPUTDIR + s.SchedulerType.ToString() + "_" + Config.JobArrivalType + "_" + Config.JobLengthType + "_" + Config.UsedBrownEnergyFile, s.Cluster.UsedBrownEnergyList);
                    FileUtil.writeIntArrayToFile(Config.OUTPUTDIR + s.SchedulerType.ToString() + "_" + Config.JobArrivalType + "_" + Config.JobLengthType + "_" + Config.UsedGreenEnergyFile, s.Cluster.UsedGreenEnergyList);
                }
                );
        }

        /// <summary>
        /// process scheduled result
        /// </summary>
        /// <param name="scheduler"></param>
        /// <returns></returns>
        private SimulateResult ProcessScheduleResultOneRepetition(IScheduler scheduler)
        {
            double greenEnergyUsed = 0;
           // int jobNum = 0;
            double totalRevenue = 0;
            double brownEnergyCost = 0;
            double brownEnergyUsed = 0;

            var totalJobNum = scheduler.ProblemSetting.Jobs.Count;

            var totalJobSlot = scheduler.ProblemSetting.Jobs.Sum(s => s.RequiredNodes * s.RequiredTimeSlots);

            var scheduledJobs = scheduler.ProblemSetting.Jobs.Where(j => { return j.Scheduled; }).ToList();
            totalRevenue += scheduledJobs.Sum(j => { 
                return j.RequiredTimeSlots*j.RequiredNodes*Config.RevenueRate; 
            });


            greenEnergyUsed = scheduledJobs.Sum(j => { 
                return j.UsedGreenEnergyAmount; 
            });
            brownEnergyCost = scheduledJobs.Sum(j => { 
                return j.UsedBrownEnergyCost; 
            });
            brownEnergyUsed = scheduledJobs.Sum(j => { 
                return j.UsedBrownEnergyAmount; 
            });

            totalRevenue -= brownEnergyCost;
            Console.WriteLine("------------------------------------------------------------------------------");
            if (Config.IsSemiPreemptive == true)
                Console.WriteLine("Job is preemptive...");
            else
                Console.WriteLine("Job is non-preemptive...");

            double totalComputingResouce = scheduler.Cluster.NodeNum * scheduler.ProblemSetting.TimeSlots;
            Console.WriteLine(" - ALG = {0}, TJV = {1}, Utilization = {2}", scheduler.SchedulerType.ToString(), scheduler.ProblemSetting.Jobs.Sum(j => { return j.Weight; }), scheduler.ProblemSetting.Jobs.Sum(j => { return j.RequiredTimeSlots * j.RequiredNodes; }) / totalComputingResouce);                        
            Console.WriteLine(" - TR = {0}, GE = {1}, BE = {2}, BEC = {3}", totalRevenue, greenEnergyUsed, brownEnergyUsed, brownEnergyCost);
            Console.WriteLine(" - SJN/JN = {1}/{0}, RunTime = {2}", scheduler.ProblemSetting.Jobs.Count, scheduledJobs.Count, (double)scheduler.RunningTime / totalJobNum);

            
            if (Config.SaveFigure)
            {
                var bitmap = Visualize(scheduler);

                var head = "";

                if (Config.IsSemiPreemptive)
                {
                    head = head + "Prempt_";
                }
                bitmap.Save(head+scheduler.SchedulerType.ToString() + "_" + RunId + ".png");
            }

            var droppedJobs = scheduler.ProblemSetting.Jobs.Where(j => !j.Scheduled);

            return new SimulateResult(greenEnergyUsed + brownEnergyUsed, scheduledJobs.Count, totalRevenue, greenEnergyUsed, brownEnergyUsed, brownEnergyCost, (double)scheduler.RunningTime / totalJobNum);
        }

        public static void printJobs(List<Job> jobs)
        {
            jobs.ForEach(j => { Console.WriteLine("Arrival= {0}, requiredNode= {1}, requiredTimeslot= {2}", j.ArrivalTime, j.RequiredNodes, j.RequiredTimeSlots); });
        }

        public Bitmap Visualize(IScheduler scheduler)
        {
            const int BlockSize = 32;
            const int InfoHeight = 32;
            int SolarBarHeight = BlockSize * scheduler.Cluster.NodeNum;
            var bitmap = new Bitmap(scheduler.Cluster.TimeSlots * BlockSize, scheduler.Cluster.NodeNum * BlockSize + SolarBarHeight + InfoHeight);

            var scheduledJobs = scheduler.ProblemSetting.Jobs.Where(j => { return j.Scheduled; }).ToList();

            using (var g = Graphics.FromImage(bitmap))
            {
                g.FillRectangle(Brushes.White, new Rectangle(0, 0, bitmap.Width, bitmap.Height));

                var s = scheduler.Cluster;

                for (var n = 0; n < s.NodeNum; n++)
                {
                    for (var t = 0; t < s.TimeSlots; t++)
                    {
                        var job = s[t, n];
                        if (job == null) continue;
                        var e = t + 1;
                        while (e < s.TimeSlots && s[e, n] == job) e++;

                        using (var backgrounBrush = new SolidBrush(job.Color))
                        {
                            var c = backgrounBrush.Color;
                            using (var textBrush = new SolidBrush(Color.FromArgb(255 - c.R, 255 - c.G, 255 - c.B)))
                            {
                                var rect = new Rectangle(t * BlockSize, n * BlockSize, (e - t) * BlockSize, BlockSize);                                
                                g.SmoothingMode = SmoothingMode.AntiAlias;
                                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                                g.FillRectangle(backgrounBrush, rect);
                                g.DrawRectangle(Pens.Black, rect);

                                var rectInner = rect;
                                rectInner.X += 1;
                                rectInner.Y += 1;
                                rectInner.Width -= 2;
                                rectInner.Height -= 2;
                              
                                rectInner.Y += 8;
                                rectInner.X += (3 - job.JobId.ToString().Length) * 3 + 4;
                                g.DrawString(job.JobId.ToString(), new Font("Tahoma", 8), textBrush, rectInner);
                            }
                        }

                        t = e - 1;
                    }
                }                

                for(var t=0;t<scheduler.Cluster.TimeSlots;t++)
                {
                    if (this.PS.SolarEnergyList[t] == 0) continue;
                    var barHeight = this.PS.SolarEnergyList[t] * BlockSize;
                    var rect = new Rectangle(t * BlockSize, bitmap.Height - barHeight-1, BlockSize, barHeight);
                    
                        g.FillRectangle(Brushes.DarkGreen, rect);
                        g.DrawRectangle(Pens.Black, rect);

                }

                var max = this.PS.BrownPriceList.Max()*1.1;
                var y = new List<Point>();

                for (var t = 0; t < scheduler.Cluster.TimeSlots; t++)
                {
                    if (this.PS.BrownPriceList[t] == 0) continue;
                    y.Add(new Point(t * BlockSize, (2 * scheduler.Cluster.NodeNum - (int)(this.PS.BrownPriceList[t] / max * scheduler.Cluster.NodeNum)) * BlockSize));

                 }

                Pen brownEnergyPen = new Pen(Brushes.Brown);
                brownEnergyPen.Width = 10;

                g.DrawLines(brownEnergyPen, y.ToArray());
 
                   



                string Ispre = "Preemptive";
                if (Config.IsPreemptive == false)
                {
                    Ispre = "Non-preemptive";
                }
                g.DrawString(string.Format(scheduler.SchedulerType.ToString() + " "+Ispre+" jobs: {0}/{1}" + ", revenue= {2}, usedGreenEnergy= {3}, usedBrownEnergy={4}", scheduledJobs.Count, scheduler.ProblemSetting.Jobs.Count, scheduledJobs.Sum(j => { return j.Weight - j.UsedBrownEnergyCost; }),
                            scheduledJobs.Sum(j => { return j.UsedGreenEnergyAmount; }), scheduledJobs.Sum(j => { return j.UsedBrownEnergyAmount; })), new Font("Tahoma", 8), Brushes.Black, new PointF(0, scheduler.Cluster.NodeNum *2* BlockSize + 4));
            }

            return bitmap;
        }


    }
}
