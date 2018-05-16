﻿
namespace RedmineMailService
{


    public class AES
    {
        protected static string strKey = "1b55ec1d96f637aa7b73c31765a12c2c8fb8b9f6ae8b14396475a20ed1a83dac";
        protected static string strIV = "d4e3381cdd39ddb70f85e96d11b667e5";


        public static string GetKey()
        {
            return strKey;
        } // End Sub GetKey


        public static string GetIV()
        {
            return strIV;
        } // End Sub GetIV


        public static void SetKey(ref string strInputKey)
        {
            strKey = strInputKey;
        } // End Sub SetKey 


        public static void SetIV(ref string strInputIV)
        {
            strIV = strInputIV;
        } // End Sub SetIV


        public static string GenerateKey()
        {
            System.Security.Cryptography.RijndaelManaged objRijndael = new System.Security.Cryptography.RijndaelManaged();

            objRijndael.GenerateKey();
            objRijndael.GenerateIV();

            byte[] bIV = objRijndael.IV;
            byte[] bKey = objRijndael.Key;
            objRijndael.Clear();

            return "IV: " + ByteArrayToHexString(bIV) + System.Environment.NewLine + "Key: " + ByteArrayToHexString(bKey);
        } // End Function GenerateKey



        public static string Encrypt(string strPlainText)
        {
            //Dim roundtrip As String
            //Dim encASCII As New System.Text.ASCIIEncoding()
            System.Text.Encoding enc = System.Text.Encoding.UTF8;
            System.Security.Cryptography.RijndaelManaged objRijndael = new System.Security.Cryptography.RijndaelManaged();
            //Dim fromEncrypt() As Byte
            byte[] baCipherTextBuffer = null;
            byte[] baPlainTextBuffer = null;
            byte[] baEncryptionKey = null;
            byte[] baInitializationVector = null;

            //Create a new key and initialization vector.
            //objRijndael.GenerateKey()
            //objRijndael.GenerateIV()
            objRijndael.Key = HexStringToByteArray(strKey);
            objRijndael.IV = HexStringToByteArray(strIV);


            //Get the key and initialization vector.
            baEncryptionKey = objRijndael.Key;
            baInitializationVector = objRijndael.IV;
            //strKey = ByteArrayToHexString(baEncryptionKey)
            //strIV = ByteArrayToHexString(baInitializationVector)

            //Get an encryptor.
            System.Security.Cryptography.ICryptoTransform ifaceAESencryptor = objRijndael.CreateEncryptor(baEncryptionKey, baInitializationVector);

            //Encrypt the data.
            System.IO.MemoryStream msEncrypt = new System.IO.MemoryStream();
            System.Security.Cryptography.CryptoStream csEncrypt = new System.Security.Cryptography.CryptoStream(msEncrypt, ifaceAESencryptor, System.Security.Cryptography.CryptoStreamMode.Write);

            //Convert the data to a byte array.
            baPlainTextBuffer = enc.GetBytes(strPlainText);

            //Write all data to the crypto stream and flush it.
            csEncrypt.Write(baPlainTextBuffer, 0, baPlainTextBuffer.Length);
            csEncrypt.FlushFinalBlock();

            //Get encrypted array of bytes.
            baCipherTextBuffer = msEncrypt.ToArray();

            return ByteArrayToHexString(baCipherTextBuffer);
        } // End Function Encrypt


        public static string DeCrypt(string strEncryptedInput)
        {
            string strReturnValue = null;

            if (string.IsNullOrEmpty(strEncryptedInput))
            {
                throw new System.ArgumentNullException("strEncryptedInput", "strEncryptedInput may not be string.Empty or NULL, because these are invid values.");
            }

            // Dim encASCII As New System.Text.ASCIIEncoding()
            System.Text.Encoding enc = System.Text.Encoding.UTF8;

            System.Security.Cryptography.RijndaelManaged objRijndael = new System.Security.Cryptography.RijndaelManaged();

            byte[] baCipherTextBuffer = HexStringToByteArray(strEncryptedInput);


            byte[] baDecryptionKey = HexStringToByteArray(strKey);
            byte[] baInitializationVector = HexStringToByteArray(strIV);

            // This is where the message would be transmitted to a recipient
            // who already knows your secret key. Optionally, you can
            // also encrypt your secret key using a public key algorithm
            // and pass it to the mesage recipient along with the RijnDael
            // encrypted message.            
            //Get a decryptor that uses the same key and IV as the encryptor.
            System.Security.Cryptography.ICryptoTransform ifaceAESdecryptor = objRijndael.CreateDecryptor(baDecryptionKey, baInitializationVector);

            //Now decrypt the previously encrypted message using the decryptor
            // obtained in the above step.
            System.IO.MemoryStream msDecrypt = new System.IO.MemoryStream(baCipherTextBuffer);
            System.Security.Cryptography.CryptoStream csDecrypt = new System.Security.Cryptography.CryptoStream(msDecrypt, ifaceAESdecryptor, System.Security.Cryptography.CryptoStreamMode.Read);

            //Dim baPlainTextBuffer() As Byte
            //baPlainTextBuffer = New Byte(baCipherTextBuffer.Length) {}
            byte[] baPlainTextBuffer = new byte[baCipherTextBuffer.Length + 1];

            //Read the data out of the crypto stream.
            csDecrypt.Read(baPlainTextBuffer, 0, baPlainTextBuffer.Length);

            //Convert the byte array back into a string.
            strReturnValue = enc.GetString(baPlainTextBuffer);
            if (!string.IsNullOrEmpty(strReturnValue))
                strReturnValue = strReturnValue.Trim('\0');

            return strReturnValue;
        } // End Function DeCrypt


        // VB.NET to convert a byte array into a hex string
        public static string ByteArrayToHexString(byte[] arrInput)
        {
            System.Text.StringBuilder strOutput = new System.Text.StringBuilder(arrInput.Length);

            for (int i = 0; i <= arrInput.Length - 1; i++)
            {
                strOutput.Append(arrInput[i].ToString("X2"));
            }

            return strOutput.ToString().ToLower();
        } // End Function ByteArrayToHexString


        public static byte[] HexStringToByteArray(string strHexString)
        {
            int iNumberOfChars = strHexString.Length;
            byte[] baBuffer = new byte[iNumberOfChars / 2];
            for (int i = 0; i <= iNumberOfChars - 1; i += 2)
            {
                baBuffer[i / 2] = System.Convert.ToByte(strHexString.Substring(i, 2), 16);
            }
            return baBuffer;
        } // End Function HexStringToByteArray

    } // End Class AES 


} // End Namespace COR.Tools.Cryptography
