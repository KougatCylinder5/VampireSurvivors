using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<GameObject> enemies;
    public int level = 1;
    public int levelXP;
    public float levelXPRequired;
    public TextMeshProUGUI levelNumber;
    public TextMeshProUGUI timer;
    public int seconds;
    public Slider levelBar;
    public Slider healthBar;
    public GameObject rewardPanel;
    public GameObject pause;

    public static float volume = 0.5f;

    private List<GameObject> enemySpawns;
    public GameObject enemySpawnPrefab;

    private GameObject player;
    private PlayerController playerController;

    public List<GameObject> targets;
    private readonly List<GameObject> pointers = new();
    public GameObject pointerImage;
    public float pointerDistInvis;
    public float pointerDistDrop;


    private SortedList<int, Upgrade> upgrades = new();
    public List<GameObject> weapons;
    public float luck;

    public GameObject ScorePanel;
    public static bool end;
    // Start is called before the first frame update
    void Start()
    {
        end = false;
        StartCoroutine(nameof(spawnEnemies));
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
        StartCoroutine(nameof(updateTime));
        pause = rewardPanel.transform.parent.GetChild(8).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        levelBar.value = levelXP / levelXPRequired;
        healthBar.value = (float)(playerController.getHealth() / playerController.getMaxHealth());
        PointToTarget();
        pauseMenu(pause);
        if(targets.Count == 0)
        {
            StartCoroutine(waitForEnd());
        }
        if (playerController.dead)
        {
            ScorePanel.SetActive(true);
            int score = 0;
            for (int i = 0; i < level; i++)
            {
                score += 15 * i;
            }
            score += levelXP;
            TextMeshProUGUI[] text = ScorePanel.GetComponentsInChildren<TextMeshProUGUI>();
            text[0].text = "Score: " + score.ToString();
            text[1].text = "Time Survived: " + (seconds / 60).ToString("00") + ":" + (seconds % 60).ToString("00");
            if (end)
            {
                text[2].text = "You Survived To Escape the Night";
            }

        }
    }

    private IEnumerator updateTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            seconds++;
            timer.text = "Timer: " + seconds / 60 + ":" + (seconds % 60).ToString("00");
        }
    }

    public void selectReward(GameObject reward)
    {
        StoreReward store = reward.GetComponent<StoreReward>();

#pragma warning disable UNT0010 // Component instance creation
        dynamic comp = new Component();
#pragma warning restore UNT0010 // Component instance creation

        if (store.upgradeInfo.weapon != -1)
        {
            comp = (Weapons)(store.upgradeInfo.weapon) switch
            {
                Weapons.Whip => weapons[store.upgradeInfo.weapon].GetComponent<Whip>(),
                Weapons.Runetracer => weapons[store.upgradeInfo.weapon].GetComponent<Runetracer>(),
                Weapons.Wand => weapons[store.upgradeInfo.weapon].GetComponent<Wand>(),
                _ => throw new Exception("Called a weapon that doesn't exist"),
            };
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
                upgrades.RemoveAt(upgrades.IndexOfValue(store.upgradeInfo));
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
                throw new Exception("Called an upgrade that doesn't exist");
        }
        if(store.upgradeInfo.limit != -1)
        {
            try
            {
                upgrades.ElementAt(upgrades.IndexOfValue(store.upgradeInfo)).Value.limit--;
            }catch{ }
        }
        if(store.upgradeInfo.limit == 0)
        {
            limitUpgrade(store);
        }
        rewardPanel.SetActive(false);
        Time.timeScale = 1;

        player.GetComponent<Animator>().Play("Decider");
    }
    public void limitUpgrade(StoreReward store)
    {
        if(store.upgradeInfo.limit == -1)
        {
            return;
        }
        int pos = upgrades.IndexOfValue(store.upgradeInfo);
        for(int i = 0; i < upgrades.Count; i++)
        {
            if(i < pos)
            {
                continue;
            }
            if (i == pos)
            {
                upgrades.RemoveAt(pos);
            }
            upgrades.Add(upgrades.ElementAt(i).Key - store.upgradeInfo.chance, upgrades.ElementAt(i).Value);
            upgrades.RemoveAt(i+1);
        }
        
    }
    public void PointToTarget()
    {
        for (int i = 0; i < targets.Count; i++)
        {
            Vector3 pos = Camera.main.WorldToScreenPoint(targets[i].transform.position);
            pos.z = 0;
            int cameraXMax = Camera.main.pixelWidth - 25;
            int cameraYMax = Camera.main.pixelHeight - 25;

            pointers[i].transform.position = new Vector3(Mathf.Clamp(pos.x, 25, cameraXMax), Mathf.Clamp(pos.y, 25, cameraYMax-100));

            pos.y -= cameraYMax / 2;
            pos.x -= cameraXMax / 2;

            pointers[i].transform.eulerAngles = new Vector3(0, 0, (float)(Math.Atan2(pos.y, pos.x) * 180 / Math.PI));
            Transform pointerChild = pointers[i].transform.GetChild(0);
            pointerChild.eulerAngles = Vector3.zero;

            int dist = Mathf.RoundToInt((targets.ElementAt<GameObject>(i).transform.position - player.transform.position).magnitude);

            pointerChild.GetComponent<TextMeshProUGUI>().text = dist.ToString();
            if (dist < pointerDistDrop)
            {
                pointers[i].GetComponent<Image>().color = new Color(255, 255, 255, (dist/pointerDistDrop) - pointerDistInvis);
                pointerChild.GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, (dist / pointerDistDrop) - pointerDistInvis);
            }
            else if (dist > pointerDistDrop)
            {
                pointers[i].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                pointerChild.GetComponent<TextMeshProUGUI>().color = new Color(255, 255, 255, 255);
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

    private IEnumerator spawnEnemies()
    {
        yield return new WaitForSeconds(1);

        while (true)
        {
            if (GameObject.FindGameObjectsWithTag("Enemy").Length < 200)
            {
                List<GameObject> closeInOrder = enemySpawns.OrderBy(t => (t.transform.position - player.transform.position).sqrMagnitude).ToList<GameObject>();
                int startPos = 0;
                if (Vector3.Distance(closeInOrder[0].transform.position, player.transform.position) < 20)
                {
                    startPos++;
                }
                for (int i = startPos; i < startPos + level + 2; i++)
                {

                    

                    yield return new WaitForSeconds(2f);

                    NavMeshPath path = new();

                    Instantiate(enemies[0], closeInOrder[i].transform.position, closeInOrder[i].transform.rotation).GetComponent<NavMeshAgent>().CalculatePath(player.transform.position,path);
                    for (int j = 0; j < UnityEngine.Random.Range(4, 10); j++)
                    {
                        Instantiate(enemies[0], closeInOrder[i].transform.position, closeInOrder[i].transform.rotation).GetComponent<NavMeshAgent>().SetPath(path);
                    }

                }
            }
            yield return new WaitForSeconds(20);
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

    public void pauseMenu(GameObject button)
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            pause.SetActive(!pause.activeSelf);
            Time.timeScale = Time.timeScale < 0.1f ? 1 : 0;
            player.GetComponent<Animator>().Play("Decider");
        }
        if (button.name.Equals("Resume"))
        {
            pause.SetActive(false);
            Time.timeScale = 1;
            player.GetComponent<Animator>().Play("Decider");
        }
        if (button.name.Equals("Exit"))
        {
            SceneManager.LoadScene("Title Screen");
            Time.timeScale = 1;

        }
    }
    private void displayUpgrades()
    {
        Time.timeScale = 0f;
        rewardPanel.SetActive(true);

        HashSet<int> chosenKeys = new();

        for (int i = 0; i < 3; i++)
        {
            Upgrade value;
            int chance;
            do
            {
                chance = UnityEngine.Random.Range(0,upgrades.Keys.Max<int>());
                for (; !upgrades.TryGetValue(chance, out value); chance++){ if (chance > upgrades.Keys.Max<int>()) { throw new Exception(); } }

            } while (!chosenKeys.Add(chance));
            GameObject reward = rewardPanel.transform.GetChild(i).gameObject;
            reward.GetComponent<StoreReward>().setRewardInfo(value);
            reward.GetComponent<Image>().sprite = Resources.Load<Sprite>(value.pathToImage);
        }
    }
    public static IEnumerator waitForEnd()
    {
        yield return new WaitForSeconds(30);
        end = true;
    }
    private SortedList<int,Upgrade> readUpgrades()
    {
        TextAsset json = Resources.Load("data/upgrades") as TextAsset;

        Upgrades listOfUpgrades = JsonUtility.FromJson<Upgrades>(json.text);

        SortedList<int,Upgrade> list = new();
        foreach (Upgrade upgrade in listOfUpgrades.upgrades)
        {
            list.Add(upgrade.chance + list.Keys.LastOrDefault(),upgrade);
        }

        return list;
    }

    public void spawnWave(GameObject waveObject)
    {
        Vector3 pos = waveObject.transform.position;

        for(float i = -Mathf.PI; i < Mathf.PI-0.3f; i+= Mathf.PI/9)
        {
            Instantiate(enemies[0], new Vector3(Mathf.Sin(i),0, Mathf.Cos(i)) * 10 + pos,Quaternion.Euler(0,0,0)).GetComponent<NavMeshAgent>().SetDestination(player.transform.position);
        }
    }
    public void updateVolume(Slider slider)
    {
        volume = slider.value;
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
    public int limit;
    public override string ToString()
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
    Runetracer,
    Wand
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


