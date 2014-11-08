package rmi.calendar;

import java.io.Serializable;
import java.rmi.RemoteException;
import java.rmi.server.UnicastRemoteObject;
import java.text.ParseException;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;

import util.Log;
import util.config;

public class CalendarObj extends UnicastRemoteObject implements CalendarObjInterface,Serializable{
	
	String owner;
	ArrayList<Event> eventList = new ArrayList<Event>();
	boolean locked = false;
	//constructor
	public CalendarObj(String name) throws RemoteException{
		owner = name;
	}
	
	
	
	// =======================================================================================
	// Utility function
	// =======================================================================================
	
	public Event getEventById(String id)
	{
		for(Event e:this.eventList)
		{
			if(e.getEventId().equals(id))
			{
				return e;
			}
		}
		return null;
	}
	
	public void setLock()
	{
		locked = true;
	}
	
	public void removeLock()
	{
		locked = false;
	}
	public void addEvent(Event e)
	{
		this.eventList.add(e);
		System.out.println("Event added for user-"+this.owner);
	}
	
	/*
	 * check if time is schedulable
	 */
	public boolean isAvailable(Event event)
	{
		if(locked) return false;
		//should not conflict with non-open event
		for(Event e: eventList)
		{
			
			if(!e.getAccessType().equals("open"))
			{
				if(e.intersect(event.getStartDate(), event.getEndDate()))
					return false;
			}
		}
		
		//if new event is open event
		if(event.getAccessType().equals("open")) return true;
		
		//if new event is not open event, it should be contained in open event
		for(Event e: eventList)
		{
			
			if(e.getAccessType().equals("open"))
			{
				if(e.contains(event.getStartDate(), event.getEndDate()))
					return true;
			}
		}
		return false; 
	}
	
	//get owner
	public String getName(){
		return owner;
	}
	
	// =======================================================================================
	
	// =======================================================================================
	// Remote function Implementation
	// =======================================================================================
	
	public synchronized String ScheduleEvent(String userList, Event e) throws RemoteException
	{	
		//System.out.println("Schedule event for: "+userList);
		String[] users = userList.split(",");
		
		if(config.twoPhase==true && users.length>1)
		{
			Log.onlyDisplayTwoPhase();
			return CalendarManager.getInstance().localScheduleEvent_twoPhase(userList, e);
		}else{
			
			return CalendarManager.getInstance().localScheduleEvent(userList,e);
		}
	}
	
	public synchronized String RemoveEvent(String id)
	{
		
		Event e = null;
		for(int i=0; i<eventList.size(); i++)
		{
			String eventId = eventList.get(i).getEventId();
			
			if(eventId.equals(id))
			{   
				//System.out.println("event found: input id "+id+";eventList id: "+eventId);
				e = eventList.get(i);
				break;
			}
		}
		if(e==null)
		{
			
			return "fail,event not exit!";
			
		}
		return CalendarManager.getInstance().localRemoveEvent(e);
		
	}
	
	public synchronized ArrayList<Event> RetrieveEvent(String visitedUser, Date startDate, Date endDate) throws RemoteException
	{
		try {
//			System.out.println("visitingUser "+owner);
//			System.out.println("visitedUser "+visitedUser);
			return CalendarManager.getInstance().localRetrieveEvent(visitedUser,owner,startDate,endDate);
		} catch (ParseException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		return null;
	}
	
	public ArrayList<Event> myEventList(String user) throws RemoteException
	{
		return CalendarManager.getInstance().localMyEventList(user);
	}

	@Override
	public synchronized String UpdateEvent(String id, Event newe) throws RemoteException {
		// TODO Auto-generated method stub
		this.RemoveEvent(id);		
		return this.ScheduleEvent(newe.getaccessList(), newe);		
		
	}
	
	

}
