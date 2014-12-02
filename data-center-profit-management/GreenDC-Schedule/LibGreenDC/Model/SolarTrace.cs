using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace LibGreenDC
{
    public class SolarEntry
    {

        public SolarEntry() { }

        /// <summary>
        /// initialize this.solar using the sum of lists
        /// </summary>
        /// <param name="lists"></param>
        public SolarEntry(List<SolarEntry> lists)
        {
            lists.ForEach(e => { this.Solar += e.Solar; });
        }

        public double Solar { get; set; }

        public override string ToString()
        {
            return Solar.ToString();
        }
    }

    /// <summary>
    /// load solar traces from file, and map it to simulation setting
    /// </summary>
    class SolarTrace
    {
        /// <summary>
        /// read solar list from file, and return the raw solar data
        /// Note: in the file, the solar in recorded in inteval of 5 minutes
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<SolarEntry> ReadEntries(string fileName)
        {
            var list = new List<SolarEntry>();

            using (var fs = new StreamReader(fileName))
            {
                fs.ReadLine();
                fs.ReadLine();
                while (!fs.EndOfStream)
                {
                    var line = fs.ReadLine();
                    var items = line.Split('\t');
                    var entry = new SolarEntry
                    {
                        Solar = double.Parse(items[11])
                    };
                    list.Add(entry);
                }
            }

            return list;
        }

        /// <summary>
        /// map the raw solar trace to create solar trace in our simulation environment
        /// i.e., the raw solar trace is recorded each 5 minutes, in our simulation, each time slot has 15 minutes
        /// thus, we sum up each continuous 3 raw solar traces to represent 1 solar trace in our simulation
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static List<SolarEntry> MapTimeSlot(string fileName)
        {
            List<SolarEntry> solarTrace = ReadEntries(fileName);

            List<SolarEntry> rnt = new List<SolarEntry>();
            for (int i = 0; i < solarTrace.Count - 2; i += 3)
            {
                //TODO: why?
                var list = new List<SolarEntry>{
                        solarTrace[i],
                        solarTrace[i+1],
                        solarTrace[i+2]
                    };
                var entry = new SolarEntry(list);
                rnt.Add(entry);
            }

            return rnt;
        }

        /// <summary>
        /// write solar trace to file, for purpose of visualizing the solar traces graph
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="se"></param>
        public static void writeSolarTraceToFile(String filename, List<SolarEntry> se)
        {
            using (var file = new System.IO.StreamWriter(filename))
            {
                se.ForEach(e =>
                {
                    if (se.IndexOf(e) != 0) file.Write(",");
                    file.Write("{0:0.00}", e.Solar);
                    // file.Write(e.Solar);
                });
            }
        }

        /// <summary>
        /// load solar traces from file, process, then write to disk
        /// use for plotting the solar traces
        /// </summary>
        /// <param name="dir">raw solar trace path</param>
        /// <param name="start">start day</param>
        /// <param name="days">number of days</param>
        /// <returns></returns>
        public static List<SolarEntry> LoadSolarTrace(string dir, int start, int days)
        {

            var mapList = new List<SolarEntry>();

            for (int i = 0; i < days; i++)
            {
                var path = dir + @"ARC-2012-05-" + (start + i) + ".txt";
                var tmpList = SolarTrace.MapTimeSlot(path);

                mapList.AddRange(tmpList);
            }
            
            return mapList;
        }

    }
}
