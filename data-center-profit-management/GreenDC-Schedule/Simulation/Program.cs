using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibGreenDC.Scheduler;
using LibGreenDC;

namespace TestLibGreenDC
{
    class Program
    {
        static void Main(string[] args)
        {

            // init ps
            var ps = new ProblemSetting();
            ps.TimeSlots = 10; // read from file
            ps.ClusterNodeNum = 20; // read from file
            ps.RevenueRate = 1.0; // read from file
            ps.BrownPriceList = new List<double>(); // read from file
            ps.SolarEnergyList = new List<int>();   // read from file            
            
            // init ps 

            var jobs = new List<Job>(); // read job from file

            var scheduler = (IScheduler)new FirstFitScheduler(ps);            

            //scheduler.sc

            for (var t = 0; t < 10; t++)
            {
                // create some jobs
                var currentJob = jobs.Where(j => j.ArrivalTime == t).ToList();

                scheduler.CurrentTime = t;
                scheduler.AddJobs(currentJob);
                scheduler.Schedule();
            }

            var sechuledJobs = scheduler.GetScheduledJobs();

        }
    }
}
