using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestRunner : MonoBehaviour
{
    public float timeDelay;

    public AI ai;
    public AI2 ai2;

    public bool ai1Turn = true;

    public bool playing = false;

    public Text buttonText;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        yield return new WaitForSeconds(timeDelay);

        if (playing && ai1Turn)
        {
            ai.Decision();
            ai1Turn = false;
        }

        yield return new WaitForSeconds(timeDelay);

        if (playing && !ai1Turn)
        {
            ai2.Decision();
            ai1Turn = true;
        }

        StartCoroutine(Run());
    }

    public void Button()
    {
        playing = !playing;
        buttonText.text = playing ? "Pause" : "Play";
    }

}
