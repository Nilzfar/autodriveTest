using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LSL;
using Tobii.XR;

// has to be added as a new component on LSL_functions

public class Focused_LSL : MonoBehaviour
{
    private TobiiXR_Settings _settings;
    private liblsl.StreamInfo _streamInfo;
    private liblsl.StreamOutlet _streamOutlet;
    
    public string StreamName = "Tobii_Focused_Object";
    public string StreamType = "Eyetracking_Focus";
    private int ChannelCount = 1;

    private GameObject testObj;
    private bool fcCheck;
    
    
    private void Awake()
    {
        TobiiXR.Start(_settings);
    }
    // Start is called before the first frame update
    void Start()
    {
        testObj = GameObject.Find("LSL_Functions");
        fcCheck = testObj.GetComponent<Menu_LSL>().FocusObjects;
        if (fcCheck)
        {
            _streamInfo = new liblsl.StreamInfo(StreamName,
                                            StreamType,
                                            ChannelCount,
                                            0,
                                            liblsl.channel_format_t.cf_string);

            _streamOutlet = new liblsl.StreamOutlet(_streamInfo);
        }
    }

    public void FixedUpdate()
    {
        
        if (fcCheck)
        {
            if (TobiiXR.FocusedObjects.Count > 0)
            {
                string[] mySample = new string[1];
                mySample[0] = TobiiXR.FocusedObjects[0].GameObject.name;
                Debug.Log(mySample[0]);  // delete comment part to see detected obj in console
                _streamOutlet.push_sample(mySample);
            }
        }
    }
}
