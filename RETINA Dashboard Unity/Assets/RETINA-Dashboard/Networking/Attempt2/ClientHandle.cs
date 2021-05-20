using UnityEngine;

namespace RetinaNetworking
{
    public class ClientHandle : MonoBehaviour
    {
        public static void Welcome(Packet _packet)
        {
            string _msg = _packet.ReadString();
            int _myID = _packet.ReadInt();

            Debug.Log($"Message from the server: {_msg}");
            Client.Instance.MyID = _myID;
            ClientSend.WelcomeReceived();
        }
    }
}