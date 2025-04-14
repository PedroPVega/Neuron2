using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;

public abstract class Simulator
{
    protected List<Number> Numbers {get; set;}
    protected System.Random rdn = new System.Random();
    protected static double MaxDouble = double.MaxValue;
    public static int ImgSize = 784;
    public int TrainingSetSize {get; set;}

    public Simulator(int m)
    {
        TrainingSetSize = m;
    }
    public abstract void LoadData(int m, string path);
    public double GetDist1(Number point, Number center)
    {
        double distance = 0;
        for (int t = 0; t < point.Pixels.Length; t++)
        {
            distance += Math.Abs(point.Pixels[t] - center.Pixels[t]);
        }
        return distance;
    }

    public double GetDist2(Number point, Number center)
    {
        double distance = 0;
        double temp;

        for (int t = 0; t < point.Pixels.Length; t++)
        {
            temp = point.Pixels[t] - center.Pixels[t];
            distance += temp*temp;
        }
        return Math.Pow(distance, 0.5);
    }

 
    /*
    public void FillIntZeros(int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = 0;
        }
    }

    public void FillDoubleZeros(double[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            array[i] = 0;
        }
    }
    
    public void FillRandomblyMatrix2(double[][] mat)
    {
        for (int i = 0; i < mat.Length; i++)
        {
            for (int j = 0; j < mat[i].Length; j++)
            {
                mat[i][j] = UnityEngine.Random.Range(0.0f,1.0f);
            }
        }
    }
    */
}