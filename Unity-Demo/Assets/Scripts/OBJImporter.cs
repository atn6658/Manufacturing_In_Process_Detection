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
    public TMPro.TMP_Dropdown PartsDropdown;
    public List<GameObject> ImportedObjects;
    public GameObject SelectedObject;
    public Material HighlighMaterial;
    public Material DefaultMaterial;

    int ObjectCounter = 1;

    void Start()
    {
        ImportedObjects = new List<GameObject>();
        PartsDropdown.ClearOptions();
    }

    void Update()
    {
    }

//     IEnumerator LoadModel()
//     {
//         string completePath = Application.streamingAssetsPath + "/" + FilePathInput.text;
//         Debug.Log("Loading file from: " + completePath);
// #pragma warning disable CS0618 // Type or member is obsolete
//         using (var www = new WWW(completePath))
//         {
//             yield return www;
//             var stream = new MemoryStream(Encoding.UTF8.GetBytes(www.text));
//             var loadedObject = new OBJLoader().Load(stream);
//         }
// #pragma warning restore CS0618 // Type or member is obsolete
//     }

    public void InvokeLoadModel() => StartCoroutine(LoadModel((Application.streamingAssetsPath + "/" + FilePathInput.text).Trim('"').Trim()));

    IEnumerator LoadModel(string URL)
    {
        Debug.Log("Loading file from: " + URL);
        using (UnityWebRequest www = UnityWebRequest.Get(URL))
        {
            yield return www.SendWebRequest();
            switch (www.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("Error: " + www.error);
                    break;
                case UnityWebRequest.Result.Success:
                    string content = www.downloadHandler.text;
                    var textStream = new MemoryStream(Encoding.UTF8.GetBytes(content));
                    var loadedObject = new OBJLoader().Load(textStream);
                    // set scale to mm
                    loadedObject.transform.localScale *= 0.001f;
                    // rename object
                    loadedObject.name = Path.GetFileNameWithoutExtension(FilePathInput.text) + "_" + ObjectCounter++;
                    // set object parent
                    loadedObject.transform.SetParent(transform);
                    // apply default material
                    foreach (var renderer in loadedObject.GetComponentsInChildren<MeshRenderer>())
                        renderer.material = DefaultMaterial;
                    // add to list
                    ImportedObjects.Add(loadedObject);
                    break;
            }
        }

        // clear the input field
        // FilePathInput.text = string.Empty;

        // update dropdown
        UpdateDropdown();
    }

    void UpdateDropdown()
    {
        PartsDropdown.ClearOptions();
        // skip if no objects are imported
        if (ImportedObjects.Count == 0) return;
        // get object names
        List<string> objectNames = new List<string>();
        foreach (var obj in ImportedObjects)
        {
            objectNames.Add(obj.name);
        }
        // update dropdown
        PartsDropdown.AddOptions(objectNames);
        // update option if it's the first object
        if (ImportedObjects.Count == 1)
            SelectedObject = ImportedObjects[0];
    }

    public void UpdateDropdownTarget()
    {
        // reset color of previous object
        if (SelectedObject != null)
            foreach (var renderer in SelectedObject.GetComponentsInChildren<MeshRenderer>())
                renderer.material = DefaultMaterial;
        SelectedObject = ImportedObjects[PartsDropdown.value];
        // highlight selected object
        foreach (var renderer in SelectedObject.GetComponentsInChildren<MeshRenderer>())
            renderer.material = HighlighMaterial;
    }
}
