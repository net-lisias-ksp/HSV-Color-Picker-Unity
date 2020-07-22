using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[RequireComponent(typeof(InputField))]
public class HexColorField : MonoBehaviour
{
    [FormerlySerializedAs("hsvpicker")] public ColorPicker hsvPicker;

    public bool displayAlpha;

    private InputField hexInputField;

    private void Awake()
    {
        hexInputField = GetComponent<InputField>();
        // Add listeners to keep text (and color) up to date
        hexInputField.onEndEdit.AddListener(UpdateColor);
        hsvPicker.onValueChanged.AddListener(UpdateHex);
    }

    private void OnDestroy()
    {
        hexInputField.onValueChanged.RemoveListener(UpdateColor);
        hsvPicker.onValueChanged.RemoveListener(UpdateHex);
    }

    private void UpdateHex(Color newColor)
    {
        hexInputField.text = ColorToHex(newColor);
    }

    private void UpdateColor(string newHex)
    {
        if(!newHex.StartsWith("#"))
            newHex = $"#{newHex}";
        if(ColorUtility.TryParseHtmlString(newHex, out var color))
            hsvPicker.CurrentColor = color;
        else
            Debug.Log(
                "hex value is in the wrong format, valid formats are: #RGB, #RGBA, #RRGGBB and #RRGGBBAA (# is optional)");
    }

    private string ColorToHex(Color32 color)
    {
        return displayAlpha
            ? $"#{color.r:X2}{color.g:X2}{color.b:X2}{color.a:X2}"
            : $"#{color.r:X2}{color.g:X2}{color.b:X2}";
    }
}
