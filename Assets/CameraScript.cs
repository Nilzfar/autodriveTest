using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraScript : MonoBehaviour
{
    public GameObject obj;
    private Transform objectToFollow;
    public Vector3 offset= new Vector3(6.75f,4.67f,1.49f);
    public float lookSpeed=100f;
    public float followSpeed=100f;
    public bool isMouse = true;
    public bool manualSteer = false;
    private bool look = false;
    private float yaw = 0f;
    private float pitch = 0f;
        private float posx = 0f;
        private float posy = 0f;
    private bool isAutonomous = false;
    public Text modeObj;
    public GameObject slideKnob;
    private float steer = 0f;
    private float initialSteerPos = 0f;
    public GameObject needle;
    private void updateMode()
    {
        if (Input.GetMouseButton(0))
        {
            modeObj.text = "Manual Mode";
            manualSteer = true;
        }
        else
        {
            manualSteer = false;
            modeObj.text = "Semiautonomous Mode";
            }
            obj.GetComponent<AutonomousEngine>().isAutonomous = !manualSteer;
    }
    private void animateTacho()
    {
        float needlePos = (obj.GetComponent<AutonomousEngine>().currentSpeed/40) * 114;
        needle.transform.eulerAngles = new Vector3(0,0,-needlePos);

    }
    private void AnimateHandle()
    {
        float offset = Screen.width * 0.5f;
        if (manualSteer)
        {
            float fac = (1 - obj.GetComponent<AutonomousEngine>().currentSpeed / 30) * 3f;
            posx += fac * Input.GetAxis("Mouse X");

            slideKnob.transform.position = new Vector3(offset + posx, 10f, 0);
            if (posx > (300))
            {
                posx = 300f;
            }
            else if (posx < (-300))
            {
                posx = -300f;
            }
            steer = posx / 300;
            obj.GetComponent<AutonomousEngine>().animAngle = steer;
        }
        else
        {
            posx = Mathf.Lerp(0, obj.GetComponent<AutonomousEngine>().FL.steerAngle, Time.deltaTime);
posx = posx * 500f;

            slideKnob.transform.position = new Vector3(offset + posx, 10f, 0);

        }
    }
    void Start()
    {

        objectToFollow = obj.transform;
    }
    public void LookAtTarget()
    {
        Vector3 pos = objectToFollow.forward;

        Quaternion _rot = Quaternion.LookRotation(pos, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, _rot, lookSpeed * Time.deltaTime);


    }
    public void MoveToTarget()
    {
        Vector3 _targetPos = objectToFollow.position +
                            (objectToFollow.forward )* offset.z +
                            objectToFollow.right * offset.x +
                            objectToFollow.up * offset.y;
        transform.position = Vector3.Lerp(transform.position, _targetPos, followSpeed * Time.deltaTime);
    }
    private void MouseRot()
    {
        yaw += 2f * Input.GetAxis("Mouse X");
        pitch -= 2f * Input.GetAxis("Mouse Y");
        transform.eulerAngles += new Vector3(pitch, yaw, 0.0f);
    }
    private void toggleLook()
    {
        look = (Input.GetMouseButton(1));
        
    }
    void Update()
    {

    }
    private void FixedUpdate()
    {
        LookAtTarget();
        animateTacho();
        MoveToTarget();
        updateMode();
        
            AnimateHandle();
        
        toggleLook();
        if (isMouse && look)
        {
            MouseRot();
        }
    }
}
