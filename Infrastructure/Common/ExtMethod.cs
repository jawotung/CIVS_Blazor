using Application.Models;
//using Crypto.CryptoManager;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using System.Reflection;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Infrastructure.Common
{
    public static class ExtMethods
    {
        //#region Encryptions
        //private static CryptoProvider cryptoProvider = new CryptoProvider("B0CP#o3n!X", "B@nCoMm3Rc3!@#$%");
        ///// <summary>
        ///// Encrypt the string
        ///// </summary> 
        ///// <returns>Encrypted String</returns>
        //public static string Encrypt(this string str)
        //{
        //    return cryptoProvider.Encrypt(str);
        //}

        ///// <summary>
        ///// Encrypt the string
        ///// </summary> 
        ///// <returns>Byte Array</returns>
        //public static byte[] EncryptToBytes(this string str)
        //{
        //    return cryptoProvider.EncryptToBytes(str);
        //}

        ///// <summary>
        ///// Decrypt the string
        ///// </summary> 
        ///// <returns>Decrypted String</returns>
        //public static string Decrypt(this string str)
        //{
        //    string x = "";
        //    try
        //    {
        //        x = cryptoProvider.Decrypt(str);
        //        //CryptoProvider c = new CryptoProvider("B0CP#o3n!X", "B@nCoMm3Rc3!@#$%");
        //        //x = c.Decrypt(str);
        //    }
        //    catch (OverflowException ex)
        //    {
        //        Console.WriteLine("Overflow exception: " + ex.Message);
        //        // Additional logging or error handling
        //    }
        //    catch (CryptographicException ex)
        //    {
        //        Console.WriteLine("Cryptographic exception: " + ex.Message);
        //        // Additional logging or error handling
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("General exception: " + ex.Message);
        //        // Additional logging or error handling
        //    }
        //    return x;
        //    //return cryptoProvider.Decrypt(str);
        //}

        ///// <summary>
        ///// Decrypt the string
        ///// </summary> 
        ///// <returns>Byte Array</returns>
        //public static byte[] DecryptToBytes(this string str)
        //{
        //    return cryptoProvider.DecryptToBytes(str);
        //}

        //public static string ToJsonString<T>(this T value)
        //{
        //    string result = "{}";
        //    var serializer = new JsonSerializer();
        //    using (var writer = new StringWriter())
        //    {
        //        serializer.Serialize(writer, value);
        //        result = writer.ToString();
        //    }

        //    return result;
        //}

        //public static T FromJsonString<T>(this string value) where T : new()
        //{
        //    try
        //    {
        //        var serializer = new JsonSerializer();
        //        using (var stringReader = new StringReader(value))
        //        {
        //            using (var jsonReader = new JsonTextReader(stringReader))
        //            {
        //                return serializer.Deserialize<T>(jsonReader);
        //            }
        //        }
        //    }
        //    catch (Exception e) { return new T(); }
        //}

        //#endregion

        public static string GetFileWOExt(this string filename)
        {
            return Path.GetFileNameWithoutExtension(filename);
        }
        public static bool IsNull(this object thisObj)
        {
            return ReferenceEquals(null, thisObj);
        }
        public static MemoryStream ToPng(this Stream thisStream)
        {
            using (var stream = new MemoryStream()) { Bitmap.FromStream(thisStream).Save(stream, ImageFormat.Png); return stream; }
        }
        public static bool IsCorruptedImage(this byte[] imageData)
        {
            try
            {
                using (var memoryStream = new MemoryStream(imageData))
                {
                    using (var image = System.Drawing.Image.FromStream(memoryStream))
                    {
                        // If the image can be loaded without exceptions, it's not corrupt
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception if needed
                Console.WriteLine($"Error while checking image: {ex.Message}");
                return true;
            }
        }
        public static string ConvertToPngBase64(this byte[] imageData)
        {
            try
            {
                using (var memoryStream = new MemoryStream(imageData))
                {
                    using (var image = Image.FromStream(memoryStream))
                    {
                        // Create a new MemoryStream to store the PNG data
                        using (var pngMemoryStream = new MemoryStream())
                        {
                            // Save the image as PNG to the PNG MemoryStream
                            image.Save(pngMemoryStream, System.Drawing.Imaging.ImageFormat.Png);

                            // Convert the PNG MemoryStream to a base64 encoded string
                            return Convert.ToBase64String(pngMemoryStream.ToArray());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log or handle the exception if needed
                Console.WriteLine($"Error while converting image to PNG: {ex.Message}");
                return null;
            }
        }
        public static string ToBase64String(this MemoryStream thisStream)
        {
            return Convert.ToBase64String(thisStream.ToArray());
        }
        public static string GetValue(this object thisObj)
        {
            return thisObj.IsNull() ? "" : thisObj.ToString();
        }
        public static double ToDouble(this object thisObj, int iDecimalPosition = 2)
        {
            string thisStr = thisObj.GetValue();
            if (thisStr.Length > iDecimalPosition)
            {
                if (thisObj.GetFloatValue().IndexOf('.') < 0)
                {
                    iDecimalPosition = thisObj.GetValue().Length - iDecimalPosition;
                    thisStr = thisStr.Insert(iDecimalPosition, ".");
                }
                double.TryParse(thisStr, out double res);
                return res;
            }
            return 0.00;
        }
        public static string GetFloatValue(this object thisObj)
        {
            double.TryParse(thisObj.ToString(), out double res);
            return res.ToString();
        }
        public static int ToInt(this string thisStr)
        {
            int.TryParse(thisStr, out int res);
            return res;
        }

        public static bool GetBoolValue(this object thisObj)
        {
            bool.TryParse(thisObj.IsNull() ? "" : thisObj.ToString(), out bool bResult);
            return bResult;
        }

        public static string GetDateValue(this object thisObj)
        {
            return thisObj.IsValidDate() ? thisObj.ToDate().ToString("MM/dd/yyyy") : "";
        }
        public static bool IsValidDate(this object thisObj)
        {
            DateTime _thisObj;
            bool result = false;
            if (thisObj.IsNull())
                return result;

            if (thisObj.GetType() == typeof(DateTime))
            {
                _thisObj = thisObj.ToDate();
                result = _thisObj.Year.Equals(1) && _thisObj.Day.Equals(1) && _thisObj.Month.Equals(1) ? false : true;
            }
            return result;
        }
        public static DateTime ToDate(this object thisStr)
        {
            DateTime res = DateTime.Now;
            DateTime.TryParse(thisStr.IsNull() ? "" : thisStr.ToString(), out res);
            return res.Date;
        }
        public static string ToDate(this object thisStr, string format)
        {
            return thisStr.ToDate().ToString(format);
        }
        public static UserClaims getUserClaims(this ClaimsPrincipal User)
        {
            UserClaims userClaims = new UserClaims();

            if (User != null)
            {
                PropertyInfo[] infos = userClaims.GetType().GetProperties(BindingFlags.Instance
                   | BindingFlags.NonPublic
                   | BindingFlags.Public);

                foreach (PropertyInfo info in infos)
                {
                    var val = User.Claims.FirstOrDefault(u => u.Type == info.Name);
                    if (val != null)
                        info.SetValue(userClaims, val.Value);
                }
            }
            return userClaims;
        }
        public static bool IsBase64String(this string thisString)
        {
            try { Convert.FromBase64String(thisString); return true; } catch (Exception e) { _ = e; return false; }
        }
        public static byte[] FromBase64String(this string thisString)
        {
            return Convert.FromBase64String(thisString);
        }
        public static string ByteArrayToPngBase64String(this byte[] bytesArr)
        {
            if (bytesArr.IsNull())
                return "";
            //return new MemoryStream(bytesArr).ToTiff().ToBase64String();
            return new MemoryStream(bytesArr).ToPng().ToBase64String();
        }
        public static bool IsCorruptedImage(this Stream thisStream)
        {
            try
            {
                using (var stream = new MemoryStream())
                {
                    Bitmap.FromStream(thisStream).Save(stream, ImageFormat.Png);
                }
                return false;
            }
            catch (Exception e) { _ = e; return true; }
        }
        #region get embeded resource
        public static string ReadResource(this string name, bool isImage = false)
        {
            // Determine path
            var assembly = Assembly.GetExecutingAssembly();
            string resourcePath = name;
            // Format: "{Namespace}.{Folder}.{filename}.{Extension}"
            if (!name.StartsWith(nameof(name)))
            {
                resourcePath = assembly.GetManifestResourceNames()
                    .Single(str => str.EndsWith(name));
            }

            using (Stream stream = assembly.GetManifestResourceStream(resourcePath))
            {
                if (isImage)
                    return stream.ToPng().ToBase64String();
                //return string.Format("data: image/jpeg; base64 , {0}", stream.ToPng().ToBase64String());

                using (StreamReader reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
        #endregion
    }
}
