using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class Whip : MonoBehaviour
{
    public double attackSpeed = 1;
    public int damage = 10;
    private MeshRenderer[] meshes;
    public Animator whipOut;
    public bool hidden = true;

    private HashSet<GameObject> enemies = new HashSet<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        meshes = gameObject.GetComponentsInChildren<MeshRenderer>();
        StartCoroutine("attack");
        //collider = GetComponent<BoxCollider>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public IEnumerator attack() {
        while (true)
        {
            yield return new WaitForSeconds((float)attackSpeed);
            
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
