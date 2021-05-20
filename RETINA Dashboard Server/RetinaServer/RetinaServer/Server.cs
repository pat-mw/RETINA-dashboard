using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace RetinaServer
{
    class Server
    {
        public static int MaxPlayers { get; private set; }
        public static int Port { get; private set; }
        public static Dictionary<int, Client> Clients = new Dictionary<int, Client>();

        // packet handling - with the extra info of the client that sent the packet
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;


        private static TcpListener tcpListener;

        public static void Start(int _MaxPlayers, int _Port)
        {
            MaxPlayers = _MaxPlayers;
            Port = _Port;

            Console.WriteLine("Starting Server...");
            InitialiseServerData();

            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);


            Console.WriteLine($"Server started on Port: {Port}");

        }

        private static void TCPConnectCallback(IAsyncResult _result)
        {
            TcpClient _client = tcpListener.EndAcceptTcpClient(_result);

            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null); // keep listening 

            Console.WriteLine($"Incoming connection from {_client.Client.RemoteEndPoint}...");

            for (int i = 1; i <= MaxPlayers; i++)
            {
                // check if empty slot
                if (Clients[i].tcp.socket == null)
                {
                    // connect the new client to a single open slot and return
                    Clients[i].tcp.Connect(_client);
                    return;
                }
            }

            // if the loop executes to completion, the server is full
            Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect - the server is full! (Max Capacity: {MaxPlayers})");

        }


        private static void InitialiseServerData() 
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                Clients.Add(i, new Client(i));
            }

            // initialise the packet handler dict
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived }
            };

            Console.WriteLine("Initialised packets...");
        }
    }
}
