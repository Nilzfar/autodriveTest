using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutonomousAnimator : MonoBehaviour
{
    public GameObject car;
    private List<Transform> nodes = new List<Transform>();
    
    // Start is called before the first frame update
    void Start()
    {
        Transform[] pathTransform = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();
        for (int i = 0; i < pathTransform.Length; i++)
        {
            if (pathTransform[i] != transform)
            {
                nodes.Add(pathTransform[i]);
            }

        }
        }

        // Update is called once per frame
        void Update()
    {
        
    }
}
