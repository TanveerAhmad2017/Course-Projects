package rmi.calendar;

import java.rmi.RemoteException;

public class LoadBalance implements Runnable {

	private CalendarManager _manager;
	
	//constructor
	public LoadBalance(CalendarManager manager)
	{
		_manager = manager;
	}
	
	@Override
	public void run() {
		// TODO Auto-generated method stub
		while(true)
		{
			
			try {
				Thread.sleep(10000); //wait for 5s
				//notify client periodically
				
				_manager.LoadBalancing();
//				_manager.showFingerTabler();
				
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				//e.printStackTrace();
			} catch (RemoteException e) {
				// TODO Auto-generated catch block
				//e.printStackTrace();
			}
		}
	}

}
