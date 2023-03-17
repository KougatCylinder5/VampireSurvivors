using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
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

    private List<Upgrade> upgrades;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("spawnEnemies", 0, 3f);
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        spawnSpawns();
        enemySpawns = GameObject.FindGameObjectsWithTag("EnemySpawns").ToList();
        targets = GameObject.FindGameObjectsWithTag("Collectables");
        foreach (GameObject t in targets)
        {
            pointers.Add(Instantiate(pointerImage, GameObject.Find("Canvas").transform.GetChild(0).transform, false));
        }
        upgrades = readUpgrades();
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

        switch ((Types)(reward.GetComponent<StoreReward>().upgradeInfo.type))
        {
            case Types.AttackDamage:

                break;
            case Types.Speed:

                break;
            case Types.AttackSpeed:
                
                break;
            case Types.Luck:

                break;
            case Types.MaxHealth:

                break;
            default:
                break;
        }

        rewardPanel.SetActive(false);
        Time.timeScale = 1;
    }
    public void PointToTarget()
    {
        for (int i = 0; i < targets.Length; i++)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(targets[i].transform.position);
            pos.z = 0;
            int cameraXMax = Camera.main.pixelWidth - 25;
            int cameraYMax = Camera.main.pixelHeight - 25;

            pointers[i].transform.position = new Vector3(Mathf.Clamp(pos.x, 25, cameraXMax), Mathf.Clamp(pos.y, 25, cameraYMax));

            pos.y -= cameraYMax / 2;
            pos.x -= cameraXMax / 2;

            pointers[i].transform.eulerAngles = new Vector3(0, 0, (float)(Math.Atan2(pos.y, pos.x) * 180 / Math.PI));
            pointers[i].transform.GetChild(0).eulerAngles = Vector3.zero;
            pointers[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().text =(Mathf.Round(pos.magnitude/10)-10).ToString();
            if (pos.magnitude < 100)
            {
                pointers[i].GetComponent<Image>().color = new Color(255, 255, 255, 0);
                pointers[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, 0);
            }
            else if (pos.magnitude < 250)
            {
                pointers[i].GetComponent<Image>().color = new Color(255, 255, 255, (pos.magnitude - 100) / 150);
                pointers[i].transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, (pos.magnitude - 100) / 150);
            }
        }

    }
    public void spawnSpawns()
    {
        for (int x = -450; x < 500; x += 50)
        {
            for (int z = -450; z < 500; z += 50)
            {
                Instantiate(enemySpawnPrefab, new Vector3(x, 15, z), new Quaternion());
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

        if (levelXP >= levelXPRequired)
        {
            levelNumber.text = "Level: " + level.ToString();
            level++;
            levelXPRequired += 15f;
            levelXP = 0;
            displayUpgrades();
        }
    }

    private void displayUpgrades()
    {
        Time.timeScale = 0f;
        rewardPanel.SetActive(true);

        HashSet<int> numbersPicked = new HashSet<int>();

        for(int i = 0; i < 3; i++)
        {

            GameObject buttonObject = rewardPanel.transform.GetChild(i).gameObject;
            Image image = buttonObject.GetComponent<Image>();
            int choice;
            do
            {
                choice = UnityEngine.Random.Range(0, upgrades.Count);
            } while (!numbersPicked.Add(choice));
            Sprite graphic = Resources.Load<Sprite>(upgrades[choice].pathToImage);
            image.sprite = graphic;
            buttonObject.GetComponent<StoreReward>().setRewardInfo(upgrades[choice]);
        }
    }

    private List<Upgrade> readUpgrades()
    {
        TextAsset json = Resources.Load("data/upgrades") as TextAsset;

        Upgrades listOfUpgrades = JsonUtility.FromJson<Upgrades>(json.text);

        List<Upgrade> list = new List<Upgrade>();

        foreach (Upgrade upgrade in listOfUpgrades.upgrades)
        {
            list.Add(upgrade);
        }

        return list;
    }
}
[System.Serializable]
public class Upgrade
{
    public string name;
    public string description;
    public string pathToImage;
    public int type;
    public override String ToString()
    {
        return name + " " + description + " " + type;
    }
}
[System.Serializable]
public class Upgrades
{
    public List<Upgrade> upgrades;
}

public enum Types
{
    AttackDamage,
    Speed,
    AttackSpeed,
    Luck,
    MaxHealth,
    HealthPlus
}