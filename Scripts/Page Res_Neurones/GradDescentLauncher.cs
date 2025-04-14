using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class GradDescentLauncher : MonoBehaviour
{
    private List<Color> Colors {get; set;}
    public DeepLearner NeuralNetwork {get; set;}
    public Transform ConnectionHolder;
    public TMP_InputField Hinput;
    public TMP_InputField Linput;
    public TMP_Text ErrorEvolution;
    public Slider epochSlider;
    public GameObject ErrorContainer;
    public GameObject TestButton;
    public GameObject LoadingScreen;
    public Button TrainButton;
    public StoreValuesScript StoringUnit;
    private int H {get; set;}
    private int L {get; set;}
    private string LastError {get; set;}
    private string LastEpoch {get; set;}
    private bool NNInitialized {get;set;}
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Camera.main.cullingMask &= ~(1 << LayerMask.NameToLayer("Invisible"));
        NNInitialized = false;
        var await = LoadAsync();
        H = 1;
        L = 1;
        Colors = new List<Color>();
        AddColors(Colors);
        LastError = "";
        LastEpoch = "";
    }

    void Update()
    {
        if (NNInitialized)
        {
            if (NeuralNetwork.LastEpoch != LastEpoch)
            {
                UpdateError();
            }
        }
    }
    public void AddColors(List<Color> list)
    {
        list.Add(new Color(77/255f, 0f, 156/255f));
        list.Add(new Color(45/255f, 18/255f, 179/255f));
        list.Add(new Color(16/255f, 145/255f, 204/255f));
        list.Add(new Color(161/255f, 0f, 119/255f));
        list.Add(new Color(169/255f, 0f, 212/255f));
        list.Add(new Color(189/255f, 11/255f, 35/255f));
    }

    public void Launch()
    {
        TrainButton.enabled = false;
        bool success = Int32.TryParse(Hinput.text, out int h);
        if (H < 1 || H > 128 || !success)
            H = 1;
        else
            H = h;

        success = Int32.TryParse(Linput.text, out int l);
        if (L < 1 || L > 8 || !success)
            L = 1;
        else
            L = l;
        ErrorContainer.SetActive(true);
        int epochQtt = Convert.ToInt32(epochSlider.value);
        NeuralNetwork.CreateNetwork(H, L);
        Debug.Log($"Reseau de neurones créé ; H = {H}   L = {L}");
        StartAsyncProcess(epochQtt);
        Debug.Log("Descente de gradient initialisé");
    }
    public void UpdateError()
    {
        LastError = NeuralNetwork.LastError;
        LastEpoch = NeuralNetwork.LastEpoch;
        ErrorEvolution.text = $"Cross Entropy à l'epoch {LastEpoch}/{Convert.ToInt32(epochSlider.value)} : {LastError}";
        _ = FadeColorAsync(new Color(1f, 0.5f, 0f),new Color(1f, 1f, 1f),2f);
    }
    public void GetAccuracy()
    {
        Debug.Log("Accuracy requested");
        double acc = NeuralNetwork.GetAccuracy();
        StoringUnit.accuracy = acc.ToString("F1");
        StoringUnit.entropy = LastError;
        Debug.Log("Accuracy retrieved");
    }
    public async Task FadeColorAsync(Color fromColor, Color toColor, float time)
    {
        float elapsed = 0f;
        Image imageToChange = ErrorContainer.GetComponent<Image>();
        imageToChange.color = fromColor;
        await Task.Delay(800);
        while (elapsed < time)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / time);
            imageToChange.color = Color.Lerp(fromColor, toColor, t);
            await Task.Yield(); 
        }
        imageToChange.color = toColor; 
    }
    public async void StartAsyncProcess(int e)
    {
        CancellationTokenSource tokenSource = new();

        _ = ColorConnections(tokenSource.Token);
        var task2 = TrainModelAsync(e);       

        await task2;                               

        tokenSource.Cancel();                  
        Debug.Log("TrainModelAsync finished, ColorConnections canceled.");
    }
    private async Task TrainModelAsync(int e)
    {
        Debug.Log("Entrainement declanché");
        Hinput.enabled = false;
        Linput.enabled = false;
        epochSlider.enabled = false;
        TestButton.SetActive(false);
        await Task.Run(() =>
        {
            // This runs on a separate thread
            NeuralNetwork.TrainNetwork(e);
        });

        // Back on Unity's main thread
        Hinput.enabled = true;
        Linput.enabled = true;
        epochSlider.enabled = true;
        StoringUnit.NeuralNetwork = NeuralNetwork;
        TestButton.SetActive(true);
        //Debug.Log("Entrainement terminé!");
        GetAccuracy();
    }
    private async Task LoadAsync()
    {
        LoadingScreen.SetActive(true);
        await Task.Yield();
        await Task.Run(() => 
        {
            NeuralNetwork = new DeepLearner(7000, Path.Combine(Application.streamingAssetsPath,"Data/mnist_train.csv"));
        });
        LoadingScreen.SetActive(false);
        await Task.Yield();
        Canvas.ForceUpdateCanvases();
        Camera.main.cullingMask |= 1 << LayerMask.NameToLayer("Invisible");
        NNInitialized = true;
    }
    private async Task ColorConnections(CancellationToken token)
    {
        try
        {
            while (!token.IsCancellationRequested)
            {
                //Debug.Log("Function1 is doing work...");
                GameObject RandomConnection = ConnectionHolder.GetChild(UnityEngine.Random.Range(0,ConnectionHolder.childCount)).GameObject();
                Image img = RandomConnection.GetComponent<Image>();
                img.color = Colors[UnityEngine.Random.Range(0,Colors.Count)];
                await Task.Delay(1500);
            }
        }
        catch (OperationCanceledException)
        {
            Debug.Log("ChangeColor was cancelled.");
        }
    }
    
}
