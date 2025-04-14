using UnityEngine;
using UnityEngine.UI;

public class PanelHandler : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public GameObject panel;
    void Start()
    {
        
    }

    public void OnMouseEnter()
    {
        panel.SetActive(true);
        //Debug.Log("Show information");
    }

    public void OnMouseExit()
    {
        panel.SetActive(false);
        //Debug.Log("Hide information");
    }

}
