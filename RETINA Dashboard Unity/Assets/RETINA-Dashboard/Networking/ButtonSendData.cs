using UnityEngine;
using UnityEngine.UI;
using RetinaNetworking;
using System.Runtime.InteropServices;

[RequireComponent(typeof(Button))]
public class ButtonSendData : MonoBehaviour
{
    public InputField dataField;
    public UDPSend Sender;

    private Button button;

    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
    private string dataString;

    private void Awake()
    {
        button = GetComponent<Button>();
        dataString = dataField.text;
        button.onClick.AddListener(delegate { SendData(dataString); });
    }

    void SendData(object data)
    {
        Debug.Log(" -- attempting to send data -- ");
        Debug.Log(" -- data type: " + data.GetType().ToString());
        Debug.Log(" -- data value: " + data);
        Debug.Log(" -----------------------------");

        Sender.SendData(data);
    }
}




