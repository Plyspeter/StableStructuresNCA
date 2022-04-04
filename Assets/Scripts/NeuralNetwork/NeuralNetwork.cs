using System;
using System.Collections;
using UnityEngine;

public class NeuralNetwork
{
    public Noedify.Net Network { get; set; }
    private static readonly Noedify_Solver solver = Noedify.CreateSolver();
    private readonly int inputsize;
    public bool RunningDone { get; private set; }
    public bool Evaluating { get; set; }
    public float Q { get; set; }

    public NeuralNetwork(int inputsize)
    {
        this.inputsize = inputsize;
        Network = new Noedify.Net();
        RunningDone = false;
        Evaluating = false;
        Q = -1;
        Build();
    }

    private void Build()
    {
        Network.AddLayer(new Noedify.Layer(Noedify.LayerType.Input, inputsize, "Input Layer"));
        Network.AddLayer(new Noedify.Layer(Noedify.LayerType.FullyConnected, Mathf.CeilToInt(inputsize / 3) * 2 + inputsize / 2, Noedify.ActivationFunction.ReLU, "Fully Connected 1"));
        Network.AddLayer(new Noedify.Layer(Noedify.LayerType.Output, inputsize / 2, Noedify.ActivationFunction.Sigmoid, "Output Layer"));

        Debug.unityLogger.logEnabled = false;
        Network.BuildNetwork();
        Debug.unityLogger.logEnabled = true;
    }

    private float[] Run(float[] input)
    {
        if (input.Length != inputsize)
        {
            Debug.LogError("Input does not match networks inputsize!");
            return input;
        }

        solver.Evaluate(Network, Noedify_Utils.AddTwoSingularDims(input), Noedify_Solver.SolverMethod.MainThread);
        return solver.prediction;
    }

    public IEnumerator Run(SimpleGraph sg, int iterations)
    {
        for (int i = 0; i < iterations; i++)
        {
            foreach(var localPos in sg.GetLocalPositions())
            {
                if (!sg.LocalExists(localPos))
                    continue;
                sg.UpdateFromLocal(Run(sg.GetLocal(localPos)), localPos);
                yield return null;
            }
        }
        RunningDone = true;
        yield return null;
    }

    public void SaveModel()
    {
        var now = DateTime.Now;
        var name = $"{now.Day}-{now.Month}-{now.Year};{now.Hour}-{now.Minute}-{now.Second}";
        Network.SaveModel(name, Application.dataPath + "/Models");
    }

    public bool LoadModel(string name)
    {
        return Network.LoadModel(name, Application.dataPath);
    }
}