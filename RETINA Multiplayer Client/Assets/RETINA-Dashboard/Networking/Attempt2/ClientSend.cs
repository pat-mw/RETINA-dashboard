using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RetinaNetworking
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

        #endregion


    }
}
