using UnityEngine;
using RetinaNetworking;
using System.IO;
using System;

public class SerializationTesterSO : MonoBehaviour
{

    public DataSO dataToTest;
    public string saveFolder = "SavedData";
    public string fileName = "Data";

    // Start is called before the first frame update
    void Start()
    {
        string serialized = JSONHandler.EncodeString(dataToTest);

        DataSO deserialized = JSONHandler.DecodeStringSO(serialized);

        Debug.Log("---------");

        Debug.Log("Original:" + dataToTest);

        Debug.Log("Serialized:" + serialized);

        Debug.Log("Deserialized: " + deserialized);

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

                Debug.Log($"path {path} does not exist yet, creating directory");
                Directory.CreateDirectory(path);
            }

            path = Path.Combine(path, NormaliseFilename(fileName));
            path = NormalisePath(path);

            File.WriteAllText(path, serialized);
            Debug.Log($"saved JSON to: {path}");
        }
        catch (Exception ex)
        {
            Debug.Log("Error writing file" + ex);
        }

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
