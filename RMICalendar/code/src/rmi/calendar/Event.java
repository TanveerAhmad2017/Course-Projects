package rmi.calendar;

import java.io.Serializable;
import java.text.SimpleDateFormat;
import java.util.Date;

public class Event implements Serializable,Comparable<Event>{
	
	
	private Date startDate;
	private Date endDate;
	private String text;
	private String accessType;
	private String accessList;
	private String eventId;
	
	
		
	public Event(Date startDate, Date endDate, String text, String accessType, String accessList)
	{
		
		this.startDate = startDate;
		this.endDate = endDate;
		this.text = text;
		this.accessType = accessType;
		this.accessList = accessList;
	}
	
	/*
	 * check user permission
	 */
	public boolean containUser(String name)
	{
		String[] users = this.accessList.split(",");
		for(int i=0; i<users.length; i++)
		{
			if(users[i].equals(name)) return true;
		}
		return false;
	}
	
	//check time is contain in open event or not
	public boolean contains(Date startDate, Date endDate) {
		return (!this.getStartDate().after(startDate) && !this.getEndDate().before(endDate));		
		
	}
	
	//check time is intersected with other events or not
	public boolean intersect(Date startDate, Date endDate){
		
		if( (!this.startDate.before(startDate)&& !this.startDate.after(endDate)) 
		||  (!this.startDate.after(startDate) && !this.endDate.before(startDate)))
		{
			return true;
		}
		return false;
	}
	
	
	
	// =======================================================================================
	// Utility function
	// =======================================================================================
	
	public String generateEventId()
	{
		this.eventId = this.hashCode()+"";
		return this.eventId;
		
	}
	
	public String getEventId()
	{
		return this.eventId;
	}
	
	public Date getStartDate(){
		return startDate;
	}
	
	public Date getEndDate(){
		return endDate;
	}
	
	public String getText(){
		return text;
	}
	
	public String getAccessType(){
		return accessType;
	}
	
	public String getaccessList(){
		return accessList;
	}
	
	public String toString(){
		SimpleDateFormat sdf = new SimpleDateFormat("MM-dd-yyyy HH:mm:ss");
		String s_date = sdf.format(this.startDate);
		String e_date = sdf.format(this.endDate);
		
		String event = "\n start Date: "+s_date +"\n"
				     + "end Date: "+e_date+"\n"
				     + "text: "+text+"\n"
				     + "EventType "+accessType+"\n"
				     + "users: "+accessList +"\n"
					 + "EventId(for delete event) "+eventId +"\n";
		return event;
	}
	
	/*
	 * return how soon an event will begin
	 */
	public long timeToBegin(){
		return this.startDate.getTime() - System.currentTimeMillis();
	}

	

	//define the order for comingEvent queue
	@Override
	public int compareTo(Event e) {
		// TODO Auto-generated method stub
		return this.startDate.compareTo(e.startDate);
		
	}
	
	// =======================================================================================
	
	
	

}
