using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;


namespace RetinaNetworking.Client
{
    public class Client : MonoBehaviour
    {
        public static Client Instance;

        // buffer size for the data stream in bytes (default = 4MB)
        public static int dataBufferSize = 4096;

        public string IP = "127.0.0.1";
        public int Port = 26950;
        public int myID;
        public TCP tcp;
        public UDP udp;

        private bool isConnected = false;

        // packet handling + dictionary for storing packet handlers
        private delegate void PacketHandler(Packet _packet);
        private static Dictionary<int, PacketHandler> packetHandlers;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else if (Instance != this)
            {
                Debug.Log("Client instance already exists - destroying object");
                Destroy(this);
            }
        }

        private void Start()
        {
            tcp = new TCP();
            udp = new UDP();
        }

        private void OnApplicationQuit()
        {
            Disconnect();
        }

        public void ConnectToServer()
        {
            InitialiseClientData();
            isConnected = true;
            tcp.Connect();
        }

        public class TCP
        {
            public TcpClient socket;

            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;

            public void Connect()
            {
                socket = new TcpClient
                {
                    ReceiveBufferSize = dataBufferSize,
                    SendBufferSize = dataBufferSize
                };

                receiveBuffer = new byte[dataBufferSize];

                socket.BeginConnect(Instance.IP, Instance.Port, ConnectCallback, socket);
            }

            private void ConnectCallback(IAsyncResult _result)
            {
                socket.EndConnect(_result);

                if (!socket.Connected)
                {
                    return;
                }

                receivedData = new Packet();

                stream = socket.GetStream();

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    if (socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch (Exception ex)
                {

                    Debug.Log($"Error Sending data to server via TCP: {ex.Message}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    int _byteLength = stream.EndRead(_result);

                    // no data received
                    if (_byteLength <= 0)
                    {
                        Instance.Disconnect();
                        return;
                    }

                    // data received
                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    // Handle data - only reset if the data is complete
                    receivedData.Reset(HandleData(_data));

                    // start listening again
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Receiving TCP Data: {ex.Message}");
                    Disconnect();
                }
            }

            private bool IsStartOfPacket(Packet _packet)
            {
                // check if the received data has 4 or more unread bytes
                // if this is true - then we are at the start of a new packet
                // this is because every new packet starts with an int (which takes up 4 bytes)
                // this int represents the length of said packer
                if (_packet.UnreadLength() >= 4)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private bool IsPacketEmpty(int _packetLength)
            {
                // The packet is empty if there is less than 1 byte contained
                if (_packetLength <= 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            private bool HandleData(byte[] _data)
            {
                int _packetLength = 0;

                receivedData.SetBytes(_data);

                // check if we're at a new packet
                if(IsStartOfPacket(receivedData))
                {
                    // if so - store this packet length
                    _packetLength = receivedData.ReadInt();
                    
                    // check if empty - if so return and reset
                    if (IsPacketEmpty(_packetLength))
                    {
                        return true;
                    }
                }

                // this loop will keep running as long as receivedData contains more data packets which we can handle
                while (_packetLength > 0 && _packetLength <= receivedData.UnreadLength())
                {
                    // read the packet byte array
                    byte[] _packetBytes = receivedData.ReadBytes(_packetLength);

                    // handle the packet
                    ThreadManager.ExecuteOnMainThread(() =>
                    {
                        using (Packet _packet = new Packet(_packetBytes))
                        {
                            int _packetID = _packet.ReadInt();

                            // get the appropriate packet handler and invoke it
                            packetHandlers[_packetID](_packet);
                        }
                    });

                    // reset the packet length to zero
                    _packetLength = 0;

                    // check if theres another packet contained in the receivedData
                    if (IsStartOfPacket(receivedData))
                    {
                        // store this packet length
                        _packetLength = receivedData.ReadInt();

                        // if the packet length is less than 1 byte - then reset the data
                        if (IsPacketEmpty(_packetLength));
                        {
                            return true;
                        }
                    }
                }

                // return to reset data if the packet length is 1 byte or less
                if (_packetLength <= 1)
                {
                    return true;
                }

                // otherwise, do not reset - because there is still a partial packet left - we need to wait for the next stream to finish unpacking it
                return false;
            }

            private void Disconnect()
            {
                Instance.Disconnect();
                stream = null;
                receivedData = null;
                receiveBuffer = null;
                socket = null;
            }
        }

        public class UDP
        {
            public UdpClient socket;
            public IPEndPoint endPoint;

            public UDP()
            {
                endPoint = new IPEndPoint(IPAddress.Parse(Instance.IP), Instance.Port);
            }

            public void Connect(int _localPort)
            {
                socket = new UdpClient(_localPort);

                socket.Connect(endPoint);
                socket.BeginReceive(ReceiveCallback, null);

                // send initial packet to open ports
                using (Packet _packet = new Packet())
                {
                    SendData(_packet);
                }
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    // add the clientID to the packet, will be used to identify from server
                    _packet.InsertInt(Instance.myID);

                    // send the packet
                    if (socket != null)
                    {
                        socket.BeginSend(_packet.ToArray(), _packet.Length(), null, null);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Log($"Error sending packer by UDP: {ex.Message}");
                }
            }

            private void ReceiveCallback(IAsyncResult _result)
            {
                try
                {
                    byte[] _data = socket.EndReceive(_result, ref endPoint);
                    socket.BeginReceive(ReceiveCallback, null);

                    if (_data.Length < 4)
                    {
                        Instance.Disconnect();
                        return;
                    }

                    HandleData(_data);
                }
                catch (Exception)
                {
                    //Debug.Log($"UDP Socket has been closed - Disconnecting");
                    Disconnect();
                }
            }

            private void HandleData(byte[] _data)
            {
                using (Packet _packet = new Packet(_data))
                {
                    int _packetLength = _packet.ReadInt();

                    // remove the first 4 bytes - representing the length of the backet
                    _data = _packet.ReadBytes(_packetLength);
                }

                ThreadManager.ExecuteOnMainThread(() =>
                {
                    using(Packet _packet = new Packet(_data))
                    {
                        int _packetID = _packet.ReadInt();
                        packetHandlers[_packetID](_packet);
                    }
                });
            }

            private void Disconnect()
            {
                Instance.Disconnect();

                endPoint = null;
                socket = null;
            }
        }


        private void InitialiseClientData()
        {
            packetHandlers = new Dictionary<int, PacketHandler>()
            {
                {(int)ServerPackets.welcome, ClientHandle.Welcome },
                {(int)ServerPackets.udpTest, ClientHandle.UDPTest },
                {(int)ServerPackets.exampleDataBytesReceived, ClientHandle.ExampleDataBytesReceived }
            };

            Debug.Log("Initialised Packets!");
        }


        private void Disconnect()
        {
            if (isConnected)
            {
                isConnected = false;

                udp.socket.Close();
                tcp.socket.Close();

                Debug.Log("Disconnected from the server.");
            }
        }
    }

}
