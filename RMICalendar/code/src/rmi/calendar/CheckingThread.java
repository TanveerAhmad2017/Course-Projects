package rmi.calendar;

import java.rmi.RemoteException;

import util.Log;

public class CheckingThread implements Runnable {

	private CalendarManager _manager;
	
	//constructor
	public CheckingThread(CalendarManager manager)
	{
		_manager = manager;
	}
	
	public void repeteCheck()
	{
		try{
			_manager.checkPredecessor();
		}catch(Exception e)
		{
			Log.log("CheckPredecessor caught, Predecessor is not alive", Log.demo);
			_manager.handlePredecessorDown();
		}
		
		try{
			_manager.checkSuccessor();
		}catch(Exception e)
		{
			Log.log("checkSuccessor caught, successor is not alive", Log.demo);
			_manager.handleSuccessorDown();
		}	
	}
	@Override
	public void run() {
		// TODO Auto-generated method stub
		while(true)
		{
			
			try {
				Thread.sleep(3000); //wait for 5s
				//notify client periodically
				
				try{
					_manager.checkPredecessor();
				}catch(Exception e)
				{
					Log.log("CheckPredecessor caught, Predecessor is not alive", Log.demo);
					_manager.handlePredecessorDown();
				}
				
				try{
					_manager.checkSuccessor();
				}catch(Exception e)
				{
					Log.log("checkSuccessor caught, successor is not alive", Log.demo);
					_manager.handleSuccessorDown();
				}	
				
				
				_manager.showFingerTabler();
				
				_manager.NotifyClient();
				_manager.stabilize();
				_manager.fix_fingers();
				//should be uncomment when demo
				_manager.fix_fingers_byOther();
				_manager.updateSOSandPOP();
				_manager.BackupData();
				
				_manager.showFingerTabler();
				
			} catch (RemoteException e) {
				// TODO Auto-generated catch block
				
				Log.log("Thread catched,server is down", Log.demo);
				//e.printStackTrace();
				repeteCheck();
			}catch(InterruptedException e)
			{
				//e.printStackTrace();
				repeteCheck();
				
			}
		}
	}

}
