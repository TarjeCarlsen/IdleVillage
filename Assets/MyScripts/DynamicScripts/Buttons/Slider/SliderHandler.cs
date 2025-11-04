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

    [SerializeField] private int sliderSteps = 100;
    [SerializeField] public CurrencyTypes maxValueCurrencytype;
    [SerializeField] private bool useCurrencyTypeMaxValue = false;
    [SerializeField] private AlphabeticNotation customMaxValue;
    [SerializeField]private bool useCustomMaxValue;
    [SerializeField] private bool useMaxValueFromOtherScript;
    [SerializeField] private bool showOutputAsPercent;
    public AlphabeticNotation maxValueFromScript;
    [SerializeField] private Slider slider;
    [SerializeField] private TMP_Text sliderAmount_txt;
    [SerializeField] private bool useWholeNumbers;
    public AlphabeticNotation sliderValue;
    public event Action OnStoppedSliderDrag;
    public event Action OnSliderDragging;
    private AlphabeticNotation maxValue;

    public void SetMaxValueFromScript(AlphabeticNotation amount) => maxValueFromScript = amount; 
    private void Start(){
        ResetSliderValues();
        sliderAmount_txt.text = "0";
    }

    private void OnEnable(){
        MoneyManager.Instance.OnCurrencyChanged += UpdateMaxValue;
    }
    private void OnDisable(){
        MoneyManager.Instance.OnCurrencyChanged -= UpdateMaxValue;

    }

    private void UpdateMaxValue(CurrencyTypes types){
        if(maxValueCurrencytype != types)return;
        if(useCurrencyTypeMaxValue){
            maxValue = MoneyManager.Instance.GetCurrency(types);
        }else if(useCustomMaxValue){
            MoneyManager.Instance.OnCurrencyChanged -= UpdateMaxValue;
            maxValue = customMaxValue;
        }else if(useMaxValueFromOtherScript){
            MoneyManager.Instance.OnCurrencyChanged -= UpdateMaxValue;
            maxValue = maxValueFromScript;
        }
    }

    public void ResetSliderValues(){
        if(useCurrencyTypeMaxValue){
        maxValue = MoneyManager.Instance.GetCurrency(maxValueCurrencytype);
        }else if(useCustomMaxValue){
            maxValue = customMaxValue;
        }else if(useMaxValueFromOtherScript){
            maxValue = maxValueFromScript;
        }
        slider.minValue = 0;
        slider.maxValue = 1f;
        slider.value = 0;
    }

    private AlphabeticNotation GetValue(){
        return maxValue * slider.value;
    }

    public void OnSliderDragger(){
        if(useWholeNumbers)
        {
        sliderValue = GetValue().Round(); 
        }else{
        sliderValue = GetValue(); 
        }
        sliderAmount_txt.text = sliderValue.ToStringSmart(1);
        OnSliderDragging?.Invoke();
    }

    public void OnSliderDraggerPercent(){
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnStoppedSliderDrag?.Invoke();
    }
}
