using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptAdder : MonoBehaviour
{
    public GameObject boardParent;

    [ContextMenu(nameof(AddScripts))]
    public void AddScripts()
    {
        foreach (Transform child in boardParent.transform)
        {
            foreach (Transform grandChild in child.transform)
            {
                grandChild.gameObject.AddComponent<IconAnimator>();
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
