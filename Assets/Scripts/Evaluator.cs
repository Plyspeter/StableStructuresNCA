using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class Evaluator : MonoBehaviour
{
    [SerializeField]
    [Min(0)]
    private float heightWeight = 0, complexityWeight = 0;
    [SerializeField]
    private int simulationLength = 5;
    private const float maxHeight = 100;
    private Queue<Evaluation> evaluationQueue;
    private Evaluation currentEvaluation;
    private GameObject plane;

    private void Start()
    {
        plane = GameObject.Find("TestPlane");
        evaluationQueue = new Queue<Evaluation>();
    }

    void Update()
    {
        if (GameManager.pause)
            return;
        if ((currentEvaluation?.Done ?? true) && evaluationQueue.Count > 0)
        {
            currentEvaluation = evaluationQueue.Dequeue();
            StartCoroutine(currentEvaluation.Run());
        }
    }
    public void Evaluate(UnityGraph unityGraph, NeuralNetwork nn, int id)
    {
        nn.Evaluating = true;
        evaluationQueue.Enqueue(new Evaluation((eval) => _Evaluate(unityGraph, nn, eval, id)));
    }

    private IEnumerator _Evaluate(UnityGraph unityGraph, NeuralNetwork nn, Evaluation eval, int id)
    {
        if (unityGraph.Vertices.Count > 0)
        {
            eval.Complexity = CalculateComplexity(unityGraph);
            yield return null;
            yield return CalculateIntegrity(unityGraph, CalculateMinimum(unityGraph), eval);
            yield return null;
            eval.Height = CalculateHeight(unityGraph);
            yield return null;

            nn.Q = (heightWeight * eval.Height + complexityWeight * eval.Complexity) * eval.Integrity + eval.Integrity;
            print("Evaluation for " + id + " was " + nn.Q + " height: " + eval.Height + " complexity" + eval.Complexity + " integrity: " + eval.Integrity);
        }
        else
        {
            nn.Q = 0;
            
        }
        nn.Evaluating = false;

        yield return null;
    }

    public float CalculateMinimum(UnityGraph unityGraph)
    {
        float min = float.MaxValue;
        foreach (Vertex v in unityGraph.Vertices)
        {
            float yPos = v.gameObject.transform.position.y;
            if (yPos < min)
                min = yPos;
        }
        return min;
    }

    private float CalculateHeight(UnityGraph unityGraph)
    {
        float max = float.MinValue;
        float min = float.MaxValue;
        foreach (Vertex v in unityGraph.Vertices)
        {
            float yPos = v.gameObject.transform.position.y;
            if (yPos < min)
                min = yPos;
            if (yPos > max)
                max = yPos;
        }
        return (max - min) / maxHeight;
    }

    private float CalculateAverageHeight(UnityGraph unityGraph)
    {
        float max = 0;
        foreach (Vertex v in unityGraph.Vertices)
        {
            max += v.gameObject.transform.position.y;

        }
        
        return (max / unityGraph.Vertices.Count - CalculateMinimum(unityGraph)) / maxHeight;
    }

    private float CalculateComplexity(UnityGraph unityGraph)
    {
        return unityGraph.Vertices.Count / 10000f;
    }

    private IEnumerator CalculateIntegrity(UnityGraph unityGraph, float min, Evaluation eval)
    {
        plane.transform.position = new Vector3(0, min - (Vertex.GetDiameter() / 2.0f), 0);
        unityGraph.SetActive(true);
        GameManager.ToggleGravity(true);
        yield return new WaitForSeconds(simulationLength);
        GameManager.ToggleGravity(false);
        unityGraph.SetActive(false);
        yield return null;
        var totalBrokenJoints = 0;
        foreach (var edge in unityGraph.Edges)
        {
            totalBrokenJoints += edge.NumBrokenJoints;
            yield return null;
        }
        eval.Integrity = (unityGraph.Edges.Count * 2 - totalBrokenJoints) / (float) unityGraph.Edges.Count * 2;
        yield return null;
    }

    private class Evaluation
    {
        private Func<IEnumerator> f;
        public bool Done { get; private set; }

        public float Integrity { get; set; }
        public float Complexity { get; internal set; }
        public float Height { get; internal set; }

        public Evaluation(Func<Evaluation, IEnumerator> function)
        {
            Done = false;
            f = (() => Func(function));
        }

        public IEnumerator Run()
        {
            yield return f.Invoke();
        }

        private IEnumerator Func(Func<Evaluation, IEnumerator> function)
        {
            yield return function.Invoke(this);
            Done = true;
            yield return null;
        }
            
    }
}
