using System;
using UnityEngine;
using UnityEngine.Events;

public class ColorPicker : MonoBehaviour
{
    [Serializable]
    public class ColorChangedEvent : UnityEvent<Color> { }

    [Serializable]
    public class HSVChangedEvent : UnityEvent<float, float, float> { }

    private float _hue;
    private float _saturation;
    private float _brightness;

    private float _red = 1;
    private float _green;
    private float _blue;

    private float _alpha = 1;

    public ColorChangedEvent onValueChanged = new ColorChangedEvent();
    public HSVChangedEvent onHSVChanged = new HSVChangedEvent();

    public Color CurrentColor
    {
        get => new Color(_red, _green, _blue, _alpha);
        set
        {
            if(CurrentColor == value)
                return;
            _red = value.r;
            _green = value.g;
            _blue = value.b;
            _alpha = value.a;
            RGBChanged();
            SendChangedEvent();
        }
    }

    private void Start()
    {
        RGBChanged();
        SendChangedEvent();
    }

    public float H
    {
        get => _hue;
        set
        {
            if(_hue.Equals(value))
                return;
            _hue = value;
            HSVChanged();
            SendChangedEvent();
        }
    }

    public float S
    {
        get => _saturation;
        set
        {
            if(_saturation.Equals(value))
                return;
            _saturation = value;
            HSVChanged();
            SendChangedEvent();
        }
    }

    public float V
    {
        get => _brightness;
        set
        {
            if(_brightness.Equals(value))
                return;
            _brightness = value;
            HSVChanged();
            SendChangedEvent();
        }
    }

    public float R
    {
        get => _red;
        set
        {
            if(_red.Equals(value))
                return;
            _red = value;
            RGBChanged();
            SendChangedEvent();
        }
    }

    public float G
    {
        get => _green;
        set
        {
            if(_green.Equals(value))
                return;
            _green = value;
            RGBChanged();
            SendChangedEvent();
        }
    }

    public float B
    {
        get => _blue;
        set
        {
            if(_blue.Equals(value))
                return;
            _blue = value;
            RGBChanged();
            SendChangedEvent();
        }
    }

    private float A
    {
        get => _alpha;
        set
        {
            if(_alpha.Equals(value))
                return;
            _alpha = value;
            SendChangedEvent();
        }
    }

    private void RGBChanged()
    {
        var color = HSVUtil.ConvertRgbToHsv(CurrentColor);
        _hue = color.normalizedH;
        _saturation = color.normalizedS;
        _brightness = color.normalizedV;
    }

    private void HSVChanged()
    {
        var color = HSVUtil.ConvertHsvToRgb(_hue * 360, _saturation, _brightness, _alpha);
        _red = color.r;
        _green = color.g;
        _blue = color.b;
    }

    private void SendChangedEvent()
    {
        onValueChanged.Invoke(CurrentColor);
        onHSVChanged.Invoke(_hue, _saturation, _brightness);
    }

    public void AssignColor(ColorValues type, float value)
    {
        // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
        switch(type)
        {
            case ColorValues.R:
                R = value;
                break;
            case ColorValues.G:
                G = value;
                break;
            case ColorValues.B:
                B = value;
                break;
            case ColorValues.A:
                A = value;
                break;
            case ColorValues.Hue:
                H = value;
                break;
            case ColorValues.Saturation:
                S = value;
                break;
            case ColorValues.Value:
                V = value;
                break;
        }
    }

    public float GetValue(ColorValues type)
    {
        return type switch
        {
            ColorValues.R => R,
            ColorValues.G => G,
            ColorValues.B => B,
            ColorValues.A => A,
            ColorValues.Hue => H,
            ColorValues.Saturation => S,
            ColorValues.Value => V,
            _ => throw new NotImplementedException("")
        };
    }
}
