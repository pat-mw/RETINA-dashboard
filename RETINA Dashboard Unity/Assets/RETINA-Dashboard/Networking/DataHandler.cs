using System;
using Sirenix.Serialization;

namespace RetinaNetworking
{
    public class DataHandler
    {
        public static object Decode(byte[] rawdatas, Type targetType)
        {
            return "coomer";
        }

        public static byte[] Encode(object dataToSend, out int size)
        {
            byte[] data;

            data = SerializationUtility.SerializeValueWeak(dataToSend, DataFormat.JSON);
            size = data.Length;

            return data;
        }
    }
}

