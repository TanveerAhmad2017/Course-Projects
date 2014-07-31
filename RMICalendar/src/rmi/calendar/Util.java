package rmi.calendar;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.Calendar;



public class Util {
	
	
	//between
	public static boolean inBetween(int candidate, int left, int right)
	{
		if(left < right)
		{
			return (candidate>=left)&&(candidate<=right);
		}
		if(left > right)
		{
			return (candidate>=left)||(candidate<=right);
		}
		else
		{
			return false;
		}
	}
	
	public static boolean exBetween(int candidate, int left, int right)
	{
		if(left<right)
		{
			return (candidate>left)&&(candidate<right);
		}else
		{
			return (candidate>left)||(candidate<right);
		}
	}
	
	public static boolean inBetweenExRight(int candidate, int left, int right)
	{
		if(left<right)
		{
			return (candidate>=left)&&(candidate<right);
		}else
		{
			return (candidate>=left)||(candidate<right);
		}
	}
    
	public static boolean inBetweenExLeft(int candidate, int left, int right)
	{
		if(left<right)
		{
			return (candidate>left)&&(candidate<=right);
		}
		if(left>right)
		{
			return (candidate>left)||(candidate<=right);
		}
		else
		{
			return true;
		}
	}
	
	
	
	


	/*
	 * parse string to create event
	 */
	public static Event parseEvent(String startDate, String endDate, String text, String accessType, String accessList) 
	{
		String[] acList = accessList.split(",");
		
		SimpleDateFormat sdf = new SimpleDateFormat("MM-dd-yyyy HH:mm:ss");
		Calendar s_time = Calendar.getInstance();
		try {
			s_time.setTime(sdf.parse(startDate));
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			//e.printStackTrace();
			System.out.println("Date error or other error!");
			return null;
		}
		Calendar e_time = Calendar.getInstance();
		try {
			e_time.setTime(sdf.parse(endDate));
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			System.out.println("Date error or other error!");
			return null;
		}
		
		return new Event(s_time.getTime(),e_time.getTime(),text,accessType,accessList);
	}
	
	/*
	 * get userInput
	 * @return user input parameters
	 */
	public static String[] getUserInput(String line, int length) 
	{
		String[] groups = line.split(": ");
		if(groups.length==2)
		{
			String[] params = groups[1].split(";");
			//System.out.println("params.length== "+params.length+" length=="+length);
			if(params.length==length)
				return params;
		}
		System.out.println("Wrong input, type \"help\" to see how to input");
		return null;		
		
	}
	
	/*
	 * print usage hint
	 */
	public static void usageStatement(){
		System.out.print("-------------------------------------\n"
				+"%%%%%%%%%%%%%%% Argument Format %%%%%%%%%%%%%%%%%%%%%%%%%%%\n"
				+"<user>: userName \n"
				+"<startDate>: MM-dd-yyyy HH:mm:ss \n"
				+"<endDate>: MM-dd-yyyy HH:mm:ss \n"
				+"<eventType>: public,group,private,open \n"
				+"<userList>: different users should be seperate by comma; if omitted, it means yourself\n"
				+"%%%%%%%%%%%%%%  Command %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% \n"
				+"connect:<user> \n"
				+"schedule: <start Date>;<endDate>;<eventName>;<eventType>;<(optional)userList> \n"
				+"retrieve: <user>;<startDate>;<endDate>"
				+"list \n"
				+"myEventList \n"
				+"delete: <eventId> \n"
				+"update: <eventId>;<start Date>;<endDate>;<eventName>;<eventType>;<(optional)userList> \n"
				+"help \n"
				+"%%%%%%%%%%%%%%  Test %%%%%%%%%%%%%%%%%%%%%%%%%%%%%%% \n"
				+"testNotify\n"
				+"--------------------------------------- \n"
				);
	}
	
	/**
     * Convert raw IP address to string.
     *
     * @param rawBytes raw IP address.
     * @return a string representation of the raw ip address.
     */
    public static String IpAddressByteToString(byte[] rawBytes) {
        int i = 4;
        String ipAddress = "";
        for (byte raw : rawBytes)
        {
            ipAddress += (raw & 0xFF);
            if (--i > 0)
            {
                ipAddress += ".";
            }
        }

        return ipAddress;
    }
	
	

}
