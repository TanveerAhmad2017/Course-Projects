package rmi.calendar;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.io.Serializable;
import java.rmi.Naming;
import java.rmi.NotBoundException;
import java.rmi.RMISecurityManager;
import java.rmi.RemoteException;
import java.rmi.registry.LocateRegistry;
import java.rmi.registry.Registry;
import java.rmi.server.UnicastRemoteObject;
import java.sql.Time;
import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Calendar;
import java.util.Date;
import java.util.Scanner;

import util.Log;
import util.config;

import DHash.Hashing;


/**
 * Calendar client
 * client can add, delete, retrive their events
 * 
 * when events are going to happen, clients will receive notification
 * the callback is realized by making the CalendarClientInterface as an remote Objct that allow servers to call
 * @author huangxin
 *
 */

//Note!!!!
//shoud remember to extends UnicastRemoteObject
//Otherwise the eventAlert will be produce at server site
public class CalendarClient extends UnicastRemoteObject implements Serializable, CalendarClientInterface {
	
	
	protected CalendarClient() throws RemoteException {
		super();
	}
	
	
	/**
	 * parameter used
	 */
	private static CalendarClient _instance;
	private static CalendarManagerInterface CalendarManager = null;
	private CalendarObjInterface CalendarObj = null;
	private static String currentUser = null;
	private static Registry registry=null;
	private static String initialHost = null;
	
	
	/**
	 * call by server to notify clients
	 */
	public void EventAlert(Event e) throws RemoteException{
		System.out.println("Alert from server: you have an upcoming schedule event!!");
		System.out.println(e);
	}
	
	public String getUser()
	{
		return this.currentUser;
	}
	
	
	/**
	 * create calendar
	 * 
	 * @param argv
	 * @throws RemoteException
	 * @throws NotBoundException
	 */
	public void createCalendar(String argv[]) throws RemoteException, NotBoundException {
		System.out.println("Find right server..");
		findRightServer(argv[1]);
		System.out.println("Create calendar : " + CalendarManager.createCalendar(argv[1]));
	}
	
	
	public void findRightServer(String user) throws RemoteException, NotBoundException
	{
		try {
			System.out.println("Old server id is: " + CalendarManager.getServerId());
			System.out.println("Find the right server...");
			CalendarManagerInterface cmi = CalendarManager.assignRightServer(user);
			String host  = cmi.getServerIP();
			this.registry = LocateRegistry.getRegistry(host, config.port);
			this.CalendarManager = (CalendarManagerInterface) (registry.lookup("CalendarServerAlias"));
			System.out.println("server id is: " + CalendarManager.getServerId());
			CalendarObj = CalendarManager.ConnectCalendar(this.currentUser);
		} catch (RemoteException e) {
			// TODO Auto-generated catch block
			//e.printStackTrace();
			Log.log("current server is down, back to initial server", Log.demo);
			 ReconnectServer();
		} catch (NotBoundException e) {
			// TODO Auto-generated catch block
			//e.printStackTrace();
			Log.log("current server is down, back to initial server", Log.demo);
			 ReconnectServer();
		}
				
	}
	
	public void ReconnectServer() 
	{
		try {
			this.registry = LocateRegistry.getRegistry(this.initialHost, config.port);
			this.CalendarManager = (CalendarManagerInterface) (registry.lookup("CalendarServerAlias"));
			if(CalendarManager!=null)
			Log.log("current server is"	+ this.CalendarManager.getServerId() , Log.demo);
			else
			Log.log("recoonect!" , Log.demo);
				
		} catch (RemoteException e) {
			// TODO Auto-generated catch block
			//e.printStackTrace();
			Log.log("Reconnect server...", Log.demo);
		} catch (NotBoundException e) {
			// TODO Auto-generated catch block
			//e.printStackTrace();
		}		
	}
	
	public void connectCalendar(String argv[]) throws RemoteException, NotBoundException {
		if(argv.length < 2) {
			System.out.println("Usage: \"connect: <user>\" ");
			return;
		}
		
		String user = argv[1].trim();	
		System.out.print("Connecting calender for " + user + "...");
		
		findRightServer(user);		
		
		CalendarObj = CalendarManager.ConnectCalendar(user);	
		System.out.println("Before fail...Current CalendarManager is "+ CalendarManager.getServerId());
		if(CalendarObj == null) {
			currentUser = null;
			System.out.println(" failed!");
		}
		else {
			//update current user
			currentUser = user;
			System.out.println(" connected!");			
			CalendarManager.registerClient(this, user);
		}
	}
	
	/*
	 * check whether CalendarObj is null or not
	 */
	public boolean checkCalendarObj() {
		if(CalendarObj == null) {
			System.out.println("CalendarObj not connected, to connect, please connect using \"connect: <user>\"!");
			return false;
		}
		return true;
	}
	
	
	
