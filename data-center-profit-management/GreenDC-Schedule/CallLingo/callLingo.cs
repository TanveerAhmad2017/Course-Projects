//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Diagnostics;
//using System.IO;
//using System.Threading;


// Call Lingo to solver optimization problem and get result

//namespace CallLingo
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            String dir = @"C:\Users\hwang14\Documents\GitHub\Course-Projects\data-center-profit-management\GreenDC-Schedule\CallLingo";
//            String path = @"..\..\..\..\Lingo-opt\";
//            FindSolution(dir, path);
//        }


//        public static double FindSolution(String dir,String path)
//        {
           
//            var process = new Process
//            {
//                StartInfo =
//                {
//                    FileName = AppDomain.CurrentDomain.BaseDirectory + @"\lingoDll\simple.exe",
//                    Arguments = "offline-opt.lng setting.txt",
//                    UseShellExecute = true,
//                    WorkingDirectory = path,
//                    WindowStyle = ProcessWindowStyle.Hidden
//                    //RedirectStandardOutput = true
//                }
//            };

//            //这里相当于传参数             
//            process.Start();

//            //测试同步执行 
//            process.WaitForExit();

//            var maxTime = DateTime.Now.AddMinutes(5);

//            var finished = false;

//            while (!finished)
//            {
//                Console.WriteLine(" - Waiting for report...");

//                using (var sr = new StreamReader(path + "Lingo.log"))
//                {
//                    while (!sr.EndOfStream)
//                    {
//                        var line = sr.ReadLine();

//                        if (line.Contains("Building solution report"))
//                        {
//                            finished = true;
//                            break;
//                        }
//                    }
//                }

//                if (finished) break;

//                Thread.Sleep(5 * 1000);

//                if (DateTime.Now > maxTime) break;
//            }

//            return getProfit(path);
//        }

//        //read profit from file
//        public static double getProfit(String path) {

//            String[] lines = System.IO.File.ReadAllLines(path+"profit.txt");
//            return double.Parse(lines[0]);
//        }
//    }
//}
