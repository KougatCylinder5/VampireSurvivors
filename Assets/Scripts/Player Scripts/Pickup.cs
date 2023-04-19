using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    bool inRange = false, warned = false;
    public GameObject warningText, pickupText;
    public int type, effect;
    // Update is called once per frame
    void Update()
    {
        if (warned && Input.GetKeyDown(KeyCode.E))
        {
            GameManager manager = GameObject.Find("GameManager").GetComponent<GameManager>();
            manager.spawnWave(gameObject);
            manager.RemovePointer(gameObject);
            warningText.SetActive(false);
            pickupText.SetActive(false);
            gameObject.SetActive(false);

        }
        else if (inRange && Input.GetKeyDown(KeyCode.E))
        {
            warned = true;
            warningText.SetActive(true);
        }
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = true;
            warned = false;
            pickupText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            inRange = false;
            warned = false;
            pickupText.SetActive(false);
            warningText.SetActive(false);
        }
    }
}
