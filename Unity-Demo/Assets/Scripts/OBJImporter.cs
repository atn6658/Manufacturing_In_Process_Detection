using Dummiesman;
using System;
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
    public GameObject SelectedObject;

    int ObjectCounter = 1;
    String fileContent = string.Empty;

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
            {
                StartCoroutine(GetStreamFromURL(filePath));
                loadedObject = new OBJLoader().Load(new MemoryStream(Encoding.UTF8.GetBytes(fileContent)));
            }
            else
            {
                throw new Exception("Only HTTP/HTTPS URLs are supported for now.");
            }
            // {
            //     string completePath = Application.streamingAssetsPath + "/" + filePath;
            //     Debug.Log("Loading file from: " + completePath);
            //     StartCoroutine(GetStreamFromURL(completePath));
            //     loadedObject = new OBJLoader().Load(new MemoryStream(Encoding.UTF8.GetBytes(fileContent)));
            // }
            // set the object's name
            loadedObject.name = "ImportedObject" + ObjectCounter++;
            // scale down object to mm
            loadedObject.transform.localScale *= 0.001f;
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

    IEnumerator GetStreamFromURL(string url)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(url))
        {
            yield return www.SendWebRequest();
            switch (www.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("Error loading model: " + www.error);
                    break;
                case UnityWebRequest.Result.Success:
                    fileContent = www.downloadHandler.text;
                    break;
            }
        }
    }
}
