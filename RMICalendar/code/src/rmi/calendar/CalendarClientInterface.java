package rmi.calendar;

import java.rmi.Remote;
import java.rmi.RemoteException;

public interface CalendarClientInterface extends Remote {

	/*
	 * for server to call 
	 * to alert clients of upcoming events
	 */
	void EventAlert(Event e) throws RemoteException;
	
	public String getUser()throws RemoteException;
}
