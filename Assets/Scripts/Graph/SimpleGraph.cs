using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SimpleGraph
{
    private Dictionary<Vector3, int> posToId;
    private Dictionary<Vector3, HashSet<int>> posToConnections;
    private readonly float gridDistance;
    private Transform structure;
    private GameObject graphParent;
    private readonly int ID;
    private UnityGraph unityGraph;

    private int vertexCount = 0;
    public SimpleGraph(float gridDistance, int id)
    {
        ID = id;
        structure = GameObject.Find("Structure").transform;
        this.gridDistance = gridDistance;
        posToId = new Dictionary<Vector3, int>();
        posToConnections = new Dictionary<Vector3, HashSet<int>>();
        posToId.Add(Vector3.zero, vertexCount++);
        posToConnections.Add(Vector3.zero, new HashSet<int>());
    }

    public IEnumerable<Vector3> GetLocalPositions()
    {
        var array = new Vector3[posToId.Keys.Count];
        posToId.Keys.CopyTo(array, 0);
        return array;
    }

    public bool LocalExists(Vector3 pos)
    {
        return posToId.TryGetValue(pos, out int _);
    }

    public float[] GetLocal(Vector3 pos)
    {
        var local = new float[26 * 2];
        if (!posToId.TryGetValue(pos, out int localId))
            throw new IndexOutOfRangeException("Pos doesn't have a vertex");
        var counter = 0;
        foreach(var adjacent in GridPositions(pos))
        {
            if (posToId.ContainsKey(adjacent))
            {
                local[counter] = 1;
                if (!posToConnections.TryGetValue(adjacent, out HashSet<int> connectionIds))
                    throw new IndexOutOfRangeException("Pos doesn't have connections");
                local[counter + 1] = connectionIds.Contains(localId) ? 1 : 0;
            }
            counter += 2;
        }
        return local;
    }

    public void UpdateFromLocal(float[] local, Vector3 pos)
    {
        if(!posToId.TryGetValue(pos, out int localId))
            throw new IndexOutOfRangeException("Pos doesn't have a vertex");
        if(!posToConnections.TryGetValue(pos, out HashSet<int> connections))
            throw new IndexOutOfRangeException("Pos doesn't have connections");
        var counter = 0;
        foreach (var adjacent in GridPositions(pos))
        {
            var exists = posToId.TryGetValue(adjacent, out int adjacentId);
            if(local[counter] >= 0.5)
            {
                if(!exists)
                {
                    adjacentId = vertexCount++;
                    posToId.Add(adjacent, adjacentId);
                    var cs= new HashSet<int>
                    {
                        localId
                    };
                    posToConnections.Add(adjacent, cs);
                }
                else
                {
                    if(!posToConnections.TryGetValue(adjacent, out HashSet<int> cs))
                        throw new IndexOutOfRangeException("Pos doesn't have connections");
                    cs.Add(localId);
                }
                connections.Add(adjacentId);
            }
            else
            {
                if(exists)
                {
                    if (!posToConnections.TryGetValue(adjacent, out HashSet<int> cs))
                        throw new IndexOutOfRangeException("Pos doesn't have connections");
                    cs.Remove(localId);
                    connections.Remove(adjacentId);
                    if (cs.Count == 0)
                    {
                        posToConnections.Remove(adjacent);
                        posToId.Remove(adjacent);
                    }
                }
            }
            counter++;
        }
        if(connections.Count == 0)
        {
            posToConnections.Remove(pos);
            posToId.Remove(pos);
        }
    }

    public UnityGraph GetUnityGraph()
    {
        graphParent = new GameObject("graphParent " + ID);
        graphParent.transform.parent = structure;
        graphParent.SetActive(false);
        Dictionary<int, Vertex> vertices = new Dictionary<int, Vertex>();
        List<Edge> edges = new List<Edge>();
        var spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<Spawner>();

        foreach (var (pos, id) in posToId)
        {
            vertices[id] = spawner.GetVertex(pos, graphParent);
        }

        var usedIds = new HashSet<int>();
        foreach (var (pos, connections) in posToConnections)
        {
            if(!posToId.TryGetValue(pos, out int usedId))
                throw new IndexOutOfRangeException("Pos doesn't have a vertex");
            usedIds.Add(usedId);

            var from = vertices[usedId];
            foreach(var id in connections)
            {
                if (usedIds.Contains(id))
                    continue;
                var to = vertices[id];
                var edge = spawner.GetEdge(from, to, graphParent);
                edges.Add(edge);
            }
        }
        
        unityGraph = new UnityGraph(graphParent, vertices.Values.ToList(), edges);
        return unityGraph;
    }

    public Vector3[] GridPositions(Vector3 center)
    {
        var result = new Vector3[26];

        // Behind layer
        result[0] = new Vector3(center.x - gridDistance, center.y - gridDistance, center.z - gridDistance);
        result[1] = new Vector3(center.x - gridDistance, center.y, center.z - gridDistance);
        result[2] = new Vector3(center.x - gridDistance, center.y + gridDistance, center.z - gridDistance);
        result[3] = new Vector3(center.x, center.y - gridDistance, center.z - gridDistance);
        result[4] = new Vector3(center.x, center.y, center.z - gridDistance);
        result[5] = new Vector3(center.x, center.y + gridDistance, center.z - gridDistance);
        result[6] = new Vector3(center.x + gridDistance, center.y - gridDistance, center.z - gridDistance);
        result[7] = new Vector3(center.x + gridDistance, center.y, center.z - gridDistance);
        result[8] = new Vector3(center.x + gridDistance, center.y + gridDistance, center.z - gridDistance);

        // Middle layer
        result[9] = new Vector3(center.x - gridDistance, center.y - gridDistance, center.z);
        result[10] = new Vector3(center.x - gridDistance, center.y, center.z);
        result[11] = new Vector3(center.x - gridDistance, center.y + gridDistance, center.z);
        result[12] = new Vector3(center.x, center.y - gridDistance, center.z);
        //result[13] = new Vector3(center.x, center.y, center.z);
        result[13] = new Vector3(center.x, center.y + gridDistance, center.z);
        result[14] = new Vector3(center.x + gridDistance, center.y - gridDistance, center.z);
        result[15] = new Vector3(center.x + gridDistance, center.y, center.z);
        result[16] = new Vector3(center.x + gridDistance, center.y + gridDistance, center.z);

        // Front layer
        result[17] = new Vector3(center.x - gridDistance, center.y - gridDistance, center.z + gridDistance);
        result[18] = new Vector3(center.x - gridDistance, center.y, center.z + gridDistance);
        result[19] = new Vector3(center.x - gridDistance, center.y + gridDistance, center.z + gridDistance);
        result[20] = new Vector3(center.x, center.y - gridDistance, center.z + gridDistance);
        result[21] = new Vector3(center.x, center.y, center.z + gridDistance);
        result[22] = new Vector3(center.x, center.y + gridDistance, center.z + gridDistance);
        result[23] = new Vector3(center.x + gridDistance, center.y - gridDistance, center.z + gridDistance);
        result[24] = new Vector3(center.x + gridDistance, center.y, center.z + gridDistance);
        result[25] = new Vector3(center.x + gridDistance, center.y + gridDistance, center.z + gridDistance);

        return result;
    }

    public void Destroy()
    {
        var spawner = GameObject.FindGameObjectWithTag("Spawner").GetComponent<Spawner>();
        foreach(Vertex vertex in unityGraph.Vertices)
        {
            spawner.ReleaseVertex(vertex.gameObject);
        }

        foreach(Edge edge in unityGraph.Edges)
        {
            spawner.ReleaseEdge(edge.gameObject);
        }

        graphParent.name = "Trash";
        graphParent.tag = "Trash";
    }
}

