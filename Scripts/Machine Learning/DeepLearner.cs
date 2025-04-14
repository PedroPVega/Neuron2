using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Encog.Engine.Network.Activation;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks.Training.Propagation.Back;
using System.Diagnostics;
using System.Threading.Tasks;
using Encog.Neural.Networks.Training.Simple;
public class DeepLearner : Simulator
{
    const int OutputSize = 10;
    private BasicMLDataSet TrainSet {get; set;}
    private BasicMLDataSet TestSet {get; set;}
    private List<IMLData> TestSetInputs {get; set;}
    private List<IMLData> TestSetLabels {get; set;}
    private BasicNetwork Network {get; set;}
    public string LastError {get; set;}
    public string LastEpoch {get; set;}
    //public string LastAccuracy {get;set;}

    public DeepLearner(int m, string dataPath) : base(m)
    {
        Network = new BasicNetwork();
        TrainSet = new BasicMLDataSet();
        TestSet = new BasicMLDataSet();
        LastError = "-";
        LastEpoch = "0";
        //LastAccuracy = "";
        LoadData(m, dataPath);
    }

    public override void LoadData(int m, string path)
    {
        //string path = "../mnist_train.csv"; // Path to CSV file
        var (data, labels) = LoadCsv(path, m);

        int total = data.Count;
        int trainCount = total - 1000;

        for (int i = 0; i < trainCount; i++)
        {
            TrainSet.Add(new BasicMLDataPair(data[i], labels[i]));
        }
        for (int i = trainCount; i < total; i++)
        {
            TestSet.Add(new BasicMLDataPair(data[i], labels[i]));
        }
    }

    public (List<IMLData> inputs, List<IMLData> outputs) LoadCsv(string file, int a)
    {
        int b = a;
        var inputs = new List<IMLData>();
        var outputs = new List<IMLData>();
        //string path = Path.Combine(Application.streamingAssetsPath, "mnist_train.csv");
        var lines = File.ReadAllLines(file).Skip(1);
        foreach (var line in lines)
        {
            b--;
            var parts = line.Split(',').Select(double.Parse).ToArray();
            var label = (int)parts[0];
            for (int i = 0; i < parts.Length; i++)
            {
                parts[i] = parts[i]/255.0;
            }
            var input = new BasicMLData(parts.Skip(1).ToArray());

            // one-hot encoding
            var outputArr = new double[10];
            outputArr[label] = 1.0;
            var output = new BasicMLData(outputArr);

            inputs.Add(input);
            outputs.Add(output);
            if (b == 0)
                return (inputs, outputs);
        }

        return (inputs, outputs);
    }

    public void CreateNetwork(int H, int L)
    {
        var network = new BasicNetwork();
        network.AddLayer(new BasicLayer(null, true, ImgSize));

        for (int i = 0; i < L; i++)
            network.AddLayer(new BasicLayer(new ActivationSigmoid(), true, H));

        network.AddLayer(new BasicLayer(new ActivationSoftMax(), false, OutputSize));

        network.Structure.FinalizeStructure();
        network.Reset();
        Network = network;
    }

    public void TrainNetwork(int E)
    {
        var train = new Backpropagation(Network, TrainSet);
        train.NumThreads = 1;
        for (int epoch = 0; epoch < E; epoch++)
        {
            train.Iteration();
            LastError = train.Error.ToString("F4");
            LastEpoch = (epoch + 1).ToString();
        }
    }
    public double GetAccuracy()
    {
        int correct = 0;
        int predicted = 0;
        int actual = 0;
        double accuracy;
        // Evaluate
        for (int i = 0; i < TestSet.Count; i++)
        {
            var output = Network.Compute(TestSet[i].Input);
            predicted = ArgMax(output); 
            actual = ArgMax(TestSet[i].Ideal);
            if (predicted == actual) 
                correct++;
        }
        accuracy = (double)correct / TestSet.Count * 100;
        return accuracy;
    }

    public int GetPrediction(double[] vector)
    {
        var input = new BasicMLData(vector);
        var output = Network.Compute(input);
        return ArgMax(output);
    }

    public int[] Get20FirstPredictions()
    {
        int[] predictions = new int[20];
        for (int i = 0; i < 20; i++)
        {
            var output = Network.Compute(TrainSet[i].Input);
            predictions[i] = ArgMax(output); 
        }
        return predictions;
    }

    public int ArgMax(IMLData data)
    {
        var basicData = (BasicMLData)data;  // Cast to BasicMLData
        var values = basicData.Data;        // Access the Data property
        return values.ToList().IndexOf(values.Max());  // Find the index of the maximum value
    }
    
}