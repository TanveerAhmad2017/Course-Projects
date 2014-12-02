using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace LibGreenDC
{
    [Serializable]
    public class Job
    {

        

        //public static Config Config { get; set; }        

        //JobId is used for debug, and visualize
        public int JobId { get; set; }
        public int ArrivalTime { get; set; }
        public int Deadline { get; set; }
        public double Weight { get; set; }
        public double EnergyCost { get; set; }
        public int ProcessingTime { get; set; }
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

        public Dictionary<int,int> UsedGreenEnergyByTime { get; set; }
        public double UsedBrownEnergyAmount { get; set; }
        public double UsedBrownEnergyCost { get; set; }

        public double SechudledBrownEnergyCost { get; set; }

        public double RevenueRate { get; set; }

        public Color Color { get; set; }

        public int SlackTime
        {
            get
            {
                return this.Deadline - this.ProcessingTime + 1 + this.AssignedTimeSlots;
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
                return this.ProcessingTime * this.RequiredNodes*this.RevenueRate;
            }
        }

        //?
        public Job()
        {
            this.AssignedTimeSlots = 0;
            this.Scheduled = false;
            this.NodeIdList = new List<int>();
            this.AssignedTimeSlotsList = new List<int>();
            this.UsedGreenEnergyByTime = new Dictionary<int, int>();

            this.StartTime = -1;
            this.UsedGreenEnergyAmount = 0;
            this.UsedBrownEnergyAmount = 0;
            this.UsedBrownEnergyCost = 0;
            this.ScheduledCost = double.MaxValue;
        }

        public Job(int arrivalTime, int deadline, int processingTime, int nodeNum, double revenuerate)
            : this()
        {
            this.ArrivalTime = arrivalTime;
            this.Deadline = deadline;
            this.ProcessingTime = processingTime;
            this.RequiredNodes = nodeNum;
            this.RevenueRate = revenuerate;
        }


        public Job(int arrivalTime, int deadline, double weight, double energyCost, int timeCost, int nodeNum)
            : this()
        {
            this.JobId = -1;
            this.ArrivalTime = arrivalTime;
            this.Deadline = deadline;
            this.Weight = weight;
            this.EnergyCost = energyCost;
            this.ProcessingTime = timeCost;
          
            this.RequiredNodes = nodeNum;
            
           // this.Color = Color.FromArgb(RandomGenerator.GetRandomInteger(0, 255), RandomGenerator.GetRandomInteger(0, 255), RandomGenerator.GetRandomInteger(0, 255));
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
            this.UsedGreenEnergyByTime = new Dictionary<int, int>();

            this.Scheduled = false;

            //TODO
            //this.ScheduledCost
        }


        public Job DeepClone()
        {
            return new Job(this.ArrivalTime, this.Deadline, this.ProcessingTime, this.RequiredNodes, this.RevenueRate);
        }

        public override string ToString()
        {
            return string.Format("JobId = {0} AT = {1} DL = {2} PS = {3} RN = {4} Sc = {5} RV = {6} PT = {7} ST ={8}",
                this.JobId,
                this.ArrivalTime,
                this.Deadline,
                this.ProcessingTime,
                this.RequiredNodes,
                this.Scheduled,
                this.Revenue,
                this.Profit,
                this.StartTime);
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
