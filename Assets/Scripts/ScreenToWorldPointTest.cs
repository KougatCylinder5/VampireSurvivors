using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenToWorldPointTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Camera.main.ScreenToWorldPoint(new Vector3(/*Camera.main.pixelWidth/2,Camera.main.pixelHeight/2*/0,0,Camera.main.nearClipPlane)));
    }
}
