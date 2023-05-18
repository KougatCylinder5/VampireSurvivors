using
/* Unmerged change from project 'Assembly-CSharp.Player'
Before:
using Unity.VisualScripting;
using UnityEngine;
using System.Linq;
After:
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
*/
UnityEngine;

public class Runetracer : MonoBehaviour
{
    public float speed;
    public float damage;

    private GameObject target;
    private GameObject player;
    private Rigidbody rb;

    private float orbitPercent = 0f;
    public float radius = 5;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
        rb = player.GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        followPlayer();
    }

    private void followPlayer()
    {
        float x, z;
        orbitPercent += Time.deltaTime * speed;

        if (orbitPercent > 100)
        {
            orbitPercent = 0f;
        }
        float degrees = 360 * (orbitPercent / 100);

        x = radius * Mathf.Cos(degrees);
        z = radius * Mathf.Sin(degrees);
        transform.position = Vector3.Lerp(transform.position, new Vector3(x, 1, z) + player.transform.position, 0.5f);
    }

    public void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<GoToPlayer>().takeDamage(damage);
        }
    }
}
