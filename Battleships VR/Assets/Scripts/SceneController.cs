using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField] private string[] sceneNames;
    // Start is called before the first frame update
    void Start()
    {
        LoadAllScenes();
    }

    private void LoadAllScenes()
    {
        foreach(string name in sceneNames)
        {
            SceneManager.LoadScene(name, LoadSceneMode.Additive);
        }

    }

}
