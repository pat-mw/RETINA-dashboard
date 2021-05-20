using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;


namespace RetinaNetworking
{
    public class UDPReceive : MonoBehaviour
    {
        public string ip;
        public IPAddress hostaddress;
        public int ReceivePort;

        private IPEndPoint ipep;
        private IPEndPoint sender;
        private UdpClient client;
        private Thread networkThread;

        private byte[] receivedData;
        private string dataString;

        public ReceivedDataEvent receivedDataEvent = new ReceivedDataEvent();


        void Start()
        {
            networkThread = new Thread(ReceiveData);
            networkThread.Start();

            receivedData = new byte[0];

            ipep = new IPEndPoint(IPAddress.Any, ReceivePort);
            client = new UdpClient(ipep);
            sender = new IPEndPoint(IPAddress.Any, 0);
        }

        void Update()
        {
            ReceiveData();
        }

        public void ReceiveData()
        {
            if (client.Available > 0)
            {
                receivedData = client.Receive(ref sender);
                dataString = Encoding.ASCII.GetString(receivedData);

                Debug.Log(" -- receiving data -- ");
                Debug.Log(" -- data type: " + receivedData.GetType().ToString());
                Debug.Log(" -- data value: " + dataString);
                Debug.Log(" ---------------------");

                receivedDataEvent.Invoke(dataString);
            }
            else
            {
                Debug.Log("No data to receive");
            }
        }
    }


    public class ReceivedDataEvent : UnityEvent<string>
    {
    }
}