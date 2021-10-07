using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NaviScreen : MonoBehaviour
{
    public RawImage speedometer;
    public GameObject speed;
    public float x, y;
    public Camera cam;
    public Transform objectToFollow;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = new Vector3(transform.position.x + x, transform.position.y + y, transform.position.z);
        Vector3 p = cam.WorldToScreenPoint(transform.position);
        speed.transform.position = p;
//        speed.transform.rotation = transform.rotation;
    }
}
