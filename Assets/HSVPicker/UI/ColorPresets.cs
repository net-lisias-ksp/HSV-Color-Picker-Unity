using UnityEngine;
using UnityEngine.UI;

public class ColorPresets : MonoBehaviour
{
    public ColorPicker picker;
    public GameObject[] presets;
    public Image createPresetImage;

    private void Awake()
    {
        picker.onValueChanged.AddListener(ColorChanged);
    }

    public void CreatePresetButton()
    {
        foreach(var preset in presets)
        {
            if(preset.activeSelf)
                continue;
            preset.SetActive(true);
            preset.GetComponent<Image>().color = picker.CurrentColor;
            break;
        }
    }

    public void PresetSelect(Image sender)
    {
        picker.CurrentColor = sender.color;
    }

    // Not working, it seems ConvertHsvToRgb() is broken. It doesn't work when fed
    // input h, s, v as shown below.
//	private void HSVChanged(float h, float s, float v)
//	{
//		createPresetImage.color = HSVUtil.ConvertHsvToRgb(h, s, v, 1);
//	}
    private void ColorChanged(Color color)
    {
        createPresetImage.color = color;
    }
}
