using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class ColorLabel : MonoBehaviour
{
    public ColorPicker picker;

    public ColorValues type;

    public string prefix = "R: ";
    public float minValue;
    public float maxValue = 255;

    public int precision;

    private Text label;

    private void Awake()
    {
        label = GetComponent<Text>();
    }

    private void OnEnable()
    {
        if(!Application.isPlaying || picker == null)
            return;
        picker.onValueChanged.AddListener(ColorChanged);
        picker.onHSVChanged.AddListener(HSVChanged);
    }

    private void OnDestroy()
    {
        if(picker == null)
            return;
        picker.onValueChanged.RemoveListener(ColorChanged);
        picker.onHSVChanged.RemoveListener(HSVChanged);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        label = GetComponent<Text>();
        UpdateValue();
    }
#endif

    private void ColorChanged(Color color)
    {
        UpdateValue();
    }

    private void HSVChanged(float hue, float saturation, float value)
    {
        UpdateValue();
    }

    private void UpdateValue()
    {
        if(picker == null)
            label.text = $"{prefix}-";
        else
        {
            var value = minValue + picker.GetValue(type) * (maxValue - minValue);
            label.text = $"{prefix}{ConvertToDisplayString(value)}";
        }
    }

    private string ConvertToDisplayString(float value)
    {
        return precision > 0 ? value.ToString($"F{precision}") : value.ToString("F0");
    }
}
