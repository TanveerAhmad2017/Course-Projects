package rmi.calendar;

import java.io.UnsupportedEncodingException;
import java.net.Inet4Address;
import java.net.InetAddress;
import java.net.NetworkInterface;
import java.net.SocketException;
import java.net.UnknownHostException;
import java.rmi.Naming;
import java.rmi.NotBoundException;
import java.rmi.RemoteException;
import java.rmi.registry.LocateRegistry;
import java.rmi.registry.Registry;
import java.rmi.server.UnicastRemoteObject;
import java.text.ParseException;
import java.util.ArrayList;
import java.util.Date;
import java.util.Enumeration;
import java.util.HashMap;
import java.util.Random;
import java.util.concurrent.PriorityBlockingQueue;

import rmi.calendar.FingerTable.Finger;
import util.Log;
import util.Order;
import util.config;
import DHash.Hashing;


/**
 * calendar Manager
 * @author huangxin
 *
 */




public class CalendarManager extends UnicastRemoteObject implements
		CalendarManagerInterface {

	
	

	/**
	 * parameter
	 */
	//store event notification queue
	private final PriorityBlockingQueue<Event> comingEvents; 															
		
	//store user calendar object
	private  final HashMap<String, CalendarObj> userCalendars; 
																
	//store client obj, user for notifying upcoming envents															
	private final HashMap<String, CalendarClientInterface> callbacks; 
	
	//store event notification queue
	private final PriorityBlockingQueue<Event> BKcomingEvents; 															
		
	//store user calendar object
	private final HashMap<String, CalendarObj> BKuserCalendars; 
																
	//store client obj, user for notifying upcoming envents															
	private final HashMap<String, CalendarClientInterface> BKcallbacks; 
	
	//TODO 
	//store log in perment memory
	private final HashMap<Event,String> log;
	private final HashMap<String, Event> lockList;
	
	// calendar manager instance
	private static CalendarManager instance;

	// chord
	static String predecessorIP;

	static int predecessorId;

	public int serverId;
	public String serverIP;
	public FingerTable fingertable;
	
	static String POPip;
	static int POPid;
	static String SOSip;
	static int SOSid;
	
		


	/**
	 * cerate calendar manager instance
	 * @return
	 */
	public static synchronized CalendarManager getInstance() {
		if (instance == null)
			try {
				instance = new CalendarManager();
			} catch (RemoteException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}

		return instance;
	}


	/**
	 * constructor
	 * 
	 * load data from file if file is not null
	 * make the data persistent even when server is down
	 * 
	 * @throws RemoteException
	 */
	public CalendarManager() throws RemoteException {
		
		// get hashed id
		getServerIPfromLocal();
		
		// super();
		CalendarManagerData.setFileName(this.serverId+"calendar.dat");
		CalendarManagerData calendarData = CalendarManagerData.load();
		if (calendarData == null) {
			this.comingEvents = new PriorityBlockingQueue<Event>();
			this.userCalendars = new HashMap<String, CalendarObj>();
			this.callbacks = new HashMap<String, CalendarClientInterface>();
			this.log = new HashMap<Event,String>();
		} else {
			comingEvents = calendarData.getComingEvents();
			userCalendars = calendarData.getUserEvent();
			callbacks = new HashMap<String, CalendarClientInterface>();		
			this.log = calendarData.getLog();
			Log.log("user list size: "+ userCalendars.size(), Log.demo);
		}
		
		this.BKcomingEvents = new PriorityBlockingQueue<Event>();
		this.BKuserCalendars = new HashMap<String, CalendarObj>();
		this.BKcallbacks = new HashMap<String, CalendarClientInterface>();
		this.lockList = new HashMap<String, Event>();
		
		(new Thread(new CheckingThread(this))).start();
		(new Thread(new LoadBalance(this))).start();
		
	}
	
	

	/**
	 * create calendar for user alert if user alreayd exists
	 */
	public synchronized String createCalendar(String user)
			throws RemoteException {
		if (userCalendars.containsKey(user)) {
			String msg = "[createCalendar] User already exists!";
			// System.out.println(msg);
			return msg;
		} else {
			CalendarObj calObj = new CalendarObj(user);
			userCalendars.put(user, calObj);

			String msg = "[createCalendar] CalendarObj created for " + user;
			// System.out.println(msg);

			this.save();
			return calObj.getName();
		}
	}

	
	/**
	 * return a list of existing users with calendar objects
	 */
	public synchronized String list() throws RemoteException {
		String users = "";
		int i = 0;
		for (String s : userCalendars.keySet()) {
			users = users + "\n" + s;
		}
		return users;
	}

	/*
	 * connect to the calendar of the specified user
	 */
	public synchronized CalendarObjInterface ConnectCalendar(String user)
			throws RemoteException {
		if (userCalendars.containsKey(user)) {
			return (CalendarObjInterface) userCalendars.get(user);
		} else {
			return null;
		}
	}

	
	/**
	 * save data to disk
	 */
	public synchronized void save() {
		CalendarManagerData.save(comingEvents, userCalendars, callbacks,log);
		Log.log("Data saved!",Log.test);
	}

	
	/**
	 * retrieve user event
	 * 
	 * @param visitedUser
	 * @param user
	 * @param startDate
	 * @param endDate
	 * @return
	 * @throws ParseException
	 */
	public synchronized ArrayList<Event> localRetrieveEvent(String visitedUser,
			String user, Date startDate, Date endDate) throws ParseException {

		CalendarObj _owner = userCalendars.get(visitedUser);
		if(_owner==null) return null;
		ArrayList<Event> ownerEventList = _owner.eventList;
		if(ownerEventList==null) return null;
		ArrayList<Event> returnEventList = checkTargetEventList(visitedUser,
				user, startDate, endDate, ownerEventList);

		return returnEventList;
	}

	/*
	 * return inquiry event to users(permission is checked)
	 */
	public synchronized ArrayList<Event> checkTargetEventList(
			String visitedUser, String user, Date startDate, Date endDate,
			ArrayList<Event> ownerEventList) {
		if (visitedUser.equals(user))
			return ownerEventList;

		ArrayList<Event> rntEvent = new ArrayList<Event>();

		for (int i = 0; i < ownerEventList.size(); i++) {
			Event event = ownerEventList.get(i);
			// System.out.println("i="+i+" :"+event.toString());

			if (event.getStartDate().after(startDate)
					&& event.getEndDate().before(endDate)) {
				// System.out.println("time right! i="+i+" :"+event.toString());
				if (event.getAccessType().equals("public")
						|| event.getAccessType().equals("open")) {
					rntEvent.add(event);
				} else {
					if (event.containUser(user)) {
						rntEvent.add(event);
					}
				}
			}
		}

		return rntEvent;
	}


	/**
	 * the main method: instantiate and register an instance 
	 * of the RMIServer with the rmi registry
	 * @param argv
	 */
	public static void main(String argv[]) {
		try {
			String name = "CalendarServerAlias";
			System.out.println("Registring as: \"" + name + "\"");
			CalendarManager theServer = CalendarManager.getInstance();
			// new CalendarManager();
			
			//path "rmi://" + "localhost" + ":" + config.port + "/" + name
			Naming.bind("rmi://" + theServer.getServerIP() + ":" + config.port + "/" + name, theServer);
//			Registry registry = LocateRegistry.getRegistry();
//			registry.bind("//" + "localhost" + ":" + config.port + "/" + name, theServer);
			theServer.joinToChord();
			System.out.println(name + " ready...");
			
			
		} catch (Exception e) {
			System.out.println("Exception while registring: " + e.getMessage());
			e.printStackTrace();
		}
	}


	/**
	 * register client, for callback(notifying users about upcoming event)
	 */
	public synchronized void registerClient(CalendarClientInterface cci,
			String name) throws RemoteException {

		callbacks.put(name, cci);

		System.out.println("Client " + name + " registered!");
		// cci.EventAlert();
	}

	
	/**
	 * notify client about upcoming event
	 * @throws RemoteException
	 */
	public synchronized void NotifyClient() throws RemoteException {

		CalendarClientInterface cci = null;

		// delete irrelevant event
		for (Event e : this.comingEvents) {
			if (e.timeToBegin() < 0 || e.getAccessType().equals("oepn")) {
				this.comingEvents.remove(e);
			}
		}

		for (Event e : this.comingEvents) {
			// System.out.println(e.toString()+"\n" + e.timeToBegin());

			// alert event, 5min before event happen
			if (e.timeToBegin() <= 300000 && !e.getAccessType().equals("oepn")) {
				String[] user = e.getaccessList().split(",");
				for (int j = 0; j < user.length; j++) {
					cci = callbacks.get(user[j]);
					if (cci != null)
						cci.EventAlert(e);
				}
				this.comingEvents.remove(e);
			} else {
				break;
			}
		}
	}

	
	/**
	 * schedule event for users
	 * not with 2-phase commit
	 * 
	 * @param userList
	 * @param e
	 * @return
	 * @throws RemoteException
	 */
	public String localScheduleEvent(String userList, Event e) throws RemoteException {
		// TODO Auto-generated method stub

		String[] users = userList.split(",");
		users = Order.sortClient(users);
		int currentUserIndex;

		for (int i = 0; i < users.length; i++) {
			String perUser = users[i];

			
			///change to distribute
			CalendarManagerInterface cmi = this.find_successor(Hashing.hash(perUser));
			if(cmi==null)
			{
				System.out.println(perUser + " does not exist!");
				return perUser + " not exist!";
			}
			if(!cmi.checkClientAvailable(perUser, e))
			{
				return perUser + " not available!";
			}
		}

		String id = e.generateEventId();

		for (int i = 0; i < users.length; i++) {
			String perUser = users[i];
//			CalendarObj co = userCalendars.get(perUser);
//			co.addEvent(e);
			CalendarManagerInterface cmi = this.find_successor(Hashing.hash(perUser));
			if(cmi!=null)
			cmi.addGroupEvent(perUser, e);
			else
			{
				System.out.println(perUser + " is null");
				return perUser + " not exist!";
			}

		}

		if(!this.comingEvents.contains(e))
		this.comingEvents.add(e);
		this.save();
		return "success!";
	}
	
	
	
	/**
	 * schedule event for users
	 * group event: user 2-phase commit
	 * @param userList
	 * @param e
	 * @return
	 * @throws RemoteException
	 */
	public String localScheduleEvent_twoPhase(String userList, Event e) throws RemoteException {
		// TODO Auto-generated method stub
		Log.log("+++++++++++++++++++++++++++++++++++[Coordinator: Two Phase Commit start]++++++++++++++++++++", Log.twoPhase);
		//Phase 1
		String[] users = userList.split(",");
		users = Order.sortClient(users);
		int currentUserIndex;
		boolean ABORT = false;
		ArrayList<String> notifyList = new ArrayList<String>();
		int haveCommit = 0;
		
		//COMMIT-REQUEST
		Log.log("Phase 1: COMMIT-REQUEST...", Log.twoPhase);
		for (int i = 0; i < users.length; i++) {
			String perUser = users[i];
			
			
			CalendarManagerInterface cmi = this.find_successor(Hashing.hash(perUser));
			if(cmi==null)
			{
				System.out.println(perUser + " does not exist!");
				ABORT = true;
				break;
				//return perUser + " not exist!";
			}
			//check keep checking for some time
			//i.e. the timeout can be set as a larger constant, currently it is almost 0
			if(!cmi.canCommit(perUser, e, this))
			{
				ABORT = true;
				Log.log("user "+perUser +" is not available!", Log.twoPhase);
				break;
				//return perUser + " not available!";
			}
			
			notifyList.add(perUser);
		}
		String id = e.generateEventId();
		//Phase 2
		if(ABORT)
		{
			Log.log("Phase 2: GLOBAL ABORT...", Log.twoPhase);
			//write GLOBAL_ABORT to log
			log.put(e, "GLOBAL_ABORT");
			for(String user:notifyList)
			{
				CalendarManagerInterface cmi = this.find_successor(Hashing.hash(user));
				if(cmi==null)
				{
					Log.log("cannot find user"+user, Log.twoPhase);
				}else
				{
					cmi.doFirstAbort(user,null);
				}
			}
		}
		
		if(!ABORT)
		{
			Log.log("Phase 2: GLOBAL COMMIT...", Log.twoPhase);
			//write GLOBAL_COMMIT to log
			log.put(e, "GLOBAL_COMMIT");
			
			//broadcast to participants to ask them to commit
			for (int i = 0; i < users.length; i++) {
				String perUser = users[i];
				Log.log("Sleep for 5 seconds!", Log.twoPhase);
				try {
					Thread.sleep(5000);
				} catch (InterruptedException e1) {
					// TODO Auto-generated catch block
					e1.printStackTrace();
				}
				
				CalendarManagerInterface cmi = this.find_successor(Hashing.hash(perUser));
				if(cmi==null)
				{
					Log.log("cannot find user"+perUser, Log.twoPhase);
				}else
				{
					boolean result = cmi.doCommit(perUser,e);
					if(result) haveCommit++;
				}
				
			}
	
			if(!this.comingEvents.contains(e))
			this.comingEvents.add(e);
			this.save();
			//return "success!";
		}
		
		if(haveCommit == users.length)
		{
			Log.log("schedule done!", Log.twoPhase);
		}
		
		Log.log("+++++++++++++++++++++++++++++++++++[Coordinator: Two Phase Commit end]++++++++++++++++++++", Log.twoPhase);
		
		if(log.get(e).equals("GLOBAL_ABORT")) return "GLOBAL_ABORT";
		else return "GLOBAL_COMMIT";
	}
	
	
	/**
	 * 2-phase commit
	 * coordinator broadcast commit-request
	 */
	public boolean canCommit(String user, Event e,CalendarManagerInterface coordinator)
	{
		Log.log("---------[Participant: Two phase Commit start]------", Log.twoPhase);
		Log.log("Phase1: canCommit?...", Log.twoPhase);
		CalendarObj co = userCalendars.get(user);
		
		if(co==null)
		{
			Log.log("Phase1: VOTE_ABORT...", Log.twoPhase);
			//write "VOTE_ABORT" to log
			log.put(e, "VOTE_ABORT");
			System.out.println(user + " does not exist in server "+this.serverId);
			Log.log("---------[Participant: Two phase Commit end]------", Log.twoPhase);
			return false;
		}
		
		synchronized(co){
			Log.log("user "+user+" is locked", Log.twoPhase);
			if(co.isAvailable(e)){
				Log.log("Phase1: VOTE_COMMIT...", Log.twoPhase);
			
				co.setLock();
				return true;
			}
		}
		
		return false;
	}
	
	public boolean reply(boolean vote)
	{
		return vote;
	}
	
	
	/**
	 * receive GLOBAL_ABORT, conduct abort
	 */
	public void doFirstAbort(String user, Event e)
	{
		Log.log("Phase2: GLOBAL_ABORT, doAbort...", Log.twoPhase);
		log.put(e, "GLOBAL_ABORT");		
		Log.log("Rease lock on user "+user, Log.twoPhase);
		CalendarObj co = userCalendars.get(user);
		co.removeLock();
		//TODO
		//unlock clients object
		//recover from backup?
		Log.log("---------[Participant: Two phase Commit end]------", Log.twoPhase);
	}
	

	/**
	 * receive GLOBAL_COMMIT, so commit
	 */
	public boolean doCommit(String user, Event e)
	{
		Log.log("Phase2: GLOBAL_COMMIT, doCommit...", Log.twoPhase);
		log.put(e, "GLOBAL_COMMIT");
		
		//release lock
		Log.log("Rease lock on user "+user, Log.twoPhase);
		CalendarObj co = userCalendars.get(user);
		co.removeLock();
//		co.notify();
//		lockList.remove(user);
		
		
		this.addGroupEvent(user, e);
		Log.log("---------[Participant: Two phase Commit end]------", Log.twoPhase);
		return true;			
		//TODO
		//unlock object
	}
	

	/**
	 * call from participants to coordinator ask for the decison on a trasaction after it has voted Yes
	 * but has still had no reply after delay
	 * Used to recover from server crash or delayed messages
	 * @param e
	 * @return
	 */
	public String getDecision(Event e)
	{
		//decision are: GLOBAL_COMMIT, GLOBAL_ABORT
		return this.log.get(e);
	}
	

	
	/**
	 * remove event, not permission is not considered here
	 * @param e
	 * @return
	 */
	public String localRemoveEvent(Event e) {
		// TODO Auto-generated method stub
		String[] user = e.getaccessList().split(",");
		for (int i = 0; i < user.length; i++) {
			CalendarObj co = userCalendars.get(user[i]);
			if (co != null) {
				// System.out.println("Event to move: "+co.eventList);
				co.eventList.remove(e);
			}
		}
		this.comingEvents.remove(e);
		this.save();
		return "success!";
	}

	
	/**
	 * check eventList for users
	 * @param user
	 * @return
	 */
	public ArrayList<Event> localMyEventList(String user) {
		
		CalendarObj co = userCalendars.get(user);
		return co.eventList;
	}

	// @Override
	public boolean hasCalendarObj(String username) {
	
		CalendarObj co = userCalendars.get(username);
		if (co == null)
			return false;
		else
			return true;
	}



	
	/**
	 * get calendarMangerinterface, used for servers to call servers, or client to call servers
	 */
	public synchronized CalendarManagerInterface getCalendarManagerInterface(String host) {
		
		Log.log("-----------------[getCalendarManagerInterface]------------------",Log.getInterface);
		if(this.serverIP.equals(host)) return this;
		Registry registry;
		CalendarManagerInterface ci = null;
		try {
			registry = LocateRegistry.getRegistry(host, config.port);
			Log.log(this.serverId + " is getting CalendarInterface for id "+Hashing.hash(host),Log.getInterface);
			
			//System.out.println("Something about registry...");
			//System.out.println("Registrylist " + registry.list().toString());			
			
			ci = (CalendarManagerInterface)registry.lookup("CalendarServerAlias");
			Log.log("After registry lookup...", Log.getInterface);
			if(ci==null)
			{	
				Log.log("CalendarInterface"+Hashing.hash(host)+" is null",Log.getInterface);			
				return ci;
			}
			
		} catch (RemoteException e) {
		
			//e.printStackTrace();
			Log.log("RemoteException caught,Fail to get CalenterManagerInterface of node "+Hashing.hash(host), Log.demo);
			return null;
		} catch (NotBoundException e) {
			
			Log.log("NotBoundException: "+ this.serverId + " fail to lookup node "+ Hashing.hash(host),Log.getInterface);
			return null;
		}

		return ci;		
	}

	
/////////////////////////////////////////  Distribute Group Event Scheduling ////////////////////
	/**
	 * check if client is avaialbe for the scheudling event
	 */
	public boolean checkClientAvailable(String user, Event event)
	{
		CalendarObj co = userCalendars.get(user);
		if(co==null)
		{
			System.out.println(user + " does not exist in server "+this.serverId);
			return false;
		}
		if(co.isAvailable(event)) return true;
		return false;
	}
	
	public boolean addGroupEvent(String user, Event e)
	{
		CalendarObj co = userCalendars.get(user);
		if(co==null) return false;//user not exist
		co.addEvent(e);
		if(!this.comingEvents.contains(e))
		this.comingEvents.add(e);
		this.save();
		return true;
	}
	
////////////////////////////////////////// Primary Backup //////////////////////////////////////
	
	
	/**
	 * copy data to new mode
	 * used in primary-backup scheme
	 */
	public void copyToNewNode(CalendarManagerInterface newNode) throws RemoteException
	{
		Log.log("--------------------------------------[CopyToNewNode]-----------------------------------",Log.demo);
		Log.log("copy from successor node "+ this.serverId +" to new node "+ newNode.getServerId(),Log.demo);
		
		for(Event e: this.comingEvents)
		{
			Log.log("Copy comingEvents",Log.demo);
			newNode.AddComingEvents(e);
		}
		

		
		for(String user:this.userCalendars.keySet())
		{
			if(Util.inBetweenExLeft(Hashing.hash(user), newNode.getPredecessorId(), newNode.getServerId()))
			{
				Log.log("Copy userCalendars",Log.demo);
				CalendarObj co = this.userCalendars.remove(user);
				//System.out.println("Remove user success, co owner is "+co.owner);
				newNode.addUserCalendars(user, co.eventList);				
				Log.log("user " + user +" calendarOBj copy from successor node "+ this.serverId +" to new node "+ newNode.getServerId(),Log.demo);
			}
		}
		
		for(String user: this.callbacks.keySet())
		{
			if(Util.inBetweenExLeft(Hashing.hash(user), newNode.getPredecessorId(), newNode.getServerId()))
			{
				Log.log("Copy callBacks",Log.demo);
				CalendarClientInterface ci = this.callbacks.remove(user);
				newNode.addCallBacks(user, ci);				
			}
		}	
		
	}
	

	
	public void addCallBacks(String user, CalendarClientInterface ci) throws RemoteException
	{
		this.callbacks.put(user, ci);
		this.save();
	}
	
	public void addUserCalendars(String user, ArrayList<Event> el) throws RemoteException
	{
		Log.log("-----[addUserCalendars]------------",Log.demo);
		Log.log("add user "+user, Log.demo);
		//System.out.println("Do nothing");
		CalendarObj co = new CalendarObj(user);
		co.eventList = el;
		this.userCalendars.put(user, co);
		//CalendarObj myco = co;
		//System.out.println("current copy is node "+ this.serverId+", copied calenarobj owner is "+ myco.owner);
		
	}
	
	public void addBKCallBacks(String user, CalendarClientInterface ci) throws RemoteException
	{
		if(this.BKcallbacks.containsKey(user))
			this.BKcallbacks.remove(user);
		this.BKcallbacks.put(user, ci);
		this.save();
	}
	
	public void addBKUserCalendars(String user, ArrayList<Event> el) throws RemoteException
	{
		Log.log("---------[add Backup UserCalendars]------------",Log.demo);
		//System.out.println("Do nothing");
		CalendarObj co = new CalendarObj(user);
		co.eventList = el;
		if(this.BKuserCalendars.containsKey(user))
		{
			this.BKuserCalendars.remove(user);
		}
		this.BKuserCalendars.put(user, co);
		//CalendarObj myco = co;
		//System.out.println("current copy is node "+ this.serverId+", copied calenarobj owner is "+ myco.owner);
	}
	
	public void removeBKCallBacks(String user, CalendarClientInterface ci) throws RemoteException
	{
		if(this.BKcallbacks.containsKey(user))
		this.BKcallbacks.remove(user);
	}
	
	public void removeBKUserCalendars(String user, ArrayList<Event> el) throws RemoteException
	{
		Log.log("-----[removeUserCalendars]------------",Log.demo);
		//System.out.println("Do nothing");
		CalendarObj co = new CalendarObj(user);
		co.eventList = el;
		if(this.BKuserCalendars.containsKey(user))
		this.BKuserCalendars.remove(user);
		//CalendarObj myco = co;
		//System.out.println("current copy is node "+ this.serverId+", copied calenarobj owner is "+ myco.owner);
	}
	
	
	public void AddComingEvents(Event e)
	{
		if(!this.comingEvents.contains(e))
		this.comingEvents.add(e);
	}
	
	public void AddBKComingEvents(Event e)
	{
		//if not exit add
		if(!this.BKcomingEvents.contains(e))
		this.BKcomingEvents.add(e);
	}
	
	public void removeBKComingEvents(Event e)
	{
		//if not exit add
		if(this.BKcomingEvents.contains(e))
		this.BKcomingEvents.remove(e);
	}
	
	
	public void BackupData() throws RemoteException
	{
		Log.log("--------------------------------------[Backup Data]-----------------------------------",Log.demo);
		if(this.serverIP.equals(this.getSuccessorIP()))
		{
			Log.log("Successor is server itself, no need to backup",Log.test);
		}
		
		CalendarManagerInterface succ = this.getCalendarManagerInterface(this.getSuccessorIP());
		if(succ==null)
		{
			Log.log("Successor is null, no need to backup data!",Log.test);
			return;
		}
		
		Log.log(this.serverId + " is backing up data to "+ this.getSuccessorId()+"...",Log.demo);
		for(Event e: this.comingEvents)
		{
			Log.log("Backup comingEvents",Log.test);
			succ.AddBKComingEvents(e);
		}		
		
		for(String user:this.userCalendars.keySet())
		{
			
			Log.log("Backup userCalendars",Log.demo);
			CalendarObj co = this.userCalendars.get(user);
			//System.out.println("Remove user success, co owner is "+co.owner);
			succ.addBKUserCalendars(user, co.eventList);				
			Log.log("user " + user +" calendarOBj backup from successor node "+ this.serverId +" to successor "+ succ.getServerId(),Log.demo);			
		}
		
		for(String user: this.callbacks.keySet())
		{
			
			Log.log("Backup callBacks",Log.test);
			CalendarClientInterface ci = this.callbacks.get(user);
			succ.addBKCallBacks(user, ci);							
		}	
		this.save();
		succ.save();
	}
	
	//not used, LoadBalancing() function will take responsible for outdate data
	public void removeBackupInOldSucc( ) throws RemoteException
	{
		System.out.println("------------[Remove outdate data]--------------");		
		if(this==null)
		{
			System.out.println("Old Succ is null, no need to remove Backup data");
			return;
		}
		this.BKcallbacks.clear();
		this.BKcomingEvents.clear();
		this.BKuserCalendars.clear();		
	}
	

	public void LoadBalancing() throws RemoteException
	{
		Log.log("------------[LoadBalancing Remove outdate data]--------------",Log.demo);
				
		//TODO remove outdata upcoming event
		for(Event e:this.BKcomingEvents)
		{
			boolean flag = true;
			String[] users = e.getAccessType().split(",");
			for(String u : users)
			{
				if(this.find_successor(Hashing.hash(u)).getSuccessorId()==this.serverId)
				{
					flag = false;
					break;
				}
			}
			if(flag) this.BKcomingEvents.remove(e);
		} 
		
		for(String user:this.BKuserCalendars.keySet())
		{	
			CalendarManagerInterface cmi = this.find_successor(Hashing.hash(user));
			if(cmi!=null &&  cmi.getSuccessorId()!=this.getServerId())
			{
				this.BKuserCalendars.remove(user);
				Log.log("user " + user +" calendarOBj remove from outdata successor node "+ this.serverId,Log.demo);	
			}
						
					
		}
		
		for(String user: this.BKcallbacks.keySet())
		{	
			Log.log("Remove Backup callBacks",Log.test);
			CalendarManagerInterface cmi = this.find_successor(Hashing.hash(user));
			if(cmi!=null && cmi.getSuccessorId()!=this.getServerId())
			{
				this.BKcallbacks.remove(user);
				Log.log("user " + user +" callback remove from outdata successor node "+ this.serverId,Log.test);	
			}																	
		}	
		
		
		//TODO remove outdata primary
				for(Event e:this.comingEvents)
				{
					boolean flag = true;
					String[] users = e.getAccessType().split(",");
					for(String u : users)
					{
						if(this.find_successor(Hashing.hash(u)).getServerId()==this.serverId)
						{
							flag = false;
							break;
						}
					}
					if(flag) this.comingEvents.remove(e);
				} 
				
				for(String user:this.userCalendars.keySet())
				{	
					CalendarManagerInterface cmi = this.find_successor(Hashing.hash(user));
					if(cmi!=null &&  cmi.getServerId()!=this.getServerId())
					{
						this.userCalendars.remove(user);
						Log.log("user " + user +" calendarOBj remove from outdata successor node "+ this.serverId,Log.demo);	
					}
								
							
				}
				
				for(String user: this.callbacks.keySet())
				{	
					Log.log("Remove primary callBacks",Log.test);
					CalendarManagerInterface cmi = this.find_successor(Hashing.hash(user));
					if(cmi!=null && cmi.getServerId()!=this.getServerId())
					{
						this.callbacks.remove(user);
						Log.log("user " + user +" callback remove from outdata successor node "+ this.serverId,Log.test);	
					}																	
				}	
				
	}
	
////////////////////////////////////////// Failure //////////////////////////////////////////////
	/**
	 * detect whether server is alive
	 */
	public boolean isAlive() throws RemoteException
	{
		return true;
	}
	
	/**
	 * check whether predecessor is alive
	 */
	public boolean checkPredecessor() throws RemoteException
	{
		Log.log("--------------[CheckPredecessor--------------------", Log.demo);
		CalendarManagerInterface pre = null;
		try{
			 pre = this.getCalendarManagerInterface(this.getPredecessorIP());
		}catch(Exception e)
		{
			Log.log("in checkPredecessor(), get pre failed", Log.demo);
			pre = null;
		}
		
		if(pre==null)
		{
			//this.setPredecessor(null);
			Log.log(this.getServerId()+" 's predecessor "+this.getPredecessorId()+" is null!", Log.demo);
		}
		

		
		if(pre!=null && pre.isAlive()==true) 
		{
			Log.log(this.getServerId()+" 's predecessor "+this.getPredecessorId()+" is alive!", Log.demo);
		}
		Log.log("--------------[CheckPredecessor End]--------------------", Log.demo);
		return true;
	}
	
	/**
	 * handle the case when predecessor is down
	 */
	public void handlePredecessorDown() 
	{
		Log.log("---------------[HandlePredecessorDown]-----------------", Log.demo);
		try {
			this.setPredecessor(this.getPOPip());
			this.BackupToPrimary();
			Log.log("current predecessor" + this.getPredecessorId(), Log.test);
			CalendarManagerInterface pre =this.getCalendarManagerInterface(this.getPredecessorIP());
			Log.log("After get predecessorInterface", Log.test);
			this.setPOP(pre.getPredecessorIP());
			
			//pre set new successor
			if(pre==null)
			{
				Log.log("new Predecessor"+ this.getPredecessorId()+" is null", Log.test);
			}else
			{
				if(pre.getSuccessorId()!=this.serverId)
				{
					//in set successor, it will backup data
					pre.setSuccessor(this.serverIP);
				}
			}
			
			//TODO
			//current id need to bring the backup data to primary
			//current server need to backup the data to successor
			this.BackupData();
			
		} catch (RemoteException e) {
			// TODO Auto-generated catch block
			//e.printStackTrace();
			Log.log("Handle predecessor down", Log.demo);
		}
	}
	
	/**
	 * handle the case when successor is down
	 */
	public void handleSuccessorDown() 
	{
		Log.log("-------------------[HandleSuccessorDown]---------------", Log.demo);
		try {
			
			//must let successor set predecessor first, so it can bring data from backup to primary bofore this server backup data to it
			//successor set predecessor
			String newSuccIP = this.getSOSip();
			CalendarManagerInterface succ = this.getCalendarManagerInterface(newSuccIP);
			if(succ==null)
			{
				Log.log("successor and SOS fail at the same time, cannot handle!",Log.demo);
			}
			if(succ.getPredecessorId()!=this.serverId)
			{
				succ.setPredecessor(this.serverIP);
				
			}
			succ.BackupToPrimary();
			
			//set new successor id
			this.setSuccessor(newSuccIP);	
		
			if(succ!=null)
			{
				this.setSOS(succ.getSuccessorIP());
			}else
			{
				Log.log("successor "+this.getSuccessorId()+" is null", Log.test);
			}
			
			CalendarManagerInterface pre = this.getCalendarManagerInterface(this.getPredecessorIP());
			if(pre!=null)
			{
				if(pre.getSOSid()!=this.getSuccessorId())
				pre.setSOS(this.getSuccessorIP());
			}else
			{
				Log.log("predecessor "+this.getPredecessorId()+" is null", Log.test);
			}
			
			if(succ!=null)
				succ.BackupData();
			else
				Log.log("successor is null", Log.test);
			
		} catch (RemoteException e) {
			// TODO Auto-generated catch block
			//e.printStackTrace();
			Log.log("handle succesor down", Log.demo);
		}
	}
	
	
	/**
	 * bring data from backup to primary when primary is down
	 */
	public void BackupToPrimary()
	{
		Log.log("---------------"+this.serverId+" [Bring backup to Primary]", Log.demo);
		for(Event e:this.BKcomingEvents)
		{
			if(!this.comingEvents.contains(e))
			this.comingEvents.add(e);			
		}
		this.BKcomingEvents.clear();
		
		for(String user: this.BKuserCalendars.keySet())
		{
			//primary is more accurate than backup
			if(!this.userCalendars.containsKey(user))
			{
				CalendarObj co = this.BKuserCalendars.get(user);
				this.userCalendars.put(user, co);
			}
		}
		this.BKuserCalendars.clear();
		
		for(String user: this.BKcallbacks.keySet())
		{
			if(!this.callbacks.containsKey(user))
			{
				CalendarClientInterface cci = this.BKcallbacks.get(user);
				this.callbacks.put(user, cci);
			}
		}
		this.BKcallbacks.clear();
	}
	
	/**
	 * check whether successor is alive
	 */
	public boolean checkSuccessor() throws RemoteException
	{
		CalendarManagerInterface succ = this.getCalendarManagerInterface(this.getSuccessorIP());
		if(succ==null)
		{
			this.setSuccessor(null);
			return false;
		}
		

		
		if(succ!=null && succ.isAlive()==true) return true;
		else return false;
	}
	
////////////////////////////////////////// CHORD  ////////////////////////////////////////////////
	
	/**
	 * node join to chord
	 * @throws RemoteException
	 */
	public void joinToChord() throws RemoteException
	{
		this.InitChordNode();
		
		CalendarManagerInterface other;
		if(this.serverIP.equals(config.joinServer))
		{
			
			Log.log("This is the first node in chord", Log.demo);
			other = null;
		}else
		{
			other = this.getCalendarManagerInterface(config.joinServer);
			Log.log("Join using arbitrary node"+config.joinServer+", new serverId= " + other.getServerId(), Log.demo);			
		}
		
		this.join(other);
		Log.log("Finger table after join...", Log.demo);
		this.showFingerTabler();
	}
	
	/**
	 * initi chord node
	 * finger table, predecessor, successor
	 */
	public void InitChordNode()
	{
		
		//serverId = Hashtool.getServerId(serverIP, port, keySize);
        // this.serverId = Hashtool.keyOfName(this.serverIP);
		this.serverId = Hashing.hash(this.serverIP);
		this.predecessorId = config.initPredecessor;
		
		fingertable = new FingerTable();		
		fingertable.initFingerTable(this.serverId);
		fingertable.show();		
	}
	
	/**
	 * stabilize chord
	 */
	public void stabilize() throws RemoteException 
	{
		Log.log("------------------------[stabilize]-----------------------------",Log.demo);
		
		CalendarManagerInterface succ = this.getCalendarManagerInterface(this.getSuccessorIP());
		if(succ==null )
		{
			Log.log("successor is null, return",Log.demo);
			return;
		}
		
		Log.log("After test whether succ is null", Log.getInterface);
		int x = succ.getPredecessorId();
		
		
	    String xip = succ.getPredecessorIP();
		
	    
	    if(xip==null) {
	    	try {
				Log.log(succ.getServerId() + "'s predecessor is null!",Log.demo);
			} catch (RemoteException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
	    	return;
	    }
	
//		if( (x>this.serverId)&&(x<this.getSuccessorId()))
		if(Util.exBetween(x, this.serverId, this.getSuccessorId()))
		{
			Log.log("x is between pre and succ",Log.test);
			try {
				this.setSuccessor(xip);
				succ.notify((CalendarManagerInterface)this);
			} catch (RemoteException e) {
				// TODO Auto-generated catch block
				//e.printStackTrace();
				Log.log("stablize.error..",Log.test);
			}			
		}		
	}
	
	/**
	 * notify server node to update finger table
	 */
	public void notify(CalendarManagerInterface other) throws RemoteException
	{
		Log.log("------------------------[notify]-----------------------------",Log.demo);
		int otherId = other.getServerId();
//		boolean inBetween = (otherId>this.getPredecessorId()) && (otherId<this.getServerId());
		boolean inBetween = Util.exBetween(otherId, this.getPredecessorId(), this.getServerId());
		//		boolean inBetween = Util.isBetween(otherId, this.getPredecessorId(), this.getServerId());
		if(this.predecessorId == config.initPredecessor || inBetween)
		{
			//System.out.println("notify function executing...");
			this.setPredecessor(other.getServerIP());
		}
	}
	
	/**
	 * fix fingers in finger table
	 */
	public void fix_fingers() throws RemoteException
	{
		Log.log("------------------------[fix_finger]-----------------------------",Log.demo);
//		Random rd = new Random();
//		int index = rd.nextInt(config.keySize);		
		
		for(int index=0; index<config.keySize; index++){
					
			Log.log("updating finger table..... index= " + index,Log.test);
			CalendarManagerInterface succ = this.find_successor(this.fingertable.table[index].start);
			if(succ==null)
			{
				Log.log("No sucessor found for"+this.fingertable.table[index].start,Log.test);
			}		
			else
			{
				if(succ.getServerId()!=this.fingertable.table[index].successorId)
				{
					Log.log(this.fingertable.table[index].start +" 's successor change from " + this.fingertable.getFinger(index).successorId
					+" to " + succ.getServerId(),Log.demo);
					this.fingertable.setFinger(index, succ.getServerIP());
					//if index=0, set successor in a different way, so that we will be able to update SOS for predecessor
					if(index==0)
						this.setSuccessor(succ.getServerIP());
				}			
			}
		}
	}
	
	/**
	 * fix finger table
	 */
	public void fix_fingers_byOther() throws RemoteException
	{
		Log.log("------------------------[fix_fingers_byOther]-----------------------------",Log.test);
//		Random rd = new Random();
//		int index = rd.nextInt(config.keySize);
		
		Random rd = new Random();
		double a = rd.nextDouble();
		CalendarManagerInterface other = null;
		if(a>0.5)
		{
			other = this.getCalendarManagerInterface(this.getSuccessorIP());
		}else
		{
			other = this.getCalendarManagerInterface(this.getPredecessorIP());
		}
		
		if(other==null)
		{
			Log.log("Other is null", Log.test);
			return;
		}
		for(int index=0; index<config.keySize; index++){
					
			Log.log("updating finger table..... index= " + index,Log.test);
			CalendarManagerInterface succ = other.find_successor(this.fingertable.table[index].start);
			if(succ==null)
			{
				Log.log("No sucessor found!", Log.test);
			}		
			else
			{
				Log.log(this.fingertable.table[index].start +" 's successor change from " + this.fingertable.getFinger(index).successorId, Log.test);
				Log.log(" to " + succ.getServerId(), Log.test);
				this.fingertable.setFinger(index, succ.getServerIP());		
				//if index=0, set the successor in a different way, so that we will be able to update the SOS for predecessor
				if(index==0)
					this.setSuccessor(succ.getServerIP());
			}
		}
	}
		
    /**
     * node join to chord
     */
	public void join(CalendarManagerInterface other) throws RemoteException {
		// TODO Auto-generated method stub		
		Log.log("------------------------[join]-----------------------------",Log.demo);
		//add other steps
		this.setPredecessor(null);
		this.setPOP(null);
		
		if(other!=null)
		{
			//System.out.println("join by id ="+other.getServerId());
			this.init_finger_table(other);
			this.update_others();
			
			//move keys in (predecessor,n] from successor
			Log.log(this.serverId + " Try to copy data from successor "+this.getPredecessorId(),Log.demo);
			CalendarManagerInterface succ = this.getCalendarManagerInterface(this.getSuccessorIP());
			if(succ==null)
			{
				Log.log("Successor is null, not able to copy",Log.demo);
			}else
			{
				Log.log("Successor "+succ.getServerId()+" got successfully",Log.test);
				succ.copyToNewNode((CalendarManagerInterface)this);
			}
			
			Log.log("Finger table after join for id= "+other.getServerId(),Log.demo);
			other.showFingerTabler();
		}
		else
		{
			
			//System.out.println("[Join] other is null, initial successor in the finger talbe as the node itself!");
			for(int i=0; i<config.keySize; i++)
			{				
				this.fingertable.setFinger(i, this.serverIP);				
			}
			this.setPredecessor(this.serverIP);	
			//call this function, so we will be able to set SOS for predecessor
			this.setSuccessor(this.serverIP);
		}		
		
	}
	
	/**
	 * initalize finger table
	 */
	public void init_finger_table(CalendarManagerInterface other) throws RemoteException
	{
		Log.log("------------------------[init_finger_table]-----------------------------",Log.demo);
		Log.log(other.getServerId() +" is finding sucessor for start node  " + this.fingertable.table[0].start,Log.demo);
		
		CalendarManagerInterface succ = other.find_successor(this.fingertable.table[0].start);
		while(succ==null){
			Log.log("successor not found, wait for 3 seconds",Log.demo);
			try {
				Thread.sleep(3000);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			succ = other.find_successor(this.fingertable.table[0].start);
		}
		
		if(succ.getServerId()==this.serverId)
		{
			Log.log("Outdate data,"+this.serverId +" should not be in chord!",Log.demo);
		}
			
		
		int succid = Hashing.hash(succ.getServerIP());
		String succip = succ.getServerIP();
		Log.log(this.fingertable.table[0].start + " successor is: " + succid,Log.demo);
		
		//this.fingertable.setFinger(0, succip);
		//change this to, so we wil lbe able to update SOS for predecessor
		this.setSuccessor(succip);
		this.setPredecessor(succ.getPredecessorIP());
		succ.setPredecessor(this.serverIP);
		Log.log(succid +" predecessor is: "+succ.getPredecessorId(),Log.demo);
		
		//add by me, e.g. 23 is the first node, 9 join using 23, then initially succ(23)=23, so 9 in(23,23), so succ(23)=9
		if(Util.exBetween(this.serverId, succid, succ.getSuccessorId()))
		{
			Log.log(this.serverId + "is between ("+succid+","+succ.getSuccessorId()+")",Log.chordFind);
			Log.log(succid +"'s successor change from "+succ.getSuccessorId()+" to "+this.serverId,Log.chordFind);
			succ.setSuccessor(this.serverIP);
		}
		
		Log.log("Begin initial finger table...",Log.demo);
		for(int i=0; i<config.keySize-1; i++)
		{
			int lastSucc = this.fingertable.table[i].successorId;
			int iStart = this.fingertable.table[i+1].start;
			
			//System.out.println("lastSucc= "+lastSucc +",nextNode = "+ iStart +" ,serverId= "+this.serverId);
			//if( (iStart>= this.serverId) && (iStart<lastSucc))
			if( Util.inBetweenExRight(iStart, this.serverId, lastSucc))
			{
				Log.log(iStart + " is between [" + this.serverId +" ," +lastSucc+")",Log.chordFind);
				Log.log("use last Succ, " + iStart +"'s successor is " + lastSucc,Log.chordFind);
				this.fingertable.setFinger(i+1, this.fingertable.table[i].successorIP);
			}else
			{
				Log.log("find new succ ",Log.chordFind);
				CalendarManagerInterface newSucc = other.find_successor(iStart);
				if(newSucc!=null)
				{
					
					String newSuccip = newSucc.getServerIP();
					Log.log("= "+Hashing.hash(newSuccip),Log.chordFind);
					this.fingertable.setFinger(i+1, newSuccip);
				}else
				{
					Log.log("[init_finger_table]'s successor is null",Log.demo);
				}
			}
		}	
		
		Log.log("Finish initial finger table for id= "+this.serverId,Log.demo);
		Log.log("current finger table for node =" + this.serverId,Log.demo);
		this.showFingerTabler();
		Log.log("current finger table for node =" + other.getServerId(),Log.demo);
		other.showFingerTabler();
		
	}
	
	/**
	 * show finter table
	 */
	public void showFingerTabler() throws RemoteException
	{
		Log.log("+++++++++++++++++++++++++++++",Log.demo);
		this.fingertable.show();
		this.printPreSucc();
		this.printPrimary();
		this.printBackup();
		Log.log("+++++++++++++++++++++++++++++",Log.demo);
		
	}
	
	/**
	 * update all nodes whose finger tables should refer to n
	 */
	public void update_others() throws RemoteException
	{
		Log.log("-----------------[update_others]-----------------------",Log.demo);
		
		for(int i=0; i<config.keySize; i++)
		{
			
			int suspect = (int) (this.serverId - Math.pow(2, i));
			if(suspect<0) suspect = (int) (suspect + config.chordSize);
			Log.log("nodes whose predecessor may need to be updated " + suspect,Log.demo);
			CalendarManagerInterface p = this.find_predecessor(suspect);
			
		    if(p!=null) p.update_finger_table(this.serverIP,i);
		    else
		    {
		    	Log.log("Update_others error, found no predecessor of id= "+suspect,Log.demo);
		    }
		}
		Log.log("-----------------[update_others end]-----------------------",Log.demo);
	}
	
	
	public void update_finger_table(String s, int i) throws RemoteException
	{
		Log.log("------------------[update_finger_table]---------------------",Log.demo);
		
		int sid = Hashing.hash(s);
		//if( (sid>=this.serverId) && (sid<this.fingertable.table[i].successorId))
		//if( Util.inBetweenExRight(sid, this.serverId, this.fingertable.table[i].successorId))
		//changed! different from paper
		if( Util.exBetween(sid, this.serverId, this.fingertable.table[i].successorId))
		{
			Log.log(sid + " is between ( " + this.serverId + "," + this.fingertable.table[i].successorId+")",Log.chordFind);
			Log.log(this.fingertable.table[i].start + "'s successor change from "+this.fingertable.table[i].successorId +" to " + sid,Log.demo);
			this.fingertable.setFinger(i,s);
			
			//add this, so we will be update to update SOS for predecessor
			if(i==0)
			{
				this.setSuccessor(s);
			}
			
			//System.out.println("Finger set succeed!");
			if(this==null) Log.log("This obj is null",Log.demo);
			if(this.getPredecessorIP()==null) Log.log("Predecessor of current node "+this.getServerId()+" is null",Log.demo);
			else{
				Log.log("Precessor IP is "+this.getPredecessorIP(),Log.test);
			}
			
			//test
			Log.log(this.serverId + " is trying to get CalendarManagerInterface for node: " + this.getPredecessorId(),Log.test);
			CalendarManagerInterface pre = null;
			try{
				pre = this.getCalendarManagerInterface(this.getPredecessorIP());
				//test, replace IP with "129.174.94.91"
				// pre = this.getCalendarManagerInterface("129.174.94.91");
			}catch(Exception e)
			{
				Log.log("fial to get predecessor for " + this.serverId, Log.test);
			}
			//CalendarManagerInterface pre = this.getCalendarManagerInterface(this.serverIP);
			Log.log("Pre get succeed!",Log.test);
			


			
			if(pre!=null){
				if(!(pre.getServerId()==this.getServerId()))
				pre.update_finger_table(s,i);
				else Log.log("Predecessor is the same with current node, so avoid dead while",Log.test);
					
			}else
			{
				Log.log(this.serverIP + " 's Precessor is null",Log.test);
			}
		}
	}

	
	public CalendarManagerInterface find_successor(int id)
			throws RemoteException {
		// TODO Auto-generated method stub
		
		Log.log("-------------------[find_successor]------------------",Log.chordFind);
		Log.log("{Input} id= "+id,Log.chordFind);
		if(id==config.nullHash)
		{
			Log.log("Input is null,  node "+this.serverId+" is not in chord!",Log.chordFind);
			return null;
		}
		
		CalendarManagerInterface n = this.find_predecessor(id);
		if(n == null) {
			Log.log(id +" 's precessor is null, so cannot find successor",Log.chordFind);
			return null;
		}
		
		String succIP = n.getSuccessorIP();
		Log.log(id +"'s successor is "+Hashing.hash(succIP),Log.chordFind);
		
		return this.getCalendarManagerInterface(succIP);
		
	}
	
	


	@Override
	/**
	 * find predecessor for given id
	 */
	public CalendarManagerInterface find_predecessor(int id)
			throws RemoteException {
		// TODO Auto-generated method stub
		
		Log.log("----------[find_predecessor]----------",Log.chordFind);
		Log.log("{Input} id= "+id,Log.chordFind);
		if(id==config.nullHash)
		{
			Log.log("Input is null,  node "+this.serverId+" is not in chord!",Log.chordFind);
			return null;
		}
		
		
		CalendarManagerInterface n = this;
		if(n==null){
			Log.log(id+" 's predecessor is null!",Log.chordFind);
			return null;
		}
		

		while(!Util.inBetweenExLeft(id, n.getServerId(), n.getSuccessorId()))
		{
			Log.log(id + " is not between ( " + n.getServerId() + "," + n.getSuccessorId()+" ]",Log.chordFind);
			 CalendarManagerInterface newN  = n.closest_preceding_finger(id);
			 if(newN==null)
			 {
				 Log.log("[find_predecessor], sub call cloest_preceding_finger fail",Log.chordFind);
			 }
			 if(n.equals(newN)){
				 Log.log("the closest_preceding_finger found no new result, it is possible there is only one node ",Log.chordFind);
				 break;
			 }
			 else n = newN;
 		}
		
		if(Util.inBetweenExLeft(id, n.getServerId(), n.getSuccessorId())){
			Log.log(id + " is  between ( " + n.getServerId() + "," + n.getSuccessorId()+" ]",Log.chordFind);
		}
	
		return n;
	}

	@Override
	public CalendarManagerInterface closest_preceding_finger(int id)
			throws RemoteException {
		
		Log.log("------[closest_preceding_finger]-----",Log.chordFind);
		Log.log("{Input} id= "+id,Log.chordFind);
		// TODO Auto-generated method stub
		Log.log(this.serverId+ " is finding closest_pre_finger for "+ id+"...",Log.chordFind);
		for(int  m=config.keySize-1; m>=0; m--)
		{
			//System.out.println("get finger "+ m );
			Finger fg = this.fingertable.getFinger(m);
			//	if( (fg.successorId>this.serverId) && (fg.successorId<id))
		    if(Util.exBetween(fg.successorId, this.serverId, id))
			{
				Log.log(fg.successorId + " is between (" + this.serverId + " ," + id +" )",Log.chordFind);
				Log.log(id+ " 's cloest preceding is " + fg.successorId,Log.chordFind);
				return this.getCalendarManagerInterface(fg.successorIP);
			}
		}
		
		//means calendarObject
		Log.log(" No finger is between ("+this.serverId+","+id +") return " +this.getServerId(),Log.chordFind);
		return this;
	}
	
	
	/**
	 * server node leave
	 * not used!
	 */
	public void leave() throws RemoteException
	{
		CalendarManagerInterface pre = this.getCalendarManagerInterface(this.getPredecessorIP());
		CalendarManagerInterface succ = this.getCalendarManagerInterface(this.getSuccessorIP());
		
		succ.setPredecessor(this.predecessorIP);
		//need add: succ set POP = my POP
		pre.setSuccessor(this.getSuccessorIP());
		//need add: pre set SOS = my SOS		
	}
	
	
	



	////////////////////////////////////////// Utility ////////////////////////////////////////////////
	/**
	 * set predecessor
	 */
	public void setPredecessor(String pre) throws RemoteException 
	{
		if(pre==null){
			this.predecessorId = config.initPredecessor;
			return;
		}
		int oldPre = this.getPredecessorId();
		this.predecessorIP = pre;
		this.predecessorId = Hashing.hash(pre);
		
		//change above two lines to this, to make it recursive
		//this.setPredecessor(pre);
		
		CalendarManagerInterface succ = this.getCalendarManagerInterface(this.getSuccessorIP());
		if(succ!=null)
		{
			if(succ.getPOPid()!=Hashing.hash(pre) || succ.getPOPid()==-1)
			{
				Log.log(succ.getServerId() + " 's old popid is"+ this.POPid +", update to new popid: "+Hashing.hash(pre),Log.demo);
				succ.setPOP(pre);
			}	
		}else
		{
			Log.log(this.getServerId() + " 's successor is null, so not able to set SOS",Log.demo);
		}
		
		if(oldPre!=this.getPredecessorId())
		{
			Log.log(this.serverId+" 's predecessor changed from "+oldPre+" to "+this.getPredecessorId(),Log.demo);
//			this.BackupToPrimary();
//			this.BackupData();
		}
	}
	
	/**
	 * update SOS and POP
	 * periodically called
	 * @throws RemoteException
	 */
	public void updateSOSandPOP() throws RemoteException
	{
		Log.log("-----------[updateSOSandPOP]----------------",Log.demo);
		//if(this.getSOSid()==config.nullHash)
		{
			CalendarManagerInterface succ = this.getCalendarManagerInterface(this.getSuccessorIP());
			if(this.getSOSid()!=succ.getSuccessorId())
				this.setSOS(succ.getSuccessorIP());
		}
		//if(this.getPOPid()==config.nullHash)
		{
			CalendarManagerInterface pre = this.getCalendarManagerInterface(this.getPredecessorIP());
			if(this.getPOPid()!=pre.getPredecessorId())
				this.setPOP(pre.getPredecessorIP());
		}
	}
	
	public String  getPredecessorIP()
	{
		return predecessorIP;
	}
	
	public int getPredecessorId()
	{
		return predecessorId;
	}
	

	public  void setSuccessor(String succ) throws RemoteException
	{		
		//if this is new successor, then backup data
		
		String oldSuccip = this.fingertable.table[0].successorIP;
		
		this.fingertable.setFinger(0,succ);
		
		CalendarManagerInterface pre = this.getCalendarManagerInterface(this.getPredecessorIP());
		if(pre!=null)
		{
			//set SOS for predecessor
			if(pre.getSOSid()!= this.getSuccessorId())
			{
				pre.setSOS(succ);
			}
		}else
		{
			Log.log(this.getServerId() +" 's predecessor is null, so not able to set SOS for him",Log.demo);
		}
		
		//backup to new successor
		if(oldSuccip==null)
		{
			BackupData();
		}else
		{
			Log.log("Old successor is not null, but is out of date",Log.demo);
			if(!oldSuccip.equals(succ))
			{
				BackupData();
				CalendarManagerInterface oldSucc = this.getCalendarManagerInterface(oldSuccip);
				if(oldSucc!=null)
				{
					oldSucc.removeBackupInOldSucc();
				}
				//this.removeBackupInOldSucc(oldSuccip);
				Log.log(this.serverId +" 's successor change from "+ Hashing.hash(oldSuccip)+" to "+ this.getSuccessorId(), Log.demo);
			}
			
		}
		
	}
	
	
	
	public String  getSuccessorIP()
	{
		return this.fingertable.getFinger(0).successorIP;
	}
	
	public int getSuccessorId()
	{
		return this.fingertable.getFinger(0).successorId;
	}
	
	
	//////// SOS and POP
	public String getSOSip()
	{
		return this.SOSip;
	}
	
	public int getSOSid()
	{
		return this.SOSid;
	}
	
	public String getPOPip()
	{
		return this.POPip;
	}
	
	public int getPOPid()
	{
		return this.POPid;
	}
	
	public void setSOS(String sosip)
	{
		Log.log("----------------[updating SOS]--------------",Log.demo);
		if(this.SOSid!=Hashing.hash(sosip))
		{
			this.SOSip = sosip;
			this.SOSid = Hashing.hash(sosip);
		}
	}
	
	public void setPOP(String popip)
	{
		Log.log("----------------[updating POP]--------------",Log.demo);
		if(this.POPid!=Hashing.hash(popip))
		{
			this.POPip = popip;
			this.POPid = Hashing.hash(popip);
		}
	}
	
	public int  getServerId()
	{
		return serverId;
	}
	
	public String getServerIP()
	{
		return serverIP;
	}
	
	public void printServerIP(byte[] address1) {
//		for (byte b : address1) {
//			System.out.print((b + 256) % 256 + " ");
//		}
		try {
			Log.log(new String(address1,"utf-8"),Log.test);
		} catch (UnsupportedEncodingException e) {
			// TODO Auto-generated catch block
			//e.printStackTrace();
			Log.log("get IP error", Log.test);
		}
	}
	
	public void printPreSucc()
	{
		Log.log(this.serverId + "'s precessor is: " + this.predecessorId,Log.test);
		Log.log(this.serverId + "'s successor is: " + this.getSuccessorId(),Log.test);
		Log.log(this.serverId + "'s sos is: " + this.getSOSid(),Log.test);
		Log.log(this.serverId + "'s pop is: " + this.getPOPid(),Log.test);
	}
	
	public void printBackup() 
	{
		Log.log("Backup data is....",Log.test);
		for(String user: this.BKuserCalendars.keySet())
		{
			CalendarObj co = this.BKuserCalendars.get(user);
			Log.log("	Calenar owner is :"+co.owner,Log.test);	
		}
		for(String user: this.BKcallbacks.keySet())
		{
			CalendarClientInterface ci = this.BKcallbacks.get(user);
			try {
				Log.log("	Callback owner is :"+ci.getUser(),Log.test);
			} catch (RemoteException e) {
				// TODO Auto-generated catch block
				//e.printStackTrace();
				Log.log("	Callback owner is not available",Log.test);
			}	
		}
		this.save();
	}
	
	public void printPrimary() 
	{
		Log.log("Primary data is....",Log.test);
		for(String user: this.userCalendars.keySet())
		{
			CalendarObj co = this.userCalendars.get(user);
			Log.log("	Calendar owner is :"+co.owner,Log.test);	
		}
		for(String user: this.callbacks.keySet())
		{
			CalendarClientInterface ci = this.callbacks.get(user);
			try {
				Log.log("	Callback owner is :"+ci.getUser(),Log.test);
			} catch (RemoteException e) {
				// TODO Auto-generated catch block
				Log.log("	Callback owner is not available",Log.test);
			}	
		}
	}
	
	private void getServerIPfromLocal() {
		InetAddress inetAddress = null;

		NetworkInterface ni = null;

		try {
			// try to get the address from eth1 first
			ni = NetworkInterface.getByName("eth1");
			// if eth1 not found, use eth0
			if (ni == null) {
				inetAddress = InetAddress.getLocalHost();
			} else {
				Enumeration<InetAddress> addresses = ni.getInetAddresses();

				while (addresses.hasMoreElements()) {
					InetAddress tempAddress = addresses.nextElement();
					if (tempAddress instanceof Inet4Address) {
						inetAddress = tempAddress;
					}
				}
			}

			this.serverIP = Util.IpAddressByteToString(inetAddress.getAddress());
			this.serverId = Hashing.hash(this.serverIP);
			Log.log("Server IP:"+this.serverIP, Log.demo);
			Log.log("Server ID:"+this.serverId, Log.demo);

		} catch (SocketException e) {
			// TODO Auto-generated catch block
			//e.printStackTrace();
			Log.log("Get server IP error", Log.test);
		} catch (UnknownHostException e) {
			// TODO Auto-generated catch block
			//e.printStackTrace();
			Log.log("Get server IP error", Log.test);
		}

	}

	/* (non-Javadoc)
	 * @see rmi.calendar.CalendarManagerInterface#assignRightServer(java.lang.String)
	 */
	@Override
	public CalendarManagerInterface assignRightServer(String user)
			throws RemoteException {
		// TODO Auto-generated method stub
	
		int uid = Hashing.hash(user);
		return this.find_successor(uid);
	}

	

	

	
	
	

	

	
	

}
