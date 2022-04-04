using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;


public class StructureTester : MonoBehaviour
{

    [SerializeField]
    [Min(1)]
    private int height = 1;
    
    [SerializeField]
    private bool gravity = true;
    // Start is called before the first frame update
    void Start()
    {
        //CreateHalfCube();
        //CreateCube();
        CreateTower();    
    }

    private void CreateTower()
    {
        var graph = new SimpleGraph(Vertex.GetDiameter() * 3, 2);
        var currentPos = Vector3.zero;
        float[] local = new float[26];
        for (int i = 1; i < height; i++)
        {
            local = new float[26];
            local[13] = 1;
            if (i > 1)
                local[12] = 1;
            graph.UpdateFromLocal(local, currentPos);
            var x = graph.GetLocalPositions().ToList();
            currentPos = graph.GetLocalPositions().ToList()[i];
        }
        local = new float[26];
        currentPos = graph.GetLocalPositions().ToList()[1];
        local[3]  = 1;
        local[9]  = 1;
        local[14] = 1;
        local[20] = 1; 
        local[13] = 1;
        local[12] = 1;
        graph.UpdateFromLocal(local, currentPos);
        graph.GetUnityGraph().SetActive(true);
    }

    private void CreateCube()
    {
        var graph = new SimpleGraph(Vertex.GetDiameter() * 3, 41941);
        float[] local = new float[26];
        local[13] = 1;
        local[21] = 1;
        local[15] = 1;
        graph.UpdateFromLocal(local, Vector3.zero);
        var localPositions = graph.GetLocalPositions().ToList();
        local = new float[26];
        local[21] = 1;
        local[15] = 1;
        local[12] = 1;
        graph.UpdateFromLocal(local, localPositions[1]);
        local = new float[26];
        local[15] = 1;
        local[13] = 1;
        local[4] = 1;
        graph.UpdateFromLocal(local, localPositions[3]);
        local = new float[26];
        local[21] = 1;
        local[13] = 1;
        local[10] = 1;
        graph.UpdateFromLocal(local, localPositions[2]);
        localPositions = graph.GetLocalPositions().ToList();
        local = new float[26];
        local[13] = 1;
        local[4] = 1;
        local[10] = 1;
        graph.UpdateFromLocal(local, localPositions[6]);
        localPositions = graph.GetLocalPositions().ToList();
        local = new float[26];
        local[12] = 1;
        local[4] = 1;
        local[10] = 1;
        graph.UpdateFromLocal(local, localPositions[7]);
        graph.GetUnityGraph().SetActive(true);
    }

    private void CreateHalfCube()
    {
        var graph = new SimpleGraph(Vertex.GetDiameter() * 3, 41941);
        float[] local = new float[26];
        local[13] = 1;
        local[21] = 1;
        local[15] = 1;
        graph.UpdateFromLocal(local, Vector3.zero);
        var localPositions = graph.GetLocalPositions().ToList();
        local = new float[26];
        local[21] = 1;
        local[15] = 1;
        local[12] = 1;
        graph.UpdateFromLocal(local, localPositions[1]);

        graph.GetUnityGraph().SetActive(true);

    }

    // Update is called once per frame
    void Update()
    {
        Physics.gravity = gravity ? new Vector3(0, -9.84f, 0) : Vector3.zero;
    }
}
