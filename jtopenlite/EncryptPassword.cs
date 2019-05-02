using System;

///////////////////////////////////////////////////////////////////////////////
//
// JTOpenLite
//
// Filename:  EncryptPassword.java
//
// The source code contained herein is licensed under the IBM Public License
// Version 1.0, which has been approved by the Open Source Initiative.
// Copyright (C) 2011-2012 International Business Machines Corporation and
// others.  All rights reserved.
//
///////////////////////////////////////////////////////////////////////////////

namespace com.ibm.jtopenlite
{

	/// <summary>
	/// Package private class to handle the encryption of a password sent to the host server.
	/// 
	/// </summary>
	internal sealed class EncryptPassword
	{
	  private EncryptPassword()
	  {
	  }

	  private static readonly sbyte[] SHA_SEQUENCE = new sbyte[] {0, 0, 0, 0, 0, 0, 0, 1};

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: static byte[] encryptPasswordSHA(byte[] userID, byte[] password, byte[] clientSeed, byte[] serverSeed) throws java.io.IOException
	  internal static sbyte[] encryptPasswordSHA(sbyte[] userID, sbyte[] password, sbyte[] clientSeed, sbyte[] serverSeed)
	  {
		try
		{
		  MessageDigest md = MessageDigest.getInstance("SHA");
		  md.update(userID);
		  md.update(password);
		  sbyte[] token = md.digest();
		  md.update(token);
		  md.update(serverSeed);
		  md.update(clientSeed);
		  md.update(userID);
		  md.update(SHA_SEQUENCE);
		  return md.digest();
		}
		catch (NoSuchAlgorithmException e)
		{
		  IOException io = new IOException("Error loading SHA encryption: " + e);
		  io.initCause(e);
		  throw io;
		}
	  }

	  internal static sbyte[] encryptPasswordDES(sbyte[] userID, sbyte[] password, sbyte[] clientSeed, sbyte[] serverSeed)
	  {
		sbyte[] sequenceNumber = new sbyte[] {0, 0, 0, 0, 0, 0, 0, 1};
		sbyte[] verifyToken = new sbyte[8];

		sbyte[] token = generateToken(userID, password);

		sbyte[] encryptedPassword = generatePasswordSubstitute(userID, token, verifyToken, sequenceNumber, clientSeed, serverSeed);
		return encryptedPassword;
	  }

	  // userID and password are in EBCDIC
	  // userID and password are terminated with 0x40 (EBCDIC blank)
	  private static sbyte[] generateToken(sbyte[] userID, sbyte[] password)
	  {
		sbyte[] token = new sbyte[8];
		sbyte[] workBuffer1 = new sbyte[10];
		sbyte[] workBuffer2 = new sbyte[] {0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40};
		sbyte[] workBuffer3 = new sbyte[] {0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40, 0x40};

		// Copy user ID into the work buffer.
		Array.Copy(userID, 0, workBuffer1, 0, 10);

		// Find userID length.
		int length = ebcdicStrLen(userID, 10);

		if (length > 8)
		{
		  // Fold user ID.
		  workBuffer1[0] ^= unchecked((sbyte)(workBuffer1[8] & 0xC0));
		  workBuffer1[1] ^= (sbyte)((workBuffer1[8] & 0x30) << 2);
		  workBuffer1[2] ^= (sbyte)((workBuffer1[8] & 0x0C) << 4);
		  workBuffer1[3] ^= (sbyte)((workBuffer1[8] & 0x03) << 6);
		  workBuffer1[4] ^= unchecked((sbyte)(workBuffer1[9] & 0xC0));
		  workBuffer1[5] ^= (sbyte)((workBuffer1[9] & 0x30) << 2);
		  workBuffer1[6] ^= (sbyte)((workBuffer1[9] & 0x0C) << 4);
		  workBuffer1[7] ^= (sbyte)((workBuffer1[9] & 0x03) << 6);
		}

		// work with password
		length = ebcdicStrLen(password, 10);

		// if password is more than 8 characters long
		if (length > 8)
		{
		  // copy the first 8 bytes of password to workBuffer2
		  Array.Copy(password, 0, workBuffer2, 0, 8);

		  // copy the remaining password to workBuffer3
		  Array.Copy(password, 8, workBuffer3, 0, length - 8);

		  // generate the token for the first 8 bytes of password
		  xorWith0x55andLshift(workBuffer2);

		  workBuffer2 = enc_des(workBuffer2, workBuffer1); // userID

		  // generate the token for the second 8 bytes of password
		  xorWith0x55andLshift(workBuffer3);

		  workBuffer3 = enc_des(workBuffer3, workBuffer1); // userID

		  // exclusive-or the first and second token to get the real token
		  xORArray(workBuffer2, workBuffer3, token);
		}
		else
		{
		  // copy password to work buffer
		  Array.Copy(password, 0, workBuffer2, 0, length);

		  // generate the token for 8 byte userID
		  xorWith0x55andLshift(workBuffer2);

		  token = enc_des(workBuffer2, workBuffer1); // userID
		}
		return token;
	  }

