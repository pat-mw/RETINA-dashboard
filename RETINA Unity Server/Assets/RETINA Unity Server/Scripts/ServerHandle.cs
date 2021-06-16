using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Wenzil.Console;


namespace RetinaNetworking.Server
{
    class ServerHandle
    {
        public static void WelcomeReceived(int _fromClient, Packet _packet)
        {
            // important to read data in the same order as it was sent
            // in this case - the welcome received message sent from the client was an int, then a string
            int _clientIDCheck = _packet.ReadInt();
            string _username = _packet.ReadString();

            Wenzil.Console.Console.Log($"{Server.clients[_fromClient].tcp.socket.Client.RemoteEndPoint} connected succesfully | ID: {_fromClient} - username: {_username}");

            // double check that client claimed the right id
            if (_fromClient != _clientIDCheck)
            {
                Wenzil.Console.Console.Log($"Player \"{_username}\" (ID: {_fromClient}) has assumed the wrong Client ID ({_clientIDCheck})");
            }

            // save the client username
            Server.clients[_fromClient].username = _username;

            Wenzil.Console.Console.Log($"Saved Client Username: {Server.clients[_fromClient].username}");
            ClientDetailsUI.Instance.AddClientPanel(Server.clients[_fromClient]);
        }


        public static void UDPTestReceived(int _fromClient, Packet _packet)
        {
            string _msg = _packet.ReadString();

            Wenzil.Console.Console.Log($"Received UDP Packet | Client ID: {_fromClient} - Message: {_msg}");
        }


        public static void ExampleDataBytes(int _fromClient, Packet _packet)
        {
            byte[] msg = _packet.ReadBytes(_packet.UnreadLength());

            Wenzil.Console.Console.Log($"Client {_fromClient} (username: {Server.clients[_fromClient].username}) has sent some example data (bytes)");
            NestedData deserialized = JSONHandler.DecodeNestedByteArray(msg);

            // save the incoming data
            string path;
            DataSave.SaveByteData(msg, _fromClient, out path);

            // log the incoming data in the Client Panels
            //ClientDetailsUI.Instance.LogClientData(Server.clients[_fromClient], msg.ToString());
            ClientDetailsUI.Instance.LogClientData(Server.clients[_fromClient], msg, path);

            // send reception message back to client
            ServerSend.ExampleDataBytesReceived(_fromClient);
        }
    }
}
