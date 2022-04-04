using System.Collections.Generic;
using UnityEngine;

public class Mutator
{
    public List<Genome> Mutate(List<Genome> parents, MutationVariables mvars)
    {
        var results = new List<Genome>();

        for(int i = 0; i < mvars.numberOfMutations; i++)
        {
            // Select random parent for mutation
            var parent = parents[Random.Range(0, parents.Count)].Network;

            var nn = new NeuralNetwork(parent.Network.layers[0].layerSize);

            MutateBias(nn, parent, mvars);
            MutateWeights(nn, parent, mvars);

            var genome = new Genome(nn);
            results.Add(genome);
        }

        return results;
    }

    private void MutateBias(NeuralNetwork network, NeuralNetwork parent, MutationVariables mvars)
    {
        for(int i = 1; i < network.Network.layers.Count; i++)
        {
            var layer = network.Network.layers[i];
            var biasesToMutate = IndiciesToMutate(layer.biases.count, mvars.biasMutationProc);

            for (int j = 0; j < layer.biases.count; j++)
            {
                if (biasesToMutate.Contains(j))
                {
                    // Mutate
                    float value = parent.Network.layers[i].biases.values[j];
                    layer.biases.values[j] = value + Random.Range(-mvars.biasMutationRange, mvars.biasMutationRange);
                }
                else
                {
                    // Copy
                    layer.biases.values[j] = parent.Network.layers[i].biases.values[j];
                }
            }
        }
    }

    private List<int> IndiciesToMutate(int max, float proc)
    {
        var result = new List<int>();
        int count = Mathf.FloorToInt(max * proc);

        while (result.Count < count)
        {
            int value = Random.Range(0, max);
            if (!result.Contains(value))
                result.Add(value);
        }

        return result;
    }

    private void MutateWeights(NeuralNetwork network, NeuralNetwork parent, MutationVariables mvars)
    {
        for(int i = 1; i < network.Network.layers.Count; i++)
        {
            var layer = network.Network.layers[i];
            var prevLayer = network.Network.layers[i - 1];
            var weightsToMutate = IndiciesToMutate2D(prevLayer.layerSize, layer.layerSize, mvars.weightMutationProc);

            for (int j = 0; j < prevLayer.layerSize; j++)
            {
                for (int k = 0; k < layer.layerSize; k++)
                {
                    layer.weights.values[j, k] = parent.Network.layers[i].weights.values[j, k];
                }
            }

            foreach((int x, int y) in weightsToMutate)
            {
                float value = parent.Network.layers[i].weights.values[x, y];
                layer.weights.values[x, y] = value + Random.Range(-mvars.weightMutationRange, mvars.weightMutationRange);
            }
        }
    }

    private List<(int, int)> IndiciesToMutate2D(int max1D, int max2D, float proc)
    {
        var result = new List<(int, int)>();
        int count = Mathf.FloorToInt(max1D * max2D * proc);

        while (result.Count < count)
        {
            int value1D = Random.Range(0, max1D);
            int value2D = Random.Range(0, max2D);

            if (!result.Contains((value1D, value2D)))
                result.Add((value1D, value2D));
        }

        return result;
    }
}