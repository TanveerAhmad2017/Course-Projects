using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibGreenDC.Model;

namespace LibGreenDC
{
    public interface IScheduler
    {
        SchedulerType SchedulerType { get; }
        ProblemSetting ProblemSetting { get; }
        Cluster Cluster { get; }

        int CurrentTime { get; set; }

        void Schedule();

        void AddJobs(List<Job> jobs);

        double RunningTime { get; }

        List<Job> GetScheduledJobs();
    }
}