	  // Add two byte arrays.
	  private static void addArray(sbyte[] array1, sbyte[] array2, sbyte[] result, int length)
	  {
		int carryBit = 0;
		for (int i = length - 1; i >= 0; i--)
		{
		  int temp = (array1[i] & 0xff) + (array2[i] & 0xff) + carryBit;
		  carryBit = (int)((uint)temp >> 8);
		  result[i] = (sbyte)temp;
		}
	  }

	  private static int ebcdicStrLen(sbyte[] @string, int maxLength)
	  {
		int i = 0;
		while ((i < maxLength) && (@string[i] != 0x40) && (@string[i] != 0))
		{
			++i;
		}
		return i;
	  }

	  private static void xORArray(sbyte[] string1, sbyte[] string2, sbyte[] string3)
	  {
		for (int i = 0; i < 8; i++)
		{
		  string3[i] = (sbyte)(string1[i] ^ string2[i]);
		}
	  }

	  private static void xorWith0x55andLshift(sbyte[] bytes)
	  {
		bytes[0] ^= 0x55;
		bytes[1] ^= 0x55;
		bytes[2] ^= 0x55;
		bytes[3] ^= 0x55;
		bytes[4] ^= 0x55;
		bytes[5] ^= 0x55;
		bytes[6] ^= 0x55;
		bytes[7] ^= 0x55;

		bytes[0] = (sbyte)(bytes[0] << 1 | (int)((uint)(bytes[1] & 0x80) >> 7));
		bytes[1] = (sbyte)(bytes[1] << 1 | (int)((uint)(bytes[2] & 0x80) >> 7));
		bytes[2] = (sbyte)(bytes[2] << 1 | (int)((uint)(bytes[3] & 0x80) >> 7));
		bytes[3] = (sbyte)(bytes[3] << 1 | (int)((uint)(bytes[4] & 0x80) >> 7));
		bytes[4] = (sbyte)(bytes[4] << 1 | (int)((uint)(bytes[5] & 0x80) >> 7));
		bytes[5] = (sbyte)(bytes[5] << 1 | (int)((uint)(bytes[6] & 0x80) >> 7));
		bytes[6] = (sbyte)(bytes[6] << 1 | (int)((uint)(bytes[7] & 0x80) >> 7));
		bytes[7] <<= 1;
	  }

	  // the E function used in the cipher function
	  private static readonly int[] EPERM = new int[] {32, 1, 2, 3, 4, 5, 4, 5, 6, 7, 8, 9, 8, 9, 10, 11, 12, 13, 12, 13, 14, 15, 16, 17, 16, 17, 18, 19, 20, 21, 20, 21, 22, 23, 24, 25, 24, 25, 26, 27, 28, 29, 28, 29, 30, 31, 32, 1};

	  // the initial scrambling of the input data
	  private static readonly int[] INITPERM = new int[] {58, 50, 42, 34, 26, 18, 10, 2, 60, 52, 44, 36, 28, 20, 12, 4, 62, 54, 46, 38, 30, 22, 14, 6, 64, 56, 48, 40, 32, 24, 16, 8, 57, 49, 41, 33, 25, 17, 9, 1, 59, 51, 43, 35, 27, 19, 11, 3, 61, 53, 45, 37, 29, 21, 13, 5, 63, 55, 47, 39, 31, 23, 15, 7};

	  // the inverse permutation of initperm - used on the proutput block
	  private static readonly int[] OUTPERM = new int[] {40, 8, 48, 16, 56, 24, 64, 32, 39, 7, 47, 15, 55, 23, 63, 31, 38, 6, 46, 14, 54, 22, 62, 30, 37, 5, 45, 13, 53, 21, 61, 29, 36, 4, 44, 12, 52, 20, 60, 28, 35, 3, 43, 11, 51, 19, 59, 27, 34, 2, 42, 10, 50, 18, 58, 26, 33, 1, 41, 9, 49, 17, 57, 25};

	  // the P function used in cipher function
	  private static readonly int[] PPERM = new int[] {16, 7, 20, 21, 29, 12, 28, 17, 1, 15, 23, 26, 5, 18, 31, 10, 2, 8, 24, 14, 32, 27, 3, 9, 19, 13, 30, 6, 22, 11, 4, 25};

