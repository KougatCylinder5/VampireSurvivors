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
    public GameManager manager;
    public new Animator animation;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();

        agent = GetComponent<NavMeshAgent>();
        agent.speed = 1.0f;

        animation = GetComponent<Animator>();
        animation.Play("Walk",0,UnityEngine.Random.Range(0f,1f));
        StartCoroutine(updatePath());

        scaling();
        
    }

    public void die()
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

    public void scaling()
    {
        health = (10 * Mathf.RoundToInt(manager.seconds / 30)) + 50;
        damage = (5 * Mathf.RoundToInt(manager.seconds / 30)) + 5;
    }
    
    private IEnumerator updatePath()
    {
        agent.destination = player.transform.position;
        float waitForTime = UnityEngine.Random.Range(0, 1f);

        while (true)
        {
            yield return new WaitForSecondsRealtime(waitForTime);
            if (Vector3.Distance(player.transform.position, transform.position) > 25)
            {
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.NoObstacleAvoidance;
            }
            if(Vector3.Distance(player.transform.position, transform.position) < 25)
            {
                agent.obstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
            }
            if (Vector3.Distance(player.transform.position, transform.position) > 100)
            {
                Destroy(gameObject);
                break;
            }
            agent.destination = player.transform.position;
        }
    }
    public void takeDamage(float dmg)
    {
        health -= dmg;
        agent.isStopped = true;
        if (health <= 0)
        {
            animation.SetTrigger("Death");
            agent.isStopped = true;
        }
        else
        {
            StartCoroutine(nameof(pauseAttack));
        }
    }
    public IEnumerator pauseAttack()
    {
        animation.SetTrigger("Idle");
        yield return new WaitForSeconds(0.5f);
        agent.isStopped = false;

    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && health > 0)
        {
            player.GetComponent<PlayerController>().health -= damage * Time.deltaTime;
            animation.SetTrigger("Attack");
        }
    }

}
