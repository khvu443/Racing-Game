using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackingSystem : MonoBehaviour
{
    public static TrackingSystem Instance;

    [Header("Line Connect Color")]
    [SerializeField] Color LineColor;

    [Header("Radius point")]
    [Range(0, 1)]
    [SerializeField] float radiusSphere;

    [Header("Points")]
    public List<Transform> nodes = new List<Transform>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = LineColor;

        //Get all nodes
        Transform[] path = GetComponentsInChildren<Transform>();
        
        nodes = new List<Transform>();
        for(int i =1; i < path.Length; i++)
        {
           nodes.Add(path[i]);
        }

        // it's working almost like double link list circular
        for(int i = 0; i < nodes.Count; i ++)
        {
            //the first node
            Vector3 curr = nodes[i].position;
            Vector3 pre = Vector3.zero;

            //making connect to all node
            if (i != 0) pre = nodes[i - 1].position;
            // making a circle
            else if (i==0) pre = nodes[nodes.Count - 1].position;

            Gizmos.DrawLine(pre, curr);
            Gizmos.DrawSphere(curr, radiusSphere);
        }
    }
}