	  private static readonly int[] PC1 = new int[] {57, 49, 41, 33, 25, 17, 9, 1, 58, 50, 42, 34, 26, 18, 10, 2, 59, 51, 43, 35, 27, 19, 11, 3, 60, 52, 44, 36, 63, 55, 47, 39, 31, 23, 15, 7, 62, 54, 46, 38, 30, 22, 14, 6, 61, 53, 45, 37, 29, 21, 13, 5, 28, 20, 12, 4};

	  private static readonly int[] PC2 = new int[] {14, 17, 11, 24, 1, 5, 3, 28, 15, 6, 21, 10, 23, 19, 12, 4, 26, 8, 16, 7, 27, 20, 13, 2, 41, 52, 31, 37, 47, 55, 30, 40, 51, 45, 33, 48, 44, 49, 39, 56, 34, 53, 46, 42, 50, 36, 29, 32};

	  private static readonly int[] S1 = new int[] {14, 4, 13, 1, 2, 15, 11, 8, 3, 10, 6, 12, 5, 9, 0, 7, 0, 15, 7, 4, 14, 2, 13, 1, 10, 6, 12, 11, 9, 5, 3, 8, 4, 1, 14, 8, 13, 6, 2, 11, 15, 12, 9, 7, 3, 10, 5, 0, 15, 12, 8, 2, 4, 9, 1, 7, 5, 11, 3, 14, 10, 0, 6, 13};

	  private static readonly int[] S2 = new int[] {15, 1, 8, 14, 6, 11, 3, 4, 9, 7, 2, 13, 12, 0, 5, 10, 3, 13, 4, 7, 15, 2, 8, 14, 12, 0, 1, 10, 6, 9, 11, 5, 0, 14, 7, 11, 10, 4, 13, 1, 5, 8, 12, 6, 9, 3, 2, 15, 13, 8, 10, 1, 3, 15, 4, 2, 11, 6, 7, 12, 0, 5, 14, 9};

	  private static readonly int[] S3 = new int[] {10, 0, 9, 14, 6, 3, 15, 5, 1, 13, 12, 7, 11, 4, 2, 8, 13, 7, 0, 9, 3, 4, 6, 10, 2, 8, 5, 14, 12, 11, 15, 1, 13, 6, 4, 9, 8, 15, 3, 0, 11, 1, 2, 12, 5, 10, 14, 7, 1, 10, 13, 0, 6, 9, 8, 7, 4, 15, 14, 3, 11, 5, 2, 12};

	  private static readonly int[] S4 = new int[] {7, 13, 14, 3, 0, 6, 9, 10, 1, 2, 8, 5, 11, 12, 4, 15, 13, 8, 11, 5, 6, 15, 0, 3, 4, 7, 2, 12, 1, 10, 14, 9, 10, 6, 9, 0, 12, 11, 7, 13, 15, 1, 3, 14, 5, 2, 8, 4, 3, 15, 0, 6, 10, 1, 13, 8, 9, 4, 5, 11, 12, 7, 2, 14};

	  private static readonly int[] S5 = new int[] {2, 12, 4, 1, 7, 10, 11, 6, 8, 5, 3, 15, 13, 0, 14, 9, 14, 11, 2, 12, 4, 7, 13, 1, 5, 0, 15, 10, 3, 9, 8, 6, 4, 2, 1, 11, 10, 13, 7, 8, 15, 9, 12, 5, 6, 3, 0, 14, 11, 8, 12, 7, 1, 14, 2, 13, 6, 15, 0, 9, 10, 4, 5, 3};

	  private static readonly int[] S6 = new int[] {12, 1, 10, 15, 9, 2, 6, 8, 0, 13, 3, 4, 14, 7, 5, 11, 10, 15, 4, 2, 7, 12, 9, 5, 6, 1, 13, 14, 0, 11, 3, 8, 9, 14, 15, 5, 2, 8, 12, 3, 7, 0, 4, 10, 1, 13, 11, 6, 4, 3, 2, 12, 9, 5, 15, 10, 11, 14, 1, 7, 6, 0, 8, 13};

	  private static readonly int[] S7 = new int[] {4, 11, 2, 14, 15, 0, 8, 13, 3, 12, 9, 7, 5, 10, 6, 1, 13, 0, 11, 7, 4, 9, 1, 10, 14, 3, 5, 12, 2, 15, 8, 6, 1, 4, 11, 13, 12, 3, 7, 14, 10, 15, 6, 8, 0, 5, 9, 2, 6, 11, 13, 8, 1, 4, 10, 7, 9, 5, 0, 15, 14, 2, 3, 12};

