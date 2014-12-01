using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace GreenSlot
{
    [Serializable]
    public class Job
    {
        public static Config Config { get; set; }        

        //JobId is used for debug, and visualize
        public int JobId { get; set; }
        public int ArrivalTime { get; set; }
        public int Deadline { get; set; }
        public double Weight { get; set; }
        public double EnergyCost { get; set; }
        public int RequiredTimeSlots { get; set; }
        public double ScheduledCost { get; set; }

        public int AssignedTimeSlots { get; set; }

        public List<int> AssignedTimeSlotsList { get; set; }

        public int RequiredNodes {get; set;}
        public bool Scheduled { get; set; }

        #region job scheduling information
        /// <summary>
        /// Whether a job is schedulable under given algorithm
        /// </summary>
        public bool Schedulable { get; set; }

        /// <summary>
        /// peek at the job profits after pre-scheduling
        /// Note: this job hasn't be assigned the real computing resources, you need to call AssignedJob to achieve it
        /// It is used to check the job profits before really execution
        /// </summary>
        public double SechudledProfit
        {
            get
            {
                return this.Revenue - this.SechudledBrownEnergyCost;
            }
        }

        #endregion

        public int StartTime { get; set; }
        public List<int> NodeIdList { get; set; }
        public double UsedGreenEnergyAmount { get; set; }

        public int[] UsedGreenEnergyByTime { get; set; }
        public double UsedBrownEnergyAmount { get; set; }
        public double UsedBrownEnergyCost { get; set; }

        public double SechudledBrownEnergyCost { get; set; }

        public Color Color { get; set; }

        public int SlackTime
        {
            get
            {
                return this.Deadline - this.RequiredTimeSlots + this.AssignedTimeSlots;
            }
        }

        public double Profit
        {
            get
            {
                return this.Revenue - this.UsedBrownEnergyCost;
            }
        }

       

        public double Revenue
        {
            get
            {
                return this.RequiredTimeSlots * this.RequiredNodes * Config.RevenueRate;
            }
        }

        //?
        public Job()
        {
            
        }

        public Job(int arrivalTime, int deadline, double weight, double energyCost, int timeCost, int nodeNum)
        {
            this.JobId = -1;
            this.ArrivalTime = arrivalTime;
            this.Deadline = deadline;
            this.Weight = weight;
            this.EnergyCost = energyCost;
            this.RequiredTimeSlots = timeCost;
            this.AssignedTimeSlots = 0;
            this.RequiredNodes = nodeNum;
            this.Scheduled = false;
            this.NodeIdList = new List<int>();
            this.AssignedTimeSlotsList = new List<int>();
            this.UsedGreenEnergyByTime = new int[Config.TimeSlots];

            this.StartTime = -1;
            this.UsedGreenEnergyAmount = 0;
            this.UsedBrownEnergyAmount = 0;
            this.UsedBrownEnergyCost = 0;
            this.ScheduledCost = double.MaxValue;
            this.Color = Color.FromArgb(RandomGenerator.GetRandomInteger(0, 255), RandomGenerator.GetRandomInteger(0, 255), RandomGenerator.GetRandomInteger(0, 255));
        }


        public static List<Job> createJob(List<Job> old) {
            var rnt = new List<Job>();
            old.ForEach(j =>
            {
                rnt.Add(new Job(j.ArrivalTime, j.Deadline, j.Weight, 0, 0, j.RequiredNodes));
            });

            return rnt;
            
        }


        //default sorting by deadline ACS
        //public int CompareTo(Job other)
        //{
        //    if (this.Deadline == other.Deadline)
        //    {
        //        if (other.Weight.CompareTo(this.Weight) == 0)
        //        {
        //            return this.JobId.CompareTo(other.JobId);
        //        }

        //        return other.Weight.CompareTo(this.Weight);
        //    }
        //    else 
        //    {
        //        return this.Deadline.CompareTo(other.Deadline);
        //    }
        //}


        public List<Job> getAvailableJob(int timeSlot)
        {
            throw new NotImplementedException();
        }

        // remove all used resources
        public void Clear()
        {
            this.AssignedTimeSlots = 0;
            this.AssignedTimeSlotsList.Clear();

            this.UsedBrownEnergyAmount = 0;
            this.UsedBrownEnergyCost = 0;

            this.UsedGreenEnergyAmount = 0;
            this.UsedGreenEnergyByTime = new int[Config.TimeSlots];

            this.Scheduled = false;

            //TODO
            //this.ScheduledCost
        }


        public override string ToString()
        {
            return string.Format("JobId = {0} AT = {1} DL = {2} TS = {3} RN = {4} Sc = {5} RV = {6} PT = {7}",
                this.JobId,
                this.ArrivalTime,
                this.Deadline,
                this.RequiredTimeSlots,
                this.RequiredNodes,
                this.Scheduled,
                this.Revenue,
                this.Profit);
        }
    }

    public class JobArrialTimeASCComparer : IComparer<Job>
    {
        public int Compare(Job x, Job y)
        {
            if (x.ArrivalTime.CompareTo(y.ArrivalTime) == 0)
            {
                return y.EnergyCost.CompareTo(x.EnergyCost);
            }
            else
            {
                return x.ArrivalTime.CompareTo(y.ArrivalTime);
            }
        }
    }

    public class JobDeadlineASCComparer : IComparer<Job>
    {

        public int Compare(Job x, Job y)
        {
            if (x.Deadline.CompareTo(y.Deadline) == 0)
            {
                return y.EnergyCost.CompareTo(x.EnergyCost);
            }
            else {
                return x.Deadline.CompareTo(y.Deadline);
            }
        }
    }


    public class WeightDescComparer : IComparer<Job>
    {

        public int Compare(Job x, Job y)
        {
            if (x.Weight.CompareTo(y.Weight) == 0)
            {
                return x.Deadline.CompareTo(y.Deadline);
            }
            else
            {
                return y.Weight.CompareTo(x.Weight);
            }
        }
    }

    public class LeastSlackTimeFirstComparer : IComparer<Job>
    {
        public int Compare(Job x, Job y)
        {
            if(x.SlackTime.CompareTo(y.SlackTime) == 0)
                return x.Deadline.CompareTo(y.Deadline);
            else
                return x.SlackTime.CompareTo(y.SlackTime);
        }
    }
    

    
      
}
