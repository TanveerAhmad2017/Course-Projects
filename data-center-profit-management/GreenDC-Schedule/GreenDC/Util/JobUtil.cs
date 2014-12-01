using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GreenSlot.Util
{
    public  class  JobUtil
    {

        /**
         * 
         * 
         */
        public static List<Job> ReadJobsFromFile(string fileName)
        {
            List<Job> rtn = new List<Job>();

            using (var file = new StreamReader(fileName))
            {
                while (!file.EndOfStream)
                {
                    var line = file.ReadLine();
                    var items = line.Split(' ');
                    if (items.Length < 3) continue;

                    //Console.WriteLine("line ={0}",line);

                    var arriveTime = int.Parse(items[0]);
                    var deadline = int.Parse(items[1]);
                    var requiredTimeSlot = int.Parse(items[2]);
                    var requiredNodes = int.Parse(items[3]);
                    var weight = requiredTimeSlot*requiredNodes;
                    var energyCost =  requiredTimeSlot*requiredNodes;
                    var job = new Job(arriveTime, deadline, weight, energyCost, requiredTimeSlot, requiredNodes);
                  
                    rtn.Add(job);
                }
            }

            return rtn;
        }



        public static void writeJobToFile(string fileName, List<Job> jobs)
        {
            using (var file = new System.IO.StreamWriter(fileName))
            {
                file.WriteLine("job1..job{0}~", jobs.Count);
                jobs.ForEach(j =>
                {
                    file.WriteLine(j.ArrivalTime + " " + j.Deadline + " " + j.RequiredTimeSlots + " " + j.RequiredNodes+ " " + j.RequiredNodes*j.RequiredTimeSlots);
                });
            }
        }


    }
}
