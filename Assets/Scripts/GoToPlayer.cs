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
        InvokeRepeating("updatePath", 0, 0.5f);
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

            float randomDrop = Random.Range(0, 100f);

            if (randomDrop <= 10)
            {
                Instantiate(drop[0], transform.position, transform.rotation);
            }
            else
            {
                Instantiate(drop[1], transform.position, transform.rotation);
            }

            Destroy(gameObject);
        }
    }

    private void updatePath()
    {
        agent.destination = player.transform.position;
    }

    public int getDamage()
    {
        return damage;
    }
    public void takeDamage(int dmg)
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
