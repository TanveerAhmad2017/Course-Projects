using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibGreenDC;

namespace Simulation.util
{
    class FileUtil
    {
        public static void ReadData(string path, ref ProblemSetting ps, ref List<Job> jobs)
        {
            // init ps            


            var timeslotString = ReadSingleParameterFromFile(path + "totaltimeslots.txt");
            ps.TimeSlots = Int16.Parse(timeslotString); // read from file
            Console.WriteLine("timeSlot=" + ps.TimeSlots);



            ps.ClusterNodeNum = Int16.Parse(ReadSingleParameterFromFile(path + "vm.txt")); // read from file
            Console.WriteLine("vm=" + ps.ClusterNodeNum);


            ps.RevenueRate = Double.Parse(ReadSingleParameterFromFile(path + "revenuerate.txt")); // read from file
            Console.WriteLine("revenue rate=" + ps.RevenueRate);



            ps.SolarEnergyList = ReadIntListFromFile(path + "solars.txt");   // read from file 
            Console.WriteLine("solar energy length" + ps.SolarEnergyList.Count);
            ps.SolarEnergyList.ForEach(e => Console.Write(e + " "));
            Console.WriteLine();


            ps.BrownPriceList = ReadDoubleListFromFile(path + "brownPrice.txt");
            Console.WriteLine("brown price length" + ps.BrownPriceList.Count);
            ps.BrownPriceList.ForEach(e => Console.Write(e + " "));
            Console.WriteLine();

            // init ps 
            jobs = ReadJobFromFile(path + "jobs.txt", ps.RevenueRate);
            Console.WriteLine("jobs");
            jobs.ForEach(j => Console.WriteLine(j.ArrivalTime + " " + j.Deadline + " " + j.ProcessingTime + " " + j.RequiredNodes));
        }

        public static String ReadSingleParameterFromFile(String path)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            return lines[0];
        }

        public static List<int> ReadIntListFromFile(String path)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            string[] items = lines[1].Split(' ');
            List<int> rntlist = new List<int>();
            foreach (string item in items)
            {
                if (!item.Equals(""))
                    rntlist.Add(Int16.Parse(item));
            }
            return rntlist;
        }

        public static List<double> ReadDoubleListFromFile(String path)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            string[] items = lines[1].Split(' ');
            List<double> rntList = new List<double>();
            foreach (string item in items)
            {
                if (!item.Equals(""))
                    rntList.Add(double.Parse(item));
            }

            return rntList;
        }


        public static List<Job> ReadJobFromFile(String path, double revenuerate)
        {
            string[] lines = System.IO.File.ReadAllLines(path);
            List<Job> rnt = new List<Job>();
            for (int i = 1; i < lines.Length; i++)
            {
                string[] items = lines[i].Split(' ');
                int arriveTime = int.Parse(items[0]);
                int deadline = int.Parse(items[1]);
                int processtime = int.Parse(items[2]);
                int vm = int.Parse(items[3]);
                //double revenuerate = double.Parse(items[4]);
                Job job = new Job(arriveTime, deadline, processtime, vm, revenuerate);
                rnt.Add(job);
            }

            return rnt;
        }
    }
}
