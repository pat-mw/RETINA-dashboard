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
        public static Dictionary<int, Client> clients = new Dictionary<int, Client>();

        // packet handling - with the extra info of the client that sent the packet
        public delegate void PacketHandler(int _fromClient, Packet _packet);
        public static Dictionary<int, PacketHandler> packetHandlers;


        private static TcpListener tcpListener;
        private static UdpClient udpListener;
        public static void Start(int _MaxPlayers, int _Port)
        {
            MaxPlayers = _MaxPlayers;
            Port = _Port;

            Console.WriteLine("Starting Server...");
            InitialiseServerData();

            // TCP set up
            tcpListener = new TcpListener(IPAddress.Any, Port);
            tcpListener.Start();
            tcpListener.BeginAcceptTcpClient(new AsyncCallback(TCPConnectCallback), null);

            // UDP set up
            udpListener = new UdpClient(Port);
            udpListener.BeginReceive(UDPReceiveCallback, null);

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
                if (clients[i].tcp.socket == null)
                {
                    // connect the new client to a single open slot and return
                    clients[i].tcp.Connect(_client);
                    return;
                }
            }

            // if the loop executes to completion, the server is full
            Console.WriteLine($"{_client.Client.RemoteEndPoint} failed to connect - the server is full! (Max Capacity: {MaxPlayers})");
        }

        private static void UDPReceiveCallback(IAsyncResult _result)
        {
            try
            {
                // create new IP Endpoint with no specific address or port
                IPEndPoint _clientEndPoint = new IPEndPoint(IPAddress.Any, 0);

                // receive data and reopen listener immediately
                byte[] _data = udpListener.EndReceive(_result, ref _clientEndPoint);
                udpListener.BeginReceive(UDPReceiveCallback, null);

                if (_data.Length < 4)
                {
                    return;
                }

                using (Packet _packet = new Packet(_data))
                {
                    int _clientID = _packet.ReadInt();

                    if (_clientID == 0)
                    {
                        Console.WriteLine("something went wrong - client ID cannot be zero");
                        return;
                    }

                    // check if senders endpoint is null
                    if (clients[_clientID].udp.endPoint == null)
                    {
                        // if true then this is a new connection - so we must connect
                        clients[_clientID].udp.Connect(_clientEndPoint);

                        // safe to return because the first packet is empty
                        return;
                    }

                    // check if client endpoint matches local endpoint (i.e. no impostor)
                    if (clients[_clientID].udp.endPoint.ToString() == _clientEndPoint.ToString())
                    {
                        clients[_clientID].udp.HandleData(_packet);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error receiving UDP data: {ex.Message}");
            }
        }

        public static void SendUDPData(IPEndPoint _clientEndPoint, Packet _packet)
        {
            try
            {
                if (_clientEndPoint != null)
                {
                    udpListener.BeginSend(_packet.ToArray(), _packet.Length(), _clientEndPoint, null, null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data to {_clientEndPoint} via UDP: {ex.Message}");
            }
        }


        private static void InitialiseServerData() 
        {
            for (int i = 1; i <= MaxPlayers; i++)
            {
                clients.Add(i, new Client(i));
            }

            // initialise the packet handler dict
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ClientPackets.welcomeReceived, ServerHandle.WelcomeReceived },
                {(int)ClientPackets.udpTestReceived, ServerHandle.UDPTestReceived }
            };

            Console.WriteLine("Initialised packets...");
        }
    }
}
