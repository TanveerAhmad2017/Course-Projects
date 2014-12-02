using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibGreenDC
{
    public class Cluster
    {
        /// <summary>
        /// each location means job assign to  (time, node) 
        /// </summary>
        public Job[,] BitMap { get; private set; }
        public int TimeSlots { get; private set; }
        public int NodeNum { get; private set; }

        public int[] UsedBrownEnergyList { get; private set; }
        public int[] UsedGreenEnergyList { get; private set; }

        public Cluster(int timeSlots, int nodeNum)
        {
            this.BitMap = new Job[timeSlots,nodeNum];
            this.TimeSlots = timeSlots;
            this.NodeNum = nodeNum;
            this.UsedBrownEnergyList = new int[timeSlots];
            this.UsedGreenEnergyList = new int[timeSlots];
        }


        #region public methods

        public void IncreaseUsedBrownEnergy(int timeslot, int number)
        {
            this.UsedBrownEnergyList[timeslot] += number;
        }

        public void IncreasedUsedGreenEnergy(int timeslot, int number)
        {
            this.UsedGreenEnergyList[timeslot] += number;
        }

        
        /// <summary>
        /// return jobs at each node at each timeslot
        /// </summary>
        /// <param name="timeSlot"></param>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public Job GetJobs(int timeSlot, int nodeId)
        {
            return this.BitMap[timeSlot, nodeId];
        }

        /// <summary>
        /// return jobs at a node
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public List<Job> GetJobsByNodeId(int nodeId)
        {
            var rnt = new List<Job>();

            for (int t = 0; t < this.TimeSlots; t++)
            {
                var job = this.BitMap[t, nodeId];
                if (job == null) continue;
                rnt.Add(job);
            }
            return rnt;
        }

        public List<Job> GetJobsByNodeLists(List<int> nodeList, int t, int length)
        {
            var list = new List<Job>();

            for (var st = 0; st < length; st++)
            {
                nodeList.ForEach(n =>
                    {
                        var job = this.BitMap[t + st, n];
                        if (job == null) return;
                        list.Add(job);
                    });
            }

            return list.Distinct().ToList();
        }

        /// <summary>
        /// return jobs at a timeSlot
        /// </summary>
        /// <param name="timeSlot"></param>
        /// <returns></returns>
        public List<Job> GetJobsByTimeSlot(int timeSlot)
        {
            var rnt = new List<Job>();

            for (int t = 0; t < this.NodeNum; t++)
            {
                var job = this.BitMap[timeSlot, t];
                if (job == null) continue;
                rnt.Add(job);
            }
            return rnt.Distinct().ToList();
        }

        public void FillBitMap(Job job)
        {

            job.NodeIdList.ForEach(node =>
            {
                for (int t = 0; t < job.ProcessingTime; t++)
                {

                    this.BitMap[job.StartTime + t, node] = job;
                }
            }
        );
        }

        public void FillBitMap(Job job, int t)
        {
            job.NodeIdList.ForEach(node =>
            {
                this.BitMap[t, node] = job;
            });
        }

        public void FillBitMap(Job job, List<int> timeSlots)
        {
            timeSlots.ForEach(t =>
            {
                this.FillBitMap(job, t);
            });
        }

        public void RemoveJob(Job job)
        {
            for(var t=0;t<this.TimeSlots;t++)
            {
                for(var n=0;n<this.NodeNum;n++)
                {
                    if (this.BitMap[t, n] == job) this.BitMap[t, n] = null;
                }                

                var usedGE = job.UsedGreenEnergyByTime[t];

                this.UsedGreenEnergyList[t] -= usedGE;
                this.UsedBrownEnergyList[t] -= (job.RequiredNodes - usedGE);
            }            
        }

        public List<int> GetFitNodeList(int requiredNode, int startTime, int length)
        {
            var nodeList = new List<int>();

            for (int n = 0; n < this.NodeNum; n++)
            {
                if (this.CanFit(n, startTime, length)) nodeList.Add(n);
                if (nodeList.Count == requiredNode) break;
            }

            return nodeList;        
        }

        public int GetFirstAvaiableNodeId(int time)
        {
            for(var i=0;i<NodeNum;i++)
            {
                if (this.BitMap[time, i] == null) return i;
            }

            return -1;
        }

        public bool IsAviable(int time, List<int> nodeList)
        {
            foreach(var n in nodeList)
                if(this.BitMap[time, n] != null) return false;            
            return true;
        }

        public Job this[int timeSlot, int nodeId]
        {
            get
            {
                return this.BitMap[timeSlot, nodeId];
            }
            set
            {
                this.BitMap[timeSlot, nodeId] = value;
            }
        }

        #endregion

        #region private methods
        public bool CanFit(int node, int startTime, int length)
        {
            if (startTime + length > this.TimeSlots) return false;
            for (int t = startTime; t < startTime + length; t++)
            {
                if(this.BitMap[t,node]!=null)
                    return false;
            }
            return true;
        }

        public bool CanFit(List<int> nodeList, int startTime, int length)
        {
            foreach(var node in nodeList)
            {
                if (!this.CanFit(node, startTime, length)) return false;
            }

            return true;
        }

       
        #endregion

        public Cluster Clone()
        {
            var cluster = new Cluster(this.TimeSlots, this.NodeNum);

            this.UsedBrownEnergyList.CopyTo(cluster.UsedBrownEnergyList, 0);
            this.UsedGreenEnergyList.CopyTo(cluster.UsedGreenEnergyList, 0);

            cluster.BitMap = (Job[,])this.BitMap.Clone();

            //for (int t = 0; t < this.TimeSlots; t++)
            //{
            //    for (int n = 0; n < this.NodeNum; n++)
            //    {
            //        if (this.BitMap[t, n] != null)
            //        {
            //            cluster.BitMap[t, n] = new Job();
            //        }
            //    }
            //}


            return cluster;
        }
    }
}
