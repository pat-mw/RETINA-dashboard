﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace RetinaServer
{
    class Client
    {
        // Size of the Data Buffer in Bytes (default 4MB)
        public static int dataBufferSize = 4096; 

        public int id;
        public TCP tcp;

        // client constructor
        public Client(int _id)
        {
            id = _id;
            tcp = new TCP(id);
        }

        #region TCP

        public class TCP
        {
            public TcpClient socket;

            private readonly int ID;
            private NetworkStream stream;
            private Packet receivedData;
            private byte[] receiveBuffer;


            // constructor
            public TCP(int _id)
            {
                ID = _id;
            }

            public void Connect(TcpClient _socket)
            {
                socket = _socket;
                socket.ReceiveBufferSize = dataBufferSize;
                socket.SendBufferSize = dataBufferSize;

                stream = socket.GetStream();

                receivedData = new Packet();

                receiveBuffer = new byte[dataBufferSize];

                stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);

                // send welcome packet to the client
                ServerSend.Welcome(ID, "Welcome to the server!");
            }

            public void SendData(Packet _packet)
            {
                try
                {
                    //make sure socket has value
                    if (socket != null)
                    {
                        stream.BeginWrite(_packet.ToArray(), 0, _packet.Length(), null, null);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Sending TCP Data Packet to Client {ID} : {ex.Message}");
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
                        // TODO: disconnect
                        return;
                    }

                    // data received
                    byte[] _data = new byte[_byteLength];
                    Array.Copy(receiveBuffer, _data, _byteLength);

                    // Handle data
                    receivedData.Reset(HandleData(_data));

                    // start listening again
                    stream.BeginRead(receiveBuffer, 0, dataBufferSize, ReceiveCallback, null);


                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error Receiving TCP Data: {ex.Message}");
                    //TODO: disconnect client
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
                if (IsStartOfPacket(receivedData))
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
                            Server.packetHandlers[_packetID](ID, _packet);
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
                        if (IsPacketEmpty(_packetLength)) ;
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
        }

        #endregion
    }
}
