using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TESTING
public class MiniBoardSetup : MonoBehaviour
{
    public GameObject boardParent;

    public GameObject cubePrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [ContextMenu("Setup")]
    public void Setup()
    {
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                GameObject go = Instantiate(cubePrefab, boardParent.transform.position + new Vector3(i * boardParent.transform.localScale.x, 0, j * boardParent.transform.localScale.z), Quaternion.identity, boardParent.transform);
                go.transform.localPosition = new Vector3(i*60, 0, j*60);
                if (i == 0)
                    go.name = (j + 1).ToString();
                else if (j == 9)
                    go.name = int.Parse((i + 1).ToString() + "0").ToString();
                else
                    go.name = int.Parse(i.ToString() + (j + 1).ToString()).ToString();
            }
        }
    }
}
