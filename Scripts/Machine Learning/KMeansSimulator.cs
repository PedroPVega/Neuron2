using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class KMeansSimulator : Simulator
{
    public Transform ImageContainer; // ScrollView/Panel Content
    public GameObject ImagePrefab; // UI Image prefab with Image component
    public List<Number> Barycenters {get; set;}
    public KMeansSimulator(int m, string path) : base(m) 
    {
        Numbers = new List<Number> {};
        Barycenters = new List<Number> {};
        // Load Data into list
        LoadData(TrainingSetSize, path);
        //Debug.Log("Data loaded");
    }

    public override void LoadData(int m, string filePath)
    {
        string[]? values;
        int[] vector = new int[785];
        using (StreamReader reader = new StreamReader(filePath))
        {
            string? line = reader.ReadLine(); // Ignore first line, titles
            for (int i = 0; i < 6000; i++) // Load 6000 numbers
            {
                
                    line = reader.ReadLine();  // Read line
                    if (line != null)
                    {
                        values = line.Split(','); // Split by comma
                        for (int t = 0; t < values.Length; t++) 
                            vector[t] = Convert.ToInt32(values[t]);                        
                
                    }
                Numbers.Add(new Number(vector));
            } 
        }
    }

    private void SimulateKMeans(int K, int iterations, int type)
    {
        switch (type)
        {
            default:
            //Debug.Log("ERROR");
            break;
            case 0:
            //Debug.Log("Randomly created barycenters");
            CreateRandomBarycenters(K);
            break;
            case 1:
            //Debug.Log("Randomly selected barycenters");
            SelectRandomBarycenters(K);
            break;
            case 2:
            //Debug.Log("Smart barycenters");
            SelectRandomBarycentersPlus();
            break;
        }
        for (int i = 0; i < iterations; i++)
        {
            // Console.WriteLine("Itération {0} en cours", i+1);
            KmeansIteration(K);
        }
    }

    public void Reset()
    {
        foreach (Number item in Numbers)
        {
            item.DesignatedClass = -1;
        }
        Barycenters.Clear();
        Numbers[0].ResetBarsIds();
    }

    public void KmeansIteration(int K)
    {
        string title;
        UpdateClasses(K);
        UpdateBarycenters(K);
        // Save barycenters in directory
        foreach (var Bary in Barycenters)
        {
            title = $"barycentre {- Bary.Id}";
            SaveImage(Bary,title);
        }
        // Show barycenters into scroller
    }

    public void UpdateClasses(int K)
    {
        double[] distances = new double[K];
        double minDist = MaxDouble;
        int temp = 0;

        // foreach point
        foreach (Number num in Numbers)
        {
            minDist = MaxDouble;
            for (int k = 0; k < K; k++)
            {
                // foreach point we calculate its distance to all barycenters
                distances[k] = GetDist1(Barycenters[k], num);
                if(distances[k] < minDist)
                {
                    minDist = distances[k];
                    temp = k; // we assign its class to the nearest barycenter
                }
            }
            // assign nearest barycenter in matrix
            num.DesignatedClass = temp;
        }
    }

    public void UpdateBarycenters(int K)
    {

        //Etape 1, remise à 0 des coordonnées des barycentres
        foreach(Number bary in Barycenters)
            bary.Set2Empty(); 

        //Etape 2, somme des coordonnées des points les plus proches à chaque barycentre
        int[] nbOfSubsPerClass = UpdatePart1(K);
        //PrintPointsPerClass(nbOfSubsPerClass);

        //Etape 3, diviser par le nombre de points afin de moyenner les coordonnées du barycentre
        foreach (Number bary in Barycenters)
        {
            //Debug.Log($"bary numero {-bary.Id}");
            UpdatePart2(bary, nbOfSubsPerClass[- bary.Id]);
        }

        //Etape 4, en cas d'une classe vide, on cherche à reinitialiser le barycentre
        if (nbOfSubsPerClass.Contains(0))
        {
            int bar = 0;
            Number num;
            for (int i = 0; i < nbOfSubsPerClass.Length; i++)
            {
                if (nbOfSubsPerClass[i] == 0)
                {
                    bar = i;
                    break;
                }
            }
            num = Numbers[UnityEngine.Random.Range(0,Numbers.Count)];
            //Console.WriteLine("oupigoupi parmis {0}, je cherche {1}", num.DesignatedClass, bar);
            Barycenters[bar] = new Number(num);
            Barycenters[bar].ChangeId(-bar);
            Barycenters[bar].DesignatedClass = -1;
            Barycenters[bar].Label = -1;
        }
    }

    public int[] UpdatePart1(int K)
    {
        int[] nbOfSubsPerClass = new int[K];
        //FillIntZeros(nbOfSubsPerClass);

        foreach (Number num in Numbers)
        {
            //Console.WriteLine("{0}",num.DesignatedClass);
            nbOfSubsPerClass[num.DesignatedClass] += 1;
            for (int i = 0; i < 784; i++)
                Barycenters[num.DesignatedClass].Pixels[i] += num.Pixels[i];
            
        } 

        int s = 0;
        foreach (int item in nbOfSubsPerClass)
            s += item;

        if (s != TrainingSetSize)
            Debug.Log($"ERROR : {s}");

        //Debug.Log("update 1");
        return nbOfSubsPerClass;
    }

    public void UpdatePart2(Number barycenter, int n)
    {
        if(n != 0)
        {
            for (int i = 0; i < barycenter.Pixels.Length; i++)
            {
                barycenter.Pixels[i] /= n;
            }
        }
        else
        {
            //Debug.Log("barycenter replaced");
            Number c = new Number(null, false, false, true);
            for (int i = 0; i < barycenter.Pixels.Length; i++)
            {
                barycenter.Pixels[i] = c.Pixels[i];
            }
            //Debug.Log($"barycenter {barycenter.Id}");
            //chiant();
        }
    }

    public double GetAccuracy()
    {
        int K = Barycenters.Count;
        int temp;
        double acc = 0;
        double classAcc;
        int[,] mat = new int[K, 11]; // 10 digits + le total de points pour la classe
        for (int i = 0; i < K; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                mat[i,j] = 0;
            }
        }
        // Chaque ligne correspond à un barycentre
        // 1e colonne : taille du cluster
        // 2e à K+1-ième colonne : nombre d'apparitions de chaque label dans ce cluster
        
        // pour chaque classe
        foreach (Number num in Numbers)
        {
            mat[num.DesignatedClass,0] += 1; // determiner la longueur de la classe
            mat[num.DesignatedClass,num.Label+1] += 1; 
        }

        for (int i = 0; i < K; i++)
        {
            temp = -1;
            for (int j = 1; j < 11; j++)
            {
                if (mat[i,j] > temp)
                {
                    temp = mat[i,j]; // determiner le label le plus commun de chaque classe
                }
            }
            if (mat[i,0] != 0)
            {
                classAcc = (double)temp / (double)mat[i,0];
                acc += classAcc; // calculer la proportion d'apparition de se label dans cette classe
            }
        }
        acc = acc / K;
        return acc * 100;
        ///////////////////////////////////////
        
    }

    public void CreateRandomBarycenters(int K)
    {
        for (int k = 0; k < K; k++)
        {
            Barycenters.Add(new Number(null, false, true, true));
            //Debug.Log($" barycentre cree{Barycenters[k].Id}");
        }
    }

    public void SelectRandomBarycenters(int K)
    {
        int selected;
        List<int> alreadySelected = new List<int>();
        for (int k = 0; k < K; k++)
        {
            selected = Convert.ToInt32(UnityEngine.Random.Range(0,Numbers.Count));
            if (!alreadySelected.Contains(selected))
            {
                Barycenters.Add(new Number(Numbers[selected].Pixels,false,true,false));
                //Console.WriteLine("Barycenter selected : " + Numbers[selected].ToString());
                alreadySelected.Add(selected);
            }
            else
                k--;
 
        }
    }

    public void SelectRandomBarycentersPlus()
    {
        int selected;
        int selectedLabel;
        List<int> alreadySelected = new List<int>();
        for (int k = 0; k < 10; k++)
        {
            selected = Convert.ToInt32(UnityEngine.Random.Range(0,Numbers.Count));
            selectedLabel = Numbers[selected].Label;
            if (!alreadySelected.Contains(selectedLabel))
            {
                Barycenters.Add(new Number(Numbers[selected].Pixels,false,true,false));
                //Console.WriteLine("Barycenter selected : " + Numbers[selected].ToString());
                alreadySelected.Add(selectedLabel);
            }
            else
                k--;
 
        }
    }

    public void ShowClasses(int K)
    {
        int[] nbPtsPerClass = new int[K];
        //FillIntZeros(nbPtsPerClass);
        foreach (Number item in Numbers)
        {
            nbPtsPerClass[item.DesignatedClass] += 1;
        }
        PrintPointsPerClass(nbPtsPerClass);
    }

    public void PrintPointsPerClass(int[] nbOfSubsPerClass)
    {
        
        double[] meanLabel = new double[nbOfSubsPerClass.Length];
        //FillDoubleZeros(meanLabel);
        int p;
        foreach (Number item in Numbers)
        {
            p = item.DesignatedClass;
            meanLabel[p] += item.Label;
        }
        for (int i = 0; i < nbOfSubsPerClass.Length; i++)
        {
            meanLabel[i] = meanLabel[i] / nbOfSubsPerClass[i];
        }
        for(int i = 0; i < Barycenters.Count; i++)
        {
            Debug.Log($"Points du barycentre {Barycenters[i].Id} : {nbOfSubsPerClass[i]} points attribués, label moyen : {Math.Round(meanLabel[i],2)}");
        }
    }

    private void SaveImage(Number number, string title)
    {
        int index;
        float gryScVal;
        Texture2D Texture = new Texture2D(28,28,TextureFormat.RGB24_SIGNED,false);
        
        for (int i = 0; i < 28; i++)
        {
                for (int j = 0; j < 28; j++)
                {
                    index = 28*i + j;
                    gryScVal = number.Pixels[index]/255f;
                    gryScVal = (float)Math.Round(gryScVal, 3);
                    //Debug.Log($"couleur à l'index {index} : {gryScVal}");
                    Color color = new Color(gryScVal,gryScVal,gryScVal);
                    Texture.SetPixel(j,i,color);
                }
        }
        
        byte[] bytes = Texture.EncodeToPNG();
        var dirPath = "./Assets/StreamingAssets/Barycenters/";
        if(!Directory.Exists(dirPath)) 
        {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + title + ".png", bytes);
        number.SetPath(dirPath + title + ".png");
    }

    
}