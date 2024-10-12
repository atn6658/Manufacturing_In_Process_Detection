using Dummiesman;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class OBJImporter : MonoBehaviour
{
    public TMPro.TMP_InputField FilePathInput;
    public List<GameObject> ImportedObjects;

    int ObjectCounter = 1;

    void Start()
    {
        ImportedObjects = new List<GameObject>();
    }

    void Update()
    {
    }

    public void LoadModel()
    {
        string filePath = FilePathInput.text;
        // trim leading and ending quotes and spaces
        filePath = filePath.Trim('"').Trim();
        // attempt to load the model
        // silently aborting if error at any point
        try
        {
            GameObject loadedObject = null;
            if (filePath.StartsWith("http") || filePath.StartsWith("https"))
                loadedObject = new OBJLoader().Load(GetStreamFromURL(filePath));
            // else if (File.Exists(filePath))
            //     loadedObject = new OBJLoader().Load(filePath);
            else
                throw new System.Exception("File not found.");
            // set the object's name
            loadedObject.name = "ImportedObject" + ObjectCounter++;
            // set the object's parent
            loadedObject.transform.SetParent(transform);
            // add the object to the list
            ImportedObjects.Add(loadedObject);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error loading model: " + e.Message);
            return;
        }
        // clear the input field
        FilePathInput.text = string.Empty;
    }

    MemoryStream GetStreamFromURL(string url)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        var www = new WWW(url);
#pragma warning restore CS0618 // Type or member is obsolete
        while (!www.isDone)
            System.Threading.Thread.Sleep(1);
        return new MemoryStream(Encoding.UTF8.GetBytes(www.text));
    }
}
