package mitm;

import java.io.File;
import java.io.FileInputStream;
import java.io.UnsupportedEncodingException;
import java.util.Vector;

public class Utility {
	
	public static final String pwdEncryptKey = "huangxin14";
	public static final String encryptePwdFile = "output";
	public static final String plainPwdFile = "input";
	
	public static String ByteToString(byte[] b) throws UnsupportedEncodingException
	{
		return new String(b);
	}
	
	public static byte[] concat2byte(byte[] salt, byte[] password) throws UnsupportedEncodingException
	{
		byte[] saltedPwd = new byte[salt.length + password.length];
		System.arraycopy(salt, 0, saltedPwd, 0, salt.length);
		System.arraycopy(password, 0, saltedPwd, salt.length, password.length);
		return saltedPwd;
	}
	
	public static byte[] concat3byte(byte[] user, byte[] salt, byte[] passwordHash)
	{
		byte[] entryByte = new byte[user.length + salt.length + passwordHash.length];
		System.arraycopy(user, 0, entryByte, 0, user.length);
		System.arraycopy(salt, 0, entryByte, user.length, salt.length);
		System.arraycopy(passwordHash, 0, entryByte, user.length + salt.length, passwordHash.length);
		return entryByte;
	}
	
	public static Vector<byte[][]> makeTable(byte[] bytes) throws Exception
    {
		if(bytes.length % 146 !=0) throw new Exception("Each entry should be 146 bytes long(100 byte user + 16 bytes salt + 30 byte hash result");
		
    	Vector<byte[][]> rnt = new Vector<byte[][]>();
    	
    	for(int i=0; i<bytes.length; i+=146)
    	{
	    	byte[] user = new byte[100];
	    	byte[] salt = new byte[16];
	    	byte[] pwdHash = new byte[30];
	    	
	    	//System.out.println("byte length: "+ bytes.length);
	    	System.arraycopy(bytes, i, user, 0, 100);
	    	System.arraycopy(bytes, i+100, salt, 0, 16);	    	
	    	System.arraycopy(bytes, i+116, pwdHash, 0, 30);
	    	
	    	byte[][] entry = new byte[3][];
	    	entry[0] = user;
	    	entry[1] = salt;
	    	entry[2] = pwdHash;
	    	
//	    	System.out.println("make tabke...");
//	    	System.out.println("entry[0]=" + Utility.ByteToString(entry[0]));
//	    	System.out.println("entry[1]=" + Utility.ByteToString(entry[1]));
//	    	System.out.println("entry[2]=" + Utility.ByteToString(entry[2]));
	    	rnt.add(entry);
	    	
	    	
    	}
    	return rnt;
    }
	
	public static Vector<byte[][]> ReadPwdFile()
	{

		Vector<byte[][]> saltedPwdHash = new Vector<byte[][]>();
		try{
			File pwdFile = new File(Utility.encryptePwdFile);
			byte[] bytes = new byte[(int) pwdFile.length()];
			FileInputStream fis = new FileInputStream(pwdFile);
			fis.read(bytes);
			fis.close();
			System.out.println("Password file read from disk... ");
//			System.out.println("Password file length" + bytes.length);
//			System.out.println("Password file content" + Utility.ByteToString(bytes));
			
			//decrypt byte
			Aes aes = new Aes(Utility.pwdEncryptKey);
			byte[] decBytes = aes.decrypt(bytes);
			System.out.println("Decrypt Password file read from disk... ");
//			System.out.println("Decrypt Password file length" + bytes.length);
//			System.out.println("Decrypt Password file content" + Utility.ByteToString(decBytes));
			
			saltedPwdHash = Utility.makeTable(decBytes);
		}catch(Exception e)
		{
			System.err.println("Fail to read password file");
		}
		
		return saltedPwdHash;
	}

}
