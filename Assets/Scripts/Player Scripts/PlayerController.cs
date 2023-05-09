using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{

    public float speed = 5;
    public float health = 100;
    public float maxHealth = 100;
    private CharacterController controller;
    private GameManager gameManager;
    [SerializeField]private TextMeshProUGUI perishText;

    public bool dead = false;
    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(GameManager.end);
        if (health <= 0 || dead || GameManager.end)
        {
            if (!dead )
            {
                foreach (Transform item in GameObject.Find("Canvas").transform)
                {
                    item.gameObject.SetActive(false);
                }
                Time.timeScale = 0;
                perishText.text = "You Perished";
            }
            dead = true;
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
        Vector3 movement = new(horizontalInput * Time.deltaTime * speed, -9.81f * Time.deltaTime, verticalInput * Time.deltaTime * speed);

        controller.Move(movement);
        movement.y = 0;
        if (movement.sqrMagnitude != 0)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.LookRotation(movement),0.25f);
        }

        
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
    public float getMaxHealth()
    {
        return maxHealth;
    }
}
