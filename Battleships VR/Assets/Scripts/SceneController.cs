using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LoadAllScenes();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void LoadAllScenes()
    {
        SceneManager.LoadScene("Dan's Scene", LoadSceneMode.Additive);
    }

}
