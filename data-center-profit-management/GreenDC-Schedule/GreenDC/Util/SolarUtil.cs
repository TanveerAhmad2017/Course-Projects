using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GreenSlot.Util;

namespace GreenSlot.Util
{
    public class SolarUtil
    {
        public static List<int> generateSolarEnergy(int timeSlots, int scale, int clusterNodeNum, double solarLoadFactor)
        {
            var SolarEnergy = new List<int>(timeSlots);
            List<double> solarTrace = readSolarTrace(Config.SolarFile);

            solarTrace = RecaleSolarTrace(solarTrace, scale);

            //re-scale
            var max = solarTrace.Max();

            for (int i = 0; i < timeSlots; i++)
            {
                SolarEnergy.Add((int)(solarTrace[i] * clusterNodeNum / max * solarLoadFactor));
            }

            
           


            return SolarEnergy;
        }


        public static List<double> readSolarTrace(String fileName)
        {

            var list = new List<double>();

            using (var ss = new StreamReader(fileName))
            {
                while (!ss.EndOfStream)
                {
                    var items = ss.ReadLine().Split(',');
                    items.ToList().ForEach(item =>
                    {
                        list.Add(Double.Parse(item));
                    });
                }
            }

            return list;
        }

        public static List<double> RecaleSolarTrace(List<double> list, int scale)
        {
            var rntList = new List<double>();

            for (int i = 0; i < list.Count; i += scale)
            {
                var sum = 0.0;
                for (int j = 0; j < scale; j++)
                {
                    sum += list[i];
                }
                rntList.Add(sum);
            }

            return rntList;
        }

        public static List<int> GenerateFixedSolarTrace(string fileName)
        {
            List<int> rnt = new List<int>();

            using (var file = new StreamReader(fileName))
            {
                while (!file.EndOfStream)
                {
                    var line = file.ReadLine();
                    var items = line.Split(' ');

                    rnt.Add(int.Parse(items[0]));
                    
                }
            }
            return rnt;
        }


        public static void writeSolarToFile(string fileName, List<int> solarEnergyList)
        {
            using (var file = new System.IO.StreamWriter(fileName))
            {
                file.WriteLine("green1..green480~");
                solarEnergyList.ForEach(j =>
                {
                    file.WriteLine(j);
                });
            }
        }



    }
}
