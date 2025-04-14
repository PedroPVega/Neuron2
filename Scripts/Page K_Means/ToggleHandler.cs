using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection.Emit;
//using UnityEngine.UIElements;

public class ToggleHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Toggle toggle;
    public TextMeshProUGUI initializationRecap;
    public TMP_Dropdown dropdown;
    private int K {get; set;}

    void Start()
    {
        toggle.onValueChanged.AddListener(delegate {ValueChangeCheck(); });
        K = 8;
    }

    void ValueChangeCheck()
    {
        Debug.Log("Toggle clicked : " + toggle.isOn.ToString());
        if (toggle.isOn)
        {
            initializationRecap.text = "Initialisation : \n - Smart KMeans (Selection automatique des barycentres)";
        }
        else if (dropdown.value == 0)
        {
            initializationRecap.text = "Initialisation : \n - Generation aléatoire des barycentres";
        }
        else
        {
            initializationRecap.text = "Initialisation : \n - Selection aléatoire des barycentres parmis les données";
        }
    }
}
