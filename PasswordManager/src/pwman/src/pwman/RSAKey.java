package pwman;

import java.nio.ByteBuffer;
import java.security.KeyPair;
import java.security.KeyPairGenerator;
import java.security.NoSuchAlgorithmException;
import java.security.PrivateKey;
import java.security.PublicKey;

import javax.crypto.Cipher;

public class RSAKey {
	
	private static RSAKey instance;
	public  String xform;
	public  PublicKey pubk;
	public  PrivateKey prvk;
	
	public  RSAKey()
	{		
		KeyPairGenerator kpg;
		try {
			kpg = KeyPairGenerator.getInstance("RSA");
			kpg.initialize(2048); // 512 is the keysize.
		    KeyPair kp = kpg.generateKeyPair();
		    pubk = kp.getPublic();
		    prvk = kp.getPrivate();
		    xform = "RSA";

		} catch (NoSuchAlgorithmException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	  
	}
	
	public static RSAKey getInstance() {
		if(instance == null) instance = new RSAKey();
		return instance;
	}
	
	
	 //xin
	  /*
	   * encrypt message with the public key of server
	   */
	  public  byte[] RSAencrypt(byte[] inpBytes) throws Exception {
		    Cipher cipher = Cipher.getInstance(xform);
		    cipher.init(Cipher.ENCRYPT_MODE, pubk);
		    return cipher.doFinal(inpBytes);
//		    ByteBuffer bi = ByteBuffer.wrap(inpBytes);
//		    ByteBuffer bo = ByteBuffer.allocate(65536);
//		    cipher.doFinal(bi, bo);
//		    byte [] output =  bo.array();
//		    System.out.println("RSAencrypt input len = " + inpBytes + " output len = " + output.length );
//		    return output;
		  }
	
	  public  byte[] RSAdecrypt(byte[] inpBytes) throws Exception{
		    Cipher cipher = Cipher.getInstance(xform);
		    cipher.init(Cipher.DECRYPT_MODE, prvk);
		    return cipher.doFinal(inpBytes);
//		    ByteBuffer bi = ByteBuffer.wrap(inpBytes);
//		    ByteBuffer bo = ByteBuffer.allocate(65536);
//		    cipher.doFinal(bi, bo);
//		    byte [] output =  bo.array();
//		    System.out.println("RSAdecrypt input len = " + inpBytes + " output len = " + output.length );
		  //  return output;
		  }

}
