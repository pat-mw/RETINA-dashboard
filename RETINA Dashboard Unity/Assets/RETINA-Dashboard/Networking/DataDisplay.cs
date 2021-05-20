using UnityEngine;
using UnityEngine.UI;
using RetinaNetworking;

[RequireComponent(typeof(Text))]
public class DataDisplay : MonoBehaviour
{
    private Text text;
    public ReceivedDataEvent receivedDataEvent = new ReceivedDataEvent();

    private void Awake()
    {
        text = GetComponent<Text>();
        receivedDataEvent.AddListener(DisplayData);
    }

    void DisplayData(string dataString)
    {
        Debug.Log(" -- DISPLAYING DATA -- ");
        text.text = dataString;
    }
}
