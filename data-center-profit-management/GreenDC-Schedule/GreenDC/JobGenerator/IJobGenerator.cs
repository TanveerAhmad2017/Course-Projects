using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot.JobGenerator
{
    public interface IJobGenerator
    {
        List<Job> GenerateJob(Config config);        
    }
}
