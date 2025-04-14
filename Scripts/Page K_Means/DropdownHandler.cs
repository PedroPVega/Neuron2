using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class DropdownHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Toggle toggle;
    public TMP_Dropdown dropdown;
    public Slider slider;
    public TextMeshProUGUI initializationRecap;

    void Start()
    {
        dropdown.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
    }

    public void ValueChangeCheck()
    {
        if (dropdown.value != 1)
        {
            toggle.isOn = false;
            toggle.enabled = false;
            initializationRecap.text = "Initialisation : \n - Generation aléatoire des barycentres";
        }
        else
        {
            initializationRecap.text = "Initialisation : \n - Selection aléatoire des barycentres parmis les données";
            if (Math.Round(18*slider.value+8) == 10)
            {
                toggle.enabled = true;
            }
            
        }
    
    }

}