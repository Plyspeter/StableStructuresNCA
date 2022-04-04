using UnityEngine;
using UnityEngine.Pool;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    private GameObject vertexPrefab;

    [SerializeField]
    private GameObject edgePrefab;

    private ObjectPool<GameObject> vertexPool;
    private ObjectPool<GameObject> edgePool;

    void Start()
    {
        vertexPool = new ObjectPool<GameObject>(
            () => { return Instantiate(vertexPrefab); },
            obj => { obj.SetActive(true); },
            obj => { obj.SetActive(false); },
            obj => { Destroy(obj); },
            false, 4000, 6000);

        edgePool = new ObjectPool<GameObject>(
            () => { return Instantiate(edgePrefab); },
            obj => { obj.SetActive(true); },
            obj => { obj.SetActive(false); },
            obj => { Destroy(obj); },
            false, 8000, 10000);
    }

    public Vertex GetVertex(Vector3 pos, GameObject parent)
    {
        GameObject vertex = vertexPool.Get();
        vertex.GetComponent<Rigidbody>().velocity = Vector3.zero;
        vertex.transform.rotation = Quaternion.identity;
        return Vertex.Init(vertex, pos, parent);
    }

    public void ReleaseVertex(GameObject vertex)
    {
        vertexPool.Release(vertex);
    }

    public Edge GetEdge(Vertex from, Vertex to, GameObject parent)
    {
        GameObject edge = edgePool.Get();
        edge.GetComponent<Rigidbody>().velocity = Vector3.zero;
        return Edge.Init(edge, from, to, parent, edgePrefab.GetComponent<FixedJoint>().breakForce);
    }

    public void ReleaseEdge(GameObject edge)
    {
        edgePool.Release(edge);
    }
}
