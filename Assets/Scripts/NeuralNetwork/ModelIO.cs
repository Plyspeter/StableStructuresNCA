using UnityEngine;

public class ModelIO : MonoBehaviour
{
    [SerializeField]
    private Evolution evolution;

    [SerializeField]
    private bool saveBestModel = false;

    [SerializeField]
    private bool loadModel = false;

    [SerializeField]
    private string loadModelName = string.Empty;

    public bool HasLoadedModel { get { return LoadedModel != null; } }

    public Genome LoadedModel { get; private set; }

    void Update()
    {
        if (saveBestModel)
            SaveModel();

        if (loadModel) 
            LoadModel();
    }

    private void SaveModel()
    {
        saveBestModel = false;
        if (evolution.CurrentBest != null)
            evolution.CurrentBest.Network.SaveModel();
        else
            Debug.LogError("No best model could be found");
    }

    private void LoadModel()
    {
        if (!string.IsNullOrEmpty(loadModelName))
        {
            loadModel = false;
            var network = new NeuralNetwork(26 * 2);
            if (network.LoadModel(loadModelName))
                LoadedModel = new Genome(network);
            else
                Debug.LogError("Failed loading the model");
        }
        else
        {
            Debug.LogError("Tried to load model with no name!");
        }
    }
}
