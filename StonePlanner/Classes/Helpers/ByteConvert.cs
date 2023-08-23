using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace StonePlanner.Classes.Helpers
{
    internal static class ByteConvert
    {
        /// <summary>
        /// 将一个object对象序列化，返回一个byte[]
        /// </summary>
        /// <param name="obj">能序列化的对象</param>
        /// <returns></returns>
        public static byte[] ObjectToBytes(object obj)
        {
            using var ms = new MemoryStream();
            IFormatter formatter = new BinaryFormatter();
            formatter.Serialize(ms, obj);
            return ms.GetBuffer();
        }

        /// <summary>
        /// 将一个序列化后的byte[]数组还原
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static object BytesToObject(byte[] bytes)
        {
            using var ms = new MemoryStream(bytes);
            IFormatter formatter = new BinaryFormatter();
            return formatter.Deserialize(ms);
        }
    }
}
