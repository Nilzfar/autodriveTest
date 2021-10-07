using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class CarEngine : MonoBehaviour
{
    public Transform path;
    public float maxSteerAngle = 45f;
    public float maxTorque = 1500f;
    public float currentSpeed;
    public float maxSpeed = 500f;
    public bool convex = false;
    private List<Transform> nodes;
    public int currentNode = 0;
    public WheelCollider FL;
    public WheelCollider FR;
    public WheelCollider RL, RR;
    public float brakeTorque=0;
    private float breakDistance;
    public float maxBrake;
    public GameObject nextCar;
    public int instantiateNode;
    private float nextCarDistance =20f;


    // Start is called before the first frame update
    public void Start()
    {

        //Get all nodes
        maxBrake = 3 * maxTorque;
        var pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        //All nodes that are not the top node
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if(pathTransforms[i] != transform && pathTransforms[i].transform != path.transform)
            {
                nodes.Add(pathTransforms[i]);            }
        };
        //        FL = GameObject.Find("Suv/WPF/WS1").GetComponent<WheelCollider>();
        //      FR = GameObject.Find("Suv/WDF/WS1").GetComponent<WheelCollider>();
        findNode();
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position);
        transform.rotation = Quaternion.LookRotation(relativeVector);
        
    }
    private void calcDist()
    {
        nextCarDistance = currentSpeed * 5f;
        if(nextCarDistance <= 15f)
        {
            nextCarDistance = 15f;
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        calcDist();
        ApplySteer();
        UpdateNode();
        Drive();
        Braking();
    }

    private void Braking()
    {
        if (path.GetComponent<Path>().speeds[currentNode] <= currentSpeed || (nextCar != null && Vector3.Distance(transform.position, nextCar.transform.position) < nextCarDistance))
        {
            brakeTorque = maxBrake;
        }
        else
        {
            brakeTorque = 0f;
        }
        if (brakeTorque > 0f)
        {
            RL.brakeTorque = brakeTorque;
            RR.brakeTorque = brakeTorque;
        }
        else
        {
            RL.brakeTorque = 0;
            RR.brakeTorque = 0;
        }
    }
    private void findNode()
    {
        float tmpMin = 50f;
        for (int i = 0; i < nodes.Count; i++)
        {
            float curDis = Vector3.Distance(nodes[i].position, transform.position);
            if(curDis < tmpMin)
            {
                tmpMin = curDis;
                currentNode = i+1;
                currentNode = currentNode % nodes.Count;
            }
        }
    }
    private void UpdateNode()
    {
        if (Vector3.Distance(nodes[currentNode].position, transform.position) < 3f)
        {
            if(currentNode == nodes.Count - 1)
            {
                if(convex == false)
                {
                    Destroy(gameObject);
                }
                currentNode = 0;
            }
            else
            {
                currentNode++;
            }
        }
    }
    private void Drive()
    {
        currentSpeed = GetComponent<Rigidbody>().velocity.magnitude;
        if(currentSpeed < path.GetComponent<Path>().speeds[currentNode] && brakeTorque == 0f)
        {

        FL.motorTorque = maxTorque;
        FR.motorTorque = maxTorque;
        }
        else
        {
            FL.motorTorque = 0;
            FR.motorTorque = 0;
        }

    }
    private void ApplySteer()
    {
        Vector3 offset = transform.right * (-2f);
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position );
        float newSteer = (relativeVector.x / relativeVector.magnitude) * maxSteerAngle;
        FL.steerAngle = newSteer;
        FR.steerAngle = newSteer;
       
    }
}


