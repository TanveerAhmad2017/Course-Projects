using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace CallLingo
{
    class Program
    {
        static void Main(string[] args)
        {
        }


        private double FindSolution(String path)
        {

            var process = new Process
            {
                StartInfo =
                {
                    FileName = dir + @"bin\lingoDll\simple.exe",
                    Arguments = "greenSLA.lng setting.txt",
                    UseShellExecute = true,
                    WorkingDirectory = path,
                    WindowStyle = ProcessWindowStyle.Hidden
                    //RedirectStandardOutput = true
                }
            };

            //这里相当于传参数             
            process.Start();

            //测试同步执行 
            process.WaitForExit();

            var maxTime = DateTime.Now.AddMinutes(5);

            var finished = false;

            while (!finished)
            {
                Console.WriteLine(" - Waiting for report...");

                using (var sr = new StreamReader(path + "lingo.log"))
                {
                    while (!sr.EndOfStream)
                    {
                        var line = sr.ReadLine();

                        if (line.Contains("Building solution report"))
                        {
                            finished = true;
                            break;
                        }
                    }
                }

                if (finished) break;

                Thread.Sleep(5 * 1000);

                if (DateTime.Now > maxTime) break;
            }

            return getProfit(path);
        }

        //read profit from file
        public double getProfit(String path) {

            String[] lines = System.IO.File.ReadAllLines(path);
            return double.Parse(lines[0]);
        }
    }
}
