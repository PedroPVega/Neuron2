using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
public class SliderHandler : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI sliderValue;
    public Toggle toggleKMeansPlusPlus;
    public TMP_Dropdown dropdown;
    public TextMeshProUGUI Krecap;
    private int K {get; set;}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        slider.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
        K = 8;
    }

    void ValueChangeCheck()
    {
        K = Convert.ToInt32(Math.Round(18*slider.value+8));
        sliderValue.text = "K = " + K.ToString();
        Krecap.text = "Nombre de clusters : " + K.ToString();
        if (K != 10)
        {
            toggleKMeansPlusPlus.isOn = false;
            toggleKMeansPlusPlus.enabled = false;
        }
        else if(dropdown.value == 1)
        {
            toggleKMeansPlusPlus.enabled = true;
        }
    }
}
