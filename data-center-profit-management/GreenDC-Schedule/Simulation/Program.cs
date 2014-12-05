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
           // String datadir = @"../../../../data/";
            String datadir = args[0];
            String schedulerName = args[1];
            RunOneSimulation(datadir, schedulerName);

        }


        static void RunOneSimulation(String path, string schedulerName) {

            var ps = new ProblemSetting();
            var jobs = new List<Job>();

            FileUtil.ReadData(path, ref ps, ref jobs);


            IScheduler scheduler = SchedulerFactory.GetScheduler(schedulerName, ps);


            var simulator = new Simulator(scheduler, ps, jobs);

            var simulationresult = simulator.Simulate();

            Console.WriteLine(simulationresult);
           

        }

    }
}
