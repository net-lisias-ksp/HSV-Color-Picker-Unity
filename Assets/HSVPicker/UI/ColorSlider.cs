using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

/// <summary>
/// Displays one of the color values of aColorPicker
/// </summary>
[RequireComponent(typeof(Slider))]
public class ColorSlider : MonoBehaviour
{
    [FormerlySerializedAs("hsvpicker")] public ColorPicker hsvPicker;

    /// <summary>
    /// Which value this slider can edit.
    /// </summary>
    public ColorValues type;

    private Slider slider;

    private bool listen = true;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        hsvPicker.onValueChanged.AddListener(ColorChanged);
        hsvPicker.onHSVChanged.AddListener(HSVChanged);
        slider.onValueChanged.AddListener(SliderChanged);
    }

    private void OnDestroy()
    {
        hsvPicker.onValueChanged.RemoveListener(ColorChanged);
        hsvPicker.onHSVChanged.RemoveListener(HSVChanged);
        slider.onValueChanged.RemoveListener(SliderChanged);
    }

    private void ColorChanged(Color newColor)
    {
        listen = false;
        slider.normalizedValue = type switch
        {
            ColorValues.R => newColor.r,
            ColorValues.G => newColor.g,
            ColorValues.B => newColor.b,
            ColorValues.A => newColor.a,
            _ => slider.normalizedValue
        };
    }

    private void HSVChanged(float hue, float saturation, float value)
    {
        listen = false;
        slider.normalizedValue = type switch
        {
            ColorValues.Hue => hue, //1 - hue;
            ColorValues.Saturation => saturation,
            ColorValues.Value => value,
            _ => slider.normalizedValue
        };
    }

    private void SliderChanged(float newValue)
    {
        if(listen)
            hsvPicker.AssignColor(type, slider.normalizedValue);
        listen = true;
    }
}
