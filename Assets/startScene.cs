using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startScene : MonoBehaviour
{
    public void Scene1()
    {
        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }
    public void Scene2()
    {
        SceneManager.LoadScene(2, LoadSceneMode.Single);
    }
}
