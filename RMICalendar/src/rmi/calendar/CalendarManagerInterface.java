package rmi.calendar;

import java.net.MalformedURLException;
import java.rmi.Remote;
import java.rmi.RemoteException;
import java.util.ArrayList;
import java.util.List;

import DHash.Hashing;





public interface CalendarManagerInterface extends Remote{
	
	///////////////////// Basic Calendar Operation /////////////////////////////////
	/*
	 * create a calendar object corresponding to the specified user
	 * return error: if the user already exists
	 */
	String createCalendar(String user) throws RemoteException;
	
	/*
	 * return a list of existing users with calendar objects
	 */
	String list() throws RemoteException;
	
	/*
	 * connect to the calendar of the specified user
	 */
	CalendarObjInterface ConnectCalendar(String user) throws RemoteException;
	
    void registerClient(CalendarClientInterface cci, String name) throws RemoteException;
    

    boolean hasCalendarObj(String username)throws RemoteException;
    

    ////////////////////////// Util /////////////////////////////////////////////////////
	public String  getPredecessorIP() throws RemoteException;
	
	
	public int getPredecessorId() throws RemoteException;
	

	public  void setSuccessor(String succ) throws RemoteException;
	
	public void setPredecessor(String pre) throws RemoteException;
	
	public String  getSuccessorIP() throws RemoteException;
	
	
	public int getSuccessorId() throws RemoteException;
	
	
	////////SOS and POP
	public String getSOSip()throws RemoteException;
	
	
	public int getSOSid()throws RemoteException;
	
	
	public String getPOPip()throws RemoteException;
	
	
	public int getPOPid()throws RemoteException;
	
	
	public void setSOS(String sosip)throws RemoteException;
	
	
	public void setPOP(String popip)throws RemoteException;

	

	/**
	 * @return
	 */
	int getServerId()throws RemoteException;

	/**
	 * @return
	 */
	String getServerIP()throws RemoteException;

	CalendarManagerInterface getCalendarManagerInterface(String host)throws RemoteException;
	
	/////////////////////////////// Primary and backup //////////////////////////////////
	public void copyToNewNode(CalendarManagerInterface newNode) throws RemoteException;
	
	public void addCallBacks(String user, CalendarClientInterface ci) throws RemoteException;
	
	public void addUserCalendars(String user,ArrayList<Event> el) throws RemoteException;	
	
	public void AddComingEvents(Event e) throws RemoteException;	
	
	public void addBKCallBacks(String user, CalendarClientInterface ci) throws RemoteException;
	
	public void addBKUserCalendars(String user,ArrayList<Event> el) throws RemoteException;	
	
	public void AddBKComingEvents(Event e) throws RemoteException;	

	public void BackupData() throws RemoteException;
	public void BackupToPrimary() throws RemoteException;
	public void save() throws RemoteException;
	
	public void removeBackupInOldSucc( ) throws RemoteException;
	
	//-----------remove backup event from outdate successor
	
	
	public void removeBKCallBacks(String user, CalendarClientInterface ci) throws RemoteException;
	
	public void removeBKUserCalendars(String user,ArrayList<Event> el) throws RemoteException;	
	
	public void removeBKComingEvents(Event e) throws RemoteException;	

	
	
	
	//-------load balancing
	public void LoadBalancing() throws RemoteException;
	
	////////////////////////////////// Chord /////////////////////////////////////////////////////////

	public CalendarManagerInterface find_successor(int id) throws RemoteException;
	
	public CalendarManagerInterface find_predecessor(int id) throws RemoteException;
	
	public CalendarManagerInterface closest_preceding_finger(int id) throws RemoteException;	
	
	public void update_others() throws RemoteException;

	public void update_finger_table(String s, int i)  throws RemoteException;

	public void init_finger_table(CalendarManagerInterface other) throws RemoteException;
	
	public void join(CalendarManagerInterface other) throws RemoteException;
  	
	public void notify(CalendarManagerInterface other) throws RemoteException;
	
	public void showFingerTabler() throws RemoteException;
	
	public void stabilize() throws RemoteException;
	
	public void leave() throws RemoteException;
	
	public CalendarManagerInterface assignRightServer(String user) throws RemoteException;
	public void fix_fingers() throws RemoteException;
	public void fix_fingers_byOther() throws RemoteException;
	
	//--------------failure------------------
	public boolean isAlive() throws RemoteException;
	public boolean checkPredecessor() throws RemoteException;
	public boolean checkSuccessor() throws RemoteException;
	public void handlePredecessorDown() throws RemoteException;
	public void handleSuccessorDown() throws RemoteException;
	
	///////////////////// Distribute schedule Event ////////////////////////////////////
	public boolean checkClientAvailable(String user, Event event) throws RemoteException;

	public boolean addGroupEvent(String user, Event e)throws RemoteException;
	
	
	///////////////////////// Two Phase Commit /////////////////////////////////////////////
	public boolean canCommit(String user, Event e, CalendarManagerInterface coordinator) throws RemoteException;
	public void doFirstAbort(String user, Event e) throws RemoteException;
	public boolean doCommit(String user, Event e) throws RemoteException;
}
