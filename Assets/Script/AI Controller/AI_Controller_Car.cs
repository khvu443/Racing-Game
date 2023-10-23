using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class AI_Controller_Car : Vehicle
{
    [Header("Tracking Point System")]
    public TrackingSystem tracking;
    public List<Transform> nodes = new List<Transform>();
    public Transform currPoint; //the node current point

    [Header("Other")]
    [Range(0, 10)][SerializeField] int offset; //skip node when tracking
    [Range(0, 10)] [SerializeField] int steeringForce;


    private void Awake()
    {
        tracking = GameObject.FindGameObjectWithTag("Path").GetComponent<TrackingSystem>(); //get list node of tracking
        nodes = tracking.nodes; //set all node
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        //Set default value of car setting
        maxRpm = 10000;
        breakingForce = 20000f;
        currPoint = nodes[1];

        gears = new float[5];
        gears[0] = 2;
        gears[1] = 1.7f;
        gears[2] = 1.3f;
        gears[3] = 0.9f;
        gears[4] = 0.4f;

        base.Start();

        //For flag to avoid some function using only player
        // Like input
        typeControll = TypeControll.AI;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        SteeringCarAI();
        CalculatorDistancePath();
        AiDriver();
    }

    void CalculatorDistancePath()
    {
        Vector3 pos = gameObject.transform.position;
        float dis = Mathf.Infinity;
        for(int i = 0; i < nodes.Count; i++)
        { 
            //find the diff between two vecto
            // node and the ai pos
            Vector3 diff = nodes[i].position - pos;
            // find the length of diff
            float currDistance = diff.magnitude;
            if(currDistance < dis)
            {
                //when index of node plus the offset is not GT than the length of nodes
                if(!((i+offset) >= nodes.Count))
                {
                    //set the position of current node point to is = index of the node + offset
                    currPoint = nodes[i + offset];
                }
                // else reset the index = 0 to avoid bond of array
                else
                {
                    i = 0;
                    currPoint = nodes[i + offset];
                }
                dis = currDistance;

            }
        }
    }

    // draw the sphere in gizmos mode in unity editor
    // check every thing is working right
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(currPoint.position, 3);
    }

    //Function to set vertical and call steering function
    // 1f -> mean forward
    private void AiDriver()
    {
        vertical = 1f;
        SteeringCar();
    }

    //to auto set horizontal -> for steering
    void SteeringCarAI()
    {
        // convert from local position to world space position
        Vector3 relative = transform.InverseTransformPoint(currPoint.position);
        relative /= relative.magnitude;

        horizontal = (relative.x/relative.magnitude) * steeringForce;

    }

}
