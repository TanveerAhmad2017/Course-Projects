using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GreenSlot
{
    public static class StatisticUtil
    {

        public static double StandardVariance(this List<double> list)
        {
            double mean = list.Average();

            double sum = list.Sum(e => { return Math.Pow(e - mean, 2);  });

            double std = Math.Sqrt(sum / (list.Count - 1));

            return std; 
        }
        
    }
}
