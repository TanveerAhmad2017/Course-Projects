package pwman;


import java.security.KeyPair;
import java.security.KeyPairGenerator;
import java.security.PrivateKey;
import java.security.PublicKey;
import java.util.*;
import java.util.concurrent.*;
import java.io.*;
import java.net.*;

import pwman.SecureBlobIO.ClientParams;
import pwman.SecureBlobIO.ServerParams;


/** The test client. */
public class Test {
  public static void fail(String message) {
    System.err.println("*** FAIL ***");
    System.err.println(message);
    System.exit(1);
  }

  public static void should_equal(String a, String b) throws Exception {
    if (a != b && (a == null || !a.equals(b))) {
      System.err.println("*** FAIL: objects not equal ***");
      System.err.println(a);
      System.err.println(b);
      throw new Exception("Failed"); /* stack trace */
    }
  }

  public static void note(String message) {
    System.out.println(message);
  }

  public static SecureBlobIO connect(final SecureBlobIO.ClientParams cp,
				     final SecureBlobIO.ServerParams sp,
				     final Map<byte[], byte[]> m)
    throws IOException, CorruptMessageException
  {
    final PipedOutputStream outA = new PipedOutputStream();
    final PipedInputStream  inA  = new PipedInputStream(outA);
    final PipedOutputStream outB = new PipedOutputStream();
    final PipedInputStream  inB  = new PipedInputStream(outB);

    new Thread() {
      public void run() {
		try {
		  new NetworkedMapServerThread
		    (new SecureBlobIO(new IOBlobIO(inA, outB), sp), m).run();
		} catch (Exception e) {
		  Debug.warn(e);
		}
	
	      }
	    }.start();

    return new SecureBlobIO(new IOBlobIO(inB, outA), cp);
  }

  public static void main(String[] args) throws Exception {
    try {
    	
    	
        
        
      Map <byte[], byte[]> m = new FileMap("test");
      //uncomment to keep the files in the disk
      m.clear();
      
      note("Setting up the connection...");
    
     
      //run two times, using different constructor, to test whether server data is stored right or not
      note("Launch server...");
      SecureBlobIO.ServerParams sp =
	new SecureBlobIO.ServerParams(new File("Device Key"), "passw0rd");
      
      SecureBlobIO.ClientParams cp =
	new SecureBlobIO.ClientParams("passw0rd");
      note("");
      note("Client 1 try to connect server...");
      SecureBlobIO one = connect(cp, sp, m);
      StringMap m1 = new StringMap(new EncryptedMap(new NetworkedMap(one),
						    cp.getMaster()));
      
      
      
      SecureBlobIO.ClientParams cp2 = new SecureBlobIO.ClientParams("passw0rd");
      note("");
      note("Client 2 try to connect server...");
      SecureBlobIO two = connect(cp2, sp, m);
      StringMap m2 = new StringMap(new EncryptedMap(new NetworkedMap(two),
						    cp2.getMaster()));
      
      note("");
      note("Basic testing");
      m1.put("a is for",      "alice");
      
      should_equal(m1.get("a is for"), "alice");
      
 
      m2.put("b is for",      "bob");
  
      m1.put("c is for",      "carol");
      m2.put("who works for", "the mob");
      
    
      
      should_equal(m2.get("b is for"), "bob");
      m1.remove("c is for"); /* busted! */
      should_equal(m1.get("b is for"), "bob");
      should_equal(m1.get("c is for"), null);
      should_equal(m2.get("c is for"), null);
  
      note("Client 2 Changing the password");
      
      two.setPassword("passw1rd", false);
      
      SecureBlobIO.ClientParams cp3 = new SecureBlobIO.ClientParams("passw1rd");
      note("");
      note("Client 3 try to connect server...");
      SecureBlobIO three = connect(cp3, sp, m);
      StringMap m3 = new StringMap(new EncryptedMap(new NetworkedMap(three),
						    cp3.getMaster()));
      
      note("");
      note("More tests...");
      m3.put("d is for",          "david");
      m2.put("he doesn't feel",   "fear");
      m1.put("his love notes to", "erin");
      
      note("Test replay attack...");
      one.testingReplayAttack();
      m1.put("are sent in the",   "clear");
      
      should_equal(m1.get("he doesn't feel"), "fear");
      
      m3.put("he doesn't feel",   "pain");
      m3.put("are sent in the",   "plain");
      
      should_equal(m2.get("he doesn't feel"), "pain");
      note("");
      note("Checking wrong password");
     
      note("Client 4 try to connect server with outdata device-key...");
      SecureBlobIO.ClientParams cp4 =
    		  new SecureBlobIO.ClientParams("passw0rd");
      try {
			connect(cp4, sp, m);
			fail("You shouldn't be able to log in with the wrong password");
      } catch (CorruptMessageException e) {
		 	note("Wrong device-key throws CorruptMessageException");
	  } catch (IOException e) {
			  note("Wrong device-key throws IOException");
	  }
      note("");
      note("cleaning up");
      m1.clear();
      should_equal(m2.get("he doesn't feel"), null);

      one.close();
      two.close();
      three.close();
      note("*** PASS ***");
    } catch(Exception e) {
      Debug.choke_on(e);
    }
    System.exit(0);
  }
}
