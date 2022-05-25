using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class TextHandler : MonoBehaviour
{
    public TextAsset textFile;
    private void Start()
    {
        //Debug.Log(textFile.ToString());
    }
    //[MenuItem("Tools/Read file")]
    public void ReadString()
    {
        StreamReader reader = new StreamReader(textFile.ToString());
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }

}
