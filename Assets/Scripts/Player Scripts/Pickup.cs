using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{

    bool inRange = false, warned = false;
    public GameObject warningText, pickupText;
    public int type, effect;
    GameManager manager;
    // Update is called once per frame
    void Update()
    {
        if (warned && Input.GetKeyDown(KeyCode.E))
        {
            manager = GameObject.Find("GameManager").GetComponent<GameManager>();
            manager.spawnWave(gameObject);
            manager.RemovePointer(gameObject);
            warningText.SetActive(false);
            pickupText.SetActive(false);
            if (!GameObject.FindWithTag("Collectables"))
            {
                StartCoroutine(nameof(waitForEnd));
            }
            else
            {
                Destroy(gameObject);
            }
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

    public IEnumerator waitForEnd()
    {
        yield return new WaitForSeconds(30);
        manager.end = true;
    }
}
