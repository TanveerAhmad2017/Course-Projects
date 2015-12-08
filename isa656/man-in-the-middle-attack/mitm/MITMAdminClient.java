/**
 * CS255 project 2
 */
package mitm;

import java.io.*;
import java.net.*;
import java.util.Random;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import javax.net.ssl.SSLContext;
import javax.net.ssl.SSLSocket;
import javax.net.ssl.TrustManager;

public class MITMAdminClient {
	private Socket m_remoteSocket;
	private String username;
	private String password;
	private String command;
	private String commonName = "";

	public static void main(String[] args) {
		MITMAdminClient admin = new MITMAdminClient(args);
		admin.run();
	}
	
	private static Integer sharedSecret(Integer x, Integer y)
	{
		return x*y-6;
	}

	private Error printUsage() {
		System.err.println("\n" + "Usage: " + "\n java "
				+ MITMAdminClient.class + " <options>" + "\n"
				+ "\n Where options can include:" + "\n"
				+ "\n   <-username <name> >   " + "\n   <-password <pass> >   "
				+ "\n   <-cmd <shudown|stats>"
				+ "\n   [-remoteHost <host name/ip>]  Default is localhost"
				+ "\n   [-remotePort <port>]          Default is 8002" + "\n");

		System.exit(1);
		return null;
	}

	private static class TrustEveryone implements
			javax.net.ssl.X509TrustManager {
		public void checkClientTrusted(
				java.security.cert.X509Certificate[] chain,
				String authenticationType) {
		}

		public void checkServerTrusted(
				java.security.cert.X509Certificate[] chain,
				String authenticationType) {
		}

		public java.security.cert.X509Certificate[] getAcceptedIssuers() {
			return null;
		}
	}

	private MITMAdminClient(String[] args) {
		int remotePort = 8002;
		String remoteHost = "localhost";

		if (args.length < 3)
			throw printUsage();

		try {
			for (int i = 0; i < args.length; i++) {
				if (args[i].equals("-remoteHost")) {
					remoteHost = args[++i];
				} else if (args[i].equals("-remotePort")) {
					remotePort = Integer.parseInt(args[++i]);
				} else if (args[i].equals("-password")) {
					password = args[++i];
				} else if (args[i].equals("-username")) {
					username = args[++i];
				} else if (args[i].equals("-cmd")) {
					command = args[++i];
					if (command.equals("enable") || command.equals("disable")) {
						commonName = args[++i];
					}
				} else {
					throw printUsage();
				}
			}

			// SSLContext sslContext = SSLContext.getInstance( "SSL" );
			//
			// sslContext.init(
			// new javax.net.ssl.KeyManager[] {}
			// , new TrustManager[] { new TrustEveryone() }
			// , null
			// );
			MITMSSLSocketFactory socketFactory = new MITMSSLSocketFactory();
			m_remoteSocket = socketFactory.createClientSocket(remoteHost,
					remotePort);

			// m_remoteSocket = (SSLSocket)
			// sslContext.getSocketFactory().createSocket( remoteHost,
			// remotePort );
		} catch (Exception e) {
			throw printUsage();
		}

	}
	
	private void sendMessage(String msg) throws IOException {
		//System.out.println("[AdminClient] Send Message: '" + msg + "'");
		// append special chars to idicate the end of the message
		msg += "$$";
		PrintWriter writer = new PrintWriter(m_remoteSocket.getOutputStream());
		writer.print(msg);
		writer.flush();
	}

	public void run() {
		try {
			Random rd = new Random();
			int clientChallenge = rd.nextInt();	
			if (m_remoteSocket != null) {
							
				String msg = "initusername:" + username+"\n challenge:"+clientChallenge;
				System.out.println("Client init the connection to server ...\n" +
						"	send challenge to server, challenge = "+clientChallenge);
				this.sendMessage(msg);
			}

			// match challenge
			byte[] buffer = new byte[40960];

			Pattern userPwdPattern =
			// Pattern.compile("password:(\\S+)\\s+command:(\\S+)\\sCN:(\\S*)\\s");
			Pattern.compile("challenge:(\\S+)verification:(\\S+)\\s");

			BufferedInputStream in = new BufferedInputStream(
					m_remoteSocket.getInputStream(), buffer.length);

			// Read a buffer full.
			int bytesRead = in.read(buffer);

			String line = bytesRead > 0 ? new String(buffer, 0, bytesRead) : "";

			Matcher userPwdMatcher = userPwdPattern.matcher(line);

			// parse username and pwd
			if (userPwdMatcher.find()) {

				String serverchallenge = userPwdMatcher.group(1);
				Integer verification = Integer.parseInt(userPwdMatcher.group(2));
				
				System.out.println("Client got challenge and verification from server: " + clientChallenge+ ", verificaiton: "+verification+"\n");
				
				Integer clientVerification = sharedSecret(clientChallenge, Integer.parseInt(serverchallenge));
                if(!clientVerification.equals(verification))
                {
                	System.out.println("Server fail to be authenticated by client");
                	return;
                }else
                {
                	System.out.println("	Client authenticates server!");
                }
				if (m_remoteSocket != null) {
					// System.out.println("client sends pwd+challenge etc to server");
				
					String msg = "username:" + username + "\n" + "password:"
							+ password + "\n" + "command:" + command + "\n"
							+ "CN:" + commonName + "\n" + "verification:"
							+ verification;
					this.sendMessage(msg);
					if(command.equals("shutdown")) System.exit(1);
				}

			}

			

			// now read back any response
			// Read a buffer full.
			bytesRead = in.read(buffer);

			line = bytesRead > 0 ? new String(buffer, 0, bytesRead) : "";

			System.out.println("");
			System.out.println("Receiving input from MITM proxy:");
			System.out.println("");
			
			 BufferedReader r = new BufferedReader(new InputStreamReader(m_remoteSocket.getInputStream()));
			// String line = null;
			while ((line = r.readLine()) != null) {
			//while (line != null) {
				System.out.println(line);
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		System.err.println("Admin Client exited");
		System.exit(0);
	}
}
