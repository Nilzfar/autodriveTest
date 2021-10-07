 using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Path : MonoBehaviour
{
    public Color lineColor;
    public bool convex = false;
    private List<Transform> nodes = new List<Transform>();
    public List<float> speeds = new List<float>();
    public List<bool> notify= new List<bool>();
    public List<bool> navigate= new List<bool>();
    public List<string> dir= new List<string>();
    public List<string> obstacle= new List<string>();
    public bool state = false;

    //TO make the path visible in the editor
    public void OnDrawGizmos()
    {
        //Set color for editor
      //  copyUntil();
        Gizmos.color = lineColor;

        //Get all nodes
        var pathTransforms = GetComponentsInChildren<Transform>();
        var speedsFloat = GetComponentsInChildren<Node>();
        //All nodes that are not the top node
        nodes = new List<Transform>();
        speeds = new List<float>();
        notify = new List<bool>();
        navigate = new List<bool>();
        dir = new List<string>();
        obstacle = new List<string>();
        for (int i = 0; i < pathTransforms.Length; i++)
        {
            if(i < speedsFloat.Length)
            {

            speeds.Add(speedsFloat[i].speed);
                notify.Add(speedsFloat[i].isNotification);
                navigate.Add(speedsFloat[i].isNavigation);
                dir.Add(speedsFloat[i].direction);
                obstacle.Add(speedsFloat[i].objectType);

            }
             
            if (pathTransforms[i] != transform)
            {
                nodes.Add(pathTransforms[i]);
                
            }
        }
        //Draw line between every node
        int maxCount = nodes.Count;
        if (convex == false)
        {
            maxCount = maxCount - 1;
//            Gizmos.DrawWireSphere(nodes[nodes.Count - 1].position, 3f);
        }





            for (var i = 0; i < maxCount; i++)
        {
            var currentNode = nodes[i].position;
            var nextNode = Vector3.zero;

            //Exist multiple nodes
            if (nodes.Count <= 1) continue;

            //if we hit the last one
            if (i + 1 > nodes.Count - 1)
                nextNode = nodes[0].position;
            if (i + 1 <= nodes.Count - 1)
                nextNode = nodes[i + 1].position;

            Gizmos.DrawLine(currentNode, nextNode);
            Gizmos.DrawWireSphere(currentNode,3f);
        }

    }


    void Update()
    {

    
    }
   }

