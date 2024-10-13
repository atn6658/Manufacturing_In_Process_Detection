using UnityEngine;
using UnityEngine.UI;

public class SliderTextHelper : MonoBehaviour
{
    public Slider TargetSlider;
    public TMPro.TMP_InputField TargetInputField;

    void Start()
    {
        SetTextToSliderValue();
    }

    void Update()
    {
        
    }

    public void SetTextToSliderValue()
    {
        TargetInputField.text = TargetSlider.value.ToString();
    }

    public void SetSliderToTextValue()
    {
        try
        {
            // clamp value to -360 to 360
            if (float.Parse(TargetInputField.text) < -360) TargetInputField.text = "-360";
            if (float.Parse(TargetInputField.text) > 360) TargetInputField.text = "360";
            TargetSlider.value = (float)System.Math.Round(float.Parse(TargetInputField.text), 3);
        }
        // silently fail
        catch (System.Exception) { }
    }

    public float GetSliderValue() => TargetSlider.value;
}
