using System;
using System.Text;
using System.Runtime.InteropServices;


//copy from http://wendang.baidu.com/view/505f764ac850ad02de8041fb.html

public class Lingo
{

    public delegate int typCallback(int pLingoEnv, int nReserved, IntPtr pUserData);

    public static int LSERR_NO_ERROR_LNG = 0;
    public static int LSERR_OUT_OF_MEMORY_LNG = 1;
    public static int LSERR_UNABLE_TO_OPEN_LOG_FILE_LNG = 2;
    public static int LSERR_INVALID_NULL_POINTER_LNG = 3;
    public static int LSERR_INVALID_INPUT_LNG = 4;
    public static int LSERR_INFO_NOT_AVAILABLE_LNG = 5;
    public static int LS_IINFO_VARIABLES_LNG = 0;
    public static int LS_IINFO_VARIABLES_INTEGER_LNG = 1;
    public static int LS_IINFO_VARIABLES_NONLINEAR_LNG = 2;
    public static int LS_IINFO_CONSTRAINTS_LNG = 3;
    public static int LS_IINFO_CONSTRAINTS_NONLINEAR_LNG = 4;
    public static int LS_IINFO_NONZEROS_LNG = 5;
    public static int LS_IINFO_NONZEROS_NONLINEAR_LNG = 6;
    public static int LS_IINFO_ITERATIONS_LNG = 7;
    public static int LS_IINFO_BRANCHES_LNG = 8;
    public static int LS_DINFO_SUMINF_LNG = 9;
    public static int LS_DINFO_OBJECTIVE_LNG = 10;
    public static int LS_DINFO_MIP_BOUND_LNG = 11;
    public static int LS_DINFO_MIP_BEST_OBJECTIVE_LNG = 12;
    public static int LS_STATUS_GLOBAL_LNG = 0;
    public static int LS_STATUS_INFEASIBLE_LNG = 1;
    public static int LS_STATUS_UNBOUNDED_LNG = 2;
    public static int LS_STATUS_UNDETERMINED_LNG = 3;
    public static int LS_STATUS_FEASIBLE_LNG = 4;
    public static int LS_STATUS_INFORUNB_LNG = 5;
    public static int LS_STATUS_LOCAL_LNG = 6;
    public static int LS_STATUS_LOCAL_INFEASIBLE_LNG = 7;
    public static int LS_STATUS_CUTOFF_LNG = 8;
    public static int LS_STATUS_NUMERIC_ERROR_LNG = 9;

    [DllImport("lingd12.dll", EntryPoint = "LSclearPointersLng")]
    public static extern int LSclearPointersLng(int pLingoEnv);

    [DllImport("lingd12.dll", EntryPoint = "LScloseLogFileLng")]
    public static extern int LScloseLogFileLng(int pLingoEnv);

    [DllImport("lingd12.dll", EntryPoint = "LScreateEnvLng")]
    public static extern int LScreateEnvLng();

    [DllImport("lingd12.dll", EntryPoint = "LSdeleteEnvLng")]
    public static extern int LSdeleteEnvLng(int pLingoEnv);

    [DllImport("lingd12.dll", EntryPoint = "LSexecuteScriptLng")]
    public static extern int LSexecuteScriptLng(int pLingoEnv,
       string pcScript);

    [DllImport("lingd12.dll", EntryPoint = "LSgetCallbackInfoLng")]
    public static extern int LSgetCallbackInfoLng(int pLingoEnv,
       int nObject, ref int pnResult);

    [DllImport("lingd12.dll", EntryPoint = "LSopenLogFileLng")]
    public static extern int LSopenLogFileLng(int pLingoEnv,
       string pcLogFile);

    [DllImport("lingd12.dll", EntryPoint = "LSsetCallbackSolverLng")]
    public static extern int LSsetCallbackSolverLng(int pLingoEnv,
       Lingo.typCallback pSolverCallbackFunction,
          [MarshalAs(UnmanagedType.AsAny)] object pMyData);

    [DllImport("lingd12.dll", EntryPoint = "LSsetPointerLng")]
    public static extern int LSsetPointerLng(int pLingoEnv,
       ref double pdPointer, ref int pnPointersNow);

    //add licence
    [DllImport("lingd12.dll", EntryPoint = "LScreateEnvLicenseLng")]
    public static extern IntPtr LScreateEnvLicenseLng(string pcLicenseKey, ref int pnError);    

   

}


[StructLayout(LayoutKind.Sequential)]
public class CallbackData
{
    public int nIterations;
    public CallbackData()
    {
        this.nIterations = 0;
    }
}


public class LngCallback
{
    public LngCallback()
    {
    }

    public static int MyCallback(int pLingoEnv, int nReserved, IntPtr pMyData)
    {

        // Lingo callback function to display the current iteration count

        CallbackData cb = new CallbackData();
        Marshal.PtrToStructure(pMyData, cb);

        int nIterations = -1, nErr;

        //回调函数
        nErr = Lingo.LSgetCallbackInfoLng(pLingoEnv,
         Lingo.LS_IINFO_ITERATIONS_LNG, ref nIterations);// LS_IINFO_VARIABLES_NONLINEAR_LNG  LS_IINFO_ITERATIONS_LNG
        if (nErr == Lingo.LSERR_NO_ERROR_LNG && nIterations != cb.nIterations)
        {
            cb.nIterations = nIterations;
            if (nIterations % 100 == 0)
            {
                Console.WriteLine("Iteration count = {0}", nIterations);
            }
        }

        Marshal.StructureToPtr(cb, pMyData, true);

        return 0;
    }
}
