namespace HookMaker.Extensions;

public static class ConvertToBytes
{
    /// <summary>
    /// Converts any object to utf-8 byte array
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
   public static byte[] ToByteArray(this object obj)
    {
        return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj);

    }
   /// <summary>
   /// converts any byte array back to original object specified by generic typed constraint
   /// </summary>
   /// <typeparam name="T"></typeparam>
   /// <param name="obj"></param>
   /// <returns></returns>
   public static T FromByteArrayTo<T>(this byte[] obj)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(obj)!;

    }
}