using UnityEngine;
using System.Linq;

public class Edge : MonoBehaviour
{
    public int NumBrokenJoints { get; private set; }
    public Vertex From { get; private set;}
    public Vertex To { get; private set; }
        
    public static Edge Init(GameObject obj, Vertex from, Vertex to, GameObject parent, float breakForce)
    {
        var edge = obj.GetComponent<Edge>();
        edge.transform.parent = parent.transform;
        edge._Init(from, to, breakForce);
        return edge;
    }

    private void _Init(Vertex from, Vertex to, float breakForce)
    {
        NumBrokenJoints = 0;
        From = from;
        To = to;
        transform.up = from.transform.position - to.transform.position;
        transform.position = Vector3.Lerp(from.transform.position, to.transform.position, 0.5f);
        transform.localScale = new Vector3(transform.localScale.x, (from.transform.position - to.transform.position).magnitude / 2 - (Vertex.GetDiameter() / 2 - 0.01f), transform.localScale.z);
        var direction = from.transform.position - to.transform.position;
        from.AddEdge(direction, this);
        to.AddEdge(-direction, this);

        var joints = GetComponents<FixedJoint>().ToList();

        for (int i = joints.Count; i < 2; i++)
        {
            var joint = gameObject.AddComponent<FixedJoint>();
            joint.breakForce = breakForce;
            joints.Add(joint);
        }
            
        joints[0].connectedBody = from.gameObject.GetComponent<Rigidbody>();
        joints[1].connectedBody = to.gameObject.GetComponent<Rigidbody>();
    }

    public void OnJointBreak(float breakForce)
    {
        NumBrokenJoints++;
    }
}
