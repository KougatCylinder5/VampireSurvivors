using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
using UnityEngine.AI;

public class Runetracer : MonoBehaviour
{
    public float speed;
    public int damage;

    private GameObject target;
    private GameObject player;
    private Rigidbody rb;

    private float orbitPercent = 0f;
    public float radius = 5;
    // Start is called before the first frame update
    void Start()
    {
        //InvokeRepeating("updateTargets", 0, 1);
        player = GameObject.Find("Player");
        rb = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        followPlayer();
    }

    /*private void updateTargets()
    //{
    //
        List<GameObject> enemies = new List<GameObject>();
        foreach(GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            enemies.Add(enemy);
        }
        GameObject closest = enemies.ToArray().OrderBy(t => (t.transform.position - player.transform.position).sqrMagnitude)
                            .FirstOrDefault();
        target = closest;
    }*/
    
    private void followPlayer()
    {
        float x,z;
        orbitPercent += Time.deltaTime * speed;

        if(orbitPercent > 100)
        {
            orbitPercent = 0f;
        }
        float degrees = 360 * (orbitPercent / 100);

        x = radius*Mathf.Cos(degrees);
        z = radius*Mathf.Sin(degrees);
        transform.position = Vector3.Lerp(transform.position,new Vector3(x,0,z) + player.transform.position,0.5f);
    }

    public void OnTriggerEnter(Collider other) { 

        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<GoToPlayer>().takeDamage(damage);
        }
    }
}
