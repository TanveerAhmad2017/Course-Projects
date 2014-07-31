package DHash;

import java.math.BigInteger;
import java.nio.ByteBuffer;
import java.security.MessageDigest;
import java.security.NoSuchAlgorithmException;

import util.config;


/**
 * Use to hash IP address/user name into integer
 * The integer will be used to chord to identify server and clients
 * @author huangxin
 *
 */

public final class Hashing
{

	private static String DEFAULT_METHOD =  "SHA-256";
	private static String hashMethod;
	private static MessageDigest md;
	
	public static void setMethod(String hashMethod)
	{
		Hashing.hashMethod = hashMethod;
	}
	
	public static int hash(String key)
	{
		
		if(key==null){
			System.out.println("Hashing input is null");
			return config.nullHash;
		}
		/* 
		 * In the odd case that we do note have access to SHA-256, we
		 * just use h as the key.
		 */
		MessageDigest md;
		try {
		    md = MessageDigest.getInstance("SHA-256");
		} catch (NoSuchAlgorithmException _){
		    System.err.println("Cannot use SHA-256, expect worse " +
				       "random-lookingness of the keys!");
		    md = null;
		}

		

		md.reset();
		md.update(key.getBytes());
		byte[] longRandomLookingKey = md.digest();

		int[] keyAsInts = new int[4];
		for (int i=0; i<4; i++) {
		    keyAsInts[i] = longRandomLookingKey[i];
		    if (keyAsInts[i] < 0) keyAsInts[i]+=256;
		}
		
		long resAsLong = keyAsInts[3];
		resAsLong = resAsLong << 24;
		resAsLong = resAsLong |  
		    (keyAsInts[2] << 16) | 
		    (keyAsInts[1] << 8) |
		    (keyAsInts[0] << 0) ;
		
		resAsLong = resAsLong % config.chordSize;
		return (int)resAsLong;
		
	}
	
	
	/**
     * @param low Must be a positive 31-bit integer
     * @param high Must be a positive 31-bit integer
     * @param candidate Must be a positive 31-bit integer
     *       
     * Test if the candidate is in the interval [low, high-1].  If low
     * > high, then the question is answered with "wrap-around modulo
     * 2^31".
     */
    public static boolean inBetween(int low, int high, int candidate) {
		if (low==high) return false;
		if ((candidate >= low) && (candidate < high)) return true;
		return false;
    } 
    

	
	//test IP address
	public static void main(String[] args)
	{
		for(int i=80; i<=99; i++)
		{
			String toHash = "129.174.94."+i;
			System.out.println(toHash + " "+ hash(toHash));
		}
		
		System.out.println("Alice" + " "+ hash("Alice"));
		System.out.println("Bob" + " "+ hash("Bob"));
		System.out.println("Jim" + " "+ hash("Jim"));
		System.out.println("Sandy" + " "+ hash("Sandy"));
		System.out.println("Helen" + " "+ hash("Helen"));
		System.out.println("keySize " + config.keySize);
		System.out.println("chordSize " + config.chordSize);
		
	}
	
	
	
	 
	   
}
