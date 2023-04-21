using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whip : MonoBehaviour
{
    public float speed = 3;
    public float damage = 10;
    private MeshRenderer[] meshes;
    public Animator whipOut;
    public bool hidden = true;
    public AudioSource whipCrack;
    public AudioClip whipCrackAudio;
    private HashSet<GameObject> enemies = new HashSet<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        meshes = gameObject.GetComponentsInChildren<MeshRenderer>();
        StartCoroutine("attack");
        whipCrackAudio = Resources.Load<AudioClip>("audio/whipcrack");

        whipCrack.clip = whipCrackAudio;

    }

    // Update is called once per frame
    void Update()
    {
        whipCrack.volume = GameManager.volume;
    }

    public IEnumerator attack()
    {
        while (true)
        {
            yield return new WaitForSeconds(speed);

            whipOut.SetTrigger("Attack");
            foreach (MeshRenderer mesh in meshes)
            {
                mesh.enabled = true;
            }
        }
    }
    public void attackEnemies()
    {
        try
        {
            foreach (GameObject obj in enemies)
            {
                if (obj != null)
                {
                    obj.GetComponent<GoToPlayer>().takeDamage(damage);
                }
            }
            whipCrack.Play();

        }
        catch (InvalidOperationException e)
        {
            Debug.LogException(e);
        }
        foreach (MeshRenderer mesh in meshes)
        {
            mesh.enabled = false;
        }
    }

    public void OnTriggerStay(Collider other)
    {

        if (other.gameObject.CompareTag("Enemy"))
        {
            enemies.Add(other.gameObject);
        }

    }

    public void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            enemies.Remove(other.gameObject);
        }
    }


}
