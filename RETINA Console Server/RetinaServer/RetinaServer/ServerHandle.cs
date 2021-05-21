using System;
using System.Collections.Generic;
using System.Text;

namespace RetinaServer
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            // important to read data in the same order as it was sent
            // in this case - the welcome received message sent from the client was an int, then a string
            int _clientIDCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Console.WriteLine($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected succesfully | ID: {_fromClient} - username: {_username}");
        
            // double check that client claimed the right id
            if(_fromClient != _clientIDCheck)
            {
                Console.WriteLine($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong Client ID ({_clientIDCheck})");
            }
        }


        public static void UDPTestReceived(int _fromClient, Packet _packet)
        {
            string _msg = _packet.ReadString();

            Console.WriteLine($"Received UDP Packet | Client ID: {_fromClient} - Message: {_msg}");
        }
    }
}
