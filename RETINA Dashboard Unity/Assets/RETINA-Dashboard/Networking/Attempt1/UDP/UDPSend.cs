using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;


namespace RetinaNetworking
{
    public class UDPSend : MonoBehaviour
    {
        public string IP;
        public IPAddress hostAddress;
        public int sendPort;

        private IPEndPoint IPEndpoint;
        private UdpClient client;
        private Thread networkThread;

        private byte[] sendData;
        private List<byte[]> dataHistory;

        void Start()
        {
            networkThread = new Thread(SendData);
            networkThread.Start();

            sendData = new byte[0];

            IPEndpoint = new IPEndPoint(IPAddress.Any, sendPort);
            client = new UdpClient(IPEndpoint);
        }

        public void SendData(object data)
        {
            int size;
            // encode data, calculate byte size, and send
            sendData = DataHandler.Encode(data, out size);
            client.Send(sendData, size);
            dataHistory.Add(sendData);
        }
    }
}