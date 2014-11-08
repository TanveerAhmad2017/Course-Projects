package rmi.calendar;

import DHash.Hashing;
import util.Log;
import util.config;


public class FingerTable {
	
	final static long intChordSize = config.chordSize;
    final static int tableSize = config.keySize;
	
   public  Finger[] table = new Finger[tableSize];
	/*
	 * finger table 
	 */
	public class Finger{
		
		int start;
		int end;
		int successorId;
		String successorIP;
		
		public boolean isContained(int tid)
		{
			if( (start<=tid) && (tid < end))
				return true;
			return false;
			
		}
	}
	
	public Finger getFinger(int index)
	{
		return this.table[index];
	}
	
	public synchronized void setFinger(int index,String ip)
	{
		this.table[index].successorIP = ip;
		int id = Hashing.hash(ip);
		this.table[index].successorId = id;
	}
	
	
	 public void initFingerTable(int intNodeID)
	 {
		    
	        for(int k = 1; k <= tableSize; k++)
	        {
	            table[k-1] = new Finger();
	            int s = ( intNodeID +  (int)Math.pow(2, k-1) ) % (int)Math.pow(2, tableSize);
	            int e = ( intNodeID +  (int)Math.pow(2, k) ) % (int)Math.pow(2, tableSize);
//
//	            table[k-1].start = s>0? s : s+(int)Math.pow(2, tableSize);
//	            table[k-1].end = e>0 ? s : e + (int)Math.pow(2, tableSize);
	            
	            table[k-1].start = s;
	            table[k-1].end = e;
	            
	            //should be update to set the write successor
	            table[k-1].successorId = intNodeID;
	        }
	        
	  }
	 
	 

	 
	 
	 
	 public void show()
	 {
	        Log.log("\t Start \t End \t Successor",Log.demo);
	        Log.log("",Log.demo);
	        
	        for(int k = 1; k <= tableSize; k++)
	        {
	        	Log.log( k+" \t" + table[k-1].start +" \t "  +
	                    table[k-1].end + " \t " + table[k-1].successorId ,Log.demo);
	        }
	  }

}
