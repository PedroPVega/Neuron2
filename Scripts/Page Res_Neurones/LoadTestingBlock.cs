using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class LoadTestingBlock : MonoBehaviour
{
    public GameObject TestButton;
    public GameObject NumberToTestPrefab;
    public Transform NumbersContainer;
    public TMP_Text EntropyAccuracy;
    public StoreValuesScript StoringUnit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void LoadAll()
    {
        string lastError = StoringUnit.entropy;
        string acc = StoringUnit.accuracy;
        EntropyAccuracy.text = $"Cross Entropy finale : {lastError}\nPrécision sur les données de Test : {acc} %";
        if (NumbersContainer.childCount == 0)
        {
            LoadImages("DA/Example Numbers/");
        }
        else
        {
            TestButton.SetActive(false);
        }
    }

    private void LoadImages(string imageFolderName)
    {
        //RectTransform rt = NumbersContainer.GetComponent<RectTransform>();
        //float height = rt.sizeDelta[1];
        //rt.sizeDelta = new Vector2(numberQtt*135f, height); 
        string folderPath = Path.Combine(Application.streamingAssetsPath, imageFolderName);
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError("Image folder not found: " + folderPath);
            return;
        }

        string[] files = Directory.GetFiles(folderPath, "*.png");
        //Debug.Log($"found {files.Length} .png files");
        StartCoroutine(LoadImagesSequentially(files));
    }
    IEnumerator LoadImageCoroutine(string path)
    {
        string url = "file://" + path;

        using (UnityWebRequest request = UnityWebRequestTexture.GetTexture(url))
        {
            yield return request.SendWebRequest();

            #if UNITY_2020_1_OR_NEWER
                if (request.result != UnityWebRequest.Result.Success)
            #else
                if (request.isNetworkError || request.isHttpError)
            #endif
            {
                Debug.LogError("Error loading image: " + request.error);
            }
            else
            {
                Texture2D tex = DownloadHandlerTexture.GetContent(request);
                Sprite sprite = Sprite.Create(tex, 
                    new Rect(0, 0, tex.width, tex.height), 
                    new Vector2(0.5f, 0.5f));

                GameObject num = Instantiate(NumberToTestPrefab, NumbersContainer);
                foreach (Transform child in num.GetComponent<Transform>())
                {
                    if (child.tag is "NUMBER")
                    {
                        Image img = child.GetComponent<Image>();
                        
                        if (img != null)
                        {
                            img.sprite = sprite;
                            //Debug.Log("Sprite assigned to Image");
                        }
                    }
                    if (child.tag is "BUTT")
                    {
                        PathHolder newHolder = child.AddComponent<PathHolder>();
                        newHolder.texture = tex;
                        /*
                        int a = child.GetComponents<PathHolder>().Count<PathHolder>();
                        Debug.Log($"path holders : {a}");
                        child.GetComponent<PathHolder>().texture = tex;
                        */
                    }
                }
            }
        }
    }

    IEnumerator LoadImagesSequentially(string[] files)
    {
        foreach (string filePath in files)
        {
            yield return StartCoroutine(LoadImageCoroutine(filePath));
        }
        TestButton.SetActive(false);
    }
}
