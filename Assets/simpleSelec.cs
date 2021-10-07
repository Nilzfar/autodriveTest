using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine.UI;

[ExecuteInEditMode]
public class simpleSelec : MonoBehaviour
{
    // Start is called before the first frame update
    public int count = 0;
    public bool state = false;
    void Start()
    {
        state = false;
    }
    public void copyUntil()
    {

        if (state ||Input.GetKeyDown("d"))
        {
                DuplicateSelectedPrefab();
            
        }

    }
    public void DuplicateSelectedPrefab()
    {

        
        count++;
        var tmp = this.gameObject;
            var o = Instantiate(tmp, tmp.transform.position, tmp.transform.rotation);
            
        o.GetComponent<simpleSelec>().count = count;
            o.name = "node " + count.ToString();
            o.SetActive(true);
        o.transform.parent = transform.parent;
            Selection.activeGameObject = o;
        state = false;
        
    }


    public void toggleState()
    {
        if (Input.GetKey("space") || Input.GetMouseButton(1))
        {
            state = true;
        }
    }


        // Update is called once per frame
        void Update()
    {
        toggleState();
        copyUntil();
    }
}