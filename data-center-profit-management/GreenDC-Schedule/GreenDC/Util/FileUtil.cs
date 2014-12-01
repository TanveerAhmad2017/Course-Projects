using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GreenSlot.Model;

namespace GreenSlot
{
    public static class FileUtil
    {



        //save to file
        public static void SavedToFile(this List<double> self, string fileName)
        {
            using (var file = new System.IO.StreamWriter(fileName))
            {
                self.ForEach(e => { file.WriteLine(e); });
            }
        }

        public static void SavedToFile(this List<int> self, string fileName)
        {
            using (var file = new System.IO.StreamWriter(fileName))
            {
                self.ForEach(e => { file.WriteLine(e); });
            }
        }

        public static void writeArrayToFile(string fileName, double[] ar)
        {
            using (var file = new System.IO.StreamWriter(fileName))
            {
                for (int i = 0; i < ar.Length; i++)
                {
                    file.WriteLine(ar[i]);
                }
            }
        }

        public static void writeIntArrayToFile(string fileName, int[] ar)
        {
            using (var file = new System.IO.StreamWriter(fileName))
            {
                for (int i = 0; i < ar.Length; i++)
                {
                    file.WriteLine(ar[i]);
                }
            }
        }
        

        public static List<double> GetExponentialList(double mean, int num)
        {
            var List = new List<double>();

            for (int i = 0; i < num; i++)
            {
                double R = RandomGenerator.GetRandomDouble();
                //Put it as a parameter to the inverse distribution function.
                var t = -mean * Math.Log(R);
                List.Add(t);
            }

            return List;
        }

        public static double GetExponentialNum(double mean)
        {
            double R = RandomGenerator.GetRandomDouble();
            //Put it as a parameter to the inverse distribution function.
            var t = -mean * Math.Log(R);
            return t;
        }

        public static List<double> AccumulateList(List<double> list)
        {
            for (int i = 1; i < list.Count; i++)
            {
                list[i] += list[i - 1];
            }
                return list;
        }

        public static List<int> GeneratePoissonArrTimeList(double mean, int timeSlot, int processTime)
        {
            var rntList = new List<int>();

            var list = new List<double>();

            var firstnum = GetExponentialNum(1/mean);
            list.Add(firstnum);

            //avoid having arrival time at the end of the simulation window
            int margin = 5;

            while (list[list.Count - 1] < (timeSlot - margin))
            {
                var num = GetExponentialNum(1/mean);
                list.Add(num);
                list[list.Count - 1] += list[list.Count - 2];
            }

            list.RemoveAt(list.Count-1);

            for (int i = 0; i < list.Count; i++)
            {
                rntList.Add( (int)(list[i]) );
            }

            return rntList;
        }

        public static void WriteDoubleListToFile(String fileName, List<double> list)
        {
            using (var file = new System.IO.StreamWriter(fileName))
            {
                list.ForEach(s =>
                {
                    Console.WriteLine("{0:0.0000} ", s);
                    file.Write("{0:0.0000} ", s);
                });
            }
        }

        /// <summary>
        /// get the mean of workloads in each repetition in terms of time series
        /// store the results to file
        /// </summary>
        /// <param name="jobsList"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static double[] PlotJobTraces(List<List<Job>> jobsList, Config config)
        {
            double[] traces = new double[config.TimeSlots];

            //get average
            jobsList.ForEach(jobs =>
                {
                    for (int i = 0; i < jobs.Count; i++)
                    {
                        var j = jobs[i];
                        var start = j.ArrivalTime;
                        for (int k = 0; k < j.RequiredTimeSlots; k++)
                        {
                            if (k + start < config.TimeSlots)
                            {
                                traces[k + start] += j.RequiredNodes;
                            }
                        }
                    }
                });

            
                
            //store the result to file
            string FileName = Config.WorkLoadFile;
            FileName = FileName +"_"+ config.JobArrivalType.ToString();
            FileName = FileName + "_" + config.JobLengthType.ToString();
            FileName = FileName + ".txt";


            writeArrayToFile(FileName, traces);
            
            return traces;
        }
     


