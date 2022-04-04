using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genome
{
    public NeuralNetwork Network { get; private set; }
    public SimpleGraph Graph { get; private set; }

    private static int counter = 0;
    public int Id { get; private set; }

    public Genome(NeuralNetwork network)
    {
        Id = counter++;
        Network = network;
        Graph = new SimpleGraph(Vertex.GetDiameter() * 3, Id);
    }

}
