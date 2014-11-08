package rmi.calendar;

import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.ObjectInputStream;
import java.io.ObjectOutputStream;
import java.io.Serializable;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.concurrent.PriorityBlockingQueue;
import java.io.File;

/**
 * store calendar data to permanent storage
 * @author huangxin
 *
 */
public class CalendarManagerData implements Serializable{
	
	private static String FILE = "calendar.dat";
	
	//save objects
	private final PriorityBlockingQueue<Event> comingEvents;
	private final HashMap<String,CalendarObj> userEvents;
	private final HashMap<String,CalendarClientInterface> clientObj;
	public final HashMap<Event,String> log;
	
	public CalendarManagerData(PriorityBlockingQueue<Event> comingEvents,HashMap<String, CalendarObj> events,HashMap<String,CalendarClientInterface> clientObj,HashMap<Event,String> log)
	{
		this.comingEvents = comingEvents;
		this.userEvents = events;
		this.clientObj = clientObj;
		this.log = log;
	}
	
	public static void setFileName(String filename)
	{
		FILE = filename;
	}
	public PriorityBlockingQueue<Event> getComingEvents()
	{
		return comingEvents;
	}
	public HashMap<String,CalendarObj> getUserEvent()
	{
		return userEvents;
	}
	
	public HashMap<Event, String> getLog()
	{
		return log;
	}
	
	/*
	 * load data from file
	 */
	public static CalendarManagerData load()
	{
		if(new File(FILE).exists()){
			try{
				FileInputStream fin = new FileInputStream(FILE);
				ObjectInputStream ois = new ObjectInputStream(fin);
				CalendarManagerData calendarData = (CalendarManagerData) ois.readObject();
				ois.close();
				return calendarData;
			}catch(Exception e){
				System.err.println("cannot load data file");
			}
		}
		return null;
	}
	
	/*
	 * save data to a file
	 */
	public static boolean save(PriorityBlockingQueue<Event> comingEvents,HashMap<String, CalendarObj> events,HashMap<String, CalendarClientInterface> clientObj,HashMap<Event,String> log) {

		try {
			FileOutputStream fout = new FileOutputStream(FILE);
			ObjectOutputStream oos = new ObjectOutputStream(fout);
			oos.writeObject(new CalendarManagerData(comingEvents, events,clientObj,log));
			oos.close();

			return true;

		} catch (Exception e) {
			return false;
		}

	}

	public HashMap<String, CalendarClientInterface> getClientObj() {
		// TODO Auto-generated method stub
		return clientObj;
	}

}
