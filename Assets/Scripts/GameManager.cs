using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    private List<GameObject> targets;
    private List<GameObject> pointers = new List<GameObject>();
    public GameObject pointerImage;
    public int pointerDistInvis;
    public int pointerDistDrop;


    private List<Upgrade> upgrades;
    public List<GameObject> weapons;
    public List<int> chance = new List<int>() {0};
    public float luck;


    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("spawnEnemies", 1, 20f);
        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        spawnSpawns();
        enemySpawns = GameObject.FindGameObjectsWithTag("EnemySpawns").ToList();
        targets = GameObject.FindGameObjectsWithTag("Collectables").ToList<GameObject>();
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
        StoreReward store = reward.GetComponent<StoreReward>();

        dynamic comp;

        if (store.upgradeInfo.weapon != -1)
        {
            switch ((Weapons)(store.upgradeInfo.weapon))
            {
                case Weapons.Whip:
                    comp = weapons[(int)Weapons.Whip].GetComponent<Whip>();
                    break;
                case Weapons.Runetracer:
                    comp = weapons[(int)Weapons.Runetracer].GetComponent<Runetracer>();
                    break;
                default:
                    throw new Exception();
            }
        }
        else
        {
            comp = new Component();
        }
        switch ((Types)(store.upgradeInfo.type))
        {
            case Types.Dmg:
                comp.damage += store.upgradeInfo.effect;
                weapons[store.upgradeInfo.weapon] = comp.gameObject;
                break;
            case Types.Speed:
                comp.speed += store.upgradeInfo.effect;
                weapons[store.upgradeInfo.weapon] = comp.gameObject;
                break;
            case Types.Unlock:
                weapons[store.upgradeInfo.weapon].SetActive(true);
                int pos = upgrades.IndexOf(store.upgradeInfo);
                chance.RemoveAt(pos);
                for(int i = 0; i < chance.Count; i++)
                {
                    pos--;
                    if (pos >= 0)
                    {
                        continue;
                    }
                    else
                    {
                        chance[i] -= store.upgradeInfo.chance;
                    }
                };
                upgrades.Remove(store.upgradeInfo);
                break;
            case Types.MxHlh:
                playerController.maxHealth += store.upgradeInfo.effect;
                playerController.health += store.upgradeInfo.effect;
                break;
            case Types.HlhBst:
                playerController.health += store.upgradeInfo.effect;
                break;
            case Types.Luck:
                luck += store.upgradeInfo.effect;
                break;

            default:
                break;
        }

        rewardPanel.SetActive(false);
        Time.timeScale = 1;
    }
    
    public void PointToTarget()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(targets[i].transform.position);
            pos.z = 0;
            int cameraXMax = Camera.main.pixelWidth - 25;
            int cameraYMax = Camera.main.pixelHeight - 25;

            pointers[i].transform.position = new Vector3(Mathf.Clamp(pos.x, 25, cameraXMax), Mathf.Clamp(pos.y, 25, cameraYMax));

            pos.y -= cameraYMax / 2;
            pos.x -= cameraXMax / 2;

            pointers[i].transform.eulerAngles = new Vector3(0, 0, (float)(Math.Atan2(pos.y, pos.x) * 180 / Math.PI));
            Transform pointerChild = pointers[i].transform.GetChild(0);
            pointerChild.eulerAngles = Vector3.zero;
            pointerChild.GetComponent<TextMeshProUGUI>().text = Mathf.RoundToInt((targets.ElementAt<GameObject>(i).transform.position - player.transform.position).magnitude).ToString();
            if (pos.magnitude < pointerDistDrop)
            {
                pointers[i].GetComponent<Image>().color = new Color(255, 255, 255, 0);
                pointerChild.GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, 0);
            }
            else if (pos.magnitude < pointerDistDrop + pointerDistInvis)
            {
                pointers[i].GetComponent<Image>().color = new Color(255, 255, 255, (pos.magnitude - pointerDistDrop) / pointerDistInvis);
                pointerChild.GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, (pos.magnitude - pointerDistDrop) / pointerDistInvis);
            }
        }
    }
    public void RemovePointer(GameObject targetToRemove)
    {
        pointers.RemoveAt(targets.IndexOf(targetToRemove));
        targets.Remove(targetToRemove);

    }

    public void spawnSpawns()
    {
        for (int x = -450; x < 500; x += 50)
        {
            for (int z = -450; z < 500; z += 50)
            {
                Instantiate(enemySpawnPrefab, new Vector3(x, 25, z), new Quaternion());
            }
        }
    }

    private void spawnEnemies()
    {
        if (GameObject.FindGameObjectsWithTag("Enemy").Length < 300)
        {
            List<GameObject> closeInOrder = enemySpawns.OrderBy(t => (t.transform.position - player.transform.position).sqrMagnitude).ToList<GameObject>();
            int startPos = 0;
            if (Vector3.Distance(closeInOrder[0].transform.position, player.transform.position) < 20)
            {
                startPos++;
            }
            for (int i = startPos; i < startPos + level + 2; i++)
            {
                for (int j = 0; j < UnityEngine.Random.Range(5, 11); j++)
                {
                    Instantiate(enemies, closeInOrder[i].transform.position, closeInOrder[i].transform.rotation);
                }

            }
        }

    }

    public void increaseXP()
    {
        levelXP++;

        if (levelXP >= levelXPRequired)
        {
            level++;
            levelNumber.text = "Level: " + level.ToString();
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

        for (int i = 0; i < 3; i++)
        {
            
            GameObject buttonObject = rewardPanel.transform.GetChild(i).gameObject;
            Image image = buttonObject.GetComponent<Image>();
            int j;
            do
            {
                j = 0;
                int chanceChosen = UnityEngine.Random.Range(0, chance.Last());
                for (; chance[j] < chanceChosen; j++) { }
            } while (!numbersPicked.Add(j));
            Sprite graphic = Resources.Load<Sprite>(upgrades[j].pathToImage);
            image.sprite = graphic;
            buttonObject.GetComponent<StoreReward>().setRewardInfo(upgrades[j]);        }
    }

    private List<Upgrade> readUpgrades()
    {
        TextAsset json = Resources.Load("data/upgrades") as TextAsset;

        Upgrades listOfUpgrades = JsonUtility.FromJson<Upgrades>(json.text);

        List<Upgrade> list = new List<Upgrade>();
        chance[0] = listOfUpgrades.upgrades[0].chance;
        foreach (Upgrade upgrade in listOfUpgrades.upgrades)
        {
            list.Add(upgrade);
            chance.Add(upgrade.chance + chance.Last<int>());
        }

        return list;
    }

    public void spawnWave(GameObject waveObject)
    {

    }
}

[System.Serializable]
public class Upgrade
{
    public string name;
    public string description;
    public string pathToImage;
    public int type;
    public float effect;
    public int weapon;
    public int chance;
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
public enum Weapons
{
    Whip,
    Runetracer
}
public enum Types
{
    Dmg,
    Speed,
    Unlock,
    Luck,
    MxHlh,
    HlhBst
}


