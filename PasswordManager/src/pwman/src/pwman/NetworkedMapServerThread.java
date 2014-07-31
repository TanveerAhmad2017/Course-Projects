package pwman;


import java.security.PrivateKey;
import java.security.PublicKey;
import java.util.*;
import java.nio.charset.Charset;
import java.util.Map.Entry;
import java.util.AbstractMap.SimpleImmutableEntry;
import java.io.*;

import javax.crypto.Cipher;

import pwman.Debug;

/**
 * Networked map server thread.  This class implements a single thread
 * of the back-end for the networked map object, and is responsible
 * for communication with a signle client.
 * <p>
 * You shouldn't need to change anything in this class for the cs255
 * programming project.
 */
public class NetworkedMapServerThread extends Thread {
  private final SecureBlobIO socket;
  private final Map<byte[], byte[]> backer;

  /**
   * Create a new networked map.
   *
   * @param socket the secure socket to receive the commands.
   * @param backer the map which underlies the server.
   */
  public NetworkedMapServerThread(SecureBlobIO socket,
				  Map<byte[], byte[]> backer) {
    this.socket = socket;
    this.backer = backer;
  }

  /**
   * Send a response with one element, unless it's null, in which
   * case, send a response with zero elements.
   */
  private void respond(byte[] res) throws IOException {
    if (res != null) {
      byte[][] response = { res };
      socket.writes(response);
    } else {
      byte[][] response = {};
      socket.writes(response);
    }    
  }

  /**
   * Run the server in the current thread.  Use start() to run it in a
   * new thread.
   */
  public void run() {
    try {
      while(true) {
	byte[][] command = socket.reads();

	/* sanity checks */
	if (command.length == 0 || command[0].length == 0)
	  throw new CorruptMessageException("empty command");

	/* Back in my day, we didn't have pattern matching.  We had to
	 * do everything with switch statements full of sanity
	 * checking...
	 */
	switch (command[0][0]) {
	case NetworkedMap.TAG_ENTRY_SET:
	  { /* for scoping */
	    Set<Entry<byte[],byte[]>> s = backer.entrySet();
	    byte[][] response = new byte[2*s.size()][];
	    int i = 0;
	    for (Entry<byte[],byte[]> e : s) {
	      response[2*i  ] = e.getKey();
	      response[2*i+1] = e.getValue();
	      i++;
	    }
	    socket.writes(response);
	  }
	  break;

	case NetworkedMap.TAG_KEY_SET:
	  { /* for scoping */
	    Set<byte[]> s = backer.keySet();
	    byte[][] response = new byte[s.size()][];
	    int i = 0;
	    for (byte[] k : s) {
	      response[i] = k;
	      i++;
	    }
	    socket.writes(response);
	  }
	  break;

	case NetworkedMap.TAG_GET:
	  if (command.length != 2)
	    throw new CorruptMessageException("invalid GET command");
	  respond(backer.get(command[1]));
	  break;

	case NetworkedMap.TAG_PUT:
	  if (command.length != 3)
	    throw new CorruptMessageException("invalid PUT command");
	  respond(backer.put(command[1], command[2]));
	  break;

	case NetworkedMap.TAG_REMOVE:
	  if (command.length != 2)
	    throw new CorruptMessageException("invalid REMOVE command");
	  respond(backer.remove(command[1]));
	  break;

	case NetworkedMap.TAG_CLEAR:
	  if (command.length != 1)
	    throw new CorruptMessageException("invalid CLEAR command");
	  backer.clear();
	  respond(null);
	  break;
	
	/*server deal with authentication*/
	case NetworkedMap.TAG_SHAKE:
		if (command.length != 2)
			throw new CorruptMessageException("invalid CLEAR command");
		String clientPassword = new String(command[1], "UTF-8");
		System.out.println("[Server] authenticating clients, clients device key is: "+clientPassword);
		if(socket.sp.deviceKey.equals(Util.toSHA1(clientPassword)))
			respond("true".getBytes());
		else
			respond("false".getBytes());
		break;
	case NetworkedMap.TAG_CHANGE_PWD:
		if (command.length != 2)
			throw new CorruptMessageException("invalid CHANGE_PWD command");
		String newPwd = new String(command[1], "UTF-8");
		socket.sp.deviceKey = Util.toSHA1(newPwd);
	    respond("Success".getBytes());
		break;
	/*deal with negotiating message*/
	case NetworkedMap.TAG_NEGO:
		if (command.length != 5)
			throw new CorruptMessageException("invalid NEGO command");
	    String pwd =new String(RSAKey.getInstance().RSAdecrypt(command[1]),"UTF-8");
	    byte[] salt = RSAKey.getInstance().RSAdecrypt(command[2]);
	    byte[] ivBytes = RSAKey.getInstance().RSAdecrypt(command[3]);
	    byte[] hmacString = RSAKey.getInstance().RSAdecrypt(command[4]);
	    socket.aes = new Aes(pwd,salt,ivBytes,hmacString);
	    System.out.println("[Server] processing negotiating packets, \"HASHCODE\" of pwd = " + pwd + " salt = " + salt.hashCode() + " ivBytes = " + ivBytes.hashCode());
	    socket.negotiated = true;
			//socket.commuKey = RSAKey.getInstance().RSAdecrypt(command[1]);		  
	    respond("true".getBytes());
		break;
	//TODO remove me, for test only
	case NetworkedMap.TAG_MY_KEY:
		if (command.length != 1)
			throw new CorruptMessageException("invalid MY_KEY command");
	    respond(this.socket.aes.COMM_PWD.getBytes());
		break;
		
	default:
	  throw new CorruptMessageException("unknown command");
	}
      }
    } catch (EOFException e) {
    } catch (Exception e) {
      /* we aren't allowed to throw here */
      Debug.warn(e);
    } finally {
      //socket.close();
    }
  }
}
