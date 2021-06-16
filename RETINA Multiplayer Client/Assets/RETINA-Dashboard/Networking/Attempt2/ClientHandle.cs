using UnityEngine;
using System.Net;

namespace RetinaNetworking.Client
{
    public class ClientHandle : MonoBehaviour
    {
        public static void Welcome(Packet _packet)
        {
            string _msg = _packet.ReadString();
            int _myID = _packet.ReadInt();

            Debug.Log($"Received TCP Packet from Server | Message: {_msg}");
            Client.Instance.myID = _myID;
            ClientSend.WelcomeReceived();

            // open the udp connection using the same port as the tcp connection
            Client.Instance.udp.Connect(((IPEndPoint)Client.Instance.tcp.socket.Client.LocalEndPoint).Port);
        }


        public static void UDPTest(Packet _packet)
        {
            string _msg = _packet.ReadString();

            Debug.Log($"Received UDP Packet from Server | Message: {_msg}");
            ClientSend.UDPTestReceived();
        }


        public static void ExampleDataBytesReceived(Packet _packet)
        {
            string _msg = _packet.ReadString();

            Debug.Log($"Received TCP Package from Server | Message: {_msg}");
        }
    }
}