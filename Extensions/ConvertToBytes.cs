namespace HookMaker.Extensions;

public static class ConvertToBytes
{
   public static byte[] ToByteArray(this object obj)
    {
        return System.Text.Json.JsonSerializer.SerializeToUtf8Bytes(obj);

    }
   
   public static T FromByteArrayTo<T>(this byte[] obj)
    {
        return System.Text.Json.JsonSerializer.Deserialize<T>(obj)!;

    }
}