using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class EpochSlider : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Slider slider;
    public TMP_Text text;
    public Button TrainButton;
    void Start()
    {
        
    }

    public void UpdateEpoch()
    {
        float val = slider.value;
        text.text = $"Number of training pochs = {val:F0}";
        if (!TrainButton.enabled)
        {
            TrainButton.enabled = true;
        }
    }
}
