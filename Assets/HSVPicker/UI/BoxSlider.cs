using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("UI/BoxSlider", 35)]
[RequireComponent(typeof(RectTransform))]
public class BoxSlider : Selectable, IDragHandler, IInitializePotentialDragHandler
{
    [Serializable]
    public class BoxSliderEvent : UnityEvent<float, float> { }

    [SerializeField] private RectTransform m_HandleRect;

    public RectTransform handleRect
    {
        get => m_HandleRect;
        set
        {
            if(!SetClass(ref m_HandleRect, value))
                return;
            UpdateCachedReferences();
            UpdateVisuals();
        }
    }

    [Space(6)] [SerializeField] private float m_MinValue;

    public float minValue
    {
        get => m_MinValue;
        set
        {
            if(!SetStruct(ref m_MinValue, value))
                return;
            Set(m_Value);
            SetY(m_ValueY);
            UpdateVisuals();
        }
    }

    [SerializeField] private float m_MaxValue = 1;

    public float maxValue
    {
        get => m_MaxValue;
        set
        {
            if(!SetStruct(ref m_MaxValue, value))
                return;
            Set(m_Value);
            SetY(m_ValueY);
            UpdateVisuals();
        }
    }

    [SerializeField] private bool m_WholeNumbers;

    public bool wholeNumbers
    {
        get => m_WholeNumbers;
        set
        {
            if(!SetStruct(ref m_WholeNumbers, value))
                return;
            Set(m_Value);
            SetY(m_ValueY);
            UpdateVisuals();
        }
    }

    [SerializeField] private float m_Value = 1f;

    public float value
    {
        get =>
            wholeNumbers
                ? Mathf.Round(m_Value)
                : m_Value;
        set => Set(value);
    }

    public float normalizedValue
    {
        get =>
            Mathf.Approximately(minValue, maxValue)
                ? 0
                : Mathf.InverseLerp(minValue, maxValue, value);
        set => this.value = Mathf.Lerp(minValue, maxValue, value);
    }

    [SerializeField] private float m_ValueY = 1f;

    public float valueY
    {
        get =>
            wholeNumbers
                ? Mathf.Round(m_ValueY)
                : m_ValueY;
        set => SetY(value);
    }

    public float normalizedValueY
    {
        get =>
            Mathf.Approximately(minValue, maxValue)
                ? 0
                : Mathf.InverseLerp(minValue, maxValue, valueY);
        set => valueY = Mathf.Lerp(minValue, maxValue, value);
    }

    // Allow for delegate-based subscriptions for faster events than 'eventReceiver', and allowing for multiple receivers.
    [SerializeField] [Space(6)] public BoxSliderEvent onValueChanged = new BoxSliderEvent();

    private Transform m_HandleTransform;
    private RectTransform m_HandleContainerRect;

    // The offset from handle position to mouse down position
    private Vector2 m_Offset = Vector2.zero;

    private DrivenRectTransformTracker m_Tracker = default;

    private static bool SetClass<T>(ref T currentValue, T newValue) where T : class
    {
        if(currentValue == null && newValue == null
           || currentValue != null && currentValue.Equals(newValue))
            return false;
        currentValue = newValue;
        return true;
    }

    private static bool SetStruct<T>(ref T currentValue, T newValue) where T : struct
    {
        if(currentValue.Equals(newValue))
            return false;
        currentValue = newValue;
        return true;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        UpdateCachedReferences();
        Set(m_Value, false);
        SetY(m_ValueY, false);
        // Update rects since they need to be initialized correctly.
        UpdateVisuals();
    }

    protected override void OnDisable()
    {
        m_Tracker.Clear();
        base.OnDisable();
    }

    private void UpdateCachedReferences()
    {
        if(m_HandleRect)
        {
            m_HandleTransform = m_HandleRect.transform;
            if(m_HandleTransform.parent != null)
                m_HandleContainerRect = m_HandleTransform.parent.GetComponent<RectTransform>();
        }
        else
        {
            m_HandleContainerRect = null;
        }
    }

    // Set the valueUpdate the visible Image.

    private void Set(float input, bool sendCallback = true)
    {
        // Clamp the input
        var newValue = Mathf.Clamp(input, minValue, maxValue);
        if(wholeNumbers)
            newValue = Mathf.Round(newValue);
        // If the stepped value doesn't match the last one, it's time to update
        if(m_Value.Equals(newValue))
            return;
        m_Value = newValue;
        UpdateVisuals();
        if(sendCallback)
            onValueChanged.Invoke(newValue, valueY);
    }

    private void SetY(float input, bool sendCallback = true)
    {
        // Clamp the input
        var newValue = Mathf.Clamp(input, minValue, maxValue);
        if(wholeNumbers)
            newValue = Mathf.Round(newValue);
        // If the stepped value doesn't match the last one, it's time to update
        if(m_ValueY.Equals(newValue))
            return;
        m_ValueY = newValue;
        UpdateVisuals();
        if(sendCallback)
            onValueChanged.Invoke(value, newValue);
    }


    protected override void OnRectTransformDimensionsChange()
    {
        base.OnRectTransformDimensionsChange();
        UpdateVisuals();
    }

    // Force-update the slider. Useful if you've changed the properties and want it to update visually.
    private void UpdateVisuals()
    {
        m_Tracker.Clear();
        if(m_HandleContainerRect == null)
            return;
        m_Tracker.Add(this, m_HandleRect, DrivenTransformProperties.Anchors);
        var anchorMin = Vector2.zero;
        var anchorMax = Vector2.one;
        anchorMin[0] = anchorMax[0] = normalizedValue;
        anchorMin[1] = anchorMax[1] = normalizedValueY;
        m_HandleRect.anchorMin = anchorMin;
        m_HandleRect.anchorMax = anchorMax;
    }

    // Update the slider's position based on the mouse.
    private void UpdateDrag(PointerEventData eventData, Camera cam)
    {
        var clickRect = m_HandleContainerRect;
        if(clickRect == null || !(clickRect.rect.size[0] > 0))
            return;
        if(!RectTransformUtility.ScreenPointToLocalPointInRectangle(clickRect,
            eventData.position,
            cam,
            out var localCursor))
            return;
        var rect = clickRect.rect;
        localCursor -= rect.position;
        normalizedValue = Mathf.Clamp01((localCursor - m_Offset)[0] / rect.size[0]);
        normalizedValueY = Mathf.Clamp01((localCursor - m_Offset)[1] / rect.size[1]);
    }

    private bool MayDrag(PointerEventData eventData) =>
        eventData.button == PointerEventData.InputButton.Left
        && IsActive()
        && IsInteractable();

    public override void OnPointerDown(PointerEventData eventData)
    {
        if(!MayDrag(eventData))
            return;
        base.OnPointerDown(eventData);
        m_Offset = Vector2.zero;
        if(m_HandleContainerRect != null
           && RectTransformUtility.RectangleContainsScreenPoint(m_HandleRect,
               eventData.position,
               eventData.enterEventCamera))
        {
            if(RectTransformUtility.ScreenPointToLocalPointInRectangle(m_HandleRect,
                eventData.position,
                eventData.pressEventCamera,
                out var localMousePos))
                m_Offset = localMousePos;
            m_Offset.y = -m_Offset.y;
        }
        else // Outside the slider handle - jump to this point instead
            UpdateDrag(eventData, eventData.pressEventCamera);
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if(!MayDrag(eventData))
            return;
        UpdateDrag(eventData, eventData.pressEventCamera);
    }

    public virtual void OnInitializePotentialDrag(PointerEventData eventData)
    {
        eventData.useDragThreshold = false;
    }
}
