using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Collections;
using TMPro;

public class KMeansLauncher : MonoBehaviour
{
    public Slider slider;
    public Toggle toggle;
    public TMP_Dropdown dropdown;
    private KMeansSimulator simulator;
    public GameObject ImagePrefab; 
    public GameObject LoadingScreen;
    public Transform ImageContainer; 
    void Start()
    {
        LoadingScreen.SetActive(true);
        _ = LoadCsv();
    }
    public async Task LoadCsv()
    {
        await Task.Run(() => 
        {
            simulator = new KMeansSimulator(6000, Path.Combine(Application.streamingAssetsPath,"Data/mnist_train.csv"));
        });
        LoadingScreen.SetActive(false);
    }

    public void Launch()
    {
        // Reset all assets
        ResetAll();
        // Initialization of variables
        int K = Convert.ToInt32(Math.Round(18*slider.value+8));
        int type = dropdown.value + Convert.ToInt32(toggle.isOn);
        // Launch KMeans Algorithm
        KMeans(K,type,20);
        Debug.Log("KMeans Finished");
    }
    private void ResetAll()
    {
        simulator.Reset();
        var dirPath = "./Assets/StreamingAssets/Barycenters/";
        if(Directory.Exists(dirPath))
        {
            Directory.Delete(dirPath, true);
            Directory.CreateDirectory(dirPath);
        }
        else
        {
            Directory.CreateDirectory(dirPath);
        }
    }

    void LoadImages(string imageFolderName, int K)
    {
        RectTransform rt = ImageContainer.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(K*120f, 164f); 
        string folderPath = Path.Combine(Application.streamingAssetsPath, imageFolderName);
        if (!Directory.Exists(folderPath))
        {
            Debug.LogError("Image folder not found: " + folderPath);
            return;
        }

        foreach (Transform child in ImageContainer) 
        {
            Destroy(child.gameObject); // Clear existing images
        }
        string[] files = Directory.GetFiles(folderPath, "*.png");
        foreach (string filePath in files)
        {
            StartCoroutine(LoadImageCoroutine(filePath));
        }
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
        #   endif
            {
                Debug.LogError("Error loading image: " + request.error);
            }
            else
            {
                Texture2D tex = DownloadHandlerTexture.GetContent(request);
                Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);

                GameObject imgObj = Instantiate(ImagePrefab, ImageContainer);
                imgObj.GetComponent<Image>().sprite = sprite;
            }
        }
    }
    public void KMeans(int K,int type, int iterations)
    {
        switch (type)
        {
            default:
            Debug.Log("ERROR");
            break;
            case 0:
            Debug.Log("Randomly created barycenters");
            simulator.CreateRandomBarycenters(K);
            break;
            case 1:
            Debug.Log("Randomly selected barycenters");
            simulator.SelectRandomBarycenters(K);
            break;
            case 2:
            Debug.Log("Smart barycenters");
            simulator.SelectRandomBarycentersPlus();
            break;
        }
        for (int i = 0; i < iterations; i++)
        {
            simulator.KmeansIteration(K);
            LoadImages("Barycenters/",K);
        }
    }
    
}