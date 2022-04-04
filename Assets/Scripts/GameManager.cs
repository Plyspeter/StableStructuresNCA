using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Vector3 gravity = new Vector3(0, -9.84f, 0);
    [SerializeField]
    private static bool gravityToggle = false;
    [SerializeField]
    private GameObject vertexPrefab;
    [SerializeField]
    private GameObject edgePrefab;
    [SerializeField]
    private Evaluator evaluator;
    [SerializeField]
    private ModelIO modelIO;

    [SerializeField]
    private bool showLoadedModel = false;

    private Genome[] genomes;

    private const int NETWORKINPUTSIZE = 26 * 2;

    [SerializeField]
    [Min(0)]
    private int maxIterations = 0;

    [SerializeField]
    private Evolution evolution;

    [SerializeField]
    [Min(0)]
    private float timeScale;

    public static bool pause = false;

    [SerializeField]
    private bool _pause = false;
    void Start()
    {
        Time.timeScale = timeScale;
        Physics.IgnoreLayerCollision(edgePrefab.layer, vertexPrefab.layer);
        Physics.IgnoreLayerCollision(edgePrefab.layer, edgePrefab.layer);
        Physics.IgnoreLayerCollision(vertexPrefab.layer, vertexPrefab.layer);

        genomes = new Genome[evolution.Population];
        for(int i = 0; i < evolution.Population; i++)
        {
            genomes[i] = new Genome(new NeuralNetwork(NETWORKINPUTSIZE));   
            StartCoroutine(genomes[i].Network.Run(genomes[i].Graph, maxIterations));
        }
    }

    void Update()
    {
        pause = _pause;
        Physics.gravity = gravityToggle ? gravity : Vector3.zero;
        if (pause)
            return;
        bool allDone = true;
        for(int i = 0; i < genomes.Length; i++)
        {
            if (genomes[i].Network.Q == -1 && genomes[i].Network.RunningDone && !genomes[i].Network.Evaluating)
            {
                var unityGraph = genomes[i].Graph.GetUnityGraph();
                evaluator.Evaluate(unityGraph, genomes[i].Network, genomes[i].Id);
            }
            allDone &= genomes[i].Network.Q != -1 && genomes[i].Network.RunningDone;
        }
        if(allDone)
        {
            genomes = evolution.Evolve(genomes, NETWORKINPUTSIZE);

            var structure = GameObject.Find("Structure");
            var trash = structure.gameObject.GetComponentsInChildren<Transform>(true);

            for(int i = 0; i < trash.Length; i++)
            {
                if (trash[i].childCount == 0 && trash[i].CompareTag("Trash"))
                    Destroy(trash[i].gameObject);
            }

            for(int i = 0; i < genomes.Length; i++)
            {
                if (!genomes[i].Network.RunningDone)
                {
                    StartCoroutine(genomes[i].Network.Run(genomes[i].Graph, maxIterations));
                }
            }
        }

         
    }

    public static void ToggleGravity(bool f)
    {
        gravityToggle = f;
    }
}
