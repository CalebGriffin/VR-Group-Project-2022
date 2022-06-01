using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImprovedSeaController : MonoBehaviour
{
    private float oldAngle = 0;
    private float newAngle = 0;
    private float lerpTime = 10f;

    public Material oceanMat;

    void Awake()
    {
        StartCoroutine(RandomizeTheFlow());
    }

    private IEnumerator RandomizeTheFlow()
    {
        newAngle = Random.Range(0, 360);

        LeanTween.value(this.gameObject, oldAngle, newAngle, lerpTime).setOnUpdate((float val) =>
        {
            ConvertAngleToVector(val);
        }).setOnComplete(() => {});

        yield return new WaitForSeconds(15f);

        StartCoroutine(RandomizeTheFlow());
    }

    private void ConvertAngleToVector(float angle)
    {
        float x1 = Mathf.Sin(angle);
        float y1 = Mathf.Cos(angle);

        UpdateFlow1(x1 / 5, y1 / 5);
    }

    private void UpdateFlow1(float x, float y)
    {
        oceanMat.SetVector("_FlowDirection1", new Vector4(x, y, 0f, 1f));
    }

    private void Finished1()
    {
        oldAngle = newAngle;
    }
}