	  private static readonly int[] S8 = new int[] {13, 2, 8, 4, 6, 15, 11, 1, 10, 9, 3, 14, 5, 0, 12, 7, 1, 15, 13, 8, 10, 3, 7, 4, 12, 5, 6, 11, 0, 14, 9, 2, 7, 11, 4, 1, 9, 12, 14, 2, 0, 6, 10, 13, 15, 3, 5, 8, 2, 1, 14, 7, 4, 10, 8, 13, 15, 12, 9, 0, 3, 5, 6, 11};

	  // Name:  enc_des - Encrypt function front interface
	  //
	  // Function:  This function is the interface to the DES encryption routine
	  //            It converts the parameters to a format expected by the actual DES encryption routine.
	  //
	  // Input:   8 byte data to encrypt
	  //          8 byte key to encrypt
	  //
	  // Output:  8 byte encrypted data passed parameter
	  //
	  //***************************************************************************
	  //
	  // enc_des(byte[] data, byte[] key, byte[] enc_data)
	  // {
	  //   Copy the passed parameters to local variables so we can have 9 bytes variables.
	  //   Expand the key and data variables so we will have one byte representing one bit of the input data.
	  //   Perform the actual encryption of the input data using the 64 bytes variable according with the DES algorithm.
	  //   Compress back the result of the encryption to return the 8 bytes data encryption result.
	  // }

