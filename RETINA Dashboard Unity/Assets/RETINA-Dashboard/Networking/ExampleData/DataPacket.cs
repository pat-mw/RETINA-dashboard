using System;
using UnityEngine;


namespace RetinaNetworking
{
    [CreateAssetMenu(menuName ="Networking/Data Packet")]
    [Serializable]
    public class DataPacket : ScriptableObject
    {
        [SerializeField] int deviceID;
        [SerializeField] int packetNo;
        [SerializeField] double timeStamp;
        [SerializeField] Data data;
    }
}

