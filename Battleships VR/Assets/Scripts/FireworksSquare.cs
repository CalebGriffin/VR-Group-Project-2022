using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireworksSquare : MonoBehaviour
{
    public List<GameObject> originalRocketList = new List<GameObject>();
    public List<GameObject> rocketList = new List<GameObject>();

    void Awake()
    {
        //StartCoroutine(FireTheRockets());
    }

    private IEnumerator FireTheRockets()
    {
        for (int i = 0; i < originalRocketList.Count; i++)
        {
            float time = Random.Range(0.5f, 1.5f);
            yield return new WaitForSeconds(time);
            int index = Random.Range(0, rocketList.Count);
            rocketList[index].SetActive(true);
            rocketList.Remove(rocketList[index]);
        }
    }

    void OnEnable()
    {
        StartCoroutine(FireTheRockets());
    }

    void OnDisable()
    {
        rocketList = new List<GameObject>(originalRocketList);
        foreach (GameObject rocket in rocketList)
        {
            rocket.SetActive(false); 
        }
    }
}
