package pwman;

import static org.junit.Assert.*;

import java.io.UnsupportedEncodingException;
import java.security.InvalidAlgorithmParameterException;
import java.security.InvalidKeyException;
import java.security.NoSuchAlgorithmException;
import java.security.spec.InvalidParameterSpecException;

import javax.crypto.BadPaddingException;
import javax.crypto.IllegalBlockSizeException;
import javax.crypto.NoSuchPaddingException;

import junit.framework.Assert;

import org.junit.Before;
import org.junit.Test;

public class AesTest {

	private Aes aes;
	
	@Before
	public void setUp() throws Exception {
		aes = new Aes();
	}

	@Test
	public void testEncrypt() {
		String plain = "Hello";
		
		
		try {
			byte [] encryptedMsg = aes.encrypt(plain.getBytes("UTF-8"));
			assertNotNull(encryptedMsg);
			try {
				String decryptedMsg = new String(aes.decrypt(encryptedMsg), "UTF-8");
				assertEquals(decryptedMsg.equals(plain), true);
				System.out.println("decrpyted msg " + decryptedMsg);
			} catch (CorruptMessageException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
				fail("...");
			}
			
		} catch (InvalidKeyException | NoSuchAlgorithmException
				| NoSuchPaddingException | InvalidAlgorithmParameterException
				| IllegalBlockSizeException | BadPaddingException
				| InvalidParameterSpecException | UnsupportedEncodingException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
			fail("...");
		}
	}


}