	  private static sbyte[] enc_des(sbyte[] key, sbyte[] data)
	  {
		// expend strings, 1 bit per byte, 1 char in 8 bytes
		sbyte[] e1 = new sbyte[65];
		sbyte[] e2 = new sbyte[65];

		// input strings, 1 character per byte password user id to be used as key encrypted data


		// expand the input string to 1 bit per byte again for the key
		for (int i = 0; i < 8; ++i)
		{
		  e1[8 * i + 1] = (sbyte)(((data[i] & 0x80) == 0) ? 0x30 : 0x31);
		  e1[8 * i + 2] = (sbyte)(((data[i] & 0x40) == 0) ? 0x30 : 0x31);
		  e1[8 * i + 3] = (sbyte)(((data[i] & 0x20) == 0) ? 0x30 : 0x31);
		  e1[8 * i + 4] = (sbyte)(((data[i] & 0x10) == 0) ? 0x30 : 0x31);
		  e1[8 * i + 5] = (sbyte)(((data[i] & 0x08) == 0) ? 0x30 : 0x31);
		  e1[8 * i + 6] = (sbyte)(((data[i] & 0x04) == 0) ? 0x30 : 0x31);
		  e1[8 * i + 7] = (sbyte)(((data[i] & 0x02) == 0) ? 0x30 : 0x31);
		  e1[8 * i + 8] = (sbyte)(((data[i] & 0x01) == 0) ? 0x30 : 0x31);
		}

		for (int i = 0; i < 8; ++i)
		{
		  e2[8 * i + 1] = (sbyte)(((key[i] & 0x80) == 0) ? 0x30 : 0x31);
		  e2[8 * i + 2] = (sbyte)(((key[i] & 0x40) == 0) ? 0x30 : 0x31);
		  e2[8 * i + 3] = (sbyte)(((key[i] & 0x20) == 0) ? 0x30 : 0x31);
		  e2[8 * i + 4] = (sbyte)(((key[i] & 0x10) == 0) ? 0x30 : 0x31);
		  e2[8 * i + 5] = (sbyte)(((key[i] & 0x08) == 0) ? 0x30 : 0x31);
		  e2[8 * i + 6] = (sbyte)(((key[i] & 0x04) == 0) ? 0x30 : 0x31);
		  e2[8 * i + 7] = (sbyte)(((key[i] & 0x02) == 0) ? 0x30 : 0x31);
		  e2[8 * i + 8] = (sbyte)(((key[i] & 0x01) == 0) ? 0x30 : 0x31);
		}

		// encryption method
		sbyte[] preout = new sbyte[65]; // preoutput block

		// generate keys 1 - 16

		// temp key gen workspace
		sbyte[] Cn = new sbyte[58];
		// create Cn from the original key
		for (int n = 1; n <= 56; n++)
		{
		  Cn[n] = e2[PC1[n - 1]];
		}

		// rotate Cn to form C1 (still called Cn...)
		lshift1(Cn);

		sbyte[] key1 = new sbyte[49]; // 48 bit key 1 to key 16
		// now Cn[] contains 56 bits for input to PC2 to generate key1
		for (int n = 1; n <= 48; n++)
		{
		  key1[n] = Cn[PC2[n - 1]];
		}

		sbyte[] key2 = new sbyte[49];
		// now derive C2 from C1 (which is called Cn)
		lshift1(Cn);
		for (int n = 1; n <= 48; n++)
		{
		  key2[n] = Cn[PC2[n - 1]];
		}

		sbyte[] key3 = new sbyte[49];
		// now derive C3 from C2 by left shifting twice
		lshift2(Cn);
		for (int n = 1; n <= 48; n++)
		{
		  key3[n] = Cn[PC2[n - 1]];
		}

		sbyte[] key4 = new sbyte[49];
		// now derive C4 from C3 by again left shifting twice
		lshift2(Cn);
		for (int n = 1; n <= 48; n++)
		{
		  key4[n] = Cn[PC2[n - 1]];
		}

		sbyte[] key5 = new sbyte[49];
		// now derive C5 from C4 by again left shifting twice
		lshift2(Cn);
		for (int n = 1; n <= 48; n++)
		{
		  key5[n] = Cn[PC2[n - 1]];
		}

		sbyte[] key6 = new sbyte[49];
		// now derive C6 from C5 by again left shifting twice
		lshift2(Cn);
		for (int n = 1; n <= 48; n++)
		{
		  key6[n] = Cn[PC2[n - 1]];
		}

		sbyte[] key7 = new sbyte[49];
		// now derive C7 from C6 by again left shifting twice
		lshift2(Cn);
		for (int n = 1; n <= 48; n++)
		{
		  key7[n] = Cn[PC2[n - 1]];
		}

		sbyte[] key8 = new sbyte[49];
		// now derive C8 from C7 by again left shifting twice
		lshift2(Cn);
		for (int n = 1; n <= 48; n++)
		{
		  key8[n] = Cn[PC2[n - 1]];
		}

		sbyte[] key9 = new sbyte[49];
		// now derive C9 from C8 by shifting left once
		lshift1(Cn);
		for (int n = 1; n <= 48; n++)
		{
		  key9[n] = Cn[PC2[n - 1]];
		}

		sbyte[] key10 = new sbyte[49];
		// now derive C10 from C9 by again left shifting twice
		lshift2(Cn);
		for (int n = 1; n <= 48; n++)
		{
		  key10[n] = Cn[PC2[n - 1]];
		}

		sbyte[] key11 = new sbyte[49];
		// now derive C11 from C10 by again left shifting twice
		lshift2(Cn);
		for (int n = 1; n <= 48; n++)
		{
		  key11[n] = Cn[PC2[n - 1]];
		}

		sbyte[] key12 = new sbyte[49];
		// now derive C12 from C11 by again left shifting twice
		lshift2(Cn);
		for (int n = 1; n <= 48; n++)
		{
		  key12[n] = Cn[PC2[n - 1]];
		}

		sbyte[] key13 = new sbyte[49];
		// now derive C13 from C12 by again left shifting twice
		lshift2(Cn);
		for (int n = 1; n <= 48; n++)
		{
		  key13[n] = Cn[PC2[n - 1]];
		}

		sbyte[] key14 = new sbyte[49];
		// now derive C14 from C13 by again left shifting twice
		lshift2(Cn);
		for (int n = 1; n <= 48; n++)
		{
		  key14[n] = Cn[PC2[n - 1]];
		}

		sbyte[] key15 = new sbyte[49];
		// now derive C15 from C14 by again left shifting twice
		lshift2(Cn);
		for (int n = 1; n <= 48; n++)
		{
		  key15[n] = Cn[PC2[n - 1]];
		}

		sbyte[] key16 = new sbyte[49];
		// now derive C16 from C15 by again left shifting once
		lshift1(Cn);
		for (int n = 1; n <= 48; n++)
		{
		  key16[n] = Cn[PC2[n - 1]];
		}

		// temp encryption workspace
		sbyte[] Ln = new sbyte[33];
		// ditto
		sbyte[] Rn = new sbyte[33];

		// perform the initial permutation and store the result in Ln and Rn
		for (int n = 1; n <= 32; n++)
		{
		  Ln[n] = e1[INITPERM[n - 1]];
		  Rn[n] = e1[INITPERM[n + 31]];
		}

		// run cipher to get new Ln and Rn
		cipher(key1, Ln, Rn);
		cipher(key2, Ln, Rn);
		cipher(key3, Ln, Rn);
		cipher(key4, Ln, Rn);
		cipher(key5, Ln, Rn);
		cipher(key6, Ln, Rn);
		cipher(key7, Ln, Rn);
		cipher(key8, Ln, Rn);
		cipher(key9, Ln, Rn);
		cipher(key10, Ln, Rn);
		cipher(key11, Ln, Rn);
		cipher(key12, Ln, Rn);
		cipher(key13, Ln, Rn);
		cipher(key14, Ln, Rn);
		cipher(key15, Ln, Rn);
		cipher(key16, Ln, Rn);

		// Ln and Rn are now at L16 and R16 - create preout[] by interposing them
		Array.Copy(Rn, 1, preout, 1, 32);
		Array.Copy(Ln, 1, preout, 33, 32);

		sbyte[] e3 = new sbyte[65];
		// run preout[] through outperm to get ciphertext
		for (int n = 1; n <= 64; n++)
		{
		  e3[n] = preout[OUTPERM[n - 1]];
		}

		sbyte[] enc_data = new sbyte[8];
		// compress back to 8 bits per byte
		for (int i = 0; i < 8; ++i)
		{
		  if (e3[8 * i + 1] == 0x31)
		  {
			  enc_data[i] |= unchecked((sbyte)0x80);
		  }
		  if (e3[8 * i + 2] == 0x31)
		  {
			  enc_data[i] |= 0x40;
		  }
		  if (e3[8 * i + 3] == 0x31)
		  {
			  enc_data[i] |= 0x20;
		  }
		  if (e3[8 * i + 4] == 0x31)
		  {
			  enc_data[i] |= 0x10;
		  }
		  if (e3[8 * i + 5] == 0x31)
		  {
			  enc_data[i] |= 0x08;
		  }
		  if (e3[8 * i + 6] == 0x31)
		  {
			  enc_data[i] |= 0x04;
		  }
		  if (e3[8 * i + 7] == 0x31)
		  {
			  enc_data[i] |= 0x02;
		  }
		  if (e3[8 * i + 8] == 0x31)
		  {
			  enc_data[i] |= 0x01;
		  }
		}
		return enc_data;
	  }

