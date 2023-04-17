using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Wand : MonoBehaviour
{
    public GameObject projectilePrefab { get; set; }
    public float speed;
    public float damage;
    public float projectileSpeed;
    public Rigidbody rb;
    public GameObject player;
    public SphereCollider sc;

    public RaycastHit hit;
    public Vector3 direction;

    [SerializeField]
    private LayerMask ignoreItems;
    [SerializeField]
    private LayerMask enemyLayer;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = 5;
        StartCoroutine("fireWand");
        player = transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator fireWand()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            Debug.Log(getClosestEnemy());
        }
    }

    public GameObject getClosestEnemy()
    {
        float dist = Mathf.Infinity;
        GameObject closestEnemy = null;
        Collider[] enemies = Physics.OverlapSphere(player.transform.position, sc.radius, enemyLayer); ;
        
        foreach(Collider collider in enemies)
        {
            direction = collider.transform.position - player.transform.position;

            Physics.Raycast(player.transform.position, direction, out hit, Mathf.Infinity, ignoreItems);

            if(hit.collider != null && 
                hit.collider.gameObject.layer == 9 && 
                hit.distance < dist)
            {
                dist = hit.distance;
                closestEnemy = collider.gameObject;
            }
        }

        return closestEnemy;
    }
    private void OnDrawGizmos()
    {
        Debug.DrawRay(player.transform.position,direction);
    }

}
