using Dummiesman;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class OBJImporter : MonoBehaviour
{
    public GameObject SelectedObject;

    public SliderTextHelper RollRotation;
    public SliderTextHelper PitchRotation;
    public SliderTextHelper YawRotation;

    public string SelectedFileName = string.Empty;
    // public bool ManualClear = false;

    bool KeepCoroutineAlive = true;
    string currentCommand = string.Empty;
    float pollInterval = 0.02f;
    bool modelLoaded = false;

    void Start()
    {
        StartCoroutine(PollCommand());
    }

    void Update()
    {
        if ((currentCommand == "CLEAR") && modelLoaded)
        {
            modelLoaded = false;
            Destroy(SelectedObject);
            SelectedObject = null;
        }
        else if (currentCommand == "LOAD" && modelLoaded == false)
        {
            modelLoaded = true;
            StartCoroutine(LoadModel());
        }
        else { } // do nothing, catch all

        // update the rotation of the object
        // only if the object is loaded
        if (SelectedObject != null)
            SelectedObject.transform.rotation = Quaternion.Euler(RollRotation.GetSliderValue(), PitchRotation.GetSliderValue(), YawRotation.GetSliderValue());
    }

    IEnumerator LoadModel()
    {
        // destroy previous object
        if (SelectedObject != null) Destroy(SelectedObject);

        // reset all the sliders
        RollRotation.TargetSlider.value = 0;
        PitchRotation.TargetSlider.value = 0;
        YawRotation.TargetSlider.value = 0;

        using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:5000/jankfileselection"))
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
                    SelectedFileName = www.downloadHandler.text.Split('\n')[0];
                    SelectedObject = new OBJLoader().Load(new MemoryStream(Encoding.UTF8.GetBytes(www.downloadHandler.text)));
                    SelectedObject.transform.localScale *= 0.001f;
                    break;
            }
        }
    }

    IEnumerator PollCommand()
    {
        while (true && KeepCoroutineAlive)
        {
            using (UnityWebRequest www = UnityWebRequest.Get("http://localhost:5000/jankcommand"))
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
                        currentCommand = www.downloadHandler.text;
                        break;
                }
            }
            yield return new WaitForSeconds(pollInterval);
        }
    }

    void OnApplicationQuit()
    {
        KeepCoroutineAlive = false;
    }
}
