using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class AutonomousEngine : MonoBehaviour
{
    public Transform path;
    public float maxSteerAngle = 45f;
    public float maxTorque =500f;
    public float currentSpeed;
    public float maxSpeed = 100f;
    public bool convex = false;
    private List<Transform> nodes;
    public int currentNode = 0;
    public WheelCollider FL, FR;
    private float turnSpeed = 5f;
    public WheelCollider RL, RR;
    public Transform FRW, FLW;
    public Transform RRW, RLW;
    public Transform steeringWheel;
    private Quaternion defaultRotation;
    private Vector3 defaultPos;
    private Vector3 defaultFor;
    private Vector3 currentEuler;
    private Quaternion currentRotation;
    public TextMeshPro textMeshPro;
    public float brakeTorque = 0;
    private float prevSteer = 0;
    private float targetSpeed = 5f;
    public float targetSteerAngle = 0;
    public bool isAutonomous = true;
    public float maxBrake = 2000f;
    public GameObject Notification;
    public GameObject Navigation;
    private int NotifyIndex = 0;
    private int NaviIndex = 0;
    public List<GameObject> activePrefabs;
    public bool isBrake = false;
    public float animAngle = 0f;
    public GameObject nextCar;
    public float distance = 30f ;
    public bool searchCar = false;
    public float carDistance = 0f;
    public float angle = 0f;
    private void findNextCar()
    {

        GameObject next = null;
      
        float nodeThr = 30f;
        float minDistance = 100000f;
        
        for (int i = 0; i < activePrefabs.Count; i++)
        
           
            {
            try
            {
                var tmp = activePrefabs[i];//.GetComponent<CarEngine>().currentNode;
                Vector3 dir = transform.position - tmp.transform.position;
                Vector3 localForward = transform.worldToLocalMatrix.MultiplyVector(transform.forward);
                Vector3 prefabForward = transform.worldToLocalMatrix.MultiplyVector(tmp.transform.forward);
                angle = Vector3.Angle(localForward, prefabForward);
                carDistance = Vector3.Distance(transform.position, activePrefabs[i].transform.position);
                if (carDistance < minDistance && activePrefabs[i].transform != transform)
                {
                    minDistance = carDistance;
                    next = activePrefabs[i].gameObject;
                }
            }
            catch (MissingReferenceException)
            {
                activePrefabs.Remove(activePrefabs[i]);
            }
            }
        nextCar = next;
    }
    public void searchCars()
    {
        float min = 20f;
        for (int i = 0; i < activePrefabs.Count; i++)
        {
            float d = Vector3.Distance(transform.position, activePrefabs[i].transform.position);
            if(d < min && activePrefabs[i].transform != transform)
            {
                isBrake = true;
            }

        }
    }
    private void holdDistance()
    {
        distance = currentSpeed * 2f;
        if(distance < 15f)
        {
            distance = 15f;
        }
        if ((nextCar != null && Vector3.Distance(transform.position, nextCar.transform.position) < distance))
        {
         isBrake = true;

    }
        else
        {
            isBrake = false;
        }
    }
    // Start is called before the first frame update
    public void Start()
    {
        defaultRotation = steeringWheel.transform.localRotation;
        defaultPos = steeringWheel.transform.localEulerAngles;
        defaultFor = steeringWheel.transform.forward;
        //Get all nodes
        var pathTransforms = path.GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();

        //All nodes that are not the top node
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if (pathTransforms[i] != transform && pathTransforms[i].transform != path.transform)
            {
                nodes.Add(pathTransforms[i]);
            }
        };
        findNode();

    }
    private void findObstacle()
    {
        if (path.GetComponent<Path>().notify[currentNode+3])
        {
            showNotification(path.GetComponent<Path>().obstacle[currentNode+3]);
            NotifyIndex = currentNode + 4;
        }
        else if(currentNode == NotifyIndex){
            Notification.SetActive(false);
        }
    }
        private void findNav()
    {
        int indexOffset = 3;
        if (path.GetComponent<Path>().navigate[currentNode + indexOffset])
        {

            NaviIndex = currentNode + indexOffset + 2;
            float tmpDist = Vector3.Distance(nodes[currentNode].transform.position, nodes[NaviIndex - 2].transform.position);
            showNavigation(path.GetComponent<Path>().dir[currentNode + indexOffset], tmpDist);
        }
        else if (currentNode < NaviIndex-2)
        {

            float tmpDist = Vector3.Distance(nodes[currentNode].transform.position, nodes[NaviIndex - 2].transform.position);
            showNavigation(path.GetComponent<Path>().dir[currentNode + indexOffset], tmpDist);
        }
        else if (currentNode < NaviIndex)
        {
            showNavigation(path.GetComponent<Path>().dir[currentNode + indexOffset], -1f);
        }
        else if (currentNode == NaviIndex)
        {
            Navigation.SetActive(false);
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isAutonomous)
        {
            UpdateNode();

        }
        else
        {
            findNode();
        }
        try
        {

        if(nextCar == null || searchCar)
        {
            findNextCar();
        }
        }
        catch (MissingReferenceException)
        {
            nextCar = null;
        }
        holdDistance();
        findNav();
        findObstacle();
        ApplySteer();
        LerpToSteerAngle();
        UpdateWheelPoses();
        Drive();
        Braking();
    }
    private void UpdateWheelPoses() {
        UpdateWheel(FL, FLW);
        UpdateWheel(FR, FRW);
        UpdateWheel(RR, RRW);
        UpdateWheel(RL, RLW);
    }
    private void UpdateWheel(WheelCollider _collider, Transform _transform)
    {
        Vector3 pos = transform.position;
        Quaternion quat = transform.rotation;
        _collider.GetWorldPose(out pos, out quat);
        _transform.position = pos;
        _transform.rotation = quat;
    }
    private void findNode()
    {
        float tmpMin = 50f;
        for (int i = 0; i < nodes.Count; i++)
        {
            float curDis = Vector3.Distance(nodes[i].position, transform.position);
            if (curDis < tmpMin)
            {
                tmpMin = curDis;
                currentNode = i + 1;
                currentNode = currentNode % nodes.Count;
            }
        }
    //    findNextCar();
    }
    private void UpdateNode()
    {
        if (Vector3.Distance(nodes[currentNode].position, transform.position) < 6f)
        {
            if (currentNode == nodes.Count - 1)
            {
                if (convex == false)
                {
                    Destroy(gameObject);
                }
                currentNode = 0;
            }
            else
            {
                currentNode++;
            }
      //      findNextCar();
        }
    }
    private void LerpToSteerAngle()
    {
        FL.steerAngle = Mathf.Lerp(FL.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);
        FR.steerAngle = Mathf.Lerp(FR.steerAngle, targetSteerAngle, Time.deltaTime * turnSpeed);

    }
    private void Drive()
    {
        currentSpeed = Mathf.Lerp(currentSpeed, GetComponent<Rigidbody>().velocity.magnitude,Time.deltaTime * 100);
        if (isAutonomous)
        {
            turnSpeed = (1 - Mathf.Abs(currentSpeed / 50)) * 5;
        }
        else
        {
            turnSpeed = 20;
        }
        textMeshPro.text = (((int)currentSpeed)).ToString() + "km/h";

        if (currentSpeed < path.GetComponent<Path>().speeds[currentNode] && brakeTorque == 0f )
        {

            FL.motorTorque = Mathf.Abs(1 - (currentSpeed / 30)) * maxTorque;
            FR.motorTorque = Mathf.Abs(1 - (currentSpeed / 30)) * maxTorque;
        }
        else
        {
            FL.motorTorque = 0;
            FR.motorTorque = 0;
        }

    }
    private void ApplySteerRotation()
    {
        steeringWheel.transform.localEulerAngles = new Vector3(0, 0, -FL.steerAngle* 5);




    }

    private void ApplySteer()
    {
        Vector3 offset = transform.right * (-2f);
        Vector3 relativeVector = transform.InverseTransformPoint(nodes[currentNode].position + offset );
        float newSteer = (relativeVector.x / relativeVector.magnitude);
        if (isAutonomous)
        {
            targetSteerAngle = newSteer * maxSteerAngle;
        }
        else
        {
            targetSteerAngle = animAngle * maxSteerAngle;
        }
        ApplySteerRotation();
        /*if(newSteer > 0f)
        {
            steeringWheel.transform.localEulerAngles += new Vector3(0, 0, 2);
        }
        else if(newSteer == 0)
        {
            steeringWheel.transform.localEulerAngles = defaultPos;
        }
        else
        {
            steeringWheel.transform.localEulerAngles += new Vector3(0, 0, -2);

        }
//        steeringWheel.transform.Rotate(transform.right * Time.deltaTime * 10);
//        steeringWheel.transform.rotation = Quaternion.AngleAxis(30*Time.deltaTime, Vector3.forward) * steeringWheel.transform.rotation;
      //  steeringWheel.transform.eulerAngles = Vector3.Lerp(steeringWheel.transform.rotation.eulerAngles, to, Time.deltaTime);
        */
    }
    public static List<Vector3> SmoothCurve(Vector3 p1, Vector3 p2, float smoothness)
    {
        int curvedLength = (2 * Mathf.RoundToInt(smoothness)) - 1;

        List<Vector3> curvedPoints = new List<Vector3>(curvedLength);
        float t = 0.0f;
        for (int i = 0; i < curvedLength + 1; i++)
        {
            t = Mathf.InverseLerp(0, curvedLength, i);
            var tmp = (1 - t) * p1 + t * p2;
            curvedPoints.Add(tmp);
        }
        return curvedPoints;
}
    public void showNotification(string obj)
    {

        GameObject noteText = Notification.transform.GetChild(3).gameObject;
        noteText.GetComponent<Text>().text = obj;
        

        Notification.SetActive(true);

    }
    public void showNavigation(string dir,float meter)
    {
        Navigation.SetActive(false);
        GameObject turnDir = Navigation.transform.GetChild(3).gameObject;
        GameObject meterText = Navigation.transform.GetChild(5).gameObject;
        if (meter >= 0)
        {
            meterText.GetComponent<Text>().text = ((int)meter).ToString() + 'm';
        }
        else
        {
            meterText.GetComponent<Text>().text = " ";

        }
            turnDir.GetComponent<Text>().text = "Turn " + dir;
        if(dir == "left")
        {
            Navigation.transform.GetChild(4).gameObject.transform.localEulerAngles = new Vector3(0, 0, 180);
        }
        else if(dir == "straight")
        {
            Navigation.transform.GetChild(4).gameObject.transform.localEulerAngles = new Vector3(0, 0, 90);
}
        Navigation.SetActive(true);


    }
    private void Braking()
    {
        if(path.GetComponent<Path>().speeds[currentNode] <= (currentSpeed) || isBrake)
        {
            brakeTorque = (currentSpeed/10) * maxBrake;
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
}