	  private static void cipher(sbyte[] key, sbyte[] Ln, sbyte[] Rn)
	  {
		sbyte[] temp1 = new sbyte[49]; // Rn run through E
		sbyte[] temp2 = new sbyte[49]; // temp1 XORed with key
		sbyte[] temp3 = new sbyte[33]; // temp2 run through S boxes
		sbyte[] fkn = new sbyte[33]; //  f(k,n)
		int[] si = new int[9]; // decimal input to S boxes
		int[] so = new int[9]; // decimal output from S boxes

		// generate temp1[] from Rn[]
		for (int n = 1; n <= 48; n++)
		{
		  temp1[n] = Rn[EPERM[n - 1]];
		}

		// XOR temp1 with key to get temp2
		for (int n = 1; n <= 48; n++)
		{
		  temp2[n] = (temp1[n] != key[n]) ? (sbyte)0x31 : (sbyte)0x30;
		}

		// we need to get the explicit representation into a form for
		// processing the s boxes...
		si[1] = ((temp2[1] == 0x31) ? 0x0020 : 0x0000) | ((temp2[6] == 0x31) ? 0x0010 : 0x0000) | ((temp2[2] == 0x31) ? 0x0008 : 0x0000) | ((temp2[3] == 0x31) ? 0x0004 : 0x0000) | ((temp2[4] == 0x31) ? 0x0002 : 0x0000) | ((temp2[5] == 0x31) ? 0x0001 : 0x0000);

		si[2] = ((temp2[7] == 0x31) ? 0x0020 : 0x0000) | ((temp2[12] == 0x31) ? 0x0010 : 0x0000) | ((temp2[8] == 0x31) ? 0x0008 : 0x0000) | ((temp2[9] == 0x31) ? 0x0004 : 0x0000) | ((temp2[10] == 0x31) ? 0x0002 : 0x0000) | ((temp2[11] == 0x31) ? 0x0001 : 0x0000);

		si[3] = ((temp2[13] == 0x31) ? 0x0020 : 0x0000) | ((temp2[18] == 0x31) ? 0x0010 : 0x0000) | ((temp2[14] == 0x31) ? 0x0008 : 0x0000) | ((temp2[15] == 0x31) ? 0x0004 : 0x0000) | ((temp2[16] == 0x31) ? 0x0002 : 0x0000) | ((temp2[17] == 0x31) ? 0x0001 : 0x0000);

		si[4] = ((temp2[19] == 0x31) ? 0x0020 : 0x0000) | ((temp2[24] == 0x31) ? 0x0010 : 0x0000) | ((temp2[20] == 0x31) ? 0x0008 : 0x0000) | ((temp2[21] == 0x31) ? 0x0004 : 0x0000) | ((temp2[22] == 0x31) ? 0x0002 : 0x0000) | ((temp2[23] == 0x31) ? 0x0001 : 0x0000);

		si[5] = ((temp2[25] == 0x31) ? 0x0020 : 0x0000) | ((temp2[30] == 0x31) ? 0x0010 : 0x0000) | ((temp2[26] == 0x31) ? 0x0008 : 0x0000) | ((temp2[27] == 0x31) ? 0x0004 : 0x0000) | ((temp2[28] == 0x31) ? 0x0002 : 0x0000) | ((temp2[29] == 0x31) ? 0x0001 : 0x0000);

		si[6] = ((temp2[31] == 0x31) ? 0x0020 : 0x0000) | ((temp2[36] == 0x31) ? 0x0010 : 0x0000) | ((temp2[32] == 0x31) ? 0x0008 : 0x0000) | ((temp2[33] == 0x31) ? 0x0004 : 0x0000) | ((temp2[34] == 0x31) ? 0x0002 : 0x0000) | ((temp2[35] == 0x31) ? 0x0001 : 0x0000);

		si[7] = ((temp2[37] == 0x31) ? 0x0020 : 0x0000) | ((temp2[42] == 0x31) ? 0x0010 : 0x0000) | ((temp2[38] == 0x31) ? 0x0008 : 0x0000) | ((temp2[39] == 0x31) ? 0x0004 : 0x0000) | ((temp2[40] == 0x31) ? 0x0002 : 0x0000) | ((temp2[41] == 0x31) ? 0x0001 : 0x0000);

		si[8] = ((temp2[43] == 0x31) ? 0x0020 : 0x0000) | ((temp2[48] == 0x31) ? 0x0010 : 0x0000) | ((temp2[44] == 0x31) ? 0x0008 : 0x0000) | ((temp2[45] == 0x31) ? 0x0004 : 0x0000) | ((temp2[46] == 0x31) ? 0x0002 : 0x0000) | ((temp2[47] == 0x31) ? 0x0001 : 0x0000);

		// Now for the S boxes
		so[1] = S1[si[1]];
		so[2] = S2[si[2]];
		so[3] = S3[si[3]];
		so[4] = S4[si[4]];
		so[5] = S5[si[5]];
		so[6] = S6[si[6]];
		so[7] = S7[si[7]];
		so[8] = S8[si[8]];

		// That wasn't too bad.  Now to convert decimal to char hex again so[1-8] must be translated to 32 bits and stored in temp3[1-32]
		dectobin(so[1], temp3, 1);
		dectobin(so[2], temp3, 5);
		dectobin(so[3], temp3, 9);
		dectobin(so[4], temp3, 13);
		dectobin(so[5], temp3, 17);
		dectobin(so[6], temp3, 21);
		dectobin(so[7], temp3, 25);
		dectobin(so[8], temp3, 29);

		// Okay. Now temp3[] contains the data to run through P
		for (int n = 1; n <= 32; n++)
		{
		  fkn[n] = temp3[PPERM[n - 1]];
		}

		// now complete the cipher function to update Ln and Rn
		sbyte[] temp = new sbyte[33]; // storage for Ln during cipher function
		Array.Copy(Rn, 1, temp, 1, 32);
		for (int n = 1; n <= 32; n++)
		{
		  Rn[n] = (Ln[n] == fkn[n]) ? (sbyte)0x30:(sbyte)0x31;
		}
		Array.Copy(temp, 1, Ln, 1, 32);
	  }

