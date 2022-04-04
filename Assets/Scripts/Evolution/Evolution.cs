using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Evolution : MonoBehaviour
{
    public Genome CurrentBest { get; private set; }

    [SerializeField]
    [Min(2)]
    private int population = 10;
    public int Population { get { return population; } }

    [SerializeField]
    [Min(2)]
    private int numberOfParents = 2;

    [SerializeField]
    [Min(1)]
    private int numberOfChildren = 4;

    [SerializeField]
    [Min(0)]
    private int numberOfMutations;

    [SerializeField]
    [Range(0, 1)]
    private float biasMutationProc = 0.1f;

    [SerializeField]
    [Range(0, 0.25f)]
    private float biasMutationRange = 0.05f;

    [SerializeField]
    [Range(0, 1)]
    private float weightMutationProc = 0.02f;

    [SerializeField]
    [Range(0, 0.25f)]
    private float weightMutationRange = 0.05f;

    private Recombinator recombinator = new Recombinator();
    private Mutator mutator = new Mutator();

    public int EvolutionNumber { get; private set; }

    private void Awake()
    {
        if (numberOfParents + numberOfChildren + numberOfMutations > population)
        {
            Debug.LogError("Population number is too low!");
            population = numberOfParents + numberOfChildren + numberOfMutations;
        }
            
    }

    public Genome[] Evolve(Genome[] genomes, int inputSize)
    {
        int totalNetworks = population;

        var results = new List<Genome>();

        var parents = SelectParents(genomes);
        
        CurrentBest = parents[0];

        Debug.Log("Best NCA: " + CurrentBest.Id);

        results.AddRange(parents);
        totalNetworks -= parents.Count;

        var recombinations = recombinator.SingleNeuronRecombine(parents, numberOfChildren);
        results.AddRange(recombinations);
        totalNetworks -= recombinations.Count;
        
        var mvars = new MutationVariables()
        {
            numberOfMutations = numberOfMutations,
            biasMutationProc = biasMutationProc,
            biasMutationRange = biasMutationRange,
            weightMutationProc = weightMutationProc,
            weightMutationRange = weightMutationRange
        };
        var mutations = mutator.Mutate(parents, mvars);
        results.AddRange(mutations);
        totalNetworks -= mutations.Count;

        if (totalNetworks > 0)
        {
            var random = GetRandomNetworks(totalNetworks, inputSize);
            results.AddRange(random);
        }

        EvolutionNumber++;

        return results.ToArray();
    }

    private List<Genome> SelectParents(Genome[] networks)
    {
        var ordered =  networks.OrderByDescending(e => e.Network.Q).ToList();
        var parents = ordered.GetRange(0, numberOfParents);
        ordered.GetRange(numberOfParents, ordered.Count - numberOfParents).ForEach(e => e.Graph.Destroy());
        return parents;
        
    }

    private Genome[] GetRandomNetworks(int count, int inputSize)
    {
        var result = new Genome[count];

        for(int i = 0; i < count; i++)
        {
            result[i] = new Genome(new NeuralNetwork(inputSize));
        }

        return result;
    }
}
