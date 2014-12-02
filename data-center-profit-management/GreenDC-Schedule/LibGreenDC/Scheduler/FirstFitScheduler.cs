using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibGreenDC.Scheduler
{
    public class FirstFitScheduler : SchedulerBase, IScheduler
    {
        public FirstFitScheduler(ProblemSetting ps)
            : base(ps)
        {
            this.SchedulerType = SchedulerType.FirstFit;
        }


        /// <summary>
        /// Find the first schedulable poition of the job
        /// </summary>
        /// <param name="job"></param>
        public override void ScheduleJob(Job job)
        {            
            if (CurrentTime + job.ProcessingTime >= this.ProblemSetting.TimeSlots) return;

            for (int t = this.CurrentTime; t <= job.SlackTime; t++)
            {
                var nodeList = this.Cluster.GetFitNodeList(job.RequiredNodes, t, job.ProcessingTime);

                if (nodeList.Count < job.RequiredNodes) continue;

                var tempCost = this.ComputeJobCost(job, t);

                job.Schedulable = true;
                job.StartTime = t;
                job.NodeIdList = nodeList;
                job.SechudledBrownEnergyCost = tempCost;
                break;
            }
        }

        /// <summary>
        /// Assign the given job if the job is schedulable
        /// </summary>
        /// <param name="job">job to assign</param>        
        public override void AssignJob(Job job)
        {
            if (!job.Schedulable) return;

            job.Scheduled = true;

            UpdateResouce(job);

            this.Cluster.FillBitMap(job);
        }


        public override bool AssignJobWithPushOut(Job job)
        {
            return false;
        }

        public override bool AssignPreemptiveJob(Job job)
        {
            var assigned = false;
           

            if (job.NodeIdList.Count == 0)
            {
                // ask for 1 time slot each time
                var nodeList = this.Cluster.GetFitNodeList(job.RequiredNodes, this.CurrentTime, 1);
                if (nodeList.Count < job.RequiredNodes) return false;
                assigned = true;
                job.NodeIdList = nodeList;
            }
            else
            {
                assigned = this.Cluster.IsAviable(this.CurrentTime, job.NodeIdList);
            }

            if (assigned)
            {
                UpdateResouce(job, this.CurrentTime);
                this.Cluster.FillBitMap(job, this.CurrentTime);                
                job.AssignedTimeSlots++;
            }

            if (job.AssignedTimeSlots == job.ProcessingTime) job.Scheduled = true;

            return true;
        }

        public void SelectNodes(int t, int startNode, Job job, List<int> nodeList)
        {
            if (job.Scheduled) return;

            if (nodeList.Count == job.RequiredNodes)
            {
                var assignedTimeSlots = new List<int>();
                for (int st = t; st < job.Deadline;st++)
                {
                    if (this.Cluster.CanFit(nodeList, st, 1))
                    {
                        assignedTimeSlots.Add(st);
                    }

                    if(assignedTimeSlots.Count == job.ProcessingTime)
                    {
                        job.StartTime = assignedTimeSlots[0];
                        job.NodeIdList = nodeList;
                        job.Scheduled = true;

                        this.UpdateResouce(job, assignedTimeSlots);
                        this.Cluster.FillBitMap(job, assignedTimeSlots);
                        

                        return;
                    }
                }

                return;
            }

            for (int n = startNode; n < Cluster.NodeNum && !job.Scheduled; n++)
            {
                if (Cluster.BitMap[t, n] == null)
                {
                    nodeList.Add(n);
                    SelectNodes(t, n + 1, job, nodeList);
                    nodeList.RemoveAt(nodeList.Count - 1);
                }
            }
        }

        public override bool AssignSemiPreemptiveJob(Job job)
        {         
            for (int t = this.CurrentTime; t <= job.SlackTime && !job.Scheduled; t++)
            {            
                var list = new List<int>();

                this.SelectNodes(t, 0, job, list);

                if (job.Scheduled) return true;
            }

            return false;
        }

        
    }
         
}
