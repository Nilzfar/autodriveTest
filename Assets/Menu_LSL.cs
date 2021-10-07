using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Diagnostics;
using Valve.VR;
using Assets.LSL4Unity.Scripts; // reference the namespace to get access to all classes
using LSL;

public class Menu_LSL : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("EDA Devices")]
    public bool Bitalino;

    [Header("Eye Tracking")]
    public bool FocusObjects;
    public bool GazeRayData;
    //public bool SRanipal;

    [Header("Simulator")]
    public bool CarPosition;

    [Header("Recording")]
    public string[] Recording = new string[] { "No", "Yes" };
    public int RecIdx = 0;

    public string Save_Path = @"C:\Users\" + Environment.UserName + @"\Desktop\Unity_Data\";

    private bool start_flag = true;

    // Start is called before the first frame update
    void Start()
    {
        if (start_flag)
        {
            start_flag = false;

            string strCmdText;
            string curr_day = System.DateTime.Now.Day.ToString();
            string curr_mon = System.DateTime.Now.Month.ToString();
            string curr_year = System.DateTime.Now.Year.ToString();
            string curr_hour = System.DateTime.Now.Hour.ToString();
            string curr_min = System.DateTime.Now.Minute.ToString();
            string curr_sec = System.DateTime.Now.Second.ToString();
            string curr_time = curr_mon + "-" + curr_day + "-" + curr_year + "_" + curr_hour + "-" + curr_min + "-" + curr_sec;

            //###################################
            //############# EDA LSL #############
            //###################################

            if (Bitalino)
            {
                System.Diagnostics.Process.Start("CMD.exe", @"/C python .\Assets\bitalino_LSL.py");
            }


            //###################################
            //######## Eye Tracking LSL #########
            //###################################

            if (FocusObjects)
            {
                // check mark will be used in another script
            }

            if (GazeRayData)
            {
                System.Diagnostics.Process.Start("CMD.exe", @"/C .\Assets\Eyetracking_ViVE\SRanipal_Sample.exe");
            }


            //###################################
            //######## Simulation LSL #########
            //###################################

            if (CarPosition)
            {
                // check mark willbe used in another script
            }


            //###################################
            //########### Recording  ############
            //###################################

            if (RecIdx == 1)
            {
                StartCoroutine(StartRecorder(curr_time));
            }
        }
    }
    IEnumerator StartRecorder(string curr_time)
    {
        yield return new WaitForSeconds(10.0f);
        if (!Directory.Exists(Save_Path)) Directory.CreateDirectory(Save_Path);
        string strCmdText = @"/C .\Assets\LabRecorder\LabRecorderCLI.exe " + Save_Path + curr_time + ".xdf 'searchstr'";
        System.Diagnostics.Process.Start("CMD.exe", strCmdText);
    }
}
