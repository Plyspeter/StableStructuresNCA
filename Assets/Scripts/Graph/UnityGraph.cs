using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UnityGraph
{
    public GameObject parent;
    public List<Vertex> Vertices { get; private set; }
    public List<Edge> Edges { get; private set; }

    public UnityGraph(GameObject parent, List<Vertex> vertices, List<Edge> edges)
    {
        this.parent = parent;
        Vertices = vertices;
        Edges = edges;
    }

    public void SetRender(bool f)
    {
        var meshes = parent.GetComponentsInChildren<MeshRenderer>();
        foreach(var renderer in meshes)
        {
            renderer.enabled = f;
        }
    }

    public void SetActive(bool f)
    {
        parent.SetActive(f);
    }
}
