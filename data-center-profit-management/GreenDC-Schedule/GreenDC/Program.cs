using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GreenSlot.Model;
using GreenSlot.Simulations;

namespace GreenSlot
{
    class Program
    {
        /// <summary>
        /// process input argements
        /// </summary>
        /// <param name="args">use input arguments</param>
        /// <param name="start">start process index</param>
        /// <param name="config">configuration, will update according to the user input</param>
        static void ParseArgs(string[] args, int start, Config config)
        {
            var i = start;
            
            while(i<args.Length)
            {
                var arg = args[i];


                if (arg == "-p")
                {
                    config.IsSemiPreemptive = true;
                }
                else if (arg == "-opt")
                {
                    config.IsComOpt = true;
                }
                else if (arg == "-sf")
                {
                    config.SaveFigure = true;
                }
                else if (arg == "-s")
                {
                    config.StartWorkLoad = int.Parse(args[++i]);
                }
                else if (arg == "-e")
                {
                    config.EndWorkLoad = int.Parse(args[++i]);
                }
                else if (arg == "-r")
                {
                    config.Repetition = int.Parse(args[++i]);
                }
                else if (arg == "-jn")
                {
                    config.JobNum = int.Parse(args[++i]);
                }
                else if (arg == "-h")
                {
                    Console.WriteLine("Usage: shuffling.exe demo.xml [-options] where optimls include:");
                    Console.WriteLine(" -s:<value>     startJobPercentage(*10%)");
                    Console.WriteLine(" -e:<value>      endJobPercentage(*10%)");
                    Console.WriteLine(" -at:<value>     job arrival type [u,p,s,r,f]");
                    Console.WriteLine(" -jl:<value>     job procTime distribution [u,e]");
                    Console.WriteLine(" -r:<value>      repetition times");
                    Console.WriteLine(" -jn:<value>     job number");
                    Console.WriteLine(" -p      preemptive");
                    Console.WriteLine(" -opt    compared to optimal");
                    Console.WriteLine(" -sf     save figures");
                    Console.WriteLine(" -runOption [running goal], e.g. \"getSolarTrace\" ");
                    //Console.WriteLine(" -pi     study night price impact");
                    Environment.Exit(0);
                }
                else if (arg == "-at")
                {
                    var type = args[++i];
                    if (type == "u") config.JobArrivalType = JobArrivalType.Uniform;
                    if (type == "p") config.JobArrivalType = JobArrivalType.Poisson;
                    if (type == "g1") config.JobArrivalType = JobArrivalType.Grid5kOne;
                    if (type == "g2") config.JobArrivalType = JobArrivalType.Grid5kTwo;
                    if (type == "g3") config.JobArrivalType = JobArrivalType.Grid5kThree;
                    if (type == "i1") config.JobArrivalType = JobArrivalType.IntrepidOne;
                    if (type == "i2") config.JobArrivalType = JobArrivalType.IntrepidTwo;
                    if (type == "i3") config.JobArrivalType = JobArrivalType.IntrepidThree;
                    if (type == "s") config.JobArrivalType = JobArrivalType.Staggered;
                    if (type == "f") config.JobArrivalType = JobArrivalType.Fixed;

                }
                else if (arg == "-jl")
                {
                    var type = args[++i];
                    if (type.ToLower().StartsWith("u")) config.JobLengthType = JobLengthType.Uniform;
                    if (type.ToLower().StartsWith("e")) config.JobLengthType = JobLengthType.Equal;
                }
                else if (arg == "-runOption") {
                    config.runOption = args[++i];
                }

                else if (arg == "saveRealTrace") {
                    config.saveRealTrace = true;
                }
                else if (arg == "-l") {
                    config.L = double.Parse(args[++i]);
                }

                ++i;
            }
        }

        static void Main(string[] args)
        {
            //load setting from file and intialize config
            if (args.Length == 0)
            {
                Console.WriteLine("GreenSlot.exe config.xml [options]");
                return;
            }

            var configPath = args[0];            

            var config = Config.Load(configPath);

            Console.WriteLine("Config loaded from {0}", config);

            ParseArgs(args, 1, config);


            if (config.runOption == "compareAlgs")
            {
                Console.WriteLine("StartWorkLoad = {0} EndWorkLoad = {1} Repeat = {2}", config.StartWorkLoad, config.EndWorkLoad, config.Repetition);
                Console.WriteLine("JobArrivalType = {0} JobLengthType = {1}", config.JobArrivalType, config.JobLengthType);
                var simulator = new Simulator(config);
                SolarTrace.LoadSolarTrace(Config.dirSolar, 26, 5);
                simulator.Simulate();
            }
            else if (config.runOption == "getSolarTrace") {
                List<SolarEntry> solarTraces = SolarTrace.LoadSolarTrace(Config.dirSolar, 26, 5);
                SolarTrace.writeSolarTraceToFile(Config.OUTPUTDIR+"solar.txt", solarTraces);
            }

            

           // Sim_RealWorkload_Analysis.AnalyzeRealTrace(config);
          
        }

        public static void TestWorkload(Config config)
        {
            string[] month = {"May", "Jun", "Jul" };
            for (int m = 0; m < month.Length; m++)
            {
                for (int day = 1; day <= 30; day++)
                {
                    Console.WriteLine("------Mon -{0}, Day ={1}", month[m], day);
                    JobTrace.ReadEntries(Config.dirWorkload, config, month[m], day);
                }
                   
            }
        }

      



    }


  
}
