using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace RetinaNetworking
{
    public class DataHandlerOld
    {
        public static object Decode(byte[] rawdatas, Type targetType)
        {
            // check byte size of the given target type
            int rawsize = Marshal.SizeOf(targetType);

            // if the data cannot fit in the given type, return null
            if (rawsize > rawdatas.Length)
            {
                Debug.Log("data cannot fit in the given type");
                return null;
            }

            // open handle
            GCHandle handle = GCHandle.Alloc(rawdatas, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();

            // create return object
            object retobj = Marshal.PtrToStructure(buffer, targetType);
            handle.Free();
            return retobj;
        }

        public static byte[] Encode(object obj, out int size)
        {
            // get byte size and pass to out parameter
            int rawsize = Marshal.SizeOf(obj);
            size = rawsize;

            byte[] rawdatas = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdatas, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            Marshal.StructureToPtr(obj, buffer, false);
            handle.Free();
            return rawdatas;
        }
    }
}

