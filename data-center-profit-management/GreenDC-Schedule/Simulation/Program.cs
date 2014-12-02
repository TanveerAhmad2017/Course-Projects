using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibGreenDC.Scheduler;
using LibGreenDC;
using Simulation.util;
using Simulation;

namespace TestLibGreenDC
{
    class Program
    {
       
        static void Main(string[] args)
        {
            var ps = new ProblemSetting();
            var jobs = new List<Job>();
            
            FileUtil.ReadData(@"../../../../data/", ref ps,  ref jobs);

            
            var firstfitscheduler = (IScheduler)new FirstFitScheduler(ps);

            var simulator = new Simulator(firstfitscheduler, ps, jobs);

            var simulationresult = simulator.Simulate();


            simulationresult.ScheduledJobs.ForEach(job =>
            {
                Console.WriteLine("sechuled " + job);
            });

            Console.WriteLine("-------------------------------------------------");

            var bestfitscheduler = (IScheduler)new BestFitScheduler(ps);
            var simulator2 = new Simulator(bestfitscheduler, ps, jobs);

            var simulationresult2 = simulator2.Simulate();
            simulationresult2.ScheduledJobs.ForEach(job =>
            {
                Console.WriteLine("sechuled " + job);
            });

            

            Console.WriteLine("-------------------------------------------------");

            Console.WriteLine(simulationresult);
            Console.WriteLine(simulationresult2);
        }

       

    }
}
