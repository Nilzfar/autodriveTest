using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.IO;

[CustomEditor(typeof(Menu_LSL))]
public class Menu_LSL_Editor : Editor
{
    SerializedProperty bitalino;
    SerializedProperty save_path;
    SerializedProperty focusobjects;
    SerializedProperty gazeraydata;
    SerializedProperty sranipal;
    SerializedProperty carposition;

    private void OnEnable()
    {
        bitalino = serializedObject.FindProperty("Bitalino");

        focusobjects = serializedObject.FindProperty("FocusObjects");
        gazeraydata = serializedObject.FindProperty("GazeRayData");
        //Ssranipal = serializedObject.FindProperty("SRanipal");

        carposition = serializedObject.FindProperty("CarPosition");

        save_path = serializedObject.FindProperty("Save_Path");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        Menu_LSL script = (Menu_LSL)target;

        GUIContent Bitalino = new GUIContent("Bitalino");
        EditorGUILayout.PropertyField(bitalino, Bitalino);

        GUIContent FocusObjects = new GUIContent("FocusObjects");
        EditorGUILayout.PropertyField(focusobjects, FocusObjects);

        GUIContent GazeRayData = new GUIContent("GazeRayData");
        EditorGUILayout.PropertyField(gazeraydata, GazeRayData);

        //GUIContent SRanipal = new GUIContent("SRanipal");
        //EditorGUILayout.PropertyField(sranipal, SRanipal);

        GUIContent CarPosition = new GUIContent("CarPosition");
        EditorGUILayout.PropertyField(carposition, CarPosition);

        GUIContent Save_Path = new GUIContent("     Save Path");
        GUIContent Recording = new GUIContent("Recording?");
        EditorGUILayout.LabelField("Record", EditorStyles.boldLabel);
        script.RecIdx = EditorGUILayout.Popup(Recording, script.RecIdx, script.Recording);

        if (script.RecIdx == 1)
        {
            script.Save_Path = EditorGUILayout.TextField(Save_Path, script.Save_Path);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