	  // Start of decimal to binary routine
	  //****************************************************************************
	  // convert decimal number to four ones and zeros in store
	  // them in the input string
	  private static void dectobin(int value, sbyte[] @string, int offset)
	  {
		@string[offset] = (sbyte)(((value & 0x0008) != 0) ? 0x31 : 0x30);
		@string[offset + 1] = (sbyte)(((value & 0x0004) != 0) ? 0x31 : 0x30);
		@string[offset + 2] = (sbyte)(((value & 0x0002) != 0) ? 0x31 : 0x30);
		@string[offset + 3] = (sbyte)(((value & 0x0001) != 0) ? 0x31 : 0x30);
	  }

	  private static void lshift1(sbyte[] Cn)
	  {
		sbyte[] hold = new sbyte[2];

		// get the two rotated bits
		hold[0] = Cn[1];
		hold[1] = Cn[29];

		// shift each position left in two 28 bit group correspondimg to Cn and Dn
		Array.Copy(Cn, 2, Cn, 1, 27);
		Array.Copy(Cn, 30, Cn, 29, 27);

		// restore the first bit of each subgroup
		Cn[28] = hold[0];
		Cn[56] = hold[1];
	  }

	  private static void lshift2(sbyte[] Cn)
	  {
		sbyte[] hold = new sbyte[4];

		hold[0] = Cn[1]; // get the four rotated bits
		hold[1] = Cn[2];
		hold[2] = Cn[29];
		hold[3] = Cn[30];

		// shift each position left in two 28 bit groups corresponding to Cn and Dn
		Array.Copy(Cn, 3, Cn, 1, 27);
		Array.Copy(Cn, 31, Cn, 29, 27);

		// restore the first bit of each subgroup
		Cn[27] = hold[0];
		Cn[28] = hold[1];
		Cn[55] = hold[2];
		Cn[56] = hold[3];
	  }