	/**
	 * schedule event
	 * @param line
	 * @throws RemoteException
	 * @throws ParseException
	 */
	public void schedule(String line) throws RemoteException, ParseException {
		if(!checkCalendarObj()) return;
		
		Event e;
		if(line.split(";").length==5){
			String[] params = Util.getUserInput(line,5);
			if(params!=null){
			 e = Util.parseEvent(params[0],params[1],params[2],params[3],params[4]);
			 System.out.println("Schedule... "+CalendarObj.ScheduleEvent(params[4], e));			
			}
			}
		else
		{
			String[] params = Util.getUserInput(line,4);
			if(params!=null){
			 e = Util.parseEvent(params[0],params[1],params[2],params[3],currentUser);
			 System.out.println("Schedule... "+CalendarObj.ScheduleEvent(currentUser, e));
			}
		}								
	}
	
	
	/**
	 * update event
	 * @param line
	 * @throws RemoteException
	 * @throws ParseException
	 */
	public void update(String line) throws RemoteException, ParseException {
		if(!checkCalendarObj()) return;
		
		Event e;
		if(line.split(";").length==6){
			String[] params = Util.getUserInput(line,6);
			if(params!=null){
			 e = Util.parseEvent(params[1],params[2],params[3],params[4],params[5]);
			 if(e==null) return;
			 System.out.println("Update... "+CalendarObj.UpdateEvent(params[0], e));
			}
		}
		else
		{
			String[] params = Util.getUserInput(line,5);
			if(params!=null){
			 e = Util.parseEvent(params[1],params[2],params[3],params[4],currentUser);
			 if(e==null) return;
			 System.out.println("Update... "+CalendarObj.UpdateEvent(params[0], e));		
			}
			}								
	}
	
	
	/**
	 * delete event
	 * @param line
	 * @throws RemoteException
	 */
	private void deleteEvent(String line) throws RemoteException {
		// TODO Auto-generated method stub
		String[] params = Util.getUserInput(line,1);
		if(params!=null){
		System.out.println("user input eventId "+params[0]);
		System.out.println("delete... "+CalendarObj.RemoveEvent(params[0]));
		}
	}
	
	
	/**
	 * retrive event
	 * @param line
	 * @throws RemoteException
	 * @throws ParseException
	 */
	public void retrieve(String line) throws RemoteException, ParseException {
		
		if(!checkCalendarObj()) return;
		
		String user = null;
		Date startDate = null;
		Date endDate = null;
		SimpleDateFormat sdf = new SimpleDateFormat("MM-dd-yyyy HH:mm:ss");
		
		if(line.split(";").length==2)
		{
			String[] params = Util.getUserInput(line,2);
			if(params!=null){
			user = this.currentUser;
			startDate = sdf.parse(params[0]);
			endDate = sdf.parse(params[1]);
			}
		}
		else{
			
			String[] params = Util.getUserInput(line,3);
			if(params!=null){
			user = params[0].trim();						
			startDate = sdf.parse(params[1]);
			endDate = sdf.parse(params[2]);
			}
		}
		
		
		if(user!=null)
		{						
			ArrayList<Event> rtn = CalendarObj.RetrieveEvent(user, startDate, endDate);	
			if(rtn==null) {
				System.out.println("return event is null");
			}else{
			System.out.println(rtn.toString());
			}
		}
	}
	
	
	/**
	 * list users
	 * note: when server is down, reconnect to initial server to find current successor of this client
	 * @throws RemoteException
	 * @throws NotBoundException
	 */
	public void list() throws RemoteException, NotBoundException {
		
		try {
			System.out.println("List users who have calendar obj: \n "+ CalendarManager.list());
		} catch (RemoteException e) {
			// TODO Auto-generated catch block
			//e.printStackTrace();
			ReconnectServer();
			findRightServer(this.currentUser);
			list();
		}
		
	}
	
	/**
	 * list event
	 * 
	 * @param user
	 * @throws RemoteException
	 * @throws NotBoundException
	 */
	public void eventList(String user) throws RemoteException, NotBoundException {
		
		try{
		if(!checkCalendarObj()) return;
		System.out.println( this.currentUser + "'s eventList: \n "
		+ CalendarObj.myEventList(user).toString());
		}catch (RemoteException e) {
			// TODO Auto-generated catch block
			//e.printStackTrace();
			ReconnectServer();
			findRightServer(this.currentUser);
			eventList(user);			
		}
		
	}
	
	
	
	
	
