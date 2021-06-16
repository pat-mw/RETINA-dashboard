using UnityEngine;
using RetinaNetworking;
using System.IO;
using System;

public class NestedSerializationTesterSO : MonoBehaviour
{

    public NestedDataSO nestedDataToTest;
    public string saveFolder = "SavedData";
    public string fileName = "NestedData";

    // Start is called before the first frame update
    void Start()
    {
        string serialized = JSONHandler.EncodeString(nestedDataToTest);

        Debug.Log("Original:" + nestedDataToTest.GetType());

        Debug.Log("Serialized:" + serialized);

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
