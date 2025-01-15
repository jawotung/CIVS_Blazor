using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Common;
public static class EncryptDecrypt
{
    public static string NewKeyHash { get; set; } = string.Empty;
    private static readonly string DefaultKeyHash = "E546C8DF278CD5931069B522E695D4F2";
    private static string KeyHash { get {  return NewKeyHash == string.Empty ? DefaultKeyHash : NewKeyHash; } }
    

    public static string Encrypt(this string text)
    {
        var key = Encoding.UTF8.GetBytes(KeyHash);

        using (var aesAlg = Aes.Create())
        {
            using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
            {
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(text);
                    }

                    var iv = aesAlg.IV;

                    var decryptedContent = msEncrypt.ToArray();

                    var result = new byte[iv.Length + decryptedContent.Length];

                    Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                    Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                    return Convert.ToBase64String(result);
                }
            }
        }
    }

    public static string Decrypt(this string text)
    {
        if (string.IsNullOrEmpty(text))
            return text;

        if (!text.IsBase64String())
            return text;

        var fullCipher = text.FromBase64String();

        var iv = new byte[16];
        var cipher = new byte[fullCipher.Length - iv.Length]; //changes here

        Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
        // Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, cipher.Length);
        Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length); // changes here
        var key = Encoding.UTF8.GetBytes(KeyHash);

        using (var aesAlg = Aes.Create())
        {
            using (var decryptor = aesAlg.CreateDecryptor(key, iv))
            {
                string result;
                using (var msDecrypt = new MemoryStream(cipher))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            result = srDecrypt.ReadToEnd();
                        }
                    }
                }

                return result;
            }
        }
    }
} 