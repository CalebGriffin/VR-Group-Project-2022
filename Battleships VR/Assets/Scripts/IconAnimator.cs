using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconAnimator : MonoBehaviour
{
    void Awake()
    {
        Vector3 fullScale = transform.localScale;
        transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(this.gameObject, fullScale, 0.5f).setEase(LeanTweenType.easeInBounce);
    }
}