        public static void SaveSimulatationResultConfIntvalToFile(List<List<SimulateResult>> SFFList, List<List<SimulateResult>> SBFList, List<List<SimulateResult>> SRFList, double nightPortion, double jobSpan, double tinv, double repetition)
        {
            int group = SFFList.Count;
            var fileName = Config.SimulationResultConfInt + (nightPortion * 100) + "night_" + jobSpan * 24 + "h.txt";

            using (var file = new System.IO.StreamWriter(fileName))
            {
                file.WriteLine("%ScheduledJobNumConfInt");
                for (int i = 0; i < group; i++)
                {
                    var FFscheduledJobNum = new List<double>();
                    var BFscheduledJobNum = new List<double>();
                    var RFscheduledJobNum = new List<double>();
                    SFFList[i].ForEach(s => { FFscheduledJobNum.Add(s.ScheduledJobNum); });
                    SBFList[i].ForEach(s => { BFscheduledJobNum.Add(s.ScheduledJobNum); });
                    SRFList[i].ForEach(s => { RFscheduledJobNum.Add(s.ScheduledJobNum); });
                    
                    file.WriteLine("{0:0.00},{1:0.00},{2:0.00}", tinv * FFscheduledJobNum.StandardVariance()/Math.Sqrt(repetition),
                       tinv  * BFscheduledJobNum.StandardVariance() / Math.Sqrt(repetition),
                       tinv  * RFscheduledJobNum.StandardVariance() / Math.Sqrt(repetition));

                }

                file.WriteLine(";");
                file.WriteLine("%WorkloadConfInt");
                for (int i = 0; i < group; i++)
                {
                    var FFworkload = new List<double>();
                    var BFworkload = new List<double>();
                    var RFworkload = new List<double>();
                    SFFList[i].ForEach(s => { FFworkload.Add(s.Workload); });
                    SBFList[i].ForEach(s => { BFworkload.Add(s.Workload); });
                    SRFList[i].ForEach(s => { RFworkload.Add(s.Workload); });

                    file.WriteLine("{0:0.00},{1:0.00},{2:0.00}", tinv * FFworkload.StandardVariance() / Math.Sqrt(repetition),
                       tinv * BFworkload.StandardVariance() / Math.Sqrt(repetition),
                       tinv  * RFworkload.StandardVariance() / Math.Sqrt(repetition));

                }

                file.WriteLine(";");
                file.WriteLine("%ScheduledRevenueConfInt");
                for (int i = 0; i < group; i++)
                {
                    var FFRevenue = new List<double>();
                    var BFRevenue = new List<double>();
                    var RFRevenue = new List<double>();
                    SFFList[i].ForEach(s => { FFRevenue.Add(s.ScheduledRevenue); });
                    SBFList[i].ForEach(s => { BFRevenue.Add(s.ScheduledRevenue); });
                    SRFList[i].ForEach(s => { RFRevenue.Add(s.ScheduledRevenue); });

                    file.WriteLine("{0:0.00},{1:0.00},{2:0.00}", tinv  * FFRevenue.StandardVariance() / Math.Sqrt(repetition),
                       tinv  * BFRevenue.StandardVariance() / Math.Sqrt(repetition),
                       tinv  * RFRevenue.StandardVariance() / Math.Sqrt(repetition));

                }
                file.WriteLine(";");

                file.WriteLine("%UsedGreenEnergyConfInt");
                for (int i = 0; i < group; i++)
                {

                    var FFUsedGreenEnergy = new List<double>();
                    var BFUsedGreenEnergy = new List<double>();
                    var RFUsedGreenEnergy = new List<double>();
                    SFFList[i].ForEach(s => { FFUsedGreenEnergy.Add(s.UsedGreenEnergy); });
                    SBFList[i].ForEach(s => { BFUsedGreenEnergy.Add(s.UsedGreenEnergy); });
                    SRFList[i].ForEach(s => { RFUsedGreenEnergy.Add(s.UsedGreenEnergy); });

                    file.WriteLine("{0:0.00},{1:0.00},{2:0.00}", tinv  * FFUsedGreenEnergy.StandardVariance() / Math.Sqrt(repetition),
                       tinv *  BFUsedGreenEnergy.StandardVariance() / Math.Sqrt(repetition),
                       tinv  * RFUsedGreenEnergy.StandardVariance() / Math.Sqrt(repetition));



                   

                }
                file.WriteLine(";");

                file.WriteLine("%UsedBrownEnergyConfInt");
                for (int i = 0; i < group; i++)
                {

                    var FFUsedGreenEnergy = new List<double>();
                    var BFUsedGreenEnergy = new List<double>();
                    var RFUsedGreenEnergy = new List<double>();
                    SFFList[i].ForEach(s => { FFUsedGreenEnergy.Add(s.UsedBrownEnergy); });
                    SBFList[i].ForEach(s => { BFUsedGreenEnergy.Add(s.UsedBrownEnergy); });
                    SRFList[i].ForEach(s => { RFUsedGreenEnergy.Add(s.UsedBrownEnergy); });

                    file.WriteLine("{0:0.00},{1:0.00},{2:0.00}", tinv  * FFUsedGreenEnergy.StandardVariance() / Math.Sqrt(repetition),
                       tinv  * BFUsedGreenEnergy.StandardVariance() / Math.Sqrt(repetition),
                       tinv  * RFUsedGreenEnergy.StandardVariance() / Math.Sqrt(repetition));
              }

                file.WriteLine(";");
                file.WriteLine("%UsedBrownEnergyCostConfInt");
                for (int i = 0; i < group; i++)
                {

                    var FFUsedGreenEnergy = new List<double>();
                    var BFUsedGreenEnergy = new List<double>();
                    var RFUsedGreenEnergy = new List<double>();
                    SFFList[i].ForEach(s => { FFUsedGreenEnergy.Add(s.UsedBrownEnergyCost); });
                    SBFList[i].ForEach(s => { BFUsedGreenEnergy.Add(s.UsedBrownEnergyCost); });
                    SRFList[i].ForEach(s => { RFUsedGreenEnergy.Add(s.UsedBrownEnergyCost); });

                    file.WriteLine("{0:0.00},{1:0.00},{2:0.00}", tinv  * FFUsedGreenEnergy.StandardVariance() / Math.Sqrt(repetition),
                       tinv  * BFUsedGreenEnergy.StandardVariance() / Math.Sqrt(repetition),
                       tinv  * RFUsedGreenEnergy.StandardVariance() / Math.Sqrt(repetition));


                  //  file.WriteLine("{0:0.00},{1:0.00},{2:0.00}", SFFList[i].Average(s => s.UsedBrownEnergyCost), SBFList[i].Average(s => s.UsedBrownEnergyCost), SRFList[i].Average(s => s.UsedBrownEnergyCost));

                }
                file.WriteLine(";");


            }


        }


    }
}
