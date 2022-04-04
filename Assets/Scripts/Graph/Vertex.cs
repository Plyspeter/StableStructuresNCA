using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Vertex : MonoBehaviour
{
    private static float diameter = -1;
    private Dictionary<Vector3, Edge> dirToConnection = new Dictionary<Vector3, Edge>();

    public static Vertex Init(GameObject obj, Vector3 pos, GameObject parent)
    {
        var vertex = obj.GetComponent<Vertex>();
        vertex.transform.parent = parent.transform;
        vertex.SetDiameter();
        vertex._Init(pos);
        return vertex;
    }

    private void _Init(Vector3 pos)
    {
        transform.position = pos;
        dirToConnection.Clear();
    }

    public static float GetDiameter()
    {
        if (diameter == -1)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Vertex");
            diameter = prefab.transform.localScale.x * prefab.GetComponent<SphereCollider>().radius * 2;
        }
        return diameter;
    }

    private void SetDiameter()
    {
        if (diameter == -1)
            diameter = transform.localScale.x * GetComponent<SphereCollider>().radius * 2;
    }



    public bool IsDirUsed(Vector3 dir)
    {
        foreach (var usedDir in dirToConnection.Keys)
        {
            if (Vector3.Angle(dir, usedDir) < 15)
                return true;
        }
        return false;
    }

    public bool IsEdgeConnected(Edge e)
    {
        return dirToConnection.ContainsValue(e);
    }

    public void AddEdge(Vector3 dir, Edge e)
    {
        if (!IsDirUsed(dir) && !IsEdgeConnected(e))
        {
            dirToConnection.Add(dir, e);
        }
        else
        {
            if (IsDirUsed(dir))
            {
                Debug.LogError("Illegal vertex operation: Direction is already used");
                Debug.Log("Vertex at: " + transform.position + " tried to add a vertex in dir: " + dir);
                foreach (var x in dirToConnection.Keys)
                {
                    Debug.Log(x);
                }
            }
            if (IsEdgeConnected(e))
            {
                Debug.LogError("Illegal vertex operation: Edge is already connected");
            }
        }
    }

    public List<Edge> GetEdges()
    {
        return dirToConnection.Values.ToList();
    }

    public void RemoveEdge(Edge e)
    {
        dirToConnection.Remove(dirToConnection.FirstOrDefault(x => x.Value == e).Key);
    }

    public bool IsConnected(Vertex vertex)
    {
        foreach (Edge e in GetEdges())
        {
            if (vertex.GetEdges().Contains(e))
                return true;
        }
        return false;
    }
}
