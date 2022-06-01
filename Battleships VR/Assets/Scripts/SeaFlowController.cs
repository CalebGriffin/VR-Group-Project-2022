using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SeaFlowController : MonoBehaviour
{
    [SerializeField] private Material oceanMat;

    private float elapsedTime = 0f;
    private float lerpTime = 10f;

    [SerializeField] private float x1, y1, x2, y2;
    [SerializeField] private float oldX1, oldY1, oldX2, oldY2;
    [SerializeField] private float newX1, newY1, newX2, newY2;

    private bool shouldBeLerping = false;

    // Start is called before the first frame update
    void Start()
    {
    }

    void Awake()
    {
        oldX1 = oceanMat.GetVector("_FlowDirection1")[0];
        oldY1 = oceanMat.GetVector("_FlowDirection1")[1];
        oldX2 = oceanMat.GetVector("_FlowDirection2")[0];
        oldY2 = oceanMat.GetVector("_FlowDirection2")[1];
        StartCoroutine(RandomizeTheFlow());
    }

    private IEnumerator RandomizeTheFlow()
    {
        Debug.Log("Running Lerp Coroutine");
        newX2 = Random.Range(-0.02f, 0.02f);
        newY2 = Random.Range(-0.02f, 0.02f);
        newX1 = Random.Range(-0.2f, 0.2f);
        newY1 = Random.Range(-0.2f, 0.2f);

        LeanTween.value(this.gameObject, oldX1, newX1, lerpTime).setOnUpdate((float val) => 
        {
            x1 = val;
        });
        LeanTween.value(this.gameObject, oldY1, newY1, lerpTime).setOnUpdate((float val) => 
        {
            y1 = val;
            UpdateFlow1();
        }).setOnComplete(() => {Finished1();});
        LeanTween.value(this.gameObject, oldX2, newX2, lerpTime).setOnUpdate((float val) => 
        {
            x2 = val;
        });
        LeanTween.value(this.gameObject, oldY2, newY2, lerpTime).setOnUpdate((float val) => 
        {
            y2 = val;
            UpdateFlow2();
        }).setOnComplete(() => {Finished2();});

        elapsedTime = 0f;
        //shouldBeLerping = true;

        yield return new WaitForSeconds(15f);

        StartCoroutine(RandomizeTheFlow());
    }

    private void UpdateFlow1()
    {
        oceanMat.SetVector("_FlowDirection1", new Vector4(x1, y1, 0f, 1f));
    }
    
    private void UpdateFlow2()
    {
        oceanMat.SetVector("_FlowDirection2", new Vector4(x2, y2, 0f, 1f));
    }

    private void Finished1()
    {
        oldX1 = newX1;
        oldY1 = newY1;
    }

    private void Finished2()
    {
        oldX2 = newX2;
        oldY2 = newY2;
    }

    // Update is called once per frame
    void Update()
    {
        //if (shouldBeLerping)
        //{
            //if (elapsedTime < lerpTime)
            //{
                //x1 = Mathf.Lerp(oldX1, newX1, elapsedTime/lerpTime);
                //y1 = Mathf.Lerp(oldY1, newY1, elapsedTime/lerpTime);
                //x2 = Mathf.Lerp(oldX2, newX2, elapsedTime/lerpTime);
                //y2 = Mathf.Lerp(oldY1, newY2, elapsedTime/lerpTime);
                //elapsedTime += Time.deltaTime;
            //}
            //else
            //{
                //x1 = newX1;
                //y1 = newY1;
                //x2 = newX2;
                //y2 = newY2;

                //oldX1 = newX1;
                //oldY1 = newY1;
                //oldX2 = newX2;
                //oldY2 = newY2;

                //shouldBeLerping = false;
                //elapsedTime = 0f;
            //}

            ////list1 = new float[] {x1, y1, 0f, 1f};
            //oceanMat.SetVector("_FlowDirection1", new Vector4(x1, y1, 0f, 1f));

            ////list2 = new float[] {x2, y2, 0f, 1f};
            //oceanMat.SetVector("_FlowDirection2", new Vector4(x2, y2, 0f, 1f));
        //}
    }
}
