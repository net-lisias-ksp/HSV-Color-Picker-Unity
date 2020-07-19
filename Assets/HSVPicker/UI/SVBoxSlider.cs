using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(BoxSlider), typeof(RawImage)), ExecuteInEditMode]
public class SVBoxSlider : MonoBehaviour
{
    public ColorPicker picker;

    private BoxSlider slider;
    private RawImage image;

    public ComputeShader compute;
    private int kernelID;
    private RenderTexture renderTexture;
    private const int textureWidth = 100;
    private const int textureHeight = 100;

    private float lastH = -1;
    private bool listen = true;

    private void Awake()
    {
        slider = GetComponent<BoxSlider>();
        image = GetComponent<RawImage>();
        if(!Application.isPlaying)
            return;
        if(SystemInfo.supportsComputeShaders)
            InitializeCompute();
        RegenerateSVTexture();
    }

    private void InitializeCompute()
    {
        if(compute == null)
            return;
        try
        {
            kernelID = compute.FindKernel("CSMain");
            if(renderTexture == null)
            {
                // ReSharper disable once UseObjectOrCollectionInitializer
                renderTexture =
                    new RenderTexture(textureWidth, textureHeight, 0, RenderTextureFormat.RGB111110Float);
                renderTexture.enableRandomWrite = true;
                renderTexture.Create();
            }
            image.texture = renderTexture;
        }
        catch(Exception e)
        {
            Debug.Log($"Cannot find the kernel in SV shader: {e.Message}");
            compute = null;
        }
    }

    private void OnEnable()
    {
        if(!Application.isPlaying || picker == null)
            return;
        slider.onValueChanged.AddListener(SliderChanged);
        picker.onHSVChanged.AddListener(HSVChanged);
    }

    private void OnDisable()
    {
        if(picker == null)
            return;
        slider.onValueChanged.RemoveListener(SliderChanged);
        picker.onHSVChanged.RemoveListener(HSVChanged);
    }

    private void OnDestroy()
    {
        if(renderTexture != null)
            renderTexture.Release();
        // ReSharper disable once InvertIf
        if(image.texture != null)
        {
            Destroy(image.texture);
            image.texture = null;
        }
    }

    private void SliderChanged(float saturation, float value)
    {
        if(listen)
        {
            picker.AssignColor(ColorValues.Saturation, saturation);
            picker.AssignColor(ColorValues.Value, value);
        }
        listen = true;
    }

    private void HSVChanged(float h, float s, float v)
    {
        if(!lastH.Equals(h))
        {
            lastH = h;
            RegenerateSVTexture();
        }
        if(!s.Equals(slider.normalizedValue))
        {
            listen = false;
            slider.normalizedValue = s;
        }
        // ReSharper disable once InvertIf
        if(!v.Equals(slider.normalizedValueY))
        {
            listen = false;
            slider.normalizedValueY = v;
        }
    }

    private void RegenerateSVTexture()
    {
        if(SystemInfo.supportsComputeShaders && compute != null)
        {
            var hue = picker != null ? picker.H : 0;
            compute.SetTexture(kernelID, "Texture", renderTexture);
            compute.SetFloats("TextureSize", textureWidth, textureHeight);
            compute.SetFloat("Hue", hue);
            var threadGroupsX = Mathf.CeilToInt(textureWidth / 32f);
            var threadGroupsY = Mathf.CeilToInt(textureHeight / 32f);
            compute.Dispatch(kernelID, threadGroupsX, threadGroupsY, 1);
        }
        else
        {
            double h = picker != null ? picker.H * 360 : 0;
            if(image.texture != null)
                Destroy(image.texture);
            var texture = new Texture2D(textureWidth, textureHeight) { hideFlags = HideFlags.DontSave };
            for(var s = 0; s < textureWidth; s++)
            {
                var colors = new Color32[textureHeight];
                for(var v = 0; v < textureHeight; v++)
                    colors[v] = HSVUtil.ConvertHsvToRgb(h, (float)s / 100, (float)v / 100, 1);
                texture.SetPixels32(s, 0, 1, textureHeight, colors);
            }
            texture.Apply();
            image.texture = texture;
        }
    }
}
