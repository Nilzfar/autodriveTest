using System.Collections;  
using System.Collections.Generic;
using UnityEngine;

public class CarAnimator : MonoBehaviour
{

    public List<GameObject> carPrefabs;
    private GameObject curPrefab;
    public GameObject mainCar;
    private List<GameObject> activePrefabs = new List<GameObject>();
    public bool keepConstant = true;
    //public GameObject road;
    public List<int> spawnIndices = new List<int>();
    private List<Transform> nodes = new List<Transform>();
    private List<Transform> spawnP = new List<Transform>();
    // Start is called before the first frame update
    private bool convex;
    private float instantiationTimer = 5f;
    public float interval = 10f;

    public float spawnDistance = 30f;
    private bool spawnFree = true;
    private int startI = 0;
    public int isNext = 0;
    void Start()
    {
        convex = GetComponent<Path>().convex;
        Transform[] pathTransform = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();
        if(mainCar != null)
        {
            activePrefabs.Add(mainCar);
            startI = 1;
        }
        for (int i = 0; i < pathTransform.Length; i++)
        {
            if (pathTransform[i] != transform)
            {
                nodes.Add(pathTransform[i]);
            }

        }

        for (int i = nodes.Count-1; i > 0;i--)
        {
            if (spawnIndices.Contains(i))
            {
                int r = Random.Range(0, carPrefabs.Count);
                curPrefab = carPrefabs[r];
                spawnAt(i,true);
            }

        }
            if(mainCar != null) { 
            for (int i = 1; i < activePrefabs.Count; i++)
            {
                mainCar.GetComponent<AutonomousEngine>().activePrefabs.Add(activePrefabs[i]);

            }
            }


    }
    private void spawnAt(int index, bool start)
    {
            checkNode();
        if (spawnFree)
        {
            Vector3 pos = new Vector3(nodes[index].transform.position.x, nodes[index].transform.position.y + 1f, nodes[index].transform.position.z);
            var tmp1 = Instantiate(curPrefab, pos, nodes[index].transform.rotation);
            tmp1.GetComponent<CarEngine>().path = gameObject.transform;
            tmp1.GetComponent<CarEngine>().convex = convex;
            tmp1.GetComponent<CarEngine>().instantiateNode = index;
            findNext(tmp1, index, start);
            activePrefabs.Add(tmp1);
        }
     }
    private void findNext(GameObject instance,int index,bool start)
    {

        GameObject next = null;
        int tmp;
        float minDistance = 100000f;
        float minDistanceMain = 100000f;
        float nodeThr = 30f;
        for (int i = 1; i < activePrefabs.Count; i++)
        {
            try
            {
                if (start)
                {
                    tmp = activePrefabs[i].GetComponent<CarEngine>().instantiateNode;
                }
                else
                {
                    tmp = activePrefabs[i].GetComponent<CarEngine>().currentNode;
                }

                if ((tmp >= index && activePrefabs[i].gameObject != instance && Vector3.Distance(instance.transform.position, activePrefabs[i].transform.position) < minDistance))
                {
                    if (mainCar != null && Vector3.Distance( mainCar.transform.position, instance.transform.position) < minDistance)
                    {
                        minDistance = Vector3.Distance(mainCar.transform.position, instance.transform.position);
                        next = mainCar;
                    }
                    else
                    {
                        minDistance = Vector3.Distance(instance.transform.position, activePrefabs[i].transform.position);
                        next = activePrefabs[i].gameObject;
                    }
                }

            }
            catch (MissingReferenceException)
            {
                continue;
            }
        }
        instance.GetComponent<CarEngine>().nextCar = next;

    }
    private void checkNode()
    {
        for (int i = startI; i < activePrefabs.Count; i++)
        {
            try
            {
                if (activePrefabs[i].GetComponent<CarEngine>().currentNode == nodes.Count - 1)
                {
                    activePrefabs.Remove(activePrefabs[i]);
                }
                if(Vector3.Distance(activePrefabs[i].transform.position,nodes[0].transform.position) < spawnDistance)
                {
                    spawnFree = false;
                }
                else
                {
                    spawnFree = true;
                }
            }
            catch (MissingReferenceException)
            {
                spawnFree = true;
                continue;
            }
        }
    }
    void Update()
    {
        
        int r = Random.Range(0, carPrefabs.Count);
        curPrefab = carPrefabs[r];
        instantiationTimer -= Time.deltaTime;
        if(FindObjectsOfType<CarEngine>().Length < spawnIndices.Count && instantiationTimer <= 0 && keepConstant)
        {
            spawnAt(0,false);
            instantiationTimer = interval;
        }
        else if(keepConstant== false && instantiationTimer<= 0){
            spawnAt(0, false);
            instantiationTimer = interval;
        }
    }
}

