using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GreenSlot.Model;
using GreenSlot.Util;

namespace GreenSlot
{
   
    public abstract class SchedulerBase
    {

        #region Properties
        protected Config Config { get; set; }
        public List<Job> CurrentJobList { get; set; }
        public int CurrentTime { get; set; }

        public Cluster Cluster { get; set; }
        public SchedulerType SchedulerType { get; protected set; }
        public ProblemSetting ProblemSetting {get; set;}

        public Dictionary<string, List<SimulateResult>> Results { get; protected set; }

        public double RunningTime { get; private set; }

        #endregion

        #region Fields
        protected List<Job> pendingList = new List<Job>();
        protected HiResTimer timer = new HiResTimer();
        #endregion

        public SchedulerBase(Config config, ProblemSetting ps)
        {
            
            this.Config = config; 

            CurrentJobList = new List<Job>();
            Cluster = new Cluster(config.TimeSlots, config.ClusterNodeNum);
            this.Results = new Dictionary<string, List<SimulateResult>>();
            
            this.ProblemSetting = ps.Clone();        
        }

        /// <summary>
        /// Preprocess jobs before scheduling, e.g., sort jobs 
        /// </summary>
        public virtual void ProcessJobList()
        {
            if (Config.IsSortJobLSTF)
            {
                this.CurrentJobList.Sort(new LeastSlackTimeFirstComparer());
            }

            if (this.SchedulerType == GreenSlot.SchedulerType.LeadTimeFit)
            {
                this.CurrentJobList.Sort(new WeightDescComparer());
            }
 
        }


        public void Schedule()
        {
            this.timer.ReStart();

            this.ProcessJobList();

            this.ScheduleCurrJobs();            

            this.CleanUpJobs();

            this.RunningTime += this.timer.TimeElapseInTenthsOfMilliseconds;
        }

        public void AddJobs(List<Job> jobs)
        {
            this.CurrentJobList.AddRange(jobs);
        }

        public void UpdateResouce(Job job)
        {
            for (int p = 0; p < job.RequiredTimeSlots; p++)
            {
                //int usedGreenCurrSlot = Math.Min(this.ProblemSetting.SolarEnergyList[job.StartTime + p], job.RequiredNodes);
                //int usedBrownCurrSlot = job.RequiredNodes - usedGreenCurrSlot;
                //double usedBrownCostCurrSlot = usedBrownCurrSlot * this.ProblemSetting.BrownPriceList[job.StartTime + p];
                //this.ProblemSetting.SolarEnergyList[job.StartTime + p] = this.ProblemSetting.SolarEnergyList[job.StartTime + p] - usedGreenCurrSlot;

                //job.usedBrownEnergyAmount += usedBrownCurrSlot;
                //job.usedBrownEnergyCost += usedBrownCostCurrSlot;
                //job.usedGreenEnergyAount += usedGreenCurrSlot;
                this.UpdateResouce(job, job.StartTime + p);
            }
        }

        public int GetAvailableSolarEngery(int t)
        {
            return Math.Max(0, this.ProblemSetting.SolarEnergyList[t] - this.Cluster.GetJobsByTimeSlot(t).Sum(j => j.RequiredNodes));
        }

        public int GetAvailableSolarEngery(int t, int length)
        {
            var sum = 0;
            for(var i=0; i<length; i++)
            {
                sum += GetAvailableSolarEngery(t + i);
            }

            return sum;
           
        }

        public void UpdateResouce(Job job, int t)
        {

            int usedGreenCurrSlot = Math.Min(GetAvailableSolarEngery(t), job.RequiredNodes);
            int usedBrownCurrSlot = job.RequiredNodes - usedGreenCurrSlot;
            double usedBrownCostCurrSlot = usedBrownCurrSlot * this.ProblemSetting.BrownPriceList[t];            

            job.UsedBrownEnergyAmount += usedBrownCurrSlot;
            job.UsedBrownEnergyCost += usedBrownCostCurrSlot;

            job.UsedGreenEnergyAmount += usedGreenCurrSlot;
            job.UsedGreenEnergyByTime[t] = usedGreenCurrSlot;

            this.Cluster.IncreaseUsedBrownEnergy(t, usedBrownCurrSlot);
            this.Cluster.IncreasedUsedGreenEnergy(t, usedGreenCurrSlot);
        }

        public void UpdateResouce(Job job, List<int> timeSlots)
        {
            timeSlots.ForEach(t =>
            {
                this.UpdateResouce(job, t);
            });
        }

        /// <summary>
        /// compute scheduling cost of a job
        /// </summary>
        /// <param name="job">job to be scheduled</param>
        /// <param name="startTime">time to start</param>
        /// <returns>return computed cost</returns>
        protected virtual double ComputeJobCost(Job job, int startTime)
        {
            var totalCost = 0.0;
            var usedGreen = 0;

            for (int p = 0; p < job.RequiredTimeSlots; p++)
            {
                totalCost += this.ComputeJobCost(job, startTime + p, out usedGreen);
            }

            return totalCost;        
        }

