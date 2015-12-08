package pwman;


import java.util.*;
import java.io.*;
import java.lang.reflect.Array;
import java.math.BigInteger;
import java.net.*;
import java.nio.ByteBuffer;

import javax.crypto.*;

import com.sun.corba.se.spi.activation.Server;

import java.security.*;
import java.security.spec.InvalidParameterSpecException;

public class SecureBlobIO extends BlobIO {
  private final BlobIO io;
  
  //only one of sp/cp will be null,depends on wehther it is server or client
  public final ServerParams sp;
  private final ClientParams cp;
  
  //indicate about whether is negotiating or not
  //we need to know whether it is negotiating or not, because negotiating message will encrypt using server's public key
  public boolean negotiated = false;
  
  //after negotiating the key, server and client will use this aes to encrypt and decrypt communication packets
  public Aes aes;
  
  // coonection will be closed when nonce exceeds maximum value
  private final int MAX_NONCE = Integer.MAX_VALUE;
  private static int cur_nonce = 1;
  
  /* hash set to keep track of used nonce, message with nonce already used will be dropped*/
  private  static HashSet<Integer> hs = new HashSet<Integer>();;
  
  /** parameters for the server side of a connection */
  public static class ServerParams {
	 
	public String deviceKey;
    /** read from a file */
    public ServerParams(File f)
      throws IOException,
	     FileNotFoundException,
	     CorruptMessageException
    {
      /* !!! write me */
    	// load password from file
    	FileBlobIO fileBlobIO = new FileBlobIO(f);
    	
    	//read encrpt password from file
    	byte[] pwd = fileBlobIO.read();
    	Aes aes = new Aes("server_store","server_store");
    	try {
			pwd = aes.decrypt(pwd);
		} catch (InvalidKeyException | NoSuchAlgorithmException
				| NoSuchPaddingException | InvalidAlgorithmParameterException
				| IllegalBlockSizeException | BadPaddingException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
    	this.deviceKey = new String(pwd, "UTF-8");
    	
    	System.out.println("[Server] load deviceKey from " + f.getName() + " deviceKey = '" + this.deviceKey + "'");
    }
    
    /** generate a new and write to a file */
    public ServerParams(File f, String password)
      throws IOException,
	     FileNotFoundException
    {
    	/* !!! write me */
    	
    	//password is stored in sha1
    	this.deviceKey = Util.toSHA1(password);
    
    	
    	System.out.println("[Server]  set hashed deviceKey to '" + this.deviceKey + "'");
    	
    	if(f != null)
    	{
	    	FileBlobIO fileBlobIO = new FileBlobIO(f);
	    	/* key server  used to ecnrypt data*/
	    	Aes aes = new Aes("server_store","server_store");
	    	try {
	    		//encrpt user deviceKey
				fileBlobIO.write(aes.encrypt(password.getBytes()));
			} catch (InvalidKeyException | NoSuchAlgorithmException
					| NoSuchPaddingException
					| InvalidAlgorithmParameterException
					| IllegalBlockSizeException | BadPaddingException
					| InvalidParameterSpecException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
	    	
	    	System.out.println("[Server] save deviceKey to " + f.getName());
    	}
    	    
    	write();
    }
    
    

    /** write out to a file */
    //write resource key pair to file
    public void write() throws IOException, FileNotFoundException {
      /* !!! write me */
    	//FileBlobIO fileBlobIO = new FileBlobIO(new File("ResourceKeyPair"));
    }
    
    
  }

  /** parameters for the client side of a connection */
  public static class ClientParams {
	  public String deviceKey;
      /* initialize with a new password */
      public ClientParams(String password) {
      /* !!! write me */
    	this.deviceKey = password;
    }

    /* get the master key; only available after handshake */
    public Aes getMaster() {
		  /* !!! write me */
		
		  /* !!! dummy code, so compiler won't complain     */
		  /* !!! remove for PP1 Milestone #2 implementation */
    	
		  //xin
    	//use static to ensure each client encrypt the resource in the same way
    	return Aes.getInstance();
    }
    
  }

  /** Create a new secure socket on the client side. */
  public SecureBlobIO(BlobIO io,
		      ClientParams params)
    throws IOException,
	   CorruptMessageException
  {
	  this.io = io;
	  this.cp = params;
	  this.sp = null;
	
	  
	  
	
	try {
		this.Negotiate();
	} catch (CorruptMessageException e){
		throw e;
	} catch (Exception e) {
		e.printStackTrace();
	}
	
	 // this.GetSharedKey();
	  this.Shake();
	  
	  
	  
	//xin: judge params to decide whether give IO or not
	//test autentication
	  
    /* !!! write me */
	//io.writes(xs);

    /* !!! dummy code, so compiler won't complain     */
    /* !!! remove for PP1 Milestone #2 implementation */
    
  }
  
  //TODO nigo
  private void Negotiate() throws Exception {
	  
	  /*generate symmetric key*/
	  this.aes = new Aes(Aes.COMM_PWD,"commukey");
	 
	   
	 //send key to server
	  byte[][] arr = {{NetworkedMap.TAG_NEGO}, RSAKey.getInstance().RSAencrypt(aes.COMM_PWD.getBytes()),
			  RSAKey.getInstance().RSAencrypt(aes.mSalt), RSAKey.getInstance().RSAencrypt(aes.ivBytes),RSAKey.getInstance().RSAencrypt(aes.hmac.key)};
	  
	 
      System.out.println("[Client] Negotiating with server...send encryptkey & hmacKey");
	  
	  this.writes(arr);
	  
	  this.negotiated = true;
	  
	 // this.write();
	  byte[][] x = this.reads();
	  String msg = new String(x[0], "UTF-8");
	  
      System.out.println("Negotiating success...");
	  
	  if(!msg.equals("true")) {
		  throw new CorruptMessageException("failed to negotiate");
	  }
	
  }
  
 
  

  
  //test whether the symmetric key the server generated using the string given by client is right or not 
  public void GetSharedKey() throws IOException, CorruptMessageException {
	  
	  byte[][] arr = {{NetworkedMap.TAG_MY_KEY}};
	  
	  System.out.println("[Client] Request key");
	  
	  this.writes(arr);
	  
	 // this.write();
	  byte[][] x = this.reads();
	  byte[] key = x[0];
	  
	  //System.out.println("Server responsd: " + key.hashCode() + " my key : " + this.cp.key.hashCode());
	  
	  if(!Arrays.equals(key, this.aes.COMM_PWD.getBytes()))
	  {
		  System.err.println("[Client] bad key");
		 // throw new CorruptMessageException("bad key");
	  }
	  else
	  {
		  System.err.println("[Client] key ok");
	  }
  }
  
  //TODO: shake 
  private void Shake() throws IOException, CorruptMessageException {
	  byte[][] arr = {{NetworkedMap.TAG_SHAKE}, this.cp.deviceKey.getBytes()};
	  
	  
	  System.out.println("[Client] Authenticate himself to server by sending Device key: '" + this.cp.deviceKey + "'");
	  
	  this.writes(arr);
	  
	 // this.write();
	  byte[][] x = this.reads();
	  String msg = new String(x[0], "UTF-8");
	  
	  //System.out.println("[Server] responsd: " + msg);
	  
	  if(!msg.equals("true")) {
		  throw new CorruptMessageException("Authentication failed...Wrong device-key!");
	  }else{
		  System.out.println("Authentication success...");
	  }
  }

  /** Create a new secure socket on the server side. */
  public SecureBlobIO(BlobIO io,
		      ServerParams params)
    throws IOException,
	   CorruptMessageException
  {
    /* !!! write me */

    /* !!! dummy code, so compiler won't complain     */
    /* !!! remove for PP1 Milestone #2 implementation */
    this.io = io;
    this.cp = null;
	this.sp = params;
  }

 
  
  
  /** Marshal, mac and send a packet of data */
  public synchronized void write(byte[] data)
    throws IOException
  {
    /* !!! write me */
	  //xin
	  
	  /*if not negotiated, then using server's public key to encrypt message, this is done in the upper layer
	   * 
	   * */
	  if(!negotiated) 
	  {
			this.io.write(this.appendNounce(data));
		} else {
			byte[] encrypted = null;
			try {
				
				//a new IV is used each time
				//marshall data 
				byte[] iv = Aes.random_block();
				aes.ivBytes = iv;
				encrypted = this.appendIV(this.aes.encrypt(this.aes.hmac.mac(this.appendNounce(data))), iv);
			
			} catch (InvalidKeyException | NoSuchAlgorithmException
					| NoSuchPaddingException
					| InvalidAlgorithmParameterException
					| IllegalBlockSizeException | BadPaddingException
					| InvalidParameterSpecException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			this.io.write(encrypted);
		}
  }

  public synchronized void setPassword(String password, boolean clean)
    throws IOException
  {
    /* 
     * If the clean parameter is set, change the master key as well.
     * Currently, nothing calls this with clean, so you can ignore
     * this.
     */

    /* !!! write me */
	  if(clean == false)
	  {
		  byte[][] arr = {{NetworkedMap.TAG_CHANGE_PWD}, password.getBytes()};
		  
		  
		  System.out.println("[Client] try to change  password to '" + password);
		  
		  this.writes(arr);
		  
		 // this.write();
		  byte[][] x;
		try {
			x = this.reads();
			 String msg = new String(x[0], "UTF-8");	  
			  System.out.println("[Server] responsd: " + msg);
		} catch (CorruptMessageException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		 
		//client need to change password
		  		
	  }
		  
  }

  /** Receive, check and unmarshal a packet of data */
  //decrypt
  public synchronized byte[] read()
    throws IOException, CorruptMessageException
  {
    /* !!! write me */
    /* !!! dummy code, so compiler won't complain     */
    /* !!! remove for PP1 Milestone #2 implementation */
    //return null;
	 
	  /* the inverse procedure with write()*/
	  byte[] decrypted = null;
	  byte[] original = this.io.read();
	  if(!negotiated)  return this.removeNonce(original);
	  else
	  {
		  try {
			//get IV
			byte[] removedIV = this.removeIV(original, this.aes);
			//unmarshall others
			decrypted = this.removeNonce(this.aes.hmac.unmac(this.aes.decrypt(removedIV)));
			
		} catch (InvalidKeyException | NoSuchAlgorithmException
				| NoSuchPaddingException | InvalidAlgorithmParameterException
				| IllegalBlockSizeException | BadPaddingException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	  }
	 return decrypted;
  }

  public void close() {
    io.close();
  }
  
 /*
  * add nonce to message to resist reply attack
  */
  public  static byte[] appendNounce(byte[] data)
  {
		byte[] nonce = new byte[4];
		nonce = Util.IntgetBytes(cur_nonce);

//		printByteArray(nonce);
		byte[] combined = new byte[data.length + nonce.length];

		System.arraycopy(data, 0, combined, 0, data.length);
		System.arraycopy(nonce, 0, combined, data.length, nonce.length);
		return combined;
	   
  }
  

  /*
   * remove nonce to from message
   */
  public  static byte[] removeNonce(byte[] data)
  {
	    byte[] tmp = new byte[data.length-4];
        byte[] getNonce = new byte[4];
	    System.arraycopy(data,0,tmp,0 ,data.length-4);
	    System.arraycopy(data,data.length-4,getNonce,0 ,4);
	    
	    int nonce = new BigInteger(getNonce).intValue();
	    cur_nonce++;
	    if(hs.contains(nonce))	 
	    {
	    	System.err.println("Replay attack!");
	    }else
	    {
	    	hs.add(nonce);
	    }

	    return tmp;
	    
  }
  
  /*
   * append initial block in each message send
   */
  public  static byte[] appendIV(byte[] data,byte[] iv)
  {
	

		byte[] combined = new byte[data.length + iv.length];

		System.arraycopy(data, 0, combined, 0, data.length);
		System.arraycopy(iv, 0, combined, data.length, iv.length);
		return combined;	   
  }
  
  //get IV to from message
  public  static byte[] removeIV(byte[] data, Aes aes)
  {
	    byte[] tmp = new byte[data.length-16];
        byte[] iv = new byte[16];
	    System.arraycopy(data,0,tmp,0 ,data.length-16);
	    System.arraycopy(data,data.length-16,iv,0 ,16);	    
	    aes.ivBytes = iv;
	    return tmp;	    
  }
  
  
  /* for test only */
  
//  private void printTag() {
//	  if(this.cp != null) 
//		  System.out.print("[CLIENT] ");
//	  else
//		  System.out.print("[SERVER] ");
//  }
  
  private static void printByteArray(byte[] msg){
	  for(int i=0;i<msg.length;i++)
	  {
		  System.out.print(msg[i] + " ");
	  }
	  
	  System.out.println();
  }
  
  /*
   * provide for test only
   */
  public static void testingReplayAttack()
  {
	  cur_nonce--;
  }
  
  

  
}
