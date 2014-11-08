package util;



import java.util.ArrayList;
import java.util.Collections;
import java.util.Comparator;

import DHash.Hashing;


public  class Order {
	
	public static class IncreaseComparator  implements Comparator<String>{

		 @Override
		    public int compare(String o1, String o2) {
			 	if (o1.equals(o2)) return 0;
			 	if (Hashing.hash(o1) == Hashing.hash(o2)) return 0;
			 	
		        if  (Hashing.hash(o1) > Hashing.hash(o2))
		        	return 1;
		        else return -1;		        
		    }
	}
	

	
 	public static void main(String[] args)
	{
		String[] user = "Bob,Sandy".split(",");
		sortClient(user);
	}
	
	public static String[] sortClient(String[] users)
	{
		Log.log("-------------------Visit clients in order--------------------------", Log.demo);
		ArrayList<String> sortedUsers = new ArrayList<String>();
		
		for(String u: users)
		{
			sortedUsers.add(u);
		}
		Collections.sort(sortedUsers, new Order.IncreaseComparator());	
		
		int length = sortedUsers.size();
		String[] rnt = new String[length];
		for(int i=0; i<sortedUsers.size(); i++)
		{
			rnt[i] = sortedUsers.get(i);
			Log.log(rnt[i] , Log.test);
		}
		return rnt;
	}

}