        /// <summary>
        /// compute energy cost for a job for the given timeslot
        /// </summary>
        /// <param name="job">job to be scheduled</param>
        /// <param name="timeSlot">given timeslot to schedule the job to</param>
        /// <param name="usedGreenEnergy">output used green energy on that time slot</param>
        /// <param name="additionGE">additional green energy</param>
        /// <returns>return computed cost</returns>
        protected virtual double ComputeJobCost(Job job, int timeSlot, out int usedGreenEnergy, double additionGE = 0)
        {
            var totalCost = 0.0;

            var usedGreenCurrSlot = Math.Min(GetAvailableSolarEngery(timeSlot) + additionGE, job.RequiredNodes);
            var usedBrownCurrSlot = job.RequiredNodes - usedGreenCurrSlot;
            var usedBrownCostCurrSlot = usedBrownCurrSlot * this.ProblemSetting.BrownPriceList[timeSlot];

            totalCost += usedBrownCostCurrSlot;

            usedGreenEnergy = (int)usedGreenCurrSlot;

            return totalCost;
        }

        /// <summary>
        /// compute energy cost for the given time slots (used by preemptive scheduling)
        /// </summary>
        /// <param name="job">job to be scheduled</param>
        /// <param name="assignTimeSlotsList">assigned slots to the schedule</param>
        /// <param name="usedGreenEnergy">scheduled green energy to this job</param>
        /// <returns>return computed cost</returns>
        protected virtual double ComputeJobCost(Job job, List<int> assignTimeSlotsList, out int usedGreenEnergy)
        {
            var totalGreen = 0;
            
            var totalCost = 0.0;
            assignTimeSlotsList.ForEach(ct =>
            {
                var usedGreen = 0;
                totalCost += this.ComputeJobCost(job, ct, out usedGreen);
                totalGreen += usedGreen;
            });

            usedGreenEnergy = totalGreen;

            return totalCost;
        }

        protected virtual void CleanUpJobs()
        {            
            
            //this.CurrentJobList.ForEach(j =>
            //    {
            //        if (!j.Scheduled && j.SelectTime <= CurrentTime)
            //        Console.WriteLine("Drop job ID ={0}, Arr ={1}, Deadline ={2}, Node ={3}, ProcessTime ={4} Weight={5}", j.JobId, j.ArrivalTime, j.Deadline, j.RequiredNodes, j.RequiredTimeSlots,j.Weight);
            //    });
            this.CurrentJobList.RemoveAll(job =>
            {
                return job.Scheduled || job.SlackTime <= CurrentTime || !job.Schedulable;
            });

           // if (!Config.IsStudySolarPredictImpact)
            
           //this.CurrentJobList.Clear();
           
                
            this.CurrentJobList.AddRange(this.pendingList);

            this.pendingList.Clear();
        }

        /// <summary>
        /// Find the position for a job
        /// </summary>
        /// <param name="job"></param>
        public abstract void ScheduleJob(Job job);

        /// <summary>
        /// Do the assignment
        /// </summary>
        /// <param name="job"></param>
        public abstract void AssignJob(Job job);

        public abstract Boolean AssignJobWithPushOut(Job job);

        public abstract Boolean AssignPreemptiveJob(Job job);

        public abstract Boolean AssignSemiPreemptiveJob(Job job);



        public void PushOutJob(Job job)
        {
            // add the job back to the list
            this.pendingList.Add(job);          

            // remove the job from cluster
            this.Cluster.RemoveJob(job);

            // clear the job
            job.Clear();
        }

        protected void ScheduleCurrJobs()
        {            
            this.CurrentJobList.ForEach(job =>
            {
                if(Config.IsSemiPreemptive)
                {
                    this.AssignSemiPreemptiveJob(job);
                }
                else if (Config.IsPreemptive)
                {
                    this.AssignPreemptiveJob(job);
                }
                else
                {                  
                    // two pheses
                    job.Schedulable = false;
                    job.Scheduled = false;

                    // 1. fint the position
                    this.ScheduleJob(job);

                    // 2. do the assignment
                    this.AssignJob(job);

                    // if job can not be sheduled, try to assign it with pushout

                    if (!job.Scheduled && Config.EnablePushOut)
                    {
                        this.AssignJobWithPushOut(job);
                    }
                }
            }
            );        
        }

        public void PrintCurrentJobs()
        {
            this.CurrentJobList.ForEach(j =>
                {
                    Console.WriteLine("JobId ={0}, Arr ={1}, LSTF ={2}", j.JobId, j.ArrivalTime, j.Deadline - j.ArrivalTime);
                }
                );
           
        }


        public double ScheduleJobList(List<Job> jobs, int lastWindowTime, int currentWindowTime) {

            for (int t = lastWindowTime; t < currentWindowTime; t++)
            {
                
                    var newJobs = jobs.Where(j => { return j.ArrivalTime == t; }).ToList();
                    this.CurrentTime = t;
                    this.AddJobs(newJobs);
                    this.Schedule();               
            }

            var scheduledJobs = jobs.Where(j => { return j.Scheduled; }).ToList();
            var profit = scheduledJobs.Sum(c => c.Profit);

            return profit;
        }

    }

    
}
