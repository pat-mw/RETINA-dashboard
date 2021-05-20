﻿using System;
using System.Collections.Generic;
using System.Text;

namespace RetinaServer
{
    class ServerSend
    {
        private static void SendTCPData(int _toClient, Packet _packet)
        {
            // get the length of the message
            _packet.WriteLength();

            Server.Clients[_toClient].tcp.SendData(_packet);

        }

        private static void SendTCPDataToAll(Packet _packet)
        {
            _packet.WriteLength();

            for (int i = 1; i <= Server.MaxPlayers; i++)
            { 
                Server.Clients[i].tcp.SendData(_packet);
            }
        }

        private static void SendTCPDataToAllExcept(int _exceptClient, Packet _packet)
        {
            _packet.WriteLength();

            for (int i = 1; i <= Server.MaxPlayers; i++)
            {
                if (i != _exceptClient)
                {
                    Server.Clients[i].tcp.SendData(_packet);
                }
            }
        }

        public static void Welcome(int _toClient, string _msg)
        {
            using (Packet _packet = new Packet((int)ServerPackets.welcome))
            {
                _packet.Write(_msg);
                _packet.Write(_toClient);

                SendTCPData(_toClient, _packet);
            }
        }

        
    }
}