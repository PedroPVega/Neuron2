using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.UI;

public class NNSlidersHandler : MonoBehaviour
{
    public TMP_InputField Hinput;
    public TMP_InputField Linput;
    public TextMeshProUGUI Recap;
    public Button TrainButton;
    public Transform HiddenLayers;
    public Transform ConnectionsHolder;
    public Transform InputLayer;
    public Transform OutputLayer;
    public GameObject LayerPrefab;
    public GameObject NeuronPrefab;
    public GameObject ConnectionPrefab;
    private int H;
    private int lastH;
    private int L;
    private int lastL;
    private int nbWeights;
    private int nbBiases;

    void Start()
    {
        H = 1;
        lastH = 1;
        L = 1;
        lastL = 1;
        nbWeights = 794;
        nbBiases = 11;
        UpdateLayers();
        StartCoroutine(ResetConnections(1));
    }

    public void HChange()
    {
        bool success = Int32.TryParse(Hinput.text, out H);
        if (Hinput.text == "")
        {
            H = 1;
        }
        else if (!success)
        {
            Hinput.text = lastH.ToString();
            H = lastH;
        }
        else if (H < 1 || H > 128)
        {
            Hinput.text = lastH.ToString();
            H = lastH;
        }
        if (H != lastH)
        {
            UpdateAllNeurons();
            lastH = H;
            nbBiases = L*H+10;
            nbWeights = 784*H + H*(L-1) + H*10;
            UpdateHText();
        }
        if (!TrainButton.enabled)
        {
            TrainButton.enabled = true;
        }
    }
    public void UpdateHText()
    {
        Recap.text = $"Le réseau de neurones est un perceptron multi-couches composé de :\n- 784 entrées correspondant aux valeurs de luminance de chaque pixel de l'image (taille 28x28).\n- 10 sorties correspondant aux probas d'appartenance à chaque classe.\n- {L} couche.s cachée.s avec {H} neurones.\n- Nombre de poids : {nbWeights}\n- Nombre de bias : {nbBiases}";
    }
    public void LChange()
    {
        bool success = Int32.TryParse(Linput.text, out L);
        if (Linput.text == "")
        {
            L = 1;
        }
        else if (!success)
        {
            Linput.text = lastL.ToString();
            L = lastL;
        }
        else if (L < 1 || L > 8)
        {
            Linput.text = lastL.ToString();
            L = lastL;
        }
        
        if (L != lastL)
        {
            UpdateLayers();
            nbBiases = L*H+10;
            nbWeights = 784*H + H*(L-1) + H*10;
            lastL = L;
            UpdateLText();
        }
        if (!TrainButton.enabled)
        {
            TrainButton.enabled = true;
        }
    }
    public void UpdateLText()
    {
        Recap.text = $"Le réseau de neurones est un perceptron multi-couches composé de :\n- 784 entrées correspondant aux valeurs de luminance de chaque pixel de l'image (taille 28x28).\n- 10 sorties correspondant aux probas d'appartenance à chaque classe.\n- {L} couche.s cachée.s avec {H} neurones.\n- Nombre de poids : {nbWeights}\n- Nombre de bias : {nbBiases}";
    }
    public void UpdateLayers()
    {
        int nbLayersShown;
        if (L > 4)
            nbLayersShown = 4;
        else
            nbLayersShown = L;
        StartCoroutine(ClearLayersAndReset(nbLayersShown));
    }
    IEnumerator ClearLayersAndReset(int l)
    {
        foreach (Transform layer in HiddenLayers)
        {
            Destroy(layer.GameObject());
            Debug.Log("layer destroyed");
        }
        yield return new WaitUntil(() => HiddenLayers.childCount == 0);
        Debug.Log("All layers destroyed, start reseting");
        for (int i = 0; i < l; i++)
        {
            AddLayer();
            yield return new WaitUntil(() => HiddenLayers.childCount == i+1);
        }
        List<Vector2> positions = GetPositionsLayers(l,HiddenLayers);
        int j = 0;
        foreach (Transform layer in HiddenLayers)
        {
            layer.position = positions[j];
            UpdateNeurons(layer);
            j++;
        }
        StartCoroutine(ResetConnections(l));
    }
    public void AddLayer()
    {
        GameObject clone = Instantiate(LayerPrefab, HiddenLayers);
        Debug.Log("Added one layer");
    }
    public void UpdateAllNeurons()
    {
        foreach (Transform layer in HiddenLayers)
        {
            UpdateNeurons(layer);
        }
        StartCoroutine(ResetConnections(HiddenLayers.childCount));
    }
    public void UpdateNeurons(Transform layer)
    {
        StartCoroutine(ClearLayerAndReset(layer));
    }
    IEnumerator ClearLayerAndReset(Transform layer)
    {
        // Destroy children
        foreach (Transform child in layer)
        {
            Destroy(child.gameObject);
        }

        // Wait until all children are gone
        yield return new WaitUntil(() => layer.childCount == 0);

        int nbNeuronsShown;
        if (H > 4)
            nbNeuronsShown = 4;
        else
            nbNeuronsShown = H;
        
        for (int j = 0; j < nbNeuronsShown; j++)
        {
            GameObject clone = Instantiate(NeuronPrefab,layer);
        }
        List<Vector2> positionsList = GetPositions(nbNeuronsShown, layer);
        int i = 0;
        foreach (Transform child in layer)
        {
            child.position = positionsList[i];
            i++;
        }
        yield return new WaitUntil(() => layer.childCount == H);
    }
    IEnumerator ResetConnections(int nbLayersShown)
    {
        foreach (Transform connection in ConnectionsHolder)
        {
            Destroy(connection.GameObject());
        }
        yield return new WaitUntil(() => ConnectionsHolder.childCount == 0);
        StartCoroutine(RebuildConnections(nbLayersShown));
    }
    IEnumerator RebuildConnections(int nbLayersShown)
    {
        float distance;
        float angle;
        // Between input neurons and first hidden layer
        foreach (Transform inputNeuron in InputLayer)
        {   
            if (inputNeuron.tag != "NotNeuron")
            {
                foreach (Transform hiddenNeuron in HiddenLayers.GetChild(0))
                {
                    Vector3 direction;
                    GameObject connection = Instantiate(ConnectionPrefab,ConnectionsHolder);
                    Vector3 pos = (hiddenNeuron.position + inputNeuron.position)/2;
                    pos[2] = 0;
                    connection.transform.position = pos;
                    direction = hiddenNeuron.position - inputNeuron.position;
                    distance = direction.magnitude; 
                    connection.transform.localScale = new Vector3(distance, connection.transform.localScale.y, 1f);
                    angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    connection.transform.rotation = Quaternion.Euler(0, 0, angle);          
                }
            }
        }
        // Between hidden layer i and hidden layer i+1
        for (int i = 0; i < nbLayersShown-1; i++)
        {
            foreach (Transform hiddenNeuron1 in HiddenLayers.GetChild(i))
            {
                foreach (Transform hiddenNeuron2 in HiddenLayers.GetChild(i+1))
                {
                    Vector3 direction;
                    GameObject connection = Instantiate(ConnectionPrefab,ConnectionsHolder);
                    Vector3 pos = (hiddenNeuron1.position + hiddenNeuron2.position)/2;
                    pos[2] = 0;
                    connection.transform.position = pos;
                    direction = hiddenNeuron1.position - hiddenNeuron2.position;
                    distance = direction.magnitude; 
                    connection.transform.localScale = new Vector3(distance, connection.transform.localScale.y, 1f);
                    angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    connection.transform.rotation = Quaternion.Euler(0, 0, angle);   
                }
            }
        }
        
        // Between last hidden layer and output layer
        foreach (Transform hiddenNeuron in HiddenLayers.GetChild(HiddenLayers.childCount-1))
        {
            foreach (Transform outputNeuron in OutputLayer)
            {
                Vector3 direction;
                GameObject connection = Instantiate(ConnectionPrefab,ConnectionsHolder);
                Vector3 pos = (hiddenNeuron.position + outputNeuron.position)/2;
                pos[2] = 0;
                connection.transform.position = pos;
                direction = hiddenNeuron.position - outputNeuron.position;
                distance = direction.magnitude; 
                connection.transform.localScale = new Vector3(distance, connection.transform.localScale.y, 1f);
                angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                connection.transform.rotation = Quaternion.Euler(0, 0, angle);          
            }
        }
        yield return new WaitUntil(() => ConnectionsHolder.childCount == nbWeights);
    }
    public List<Vector2> GetPositions(int numberOfNeurons, Transform layerTransform)
    {
        if (numberOfNeurons > 4)
        {
            Debug.Log("Replace for prefab");
        }
        List<Vector2> liste = new List<Vector2>();
        switch (numberOfNeurons)
        {
            case 0:
            Debug.Log("ERROR, 0 neurones");
            break;
            case 1:
            liste.Add(layerTransform.position);
            break;
            case 2:
            liste.Add(layerTransform.position + new Vector3(0,1f,0));
            liste.Add(layerTransform.position + new Vector3(0,-1f,0));
            break;
            case 3:
            liste.Add(layerTransform.position + new Vector3(0,1.5f,0));
            liste.Add(layerTransform.position);
            liste.Add(layerTransform.position + new Vector3(0,-1.5f,0));
            break;
            case 4:
            liste.Add(layerTransform.position + new Vector3(0,2.25f,0));
            liste.Add(layerTransform.position + new Vector3(0,0.75f,0));
            liste.Add(layerTransform.position + new Vector3(0,-0.75f,0));
            liste.Add(layerTransform.position + new Vector3(0,-2.25f,0));
            break;
            default:
            Debug.Log("ERROR, default");
            break;
        }
        return liste;
    }
    public List<Vector2> GetPositionsLayers(int numberOfLayers, Transform layerTransform)
    {
        if (numberOfLayers > 4)
        {
            Debug.Log("Replace for prefab");
        }
        List<Vector2> list = new List<Vector2>();
        switch (numberOfLayers)
        {
            case 0:
            Debug.Log("ERROR, 0 layers");
            break;
            case 1:
            list.Add(layerTransform.position);
            break;
            case 2:
            list.Add(layerTransform.position + new Vector3(-0.5f,0,0));
            list.Add(layerTransform.position + new Vector3(0.5f,0,0));
            break;
            case 3:
            list.Add(layerTransform.position + new Vector3(-1,0,0));
            list.Add(layerTransform.position);
            list.Add(layerTransform.position + new Vector3(1,0,0));
            break;
            case 4:
            list.Add(layerTransform.position + new Vector3(-1.5f,0,0));
            list.Add(layerTransform.position + new Vector3(-0.5f,0,0));
            list.Add(layerTransform.position + new Vector3(0.5f,0,0));
            list.Add(layerTransform.position + new Vector3(1.5f,0,0));
            break;
            default:
            Debug.Log("ERROR, default");
            break;
        }
        return list;
    }
}