	/**
	 * main
	 * 
	 * @param argv
	 * @throws RemoteException
	 */
	public static void main(String argv[]) throws RemoteException{
		
		//Parse the input to get the hostname(the address of server)		
	
		
		String host = "";		
		if(argv.length==1){
			host = argv[0];
			initialHost = host;
		}else{
			System.out.println("Usage: RMIClient");
			System.exit(1);
		}
		
		System.out.println("Current CalendarManager connect to is: " + host);
		//install a security manager
		System.setSecurityManager(new RMISecurityManager());
		
		//method 1
		//request a reference to the server object
		//comemnt by Huangxin
		//String name = "rmi://"+host+"/RMIServer";
		//System.out.println("Looking up: "+name);
		//String name = "rmi://"+host;
		
		//method 2
		registry = LocateRegistry.getRegistry(host, config.port);
		
		
		
		
		try{
			
			//In reality, Naming.lookup()will return an instance of
			//examples.RMI_stub
			//this is typecast into the ServerInterface, which is 
			//what specifies the avilable server methods
			
			//method 1 & method2
			//server = (ServerInterface)Naming.lookup(name);
			CalendarManager = (CalendarManagerInterface) (registry.lookup("CalendarServerAlias"));
			//CalendarManager = (CalendarManagerInterface) (Naming.lookup("CalendarServerAlias"));
			
			System.out.println("CalendarManager connected!");
			
			_instance = new CalendarClient();
			_instance.mainLoop();
			
		}catch(Exception e){
			System.out.println("Exception "+e);
			System.exit(1);
		}
		
		
		
	}
	
	
	/**
	 * process user input
	 */
	public void mainLoop() {
		//Usage
		Util.usageStatement();
		
		Scanner scanner = new Scanner(System.in);
		//Given a reference to the server object, it is now
		//possible to invoke methods as usual
		try{
			while(scanner.hasNextLine())
			{
				String line=scanner.nextLine();
				if(processCommand(line)) break;
			}
		}catch(Exception e){
			System.out.println("Exception "+e);
			System.exit(1);
		}
	}
		
	
	/**
	 * deltail process user command
	 * @param line
	 * @return
	 * @throws ParseException
	 * @throws NotBoundException
	 * @throws RemoteException
	 */
	private boolean processCommand(String line) throws  ParseException, NotBoundException, RemoteException
	{
		
			String[] args = line.split(": ");
			String cmd = args[0];
			if(cmd.equals("exit")) return true;
			else if(cmd.equals("create")) {
				this.createCalendar(args);
				
			}
			else if(cmd.equals("schedule")) {
				this.schedule(line);
			}
			else if(cmd.equals("retrieve")) {
				this.retrieve(line);
			}
			else if(cmd.equals("connect")) {
				this.connectCalendar(args);			
			}
			else if(cmd.equals("list")) {
				this.list();
			}
			else if(cmd.equals("myEventList")) {
				this.eventList(this.currentUser);
			}
			else if(cmd.equals("delete")) {
				this.deleteEvent(line);
			}
			else if(cmd.equals("update")) {
				this.update(line);
			}
			else if(cmd.equals("test")) {
				this.test(args[1]);
			}
			else if(cmd.equals("testNotify"))
			{
				this.testNotify(30);
			}
	//		else if(cmd.equals("deleteDatabase")) {
	//			this.deleteDatabase();
	//		}
			else if(cmd.equals("help")){
				Util.usageStatement();
			}else
			{
				System.out.println("Please type \"help: \" to see the usage");
			}
			
			System.out.println("------------------------------------");

			return false;
	}

	
	
	/**
	 * read test command from file and execute
	 * @param filename
	 * @throws RemoteException
	 * @throws ParseException
	 * @throws NotBoundException
	 */
	private void test(String filename) throws RemoteException, ParseException, NotBoundException {
		// TODO Auto-generated method stub
		Scanner s;
		try {
			s = new Scanner(new File(filename));
			System.out.println("Read test script from file: "+filename);	
			while(s.hasNextLine())
			{
				String line=s.nextLine();
				System.out.println("USER INPUT COMMAND  \"" +line +"\"");
				Thread.sleep(50);
				if(processCommand(line)) break;
			}
		} catch (FileNotFoundException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
	}
	
	
	/**
	 * test callback notification
	 * @param timeLeft
	 * @throws ParseException
	 * @throws RemoteException
	 */
	private void testNotify(int timeLeft) throws ParseException, RemoteException
	{
		System.out.println("Test Notify function: event will happen "+timeLeft+" sec later for Bob,Sandy");
		SimpleDateFormat sdf = new SimpleDateFormat("MM-dd-yyyy HH:mm:ss");
		Calendar cal = Calendar.getInstance();
	
		cal.add(cal.SECOND,timeLeft);
		String  startTime = sdf.format(cal.getTime());
		cal.add(cal.SECOND,timeLeft);
		String  endTime = sdf.format(cal.getTime());
		
		String line = "schedule: "+startTime+";"+endTime+";"+"testnotifyEvent"+";group"+";Bob,Sandy";
		schedule(line);
	}
	

	

}
