using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tobii.G2OM;

// have be added as new component on cubes

public class set_focusable : MonoBehaviour, IGazeFocusable
{
    public void GazeFocusChanged(bool hasFocus)
    {
        //This object either received or lost focused this frame, as indicated by the hasFocus parameter.
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
