using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;

using GreenSlot.Model;

namespace GreenSlot
{
    [Serializable]
    public class Config
    {

    

        #region input filePath
       
        //public const string dir = @"C:\Users\hwang000";

        //path of job file with type "FixedJob"
        public const string FixedJobFile = GENERATEDDIR + @"FixedJobTrace.txt";
        public const string FixedSolarFile = GENERATEDDIR + "FixedSolarTrace.txt";

        //path of solar trace 
        public const string dirSolar = DATADIR + @"Archive\";
        //path of workload trace -- grid5k
        public const string dirWorkload = DATADIR + @"anon_jobs_gwf\grid5000_clean_trace.log";
        public const string serializedGrid5k = DATADIR + @"anon_jobs_gwf\serializedGrid5k.txt";
        //real workload trace -- ANL-Intrepid
        public const string dirIntrepidOne = DATADIR + @"\ANL-Intrepid-2009-1\IntrepidOne.txt";
        public const string dirIntrepidTwo = DATADIR + @"\ANL-Intrepid-2009-1\IntrepidTwo.txt";
        public const string dirIntrepidThree = DATADIR + @"\ANL-Intrepid-2009-1\IntrepidThree.txt";

        //generate config file for only one time
        public const string DefaultPath = "demo.xml";
        #endregion


        #region outputFile path
        public const string OUTPUTDIR = @"..\..\Output\";
        
        public const string DATADIR = @"..\..\..\..\Data\";
        public const string GENERATEDDIR = DATADIR + @"generated-data\";
        public const string WorkLoadFile = OUTPUTDIR + @"WorkloadTrace";
        public const string UsedBrownEnergyFile = "UsedBrown.txt";
        public const string UsedGreenEnergyFile = "UsedGreen.txt";
        //output real workload, for purpose of analysis
        public const string RealJobRecord = OUTPUTDIR + "RealJobRecordForAnalysis.txt";
        //onput of confidential interval data
        public const string SimulationResultConfInt = OUTPUTDIR + "VaryWorkloadResultConfInt_";

        public static readonly string SolarFile = GENERATEDDIR + @"solar.txt";
        
        public const string SimulationResult = OUTPUTDIR + "VaryWorkloadResult_";
        #endregion

        #region LingoOutput
        public const string LINGODIR = @"..\..\..\..\Lingo_OPT\Lingo_GreenDC\";
        public static readonly string LingoSolarFile = LINGODIR + @"solars.txt";
        public static readonly string LingoJobFile = LINGODIR + @"jobs.txt";
        #endregion


        #region simulations
        public string runOption { get; set; }
        #endregion

        #region output option
        public bool outputWorkloadTrace { get; set; }
        public bool SaveFigure { get; set; }
        public bool OutputToLingo { get; set; }
        public bool saveRealTrace { get; set; }
        #endregion

        #region simulation setting
        //repetition times
        public int Repetition { get; set; }
        public int Seed { get; set; }
        public int Scale { get; set; }
       
        //workload, [startWorkload, EndWorkload] perentage
        public int StartWorkLoad { get; set; }
        public int EndWorkLoad { get; set; }
        public bool IsComOpt { get; set; } //when compare with optimal, solar energy and brown energy are fixed
        #endregion

        #region data center setting
        public int TimeSlots { get; set; }
        public int ClusterNodeNum { get; set; }
        public int SlotPerDay { get; set; }
        public int DayBegin { get; set; }
        #endregion

        #region energy setting
        public double DayPrice { get; set; }
        public double NightPrice { get; set; }
        public double SolarLoadFactor { get; set; }
        #endregion

        #region job setting

        public double JobUtilization { get; set; }
        public int JobNum { get; set; }
        public int WeightRange { get; set; }
        public int AvgProcessTime { get; set; }
        public double AvgNodeNum { get; set; }
        public bool IsPreemptive { get; set; }
        //night portion of job, used by staggered workload trace
        public double NightPortion { get; set; }
        public double RevenueRate { get; set; }
         public bool IsSortJobLSTF { get; set; }
         public bool IsSemiPreemptive { get; set; }
         public double ArrivalRate { get; set; }
         public double StaggeredJobSpan { get; set; } //day
         public bool EnablePushOut { get; set; }
         public int LeadTime { get; set; }
         public JobArrivalType JobArrivalType { get; set; }
         public JobLengthType JobLengthType { get; set; }
         public bool IsFilterLargeJob { get; set; }
         public double CurrWorkload { get; set; }

         //if true, relative deadline is proportional to the job execution time
         public bool isDeadlineBounded { get; set; }

        //parameter for relative deadline, d = r + processing time/L
         public double L { get; set; }
        #endregion

        #region confidence interval
        public double Tinv { get; set; }
        #endregion


        #region greenSlotScheduler
        public double GreenSlotPenaltyFact { get; set; }
        #endregion

        #region semiBestFitScheduler
        public double loadingFactor { get; set; }
        #endregion


       

        public Config()
        {
        }


        /// <summary>
        /// save config to file, using seralization
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            var ser = new XmlSerializer(this.GetType());
            using (var sw = new StreamWriter(path))
            {
                ser.Serialize(sw, this);
            }
        }

        public override string ToString()
        {
            return base.ToString();
            //return string.Format("Config:\n\tJobNum={0}\n\tRequiredNodes={1}\n\tPeakCost={2}\n\tNonPeakCost={3}", JobNum, RequiredNodes, PeakCost, NonPeakCost);
        }
    
        /// <summary>
        /// load config from file, to initialize config
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Config Load(string path)
        {
            var ser = new XmlSerializer(typeof(Config));
            Config config = null;
            using (var sr = new StreamReader(path))
            {
                config = (Config)ser.Deserialize(sr);
            }

            return config;
        }

        /// <summary>
        /// rescale parameters
        /// </summary>
        public void RescaleConfig()
        {

            TimeSlots /= Scale;
            JobNum /= Scale;
            ArrivalRate /= Scale;
            AvgProcessTime /= Scale;

            SlotPerDay /= Scale;
            DayPrice *= Scale;
            NightPrice *= Scale;

            RevenueRate *= Scale;
        }





    }
}
