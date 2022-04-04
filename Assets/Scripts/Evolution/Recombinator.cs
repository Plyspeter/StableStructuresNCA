using System.Collections.Generic;
using UnityEngine;
public class Recombinator
{
    public List<Genome> SingleNeuronRecombine(List<Genome> parents, int numOfChildren)
    {
        List<Genome> results = new List<Genome>(); 

        for (int i = 0; i < numOfChildren; i += 2)
        {
            var child1 = new NeuralNetwork(26 * 2);
            var child2 = new NeuralNetwork(26 * 2);
            var parent1Index = Random.Range(0, parents.Count);
            var parent2Index = Random.Range(0, parents.Count);
            while (parent1Index == parent2Index)
                parent2Index = Random.Range(0, parents.Count);

            var parent1 = parents[parent1Index].Network;
            var parent2 = parents[parent2Index].Network;

            int numOfLayers = child1.Network.LayerCount();
            int layerId = Random.Range(1, numOfLayers);
            int neuronId = Random.Range(0, child1.Network.layers[layerId].layerSize);

            for (int j = 1; j < numOfLayers - 1; j++)
            {
                var bias1 = parent1.Network.layers[j].biases.values;
                var bias2 = parent2.Network.layers[j].biases.values;
                var weights1 = parent1.Network.layers[j].weights.values;
                var weights2 = parent2.Network.layers[j].weights.values;

                var childBias1 = new float[bias1.Length];
                var childBias2 = new float[bias1.Length];
                
                bias1.CopyTo(childBias1, 0);
                bias2.CopyTo(childBias2, 0);
                var childWeights1 = weights1.Clone() as float[,];
                var childWeights2 = weights2.Clone() as float[,];
                
                child1.Network.layers[j].biases.values = childBias1;
                child2.Network.layers[j].biases.values = childBias2;
                child1.Network.layers[j].weights.values = childWeights1;
                child2.Network.layers[j].weights.values = childWeights2;
            }
            for (int j = 0; j < child1.Network.layers[layerId].weights.values.GetLength(0); j++)
            {
                child1.Network.layers[layerId].weights.values[j, neuronId] = parent2.Network.layers[layerId].weights.values[j, neuronId];
                child2.Network.layers[layerId].weights.values[j, neuronId] = parent1.Network.layers[layerId].weights.values[j, neuronId];
            }

            child1.Network.layers[layerId].biases.values[neuronId] = parent2.Network.layers[layerId].biases.values[neuronId];
            child2.Network.layers[layerId].biases.values[neuronId] = parent1.Network.layers[layerId].biases.values[neuronId];

            results.Add(new Genome(child1));
            results.Add(new Genome(child2));
        }
        return results;
    }

    public List<Genome> GetRecombinations(List<Genome> parents, int numOfChildren)
    {
        List<Genome> results = new List<Genome>();
        for(int i = 0; i < numOfChildren; i++)
        {
            var child = new NeuralNetwork(26 * 2);
            var parent1Index = Random.Range(0, parents.Count);
            var parent2Index = Random.Range(0, parents.Count);
            while(parent1Index == parent2Index)
                parent2Index = Random.Range(0, parents.Count);
            var parent1 = parents[parent1Index].Network;
            var parent2 = parents[parent2Index].Network;
            int numOfLayers = child.Network.LayerCount();
            for(int j = 0; j < numOfLayers; j++)
            {
                float randomChoice = Random.Range(0f, 1f);
                if(randomChoice > 0.5f) {
                    child.Network.layers[j].weights = parent1.Network.layers[j].weights;
                    child.Network.layers[j].biases  = parent2.Network.layers[j].biases;
                } else {
                    child.Network.layers[j].weights = parent2.Network.layers[j].weights;
                    child.Network.layers[j].biases  = parent1.Network.layers[j].biases;
                }
            }
            var genome = new Genome(child);
            results.Add(genome);
        }
        return results;
    }

    public List<Genome> Recombine(List<Genome> parents, int numOfChildren)
    {
        List<Genome> results = new List<Genome>();
        for (int i = 0; i < numOfChildren; i++)
        {
            var child = new NeuralNetwork(26 * 2);
            var parent1Index = Random.Range(0, parents.Count);
            var parent2Index = Random.Range(0, parents.Count);
            while (parent1Index == parent2Index)
                parent2Index = Random.Range(0, parents.Count);
            var parent1 = parents[parent1Index].Network;
            var parent2 = parents[parent2Index].Network;
            int numOfLayers = child.Network.LayerCount();
            for (int j = 0; j < numOfLayers; j++)
            {
                if (parent1.Network.layers[j].biases != null)
                {
                    var bias1 = parent1.Network.layers[j].biases.values;
                    var bias2 = parent2.Network.layers[j].biases.values;
                    var halfSize = Mathf.CeilToInt(bias1.Length/2);

                    var childBias = new float[bias1.Length];
                    bias1.CopyTo(childBias, 0);
                    for(int z = halfSize; z < bias1.Length; z++)
                    {
                        childBias[z] = bias2[z];
                    }
                    child.Network.layers[j].biases.values = childBias;
                }
                if (parent1.Network.layers[j].weights != null)
                {
                    var weights1 = parent1.Network.layers[j].weights.values;
                    var weights2 = parent2.Network.layers[j].weights.values;

                    var halfSize = Mathf.CeilToInt(weights1.Length);

                    var childWeights = new float[weights1.GetLength(0), weights1.GetLength(1)];

                    for (int k = 0; k < weights1.GetLength(0); k++)
                    {
                        for (int l = 0; l < weights1.GetLength(1); l++)
                        {
                            childWeights[k, l] = k < halfSize ? weights1[k, l] : weights2[k, l];
                        }
                    }
                }
            }
            results.Add(new Genome(child));
        }
        return results;
    }
}
