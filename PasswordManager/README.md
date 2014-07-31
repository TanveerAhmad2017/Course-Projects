PasswordManager
===============
Course Project for ISA656 Network Security

This tool allows uses to store their encrypted resource key pair in server. Whenever client needs the resource key, he can request it from server using master-password as authentication.

Security protocol is designed to ensure the security of this tools. Attacks such as eavesdrop, replay attack, integrity attack can be avoided.

AES/CTR is used to encrypt the traffic. Nonce is used to resist reply attack. Hmac is associated to check integrity.       
