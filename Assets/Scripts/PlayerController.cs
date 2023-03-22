using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed = 5;
    public float health = 100;
    public float maxHealth = 100;
    private CharacterController controller;
    private GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {

        if (health <= 0)
        {

        }
        else
        {
            move();
        }

    }

    private void move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 movement = new Vector3(horizontalInput * Time.deltaTime * speed, -9.81f * Time.deltaTime, verticalInput * Time.deltaTime * speed);

        if (new Vector2(horizontalInput, verticalInput) != Vector2.zero)
        {
            transform.rotation = Quaternion.LookRotation(new(movement.x, 0, movement.z), transform.up);
        }

        controller.Move(movement);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("XP"))
        {
            gameManager.increaseXP();
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Health"))
        {
            health += 10;
            health = Mathf.Clamp(health, 0, maxHealth);
            Destroy(collision.gameObject);
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            health -= Time.deltaTime * collision.gameObject.GetComponent<GoToPlayer>().getDamage();
        }
    }
    public double getHealth()
    {
        return health;
    }
    public int getMaxHealth()
    {
        return maxHealth;
    }
}
