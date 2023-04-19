using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GoToPlayer : MonoBehaviour
{
    private GameObject player;
    private NavMeshAgent agent;
    public GameObject[] drop;
    public double health = 100;
    private int damage = 10;
    public Material dmgMaterial;
    public Material normalMaterial;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        agent = GetComponent<NavMeshAgent>();
        agent.speed = 1.0f;
        
        StartCoroutine("updatePath");
        GetComponent<MeshRenderer>().material = normalMaterial;
    }

    // Update is called once per frame
    void Update()
    {
        checkHealth();
         
       
        
    }

    private void checkHealth()
    {
        if (health <= 0)
        {

            float randomDrop = UnityEngine.Random.Range(0, 100f);

            if (randomDrop <= 10)
            {
                Instantiate(drop[0], transform.position, transform.rotation);
            }
            else
            {
                Instantiate(drop[1], transform.position, transform.rotation);
            }
            StopAllCoroutines();
            Destroy(gameObject);
            

        }
    }

    private IEnumerator updatePath()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            if (Vector3.Distance(player.transform.position, transform.position) > 50)
            {
                Destroy(gameObject);
                break;
            }
            agent.destination = player.transform.position;
            

        }
    }

    public int getDamage()
    {
        return damage;
    }
    public void takeDamage(float dmg)
    {
        health -= dmg;
        GetComponent<MeshRenderer>().material = dmgMaterial;
        StartCoroutine("resetAppearance");
    }
    public IEnumerator resetAppearance()
    {

        yield return new WaitForSeconds(0.5f);
        GetComponent<MeshRenderer>().material = normalMaterial;

    }

}
