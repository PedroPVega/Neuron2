using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

[RequireComponent(typeof(TMP_Text))]
public class Linkedin : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        TMP_Text pTextMeshPro = GetComponent<TMP_Text>();

        int linkIndex = TMP_TextUtilities.FindIntersectingLink(pTextMeshPro, eventData.position, Camera.main);
        Debug.Log(linkIndex);
        if(linkIndex != -1)
        {
            TMP_LinkInfo linkInfo = pTextMeshPro.textInfo.linkInfo[linkIndex];
            Debug.Log("Opening URL: " + linkInfo.GetLinkID());
            Application.OpenURL(linkInfo.GetLinkID());
        }
    }
}
