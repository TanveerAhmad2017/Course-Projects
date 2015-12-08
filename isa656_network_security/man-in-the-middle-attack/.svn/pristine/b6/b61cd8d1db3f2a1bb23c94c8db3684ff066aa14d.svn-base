package mitm;

import java.util.Arrays;
import java.nio.charset.Charset;

import javax.crypto.*;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.PBEKeySpec;
import javax.crypto.spec.SecretKeySpec;



//import javax.security.auth.kerberos.KeyTab;

import java.security.*;
import java.security.spec.InvalidKeySpecException;
import java.security.spec.InvalidParameterSpecException;
import java.security.spec.KeySpec;

/**
 * Utility class; provides easy-to-use, statically-safe wrapper around
 * authenticated AES/CTR in Java crypto library.
 */
public final class Aes {
  
  //original
  //public final Hmac hmac;
  public Hmac hmac;
  
  //xin
  public final static int SALT_LEN = 8;
  /*String used to generate communication password, this can be a variable*/
  public final static String COMM_PWD = "COMM_PWD";
  /*AES key*/
  public SecretKey key;
  /*initial block*/
  public byte[] ivBytes;
  public IvParameterSpec ivSpec;
  /*salt*/
  public byte[] mSalt;
 
 
  /**
   * Construct an AES key from a password.
 * @throws NoSuchAlgorithmException 
 * @throws InvalidKeySpecException 
   */
  //usage;
  //Aes("server_store","server_store"), drive the key for server to encrypt data stored in disk
  //Aes("Resource_PASSWORD","resourceKeyEncrypt"), drive the key for client to encrypt resource-key pair
  public Aes(String password) {
    
	
	 
	
	//xin
	mSalt = new byte[SALT_LEN];
	mSalt="Client_or_Server".getBytes();
	SecureRandom rnd = new SecureRandom();
	ivBytes = new byte[16];
	ivBytes = new byte[] { (byte) 0x69, (byte) 0xdd, (byte) 0xa8,
            (byte) 0x45, (byte) 0x5c, (byte) 0x7d, (byte) 0xd4,
            (byte) 0x25, (byte) 0x4b, (byte) 0xf3, (byte) 0x53,
            (byte) 0xb7, (byte) 0x73, (byte) 0x30, (byte) 0x4e, (byte) 0xec };
	ivSpec = new IvParameterSpec(ivBytes);	
	

	
	
	/*generate Aes key*/
	SecretKeyFactory factory;
	
	try {
		factory = SecretKeyFactory.getInstance("PBKDF2WithHmacSHA1");
		KeySpec spec = new PBEKeySpec(password.toCharArray(), mSalt, 65536, 128);
		key = new SecretKeySpec(factory.generateSecret(spec).getEncoded(), "AES");
	
	} catch (NoSuchAlgorithmException e) {
		// TODO Auto-generated catch block
		e.printStackTrace();
	} catch (InvalidKeySpecException e) {
		// TODO Auto-generated catch block
		e.printStackTrace();
	}	

    hmac = new Hmac(password.getBytes());    
  }
  
  
 
  /**
   * Encrypt a message with AES.
   *
   * @param message the message to encrypt
   * @return the ciphertext
 * @throws NoSuchPaddingException 
 * @throws NoSuchAlgorithmException 
 * @throws InvalidAlgorithmParameterException 
 * @throws InvalidKeyException 
 * @throws BadPaddingException 
 * @throws IllegalBlockSizeException 
 * @throws InvalidParameterSpecException 
   */
  public byte[] encrypt(byte[] message) throws NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException, InvalidAlgorithmParameterException, IllegalBlockSizeException, BadPaddingException, InvalidParameterSpecException {
    /* !!! write me */

    /* !!! dummy code, so compiler won't complain     */
    /* !!! remove for PP1 Milestone #2 implementation */
	
	    
	/* Encrypt the message. */
	Cipher cipher = Cipher.getInstance("AES/CTR/NoPadding");
	cipher.init(Cipher.ENCRYPT_MODE, this.key, this.ivSpec);
	byte[] ciphered = cipher.doFinal(message);
		
	return ciphered;
  }

  /**
   * Decrypt a message with AES.
   *
   * @param ciphertext the ciphertext to decrypt
   * @return the plaintext
   * @throws CorruptMessageException if the ciphertext is too short to
   * have an IV
 * @throws NoSuchPaddingException 
 * @throws NoSuchAlgorithmException 
 * @throws InvalidAlgorithmParameterException 
 * @throws InvalidKeyException 
 * @throws BadPaddingException 
 * @throws IllegalBlockSizeException 
   */
  public byte[] decrypt(byte[] message_)
    throws  NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException, InvalidAlgorithmParameterException, IllegalBlockSizeException, BadPaddingException
  {
    /* !!! write me */

    /* !!! dummy code, so compiler won't complain     */
    /* !!! remove for PP1 Milestone #2 implementation */
	  
	/*decrypt message*/
	Cipher cipher; 
	byte[] plain = null;
	
	cipher= Cipher.getInstance("AES/CTR/NoPadding");
	cipher.init(Cipher.DECRYPT_MODE, key, ivSpec);
	plain = cipher.doFinal(message_);
	
	return plain;

  }

  /**
   * Decrypt a message with AES, returning null if an error occurs.
   *
   * @param ciphertext the ciphertext to decrypt
   * @return the plaintext, or null if it doesn't decrypt cleanly.
 * @throws InvalidAlgorithmParameterException 
 * @throws NoSuchPaddingException 
 * @throws NoSuchAlgorithmException 
 * @throws InvalidKeyException 
 * @throws BadPaddingException 
 * @throws IllegalBlockSizeException 
   */
  public byte[] tryDecrypt(byte[] ciphertext) throws InvalidKeyException, NoSuchAlgorithmException, NoSuchPaddingException, InvalidAlgorithmParameterException, IllegalBlockSizeException, BadPaddingException {
    if (ciphertext == null) return null;
    try {
      return decrypt(ciphertext);
    } catch (Exception e) {
      //Debug.warn(e);
      return null;
    }
  }

 
}
