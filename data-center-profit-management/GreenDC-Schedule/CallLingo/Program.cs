using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CallLingo
{
    class Program
    {
        private static int pLingoEnv;
        private static int nError;
        private static int nPointersNow;
        private static double dSatus;
        private static double dObjective;


        private static void CheckError(int errorNum)
        {
            if (errorNum != Lingo.LSERR_NO_ERROR_LNG)
            {
                Console.WriteLine("Lingo error: " + errorNum);
                System.Environment.Exit(-1);
            }
        }

        private static void InitLingo()
        {
            //create lingo environement
            pLingoEnv = Lingo.LScreateEnvLng();
            if (pLingoEnv == 0)
            {
                Console.WriteLine("Unable to create Lingo environment.\n");
                return;
            }

            //create and open lingo log
            int nError = Lingo.LSopenLogFileLng(pLingoEnv, "Lingo.log");
            CheckError(nError);

            dSatus = -1.0;
            nPointersNow = -1;

            // Let Lingo know we have a callback function
            var cbd = new CallbackData();
            var cb = new Lingo.typCallback(LngCallback.MyCallback);

            nError = Lingo.LSsetCallbackSolverLng(pLingoEnv, cb, cbd);
            CheckError(nError);

            //// Pointer to the solution dSatus code
            //nError = Lingo.LSsetPointerLng(pLingoEnv, ref dSatus, ref nPointersNow);
            //CheckError(nError);

            //// Point to dObjective, where Lingo will return the objective value
            //nError = Lingo.LSsetPointerLng(pLingoEnv, ref dObjective, ref nPointersNow);
            //CheckError(nError);


            //add license
            Lingo.LScreateEnvLicenseLng("key", ref nError);
        }

  

        public static void CleanUp(){
        
            //close log file
            Lingo.LScloseLogFileLng(pLingoEnv);

            //delete environment log
            Lingo.LSdeleteEnvLng(pLingoEnv);

            Console.WriteLine("byebye Zhonghua!");
        
        }

        public static void ExecuteScript(String filename, String settingsFileName)
        {
            unsafe
            {

                var text = new StringBuilder("set echoin 1 \n");

                if (settingsFileName != null)
                {
                    using (StreamReader streamReader = new StreamReader(settingsFileName))
                    {
                        text.Append("\n" + streamReader.ReadToEnd() + "\n");
                    }
                }

                text.Append(" take " + filename + " \n go \n quit \n");

                Console.WriteLine("Running lingo with commands: \n {0}", text);
                nError = Lingo.LSexecuteScriptLng(pLingoEnv, text.ToString());
                //Lingo.LScloseLogFileLng(pLingoEnv);

                //// dSatus != (double)Lingo.LS_STATUS_GLOBAL_LNG
                //if (nError != 0)
                //{
                //    Console.WriteLine("Unable to solve!");
                //    CheckError(nError);
                //}
                //else
                //{
                //    Console.WriteLine("Profit: {0} \n", dObjective);
                //}
            }

        }


        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length <= 1)
            {
                Console.WriteLine("Usage: CallLingo.exe script [settings]");
            }

            InitLingo();

            ExecuteScript(args[0], args.Length > 1 ? args[1] : null);

           // CleanUp();


        }
    }
}
