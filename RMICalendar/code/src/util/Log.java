package util;

import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingQueue;

/**
 * used for printing message
 * @author huangxin
 *
 */

public final class Log
{
	
	public static  boolean demo = true;
	public static  boolean error = false;
	public static  boolean test = true;
	public static  boolean chordFind = false;
	public static  boolean getInterface = false;
	public static  boolean twoPhase = true;


	public static void log(String message, boolean type)
	{
		if(type)
			System.out.println(message);
	}
	
	public static void onlyDisplayTwoPhase()
	{
		demo = false;
		error = false;
		test = false;
		chordFind = false;
		getInterface = false;
		twoPhase = true;
	}
	
}

