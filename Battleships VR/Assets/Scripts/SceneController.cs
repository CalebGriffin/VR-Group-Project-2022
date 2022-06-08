using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Valve.VR.InteractionSystem;
using Valve.VR;

public class SceneController : MonoBehaviour
{
    #region Singleton
    public static SceneController instance;
    void Awake()
    {
        if (instance == null)
            instance = this;
    }
    #endregion

    [SerializeField] private SceneReference[] scenes;
    [SerializeField] private SceneReference mainScene;
    
    private float animationTime = 1f;
    private bool isLoading = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeInDelay());
    }

    private void LoadAllScenes()
    {
        foreach(SceneReference scene in scenes)
        {
            SceneManager.LoadScene(scene, LoadSceneMode.Additive);
        }
    }

    private void FadeIn()
    {
        SteamVR_Fade.Start(Color.black, 0);
        SteamVR_Fade.Start(Color.clear, animationTime);
    }

    public IEnumerator FadeInDelay()
    {
        SteamVR_Fade.Start(Color.black, 0);

        foreach (SceneReference scene in scenes)
        {
            AsyncOperation loadScene = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Additive);

            while (!loadScene.isDone)
            {
                yield return null;
            }
        }

        yield return new WaitForSeconds(animationTime);

        SteamVR_Fade.Start(Color.clear, animationTime);
    }

    [ContextMenu(nameof(ReloadScene))]
    public void ReloadScene()
    {
        if (isLoading)
            return;

        StartCoroutine(ReloadSceneDelay());
    }

    public IEnumerator ReloadSceneDelay()
    {
        isLoading = true;

        SteamVR_Fade.Start(Color.clear, 0);
        SteamVR_Fade.Start(Color.black, animationTime);

        AsyncOperation loadScene = SceneManager.LoadSceneAsync(mainScene, LoadSceneMode.Single);
        loadScene.allowSceneActivation = false;

        yield return new WaitForSeconds(animationTime);

        while (loadScene.progress < 0.9f)
            yield return null;
        
        loadScene.allowSceneActivation = true;

        isLoading = false;
    }
}
