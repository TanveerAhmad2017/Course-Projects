package pwman;

import java.util.Arrays;
import java.nio.charset.Charset;

import javax.crypto.*;
import javax.crypto.spec.IvParameterSpec;
import javax.crypto.spec.PBEKeySpec;
import javax.crypto.spec.SecretKeySpec;



import javax.security.auth.kerberos.KeyTab;

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
  /*String used to generate resource-key, we can used master-password, but in this case, the string needed to be store
   * in the disk*/
  public final static String pwd_ForDriveResourceKey =  "Resource_PASSWORD";
  /*String used to generate the key servers used to encrypt hashed resource key*/
  public final static String server_store =  "server_store";
  
 
  private static Aes instance;
  
  
  /**
   * for connection authenticated using the same key, they use the same AES instance to encrypt resource key
   */
  public static synchronized Aes getInstance()
  {
	  if(instance==null)
	  {
		  //get key for resource
		  instance = new Aes(pwd_ForDriveResourceKey,"resourceKeyEncrypt");
	  }
	  return instance;
  }
  
  /**
   * Constructs a new instance of AES with a random key.
   */
  public Aes() {
    this(random_block());
  }

  /**
   * Construct an AES key from an array of bytes.  TODO: throw a
   * useful exception if keybytes are broken.
   */
  public Aes(byte[] key) {
    /* !!! write me */

    /* !!! dummy code, so compiler won't complain     */
    /* !!! remove for PP1 Milestone #2 implementation */
	  
	/* used to generate communication symmetric key*/
    this(key.toString(),"commuKey");
	hmac = new Hmac();
  }

  /**
   * Construct an AES key from a password.
 * @throws NoSuchAlgorithmException 
 * @throws InvalidKeySpecException 
   */
  //usage;
  //Aes("server_store","server_store"), drive the key for server to encrypt data stored in disk
  //Aes("Resource_PASSWORD","resourceKeyEncrypt"), drive the key for client to encrypt resource-key pair
  public Aes(String password, String type) {
    /* !!! write me */

    /* !!! dummy code, so compiler won't complain     */
    /* !!! remove for PP1 Milestone #2 implementation */
	
	 
	
	//xin
	mSalt = new byte[SALT_LEN];
	SecureRandom rnd = new SecureRandom();
	/* For the following two cases, the mSalt should be keep the same to keep the key generated exactly the same
	 * Case1: generate key to encrypt resource key
	 * Case2: generate key for server to encrypt data(device-key and other) 
	 * */
	if(type.equals("resourceKeyEncrypt") || type.equals("server_store"))
	{
		mSalt="Client_or_Server".getBytes();
	}else{
		rnd.nextBytes(mSalt);
	}	
	
	/*generate Aes key*/
	SecretKeyFactory factory;
	
	try {
		factory = SecretKeyFactory.getInstance("PBKDF2WithHmacSHA1");
		KeySpec spec = new PBEKeySpec(password.toCharArray(), mSalt, 65536, 128);
		key = new SecretKeySpec(factory.generateSecret(spec).getEncoded(), "AES");
		
		/* For the following two cases, the ivBytes should be keep the same to keep the key generated exactly the same
		 * Case1: generate key to encrypt resource key
		 * Case2: generate key for server to encrypt data(device-key and other) 
		 * */
		ivBytes = new byte[16];
		if(type.equals("resourceKeyEncrypt") || type.equals("server_store"))
		{
			ivBytes = new byte[] { (byte) 0x69, (byte) 0xdd, (byte) 0xa8,
		            (byte) 0x45, (byte) 0x5c, (byte) 0x7d, (byte) 0xd4,
		            (byte) 0x25, (byte) 0x4b, (byte) 0xf3, (byte) 0x53,
		            (byte) 0xb7, (byte) 0x73, (byte) 0x30, (byte) 0x4e, (byte) 0xec };
		}else{
			rnd.nextBytes(ivBytes);
		}
				 
		ivSpec = new IvParameterSpec(ivBytes);	
	
		//System.out.println("new AES pwd = " + password + " salt = " + this.mSalt.hashCode() + " ivBytes = " + this.ivBytes.hashCode());
		
	} catch (NoSuchAlgorithmException e) {
		// TODO Auto-generated catch block
		e.printStackTrace();
	} catch (InvalidKeySpecException e) {
		// TODO Auto-generated catch block
		e.printStackTrace();
	}	

    hmac = new Hmac();
    
  }
  
  /*
   * for server to get AES by parameters send from client
   * 
   */
  public Aes(String password, byte[] mSalt,byte[] ivBytes,byte[] macString) {
	    /* !!! write me */

	    /* !!! dummy code, so compiler won't complain     */
	    /* !!! remove for PP1 Milestone #2 implementation */
		
	
	  	this.ivBytes = ivBytes.clone();
	  	this.mSalt = mSalt.clone();
	  	
		SecretKeyFactory factory;		
		try {
			factory = SecretKeyFactory.getInstance("PBKDF2WithHmacSHA1");
			KeySpec spec = new PBEKeySpec(password.toCharArray(), mSalt, 65536, 128);
			key = new SecretKeySpec(factory.generateSecret(spec).getEncoded(), "AES");			 
			ivSpec = new IvParameterSpec(ivBytes);	
			//System.out.println("Degerated key: "+ key.hashCode());
			
		} catch (NoSuchAlgorithmException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		} catch (InvalidKeySpecException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}	

	    hmac = new Hmac(macString);
	    
	  }
  
  

  /** Creates a new random IV */
  public static byte[] random_block() {
    byte[] out = new byte[16];
    new SecureRandom().nextBytes(out);
    return out;
  }
  
  /*create a random block of byte of given length*/
  public static byte[] random_length_block(int len) {
	    byte[] out = new byte[len];
	    new SecureRandom().nextBytes(out);
	    return out;
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
    throws CorruptMessageException, NoSuchAlgorithmException, NoSuchPaddingException, InvalidKeyException, InvalidAlgorithmParameterException, IllegalBlockSizeException, BadPaddingException
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
    } catch (CorruptMessageException e) {
      Debug.warn(e);
      return null;
    }
  }

  //xin: haven't been used
  /**
   * Encrypt an AES key with AES.
   *
   * @param wrappee the AES instance to wrap
   * @return the wrapped key
 * @throws InvalidKeyException 
 * @throws NoSuchPaddingException 
 * @throws NoSuchAlgorithmException 
 * @throws IllegalBlockSizeException 
   */
  public byte[] wrapAes(Aes wrappee) throws InvalidKeyException, NoSuchAlgorithmException, NoSuchPaddingException, IllegalBlockSizeException {
    /* !!! write me */

    /* !!! dummy code, so compiler won't complain     */
    /* !!! remove for PP1 Milestone #2 implementation */
	Cipher cipher;
	byte[] wrappedKey = null;
	
	cipher = Cipher.getInstance("RSA");
	cipher.init(Cipher.WRAP_MODE, RSAKey.getInstance().pubk);
	wrappedKey = cipher.wrap(wrappee.key);	    
    return wrappedKey;
  }

  
  //This function is not used, the function with the same future is written in 
  //Aes(String password, byte[] mSalt,byte[] ivBytes,byte[] macString)
  /**
   * Decrypt an AES key which has been encrypted with wrapAes().
   *
   * @param wrapped the AES instance to unwrap
   * @return an AES instance initialized with the unwrapped key
 * @throws BadPaddingException 
 * @throws IllegalBlockSizeException 
 * @throws InvalidAlgorithmParameterException 
 * @throws NoSuchPaddingException 
 * @throws NoSuchAlgorithmException 
 * @throws InvalidKeyException 
   */
  public Aes unwrapAes(byte[] wrapped) throws CorruptMessageException, InvalidKeyException, NoSuchAlgorithmException, NoSuchPaddingException, InvalidAlgorithmParameterException, IllegalBlockSizeException, BadPaddingException {
    /* !!! write me */

    /* !!! dummy code, so compiler won't complain     */
    /* !!! remove for PP1 Milestone #2 implementation */
		byte[] aeskey = this.tryDecrypt(wrapped);
		if (aeskey != null) {
			Aes unwrapped = new Aes(aeskey);
			return unwrapped;
		} else {
			return null;
		}
  }

  //xin: haven't been used
  /**
   * Encrypt a MAC key with AES.
   *
   * @param wrappee the MAC instance to wrap
   * @return the wrapped key
   */
  public byte[] wrapMac(Hmac wrappee) {
    /* !!! write me */

    /* !!! dummy code, so compiler won't complain     */
    /* !!! remove for PP1 Milestone #2 implementation */
    return null;
  }

  //xin: haven't been used
  /**
   * Decrypt a MAC key which has been encrypted with wrapMac().
   *
   * @param wrapped the MAC instance to unwrap
   * @return a MAC instance initialized with the unwrapped key
   */
  public Hmac unwrapMac(byte[] wrapped) throws CorruptMessageException {
    /* !!! write me */

    /* !!! dummy code, so compiler won't complain     */
    /* !!! remove for PP1 Milestone #2 implementation */
    return null;
  }
}
