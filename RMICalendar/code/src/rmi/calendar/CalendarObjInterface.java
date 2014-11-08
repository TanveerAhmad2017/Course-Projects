package rmi.calendar;

import java.rmi.Remote;
import java.rmi.RemoteException;
import java.util.ArrayList;
import java.util.Date;


public interface CalendarObjInterface extends Remote{
	
	/*
	 * schedule an event in calendars of each user specified in the user-list
	 *        
	 */
	public String ScheduleEvent(String userList, Event event) throws RemoteException;

	
	/*
	 * retrieve the schedule of a user for the specifiedtime-range
	 * if user is omitted, it defaults to the owner of the calendar
	 */
	public ArrayList<Event> RetrieveEvent(String user, Date startDate, Date endDate) throws RemoteException;
	
	/*
	 * extra function
	 * for testing
	 * show eventList for user 
	 */
	public ArrayList<Event> myEventList(String user) throws RemoteException;

	/*
	 * remove event 
	 */
	String RemoveEvent(String id) throws RemoteException;
	
	/*
	 * update event
	 */
	String UpdateEvent(String id, Event e) throws RemoteException;
	
}
