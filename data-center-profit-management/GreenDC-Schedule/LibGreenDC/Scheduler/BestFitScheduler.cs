using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibGreenDC.Scheduler
{
    public class BestFitScheduler : SchedulerBase, IScheduler
    {

        private List<int> bestNodeList = new List<int>();         
        private int bestStartTime = -1;
        private bool schedulable = false;

        public BestFitScheduler(ProblemSetting ps)
            : base(ps)
        {
            this.SchedulerType = SchedulerType.BestFit;
        }

        public override void AssignJob(Job job)
        {
            if (!job.Schedulable) return;

            job.Scheduled = true;

            this.UpdateResouce(job);

            this.Cluster.FillBitMap(job);
        }

        /// <summary>
        /// Find the best position of given job
        /// </summary>
        /// <param name="job">job to assign</param>
        /// <returns>true if job assigned, false if job are not assignable</returns>
        public override void ScheduleJob(Job job)
        {
            this.bestStartTime = this.CurrentTime;
            double cost = double.MaxValue;
            this.bestNodeList = new List<int>(); 
            // this.schedulable = false;

            if (CurrentTime + job.ProcessingTime >= this.ProblemSetting.TimeSlots) return;
            //find best schedule
            for (int t = CurrentTime; t <= job.SlackTime; t++)
            {
                var nodeList = this.Cluster.GetFitNodeList(job.RequiredNodes, t, job.ProcessingTime);

                if (nodeList.Count < job.RequiredNodes) continue;

                var tempCost = this.ComputeJobCost(job, t);

                if (tempCost >= cost) continue;
                
                cost = tempCost;

                bestStartTime = t;
                bestNodeList = nodeList;

                job.Schedulable = true;
                job.NodeIdList = bestNodeList;
                job.StartTime = bestStartTime;
                job.SechudledBrownEnergyCost = cost;

                if (cost == 0) break;
            }

            //if (!schedulable) return;

            //this.AssignJobBest(job);
            
            //return true;
        }

        /// <summary>
        /// Assign the job, internal used by preemptive only
        /// </summary>
        /// <param name="job"></param>
        private void AssignJobBest(Job job)
        {
            job.StartTime = bestStartTime;
            job.NodeIdList = bestNodeList;
            job.Scheduled = true;

            this.UpdateResouce(job);

            this.Cluster.FillBitMap(job);          
        }

        /// <summary>
        /// used by preemptive version
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public override bool AssignJobWithPushOut(Job job)
        {            
            var maxImprovement = 0.0;
            var jobsToPushOut = new List<Job>();

            this.bestNodeList = new List<int>();
            this.bestStartTime = -1;

            if(job.JobId == 172)
            {
                job.JobId = 172;
                Console.WriteLine("Try scheduling job 172, CurrTime = {0}", this.CurrentTime);
            }

            #region Find the best tiem
            for (var t = CurrentTime; t <= job.SlackTime; ++t )
            {
               
                var nodeList = new List<int>();
                // init node list = {0, 1, 2, ... job.RequiredNodes-1}
                for (var n = 0; n < job.RequiredNodes; n++)
                {
                    nodeList.Add(n);
                }

                // select nodes, n+1, n+2, ... , n + job.RequiredNodes-1
                for (var n = 0; n < Cluster.NodeNum - job.RequiredNodes + 1; n++)
                {
                    var assignedJobs = Cluster.GetJobsByNodeLists(nodeList, t, job.ProcessingTime);

                    var assignedProfit = assignedJobs.Sum(j => j.Profit);

                    var totalJobCost = 0.0;

                    var usedGreen = 0;

                    for (var tp = 0; tp < job.ProcessingTime; ++tp)
                    {
                        var assignedGE = assignedJobs.GetUsedGreenEnergyByTimeSlot(t + tp);

                        totalJobCost += this.ComputeJobCost(job, t + tp, out usedGreen, assignedGE);
                    }

                    var jobProfit = job.Revenue - totalJobCost;

                    var improvement = jobProfit - assignedProfit;

                    if (improvement > maxImprovement)
                    {
                        maxImprovement = improvement;
                        bestNodeList = new List<int>(nodeList);
                        jobsToPushOut = assignedJobs;
                        bestStartTime = t;
                        schedulable = true;
                    }

                    // update node list                    
                    nodeList.RemoveAt(0);
                    nodeList.Add(n + job.RequiredNodes);
                }
            }
            #endregion

            if (!schedulable) {
                //remove current job
                //this.CurrentJobList.Remove(job);
                return false; 
            }           

            jobsToPushOut.ForEach(PushOutJob);

            this.AssignJobBest(job);

            return true;
        }


        public override bool AssignPreemptiveJob(Job job)
        {
            var bestStartTime = this.CurrentTime;
            var cost = double.MaxValue;
            var bestNodeList = new List<int>();
            var assigned = false;

            // machine not assigned yet
            if (job.NodeIdList.Count == 0)
            {

                //find best schedule
                for (int t = CurrentTime; t <= job.SlackTime; t++)
                {
                    var nodeList = this.Cluster.GetFitNodeList(job.RequiredNodes, t, 1);

                    if (nodeList.Count < job.RequiredNodes) continue;

                    var usedGreen = 0;

                    var tempCost = this.ComputeJobCost(job, t, out usedGreen);

                    if (tempCost > cost) continue;

                    assigned = true;
                    cost = tempCost;

                    bestStartTime = t;
                    bestNodeList = nodeList;

                    if (cost == 0) break;
                }

                if (!assigned) return false;

                job.StartTime = bestStartTime;
                job.NodeIdList = bestNodeList;

            }
            else
            {
                for (int t = CurrentTime; t <= job.SlackTime; t++)
                {
                    if (!this.Cluster.IsAviable(t, job.NodeIdList)) continue;
                    
                    var usedGreen = 0;

                    var tempCost = this.ComputeJobCost(job, t, out usedGreen);

                    if (tempCost > cost) continue;

                    assigned = true;
                    cost = tempCost;

                    bestStartTime = t;                    

                    if (cost == 0) break;
                }                
            }

            if (!assigned) return false;

            job.AssignedTimeSlots++;

            if (job.AssignedTimeSlots == job.ProcessingTime) job.Scheduled = true;

            this.Cluster.FillBitMap(job, bestStartTime);

            this.UpdateResouce(job, bestStartTime);

            return true;            
        }


        public override bool AssignSemiPreemptiveJob(Job job)
        {           
            if (job.Scheduled) throw new Exception();

            job.ScheduledCost = double.MaxValue;

            for (int time = this.CurrentTime; time <= job.SlackTime; time++)
            {
                var list = new List<int>();
                if(SelectNodesOrderByTime(time, 0, job, list))
                {
                    // found full green
                    break;
                }
            }
            
            if (job.AssignedTimeSlotsList.Count > job.ProcessingTime)
            {
                Console.WriteLine("AssignTimeSlots={0}, ProcessingTime={1}", job.AssignedTimeSlotsList.Count, job.ProcessingTime);
                throw new Exception();
            }


            if (job.ScheduledCost < double.MaxValue)
            {

                //allocate resource
                this.UpdateResouce(job, job.AssignedTimeSlotsList);

                this.Cluster.FillBitMap(job, job.AssignedTimeSlotsList);

                job.Scheduled = true;

                return true;
            }

            return false;
        }


        
        public bool SelectNodesOrderByNode(int t, int startNode, Job job, List<int> nodeList)
        {
            if (job.ScheduledCost == 0.0)
            {
                return true;
            }
            //assign nodeList to job
            //assign assignTimeSlots to job
            if (nodeList.Count == job.RequiredNodes)
            {
                for (int ot = t; ot <= job.SlackTime && job.ScheduledCost > 0; ot++)
                {
                    var assignTimeSlots = new List<int>();
                    for (int st = ot; st < job.Deadline; st++)
                    {
                        if (this.Cluster.CanFit(nodeList, st, 1))
                        {
                            assignTimeSlots.Add(st);
                        }
                        else
                        {
                            break;
                        }

                        if (assignTimeSlots.Count == job.ProcessingTime)
                        {
                            var usedGreen = 0;
                            var cost = ComputeJobCost(job, assignTimeSlots, out usedGreen);

                            if(job.JobId == 47)
                            {
                               // Console.WriteLine("ot = {0} st = {1} cost = {2}", ot, st, cost);
                            }
                            if (cost < job.ScheduledCost)
                            {
                                job.AssignedTimeSlotsList = assignTimeSlots;
                                job.NodeIdList = new List<int>(nodeList);
                                job.ScheduledCost = cost;
                            }



                            if (usedGreen >=  job.RequiredNodes * job.ProcessingTime)
                            {
                                return true;
                            }

                            break;
                        }
                    }
                }

                return false;
            }


            for (int n = startNode; n < Cluster.NodeNum; n++)
            {
                nodeList.Add(n);
                if(SelectNodesOrderByNode(t, n + 1, job, nodeList))
                {
                    return true;
                }
                nodeList.RemoveAt(nodeList.Count - 1);
            }

            return false;
        }


        /// <summary>
        /// Return true if it's all used energy are green
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="startNode"></param>
        /// <param name="job"></param>
        /// <param name="nodeList"></param>
        /// <returns></returns>
        public bool SelectNodesOrderByTime(int t, int startNode, Job job, List<int> nodeList)
        {
            if (job.ScheduledCost == 0.0)
            {
                return true;
            }
            //assign nodeList to job
            //assign assignTimeSlots to job
            if (nodeList.Count == job.RequiredNodes)
            {

                var assignTimeSlots = new List<int>();
                for (int st = t; st < job.Deadline; st++)
                {
                    if (this.Cluster.CanFit(nodeList, st, 1))
                    {
                        assignTimeSlots.Add(st);
                    }
                    else
                    {
                        continue;
                    }

                    if (assignTimeSlots.Count == job.ProcessingTime)
                    {
                        var usedGreen = 0;
                        var cost = ComputeJobCost(job, assignTimeSlots, out usedGreen);

                        if (cost < job.ScheduledCost)
                        {
                            job.AssignedTimeSlotsList = assignTimeSlots;
                            job.NodeIdList = new List<int>(nodeList);
                            job.ScheduledCost = cost;
                        }

                        if (usedGreen >= job.RequiredNodes * job.ProcessingTime)
                        {
                            return true;
                        }

                        break;
                    }
                }

                return false;
            }


            for (int n = startNode; n < Cluster.NodeNum; n++)
            {
                if (this.Cluster.CanFit(n, t, 1))
                {
                    nodeList.Add(n);
                    SelectNodesOrderByTime(t, n + 1, job, nodeList);                   
                    nodeList.RemoveAt(nodeList.Count - 1);
                }
            }

            return false;
        }








        public void updateWeight()
        {
            throw new NotImplementedException();
        }



    }

}
