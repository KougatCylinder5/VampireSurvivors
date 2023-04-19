using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class Wand : MonoBehaviour
{
    public GameObject projectilePrefab;
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
    [SerializeField]
    private List<Projectile> projectiles = new List<Projectile>();
    // Start is called before the first frame update
    private void Start()
    {
        StartCoroutine("fireWand");
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = 15;
        player = transform.parent.gameObject;
    }

    // Update is called once per frame
    private void Update()
    {
        Debug.Log(projectiles.Count);

        foreach(Projectile projectile in projectiles)
        {
            
            if(projectile.actualProjectile.activeSelf)
            {
                projectile.FollowTarget();
            }
        }
    }

    private void OnEnable()
    {
        StartCoroutine("fireWand");
        rb = GetComponent<Rigidbody>();
        sc = GetComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius = 5;
        player = transform.parent.gameObject;
    }

    public IEnumerator fireWand()
    {
        while (true)
        {
            yield return new WaitForSeconds(speed);
            GameObject enemy = getClosestEnemy();
            if (enemy != null)
            {
                ShootWand(enemy);
            }

        }
    }

    public GameObject getClosestEnemy()
    {
        float dist = Mathf.Infinity;
        GameObject closestEnemy = null;
#pragma warning disable UNT0028 // Use non-allocating physics APIs
        Collider[] enemies = Physics.OverlapSphere(player.transform.position, sc.radius, enemyLayer);
#pragma warning restore UNT0028 // Use non-allocating physics APIs

        foreach (Collider collider in enemies)
        {
            direction = collider.transform.position - player.transform.position;

            Physics.Raycast(player.transform.position, direction, out hit, Mathf.Infinity, ignoreItems);

            if (hit.collider != null &&
                GameObject.ReferenceEquals(hit.collider.gameObject, collider.gameObject) &&
                hit.distance < dist)
            {
                dist = hit.distance;
                closestEnemy = collider.gameObject;
            }
        }

        return closestEnemy;
    }
    public void ShootWand(GameObject enemy)
    {
        foreach (Projectile projectile in projectiles)
        {
            if (!projectile.actualProjectile.activeSelf)
            {
                projectile.actualProjectile.SetActive(true);
                projectile.target = enemy;
                projectile.actualProjectile.transform.position = player.transform.position;
                return;
            }
        }
        projectiles.Add(new Projectile(Instantiate(projectilePrefab, player.transform.position, player.transform.rotation), enemy, damage, projectileSpeed));
    }
    
    private void OnDrawGizmos()
    {
        Debug.DrawRay(player.transform.position, direction);
    }
    
}
class Projectile
{

    public GameObject target { get; set; }
    public GameObject actualProjectile { get; set; }
    public float damage { get; set; }
    public float speed { get; set; }

    public LayerMask enemyLayer;

    public Projectile(GameObject actualProjectile, GameObject target, float damage, float speed)
    {
        this.actualProjectile = actualProjectile;
        this.target = target;
        this.damage = damage;
        this.speed = speed;
        enemyLayer.value = 64;
    }

    public void FollowTarget()
    {
        try
        {
            actualProjectile.transform.position = Vector3.MoveTowards(actualProjectile.transform.position, target.transform.position, Time.deltaTime * speed);
            Collider[] enemies = Physics.OverlapSphere(actualProjectile.transform.position, 0.25f, enemyLayer);
            if (enemies.Length > 0)
            {
                foreach (Collider c in enemies)
                {
                    if (GameObject.ReferenceEquals(c.gameObject, target))
                    {
                        actualProjectile.SetActive(false);
                        c.gameObject.GetComponent<GoToPlayer>().takeDamage(damage);
                    }
                }
            }
        }
        catch
        {
            actualProjectile.SetActive(false);
        }
    }
}

