using LargeNumbers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;
using System;
using TMPro;

public class SliderHandler : MonoBehaviour, IPointerUpHandler
{
    /***
    *   Move slider to get whole numbers
    *   Function that returns the amount the slider has stopped at
    *   event that triggers when slider stops dragging
    *   boolean to chose for whole numbers or floats
    *   serilizefield for input values? how do i chose what max value is
    *   serilizefield for input. use currencytype
    ***/

    [SerializeField] public CurrencyTypes maxValueCurrencytype;
    [SerializeField] private bool useCurrencyTypeMaxValue = false;
    [SerializeField] private AlphabeticNotation customMaxValue;
    [SerializeField] private bool useCustomMaxValue;
    [SerializeField] private bool useMaxValueFromOtherScript;
    [SerializeField] private bool resetValuesOnStart = true;
    [SerializeField] private bool showOutputAsPercent = false;
    [SerializeField] private bool useColorsOnFill = false;
    [SerializeField] private Image fillImage;
    private Color originalColorFill;
    public AlphabeticNotation maxValueFromScript;
    private AlphabeticNotation midValue;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text sliderAmount_txt;
    [SerializeField] private bool useWholeNumbers;
    public AlphabeticNotation sliderValue;
    public event Action OnStoppedSliderDrag;
    public event Action OnSliderDragging;
    private AlphabeticNotation maxValue;

    public void SetMaxValueFromScript(AlphabeticNotation amount) => maxValueFromScript = amount;
    private void Start()
    {
        if (fillImage != null)
        {
            originalColorFill = fillImage.color;
        }
        if (resetValuesOnStart)
        {
            ResetSliderValues();
            sliderAmount_txt.text = "0";
        }
    }

    private void OnEnable()
    {
        MoneyManager.Instance.OnCurrencyChanged += UpdateMaxValue;
    }
    private void OnDisable()
    {
        MoneyManager.Instance.OnCurrencyChanged -= UpdateMaxValue;

    }

    private void UpdateMaxValue(CurrencyTypes types)
    {
        if (maxValueCurrencytype != types) return;
        if (useCurrencyTypeMaxValue)
        {
            maxValue = MoneyManager.Instance.GetCurrency(types);
        }
        else if (useCustomMaxValue)
        {
            MoneyManager.Instance.OnCurrencyChanged -= UpdateMaxValue;
            maxValue = customMaxValue;
        }
        else if (useMaxValueFromOtherScript)
        {
            MoneyManager.Instance.OnCurrencyChanged -= UpdateMaxValue;
            maxValue = maxValueFromScript;
        }
    }

    public void ResetSliderValues()
    {
        if (useCurrencyTypeMaxValue)
        {
            maxValue = MoneyManager.Instance.GetCurrency(maxValueCurrencytype);
        }
        else if (useCustomMaxValue)
        {
            maxValue = customMaxValue;
        }
        else if (useMaxValueFromOtherScript)
        {
            maxValue = maxValueFromScript;
        }
        slider.minValue = 0;
        slider.maxValue = 1f;
        slider.value = 0f;
    }

    private AlphabeticNotation GetValue()
    {
        return maxValue * slider.value;
    }

    public void SetSliderSpecific(AlphabeticNotation _midValue, float sliderStart)
    {
        slider.value = sliderStart;
        midValue = _midValue;
        if (showOutputAsPercent)
        {
            double percent = (GetValue().Standard() / _midValue.Standard() * 100);
            sliderAmount_txt.text = sliderAmount_txt.text = $"{percent:F0}%";
        }
        else
        {
            sliderAmount_txt.text = _midValue.ToStringSmart(1);
        }
    }

    public void OnSliderDragger()
    {
        if (useWholeNumbers)
        {
            sliderValue = GetValue().Round();
        }
        else
        {
            sliderValue = GetValue();
        }
        if (showOutputAsPercent)
        {
            double percent = (GetValue().Standard() / midValue.Standard() * 100);
            sliderAmount_txt.text = sliderAmount_txt.text = $"{percent:F0}%";
        }
        else
        {
            sliderAmount_txt.text = sliderValue.ToStringSmart(1);
        }
        OnSliderDragging?.Invoke();

        if (useColorsOnFill && fillImage != null)
        {
            fillImage.color = GetFillColor(slider.value);
        }
    }

    private Color GetFillColor(float value)
    {
        // Clamp to 0–1 just in case
        value = Mathf.Clamp01(value);

        // Define the 3 key colors
        Color yellow = new Color(1f, 1f, 0f); // 0%
        Color green = new Color(0f, 1f, 0f); // 50%
        Color red = new Color(1f, 0f, 0f); // 100%

        if (value < 0.5f)
        {
            // Lerp between yellow (0) → green (0.5)
            float t = value / 0.5f; // scale 0–0.5 → 0–1
            return Color.Lerp(yellow, green, t);
        }
        else
        {
            // Lerp between green (0.5) → red (1)
            float t = (value - 0.5f) / 0.5f; // scale 0.5–1 → 0–1
            return Color.Lerp(green, red, t);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnStoppedSliderDrag?.Invoke();
    }
}
