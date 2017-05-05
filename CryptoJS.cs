using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace fpgx
{
    class CryptoJS
    {
        public static byte[] HexStringToByteArray(string strHex)
        {
            dynamic r = new byte[strHex.Length / 2];
            for (int i = 0; i <= strHex.Length - 1; i += 2)
            {
                r[i / 2] = Convert.ToByte(Convert.ToInt32(strHex.Substring(i, 2), 16));
            }
            return r;
        }

        private static string AESDecrypt(string text, string salt, string pass, string iv)
        {
            byte[] ivBytes = HexStringToByteArray(iv);

            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;
            rijndaelCipher.Key = generateKey(salt, pass);
            rijndaelCipher.IV = ivBytes;

            ICryptoTransform transform = rijndaelCipher.CreateDecryptor();
            byte[] encryptedData = Convert.FromBase64String(text);
            byte[] plainText = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

            return Encoding.UTF8.GetString(plainText);
        }

        private static byte[] generateKey(string salt, string pass)
        {
            Rfc2898DeriveBytes k2 = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(pass), HexStringToByteArray(salt), 100);
            return k2.GetBytes(16);
        }
    }

    /*
     * http://stackoverflow.com/questions/19094547/aes-encryption-in-c-sharp-and-decryption-in-cryptojs
     */

    /*
It is working now after getting some reference from Google CryptoJS group (https://groups.google.com/forum/#!msg/crypto-js/ysgzr2Wxt_k/_Wh8l_1rhQAJ).

Here is encryption code in C#.NET.

public class ClsCrypto
{
    private RijndaelManaged myRijndael = new RijndaelManaged();
    private int iterations;
    private byte [] salt;

    public ClsCrypto(string strPassword)
    {
        myRijndael.BlockSize = 128;
        myRijndael.KeySize = 128;
        myRijndael.IV = HexStringToByteArray("e84ad660c4721ae0e84ad660c4721ae0");

        myRijndael.Padding = PaddingMode.PKCS7;
        myRijndael.Mode = CipherMode.CBC;
        iterations = 1000;
        salt = System.Text.Encoding.UTF8.GetBytes("insight123resultxyz");
        myRijndael.Key = GenerateKey(strPassword);
    }

    public string Encrypt(string strPlainText)
    {
        byte [] strText = new System.Text.UTF8Encoding().GetBytes(strPlainText);
        ICryptoTransform transform = myRijndael.CreateEncryptor();
        byte [] cipherText = transform.TransformFinalBlock(strText, 0, strText.Length);

        return Convert.ToBase64String(cipherText);
    }

    public static byte [] HexStringToByteArray(string strHex)
    {
        dynamic r = new byte[strHex.Length / 2];
        for (int i = 0; i <= strHex.Length - 1; i += 2)
        {
            r[i/2] = Convert.ToByte(Convert.ToInt32(strHex.Substring(i, 2), 16));
        }
        return r;
    }

    private byte[] GenerateKey(string strPassword)
    {
        Rfc2898DeriveBytes rfc2898 = new          Rfc2898DeriveBytes(System.Text.Encoding.UTF8.GetBytes(strPassword), salt, iterations);

        return rfc2898.GetBytes(128 / 8);
    }
}

Following is decryption code in Java script.
<head runat="server">
    <script src="rollups/aes.js" type="text/javascript"></script>
    <script src="rollups/sha256.js" type="text/javascript"></script>
    <script src="rollups/pbkdf2.js" type="text/javascript"></script>
    <script type="text/javascript">
      function DecryptData() {
        var encryptData = document.getElementById('TextEncrypted').value;
        var decryptElement = document.getElementById('TextDecrypt');

        try {
            //Creating the Vector Key
            var iv = CryptoJS.enc.Hex.parse('e84ad660c4721ae0e84ad660c4721ae0');
            //Encoding the Password in from UTF8 to byte array
            var Pass = CryptoJS.enc.Utf8.parse('insightresult');
            //Encoding the Salt in from UTF8 to byte array
            var Salt = CryptoJS.enc.Utf8.parse("insight123resultxyz");
            //Creating the key in PBKDF2 format to be used during the decryption
            var key128Bits1000Iterations = CryptoJS.PBKDF2(Pass.toString(CryptoJS.enc.Utf8), Salt, { keySize: 128 / 32, iterations: 1000 });
            //Enclosing the test to be decrypted in a CipherParams object as supported by the CryptoJS libarary
            var cipherParams = CryptoJS.lib.CipherParams.create({
                ciphertext: CryptoJS.enc.Base64.parse(encryptData)
            });

            //Decrypting the string contained in cipherParams using the PBKDF2 key
            var decrypted = CryptoJS.AES.decrypt(cipherParams, `enter code here`key128Bits1000Iterations, { mode: CryptoJS.mode.CBC, iv: iv, padding: CryptoJS.pad.Pkcs7 });
            decryptElement.value = decrypted.toString(CryptoJS.enc.Utf8);
        }
        //Malformed UTF Data due to incorrect password
        catch (err) {
            return "";
        }
    }
  </script>
     */
}
