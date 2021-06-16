using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RetinaNetworking.Client
{
    public class ClientSend : MonoBehaviour
    {
        private static void SendTCPData(Packet _packet)
        {
            _packet.WriteLength();
            Client.Instance.tcp.SendData(_packet);
        }


        private static void SendUDPData(Packet _packet)
        {
            _packet.WriteLength();
            Client.Instance.udp.SendData(_packet);
        }

        #region Packets

        // example packet
        // sends the received Client ID and the given client Username back to server upon connection
        public static void WelcomeReceived()
        {
            using (Packet _packet = new Packet((int)ClientPackets.welcomeReceived))
            {
                _packet.Write(Client.Instance.myID);
                _packet.Write(UIManager.Instance.usernameField.text);

                SendTCPData(_packet);
            }
        }

        // responds to the UDP test
        public static void UDPTestReceived()
        {
            using (Packet _packet = new Packet((int)ClientPackets.udpTestReceived))
            {
                _packet.Write("I received your UDP Packet.");

                SendUDPData(_packet);
            }
        }

        // sends example data packet (JSON byte array)
        public static void SendExampleData(byte[] data)
        {
            using (Packet _packet = new Packet((int)ClientPackets.exampleDataBytes))
            {
                _packet.Write(data);

                SendTCPData(_packet);
            }
        }


        // sends example data packet (JSON string)
        public static void SendExampleData(string data)
        {
            using (Packet _packet = new Packet((int)ClientPackets.exampleDataString))
            {
                _packet.Write(data);

                SendTCPData(_packet);
            }
        }

        #endregion


    }
}
