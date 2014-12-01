using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot
{
    public enum SchedulerType
    {
        FirstFit,
        BestFit,
        RandomFit,
        LeadTimeFit,
        GreenSlot,
        DensityFit,
        WeightedSmallWindow,
        WeightedLargeWindow,
        RandomChoiceSmall,
        RandomChoiceMedium,
        RandomChoiceLarge,
        Abstract
    }
}