	  // void gen_pwd_sbs( byte[] user_id,
	  //                   byte[] password_token,
	  //                   byte[] password_substitute,
	  //                   byte[] pwdseq,
	  //                   byte[] rds,
	  //                   byte[] rdr)
	  //
	  // Note: rdr, rds and pwdseq are all members of the PWDSBS_SEEDS structure.
	  //
	  // Function: Generate password substitute.
	  //           Perform steps 5 to 7 of the password substitute formula.
	  //           Steps 1 to 4 were already performed by the generate password token routine.
	  //           It also generate the password verifier.
	  //
	  //  Passwor Substitute formula:
	  //  (5) Increment PWSEQs and store it.
	  //
	  //  (6) Add PWSEQs to RDr to get RDrSEQ.
	  //
	  //  (7) PW_SUB = MAC:sub.DES:esub.( PW_TOKEN,(RDrSEQ,RDs,ID xor RDrSEQ)):
	  //
	  //  LEGEND:
	  //    PW  User password
	  //    XOR  EXCLUSIVE OR
	  //    ID  User identifier
	  //    ENC:sub.DES:esub  Encipher using the Data Encryption Standard algorithm
	  //    MAC:sub.DES:esub  Generate a Message authentication code using DES
	  //    RDs  Random data sent to the partner LU on BIND
	  //    RDr  Random data received from the partner LU on BIND
	  //    PWSEQs  Sequence number for password substitution on the send side
	  //    RDrSEQ  The arithmetic sum of RDr and the current value of PWSEQs.
	  //    DES  Data Encryption Standard algorithm
	  //
	  //  Note: The MAC(DES) function was implemented according to the description given in the MI functional reference for the CIPHER function. Under the section "Cipher Block Chaining".  Basically what it says is that the MAC des use the DES algorithm to encrypt the first data block (8 bytes) the result is then exclusive ORed with the next data block and it become the data input for the DES algorithm. For subsequents blocks of data the same operation is repeated.
	  private static sbyte[] generatePasswordSubstitute(sbyte[] userID, sbyte[] token, sbyte[] password_verifier, sbyte[] sequenceNumber, sbyte[] clientSeed, sbyte[] serverSeed)
	  {
		sbyte[] RDrSEQ = new sbyte[8];
		sbyte[] nextData = new sbyte[8];
		sbyte[] nextEncryptedData = new sbyte[8];

		//first data or RDrSEQ = password sequence + host seed
		addArray(sequenceNumber, serverSeed, RDrSEQ, 8);

		// first encrypted data = DES(token, first data)
		nextEncryptedData = enc_des(token, RDrSEQ);

		// second data = first encrypted data ^ client seed
		xORArray(nextEncryptedData, clientSeed, nextData);

		// second encrypted data (password verifier) = DES(token, second data)
		nextEncryptedData = enc_des(token, nextData);

		// let's copy second encrypted password to password verifier.
		// Don't know what it is yet but will ask Leonel.
		Array.Copy(nextEncryptedData, 0, password_verifier, 0, 8);

		// third data = RDrSEQ ^ first 8 bytes of userID
		xORArray(userID, RDrSEQ, nextData);

		// third data ^= third data ^ second encrypted data
		xORArray(nextData, nextEncryptedData, nextData);

		// third encrypted data = DES(token, third data)
		nextEncryptedData = enc_des(token, nextData);

		// leftJustify the second 8 bytes of user ID
		for (int i = 0; i < 8; i++)
		{
		  nextData[i] = (sbyte)0x40;
		}

		nextData[0] = userID[8];
		nextData[1] = userID[9];

		// fourth data = second half of userID ^ RDrSEQ;
		xORArray(RDrSEQ, nextData, nextData);

		// fourth data = third encrypted data ^ fourth data
		xORArray(nextData, nextEncryptedData, nextData);

		// fourth encrypted data = DES(token, fourth data)
		nextEncryptedData = enc_des(token, nextData);

		// fifth data = fourth encrypted data ^ sequence number
		xORArray(sequenceNumber, nextEncryptedData, nextData);

		// fifth encrypted data = DES(token, fifth data) this is the encrypted password
		return enc_des(token, nextData);
	  }
	}

}