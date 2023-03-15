using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class GameManager : MonoBehaviour
{
    public GameObject enemies;
    public int level = 1;
    public int levelXP;
    public float levelXPRequired;
    public TextMeshProUGUI levelNumber;
    public Slider levelBar;
    public Slider healthBar;
    public GameObject rewardPanel;

    private List<GameObject> enemySpawns;
    public GameObject enemySpawnPrefab;

    private GameObject player;
    private PlayerController playerController;

    private GameObject[] targets;
    private List<GameObject> pointers = new List<GameObject>();
    public GameObject pointerImage;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("spawnEnemies", 0, 3f);
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        spawnSpawns();
        enemySpawns = GameObject.FindGameObjectsWithTag("EnemySpawns").ToList();
        targets = GameObject.FindGameObjectsWithTag("Collectables");
        foreach(GameObject t in targets) {
            pointers.Add(Instantiate(pointerImage,GameObject.Find("Canvas").transform,false));
        }
    }

    // Update is called once per frame
    void Update()
    {
        levelBar.value = levelXP / levelXPRequired;
        healthBar.value = (float)(playerController.getHealth() / playerController.getMaxHealth());
        PointToTarget();
    }

    public void selectReward(GameObject reward)
    {
        Debug.Log(reward);
        rewardPanel.SetActive(false);
        Time.timeScale = 1;
    }
    public void PointToTarget()
    {
        for(int i = 0; i < targets.Length; i++)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(targets[i].transform.position);
            pos.z = 0;
            int cameraXMax = Camera.main.pixelWidth-25;
            int cameraYMax = Camera.main.pixelHeight- 25;
            
            
            pointers[i].transform.position = new Vector3(Mathf.Clamp(pos.x, 25, cameraXMax), Mathf.Clamp(pos.y, 25, cameraYMax));

            pos.y -= cameraYMax / 2;
            pos.x -= cameraXMax / 2;

            pointers[i].transform.eulerAngles = new Vector3(0,0, (float)(Math.Atan2(pos.y ,pos.x) * 180/Math.PI));
            if(pos.magnitude < 100)
            {
                pointers[i].GetComponent<Image>().color = new Color(255, 255, 255, 0);
            }
            else if(pos.magnitude < 250)
            {
                pointers[i].GetComponent<Image>().color = new Color(255,255,255,(pos.magnitude-100) / 150);
            }
        }

    }
    public void spawnSpawns()
    {
        for(int x = -450; x < 500; x += 50)
        {
            for (int z = -450; z < 500; z += 50)
            {
                Instantiate(enemySpawnPrefab, new Vector3(x, 0, z), new Quaternion());
            }
        }
    }

    private void spawnEnemies()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length < 100)
        {
            List<GameObject> closeInOrder = enemySpawns.OrderBy(t => (t.transform.position - player.transform.position).sqrMagnitude).ToList<GameObject>();
            int startPos = 0;
            if (Vector3.Distance(closeInOrder[0].transform.position, player.transform.position) < 20)
            {
                startPos++;
            }
            for (int i = startPos; i < startPos + level; i++)
            {
                Instantiate(enemies, closeInOrder[i].transform.position, closeInOrder[i].transform.rotation);

            }
        }
        
    }
    
    public void increaseXP()
    {
        levelXP++;
        
        if(levelXP >= levelXPRequired)
        {
            levelNumber.text = "Level: " + level.ToString();
            level++;
            levelXPRequired *= 1.5f;
            levelXP = 0;
            Time.timeScale = 0;
            rewardPanel.SetActive(true);
        }
    }
    

}
