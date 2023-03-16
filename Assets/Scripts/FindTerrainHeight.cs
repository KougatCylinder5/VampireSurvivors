using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindTerrainHeight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        RaycastHit hit;
        Physics.Raycast(gameObject.transform.position, Vector3.down, out hit, 15);
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, 15 - hit.distance, gameObject.transform.position.z);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
