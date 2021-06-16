using UnityEngine;
using RetinaNetworking;
using System.IO;
using System;
using RetinaNetworking.Client;

public class NestedSerializationTester : MonoBehaviour
{
    private NestedData nestedDataToTest;
    public string saveFolder = "SavedData";
    public string fileName = "NestedData";
    public bool usingSirenixSerialiser = false;
    public bool autoStart = false;

    void Start()
    {
        if (autoStart)
        {
            SendTestData();
        }
    }

    // Start is called before the first frame update
    public void SendTestData()
    {
        nestedDataToTest = GenerateNestedData();

        byte[] serializedByte = null;
        string serializedString = null;

        if (usingSirenixSerialiser)
        {
            serializedByte = JSONHandler.EncodeByteArray(nestedDataToTest);
            NestedData deserialized = JSONHandler.DecodeNestedByteArray(serializedByte);

            Debug.Log("Original:" + nestedDataToTest.GetType());

            Debug.Log("Serialized (byte array):" + serializedByte);

            Debug.Log("Deserialized: " + deserialized);
        }
        else
        {
            serializedString = JSONHandler.EncodeString(nestedDataToTest);
            NestedData deserialized = JSONHandler.DecodeNestedString(serializedString);

            Debug.Log("Original:" + nestedDataToTest.GetType());

            Debug.Log("Serialized (string):" + serializedString);

            Debug.Log("Deserialized: " + deserialized);
        }


        // sending data on network
        try
        {
            if (usingSirenixSerialiser)
            {
                ClientSend.SendExampleData(serializedByte);
                Debug.Log($"Sent Example data (byte array): {serializedByte}");
            }
            else
            {
                ClientSend.SendExampleData(serializedString);
                Debug.Log($"Sent Example data (string): {serializedString}");
            }
        }
        catch(Exception ex)
        {
            Debug.Log("Error sending data: " + ex);
        }
        
        // saving data locally
        try
        {
            // use persistent path on builds, use normal data path in Editor for easier testing
            #if UNITY_EDITOR
            string path = Path.Combine(Application.dataPath, saveFolder);
            #else
            string path = Path.Combine(Application.persistentDataPath, saveFolder);
            #endif


            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            path = Path.Combine(path, NormaliseFilename(fileName));
            path = NormalisePath(path);

            
            if (usingSirenixSerialiser)
            {
                File.WriteAllBytes(path, serializedByte);
            }
            else
            {
                File.WriteAllText(path, serializedString);
            }

            Debug.Log($"saved JSON to: {path}");
        }
        catch (Exception ex)
        {
            Debug.Log("Error writing file" + ex);
        }

    }

    private NestedData GenerateNestedData()
    {
        Data exampleData = GenerateData();
        NestedData result = new NestedData(1, 2, exampleData);
        return result;
    }

    private Data GenerateData()
    {
        Data result = new Data(1, 2, "three", enumSample.bob);
        return result;
    }

    private string NormalisePath(string path)
    {
        var normalised = path.Replace(@"\", "/");
        return normalised;
    }

    private string NormaliseFilename(string _fileName)
    {
        var _fileExtension = ".txt";
        var _DateAndTime = DateTime.Now.ToString("yyyy-dd-M--HH-mm-ss");
        return $"{_fileName}_{_DateAndTime}{_fileExtension}";
    }
}
