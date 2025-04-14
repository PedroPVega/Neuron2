using UnityEngine;
using TMPro;

public class StoreValuesScript : MonoBehaviour
{
    public TMP_Text predictionText;
    public GameObject NeuralVisual;
    public string accuracy = "";
    public string entropy = "";
    public DeepLearner NeuralNetwork {get;set;}
    public void TestNumber(GameObject gameObj)
    {
        Debug.Log("On tente de predire ce nombre");
        PathHolder pH = gameObj.GetComponent<PathHolder>();
        //Texture2D tex = gameObj.GetComponent<Texture2D>();
        Texture2D tex = pH.texture;
        double[] vector = new double[784];
        for (int i = 0; i < 28; i++)
        {
            //Debug.Log($"{tex.GetPixel(i,0)[0]},{tex.GetPixel(i,1)[0]},{tex.GetPixel(i,2)[0]},{tex.GetPixel(i,3)[0]},{tex.GetPixel(i,4)[0]},{tex.GetPixel(i,5)[0]},{tex.GetPixel(i,6)[0]},{tex.GetPixel(i,7)[0]},{tex.GetPixel(i,8)[0]},{tex.GetPixel(i,9)[0]},{tex.GetPixel(i,10)[0]},{tex.GetPixel(i,11)[0]},{tex.GetPixel(i,12)[0]},{tex.GetPixel(i,13)[0]},{tex.GetPixel(i,14)[0]},{tex.GetPixel(i,15)[0]},{tex.GetPixel(i,16)[0]},{tex.GetPixel(i,17)[0]},{tex.GetPixel(i,18)[0]},{tex.GetPixel(i,19)[0]},{tex.GetPixel(i,20)[0]},{tex.GetPixel(i,21)[0]},{tex.GetPixel(i,22)[0]},{tex.GetPixel(i,23)[0]},{tex.GetPixel(i,24)[0]},{tex.GetPixel(i,25)[0]},{tex.GetPixel(i,26)[0]},{tex.GetPixel(i,27)[0]}");
            for (int j = 0; j < 28; j++)
            {
                vector[i*28 + j] = (double)tex.GetPixel(i,j)[0];
            }
        }
        int a = NeuralNetwork.GetPrediction(vector);
        predictionText.text = $"Le chiffre était un\n{a}!";
        NeuralVisual.SetActive(false);
        //int[] firstPredictions = NeuralNetwork.Get20FirstPredictions();
        //Debug.Log($"{firstPredictions[0]},{firstPredictions[1]},{firstPredictions[2]},{firstPredictions[3]},{firstPredictions[4]},{firstPredictions[5]},{firstPredictions[6]},{firstPredictions[7]},{firstPredictions[8]},{firstPredictions[9]},{firstPredictions[10]},{firstPredictions[11]},{firstPredictions[12]},{firstPredictions[13]},{firstPredictions[14]},{firstPredictions[15]},{firstPredictions[16]},{firstPredictions[17]},{firstPredictions[18]},{firstPredictions[19]}");
    }
}
